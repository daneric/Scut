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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// 登录处理基类
    /// </summary>
    public abstract class AbstractLogin : ILogin {
        /// <summary>
        /// 
        /// </summary>
        /// <value>The passport id.</value>
        public string PassportId { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>The user id.</value>
        public string UserId { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>The session id.</value>
        public string SessionId { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserType { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public string DeviceId { get; protected set; }
        /// <summary>
        /// 注册通行证
        /// </summary>
        /// <returns></returns>
        public virtual string GetRegPassport() { return PassportId; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckLogin();
        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <returns>The session identifier.</returns>
        protected string GetSessionId() {
            string sessionId = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Session != null) {
                sessionId = HttpContext.Current.Session.SessionID;
            } else {
                sessionId = Guid.NewGuid().ToString("N");
            }
            return sessionId;
        }
        /// <summary>
        /// 
        /// </summary>
        public string AccessToken { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public string RetailUserId { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public int ExpiresIn { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public string Scope { get; protected set; }

        /// <summary>
        /// MD5 Encrypt.
        /// </summary>
        /// <returns>The md5.</returns>
        /// <param name="str">String.</param>
        protected string MD5(string str) {
            return ZyGames.Framework.Common.Security.CryptoHelper.MD5_Encrypt(str, Encoding.UTF8).ToLower();
        }

        /// <summary>
        /// SHA256 Encrypt.
        /// </summary>
        /// <returns>The sha256.</returns>
        /// <param name="str">String.</param>
        protected string SHA256(string str) {
            byte[] tmpByte;
            SHA256 sha256 = new SHA256Managed();
            tmpByte = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
            sha256.Clear();
            string result = string.Empty;
            foreach (byte x in tmpByte) {
                result += string.Format("{0:x2}", x);
            }
            return result.ToUpper();
        }

        /// <summary>
        /// UTF8编码字符串计算MD5值(十六进制编码字符串)
        /// </summary>
        /// <param name="str">UTF8编码的字符串</param>
        /// <returns>MD5(十六进制编码字符串)</returns>
        protected string HashToMD5Hex(string str) {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider()) {
                byte[] result = md5.ComputeHash(bytes);
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < result.Length; i++) {
                    sBuilder.Append(result[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

    }
}