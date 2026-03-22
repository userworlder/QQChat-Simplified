using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using QQCommon.Models;

namespace QQClient.Business
{
    // 全局缓存管理器（线程安全）
    public static class CacheManager
    {
        // 私聊消息缓存
        private static readonly ConcurrentDictionary<string, List<Message>> _messageCache
            = new ConcurrentDictionary<string, List<Message>>();

        // 群消息缓存
        private static readonly ConcurrentDictionary<string, List<GroupMessage>> _groupMessageCache
            = new ConcurrentDictionary<string, List<GroupMessage>>();

        // 群未读计数
        private static readonly ConcurrentDictionary<string, int> _groupUnreadCount
            = new ConcurrentDictionary<string, int>();

        // 好友未读计数
        private static readonly ConcurrentDictionary<string, int> _friendUnreadCount
            = new ConcurrentDictionary<string, int>();

        // 用户信息缓存
        private static readonly ConcurrentDictionary<string, User> _userCache
            = new ConcurrentDictionary<string, User>();

        #region 私聊消息缓存

        public static void AddMessage(string friendId, Message message)
        {
            _messageCache.AddOrUpdate(friendId,
                new List<Message> { message },
                (key, list) =>
                {
                    lock (list)
                    {
                        list.Add(message);
                        // 保持最近100条消息
                        while (list.Count > 100) list.RemoveAt(0);
                    }
                    return list;
                });
        }

        public static void AddMessages(string friendId, List<Message> messages)
        {
            if (messages == null || messages.Count == 0) return;

            _messageCache.AddOrUpdate(friendId,
                new List<Message>(messages),
                (key, list) =>
                {
                    lock (list)
                    {
                        foreach (var msg in messages)
                        {
                            list.Add(msg);
                        }
                        // 保持最近100条消息
                        while (list.Count > 100) list.RemoveAt(0);
                    }
                    return list;
                });
        }

        public static List<Message> GetMessages(string friendId)
        {
            if (_messageCache.TryGetValue(friendId, out var messages))
            {
                lock (messages)
                {
                    return messages.OrderBy(m => m.SendTime).ToList();
                }
            }
            return new List<Message>();
        }

        public static Message GetLastMessage(string friendId)
        {
            if (_messageCache.TryGetValue(friendId, out var messages))
            {
                lock (messages)
                {
                    return messages.OrderByDescending(m => m.SendTime).FirstOrDefault();
                }
            }
            return null;
        }

        public static int GetFriendUnreadCount(string friendId)
        {
            return _friendUnreadCount.TryGetValue(friendId, out int count) ? count : 0;
        }

        public static void IncrementFriendUnreadCount(string friendId)
        {
            _friendUnreadCount.AddOrUpdate(friendId, 1, (key, old) => old + 1);
        }

        public static void ClearFriendUnreadCount(string friendId)
        {
            _friendUnreadCount.TryRemove(friendId, out _);
        }

        public static void MarkMessagesAsRead(string friendId)
        {
            ClearFriendUnreadCount(friendId);

            if (_messageCache.TryGetValue(friendId, out var messages))
            {
                lock (messages)
                {
                    foreach (var msg in messages)
                    {
                        if (msg.ReceiverId == CurrentUser.UserId && !msg.IsRead)
                        {
                            msg.IsRead = true;
                        }
                    }
                }
            }
        }

        #endregion

        #region 群消息缓存

        public static void AddGroupMessage(string groupId, GroupMessage message)
        {
            _groupMessageCache.AddOrUpdate(groupId,
                new List<GroupMessage> { message },
                (key, list) =>
                {
                    lock (list)
                    {
                        list.Add(message);
                        while (list.Count > 100) list.RemoveAt(0);
                    }
                    return list;
                });
        }

        public static void AddGroupMessages(string groupId, List<GroupMessage> messages)
        {
            if (messages == null || messages.Count == 0) return;

            _groupMessageCache.AddOrUpdate(groupId,
                new List<GroupMessage>(messages),
                (key, list) =>
                {
                    lock (list)
                    {
                        foreach (var msg in messages)
                        {
                            list.Add(msg);
                        }
                        while (list.Count > 100) list.RemoveAt(0);
                    }
                    return list;
                });
        }

        public static List<GroupMessage> GetGroupMessages(string groupId)
        {
            if (_groupMessageCache.TryGetValue(groupId, out var messages))
            {
                lock (messages)
                {
                    return messages.OrderBy(m => m.SendTime).ToList();
                }
            }
            return new List<GroupMessage>();
        }

        public static GroupMessage GetLastGroupMessage(string groupId)
        {
            if (_groupMessageCache.TryGetValue(groupId, out var messages))
            {
                lock (messages)
                {
                    return messages.OrderByDescending(m => m.SendTime).FirstOrDefault();
                }
            }
            return null;
        }

        public static int GetGroupUnreadCount(string groupId)
        {
            return _groupUnreadCount.TryGetValue(groupId, out int count) ? count : 0;
        }

        public static void IncrementGroupUnreadCount(string groupId)
        {
            _groupUnreadCount.AddOrUpdate(groupId, 1, (key, old) => old + 1);
        }

        public static void ClearGroupUnreadCount(string groupId)
        {
            _groupUnreadCount.TryRemove(groupId, out _);
        }

        #endregion

        #region 用户信息缓存

        public static void AddUser(User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.UserId))
            {
                _userCache.AddOrUpdate(user.UserId, user, (key, old) => user);
            }
        }

        public static User GetUser(string userId)
        {
            return _userCache.TryGetValue(userId, out var user) ? user : null;
        }

        #endregion

        #region 清理方法

        public static void ClearAll()
        {
            _messageCache.Clear();
            _groupMessageCache.Clear();
            _groupUnreadCount.Clear();
            _friendUnreadCount.Clear();
            _userCache.Clear();
        }

        public static void ClearUserData(string userId)
        {
            // 清理与指定用户相关的所有缓存
            var keysToRemove = _messageCache.Keys.Where(k => k == userId).ToList();
            foreach (var key in keysToRemove)
            {
                _messageCache.TryRemove(key, out _);
            }

            _friendUnreadCount.TryRemove(userId, out _);
            _userCache.TryRemove(userId, out _);
        }

        #endregion
    }
}