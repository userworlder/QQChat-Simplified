using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QQCommon.Interfaces;
using QQCommon.Models;
using QQServer.DataAccess;
using System.Collections.Generic;

namespace QQServer.Business
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly GroupMemberDao _memberDao;

        public GroupMemberService()
        {
            _memberDao = new GroupMemberDao();
        }

        public List<GroupMember> GetGroupMembersByGroupId(string groupId)
        {
            return _memberDao.GetGroupMembersByGroupId(groupId);
        }

        public bool AddGroupMember(GroupMember member)
        {
            return _memberDao.AddGroupMember(member);
        }

        public bool RemoveGroupMember(string groupId, string userId)
        {
            return _memberDao.RemoveGroupMember(groupId, userId);
        }
    }
}