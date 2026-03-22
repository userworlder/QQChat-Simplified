using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQCommon.Models;

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

        // 发送私聊消息
        bool SendMessage(string username, string receiver, string content);

        // 添加好友
        bool AddFriend(string fromUserId, string toUserId);

        // 根据ID搜索用户
        bool SearchId(string fromUserId, string userId);

        // 接受好友请求
        bool AcceptFriendRequest(string fromUserId);

        // 拒绝好友请求
        bool RejectFriendRequest(string fromUserId);

        // 获取所有好友列表
        List<Friend> SearchAllFriends(string userId);

        // 获取离线消息和好友请求
        List<Message> GetOfflineMessages(out List<string> friendRequests);

        // 获取与指定好友的历史聊天记录
        List<Message> GetHistoryMessages(string friendId);

        // 标记与指定好友的未读消息为已读
        bool MarkMessagesAsRead(string friendId);

        // 根据用户名获取用户详细信息
        User GetUserInfo(string userId);

        // 更新当前用户的个人信息
        bool UpdateUserInfo(User updatedUser);

        // 获取当前用户加入的群组列表
        List<Group> GetGroupList();

        // 发送群聊消息
        bool SendGroupMessage(string groupId, string content);

        // 获取群历史消息
        List<GroupMessage> GetGroupHistory(string groupId, int limit = 50);

        string CreateGroup(string groupName, string description = "");

        // 邀请好友入群
        bool InviteToGroup(string groupId, string invitedUserId);
        // 搜索群
        List<Group> SearchGroups(string keyword);
        // 申请加入群
        bool JoinGroup(string groupId);

        // 事件：收到消息
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        // 事件：连接状态改变
        event EventHandler<ConnectionEventArgs> ConnectionChanged;

        void SendPacket(ChatPacket packet);
    }

    // 当通信端收到新消息时，通过事件通知UI层，并传递消息的详细信息。
    public class MessageReceivedEventArgs : EventArgs
    {
        public ChatPacket Packet { get; set; }
        public MessageReceivedEventArgs(ChatPacket packet)
        {
            Packet = packet;
        }
    }

    // 当连接状态发生变化时，通知UI层更新界面。
    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
        public ConnectionEventArgs(bool isConnected, string message)
        {
            IsConnected = isConnected;
            Message = message;
        }
    }
}
