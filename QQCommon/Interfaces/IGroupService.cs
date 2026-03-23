using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Interfaces
{
    public interface IGroupService
    {
        List<Group> GetGroupsByUserId(string userId);
        bool SendGroupMessage(GroupMessage message);
        List<GroupMessage> GetGroupMessagesByGroupId(string groupId, int limit = 50);
        string CreateGroup(string creatorUsername, string groupName, string description);
        Group GetGroupById(string groupId);
        List<Group> SearchGroups(string keyword);
        bool DeleteGroup(string groupId);
    }
}
