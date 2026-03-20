using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Interfaces
{
    public interface IGroupMessageService
    {
        bool SendGroupMessage(GroupMessage message);
        List<GroupMessage> GetGroupMessagesByGroupId(string groupId, int limit = 50);
    }
}
