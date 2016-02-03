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
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ZyGames.Framework.Common;
using ZyGames.Framework.Data;
using ZyGames.Framework.Data.Sql;

namespace ZyGames.Framework.Game.Pay {
    /// <summary>
    /// Order form BL.
    /// </summary>
    public class OrderFormBLL {
        /// <summary>
        /// Add the specified model.
        /// </summary>
        /// <param name="model">Model.</param>
        public bool Add(OrderInfo model) {
            model.CreateDate = MathUtils.Now;
            var command = ConfigManger.Provider.CreateCommandStruct("OrderInfo", CommandMode.Insert);
            command.AddParameter("OrderNo", model.OrderNo);
            command.AddParameter("MerchandiseName", model.MerchandiseName);
            command.AddParameter("PayType", model.PayType);
            command.AddParameter("Amount", model.Amount);
            command.AddParameter("Currency", model.Currency);
            command.AddParameter("Expand", model.Expand);
            command.AddParameter("SerialNumber", model.SerialNumber);
            command.AddParameter("PassportId", model.PassportId);
            command.AddParameter("ServerId", model.ServerId);
            command.AddParameter("GameId", model.GameId);
            command.AddParameter("gameName", model.GameName);
            command.AddParameter("ServerName", model.ServerName);
            command.AddParameter("PayStatus", model.PayStatus);
            command.AddParameter("Signature", model.Signature);
            command.AddParameter("Remarks", model.Remarks);
            command.AddParameter("GameCoins", model.GameCoins);
            command.AddParameter("SendState", model.SendState);
            command.AddParameter("RetailId", model.RetailId);
            command.AddParameter("DeviceId", model.DeviceId == null ? string.Empty : model.DeviceId);
            if (model.SendDate > DateTime.MinValue) {
                command.AddParameter("SendDate", model.SendDate);
            }
            command.AddParameter("CreateDate", model.CreateDate);
            command.Parser();

            return ConfigManger.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(OrderInfo model) {
            var command = ConfigManger.Provider.CreateCommandStruct("OrderInfo", CommandMode.Modify);
            command.AddParameter("SerialNumber", model.SerialNumber);
            command.AddParameter("PayStatus", model.PayStatus);
            command.AddParameter("@Signature", model.Signature);
            command.Filter = ConfigManger.Provider.CreateCommandFilter();
            command.Filter.Condition = command.Filter.FormatExpression("OrderNo");
            command.Filter.AddParam("OrderNo", model.OrderNo);
            command.Parser();

            return ConfigManger.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        /// <summary>
        /// 获取游戏币.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="gameId">Game.</param>
        /// <param name="serverId">Server.</param>
        /// <param name="pId">Account.</param>
        public OrderInfo[] GetList(int gameId, int serverId, string pId) {
            var command = ConfigManger.Provider.CreateCommandStruct("OrderInfo", CommandMode.Inquiry);
            command.OrderBy = "Id ASC";
            command.Columns = "OrderNo,MerchandiseName,PayType,Amount,Currency,Expand,SerialNumber,PassportId,ServerId,GameId,gameName,ServerName,PayStatus,Signature,Remarks,GameCoins,SendState,CreateDate,SendDate,RetailId,DeviceId";
            command.Filter = ConfigManger.Provider.CreateCommandFilter();
            command.Filter.Condition = string.Format("{0} AND {1} AND {2} AND {3} AND {4}",
                command.Filter.FormatExpression("GameId"),
                command.Filter.FormatExpression("ServerId"),
                command.Filter.FormatExpression("PassportId"),
                command.Filter.FormatExpression("SendState"),
                command.Filter.FormatExpression("PayStatus")
                );
            command.Filter.AddParam("GameId", gameId);
            command.Filter.AddParam("ServerId", serverId);
            command.Filter.AddParam("PassportId", pId);
            command.Filter.AddParam("SendState", 1);
            command.Filter.AddParam("PayStatus", 2);
            command.Parser();

            using (var reader = ConfigManger.Provider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters)) {
                List<OrderInfo> olist = new List<OrderInfo>();
                while (reader.Read()) {
                    OrderInfo ordermode = SetOrderInfo(reader);
                    olist.Add(ordermode);
                }
                return olist.ToArray();
            }
        }

        /// <summary>
        /// Determines whether this instance is exists the specified OrderNo.
        /// </summary>
        /// <returns><c>true</c> if this instance is exists the specified OrderNo; otherwise, <c>false</c>.</returns>
        /// <param name="orderNo">Order no.</param>
        public bool IsExists(string orderNo) {
            var command = ConfigManger.Provider.CreateCommandStruct("OrderInfo", CommandMode.Inquiry, "OrderNo");
            command.OrderBy = "Id ASC";
            command.Top = 1;
            command.Filter = ConfigManger.Provider.CreateCommandFilter();
            command.Filter.Condition = command.Filter.FormatExpression("OrderNo");
            command.Filter.AddParam("OrderNo", orderNo);
            command.Parser();
            return ConfigManger.Provider.ExecuteScalar(CommandType.Text, command.Sql, command.Parameters) != null;
        }

        /// <summary>
        /// Updates the by91.
        /// </summary>
        /// <returns><c>true</c>, if by91 was updated, <c>false</c> otherwise.</returns>
        /// <param name="model">Model.</param>
        /// <param name="callback">If set to <c>true</c> callback.</param>
        public bool UpdateBy91(OrderInfo model, bool callback) {
            var command = ConfigManger.Provider.CreateCommandStruct("OrderInfo", CommandMode.Modify);
            if (callback) {
                command.AddParameter("MerchandiseName", model.MerchandiseName);
                command.AddParameter("PayType", model.PayType);
                command.AddParameter("Amount", model.Amount);
                command.AddParameter("SendState", model.SendState);
                command.AddParameter("PayStatus", model.PayStatus);
                command.AddParameter("GameCoins", model.GameCoins);
                command.AddParameter("Signature", model.Signature);
            } else {
                command.AddParameter("ServerId", model.ServerId);
                command.AddParameter("PassportId", model.PassportId);
                if (!string.IsNullOrEmpty(model.Expand))
                    command.AddParameter("Expand", model.Expand);
                command.AddParameter("GameId", model.GameId);
                command.AddParameter("RetailId", model.RetailId);//20

                if (!string.IsNullOrEmpty(model.ServerName)) {
                    command.AddParameter("ServerName", model.ServerName);
                }
                if (!string.IsNullOrEmpty(model.GameName)) {
                    command.AddParameter("gameName", model.GameName);
                }
            }
            command.Filter = ConfigManger.Provider.CreateCommandFilter();
            command.Filter.Condition = command.Filter.FormatExpression("OrderNo");
            command.Filter.AddParam("OrderNo", model.OrderNo);
            command.Parser();
            return ConfigManger.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;

        }

        /// <summary>
        /// Add91s the pay.
        /// </summary>
        /// <returns><c>true</c>, if pay was add91ed, <c>false</c> otherwise.</returns>
        /// <param name="order">Order.</param>
        /// <param name="callback">If set to <c>true</c> callback.</param>
        public bool Add91Pay(OrderInfo order, bool callback) {
            if (!IsExists(order.OrderNo)) {
                return Add(order);
            } else {
                return UpdateBy91(order, callback);
            }
        }


        private OrderInfo SetOrderInfo(IDataReader reader) {
            OrderInfo ordermode = new OrderInfo();
            ordermode.OrderNo = reader["OrderNo"].ToNotNullString();
            ordermode.MerchandiseName = reader["MerchandiseName"].ToNotNullString();
            ordermode.PayType = reader["PayType"].ToNotNullString();
            ordermode.Amount = reader["Amount"].ToDecimal();
            ordermode.Currency = reader["Currency"].ToNotNullString();
            ordermode.Expand = reader["Expand"].ToNotNullString();
            ordermode.SerialNumber = reader["SerialNumber"].ToNotNullString();
            ordermode.PassportId = reader["PassportId"].ToNotNullString();
            ordermode.ServerId = reader["ServerId"].ToInt();
            ordermode.GameId = reader["GameId"].ToInt();
            ordermode.GameName = reader["gameName"].ToNotNullString();
            ordermode.ServerName = reader["ServerName"].ToNotNullString();
            ordermode.PayStatus = reader["PayStatus"].ToInt();
            ordermode.Signature = reader["Signature"].ToNotNullString();
            ordermode.Remarks = reader["Remarks"].ToNotNullString();
            ordermode.GameCoins = reader["GameCoins"].ToInt();
            ordermode.SendState = reader["SendState"].ToInt();
            ordermode.CreateDate = reader["CreateDate"].ToDateTime();
            ordermode.SendDate = reader["SendDate"].ToDateTime();
            ordermode.RetailId = reader["RetailId"].ToNotNullString();
            ordermode.DeviceId = reader["DeviceId"].ToNotNullString();
            return ordermode;
        }


        /// <summary>
        /// Updatestr the specified OrderNo.
        /// </summary>
        /// <param name="OrderNo">Order no.</param>
        public bool Updatestr(string OrderNo) {
            var command = ConfigManger.Provider.CreateCommandStruct("OrderInfo", CommandMode.Modify);
            command.AddParameter("SendState", 2);
            command.AddParameter("SendDate", DateTime.Now);
            command.Filter = ConfigManger.Provider.CreateCommandFilter();
            command.Filter.Condition = command.Filter.FormatExpression("OrderNo");
            command.Filter.AddParam("OrderNo", OrderNo);
            command.Parser();

            return ConfigManger.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }

        /// <summary>
        /// 支付取游戏名改为直接查库-wuzf
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public ServerInfo GetServerData(int gameId, int serverId) {
            var command = ConfigManger.Provider.CreateCommandStruct("ServerInfo", CommandMode.Inquiry);
            command.Columns = "Id,GameId,(SELECT GameName FROM GameInfo WHERE GameInfo.GameId=ServerInfo.GameId) GameName,ServerName";
            command.OrderBy = "GameId ASC,Id ASC";
            command.Top = 1;
            command.Filter = ConfigManger.Provider.CreateCommandFilter();
            command.Filter.Condition = string.Format("{0} AND {1}",
                    command.Filter.FormatExpression("GameId"),
                    command.Filter.FormatExpression("Id")
                );
            command.Filter.AddParam("GameId", gameId);
            command.Filter.AddParam("Id", serverId);
            command.Parser();

            using (var reader = ConfigManger.Provider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters)) {
                var serverInfo = new ServerInfo {
                    GameId = gameId,
                    Id = serverId,
                    Name = string.Empty,
                    GameName = string.Empty
                };
                if (reader.Read()) {
                    serverInfo.GameName = reader["GameName"].ToNotNullString();
                    serverInfo.Name = reader["ServerName"].ToNotNullString();
                }
                return serverInfo;
            }
        }

        internal bool PaySuccess(string orderNo, OrderInfo orderInfo) {
            var command = ConfigManger.Provider.CreateCommandStruct("OrderInfo", CommandMode.Modify);
            orderInfo.PayStatus = 2;
            command.AddParameter("PayStatus", orderInfo.PayStatus);
            if (!string.IsNullOrEmpty(orderInfo.PayType)) {
                command.AddParameter("PayType", orderInfo.PayType);
            }
            if (orderInfo.Amount > 0 && orderInfo.GameCoins > 0) {
                command.AddParameter("Amount", orderInfo.Amount);
                command.AddParameter("GameCoins", orderInfo.GameCoins);
            }
            command.Filter = ConfigManger.Provider.CreateCommandFilter();
            command.Filter.Condition = command.Filter.FormatExpression("OrderNo");
            command.Filter.AddParam("OrderNo", orderNo);
            command.Parser();

            return ConfigManger.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters) > 0;
        }
    }
}