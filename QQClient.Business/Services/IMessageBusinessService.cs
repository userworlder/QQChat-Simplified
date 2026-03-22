using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQClient.Business.Services
{
    public interface IMessageBusinessService
    {

        //发送私聊消息

        Task<bool> SendMessageAsync(string receiver, string content);


        //获取与指定好友的历史消息（从本地数据库或服务器）

        Task<List<Message>> GetHistoryMessagesAsync(string friendId);


        //标记与指定好友的未读消息为已读

        Task<bool> MarkMessagesAsReadAsync(string friendId);


        //获取离线消息和好友请求

        Task<(List<Message> messages, List<string> friendRequests)> GetOfflineMessagesAsync();


        //推送事件（收到新私聊消息）

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}