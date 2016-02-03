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
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Configuration;
using ZyGames.Framework.Game.Context;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// Login360_ v2.
    /// </summary>
    public class Login360V2 : AbstractLogin {
        private string retailId = string.Empty;
        private string retailUser = string.Empty;
        private string pid = string.Empty;
        private string appId = string.Empty;
        private string appKey = string.Empty;
        private string appSecret = string.Empty;
        private string code = string.Empty;
        private string url = string.Empty;
        private string tokenUrl = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.Login360V2"/> class.
        /// </summary>
        /// <param name="retailId">Retail I.</param>
        /// <param name="retailUser">Retail user.</param>
        /// <param name="pid">Pid.</param>
        /// <param name="code">Code.</param>
        public Login360V2(string retailId, string retailUser, string pid, string code) {
            this.retailId = retailId;
            this.pid = pid;
            this.retailUser = retailUser;
            this.code = code;
            GameChannel gameChannel = ZyGameBaseConfigManager.GameSetting.GetChannelSetting(ChannelType.channel360);
            if (gameChannel != null) {
                url = gameChannel.Url;
                tokenUrl = gameChannel.TokenUrl;
                GameSdkSetting setting = gameChannel.GetSetting(retailId);
                if (setting != null) {
                    appId = setting.AppId;
                    appKey = setting.AppKey;
                    appSecret = setting.AppSecret;
                } else {
                    TraceLog.ReleaseWrite("The sdkChannel section channel360:{0} is null.", retailId);
                }
            } else {
                TraceLog.ReleaseWrite("The sdkChannel 360 section is null.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckLogin() {
            string getAccessTokenUrl = string.Format("{0}?grant_type=authorization_code&code={1}&client_id={2}&client_secret={3}&redirect_uri=oob",
                tokenUrl, code, appKey, appSecret);
            string resultGetToken = HttpRequestManager.GetStringData(getAccessTokenUrl, "GET");
            if (string.IsNullOrEmpty(resultGetToken)) {
                return false;
            }
            var sdkGetToken = JsonUtils.Deserialize<SDK360GetTokenError>(resultGetToken);
            if (!string.IsNullOrEmpty(sdkGetToken.error_code)) {
                TraceLog.ReleaseWrite("360sdk login get token fail:{0},errorCode:{1},request url:{2}",
                    sdkGetToken.error, sdkGetToken.error_code, getAccessTokenUrl);
                return false;
            }
            AccessToken = sdkGetToken.access_token;
            RefreshToken = sdkGetToken.refresh_token;
            Scope = sdkGetToken.scope;
            ExpiresIn = Convert.ToInt32(sdkGetToken.expires_in);

            string urlData = string.Format("{0}?access_token={1}", url, AccessToken);
            string result = HttpRequestManager.GetStringData(urlData, "GET");
            try {
                if (!string.IsNullOrEmpty(result)) {
                    var sdk = JsonUtils.Deserialize<SDK360Error>(result);
                    if (!string.IsNullOrEmpty(sdk.error_code)) {
                        TraceLog.ReleaseWrite("360sdk login get user info fail:{0},errorCode:{1},request url:{2}", 
                            sdk.error, sdk.error_code, urlData);
                        return false;
                    }
                    string[] arr = SnsManager.LoginByRetail(retailId, sdk.id);
                    UserId = arr[0];
                    PassportId = arr[1];
                    RetailUserId = sdk.id;
                    SessionId = GetSessionId();
                    return true;
                }
            } catch (Exception ex) {
                new BaseLog().SaveLog(ex);
            }
            return false;
        }

        /// <summary>
        /// SDK360Error.
        /// </summary>
        public class SDK360Error {
            /// <summary>
            /// Gets or sets the error_code.
            /// </summary>
            /// <value>The error_code.</value>
            public string error_code { get; set; }
            /// <summary>
            /// Gets or sets the error.
            /// </summary>
            /// <value>The error.</value>
            public string error { get; set; }
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public string id { get; set; }
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string name { get; set; }
            /// <summary>
            /// Gets or sets the avatar.
            /// </summary>
            /// <value>The avatar.</value>
            public string avatar { get; set; }
            /// <summary>
            /// Gets or sets the sex.
            /// </summary>
            /// <value>The sex.</value>
            public string sex { get; set; }
            /// <summary>
            /// Gets or sets the area.
            /// </summary>
            /// <value>The area.</value>
            public string area { get; set; }
            /// <summary>
            /// Gets or sets the nick.
            /// </summary>
            /// <value>The nick.</value>
            public string nick { get; set; }
        }
        /// <summary>
        /// SDK360GetTokenError.
        /// </summary>
        public class SDK360GetTokenError {
            /// <summary>
            /// Gets or sets the error_code.
            /// </summary>
            /// <value>The error_code.</value>
            public string error_code { get; set; }
            /// <summary>
            /// Gets or sets the error.
            /// </summary>
            /// <value>The error.</value>
            public string error { get; set; }
            /// <summary>
            /// Gets or sets the access_token.
            /// </summary>
            /// <value>The access_token.</value>
            public string access_token { get; set; }
            /// <summary>
            /// Gets or sets the expires_in.
            /// </summary>
            /// <value>The expires_in.</value>
            public string expires_in { get; set; }
            /// <summary>
            /// Gets or sets the refresh_token.
            /// </summary>
            /// <value>The refresh_token.</value>
            public string refresh_token { get; set; }
            /// <summary>
            /// Gets or sets the scope.
            /// </summary>
            /// <value>The scope.</value>
            public string scope { get; set; }
        }
    }
}