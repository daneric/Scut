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
using System.Data;
using ZyGames.Framework.Common;
using ZyGames.Framework.Common.Configuration;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Common.Security;
using ZyGames.Framework.Data;
using ZyGames.Framework.Game.Config;

namespace ZyGames.Framework.Game.Sns {
    /// <summary>
    /// Reg type.
    /// </summary>
    public enum RegType {
        /// <summary>
        /// 正常形式
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 游客通过设备ID登录
        /// </summary>
        Guest,
        /// <summary>
        /// The other.
        /// </summary>
        Other
    }
    /// <summary>
    /// Pwd type.
    /// </summary>
    public enum PwdType {
        /// <summary>
        /// The DE.
        /// </summary>
        DES = 0,
        /// <summary>
        /// The M d5.
        /// </summary>
        MD5
    }

    /// <summary>
    /// SnsCenterUser 的摘要说明
    /// </summary>
    public class SnsCenterUser {
        /// <summary>
        /// Md5 key
        /// </summary>
        private const string PasswordMd5Key = "1736e1c9-6f40-48b6-8210-da39cf333784";
        /// <summary>
        /// Passwords the encrypt md5.
        /// </summary>
        /// <returns>The encrypt md5.</returns>
        /// <param name="str">String.</param>
        public static string PasswordEncryptMd5(string str) {
            return CryptoHelper.RegUser_MD5_Pwd(str);
        }

        /// <summary>
        /// 官网渠道ID
        /// </summary>
        private const string SystemRetailId = "0000";
        private int userId;
        /// <summary>
        /// 获得用户ID
        /// </summary>
        /// 
        public int UserId { get { return userId; } }
        private string passportId = String.Empty;
        private string password = String.Empty;
        private string deviceId = String.Empty;
        private BaseLog logger = new BaseLog();

        /// <summary>
        /// Gets the passport identifier.
        /// </summary>
        /// <value>The passport identifier.</value>
        public string PassportId {
            get { return passportId; }
        }
        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password {
            get { return password; }
        }
        /// <summary>
        /// Gets or sets the retail I.
        /// </summary>
        /// <value>The retail I.</value>
        public string RetailId {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the weixin code.
        /// </summary>
        /// <value>The weixin code.</value>
        public string WeixinCode {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the retail user.
        /// </summary>
        /// <value>The retail user.</value>
        public string RetailUser {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of the reg.
        /// </summary>
        /// <value>The type of the reg.</value>
        public RegType RegType {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ZyGames.Framework.Game.Sns.SnsCenterUser"/> class.
        /// </summary>
        public SnsCenterUser() {
            RegType = RegType.Other;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="passportId"></param>
        /// <param name="password"></param>
        /// <param name="imei"></param>
        public SnsCenterUser(string passportId, string password, string imei) {
            this.passportId = passportId.ToUpper();
            this.password = password;
            this.deviceId = imei;
            RegType = RegType.Guest;
            RetailId = SystemRetailId;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="snsUser"></param>
        /// <returns></returns>
        public bool ValidatePassport(SnsUser snsUser) {
            string pwd = password;

            if (snsUser.RegType == RegType.Normal) {
                if (snsUser.PwdType == PwdType.MD5) {
                    pwd = PasswordEncryptMd5(pwd);
                }
                return snsUser.PassportId == passportId && snsUser.Password == pwd;
            }

            if (snsUser.RegType == RegType.Guest) {
                if (snsUser.PwdType == PwdType.MD5) {
                    //判断是否已经MD5加密
                    pwd = PasswordEncryptMd5(pwd);
                }
                return snsUser.PassportId == passportId &&
                    snsUser.Password == pwd;
            }

            return snsUser.RetailId == RetailId &&
                   snsUser.RetailUser == RetailUser;
        }

        /// <summary>
        /// 是否有绑定DeviceId
        /// </summary>
        /// <returns></returns>
        public SnsUser GetUserByDeviceId(string imei) {
            SnsUser snsUser = new SnsUser();
            if (string.IsNullOrEmpty(imei)) {
                return snsUser;
            }
            SetUserInfo(f => {
                f.Condition = string.Format("RegType = 1 AND {0}", f.FormatExpression("DeviceId")); //Guest
                f.AddParam("DeviceId", imei);
            }, snsUser);
            return snsUser;
        }

        /// <summary>
        /// Inserts the sns user.
        /// </summary>
        /// <returns>The sns user.</returns>
        /// <param name="paramNames">Parameter names.</param>
        /// <param name="paramValues">Parameter values.</param>
        /// <param name="isCustom"></param>
        public int InsertSnsUser(string[] paramNames, string[] paramValues, bool isCustom) {
            SnsPassport oSnsPassportLog = new SnsPassport();
            if (!isCustom && !oSnsPassportLog.VerifyRegPassportId(passportId)) {
                return 0;
            }
            //md5加密
            string pwd = PasswordEncryptMd5(this.password);

            var command = ConnectManager.Provider.CreateCommandStruct("SnsUserInfo", CommandMode.Insert);
            command.ReturnIdentity = true;
            command.AddParameter("PassportId", passportId);
            command.AddParameter("Password", pwd);
            command.AddParameter("DeviceId", deviceId);
            command.AddParameter("RegType", (int)RegType);
            command.AddParameter("RegTime", DateTime.Now);
            command.AddParameter("RetailId", RetailId);
            command.AddParameter("RetailUser", RetailUser);
            command.AddParameter("PwdType", (int)PwdType.MD5);

            if (paramNames != null && paramValues != null) {
                for (int i = 0; i < paramNames.Length; i++) {
                    command.AddParameter(paramNames[i], paramValues.Length > i ? paramValues[i] : "");
                }
            }
            command.Parser();

            try {
                if (!isCustom && !oSnsPassportLog.SetPassportReg(passportId)) {
                    throw new Exception("Set passport  State.Reg fail.");
                }
                using (var aReader = ConnectManager.Provider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters)) {
                    if (aReader.Read()) {
                        userId = Convert.ToInt32(aReader[0]);
                    }
                }
                return userId;
            } catch (Exception ex) {
                logger.SaveLog(ex);
                return 0;
            }
        }
        /// <summary>
        /// 向社区中心添加用户
        /// </summary>
        /// <returns></returns>
        public int InsertSnsUser(bool isCustom) {
            return InsertSnsUser(new string[0], new string[0], isCustom);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isReset">只重置密码</param>
        /// <returns></returns>
        public int ChangePassword(string userId, bool isReset = false) {
            try {
                //md5加密
                string pwd = PasswordEncryptMd5(this.password);

                var command = ConnectManager.Provider.CreateCommandStruct("SnsUserInfo", CommandMode.Modify);
                command.AddParameter("Password", pwd);
                if (!isReset) {
                    command.AddParameter("RegType", (int)RegType.Normal);
                    command.AddParameter("PwdType", (int)PwdType.MD5);
                }

                command.Filter = ConnectManager.Provider.CreateCommandFilter();

                var section = ConfigManager.Configger.GetFirstOrAddConfig<MiddlewareSection>();
                if (userId.ToUpper().StartsWith(section.PreAccount)) {
                    command.Filter.Condition = command.Filter.FormatExpression("PassportId");
                    command.Filter.AddParam("PassportId", userId);
                } else {
                    command.Filter.Condition = command.Filter.FormatExpression("UserId");
                    command.Filter.AddParam("UserId", userId);
                }
                command.Parser();

                return ConnectManager.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters);
            } catch (Exception ex) {
                logger.SaveLog(ex);
                return 0;
            }
        }

        internal int ChangeUserInfo(string pid, SnsUser snsuser) {
            try {
                var command = ConnectManager.Provider.CreateCommandStruct("SnsUserInfo", CommandMode.Modify);
                if (!string.IsNullOrEmpty(snsuser.Mobile)) {
                    command.AddParameter("Mobile", snsuser.Mobile);
                }
                if (!string.IsNullOrEmpty(snsuser.Mail)) {
                    command.AddParameter("Mail", snsuser.Mail);
                }
                if (!string.IsNullOrEmpty(snsuser.RealName)) {
                    command.AddParameter("RealName", snsuser.RealName);
                }
                if (!string.IsNullOrEmpty(snsuser.IdCards)) {
                    command.AddParameter("IdCards", snsuser.IdCards);
                }
                if (!string.IsNullOrEmpty(snsuser.ActiveCode)) {
                    command.AddParameter("ActiveCode", snsuser.ActiveCode);
                }
                if (snsuser.SendActiveDate > DateTime.MinValue) {
                    command.AddParameter("SendActiveDate", snsuser.SendActiveDate);
                }
                if (snsuser.ActiveDate > DateTime.MinValue) {
                    command.AddParameter("ActiveDate", snsuser.ActiveDate);
                }
                if (!string.IsNullOrEmpty(snsuser.WeixinCode)) {
                    command.AddParameter("WeixinCode", snsuser.WeixinCode);
                }
                command.Filter = ConnectManager.Provider.CreateCommandFilter();
                command.Filter.Condition = command.Filter.FormatExpression("PassportId");
                command.Filter.AddParam("PassportId", pid);
                command.Parser();

                return ConnectManager.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters);
            } catch (Exception ex) {
                logger.SaveLog(ex);
                return 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static bool CheckDevice(string device) {
            if (string.IsNullOrEmpty(device))
                return true;

            var command = ConnectManager.Provider.CreateCommandStruct("LimitDevice", CommandMode.Inquiry);
            command.Columns = "COUNT(DeviceId)";
            command.Filter = ConnectManager.Provider.CreateCommandFilter();
            command.Filter.Condition = command.Filter.FormatExpression("DeviceId");
            command.Filter.AddParam("DeviceId", device);
            command.Parser();

            int count = Convert.ToInt32(ConnectManager.Provider.ExecuteScalar(CommandType.Text, command.Sql, command.Parameters));
            return count <= 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="passportId"></param>
        /// <returns></returns>
        public SnsUser GetUserInfo(string passportId) {
            SnsUser snsUser = new SnsUser();
            SetUserInfo(f => {
                f.Condition = f.FormatExpression("PassportId");
                f.AddParam("PassportId", passportId);
            }, snsUser);
            return snsUser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retailId"></param>
        /// <param name="retailUser"></param>
        /// <returns></returns>
        public SnsUser GetUserInfo(string retailId, string retailUser) {
            SnsUser snsUser = new SnsUser();
            SetUserInfo(f => {
                f.Condition = string.Format("{0} AND {1}", f.FormatExpression("RetailId"), f.FormatExpression("RetailUser"));
                f.AddParam("RetailId", retailId);
                f.AddParam("RetailUser", retailUser);
            }, snsUser);
            return snsUser;
        }

        /// <summary>
        /// 通过微信号
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public SnsUser GetUserByWeixin(string openId) {
            SnsUser snsUser = new SnsUser();
            SetUserInfo(f => {
                f.Condition = f.FormatExpression("WeixinCode");
                f.AddParam("WeixinCode", openId);
            }, snsUser);
            return snsUser;
        }

        private void SetUserInfo(Action<CommandFilter> match, SnsUser snsUser) {
            var command = ConnectManager.Provider.CreateCommandStruct("SnsUserInfo", CommandMode.Inquiry);
            command.OrderBy = "USERID ASC";
            command.Columns = "UserId,PassportId,Password,DeviceId,RegType,RegTime,RetailId,RetailUser,Mobile,Mail,PwdType,RealName,IdCards,ActiveCode,SendActiveDate,ActiveDate,WeixinCode";
            command.Filter = ConnectManager.Provider.CreateCommandFilter();
            match(command.Filter);
            command.Parser();

            using (var reader = ConnectManager.Provider.ExecuteReader(CommandType.Text, command.Sql, command.Parameters)) {
                if (reader.Read()) {
                    snsUser.UserId = Convert.ToInt32(reader["UserId"]);
                    snsUser.DeviceId = Convert.ToString(reader["DeviceId"]);
                    snsUser.PassportId = Convert.ToString(reader["PassportId"]);
                    snsUser.Password = Convert.ToString(reader["Password"]);
                    snsUser.RegTime = Convert.ToDateTime(reader["RegTime"]);
                    snsUser.RetailId = Convert.ToString(reader["RetailId"]);
                    snsUser.RetailUser = Convert.ToString(reader["RetailUser"]);
                    snsUser.Mobile = Convert.ToString(reader["Mobile"]);
                    snsUser.Mail = Convert.ToString(reader["Mail"]);
                    snsUser.RealName = Convert.ToString(reader["RealName"]);
                    snsUser.IdCards = Convert.ToString(reader["IdCards"]);
                    snsUser.ActiveCode = Convert.ToString(reader["ActiveCode"]);
                    snsUser.SendActiveDate = ToDate(Convert.ToString(reader["SendActiveDate"]));
                    snsUser.ActiveDate = ToDate(Convert.ToString(reader["ActiveDate"]));
                    snsUser.WeixinCode = Convert.ToString(reader["WeixinCode"]);
                    snsUser.PwdType = (PwdType)Enum.ToObject(typeof(PwdType), Convert.ToInt32(reader["PwdType"]));
                    snsUser.RegType = (RegType)Enum.ToObject(typeof(RegType), Convert.ToInt32(reader["RegType"]));
                }
            }
        }

        private DateTime ToDate(string str) {
            DateTime result;
            DateTime.TryParse(str, out result);
            return result;
        }

        /// <summary>
        /// Adds the login log.
        /// </summary>
        /// <param name="deviceId">Device I.</param>
        /// <param name="passportId">Passport I.</param>
        public static void AddLoginLog(string deviceId, string passportId) {
            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(passportId)) {
                return;
            }
            var command = ConnectManager.Provider.CreateCommandStruct("PassportLoginLog", CommandMode.Insert);
            command.AddParameter("DeviceId", deviceId);
            command.AddParameter("PassportId", passportId);
            command.AddParameter("LoginTime", MathUtils.Now);
            command.Parser();

            ConnectManager.Provider.ExecuteQuery(CommandType.Text, command.Sql, command.Parameters);
        }
    }
}