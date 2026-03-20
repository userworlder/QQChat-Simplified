using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QQCommon.Interfaces;
using QQCommon.Models;
using QQServer.DataAccess;

namespace QQServer.Business
{
    public class GroupMessageService : IGroupMessageService
    {
        private readonly GroupMessageDao _messageDao;

        public GroupMessageService()
        {
            _messageDao = new GroupMessageDao();
        }

        public bool SendGroupMessage(GroupMessage message)
        {
            return _messageDao.SendGroupMessage(message);
        }

        public List<GroupMessage> GetGroupMessagesByGroupId(string groupId, int limit = 50)
        {
            return _messageDao.GetGroupMessagesByGroupId(groupId, limit);
        }
    }
}
