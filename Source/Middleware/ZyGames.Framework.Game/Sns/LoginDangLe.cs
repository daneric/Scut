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
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Common.Security;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Configuration;
using ZyGames.Framework.Game.Runtime;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// 当乐0037
    /// </summary>
    public class LoginDangLe : AbstractLogin {
        private string retailId = string.Empty;
        private string retailUser = string.Empty;
        private string mid;
        private string token;
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.LoginDangLe"/> class.
        /// </summary>
        /// <param name="retailId">Retail id.</param>
        /// <param name="retailUser">Retail user id.</param>
        /// <param name="password">Password.</param>
        /// <param name="passportId">Passport id.</param>
        public LoginDangLe(string retailId, string retailUser, string password, string passportId) {
            this.retailId = retailId;
            this.retailUser = HttpUtility.UrlEncode(retailUser, Encoding.UTF8).ToUpper();
            Password = new DESAlgorithmNew().DecodePwd(password, GameEnvironment.Setting.ClientDesDeKey);
            mid = passportId.Equals("0") ? string.Empty : passportId;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.LoginDangLe"/> class.
        /// </summary>
        /// <param name="retailId">Retail id.</param>
        /// <param name="mid">Middle.</param>
        /// <param name="token">Token.</param>
        public LoginDangLe(string retailId, string mid, string token) {
            this.retailId = retailId ?? "0037";
            this.mid = mid.Equals("0") ? string.Empty : mid;
            this.token = token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckLogin() {
            string url = "";
            string appId = "";
            string appKey = "";
            bool isOldVersion = false;
            GameChannel gameChannel = ZyGameBaseConfigManager.GameSetting.GetChannelSetting(ChannelType.channelDanle);
            if (gameChannel != null) {
                url = gameChannel.Url;
                isOldVersion = "0.1".Equals(gameChannel.Version);
                GameSdkSetting setting = gameChannel.GetSetting(retailId);
                if (setting != null) {
                    appId = setting.AppId;
                    appKey = setting.AppKey;
                }
            } else {
                TraceLog.ReleaseWrite("The sdkChannel Danle section is null.");
            }
            string Url = "";
            if (isOldVersion) {
                string sig = MD5(string.Format("api_key={0}&mid={1}&username={2}&sha256_pwd={3}&secret_key={4}", appId, mid, retailUser, SHA256(Password), appKey));
                string vc = MD5(string.Format("api_key={0}&mid={1}&username={2}&sig={3}", appId, PassportId, retailUser, sig));
                Url = string.Format("http://connect.d.cn/connect/json/member/login?api_key={0}&mid={1}&username={2}&vc={3}&sig={4}", appId, mid, retailUser, vc, sig);
            } else {
                string sig = MD5(string.Format("{0}|{1}", token, appKey));
                Url = string.Format("{0}?app_id={1}&mid={2}&token={3}&sig={4}", url, appId, mid, token, sig);
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
            req.Method = "Get";
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();

            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            DangLeSDK sdk = null;
            try {
                sdk = JsonUtils.Deserialize<DangLeSDK>(result);
            } catch (Exception ex) {
                new BaseLog().SaveLog(ex);
                return false;
            }
            if (sdk == null || sdk.error_code != 0) {
                TraceLog.ReleaseWrite("Danlesdk login fail:{0},request url:{1}", result, Url);
                return false;
            }

            string[] arr = SnsManager.LoginByRetail(retailId, sdk.memberId);
            UserId = arr[0];
            PassportId = arr[1];
            SessionId = GetSessionId();
            return true;
        }


    }

    /// <summary>
    /// DangLe SDK.
    /// </summary>
    public class DangLeSDK {
        /// <summary>
        /// The member identifier.
        /// </summary>
        public string memberId;
        /// <summary>
        /// The username.
        /// </summary>
        public string username;
        /// <summary>
        /// The nickname.
        /// </summary>
        public string nickname;
        /// <summary>
        /// The gender.
        /// </summary>
        public string gender;
        /// <summary>
        /// The level.
        /// </summary>
        public int level;
        /// <summary>
        /// The avatar_url.
        /// </summary>
        public string avatar_url;
        /// <summary>
        /// The created_date.
        /// </summary>
        public string created_date;
        /// <summary>
        /// The token.
        /// </summary>
        public string token;
        /// <summary>
        /// The error_code.
        /// </summary>
        public int error_code;
        /// <summary>
        /// The error_msg.
        /// </summary>
        public string error_msg;
    }
}