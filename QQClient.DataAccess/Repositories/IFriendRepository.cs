using System.Collections.Generic;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQClient.DataAccess.Repositories
{
    public interface IFriendRepository
    {
        Task SaveFriendAsync(Friend friend);
        Task<List<Friend>> GetFriendsAsync(string userId);
        Task<Friend> GetFriendAsync(string userId, string friendId);  // 添加这个方法
        Task UpdateFriendRemarkAsync(string userId, string friendId, string remark);
        Task<bool> DeleteFriendAsync(string userId, string friendId);
        Task<bool> IsFriendAsync(string userId, string friendId);     // 添加这个方法
        Task<int> GetFriendsCountAsync(string userId);                // 添加这个方法
    }
}