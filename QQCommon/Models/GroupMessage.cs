using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class GroupMessage
    {
        public string MessageId { get; set; }
        public string GroupId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
        public int MessageType { get; set; } // 1=文本，2=图片，3=文件
    }
}