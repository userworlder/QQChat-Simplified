using QQCommon.Interfaces;
using QQCommon.Models;
using QQServer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQServer.Business
{
    public class UserService : IUserService
    {
        private readonly UserDao userDao;

        public UserService()
        {
            userDao = new UserDao();
        }

        // 登录，返回用户信息，失败返回null
        public User Login(string username, string password)
        {
            User user = userDao.GetUserByUsernameAndPassword(username, password);
            if (user != null)
            {
                // 登录成功，更新用户在线状态为在线
                userDao.UpdateUserStatus(user.UserId, true);
                user.IsOnline = true;
            }
            return user;
        }

        // 注册，返回是否成功
        public bool Register(User user)
        {
            // 生成唯一的用户ID
            user.UserId = Guid.NewGuid().ToString();
            user.RegisterTime = DateTime.Now;
            user.IsOnline = false;
            return userDao.InsertUser(user);
        }

        // 根据用户名搜索用户（用于添加好友）
        public User SearchUser(string username)
        {
            return userDao.GetUserByUsername(username);
        }

        // 更新用户在线状态
        public void UpdateUserStatus(string userId, bool isOnline)
        {
            userDao.UpdateUserStatus(userId, isOnline);
        }
    }
}

