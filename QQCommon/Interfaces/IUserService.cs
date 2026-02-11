using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Interfaces
{
    public interface IUserService
    {
        // 登录，返回用户信息，失败返回null
        User Login(string username, string password);

        // 注册，返回是否成功
        bool Register(User user);

        // 根据用户名搜索用户（用于添加好友）
        User SearchUser(string username);

        // 更新用户在线状态
        void UpdateUserStatus(string userId, bool isOnline);
    }
}
