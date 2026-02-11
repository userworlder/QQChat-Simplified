using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class Friend
    {
        public string FriendId { get; set; }
        public string UserId { get; set; }
        public string FriendUserId { get; set; }
        public string Remark { get; set; }      // 备注
        public string GroupName { get; set; }   // 分组名
        public DateTime AddTime { get; set; }
    }
}
