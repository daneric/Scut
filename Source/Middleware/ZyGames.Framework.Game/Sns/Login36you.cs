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
using System.Web;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Common.Security;
using ZyGames.Framework.Game.Runtime;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// 36you官网登录0000
    /// </summary>
    public class Login36you : AbstractLogin {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.Login36you"/> class.
        /// </summary>
        /// <param name="passportId">Passport I.</param>
        /// <param name="password">Password.</param>
        /// <param name="deviceId">Device I.</param>
        public Login36you(string passportId, string password, string deviceId) {
            PassportId = passportId;
            Password = password;
            DeviceId = deviceId;
        }
        /// <summary>
        /// 注册通行证
        /// </summary>
        /// <returns></returns>
        public override string GetRegPassport() {
            string[] userList = SnsManager.GetRegPassport(DeviceId, false);
            if (userList.Length == 2) {
                PassportId = userList[0];
                Password = userList[1];
            }
            return PassportId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckLogin() {
            //快速登录
            RegType regType;
            UserId = SnsManager.LoginByDevice(PassportId, Password, DeviceId, out regType).ToString();
            if (!string.IsNullOrEmpty(UserId) && UserId != "0") {
                UserType = (int)regType;
                if (string.IsNullOrEmpty(SessionId)) {
                    SessionId = GetSessionId();
                }
                return true;
            }
            //TraceLog.WriteError("LoginByDevice pid:{0},pwd:{1},device:{2},uid:{3}", PassportID, Password, DeviceID, UserID);
            return false;
        }

    }
}