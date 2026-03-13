using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class GroupMember
    {
        public string GroupMemberId { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int Role { get; set; } // 0=普通成员，1=管理员，2=群主
        public DateTime JoinTime { get; set; }
    }
}