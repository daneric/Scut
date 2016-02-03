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

using ZyGames.Framework.Common;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Sns;

namespace ZyGames.Framework.Game.Contract.Action {
    /// <summary>
    /// 提供扩展渠道登录
    /// </summary>
    public abstract class LoginExtendAction : LoginAction {
        /// <summary>
        /// The refresh token.
        /// </summary>
        protected string refreshToken;
        /// <summary>
        /// The scope.
        /// </summary>
        protected string scope;
        /// <summary>
        /// The partner user id.
        /// </summary>
        protected string retailUserId;
        /// <summary>
        /// The access token.
        /// </summary>
        protected string accessToken;
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Contract.Action.LoginExtendAction"/> class.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <param name="httpGet">Http get.</param>
        protected LoginExtendAction(object actionId, ActionGetter httpGet)
            : base(actionId, httpGet) {
        }
        /// <summary>
        /// 创建返回协议内容输出栈
        /// </summary>
        public override void BuildPacket() {
            PushIntoStack(Current.SessionId);
            PushIntoStack(Current.UserId.ToNotNullString());
            PushIntoStack(userType);
            PushIntoStack(MathUtils.Now.ToString("yyyy-MM-dd HH:mm"));
            PushIntoStack(guideId);
            PushIntoStack(passportId);
            PushIntoStack(accessToken);
            PushIntoStack(refreshToken);
            PushIntoStack(retailUserId);
            PushIntoStack(scope);
        }
        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="login">Login.</param>
        protected override void SetParameter(ILogin login) {
            AbstractLogin baseLogin = login as AbstractLogin;
            if (baseLogin != null) {
                refreshToken = baseLogin.RefreshToken;
                retailUserId = baseLogin.RetailUserId;
                scope = baseLogin.Scope;
                accessToken = baseLogin.AccessToken;
            }
        }
    }
}