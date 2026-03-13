using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class FriendDao
    {
        // 添加好友
        public bool AddFriend(Friend friend)
        {
            string sql = "INSERT INTO Friends (FriendId, UserId, FriendUserId, Remark, GroupName, AddTime) " +
                         "VALUES (@FriendId, @UserId, @FriendUserId, @Remark, @GroupName, @AddTime)";
            SqlParameter[] parameters = {
                new SqlParameter("@FriendId", friend.FriendId),
                new SqlParameter("@UserId", friend.UserId),
                new SqlParameter("@FriendUserId", friend.FriendUserId),
                new SqlParameter("@Remark", friend.Remark ?? (object)DBNull.Value),
                new SqlParameter("@GroupName", friend.GroupName),
                new SqlParameter("@AddTime", friend.AddTime)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 获取用户的好友列表
        public List<Friend> GetFriendsByUserId(string userId)
        {
            string sql = "SELECT * FROM Friends WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<Friend> friends = new List<Friend>();
            foreach (DataRow row in dt.Rows)
            {
                friends.Add(DataRowToFriend(row));
            }
            return friends;
        }

        // 根据用户ID和好友用户ID获取好友信息
        public Friend GetFriendByUserIdAndFriendUserId(string userId, string friendUserId)
        {
            string sql = "SELECT * FROM Friends WHERE UserId = @UserId AND FriendUserId = @FriendUserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendUserId)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                return DataRowToFriend(dt.Rows[0]);
            }
            return null;
        }

        // 删除好友
        public bool RemoveFriend(string userId, string friendUserId)
        {
            string sql = "DELETE FROM Friends WHERE UserId = @UserId AND FriendUserId = @FriendUserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendUserId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 更新好友备注
        public bool UpdateFriendRemark(string userId, string friendUserId, string remark)
        {
            string sql = "UPDATE Friends SET Remark = @Remark WHERE UserId = @UserId AND FriendUserId = @FriendUserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendUserId),
                new SqlParameter("@Remark", remark ?? (object)DBNull.Value)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 更新好友分组
        public bool UpdateFriendGroup(string userId, string friendUserId, string groupName)
        {
            string sql = "UPDATE Friends SET GroupName = @GroupName WHERE UserId = @UserId AND FriendUserId = @FriendUserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendUserId),
                new SqlParameter("@GroupName", groupName)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 将DataRow转换为Friend对象
        private Friend DataRowToFriend(DataRow row)
        {
            return new Friend
            {
                FriendId = row["FriendId"].ToString(),
                UserId = row["UserId"].ToString(),
                FriendUserId = row["FriendUserId"].ToString(),
                Remark = row["Remark"] is DBNull ? null : row["Remark"].ToString(),
                GroupName = row["GroupName"].ToString(),
                AddTime = Convert.ToDateTime(row["AddTime"])
            };
        }
    }
}