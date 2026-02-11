using QQCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQClient.Communication
{
    public class NetworkClient : INetworkClient
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionEventArgs> ConnectionChanged;

        public bool AddFriend(string friendUsername)
        {
            throw new NotImplementedException();
        }

        public bool Connect(string serverIp, int port)
        {
            return false;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool Register(string username, string password, string nickname)
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(string receiver, string content)
        {
            throw new NotImplementedException();
        }

        void Disconnected()
        {

        }

    }
}
