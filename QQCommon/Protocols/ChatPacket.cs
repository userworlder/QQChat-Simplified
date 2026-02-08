using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QQCommon.Protocols
{
    public class ChatPacket
    {
        public MessageType Type { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }  // 可以是用户名或"ALL"
        public string Content { get; set; }   // JSON字符串或普通文本
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Extras { get; set; }

        public ChatPacket()
        {
            Extras = new Dictionary<string, string>();
            Timestamp = DateTime.Now;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ChatPacket FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ChatPacket>(json);
        }
    }
}