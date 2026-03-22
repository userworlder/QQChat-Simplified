using System.Collections.Generic;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQClient.DataAccess.Repositories
{
    public interface IGroupRepository
    {
        // 群组基本信息操作
        Task SaveGroupAsync(Group group);
        Task<List<Group>> GetGroupsAsync(string userId);
        Task<Group> GetGroupByIdAsync(string groupId);                    // 添加这个方法
        Task<List<Group>> SearchGroupsAsync(string keyword);

        // 群消息操作
        Task SaveGroupMessageAsync(GroupMessage message);
        Task<List<GroupMessage>> GetGroupHistoryAsync(string groupId, int limit = 50);

        // 群成员操作
        Task SaveGroupMemberAsync(GroupMember member);                    // 添加这个方法
        Task<List<GroupMember>> GetGroupMembersAsync(string groupId);     // 添加这个方法
        Task<bool> IsGroupMemberAsync(string groupId, string userId);     // 添加这个方法
        Task<bool> RemoveGroupMemberAsync(string groupId, string userId); // 添加这个方法
        Task<int> GetGroupMemberCountAsync(string groupId);               // 添加这个方法
    }
}