using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Threading.Tasks;

namespace QQClient.Business.Services
{
    // 用户相关业务服务接口

    public interface IUserBusinessService
    {
    
        // 登录
        Task<bool> LoginAsync(string username, string password);

    
        //注册
    
        Task<bool> RegisterAsync(string username, string password, string nickname);

    
        // 获取用户信息
    
        Task<User> GetUserInfoAsync(string userId);

    
        // 更新用户信息
    
        Task<bool> UpdateUserInfoAsync(User updatedUser);

    
        // 推送消息事件（如收到系统通知、好友请求等）
    
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}