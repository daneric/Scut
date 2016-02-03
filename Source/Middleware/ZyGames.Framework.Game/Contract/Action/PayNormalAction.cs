/****************************************************************************
Copyright (c) 2013-2015 scutgame.com

http://www.scutgame.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/
using System;
using System.Text;
using System.Xml;
using ZyGames.Framework.Common;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Game.Configuration;
using ZyGames.Framework.Game.Lang;
using ZyGames.Framework.Game.Pay;
using ZyGames.Framework.Game.Service;

namespace ZyGames.Framework.Game.Contract.Action {
    /// <summary>
    /// 充值通用接口
    /// </summary>
    public class PayNormalAction : BaseStruct {
        /// <summary>
        /// The amount.
        /// </summary>
        protected int amount;
        /// <summary>
        /// The game Id.
        /// </summary>
        protected int gameId;
        /// <summary>
        /// The name of the game.
        /// </summary>
        protected string gameName;
        /// <summary>
        /// The server Id.
        /// </summary>
        protected int serverId;
        /// <summary>
        /// The name of the server.
        /// </summary>
        protected string serverName;
        /// <summary>
        /// The order no.
        /// </summary>
        protected string orderNo;
        /// <summary>
        /// The passport Id.
        /// </summary>
        protected string passportId;
        /// <summary>
        /// The currency.
        /// </summary>
        protected string currency;
        /// <summary>
        /// The retail Id.
        /// </summary>
        protected string retailId;
        /// <summary>
        /// The device Id.
        /// </summary>
        protected string deviceId;
        /// <summary>
        /// The product Id.
        /// </summary>
        protected string productId;
        /// <summary>
        /// The type of the pay.
        /// </summary>
        protected string payType;
        /// <summary>
        /// The pay status.
        /// </summary>
        protected int payStatus;
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Contract.Action.PayNormalAction"/> class.
        /// </summary>
        /// <param name="actionId">A action identifier.</param>
        /// <param name="httpGet">Http get.</param>
        public PayNormalAction(object actionId, ActionGetter httpGet)
            : base(actionId, httpGet) {
        }
        /// <summary>
        /// 创建返回协议内容输出栈
        /// </summary>
        public override void BuildPacket() {
            PushIntoStack(payStatus);
        }
        /// <summary>
        /// 接收用户请求的参数，并根据相应类进行检测
        /// </summary>
        /// <returns></returns>
        public override bool GetUrlElement() {
            if (actionGetter.GetInt("GameId", ref gameId) && 
                actionGetter.GetInt("ServerId", ref serverId) &&
                actionGetter.GetString("ServerName", ref serverName) &&
                actionGetter.GetString("OrderNo", ref orderNo) &&
                actionGetter.GetString("PassportId", ref passportId) &&
                actionGetter.GetString("PayType", ref payType)) {
                if (!actionGetter.GetString("Currency", ref currency)) {
                    currency = "CNY";
                }
                if (!actionGetter.GetString("RetailId", ref retailId)) {
                    retailId = payType;
                }
                actionGetter.GetString("DeviceId", ref deviceId);
                actionGetter.GetString("ProductId", ref productId);
                actionGetter.GetInt("Amount", ref amount);
                actionGetter.GetString("GameName", ref gameName);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 子类实现Action处理
        /// </summary>
        /// <returns></returns>
        public override bool TakeAction() {
            OrderInfo orderInfo = new OrderInfo();
            orderInfo.OrderNo = orderNo;
            orderInfo.MerchandiseName = productId;
            orderInfo.Currency = currency;
            orderInfo.Amount = amount;
            orderInfo.PassportId = passportId;
            orderInfo.RetailId = retailId;
            orderInfo.PayStatus = 1;
            orderInfo.GameId = gameId;
            orderInfo.ServerId = serverId;
            orderInfo.GameName = gameName;
            orderInfo.ServerName = serverName;
            orderInfo.GameCoins = (amount * 10).ToInt();
            orderInfo.SendState = 1;
            orderInfo.PayType = payType;
            orderInfo.Signature = "000000";
            orderInfo.DeviceId = deviceId;

            if (PayManager.AddOrder(orderInfo)) {
                DoSuccess(orderInfo);
                payStatus = orderInfo.PayStatus;
                TraceLog.ReleaseWriteFatal(string.Format("PayNormal Order:{0},pid:{1} successfully!", orderNo, passportId));
                return true;
            }
            ErrorCode = Language.Instance.ErrorCode;
            ErrorInfo = Language.Instance.AppStorePayError;
            TraceLog.ReleaseWriteFatal(string.Format("PayNormal Order:{0},pid:{1} faild!", orderNo, passportId));
            return false;
        }

        private void DoSuccess(OrderInfo orderInfo) {
            //移动MM SDK
            Check10086Payment(orderInfo);
        }

        private void Check10086Payment(OrderInfo orderInfo) {
            try {
                string url = "http://ospd.mmarket.com:8089/trust";
                string appId = "";
                string version = "1.0.0";
                int orderType = 1;
                GameChannel gameChannel = ZyGameBaseConfigManager.GameSetting.GetChannelSetting(ChannelType.channel10086);
                if (gameChannel != null) {
                    url = gameChannel.Url;
                    version = gameChannel.Version;
                    orderType = gameChannel.CType.ToInt();
                    GameSdkSetting setting = gameChannel.GetSetting(orderInfo.PayType);
                    if (setting != null) {
                        appId = setting.AppId;
                    } else {
                        return;
                    }
                }
                StringBuilder paramData = new StringBuilder();
                paramData.Append("<?xml version=\"1.0\"?>");
                paramData.AppendFormat("<Trusted2ServQueryReq>");
                paramData.AppendFormat("<MsgType>{0}</MsgType>", "Trusted2ServQueryReq");
                paramData.AppendFormat("<Version>{0}</Version>", version);
                paramData.AppendFormat("<AppID>{0}</AppID>", appId);
                paramData.AppendFormat("<OrderID>{0}</OrderID>", orderInfo.OrderNo);
                paramData.AppendFormat("<OrderType>{0}</OrderType>", orderType);
                paramData.AppendFormat("</Trusted2ServQueryReq>");

                var stream = HttpUtils.Post(url, paramData.ToString(), Encoding.UTF8, HttpUtils.XmlContentType);
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                TraceLog.ReleaseWriteFatal("10068 order:{0} response:{1}", orderInfo.OrderNo, doc.InnerXml);
                var returnCode = doc.SelectSingleNode("Trusted2ServQueryResp/ReturnCode");
                if (returnCode != null && !string.IsNullOrEmpty(returnCode.InnerText)) {
                    int code = returnCode.InnerText.ToInt();
                    if (code == 0) {
                        string orderNo = "";
                        var orderIDNode = doc.SelectSingleNode("Trusted2ServQueryResp/OrderID");
                        if (orderIDNode != null) {
                            orderNo = orderIDNode.InnerText;
                        }
                        var priceNode = doc.SelectSingleNode("Trusted2ServQueryResp/TotalPrice");
                        if (priceNode != null) {
                            decimal amount = priceNode.InnerText.ToDecimal();
                            orderInfo.Amount = amount;
                            orderInfo.GameCoins = (int)amount * 10;
                        }
                        PayManager.PaySuccess(orderNo, orderInfo);
                    }
                    TraceLog.ReleaseWriteFatal("10086 payment order:{0} fail code:{1}", orderInfo.OrderNo, code);
                }
            } catch (Exception ex) {
                TraceLog.WriteError("10086 payment error:", ex);
            }
        }
    }
}