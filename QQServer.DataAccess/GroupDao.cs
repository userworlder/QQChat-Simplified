using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class GroupDao
    {
        // 创建群聊
        public bool CreateGroup(Group group)
        {
            string sql = "INSERT INTO Groups (GroupId, GroupName, GroupAvatar, CreatorId, CreateTime, Description) " +
                         "VALUES (@GroupId, @GroupName, @GroupAvatar, @CreatorId, @CreateTime, @Description)";
            SqlParameter[] parameters = {
                new SqlParameter("@GroupId", group.GroupId),
                new SqlParameter("@GroupName", group.GroupName),
                new SqlParameter("@GroupAvatar", group.GroupAvatar ?? (object)DBNull.Value),
                new SqlParameter("@CreatorId", group.CreatorId),
                new SqlParameter("@CreateTime", group.CreateTime),
                new SqlParameter("@Description", group.Description ?? (object)DBNull.Value)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 获取群聊信息
        public Group GetGroupById(string groupId)
        {
            string sql = "SELECT * FROM Groups WHERE GroupId = @GroupId";
            SqlParameter[] parameters = {
                new SqlParameter("@GroupId", groupId)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                return DataRowToGroup(dt.Rows[0]);
            }
            return null;
        }

        // 更新群聊信息
        public bool UpdateGroup(Group group)
        {
            string sql = "UPDATE Groups SET GroupName = @GroupName, GroupAvatar = @GroupAvatar, Description = @Description " +
                         "WHERE GroupId = @GroupId";
            SqlParameter[] parameters = {
                new SqlParameter("@GroupName", group.GroupName),
                new SqlParameter("@GroupAvatar", group.GroupAvatar ?? (object)DBNull.Value),
                new SqlParameter("@Description", group.Description ?? (object)DBNull.Value),
                new SqlParameter("@GroupId", group.GroupId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 删除群聊
        public bool DeleteGroup(string groupId)
        {
            string sql = "DELETE FROM Groups WHERE GroupId = @GroupId";
            SqlParameter[] parameters = {
                new SqlParameter("@GroupId", groupId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 将DataRow转换为Group对象
        private Group DataRowToGroup(DataRow row)
        {
            return new Group
            {
                GroupId = row["GroupId"].ToString(),
                GroupName = row["GroupName"].ToString(),
                GroupAvatar = row["GroupAvatar"] is DBNull ? null : row["GroupAvatar"].ToString(),
                CreatorId = row["CreatorId"].ToString(),
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                Description = row["Description"] is DBNull ? null : row["Description"].ToString()
            };
        }
    }
}