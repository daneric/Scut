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

namespace ZyGames.Framework.Game.Sns {

    class LoginFeiliu : AbstractLogin {

        private string retailId = string.Empty;
        private string retailUser = string.Empty;
        private string sign = string.Empty;
        private string timestamp = string.Empty;
        private string publicKey = string.Empty;


        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.LoginTencent"/> class.
        /// </summary>
        public LoginFeiliu(string retailId, string retailUser, string sign, string timestamp) {
            this.retailId = retailId;
            this.retailUser = retailUser;
            this.sign = sign;
            this.timestamp = timestamp;
        }

        public override string GetRegPassport() {
            return PassportId;
        }

        public override bool CheckLogin() {
            var signformbase64 = Decrypt(sign);
            string[] arr = SnsManager.LoginByRetail(retailId, retailUser);
            UserId = arr[0];
            PassportId = arr[1];
            RetailUserId = retailUser;
            SessionId = GetSessionId();
            return true;
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="base64code">传入加密数据</param>
        /// <returns>返回解密数据</returns>
        static public string Decrypt(string base64code) {
            try {
                UnicodeEncoding ByteConverter = new UnicodeEncoding();

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString("");

                RSAParameters rsaParameters = new RSAParameters();

                rsaParameters.Exponent = Convert.FromBase64String("AQAB");
                rsaParameters.Modulus = Convert.FromBase64String("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyq3xJ3jtuWSWk4nCCgysplqV3DyFGaF7iP7PO2vEUsgEq+vqKr+frlwji2n7A1TbpV7KhEGJIT9LW/9WCdBhlu6gnBdErtAA4Or43ol2K1BnY6VBcLWccloMd3YFHG8gOohCVIDbw863Wg0FNS27SM25U+XQfrNFaqBIa093WgAbwRIK06uzC01sW+soutvk+yAYBtbH7I7/1/dFixHKS2KN/7y3pvmXYBIRuBvn35IqwY3Gk0duEfbEr9F6wm2VKhS1zQG760FrHfhbXR+IN5nSTQBHBkw4QukLLvUqueKYfVdp2/2RCnY/At0bbOcA2tAPohDAfUDRdOZsFiTIMQID");

                byte[] encryptedData;
                byte[] decryptedData;

                encryptedData = Convert.FromBase64String(base64code);

                decryptedData = RSADecrypt(encryptedData, rsaParameters, true);
                return ByteConverter.GetString(decryptedData);
            } catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="DataToDeCrypto">要解密的数据</param>
        /// <param name="RSAKeyInfo"></param>
        /// <param name="DoOAEPPadding"></param>
        /// <returns></returns>
        static public byte[] RSADecrypt(byte[] DataToDeCrypto, RSAParameters RSAKeyInfo, bool DoOAEPPadding) {
            try {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                // System.Security.Cryptography.RSA 的参数。
                RSA.ImportParameters(RSAKeyInfo);
                //
                // 参数:
                //  
                //     要解密的数据。
                //
                //
                //     如果为 true，则使用 OAEP 填充（仅在运行 Microsoft Windows XP 或更高版本的计算机上可用）执行直接的 System.Security.Cryptography.RSA
                //     解密；否则，如果为 false，则使用 PKCS#1 1.5 版填充。
                return RSA.Decrypt(DataToDeCrypto, DoOAEPPadding);
            } catch (CryptographicException e) {
                Console.WriteLine(e.Message);
                return null;
            }
        }

    }
}
