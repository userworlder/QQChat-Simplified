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
    public class GroupBusinessService : BaseService, IGroupBusinessService
    {
        private string _currentUserId;

        public GroupBusinessService(INetworkClient client) : base(client)
        {
        }

        public GroupBusinessService(INetworkClient client, IMessageRepository messageRepo, IFriendRepository friendRepo, IGroupRepository groupRepo, string currentUserId)
            : base(client, messageRepo, friendRepo, groupRepo)
        {
            _currentUserId = currentUserId;
        }

        public void SetCurrentUserId(string userId)
        {
            _currentUserId = userId;
        }

        public async Task<List<Group>> GetGroupListAsync()
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetGroupListRequest,
                Sender = _currentUserId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response) && response.Extras.TryGetValue("GroupList", out string json))
                {
                    var groups = JsonConvert.DeserializeObject<List<Group>>(json);

                    // 保存到本地数据库
                    if (_groupRepo != null && groups != null)
                    {
                        foreach (var group in groups)
                        {
                            await _groupRepo.SaveGroupAsync(group);
                        }
                    }

                    return groups ?? new List<Group>();
                }
                return new List<Group>();
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[GroupBusinessService] 获取群列表超时");
                return new List<Group>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 获取群列表异常: {ex.Message}");
                return new List<Group>();
            }
        }
        public int GetGroupUnreadCount(string groupId)
        {
            return CacheManager.GetGroupUnreadCount(groupId);
        }
        public GroupMessage GetLastGroupMessage(string groupId)
        {
            return CacheManager.GetLastGroupMessage(groupId);
        }

        public async Task<bool> SendGroupMessageAsync(string groupId, string content)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GroupChatMessage,
                Sender = _currentUserId,
                Receiver = groupId,
                Content = content,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                _client.SendPacket(packet);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 发送群消息异常: {ex.Message}");
                return false;
            }
        }

        public async Task<List<GroupMessage>> GetGroupHistoryAsync(string groupId, int limit = 50)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetGroupHistoryRequest,
                Sender = _currentUserId,
                Content = groupId,
                Extras = new Dictionary<string, string> { ["Limit"] = limit.ToString() },
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response) && response.Extras.TryGetValue("GroupMessages", out string json))
                {
                    return JsonConvert.DeserializeObject<List<GroupMessage>>(json) ?? new List<GroupMessage>();
                }
                return new List<GroupMessage>();
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[GroupBusinessService] 获取群历史消息超时");
                return new List<GroupMessage>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 获取群历史消息异常: {ex.Message}");
                return new List<GroupMessage>();
            }
        }

        public async Task<string> CreateGroupAsync(string groupName, string description = "")
        {
            var data = new { GroupName = groupName, Description = description };
            var packet = new ChatPacket
            {
                Type = MessageType.CreateGroupRequest,
                Sender = _currentUserId,
                Content = JsonConvert.SerializeObject(data),
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response) && response.Extras.TryGetValue("GroupId", out string groupId))
                {
                    return groupId;
                }
                return null;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[GroupBusinessService] 创建群组超时");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 创建群组异常: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> InviteToGroupAsync(string groupId, string invitedUserId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.InviteToGroupRequest,
                Sender = _currentUserId,
                Receiver = groupId,
                Content = invitedUserId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                return IsSuccessResponse(response);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[GroupBusinessService] 邀请入群超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 邀请入群异常: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Group>> SearchGroupsAsync(string keyword)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.SearchGroupRequest,
                Sender = _currentUserId,
                Content = keyword,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response) && response.Extras.TryGetValue("Groups", out string json))
                {
                    return JsonConvert.DeserializeObject<List<Group>>(json) ?? new List<Group>();
                }
                return new List<Group>();
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[GroupBusinessService] 搜索群组超时");
                return new List<Group>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 搜索群组异常: {ex.Message}");
                return new List<Group>();
            }
        }

        public async Task<bool> JoinGroupAsync(string groupId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.JoinGroupRequest,
                Sender = _currentUserId,
                Content = groupId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                return IsSuccessResponse(response);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[GroupBusinessService] 加入群组超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 加入群组异常: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> LeaveGroupAsync(string groupId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.LeaveGroupRequest,
                Sender = _currentUserId,
                Content = groupId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                return IsSuccessResponse(response);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[GroupBusinessService] 退出群组超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GroupBusinessService] 退出群组异常: {ex.Message}");
                return false;
            }
        }
        // 实现 IGroupBusinessService 接口的其他方法（如果需要）
        public async Task SaveGroupAsync(Group group)
        {
            if (_groupRepo != null)
            {
                await _groupRepo.SaveGroupAsync(group);
            }
        }

        public async Task<List<Group>> GetGroupsAsync(string userId)
        {
            if (_groupRepo != null)
            {
                return await _groupRepo.GetGroupsAsync(userId);
            }
            return new List<Group>();
        }

        public async Task<Group> GetGroupByIdAsync(string groupId)
        {
            if (_groupRepo != null)
            {
                return await _groupRepo.GetGroupByIdAsync(groupId);
            }
            return null;
        }

        public async Task SaveGroupMessageAsync(GroupMessage message)
        {
            if (_groupRepo != null)
            {
                await _groupRepo.SaveGroupMessageAsync(message);
            }
        }

        public async Task SaveGroupMemberAsync(GroupMember member)
        {
            if (_groupRepo != null)
            {
                await _groupRepo.SaveGroupMemberAsync(member);
            }
        }

        public async Task<List<GroupMember>> GetGroupMembersAsync(string groupId)
        {
            if (_groupRepo != null)
            {
                return await _groupRepo.GetGroupMembersAsync(groupId);
            }
            return new List<GroupMember>();
        }

        public async Task<bool> IsGroupMemberAsync(string groupId, string userId)
        {
            if (_groupRepo != null)
            {
                return await _groupRepo.IsGroupMemberAsync(groupId, userId);
            }
            return false;
        }

        public async Task<bool> RemoveGroupMemberAsync(string groupId, string userId)
        {
            if (_groupRepo != null)
            {
                return await _groupRepo.RemoveGroupMemberAsync(groupId, userId);
            }
            return false;
        }

        public async Task<int> GetGroupMemberCountAsync(string groupId)
        {
            if (_groupRepo != null)
            {
                return await _groupRepo.GetGroupMemberCountAsync(groupId);
            }
            return 0;
        }

        protected override void OnPushMessageReceived(ChatPacket packet)
        {
            if (packet.Type == MessageType.GroupChatMessage)
            {
                var message = new GroupMessage
                {
                    MessageId = packet.MessageId,
                    GroupId = packet.Receiver,
                    SenderId = packet.Sender,
                    Content = packet.Content,
                    SendTime = packet.Timestamp,
                    MessageType = 1
                };

                // 保存到本地数据库
                if (_groupRepo != null)
                {
                    Task.Run(async () => await _groupRepo.SaveGroupMessageAsync(message));
                }

                // 更新缓存
                CacheManager.AddGroupMessage(packet.Receiver, message);

                // 更新未读计数（如果不是自己发送的）
                if (packet.Sender != _currentUserId)
                {
                    CacheManager.IncrementGroupUnreadCount(packet.Receiver);
                }

                OnMessageReceived(packet);
            }
            else if (packet.Type == MessageType.GroupJoinRequestNotification)
            {
                OnMessageReceived(packet);
            }
            else
            {
                base.OnPushMessageReceived(packet);
            }
        }
    }
}