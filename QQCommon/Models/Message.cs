using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class Message
    {
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }  // 用户ID或群ID
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
        public bool IsRead { get; set; }
        public int MessageType { get; set; }    // 1=文本，2=图片，3=文件
    }
}
