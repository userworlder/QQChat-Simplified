using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class Friend
    {
        public string FriendNickName { get; set; }//好友昵称
        public string UserName { get; set; }//账号
        public string FriendUserName { get; set; }//好友账号
        public string Remark { get; set; }      // 备注
        public string GroupName { get; set; }   // 分组名
        public DateTime AddTime { get; set; }
    }
}
