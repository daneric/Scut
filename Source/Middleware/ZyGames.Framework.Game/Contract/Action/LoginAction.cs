﻿/****************************************************************************
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
using ZyGames.Framework.Common;
using ZyGames.Framework.Common.Timing;
using ZyGames.Framework.Game.Context;
using ZyGames.Framework.Game.Lang;
using ZyGames.Framework.Game.Runtime;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Sns;
using ZyGames.Framework.Game.Sns.Service;

namespace ZyGames.Framework.Game.Contract.Action {
    /// <summary>
    /// Login action.
    /// </summary>
    public abstract class LoginAction : BaseStruct {
        /// <summary>
        /// The type of the mobile.
        /// </summary>
        protected MobileType mobileType;
        /// <summary>
        /// The passport id.
        /// </summary>
        protected string passportId = string.Empty;
        /// <summary>
        /// The password.
        /// </summary>
        protected string password = string.Empty;
        /// <summary>
        /// The device id.
        /// </summary>
        protected string deviceId = string.Empty;
        /// <summary>
        /// The sex.
        /// </summary>
        protected byte sex;
        /// <summary>
        /// The name of the nick.
        /// </summary>
        protected string nickName = string.Empty;
        /// <summary>
        /// The avatar id.
        /// </summary>
        protected string avatarId = string.Empty;
        /// <summary>
        /// The screen x.
        /// </summary>
        protected Int16 screenX;
        /// <summary>
        /// The screen y.
        /// </summary>
        protected Int16 screenY;
        /// <summary>
        /// The retail id.
        /// </summary>
        protected string retailId = string.Empty;
        /// <summary>
        /// The type of the user.
        /// </summary>
        protected int userType;
        /// <summary>
        /// The server id.
        /// </summary>
        protected int serverId;
        /// <summary>
        /// The type of the game.
        /// </summary>
        protected int gameId;
        /// <summary>
        /// Gets or sets the guide id.
        /// </summary>
        /// <value>The guide id.</value>
        protected int guideId { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Contract.Action.LoginAction"/> class.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <param name="httpGet">Http get.</param>
        protected LoginAction(object actionId, ActionGetter httpGet)
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
        }
        /// <summary>
        /// 接收用户请求的参数，并根据相应类进行检测
        /// </summary>
        /// <returns></returns>
        public override bool GetUrlElement() {
            if (actionGetter.GetEnum("MobileType", ref mobileType) &&
                actionGetter.GetString("PassportId", ref passportId) &&
                actionGetter.GetString("Password", ref password) &&
                actionGetter.GetString("RetailId", ref retailId)) {
                actionGetter.GetInt("GameId", ref gameId);
                actionGetter.GetInt("ServerId", ref serverId);
                actionGetter.GetString("DeviceId", ref deviceId);
                actionGetter.GetByte("Sex", ref sex);
                actionGetter.GetString("NickName", ref nickName);
                actionGetter.GetString("AvatarId", ref avatarId);
                actionGetter.GetWord("ScreenX", ref screenX);
                actionGetter.GetWord("ScreenY", ref screenY);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckAction() {
            if (!GameEnvironment.IsRunning) {
                ErrorCode = Language.Instance.ErrorCode;
                ErrorInfo = Language.Instance.ServerLoading;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual ILogin CreateLogin() {
            return LoginProxy.GetLogin(actionGetter, retailId);
        }

        /// <summary>
        /// 子类实现Action处理
        /// </summary>
        /// <returns></returns>
        public override bool TakeAction() {
            ILogin login = CreateLogin();
            login.Password = DecodePassword(login.Password);
            //todo: login test
            var watch = RunTimeWatch.StartNew("Request login server");
            try {
                sessionId = string.Empty;
                if (login.CheckLogin()) {
                    watch.Check("GetResponse");

                    Current.SetExpired();
                    Current = GameSession.CreateNew(Guid.NewGuid());
                    sessionId = Current.SessionId; //create new
                    passportId = login.PassportId;
                    userType = login.UserType;
                    SetParameter(login);
                    int userId = login.UserId.ToInt();
                    IUser user;
                    if (!IsError && DoSuccess(userId, out user)) {
                        watch.Check("DoSuccess");
                        var session = GameSession.Get(sessionId);
                        if (session != null) {
                            //user is null in create role.
                            session.Bind(user ?? new SessionUser() { PassportId = passportId, UserId = userId });
                            return true;
                        }
                    }
                } else {
                    DoLoginFail(login);
                }
            } catch (HandlerException error) {
                ErrorCode = (int)error.StateCode;
                ErrorInfo = error.Message;
            } finally {
                watch.Flush(true, 100);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void DoLoginFail(ILogin login) {
            ErrorCode = Language.Instance.ErrorCode;
            ErrorInfo = Language.Instance.PasswordError;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="login">Login.</param>
        protected virtual void SetParameter(ILogin login) { }

        /// <summary>
        /// 是否此请求忽略UID参数
        /// </summary>
        /// <returns></returns>
        protected override bool IsIgnoreUid() {
            return true;
        }

        /// <summary>
        /// Dos the success.
        /// </summary>
        /// <returns><c>true</c>, if success was done, <c>false</c> otherwise.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="user"></param>
        protected abstract bool DoSuccess(int userId, out IUser user);
    }
}