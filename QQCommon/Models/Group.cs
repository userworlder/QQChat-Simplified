using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class Group
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupAvatar { get; set; }
        public string CreatorId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
    }
}