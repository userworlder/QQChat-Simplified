using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Interfaces
{
    public interface INetworkClient
    {
        // 连接服务器
        bool Connect(string serverIp, int port);

        // 断开连接
        void Disconnect();

        // 登录
        bool Login(string username, string password);

        // 注册
        bool Register(string username, string password, string nickname);

        // 发送消息
        bool SendMessage(string username,string receiver, string content);

        // 添加好友
        bool AddFriend(string fromUserId,string toUserId);

        //Id查询
        bool SearchId(string fromUserId,string userId);

        bool AcceptFriendRequest(string fromUserId);
        bool RejectFriendRequest(string fromUserId);

        // 事件：收到消息
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        // 事件：连接状态改变
        event EventHandler<ConnectionEventArgs> ConnectionChanged;
    }

    //当通信端收到新消息时，通过事件通知UI层，并传递消息的详细信息。
    public class MessageReceivedEventArgs : EventArgs
    {
        public ChatPacket Packet { get; set; }
        public MessageReceivedEventArgs(ChatPacket packet)
        {
            Packet = packet;
        }
    }
    //当连接状态发生变化时，通知UI层更新界面。
    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
        public ConnectionEventArgs(bool isConnected , string message)
        {
            IsConnected = isConnected;
            Message = message;
        }

    }
}
