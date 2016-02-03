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
using ZyGames.Framework.Common;
using ZyGames.Framework.Game.Configuration;
using ZyGames.Framework.Game.Sns._91sdk;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Common.Serialization;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// 91SDK 0001
    /// </summary>
    public class Login91sdk : AbstractLogin {
        private string retailId = string.Empty;
        private string retailUser = string.Empty;
        private string appId;
        private string appKey;
        private string url;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.Login91sdk"/> class.
        /// </summary>
        /// <param name="retailId">Retail id.</param>
        /// <param name="retailUser">Retail user id.</param>
        /// <param name="sessionId">Session id.</param>
        public Login91sdk(string retailId, string retailUser, string sessionId) {
            this.retailId = retailId;
            this.retailUser = retailUser;
            SessionId = sessionId;
            GameChannel gameChannel = ZyGameBaseConfigManager.GameSetting.GetChannelSetting(ChannelType.channel91);
            if (gameChannel != null) {
                url = gameChannel.Url;
                GameSdkSetting setting = gameChannel.GetSetting(retailId);
                if (setting != null) {
                    appId = setting.AppId;
                    appKey = setting.AppKey;
                } else {
                    TraceLog.ReleaseWrite("The sdkChannel section channel91:{0} is null.", retailId);
                }
            } else {
                TraceLog.ReleaseWrite("The sdkChannel 91 section is null.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckLogin() {
            int Act = 4;
            string Sign = string.Format("{0}{1}{2}{3}{4}", appId.ToNotNullString(), Act, retailUser.ToNotNullString(), SessionId.ToNotNullString(), appKey.ToNotNullString());
            Sign = HashToMD5Hex(Sign);

            string urlData = string.Format("{0}?AppId={1}&Act={2}&Uin={3}&SessionID={4}&Sign={5}",
                url.ToNotNullString(),
                appId.ToNotNullString(),
                Act,
                retailUser.ToNotNullString(),
                SessionId.ToNotNullString(),
                Sign.ToNotNullString()
            );

            string result = HttpPostManager.GetStringData(urlData);
            try {
                if (string.IsNullOrEmpty(result)) {
                    TraceLog.ReleaseWrite("91sdk login fail result:{0},request url:{1}", result, urlData);
                    return false;
                }
                SDKError sdk = JsonUtils.Deserialize<SDKError>(result);
                if (sdk.ErrorCode != "1") {
                    TraceLog.ReleaseWrite("91sdk login fail:{0},request url:{1}", sdk.ErrorDesc, urlData);
                    return false;
                }
                string[] arr = SnsManager.LoginByRetail(retailId, retailUser);
                UserId = arr[0];
                PassportId = arr[1];
                return true;
            } catch (Exception ex) {
                new BaseLog().SaveLog(ex);
            }
            return false;

            //return !string.IsNullOrEmpty(UserID) && UserID != "0";
        }

    }
    /// <summary>
    /// SDK error.
    /// </summary>
    public class SDKError {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Gets or sets the error desc.
        /// </summary>
        /// <value>The error desc.</value>
        public string ErrorDesc { get; set; }
    }
}