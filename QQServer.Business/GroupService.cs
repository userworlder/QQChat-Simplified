using QQCommon.Models;
using QQServer.DataAccess;
using QQCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQServer.Business
{
    public class GroupService : IGroupService
    {
        private readonly GroupDao _groupDao;
        private readonly GroupMemberDao _memberDao;
        private readonly GroupMessageDao _messageDao;

        public GroupService()
        {
            _groupDao = new GroupDao();
            _memberDao = new GroupMemberDao();
            _messageDao = new GroupMessageDao();
        }

        public List<Group> GetGroupsByUserId(string userId)
        {
            var members = _memberDao.GetGroupMembersByUserId(userId);
            List<Group> groups = new List<Group>();
            foreach (var member in members)
            {
                var group = _groupDao.GetGroupById(member.GroupId);
                if (group != null)
                    groups.Add(group);
            }
            return groups;
        }
        public string CreateGroup(string creatorUsername, string groupName, string description)
        {
            // 生成群ID（可以使用GUID）
            string groupId = Guid.NewGuid().ToString();

            // 1. 插入群信息
            var group = new Group
            {
                GroupId = groupId,
                GroupName = groupName,
                Description = description,
                CreatorId = creatorUsername,   // 注意：CreatorId 存储用户名，需与 Users.Username 关联
                CreateTime = DateTime.Now
            };
            bool groupCreated = _groupDao.CreateGroup(group);
            if (!groupCreated) return null;

            // 2. 将创建者加入群成员（角色为群主，Role=2）
            var member = new GroupMember
            {
                GroupMemberId = Guid.NewGuid().ToString(),
                GroupId = groupId,
                UserId = creatorUsername,
                Nickname = null,    // 群内昵称可为空，默认使用用户名
                Role = 2,           // 群主
                JoinTime = DateTime.Now
            };
            bool memberAdded = _memberDao.AddGroupMember(member);
            if (!memberAdded)
            {
                // 可选：回滚群创建
                _groupDao.DeleteGroup(groupId);
                return null;
            }

            return groupId;
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
