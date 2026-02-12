using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class MessageDao
    {
        // 发送消息（插入消息）
        public bool SendMessage(Message message)
        {
            string sql = "INSERT INTO Messages (MessageId, SenderId, ReceiverId, Content, SendTime, IsRead, MessageType) " +
                         "VALUES (@MessageId, @SenderId, @ReceiverId, @Content, @SendTime, @IsRead, @MessageType)";
            SqlParameter[] parameters = {
                new SqlParameter("@MessageId", message.MessageId),
                new SqlParameter("@SenderId", message.SenderId),
                new SqlParameter("@ReceiverId", message.ReceiverId),
                new SqlParameter("@Content", message.Content),
                new SqlParameter("@SendTime", message.SendTime),
                new SqlParameter("@IsRead", message.IsRead),
                new SqlParameter("@MessageType", message.MessageType)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 获取用户的消息列表（与特定用户的聊天记录）
        public List<Message> GetMessagesBetweenUsers(string userId1, string userId2)
        {
            string sql = "SELECT * FROM Messages WHERE (SenderId = @UserId1 AND ReceiverId = @UserId2) OR (SenderId = @UserId2 AND ReceiverId = @UserId1) ORDER BY SendTime";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId1", userId1),
                new SqlParameter("@UserId2", userId2)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<Message> messages = new List<Message>();
            foreach (DataRow row in dt.Rows)
            {
                messages.Add(DataRowToMessage(row));
            }
            return messages;
        }

        // 标记消息为已读
        public void MarkMessageAsRead(string messageId)
        {
            string sql = "UPDATE Messages SET IsRead = 1 WHERE MessageId = @MessageId";
            SqlParameter[] parameters = {
                new SqlParameter("@MessageId", messageId)
            };

            DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // 将DataRow转换为Message对象
        private Message DataRowToMessage(DataRow row)
        {
            return new Message
            {
                MessageId = row["MessageId"].ToString(),
                SenderId = row["SenderId"].ToString(),
                ReceiverId = row["ReceiverId"].ToString(),
                Content = row["Content"].ToString(),
                SendTime = Convert.ToDateTime(row["SendTime"]),
                IsRead = Convert.ToBoolean(row["IsRead"]),
                MessageType = Convert.ToInt32(row["MessageType"])
            };
        }
    }
}