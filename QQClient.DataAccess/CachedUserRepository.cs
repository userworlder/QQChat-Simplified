using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQClient.DataAccess.Repositories
{
    public class CachedUserRepository
    {
        public async Task SaveCachedUserAsync(User user)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM CachedUsers WHERE UserId = @UserId)
                BEGIN
                    INSERT INTO CachedUsers (UserId, Username, Nickname, Signature, Avatar, CacheTime)
                    VALUES (@UserId, @Username, @Nickname, @Signature, @Avatar, @CacheTime)
                END
                ELSE
                BEGIN
                    UPDATE CachedUsers 
                    SET Username = @Username,
                        Nickname = @Nickname,
                        Signature = @Signature,
                        Avatar = @Avatar,
                        CacheTime = @CacheTime
                    WHERE UserId = @UserId
                END";

            var parameters = new[]
            {
                new SqlParameter("@UserId", user.UserId),
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Nickname", user.Nickname ?? (object)DBNull.Value),
                new SqlParameter("@Signature", user.Signature ?? (object)DBNull.Value),
                new SqlParameter("@Avatar", user.Avatar ?? (object)DBNull.Value),
                new SqlParameter("@CacheTime", DateTime.Now)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<User> GetCachedUserAsync(string userId)
        {
            const string sql = @"
                SELECT UserId, Username, Nickname, Signature, Avatar, CacheTime
                FROM CachedUsers
                WHERE UserId = @UserId";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return ConvertDataRowToUser(dt.Rows[0]);
        }

        public async Task<User> GetCachedUserByUsernameAsync(string username)
        {
            const string sql = @"
                SELECT UserId, Username, Nickname, Signature, Avatar, CacheTime
                FROM CachedUsers
                WHERE Username = @Username";

            var parameters = new[]
            {
                new SqlParameter("@Username", username)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return ConvertDataRowToUser(dt.Rows[0]);
        }

        public async Task<bool> DeleteCachedUserAsync(string userId)
        {
            const string sql = @"
                DELETE FROM CachedUsers 
                WHERE UserId = @UserId";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId)
            };

            var rows = await DbHelper.ExecuteNonQueryAsync(sql, parameters);
            return rows > 0;
        }

        public async Task ClearOldCacheAsync(int daysToKeep = 30)
        {
            const string sql = @"
                DELETE FROM CachedUsers 
                WHERE CacheTime < DATEADD(day, -@DaysToKeep, GETDATE())";

            var parameters = new[]
            {
                new SqlParameter("@DaysToKeep", daysToKeep)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        private User ConvertDataRowToUser(DataRow row)
        {
            return new User
            {
                UserId = row["UserId"].ToString(),
                Username = row["Username"].ToString(),
                Nickname = row["Nickname"]?.ToString(),
                Signature = row["Signature"]?.ToString(),
                Avatar = row["Avatar"]?.ToString()
            };
        }
    }
}