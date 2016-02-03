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

using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Game.Pay;
using ZyGames.Framework.Game.Service;

namespace ZyGames.Framework.Game.Contract.Action {
    /// <summary>
    /// Sdk91 pay action.
    /// </summary>
    public class Sdk91PayAction : BaseStruct {
        private string orderId = string.Empty;
        private int gameId = 0;
        private int serverId = 0;
        private string passportId = string.Empty;
        private string serverName = string.Empty;
        private string retailId = "0000";
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Contract.Action.Sdk91PayAction"/> class.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <param name="httpGet">Http get.</param>
        public Sdk91PayAction(object actionId, ActionGetter httpGet)
            : base(actionId, httpGet) {
        }
        /// <summary>
        /// 创建返回协议内容输出栈
        /// </summary>
        public override void BuildPacket() {
        }
        /// <summary>
        /// 接收用户请求的参数，并根据相应类进行检测
        /// </summary>
        /// <returns></returns>
        public override bool GetUrlElement() {
            TraceLog.ReleaseWriteFatal("url");
            if (actionGetter.GetString("OrderId", ref orderId) && 
                actionGetter.GetInt("GameId", ref gameId) && 
                actionGetter.GetInt("ServerId", ref serverId) && 
                actionGetter.GetString("ServerName", ref serverName) && 
                actionGetter.GetString("PassportId", ref passportId)) {
                actionGetter.GetString("RetailId", ref retailId);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 子类实现Action处理
        /// </summary>
        /// <returns></returns>
        public override bool TakeAction() {
            SaveLog(string.Format("91SKD充值>>Order:{0},Pid:{1},ServerName:{2}", orderId, passportId, serverName));
            PayManager.Get91PayInfo(gameId, serverId, passportId, serverName, orderId, retailId);
            return true;
        }
    }
}