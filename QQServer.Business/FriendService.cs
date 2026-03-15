using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQServer.DataAccess;

namespace QQServer.Business
{
    public class FriendService : IFriendService
    {
        private readonly FriendDao friendDao;
        private readonly UserDao userDao;
        private readonly FriendRequestDao friendRequestDao;

        public FriendService()
        {
            friendDao = new FriendDao();
            userDao = new UserDao();
            friendRequestDao = new FriendRequestDao();
        }

        public bool AddFriendRequest(string fromUser, string toUser)
        {
            // 使用FriendRequestDao发送好友请求
            try
            {
                Console.WriteLine($"[FriendService] 开始添加好友请求: {fromUser} -> {toUser}");
                bool result = friendRequestDao.AddFriendRequest(fromUser, toUser);
                Console.WriteLine($"[FriendService] 添加好友请求结果: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FriendService] 添加好友请求异常: {ex.Message}");
                Console.WriteLine($"[FriendService] 异常堆栈: {ex.StackTrace}");
                return false;
            }
        }

        public bool AcceptFriendRequest(string fromUser, string toUser)
        {
            // 首先更新好友请求状态为已接受
            bool updateStatus = friendRequestDao.AcceptFriendRequest(fromUser, toUser);
            if (!updateStatus)
            {
                return false;
            }

            // 然后创建双向好友关系
            Friend friend1 = new Friend
            {
                FriendId = Guid.NewGuid().ToString(),
                UserId = fromUser,
                FriendUserId = toUser,
                Remark = string.Empty,
                GroupName = "好友",
                AddTime = DateTime.Now
            };

            Friend friend2 = new Friend
            {
                FriendId = Guid.NewGuid().ToString(),
                UserId = toUser,
                FriendUserId = fromUser,
                Remark = string.Empty,
                GroupName = "好友",
                AddTime = DateTime.Now
            };

            return friendDao.AddFriend(friend1) && friendDao.AddFriend(friend2);
        }

        public bool RejectFriendRequest(string fromUser, string toUser)
        {
            // 使用FriendRequestDao拒绝好友请求
            return friendRequestDao.RejectFriendRequest(fromUser, toUser);
        }

        public bool RemoveFriend(string userId, string friendUserId)
        {
            // 删除好友，需要删除双向好友关系
            return friendDao.RemoveFriend(userId, friendUserId) && friendDao.RemoveFriend(friendUserId, userId);
        }

        public List<Friend> GetFriendList(string userId)
        {
            // 使用FriendDao获取用户的好友列表
            return friendDao.GetFriendsByUserId(userId);
        }

        public Friend GetFriendInfo(string userId, string friendUserId)
        {
            // 使用FriendDao获取好友详细信息
            return friendDao.GetFriendByUserIdAndFriendUserId(userId, friendUserId);
        }

        public bool UpdateFriendRemark(string userId, string friendUserId, string remark)
        {
            // 使用FriendDao更新好友备注
            return friendDao.UpdateFriendRemark(userId, friendUserId, remark);
        }

        public bool MoveFriendToGroup(string userId, string friendUserId, string groupName)
        {
            // 使用FriendDao更新好友分组
            return friendDao.UpdateFriendGroup(userId, friendUserId, groupName);
        }

        public List<string> GetFriendRequests(string userId)
        {
            // 使用FriendRequestDao获取用户的好友请求列表
            return friendRequestDao.GetFriendRequests(userId);
        }

        public List<User> SearchUsers(string keyword)
        {
            // 使用UserDao搜索用户
            return userDao.SearchUsers(keyword);
        }
    }
}
