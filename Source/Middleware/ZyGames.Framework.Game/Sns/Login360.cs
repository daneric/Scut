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
using System.Web;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// Login360.
    /// </summary>
    public class Login360 : AbstractLogin {
        private string retailId = string.Empty;
        private string pid = string.Empty;
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.Login360"/> class.
        /// </summary>
        /// <param name="retailId">Retail id.</param>
        /// <param name="pid">Pid.</param>
        public Login360(string retailId, string pid) {
            this.retailId = retailId;
            this.pid = pid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckLogin() {
            string[] arr = SnsManager.LoginByRetail(retailId, pid);
            UserId = arr[0];
            PassportId = arr[1];
            SessionId = GetSessionId();
            return true;
        }

    }
}