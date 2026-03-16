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
            string sql = @"INSERT INTO Friends 
                (UserName, FriendUserName, FriendNickName, Remark, GroupName, AddTime) 
                VALUES 
                (@UserName, @FriendUserName, @FriendNickName, @Remark, @GroupName, @AddTime)";

            SqlParameter[] parameters = {
                new SqlParameter("@UserName", friend.UserName),
                new SqlParameter("@FriendUserName", friend.FriendUserName),
                new SqlParameter("@FriendNickName", friend.FriendNickName ?? (object)DBNull.Value),
                new SqlParameter("@Remark", friend.Remark ?? (object)DBNull.Value),
                new SqlParameter("@GroupName", friend.GroupName),
                new SqlParameter("@AddTime", friend.AddTime)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 获取用户的好友列表
        public List<Friend> GetFriendsByUserName(string userName)
        {
            string sql = "SELECT * FROM Friends WHERE UserName = @UserName";
            SqlParameter[] parameters = {
                new SqlParameter("@UserName", userName)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<Friend> friends = new List<Friend>();
            foreach (DataRow row in dt.Rows)
            {
                friends.Add(DataRowToFriend(row));
            }
            return friends;
        }

        // 根据用户名和好友用户名获取好友信息
        public Friend GetFriendByUserNames(string userName, string friendUserName)
        {
            string sql = "SELECT * FROM Friends WHERE UserName = @UserName AND FriendUserName = @FriendUserName";
            SqlParameter[] parameters = {
                new SqlParameter("@UserName", userName),
                new SqlParameter("@FriendUserName", friendUserName)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                return DataRowToFriend(dt.Rows[0]);
            }
            return null;
        }

        // 删除好友
        public bool RemoveFriend(string userName, string friendUserName)
        {
            string sql = "DELETE FROM Friends WHERE UserName = @UserName AND FriendUserName = @FriendUserName";
            SqlParameter[] parameters = {
                new SqlParameter("@UserName", userName),
                new SqlParameter("@FriendUserName", friendUserName)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 更新好友备注
        public bool UpdateFriendRemark(string userName, string friendUserName, string remark)
        {
            string sql = "UPDATE Friends SET Remark = @Remark WHERE UserName = @UserName AND FriendUserName = @FriendUserName";
            SqlParameter[] parameters = {
                new SqlParameter("@UserName", userName),
                new SqlParameter("@FriendUserName", friendUserName),
                new SqlParameter("@Remark", remark ?? (object)DBNull.Value)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 更新好友分组
        public bool UpdateFriendGroup(string userName, string friendUserName, string groupName)
        {
            string sql = "UPDATE Friends SET GroupName = @GroupName WHERE UserName = @UserName AND FriendUserName = @FriendUserName";
            SqlParameter[] parameters = {
                new SqlParameter("@UserName", userName),
                new SqlParameter("@FriendUserName", friendUserName),
                new SqlParameter("@GroupName", groupName)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 将DataRow转换为Friend对象
        private Friend DataRowToFriend(DataRow row)
        {
            return new Friend
            {
                UserName = row["UserName"].ToString(),
                FriendUserName = row["FriendUserName"].ToString(),
                FriendNickName = row["FriendNickName"] is DBNull ? null : row["FriendNickName"].ToString(),
                Remark = row["Remark"] is DBNull ? null : row["Remark"].ToString(),
                GroupName = row["GroupName"].ToString(),
                AddTime = Convert.ToDateTime(row["AddTime"])
            };
        }
    }
}