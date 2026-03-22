using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQClient.Business.Services
{
    public interface IFriendBusinessService
    {
    
        // 搜索用户（用于添加好友）
    
        Task<bool> SearchUserAsync(string currentUserId, string targetUserId);

    
        //发送好友申请
    
        Task<bool> AddFriendAsync(string fromUserId, string toUserId);

    
        // 接受好友申请
    
        Task<bool> AcceptFriendRequestAsync(string fromUserId);

    
        // 拒绝好友申请
    
        Task<bool> RejectFriendRequestAsync(string fromUserId);

    
        // 获取好友列表
    
        Task<List<Friend>> GetFriendListAsync(string userId);

    
        //推送事件（如收到好友请求）
    
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        Task<Friend> GetFriendAsync(string userId, string friendId);

        Task<bool> IsFriendAsync(string userId, string friendId);     
        Task<int> GetFriendsCountAsync(string userId);
    }
}