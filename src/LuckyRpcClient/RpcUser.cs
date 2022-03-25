using Lucky.User;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Lucky
{
    // 按照访问权限划分的话，服务端的接口分两类：
    // 1 不验证签名任何人都能访问；
    // 2 标记了Role.User或Role.Admin属性会验证签名和角色。
    // RpcUser 就是用来在内存中（因为使用的 SecureString，内存中的值会被即时删除）存放用户登录时填写的用户名密码的地方，以供访问需要签名的服务时使用。
    public class RpcUser : ISignUser
    {
        public static readonly RpcUser Empty = new RpcUser(LuckyKeyword.Localhost, HashUtil.Sha1(LuckyKeyword.Localhost));

        public static Dictionary<string, string> GetSignData(string loginName, string passwordSha1, object data = null)
        {
            long timestamp = Timestamp.GetTimestamp(DateTime.Now);
            return new Dictionary<string, string>
            {
                ["loginName"] = loginName,
                ["sign"] = HashUtil.CalcSign(loginName, passwordSha1, timestamp, data),
                ["timestamp"] = timestamp.ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginedUser"></param>
        /// <param name="passwordSha1">密码是必须的，因为传输 Windows 远程桌面密码时会用这个密码加密</param>
        public RpcUser(LoginedUser loginedUser, string passwordSha1)
        {
            if (string.IsNullOrEmpty(passwordSha1))
            {
                throw new InvalidProgramException();
            }
            LoginedUser = loginedUser;
            LoginName = loginedUser.LoginName;
            Password = passwordSha1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="passwordSha1">密码是必须的，因为传输Windows远程桌面密码时会用这个密码加密</param>
        public RpcUser(string loginName, string passwordSha1)
        {
            if (string.IsNullOrEmpty(passwordSha1))
            {
                throw new InvalidProgramException();
            }
            LoginName = loginName;
            Password = passwordSha1;
            LoginedUser = LoginedUser.Empty;
        }

        public LoginedUser LoginedUser { get; private set; }

        public string LoginName { get; private set; }

        public string Password
        {
            get => GetPassword();
            private set => SetPassword(value);
        }

        private IntPtr ptr = IntPtr.Zero;
        private void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return;
            }
            using (SecureString secureString = new SecureString())
            {
                foreach (char c in password.ToCharArray())
                {
                    secureString.AppendChar(c);
                }
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ptr);
                }
                ptr = Marshal.SecureStringToBSTR(secureString);
            }
        }

        private string GetPassword()
        {
            if (ptr == IntPtr.Zero)
            {
                return string.Empty;
            }
            return Marshal.PtrToStringBSTR(ptr);
        }

        public void Logout()
        {
            LoginName = string.Empty;
            LoginedUser = LoginedUser.Empty;
            if (ptr != IntPtr.Zero)
            {
                Marshal.ZeroFreeBSTR(ptr);
            }
        }

        /// <summary>
        /// 对给定的数据和内部附加的时间戳进行签名，传入的签名数据可以为null，此时相当于只签名内部附加的时间戳。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSignData(object data = null)
        {
            var timestamp = Timestamp.GetTimestamp(DateTime.Now);
            return new Dictionary<string, string>
            {
                ["loginName"] = LoginName,
                ["sign"] = HashUtil.CalcSign(LoginName, Password, timestamp, data),
                ["timestamp"] = timestamp.ToString()
            };
        }
    }
}
