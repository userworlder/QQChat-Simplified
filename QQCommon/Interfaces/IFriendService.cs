using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Interfaces
{
    public interface IFriendService
    {
        // 发送好友请求
        bool AddFriendRequest(string fromUser, string toUser);
        
        // 接受好友请求
        bool AcceptFriendRequest(string fromUser, string toUser);
        
        // 拒绝好友请求
        bool RejectFriendRequest(string fromUser, string toUser);
        
        // 删除好友
        bool RemoveFriend(string userId, string friendUserId);
        
        // 获取好友列表
        List<Friend> GetFriendList(string userId);
        
        // 查询一个人所有好友
        Friend GetFriendInfo(string userId, string friendUserId);
        
        // 更新好友备注
        bool UpdateFriendRemark(string userId, string friendUserId, string remark);
        
        // 移动好友到不同分组
        bool MoveFriendToGroup(string userId, string friendUserId, string groupName);
        
        // 获取好友请求列表
        List<string> GetFriendRequests(string userId);
        
        // 搜索用户（用于添加好友）
        List<User> SearchUsers(string keyword);
    }

}
