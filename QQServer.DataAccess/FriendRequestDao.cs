using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class FriendRequestDao
    {
        // 发送好友请求
        public bool AddFriendRequest(string fromUserId, string toUserId)
        {
            string sql = "INSERT INTO FriendRequests (RequestId, FromUserId, ToUserId, Status, SendTime) " +
                         "VALUES (@RequestId, @FromUserId, @ToUserId, @Status, @SendTime)";

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RequestId", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@FromUserId", fromUserId);
                    cmd.Parameters.AddWithValue("@ToUserId", toUserId);
                    cmd.Parameters.AddWithValue("@Status", 0);
                    cmd.Parameters.AddWithValue("@SendTime", DateTime.Now);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        // 获取用户的好友请求列表
        public List<string> GetFriendRequests(string userId)
        {
            string sql = "SELECT FromUserId FROM FriendRequests WHERE ToUserId = @UserId AND Status = 0";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<string> requestIds = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                requestIds.Add(row["FromUserId"].ToString());
            }
            return requestIds;
        }

        // 接受好友请求
        public bool AcceptFriendRequest(string fromUserId, string toUserId)
        {
            string sql = "UPDATE FriendRequests SET Status = 1 WHERE FromUserId = @FromUserId AND ToUserId = @ToUserId AND Status = 0";
            SqlParameter[] parameters = {
                new SqlParameter("@FromUserId", fromUserId),
                new SqlParameter("@ToUserId", toUserId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 拒绝好友请求
        public bool RejectFriendRequest(string fromUserId, string toUserId)
        {
            string sql = "UPDATE FriendRequests SET Status = 2 WHERE FromUserId = @FromUserId AND ToUserId = @ToUserId AND Status = 0";
            SqlParameter[] parameters = {
                new SqlParameter("@FromUserId", fromUserId),
                new SqlParameter("@ToUserId", toUserId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }
}