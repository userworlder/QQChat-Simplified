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
        // 用户信息获取与更新
        GetUserInfoRequest = 114,
        GetUserInfoResponse = 115,
        UpdateUserInfoRequest = 116,
        UpdateUserInfoResponse = 117,
        // 好友相关 (200系列)
        AddFriendRequest = 201,
        AddFriendResponse = 202,
        FriendListRequest = 203,
        FriendListResponse = 204,
        FriendStatusUpdate = 205,//好友列表更新
        AcceptFriendRequest = 206,
        AcceptFriendResponse = 207,
        RejectFriendRequest = 208,
        RejectFriendResponse = 209,

        // 消息相关 (300系列)
        ChatMessage = 301,
        MessageReceived = 302,  // 消息送达确认
        MessageRead = 303,      // 消息已读确认
        SearchId=304,
        SearchIdResponse=305,
        SearchAllFriendsRequest=306,
        SearchAllFriendsResponse=307,
        GetOfflineMessagesRequest = 308,   // 客户端请求离线消息
        GetOfflineMessagesResponse = 309,    // 服务器返回离线数据
        // 历史消息
        GetHistoryMessagesRequest = 310,
        GetHistoryMessagesResponse = 311,
        // 标记已读
        MarkMessagesReadRequest = 312,
        MarkMessagesReadResponse = 313,

        // 系统消息 (400系列)
        Heartbeat = 401,        // 心跳包（保证连接不中断）
        Disconnect = 402,
        Error = 403,

        GetGroupListRequest = 501,          // 请求获取群列表
        GetGroupListResponse = 502,         // 返回群列表
        GroupChatMessage = 503,             // 群聊消息（发送/接收）
        GetGroupHistoryRequest = 504,       // 请求获取群历史消息
        GetGroupHistoryResponse = 505,      // 返回群历史消息
                                            // 群聊创建
        CreateGroupRequest = 506,
        CreateGroupResponse = 507,
        InviteToGroupRequest = 508,
        InviteToGroupResponse = 509,
        SearchGroupRequest = 510,
        SearchGroupResponse = 511,
        JoinGroupRequest = 512,
        JoinGroupResponse = 513,
        GroupJoinRequestNotification = 514,
        GroupJoinRequestResponse = 515,
        // 群成员变更
        GroupMemberChanged =516,
        // 群信息变更
        GroupInfoChanged = 517,
        // 删除好友请求/响应
        RemoveFriendRequest = 518,
        RemoveFriendResponse = 519,
        // 退出群组
        LeaveGroupRequest = 520,
        LeaveGroupResponse = 521
    }
}
