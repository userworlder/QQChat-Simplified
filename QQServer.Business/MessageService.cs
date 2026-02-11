using QQCommon.Interfaces;
using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQServer.Business
{
    public class MessageService : IMessageService
    {
        List<Message> IMessageService.GetChatHistory(string userId1, string userId2, int limit)
        {
            throw new NotImplementedException();
        }

        List<Message> IMessageService.GetUnreadMessages(string userId)
        {
            throw new NotImplementedException();
        }

        bool IMessageService.SendMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
