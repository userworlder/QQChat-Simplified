using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Protocols
{
    public enum MessageType
    {
        // 用户相关 (100系列)
        LoginRequest = 101,//登录请求
        LoginResponse = 102,
        RegisterRequest = 103,
        RegisterResponse = 104,
        UserInfoRequest = 105,//获取信息请求
        UserInfoResponse = 106,

        // 好友相关 (200系列)
        AddFriendRequest = 201,
        AddFriendResponse = 202,
        FriendListRequest = 203,
        FriendListResponse = 204,
        FriendStatusUpdate = 205,//好友列表更新

        // 消息相关 (300系列)
        ChatMessage = 301,
        MessageReceived = 302,  // 消息送达确认
        MessageRead = 303,      // 消息已读确认

        // 系统消息 (400系列)
        Heartbeat = 401,        // 心跳包（保证连接不中断）
        Disconnect = 402,
        Error = 403
    }
}
