using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class GroupMessageDao
    {
        // 删除群组的所有消息
        public bool DeleteGroupMessages(string groupId)
        {
            string sql = "DELETE FROM GroupMessages WHERE GroupId = @GroupId";
            SqlParameter[] parameters = { new SqlParameter("@GroupId", groupId) };
            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
        // 发送群消息
        public bool SendGroupMessage(GroupMessage message)
        {
            string sql = "INSERT INTO GroupMessages (MessageId, GroupId, SenderId, Content, SendTime, MessageType) " +
                         "VALUES (@MessageId, @GroupId, @SenderId, @Content, @SendTime, @MessageType)";
            SqlParameter[] parameters = {
                new SqlParameter("@MessageId", message.MessageId),
                new SqlParameter("@GroupId", message.GroupId),
                new SqlParameter("@SenderId", message.SenderId),
                new SqlParameter("@Content", message.Content),
                new SqlParameter("@SendTime", message.SendTime),
                new SqlParameter("@MessageType", message.MessageType)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 获取群消息列表
        public List<GroupMessage> GetGroupMessagesByGroupId(string groupId, int limit = 100)
        {
            string sql = @"
        SELECT * FROM GroupMessages 
        WHERE GroupId = @GroupId 
        ORDER BY SendTime DESC 
        OFFSET 0 ROWS 
        FETCH NEXT @Limit ROWS ONLY";

            SqlParameter[] parameters = {
        new SqlParameter("@Limit", limit),
        new SqlParameter("@GroupId", groupId)
    };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<GroupMessage> messages = new List<GroupMessage>();
            foreach (DataRow row in dt.Rows)
            {
                messages.Add(DataRowToGroupMessage(row));
            }
            // 反转列表，使消息按时间顺序排列（如果需要升序）
            messages.Reverse();
            return messages;
        }

        // 获取群消息列表（带时间戳）
        public List<GroupMessage> GetGroupMessagesByGroupIdAndTime(string groupId, DateTime startTime, int limit = 100)
        {
            string sql = "SELECT TOP @Limit * FROM GroupMessages WHERE GroupId = @GroupId AND SendTime > @StartTime ORDER BY SendTime ASC";
            SqlParameter[] parameters = {
                new SqlParameter("@Limit", limit),
                new SqlParameter("@GroupId", groupId),
                new SqlParameter("@StartTime", startTime)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<GroupMessage> messages = new List<GroupMessage>();
            foreach (DataRow row in dt.Rows)
            {
                messages.Add(DataRowToGroupMessage(row));
            }
            return messages;
        }

        // 将DataRow转换为GroupMessage对象
        private GroupMessage DataRowToGroupMessage(DataRow row)
        {
            return new GroupMessage
            {
                MessageId = row["MessageId"].ToString(),
                GroupId = row["GroupId"].ToString(),
                SenderId = row["SenderId"].ToString(),
                Content = row["Content"].ToString(),
                SendTime = Convert.ToDateTime(row["SendTime"]),
                MessageType = Convert.ToInt32(row["MessageType"])
            };
        }
    }
}