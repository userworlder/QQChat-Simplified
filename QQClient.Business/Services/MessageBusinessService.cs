using Newtonsoft.Json;
using QQClient.DataAccess.Repositories;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQClient.Business.Services
{
    public class MessageBusinessService : BaseService, IMessageBusinessService
    {
        private string _currentUserId;

        public MessageBusinessService(INetworkClient client) : base(client)
        {
        }

        public MessageBusinessService(INetworkClient client, IMessageRepository messageRepo, IFriendRepository friendRepo, IGroupRepository groupRepo, string currentUserId)
                    : base(client, messageRepo, friendRepo, groupRepo)
        {
            _currentUserId = currentUserId;
            if (!string.IsNullOrEmpty(_currentUserId))
            {
                CurrentUser.UserId = _currentUserId;
                CurrentUser.Username = _currentUserId;
            }
        }

        public void SetCurrentUserId(string userId)
        {
            _currentUserId = userId;
            CurrentUser.UserId = userId;
            CurrentUser.Username = userId;
        }

        public async Task<bool> SendMessageAsync(string receiver, string content)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.ChatMessage,
                Sender = _currentUserId,
                Receiver = receiver,
                Content = content,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                _client.SendPacket(packet);

                var message = new Message
                {
                    MessageId = packet.MessageId,
                    SenderId = _currentUserId,
                    ReceiverId = receiver,
                    Content = content,
                    SendTime = packet.Timestamp,
                    IsRead = true,
                    MessageType = 1
                };

                // 保存到本地数据库
                if (_messageRepo != null)
                {
                    await _messageRepo.SaveMessageAsync(message);
                }

                // 更新缓存
                CacheManager.AddMessage(receiver, message);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageBusinessService] 发送消息异常: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Message>> GetHistoryMessagesAsync(string friendId)
        {
            // 先尝试从缓存获取
            var cachedMessages = CacheManager.GetMessages(friendId);
            if (cachedMessages.Count > 0)
            {
                return cachedMessages;
            }

            var packet = new ChatPacket
            {
                Type = MessageType.GetHistoryMessagesRequest,
                Sender = _currentUserId,
                Content = friendId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response) && response.Extras.TryGetValue("Messages", out string json))
                {
                    var messages = JsonConvert.DeserializeObject<List<Message>>(json);

                    // 保存到本地数据库
                    if (_messageRepo != null && messages != null)
                    {
                        foreach (var msg in messages)
                        {
                            await _messageRepo.SaveMessageAsync(msg);
                        }
                    }

                    // 更新缓存
                    if (messages != null && messages.Count > 0)
                    {
                        CacheManager.AddMessages(friendId, messages);
                    }

                    return messages ?? new List<Message>();
                }
                return new List<Message>();
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[MessageBusinessService] 获取历史消息超时");
                // 从本地数据库读取
                if (_messageRepo != null)
                {
                    var dbMessages = await _messageRepo.GetHistoryAsync(_currentUserId, friendId);
                    CacheManager.AddMessages(friendId, dbMessages);
                    return dbMessages;
                }
                return new List<Message>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageBusinessService] 获取历史消息异常: {ex.Message}");
                return new List<Message>();
            }
        }

        public async Task<bool> MarkMessagesAsReadAsync(string friendId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.MarkMessagesReadRequest,
                Sender = _currentUserId,
                Content = friendId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response))
                {
                    // 更新缓存
                    CacheManager.MarkMessagesAsRead(friendId);

                    if (_messageRepo != null)
                    {
                        await _messageRepo.MarkAsReadAsync(_currentUserId, friendId);
                    }
                    return true;
                }
                return false;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[MessageBusinessService] 标记已读超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageBusinessService] 标记已读异常: {ex.Message}");
                return false;
            }
        }

        public async Task<(List<Message> messages, List<string> friendRequests)> GetOfflineMessagesAsync()
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetOfflineMessagesRequest,
                Sender = _currentUserId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response))
                {
                    List<Message> messages = null;
                    List<string> friendRequests = null;

                    if (response.Extras.TryGetValue("OfflineMessages", out string msgJson))
                    {
                        messages = JsonConvert.DeserializeObject<List<Message>>(msgJson);

                        // 保存到本地数据库和缓存
                        if (_messageRepo != null && messages != null)
                        {
                            foreach (var msg in messages)
                            {
                                await _messageRepo.SaveMessageAsync(msg);

                                // 更新缓存和未读计数
                                string otherId = msg.SenderId == _currentUserId ? msg.ReceiverId : msg.SenderId;
                                CacheManager.AddMessage(otherId, msg);

                                if (!msg.IsRead && msg.ReceiverId == _currentUserId)
                                {
                                    CacheManager.IncrementFriendUnreadCount(otherId);
                                }
                            }
                        }
                    }

                    if (response.Extras.TryGetValue("FriendRequests", out string reqJson))
                    {
                        friendRequests = JsonConvert.DeserializeObject<List<string>>(reqJson);
                    }

                    return (messages ?? new List<Message>(), friendRequests ?? new List<string>());
                }
                return (new List<Message>(), new List<string>());
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[MessageBusinessService] 获取离线消息超时");
                return (new List<Message>(), new List<string>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageBusinessService] 获取离线消息异常: {ex.Message}");
                return (new List<Message>(), new List<string>());
            }
        }

        // 获取好友的未读消息数（供UI调用）
        public int GetFriendUnreadCount(string friendId)
        {
            return CacheManager.GetFriendUnreadCount(friendId);
        }

        // 获取好友的最后消息（供UI调用）
        public Message GetLastMessage(string friendId)
        {
            return CacheManager.GetLastMessage(friendId);
        }

        protected override void OnPushMessageReceived(ChatPacket packet)
        {
            if (packet.Type == MessageType.ChatMessage)
            {
                var message = new Message
                {
                    MessageId = packet.MessageId,
                    SenderId = packet.Sender,
                    ReceiverId = packet.Receiver,
                    Content = packet.Content,
                    SendTime = packet.Timestamp,
                    IsRead = false,
                    MessageType = 1
                };

                // 保存到本地数据库
                if (_messageRepo != null)
                {
                    Task.Run(async () => await _messageRepo.SaveMessageAsync(message));
                }

                // 更新缓存和未读计数
                string otherId = packet.Sender == _currentUserId ? packet.Receiver : packet.Sender;
                CacheManager.AddMessage(otherId, message);

                if (packet.Receiver == _currentUserId)
                {
                    CacheManager.IncrementFriendUnreadCount(packet.Sender);
                }

                OnMessageReceived(packet);
            }
            else
            {
                base.OnPushMessageReceived(packet);
            }
        }
    }
}
    
