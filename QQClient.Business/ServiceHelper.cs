using QQClient.Business.Services;
using QQCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQClient.Business
{
    public static class ServiceHelper
    {
        public static IUserBusinessService UserService => ServiceContainer.Resolve<IUserBusinessService>();
        public static IFriendBusinessService FriendService => ServiceContainer.Resolve<IFriendBusinessService>();
        public static IMessageBusinessService MessageService => ServiceContainer.Resolve<IMessageBusinessService>();
        public static IGroupBusinessService GroupService => ServiceContainer.Resolve<IGroupBusinessService>();
        public static INetworkClient NetworkClient => ServiceContainer.Resolve<INetworkClient>();

        public static bool IsInitialized => ServiceContainer.IsRegistered<IUserBusinessService>();
    }
}