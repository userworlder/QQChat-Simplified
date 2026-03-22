using QQClient.DataAccess.Repositories;
using QQCommon.Interfaces;
using QQCommon.Protocols;
using System;
using System.Threading.Tasks;

namespace QQClient.Business.Services
{
    //业务服务基类
    public abstract class BaseService
    {
        protected readonly INetworkClient _client;
        protected readonly IMessageRepository _messageRepo;
        protected readonly IFriendRepository _friendRepo;
        protected readonly IGroupRepository _groupRepo;

        protected BaseService(INetworkClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            // 订阅网络客户端的消息事件
            _client.MessageReceived += OnNetworkMessageReceived;
        }

        protected BaseService(INetworkClient client, IMessageRepository messageRepo, IFriendRepository friendRepo, IGroupRepository groupRepo)
            : this(client)
        {
            _messageRepo = messageRepo;
            _friendRepo = friendRepo;
            _groupRepo = groupRepo;
        }

        // 处理网络消息
        private void OnNetworkMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // 先尝试匹配等待中的请求
            if (!RequestManager.HandleResponse(e.Packet))
            {
                // 未匹配，作为推送消息处理
                OnPushMessageReceived(e.Packet);
            }
        }

        //处理推送消息（子类可重写）
        protected virtual void OnPushMessageReceived(ChatPacket packet)
        {
            // 默认实现：触发事件
            OnMessageReceived(packet);
        }

        //发送请求并等待响应
        protected async Task<ChatPacket> SendRequestAsync(ChatPacket packet, int timeoutMs = 10000)
        {
            return await RequestManager.SendRequestAsync(_client, packet, timeoutMs);
        }

        //通用响应验证
        protected bool IsSuccessResponse(ChatPacket response)
        {
            return response != null && response.Content == "SUCCESS";
        }

        //消息接收事件
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        protected virtual void OnMessageReceived(ChatPacket packet)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packet));
        }
    }
}