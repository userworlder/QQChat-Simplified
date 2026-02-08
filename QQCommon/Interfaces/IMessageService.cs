using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQCommon.Interfaces
{
    public interface IMessageService
    {
        // 发送消息，返回是否成功
        bool SendMessage(Message message);

        // 获取两个用户之间的聊天记录
        List<Message> GetChatHistory(string userId1, string userId2, int limit = 50);

        // 获取未读消息
        List<Message> GetUnreadMessages(string userId);
    }
}
