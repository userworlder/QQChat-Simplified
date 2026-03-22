using Newtonsoft.Json;
using QQClient.DataAccess.Repositories;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Threading.Tasks;

namespace QQClient.Business.Services
{
    public class UserBusinessService : BaseService, IUserBusinessService
    {
        public UserBusinessService(INetworkClient client) : base(client)
        {
        }

        public UserBusinessService(INetworkClient client, IMessageRepository messageRepo, IFriendRepository friendRepo, IGroupRepository groupRepo)
            : base(client, messageRepo, friendRepo, groupRepo)
        {
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.LoginRequest,
                Sender = username,
                Content = password,
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
                Console.WriteLine("[UserBusinessService] 登录超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserBusinessService] 登录异常: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string password, string nickname)
        {
            var user = new User
            {
                Username = username,
                Password = password,
                Nickname = string.IsNullOrEmpty(nickname) ? username : nickname,
                RegisterTime = DateTime.Now
            };

            var packet = new ChatPacket
            {
                Type = MessageType.RegisterRequest,
                Sender = username,
                Content = JsonConvert.SerializeObject(user),
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
                Console.WriteLine("[UserBusinessService] 注册超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserBusinessService] 注册异常: {ex.Message}");
                return false;
            }
        }

        public async Task<User> GetUserInfoAsync(string userId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetUserInfoRequest,
                Sender = userId,
                Content = userId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await SendRequestAsync(packet);
                if (IsSuccessResponse(response) && response.Extras.TryGetValue("UserInfo", out string json))
                {
                    return JsonConvert.DeserializeObject<User>(json);
                }
                return null;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[UserBusinessService] 获取用户信息超时");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserBusinessService] 获取用户信息异常: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateUserInfoAsync(User updatedUser)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.UpdateUserInfoRequest,
                Sender = updatedUser.Username,
                Content = JsonConvert.SerializeObject(updatedUser),
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
                Console.WriteLine("[UserBusinessService] 更新用户信息超时");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserBusinessService] 更新用户信息异常: {ex.Message}");
                return false;
            }
        }
    }
}