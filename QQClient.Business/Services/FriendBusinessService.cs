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
    /// <summary>
    /// 好友相关的业务服务实现类
    /// 负责处理好友搜索、添加、接受/拒绝请求、获取好友列表等业务逻辑
    /// 继承自 BaseService，拥有网络通信和本地数据访问能力
    /// </summary>
    public class FriendBusinessService : BaseService, IFriendBusinessService
    {
        // 构造函数1：仅依赖网络客户端（用于不需要本地数据库的场景）
        public FriendBusinessService(INetworkClient client) : base(client)
        {
        }

        // 构造函数2：同时依赖网络客户端和各类仓储（用于需要本地持久化的场景）
        public FriendBusinessService(INetworkClient client, IMessageRepository messageRepo, IFriendRepository friendRepo, IGroupRepository groupRepo)
            : base(client, messageRepo, friendRepo, groupRepo)
        {
        }

        /// <summary>
        /// 搜索用户（用于添加好友前验证对方账号是否存在）
        /// </summary>
        /// <param name="currentUserId">当前登录用户ID</param>
        /// <param name="targetUserId">要搜索的目标用户ID</param>
        /// <returns>用户存在返回 true，否则返回 false</returns>
        public async Task<bool> SearchUserAsync(string currentUserId, string targetUserId)
        {
            // 构造请求包：类型为搜索用户
            var packet = new ChatPacket
            {
                Type = MessageType.SearchId,
                Sender = currentUserId,
                Content = targetUserId,
                MessageId = Guid.NewGuid().ToString(),  // 唯一标识，用于匹配响应
                Timestamp = DateTime.Now
            };

            try
            {
                // 发送请求并等待响应（基类方法）
                var response = await SendRequestAsync(packet);
                // 响应成功则返回 true（服务端返回 "SUCCESS" 表示用户存在）
                return IsSuccessResponse(response);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[FriendBusinessService] 搜索用户超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FriendBusinessService] 搜索用户异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 发送好友申请
        /// </summary>
        /// <param name="fromUserId">发起申请的用户ID</param>
        /// <param name="toUserId">目标用户ID</param>
        /// <returns>申请发送成功返回 true，否则 false</returns>
        public async Task<bool> AddFriendAsync(string fromUserId, string toUserId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.AddFriendRequest,
                Sender = fromUserId,
                Content = toUserId,          // 内容为目标用户ID
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
                Console.WriteLine("[FriendBusinessService] 添加好友超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FriendBusinessService] 添加好友异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 接受好友申请
        /// </summary>
        /// <param name="fromUserId">申请发送方的用户ID</param>
        /// <returns>接受成功返回 true，否则 false</returns>
        public async Task<bool> AcceptFriendRequestAsync(string fromUserId)
        {
            // 注意：这里 Sender 设置为 fromUserId，可能有问题。
            // 根据协议，应该是当前登录用户（接受者）发送接受请求，但这里用了申请者的ID。
            // 需要确认服务端实现。建议改为当前用户ID。
            var packet = new ChatPacket
            {
                Type = MessageType.AcceptFriendRequest,
                Sender = fromUserId,        // 待修正：应该是当前用户ID
                Content = fromUserId,       // 内容为申请者ID
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
                Console.WriteLine("[FriendBusinessService] 接受好友请求超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FriendBusinessService] 接受好友请求异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 拒绝好友申请
        /// </summary>
        /// <param name="fromUserId">申请发送方的用户ID</param>
        /// <returns>拒绝成功返回 true，否则 false</returns>
        public async Task<bool> RejectFriendRequestAsync(string fromUserId)
        {
            // 同样，Sender 应使用当前用户ID
            var packet = new ChatPacket
            {
                Type = MessageType.RejectFriendRequest,
                Sender = fromUserId,        // 待修正：应该是当前用户ID
                Content = fromUserId,
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
                Console.WriteLine("[FriendBusinessService] 拒绝好友请求超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FriendBusinessService] 拒绝好友请求异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取当前用户的好友列表
        /// 优先从服务器获取最新列表，失败时从本地数据库读取
        /// </summary>
        /// <param name="userId">当前用户ID</param>
        /// <returns>好友列表，可能为空列表</returns>
        public async Task<List<Friend>> GetFriendListAsync(string userId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.SearchAllFriendsRequest,
                Sender = userId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                // 成功响应且包含好友列表JSON
                if (IsSuccessResponse(response) && response.Extras.TryGetValue("FriendsList", out string json))
                {
                    var friends = JsonConvert.DeserializeObject<List<Friend>>(json);

                    // 将获取到的好友列表保存到本地数据库，供离线使用
                    if (_friendRepo != null && friends != null)
                    {
                        foreach (var friend in friends)
                        {
                            await _friendRepo.SaveFriendAsync(friend);
                        }
                    }

                    return friends ?? new List<Friend>();
                }
                return new List<Friend>();
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[FriendBusinessService] 获取好友列表超时");
                // 超时则从本地数据库读取缓存的列表
                if (_friendRepo != null)
                {
                    return await _friendRepo.GetFriendsAsync(userId);
                }
                return new List<Friend>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FriendBusinessService] 获取好友列表异常: {ex.Message}");
                return new List<Friend>();
            }
        }

        /// <summary>
        /// 从本地数据库获取指定好友的信息（不经过网络）
        /// </summary>
        /// <param name="userId">当前用户ID</param>
        /// <param name="friendId">好友ID</param>
        /// <returns>好友信息，不存在返回 null</returns>
        public async Task<Friend> GetFriendAsync(string userId, string friendId)
        {
            if (_friendRepo != null)
            {
                return await _friendRepo.GetFriendAsync(userId, friendId);
            }
            return null;
        }

        /// <summary>
        /// 判断两人是否为好友关系（从本地数据库查询）
        /// </summary>
        /// <param name="userId">当前用户ID</param>
        /// <param name="friendId">待判断的用户ID</param>
        /// <returns>是好友返回 true，否则 false</returns>
        public async Task<bool> IsFriendAsync(string userId, string friendId)
        {
            if (_friendRepo != null)
            {
                return await _friendRepo.IsFriendAsync(userId, friendId);
            }
            return false;
        }

        /// <summary>
        /// 获取好友数量（从本地数据库统计）
        /// </summary>
        /// <param name="userId">当前用户ID</param>
        /// <returns>好友数量</returns>
        public async Task<int> GetFriendsCountAsync(string userId)
        {
            if (_friendRepo != null)
            {
                return await _friendRepo.GetFriendsCountAsync(userId);
            }
            return 0;
        }

        /// <summary>
        /// 处理来自网络的推送消息（非请求响应）
        /// 重写基类方法，处理好友请求推送
        /// </summary>
        /// <param name="packet">收到的数据包</param>
        protected override void OnPushMessageReceived(ChatPacket packet)
        {
            // 如果是好友请求消息，则触发业务事件，供上层UI处理
            if (packet.Type == MessageType.AddFriendRequest)
            {
                OnMessageReceived(packet);
            }
            else
            {
                // 其他类型的推送交给基类处理
                base.OnPushMessageReceived(packet);
            }
        }
    }
}