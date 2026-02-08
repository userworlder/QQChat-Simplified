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
        bool SendMessage(string receiver, string content);

        // 添加好友
        bool AddFriend(string friendUsername);

        // 事件：收到消息
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        // 事件：连接状态改变
        event EventHandler<ConnectionEventArgs> ConnectionChanged;
    }

    //当通信端收到新消息时，通过事件通知UI层，并传递消息的详细信息。
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
    //当连接状态发生变化时，通知UI层更新界面。
    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
    }
}
