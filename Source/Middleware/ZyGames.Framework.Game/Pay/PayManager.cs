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

THE SOFTWARE IS PROVIdED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/
using System;
using ZyGames.Framework.Common;
using ZyGames.Framework.Common.Log;

namespace ZyGames.Framework.Game.Pay {
    /// <summary>
    /// 
    /// </summary>
    public class PayManager {
        private static PayOperator _operator;
        static PayManager() {
            _operator = new PayOperator();
        }

        /// <summary>
        /// 
        /// </summary>
        public static PayOperator Operator { get { return _operator; } }

        /// <summary>
        /// 获取个人充值未下发全部信息
        /// </summary>
        /// <param name="game"></param>
        /// <param name="Server"></param>
        /// <param name="Account"></param>
        /// <returns></returns>
        public static OrderInfo[] GetPayment(int game, int Server, string Account) {
            OrderFormBLL ordef = new OrderFormBLL();
            return ordef.GetList(game, Server, Account);
        }

        /// <summary>
        /// Get91s the pay info.
        /// </summary>
        /// <param name="gameId">Game.</param>
        /// <param name="serverId">Server.</param>
        /// <param name="passportId">Account.</param>
        /// <param name="serverName">Service name.</param>
        /// <param name="orderNo">Order no.</param>
        /// <param name="retailId">Retail Id.</param>
        /// <param name="userId"></param>
        public static void Get91PayInfo(int gameId, int serverId, string passportId, string serverName, string orderNo, string retailId, string userId = "") {
            //增加游戏名称避免出现游戏名称为空的现象 panx 2012-11-26
            string gameName = string.Empty;
            ServerInfo serverinfo = GetServerData(gameId, serverId);
            if (serverinfo != null) {
                gameName = serverinfo.GameName;
            }

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.OrderNo = orderNo;
            orderInfo.MerchandiseName = string.Empty;
            orderInfo.Currency = "CNY";
            orderInfo.Amount = 0;
            orderInfo.PassportId = passportId;
            orderInfo.Expand = userId;
            orderInfo.RetailId = retailId;
            orderInfo.PayStatus = 1;
            orderInfo.GameId = gameId;
            orderInfo.ServerId = serverId;
            orderInfo.GameName = gameName;
            orderInfo.ServerName = serverName;
            orderInfo.GameCoins = 0;
            orderInfo.SendState = 1;
            orderInfo.PayType = orderInfo.RetailId;
            orderInfo.Signature = "123456";
            OrderFormBLL obll = new OrderFormBLL();
            obll.Add91Pay(orderInfo, false);
        }


        /// <summary>
        /// appstroe充值
        /// </summary>
        /// <param name="gameId">Game.</param>
        /// <param name="serverId">Server.</param>
        /// <param name="passportId">Account.</param>
        /// <param name="coins">Coins.</param>
        /// <param name="amount">Amount.</param>
        /// <param name="orderNo">Order no.</param>
        /// <param name="retailId">Retail I.</param>
        /// <param name="deviceId">Member mac.</param>
        /// <param name="payState"></param>
        /// <param name="userId"></param>
        public static void AppStorePay(int gameId, int serverId, string passportId, int coins, int amount, string orderNo, string retailId, string deviceId, bool payState = true, string userId = "") {
            try {
                string gameName = string.Empty;
                string serverName = string.Empty;
                ServerInfo serverinfo = GetServerData(gameId, serverId);
                if (serverinfo != null) {
                    gameName = serverinfo.GameName;
                    serverName = serverinfo.Name;
                }

                OrderInfo orderInfo = new OrderInfo();
                orderInfo.OrderNo = orderNo;
                orderInfo.MerchandiseName = gameName;
                orderInfo.Currency = "CNY";
                orderInfo.Amount = amount;
                orderInfo.PassportId = passportId;
                orderInfo.Expand = userId;
                orderInfo.RetailId = retailId;
                orderInfo.PayStatus = payState ? 2 : 3;
                orderInfo.GameId = gameId;
                orderInfo.ServerId = serverId;
                orderInfo.GameName = gameName;
                orderInfo.ServerName = serverName;
                orderInfo.GameCoins = coins;
                orderInfo.SendState = 1;
                orderInfo.PayType = "0004";
                orderInfo.Signature = "123456";
                orderInfo.DeviceId = deviceId;
                OrderFormBLL obll = new OrderFormBLL();
                obll.Add(orderInfo);
                TraceLog.ReleaseWrite("User:{0} AppStore充值{1}完成,order:{2}", passportId, amount, orderNo);
            } catch (Exception ex) {
                TraceLog.ReleaseWriteFatal("User:{0} AppStore充值{1}异常, order:{2}\r\nError:{3}", passportId, amount, orderNo, ex.ToString());
            }
        }

        private static ServerInfo GetServerData(int gameId, int serverId) {
            OrderFormBLL ordrBLL = new OrderFormBLL();
            return ordrBLL.GetServerData(gameId, serverId);
        }

        /// <summary>
        /// Abnormal the specified OrderNo.
        /// </summary>
        /// <param name="orderNo">Order No.</param>
        public static void Abnormal(string orderNo) {
            OrderFormBLL ordrBLL = new OrderFormBLL();
            ordrBLL.Updatestr(orderNo);
        }

        /// <summary>
        /// 补订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="gameId"></param>
        /// <param name="serverId"></param>
        /// <param name="passportId"></param>
        /// <returns></returns>
        public static bool ModifyOrder(string orderNo, int gameId, int serverId, string passportId) {
            OrderFormBLL obll = new OrderFormBLL();
            return obll.UpdateBy91(new OrderInfo() { OrderNo = orderNo, GameId = gameId, ServerId = serverId, PassportId = passportId }, false);
        }

        /// <summary>
        /// 更新订单支付成功状态
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        public static bool PaySuccess(string orderNo, OrderInfo orderInfo) {
            OrderFormBLL obll = new OrderFormBLL();
            return obll.PaySuccess(orderNo, orderInfo);
        }

        /// <summary>
        /// 触控android订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="amount"></param>
        /// <param name="passportId"></param>
        /// <param name="serverId"></param>
        /// <param name="gameId"></param>
        /// <param name="gameCoins"></param>
        /// <param name="deviceId"></param>
        /// <param name="retailId"></param>
        public static void AddOrderInfo(string orderNo, decimal amount, string passportId, int serverId, int gameId, int gameCoins, string deviceId, string retailId) {
            try {
                string GameName = string.Empty;
                string ServerName = string.Empty;
                ServerInfo serverinfo = GetServerData(gameId, serverId);
                if (serverinfo != null) {
                    GameName = serverinfo.GameName;
                    ServerName = serverinfo.Name;
                }

                OrderInfo orderInfo = new OrderInfo();
                orderInfo.OrderNo = orderNo;
                orderInfo.MerchandiseName = GameName;
                orderInfo.Currency = "CNY";
                orderInfo.Amount = amount;
                orderInfo.PassportId = passportId;
                orderInfo.RetailId = retailId;
                orderInfo.PayStatus = 1;
                orderInfo.GameId = gameId;
                orderInfo.ServerId = serverId;
                orderInfo.GameName = GameName;
                orderInfo.ServerName = ServerName;
                orderInfo.GameCoins = gameCoins;
                orderInfo.SendState = 1;
                orderInfo.PayType = "6002";
                orderInfo.Signature = "123456";
                orderInfo.DeviceId = deviceId;
                OrderFormBLL obll = new OrderFormBLL();
                obll.Add91Pay(orderInfo, false);
                TraceLog.ReleaseWrite("触控android充值完成");
            } catch (Exception ex) {
                TraceLog.ReleaseWriteFatal(ex.ToString());
            }
        }
        /// <summary>
        /// Adds the order.
        /// </summary>
        /// <returns><c>true</c>, if order was added, <c>false</c> otherwise.</returns>
        /// <param name="orderInfo">Order info.</param>
        public static bool AddOrder(OrderInfo orderInfo) {
            try {
                OrderFormBLL obll = new OrderFormBLL();
                obll.Add91Pay(orderInfo, false);
                return true;
            } catch (Exception ex) {
                TraceLog.ReleaseWriteFatal(ex.ToString());
                return false;
            }
        }
    }
}