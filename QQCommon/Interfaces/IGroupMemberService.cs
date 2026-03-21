using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Interfaces
{
    public interface IGroupMemberService
    {
        List<GroupMember> GetGroupMembersByGroupId(string groupId);
        bool AddGroupMember(GroupMember member);
        bool RemoveGroupMember(string groupId, string userId);
        GroupMember GetGroupMember(string groupId, string userId);
    }
}
