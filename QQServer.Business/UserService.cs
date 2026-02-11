using QQCommon.Interfaces;
using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQServer.Business
{
    public class UserService : IUserService
    {
        User IUserService.Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        bool IUserService.Register(User user)
        {
            throw new NotImplementedException();
        }

        User IUserService.SearchUser(string username)
        {
            throw new NotImplementedException();
        }

        void IUserService.UpdateUserStatus(string userId, bool isOnline)
        {
            throw new NotImplementedException();
        }
    }
}
