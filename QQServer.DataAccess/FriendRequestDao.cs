using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class FriendRequestDao
    {
        public bool AddFriendRequest(string fromUserName, string toUserName)
        {
            string sql = @"INSERT INTO FriendRequests (RequestId, FromUserName, ToUserName, Status, SendTime) 
                   VALUES (@RequestId, @FromUserName, @ToUserName, @Status, @SendTime)";

            // 使用 AddWithValue 方式，简单直接
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RequestId", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@FromUserName", fromUserName);
                    cmd.Parameters.AddWithValue("@ToUserName", toUserName);
                    cmd.Parameters.AddWithValue("@Status", 0);  // 直接传整数
                    cmd.Parameters.AddWithValue("@SendTime", DateTime.Now);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        public List<string> GetFriendRequests(string userName)
        {
            string sql = "SELECT FromUserName FROM FriendRequests WHERE ToUserName = @UserName AND Status = 0";
            SqlParameter[] parameters = { new SqlParameter("@UserName", userName) };
            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<string> result = new List<string>();
            foreach (DataRow row in dt.Rows)
                result.Add(row["FromUserName"].ToString());
            return result;
        }

        public bool AcceptFriendRequest(string fromUserName, string toUserName)
        {
            string sql = "UPDATE FriendRequests SET Status = 1 WHERE FromUserName = @FromUserName AND ToUserName = @ToUserName AND Status = 0";
            SqlParameter[] parameters = {
                new SqlParameter("@FromUserName", fromUserName),
                new SqlParameter("@ToUserName", toUserName)
            };
            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool RejectFriendRequest(string fromUserName, string toUserName)
        {
            string sql = "UPDATE FriendRequests SET Status = 2 WHERE FromUserName = @FromUserName AND ToUserName = @ToUserName AND Status = 0";
            SqlParameter[] parameters = {
                new SqlParameter("@FromUserName", fromUserName),
                new SqlParameter("@ToUserName", toUserName)
            };
            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }
    }
}