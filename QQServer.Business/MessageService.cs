using QQCommon.Interfaces;
using QQCommon.Models;
using QQServer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQServer.Business
{
    public class MessageService : IMessageService
    {
        private readonly MessageDao messageDao;

        public MessageService()
        {
            messageDao = new MessageDao();
        }
        public bool SendMessage(Message message)
        {
            // 生成消息ID
            message.MessageId = Guid.NewGuid().ToString();
            message.SendTime = DateTime.Now;
            message.IsRead = false;

            // 使用MessageDao发送消息
            return messageDao.SendMessage(message);
        }

        public List<Message> GetChatHistory(string userId1, string userId2, int limit = 50)
        {
            // 使用MessageDao获取聊天记录
            List<Message> messages = messageDao.GetMessagesBetweenUsers(userId1, userId2);

            // 限制返回的消息数量
            if (messages.Count > limit)
            {
                return messages.Skip(messages.Count - limit).ToList();
            }

            return messages;
        }
        public List<Message> GetChatHistory(string userId1, string userId2)
        {
            return messageDao.GetMessagesBetweenUsers(userId1, userId2);
        }

        public void MarkMessagesAsRead(string receiverId, string senderId)
        {
            messageDao.MarkMessagesAsRead(receiverId, senderId);
        }
        public List<Message> GetUnreadMessages(string userId)
        {
            return messageDao.GetUnreadMessages(userId);
        }
    }
}
