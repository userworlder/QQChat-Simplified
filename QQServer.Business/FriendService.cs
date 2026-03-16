using QQCommon.Interfaces;
using QQCommon.Models;
using QQServer.DataAccess;
using System;
using System.Collections.Generic;

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
            if (fromUser == toUser) return false;
            if (userDao.GetUserByUsername(fromUser) == null || userDao.GetUserByUsername(toUser) == null)
                return false;
            if (friendDao.GetFriendByUserNames(fromUser, toUser) != null)
                return false; // 已是好友
            // 可选：检查是否已有待处理请求（可自行实现）
            return friendRequestDao.AddFriendRequest(fromUser, toUser);
        }

        public bool AcceptFriendRequest(string fromUser, string toUser)
        {
            if (!friendRequestDao.AcceptFriendRequest(fromUser, toUser))
                return false;

            var fromUserInfo = userDao.GetUserByUsername(fromUser);
            var toUserInfo = userDao.GetUserByUsername(toUser);

            var friend1 = new Friend
            {
                UserName = fromUser,
                FriendUserName = toUser,
                FriendNickName = toUserInfo?.Nickname,
                Remark = "",
                GroupName = "好友",
                AddTime = DateTime.Now
            };
            var friend2 = new Friend
            {
                UserName = toUser,
                FriendUserName = fromUser,
                FriendNickName = fromUserInfo?.Nickname,
                Remark = "",
                GroupName = "好友",
                AddTime = DateTime.Now
            };

            return friendDao.AddFriend(friend1) && friendDao.AddFriend(friend2);
        }

        public bool RejectFriendRequest(string fromUser, string toUser)
        {
            return friendRequestDao.RejectFriendRequest(fromUser, toUser);
        }

        public bool RemoveFriend(string userName, string friendUserName)
        {
            return friendDao.RemoveFriend(userName, friendUserName) &&
                   friendDao.RemoveFriend(friendUserName, userName);
        }

        public List<Friend> GetFriendList(string userName)
        {
            return friendDao.GetFriendsByUserName(userName);
        }

        public Friend GetFriendInfo(string userName, string friendUserName)
        {
            return friendDao.GetFriendByUserNames(userName, friendUserName);
        }

        public bool UpdateFriendRemark(string userName, string friendUserName, string remark)
        {
            return friendDao.UpdateFriendRemark(userName, friendUserName, remark);
        }

        public bool MoveFriendToGroup(string userName, string friendUserName, string groupName)
        {
            return friendDao.UpdateFriendGroup(userName, friendUserName, groupName);
        }

        public List<string> GetFriendRequests(string userName)
        {
            return friendRequestDao.GetFriendRequests(userName);
        }

        public List<User> SearchUsers(string keyword)
        {
            return userDao.SearchUsers(keyword);
        }
    }
}