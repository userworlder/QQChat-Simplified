// Repositories/MessageRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQClient.DataAccess.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        public async Task SaveMessageAsync(Message message)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM Messages WHERE MessageId = @MessageId)
                BEGIN
                    INSERT INTO Messages (MessageId, SenderId, ReceiverId, Content, SendTime, IsRead, MessageType)
                    VALUES (@MessageId, @SenderId, @ReceiverId, @Content, @SendTime, @IsRead, @MessageType)
                END";

            var parameters = new[]
            {
                new SqlParameter("@MessageId", message.MessageId),
                new SqlParameter("@SenderId", message.SenderId),
                new SqlParameter("@ReceiverId", message.ReceiverId),
                new SqlParameter("@Content", message.Content ?? (object)DBNull.Value),
                new SqlParameter("@SendTime", message.SendTime),
                new SqlParameter("@IsRead", message.IsRead),
                new SqlParameter("@MessageType", message.MessageType)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<List<Message>> GetHistoryAsync(string userId, string friendId, int limit = 50)
        {
            const string sql = @"
                SELECT TOP (@Limit) MessageId, SenderId, ReceiverId, Content, SendTime, IsRead, MessageType
                FROM Messages
                WHERE (SenderId = @UserId AND ReceiverId = @FriendId)
                   OR (SenderId = @FriendId AND ReceiverId = @UserId)
                ORDER BY SendTime DESC";

            var parameters = new[]
            {
                new SqlParameter("@Limit", limit),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendId", friendId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            return ConvertDataTableToMessages(dt);
        }

        public async Task MarkAsReadAsync(string userId, string friendId)
        {
            const string sql = @"
                UPDATE Messages 
                SET IsRead = 1 
                WHERE ReceiverId = @UserId 
                  AND SenderId = @FriendId 
                  AND IsRead = 0";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendId", friendId)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM Messages 
                WHERE ReceiverId = @UserId AND IsRead = 0";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId)
            };

            var result = await DbHelper.ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result);
        }

        public async Task<List<Message>> GetUnreadMessagesBySenderAsync(string userId)
        {
            const string sql = @"
                SELECT MessageId, SenderId, ReceiverId, Content, SendTime, IsRead, MessageType
                FROM Messages
                WHERE ReceiverId = @UserId AND IsRead = 0
                ORDER BY SendTime ASC";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            return ConvertDataTableToMessages(dt);
        }

        private List<Message> ConvertDataTableToMessages(DataTable dt)
        {
            var messages = new List<Message>();
            foreach (DataRow row in dt.Rows)
            {
                messages.Add(new Message
                {
                    MessageId = row["MessageId"].ToString(),
                    SenderId = row["SenderId"].ToString(),
                    ReceiverId = row["ReceiverId"].ToString(),
                    Content = row["Content"]?.ToString(),
                    SendTime = Convert.ToDateTime(row["SendTime"]),
                    IsRead = Convert.ToBoolean(row["IsRead"]),
                    MessageType = Convert.ToInt32(row["MessageType"])
                });
            }
            return messages;
        }
    }
}