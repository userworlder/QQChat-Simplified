using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class GroupMemberDao
    {
        // 添加群成员
        public bool AddGroupMember(GroupMember groupMember)
        {
            string sql = "INSERT INTO GroupMembers (GroupMemberId, GroupId, UserId, Nickname, Role, JoinTime) " +
                         "VALUES (@GroupMemberId, @GroupId, @UserId, @Nickname, @Role, @JoinTime)";
            SqlParameter[] parameters = {
                new SqlParameter("@GroupMemberId", groupMember.GroupMemberId),
                new SqlParameter("@GroupId", groupMember.GroupId),
                new SqlParameter("@UserId", groupMember.UserId),
                new SqlParameter("@Nickname", groupMember.Nickname ?? (object)DBNull.Value),
                new SqlParameter("@Role", groupMember.Role),
                new SqlParameter("@JoinTime", groupMember.JoinTime)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 获取群成员列表
        public List<GroupMember> GetGroupMembersByGroupId(string groupId)
        {
            string sql = "SELECT * FROM GroupMembers WHERE GroupId = @GroupId";
            SqlParameter[] parameters = {
                new SqlParameter("@GroupId", groupId)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<GroupMember> members = new List<GroupMember>();
            foreach (DataRow row in dt.Rows)
            {
                members.Add(DataRowToGroupMember(row));
            }
            return members;
        }

        // 获取用户加入的群列表
        public List<GroupMember> GetGroupMembersByUserId(string userId)
        {
            string sql = "SELECT * FROM GroupMembers WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters);
            List<GroupMember> members = new List<GroupMember>();
            foreach (DataRow row in dt.Rows)
            {
                members.Add(DataRowToGroupMember(row));
            }
            return members;
        }

        // 移除群成员
        public bool RemoveGroupMember(string groupId, string userId)
        {
            string sql = "DELETE FROM GroupMembers WHERE GroupId = @GroupId AND UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@GroupId", groupId),
                new SqlParameter("@UserId", userId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 更新群成员角色
        public bool UpdateGroupMemberRole(string groupId, string userId, int role)
        {
            string sql = "UPDATE GroupMembers SET Role = @Role WHERE GroupId = @GroupId AND UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@Role", role),
                new SqlParameter("@GroupId", groupId),
                new SqlParameter("@UserId", userId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 更新群成员昵称
        public bool UpdateGroupMemberNickname(string groupId, string userId, string nickname)
        {
            string sql = "UPDATE GroupMembers SET Nickname = @Nickname WHERE GroupId = @GroupId AND UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@Nickname", nickname ?? (object)DBNull.Value),
                new SqlParameter("@GroupId", groupId),
                new SqlParameter("@UserId", userId)
            };

            return DbHelper.ExecuteNonQuery(sql, parameters) > 0;
        }

        // 将DataRow转换为GroupMember对象
        private GroupMember DataRowToGroupMember(DataRow row)
        {
            return new GroupMember
            {
                GroupMemberId = row["GroupMemberId"].ToString(),
                GroupId = row["GroupId"].ToString(),
                UserId = row["UserId"].ToString(),
                Nickname = row["Nickname"] is DBNull ? null : row["Nickname"].ToString(),
                Role = Convert.ToInt32(row["Role"]),
                JoinTime = Convert.ToDateTime(row["JoinTime"])
            };
        }
    }
}