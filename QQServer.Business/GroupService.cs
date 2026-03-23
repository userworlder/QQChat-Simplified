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
        /// 删除群组（同时删除群成员和群消息）
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteGroup(string groupId)
        {
            // 1. 删除群消息
            bool msgDeleted = _messageDao.DeleteGroupMessages(groupId);
            if (!msgDeleted)
            {
                Console.WriteLine($"[GroupService] 删除群消息失败，群ID: {groupId}");
                // 可以继续执行，但不影响群基本信息的删除，但为了数据一致性，可以选择返回false
                // 这里选择继续，但记录日志
            }

            // 2. 删除群成员
            bool membersDeleted = _memberDao.RemoveAllGroupMembers(groupId);
            if (!membersDeleted)
            {
                Console.WriteLine($"[GroupService] 删除群成员失败，群ID: {groupId}");
                // 继续
            }

            // 3. 删除群基本信息
            bool groupDeleted = _groupDao.DeleteGroup(groupId);
            if (!groupDeleted)
            {
                Console.WriteLine($"[GroupService] 删除群基本信息失败，群ID: {groupId}");
                return false;
            }

            return true;
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
        public Group GetGroupById(string groupId)
        {
            return _groupDao.GetGroupById(groupId);
        }

        public List<Group> SearchGroups(string keyword)
        {
            return _groupDao.SearchGroups(keyword);
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
