using QQCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//防止关键词冲突，Msg同模型Message
using Msg = QQCommon.Models.Message;
namespace QQCommon.Models
{
    public static class GlobalClient
    {
        // 静态属性，保存全局唯一的 INetworkClient 实例
        public static INetworkClient Current { get; set; }
        public  static string CurrentUserId {  get; set; }

        //// 新增：存储离线消息，键为好友用户名
        //public static Dictionary<string, List<Msg>> MessageCache { get; } 
        //    = new Dictionary<string, List<Msg>>();

        //public static Dictionary<string, List<GroupMessage>> GroupMessageCache { get; } 
        //    = new Dictionary<string, List<GroupMessage>>();

        //public static Dictionary<string, int> GroupUnreadCount { get; } = new Dictionary<string, int>();
    }
}
