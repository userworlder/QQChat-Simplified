using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQClient.Business.Services
{
    public interface IGroupBusinessService
    {
        // 群组基本信息操作
        Task SaveGroupAsync(Group group);
        Task<List<Group>> GetGroupsAsync(string userId);
        Task<Group> GetGroupByIdAsync(string groupId);
        Task<List<Group>> SearchGroupsAsync(string keyword);

        // 群消息操作
        Task SaveGroupMessageAsync(GroupMessage message);
        Task<List<GroupMessage>> GetGroupHistoryAsync(string groupId, int limit = 50);

        // 群成员操作
        Task SaveGroupMemberAsync(GroupMember member);
        Task<List<GroupMember>> GetGroupMembersAsync(string groupId);
        Task<bool> IsGroupMemberAsync(string groupId, string userId);
        Task<bool> RemoveGroupMemberAsync(string groupId, string userId);
        Task<int> GetGroupMemberCountAsync(string groupId);

        // 新增方法
        Task<List<Group>> GetGroupListAsync();
        Task<bool> SendGroupMessageAsync(string groupId, string content);
        Task<string> CreateGroupAsync(string groupName, string description = "");
        Task<bool> InviteToGroupAsync(string groupId, string invitedUserId);
        Task<bool> JoinGroupAsync(string groupId);

        Task<bool> LeaveGroupAsync(string groupId);

        // 事件
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}