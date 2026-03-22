using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQClient.DataAccess.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        public async Task SaveFriendAsync(Friend friend)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM Friends WHERE UserId = @UserId AND FriendUserId = @FriendUserId)
                BEGIN
                    INSERT INTO Friends (UserId, FriendUserId, FriendNickName, Remark, GroupName, AddTime)
                    VALUES (@UserId, @FriendUserId, @FriendNickName, @Remark, @GroupName, @AddTime)
                END
                ELSE
                BEGIN
                    UPDATE Friends 
                    SET FriendNickName = @FriendNickName, 
                        Remark = @Remark, 
                        GroupName = @GroupName
                    WHERE UserId = @UserId AND FriendUserId = @FriendUserId
                END";

            var parameters = new[]
            {
                new SqlParameter("@UserId", friend.UserName),
                new SqlParameter("@FriendUserId", friend.FriendUserName),
                new SqlParameter("@FriendNickName", friend.FriendNickName ?? (object)DBNull.Value),
                new SqlParameter("@Remark", friend.Remark ?? (object)DBNull.Value),
                new SqlParameter("@GroupName", friend.GroupName ?? (object)DBNull.Value),
                new SqlParameter("@AddTime", friend.AddTime)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<List<Friend>> GetFriendsAsync(string userId)
        {
            const string sql = @"
                SELECT UserId, FriendUserId, FriendNickName, Remark, GroupName, AddTime
                FROM Friends
                WHERE UserId = @UserId
                ORDER BY FriendNickName";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            return ConvertDataTableToFriends(dt, userId);
        }

        public async Task UpdateFriendRemarkAsync(string userId, string friendId, string remark)
        {
            const string sql = @"
                UPDATE Friends 
                SET Remark = @Remark 
                WHERE UserId = @UserId AND FriendUserId = @FriendUserId";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendId),
                new SqlParameter("@Remark", remark ?? (object)DBNull.Value)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<bool> DeleteFriendAsync(string userId, string friendId)
        {
            const string sql = @"
                DELETE FROM Friends 
                WHERE UserId = @UserId AND FriendUserId = @FriendUserId";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendId)
            };

            var rows = await DbHelper.ExecuteNonQueryAsync(sql, parameters);
            return rows > 0;
        }

        public async Task<Friend> GetFriendAsync(string userId, string friendId)
        {
            const string sql = @"
                SELECT UserId, FriendUserId, FriendNickName, Remark, GroupName, AddTime
                FROM Friends
                WHERE UserId = @UserId AND FriendUserId = @FriendUserId";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return ConvertDataRowToFriend(dt.Rows[0], userId);
        }

        public async Task<bool> IsFriendAsync(string userId, string friendId)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM Friends 
                WHERE UserId = @UserId AND FriendUserId = @FriendUserId";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendUserId", friendId)
            };

            var result = await DbHelper.ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public async Task<int> GetFriendsCountAsync(string userId)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM Friends 
                WHERE UserId = @UserId";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId)
            };

            var result = await DbHelper.ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result);
        }

        private List<Friend> ConvertDataTableToFriends(DataTable dt, string currentUserId)
        {
            var friends = new List<Friend>();
            foreach (DataRow row in dt.Rows)
            {
                friends.Add(ConvertDataRowToFriend(row, currentUserId));
            }
            return friends;
        }

        private Friend ConvertDataRowToFriend(DataRow row, string currentUserId)
        {
            return new Friend
            {
                UserName = currentUserId,
                FriendUserName = row["FriendUserId"].ToString(),
                FriendNickName = row["FriendNickName"]?.ToString(),
                Remark = row["Remark"]?.ToString(),
                GroupName = row["GroupName"]?.ToString(),
                AddTime = Convert.ToDateTime(row["AddTime"])
            };
        }
    }
}