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
        bool AddFriendRequest(string fromUser, string toUser);
    }

}
