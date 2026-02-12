using QQCommon.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class UserDao
    {
        // 根据用户名和密码查询用户（登录）
        public User GetUserByUsernameAndPassword(string username, string password)
        {
            string sql = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                return DataRowToUser(dt.Rows[0]);
            }
            return null;
        }

        // 根据用户名查询用户（搜索用户）
        public User GetUserByUsername(string username)
        {
            string sql = "SELECT * FROM Users WHERE Username = @Username";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", username)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                return DataRowToUser(dt.Rows[0]);
            }
            return null;
        }

        // 插入新用户（注册）
        public bool InsertUser(User user)
        {
            string sql = "INSERT INTO Users (UserId, Username, Password, Nickname, Avatar, Signature, RegisterTime) " +
                         "VALUES (@UserId, @Username, @Password, @Nickname, @Avatar, @Signature, @RegisterTime)";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", user.UserId),
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@Nickname", user.Nickname),
                new SqlParameter("@Avatar", user.Avatar ?? (object)DBNull.Value),
                new SqlParameter("@Signature", user.Signature ?? (object)DBNull.Value),
                new SqlParameter("@RegisterTime", user.RegisterTime)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 更新用户在线状态
        public void UpdateUserStatus(string userId, bool isOnline)
        {
            string sql = "UPDATE Users SET IsOnline = @IsOnline, LastLoginTime = @LastLoginTime WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@IsOnline", isOnline),
                new SqlParameter("@LastLoginTime", DateTime.Now),
                new SqlParameter("@UserId", userId)
            };

            DbHelper.ExecuteNonQuery(sql, parameters);
        }

        // 将DataRow转换为User对象
        private User DataRowToUser(DataRow row)
        {
            return new User
            {
                UserId = row["UserId"].ToString(),
                Username = row["Username"].ToString(),
                Password = row["Password"].ToString(),
                Nickname = row["Nickname"].ToString(),
                Avatar = row["Avatar"] is DBNull ? null : row["Avatar"].ToString(),
                Signature = row["Signature"] is DBNull ? null : row["Signature"].ToString(),
                IsOnline = Convert.ToBoolean(row["IsOnline"]),
                LastLoginTime = row["LastLoginTime"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["LastLoginTime"]),
                RegisterTime = Convert.ToDateTime(row["RegisterTime"])
            };
        }
    }
}