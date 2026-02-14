-- QQChat 数据库

-- 创建数据库
CREATE DATABASE QQChat;
GO

USE QQChat;
GO

-- Users 表
CREATE TABLE Users (
    UserId NVARCHAR(50) PRIMARY KEY,--用户ID
    Username NVARCHAR(50) UNIQUE NOT NULL,--用户名字
    Password NVARCHAR(100) NOT NULL,--密码
    Nickname NVARCHAR(50) NOT NULL,--昵称
    Avatar NVARCHAR(MAX),--头像路径
    Signature NVARCHAR(200),--个性签名
    IsOnline BIT DEFAULT 0,--在线状态
    LastLoginTime DATETIME,--最后登录时间
    RegisterTime DATETIME NOT NULL DEFAULT GETDATE()--注册时间
);

-- 创建用户名索引，加速搜索
CREATE INDEX IX_Users_Username ON Users(Username);

-- Friends 表
CREATE TABLE Friends (
    FriendId NVARCHAR(50) PRIMARY KEY,--好友ID
    UserId NVARCHAR(50) NOT NULL,--用户ID
    FriendUserId NVARCHAR(50) NOT NULL,--好友关系ID
    Remark NVARCHAR(50),--备注名称
    GroupName NVARCHAR(50) DEFAULT '我的好友',--分组
    AddTime DATETIME NOT NULL DEFAULT GETDATE(),--加入时间
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (FriendUserId) REFERENCES Users(UserId)
);

-- 创建用户ID索引，加速查询好友列表
CREATE INDEX IX_Friends_UserId ON Friends(UserId);

-- Messages 表
CREATE TABLE Messages (
    MessageId NVARCHAR(50) PRIMARY KEY,--消息ID
    SenderId NVARCHAR(50) NOT NULL,--发送者ID
    ReceiverId NVARCHAR(50) NOT NULL,--接受者ID
    Content NVARCHAR(MAX) NOT NULL,--消息内容
    SendTime DATETIME NOT NULL DEFAULT GETDATE(),--发送时间
    IsRead BIT DEFAULT 0,--已读
    MessageType INT NOT NULL DEFAULT 1, -- 1=文本，2=图片，3=文件
    FOREIGN KEY (SenderId) REFERENCES Users(UserId)
);

-- 创建发送者和接收者ID索引，加速消息查询
CREATE INDEX IX_Messages_SenderId ON Messages(SenderId);
CREATE INDEX IX_Messages_ReceiverId ON Messages(ReceiverId);
GO

-- -- 查看表结构
-- EXEC sp_help Users;
-- EXEC sp_help Friends;
-- EXEC sp_help Messages;