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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ZyGames.Framework.Common.Log;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// 机峰0011
    /// </summary>
    public class LoginGFan : AbstractLogin {
        private string retailId;
        private string retailUser;
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.LoginGFan"/> class.
        /// </summary>
        /// <param name="retailId">Retail id.</param>
        /// <param name="retailUser">Retail user.</param>
        public LoginGFan(string retailId, string retailUser) {
            this.retailId = retailId ?? "0011";
            this.retailUser = retailUser.Equals("0") ? string.Empty : retailUser;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckLogin() {
            if (string.IsNullOrEmpty(retailUser)) {
                TraceLog.ReleaseWrite("The ChannelGFansdk  uid is null.");
                return false;
            }
            string[] arr = SnsManager.LoginByRetail(retailId, retailUser);
            UserId = arr[0];
            PassportId = arr[1];
            SessionId = GetSessionId();
            return true;
        }

    }
}