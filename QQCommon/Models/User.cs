using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class User
    {
        public string UserId { get; set; }      // 可以用GUID或自增ID
        public string Username { get; set; }
        public string Password { get; set; }    // 存储加密后的密码
        public string Nickname { get; set; } //昵称
        public string Avatar { get; set; }      // 头像Base64或路径
        public string Signature { get; set; }   // 个性签名
        public bool IsOnline { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime RegisterTime { get; set; }
    }
}
