using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQClient.DataAccess.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        public async Task SaveGroupAsync(Group group)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM Groups WHERE GroupId = @GroupId)
                BEGIN
                    INSERT INTO Groups (GroupId, GroupName, CreatorId, CreateTime, Description)
                    VALUES (@GroupId, @GroupName, @CreatorId, @CreateTime, @Description)
                END
                ELSE
                BEGIN
                    UPDATE Groups 
                    SET GroupName = @GroupName, 
                        Description = @Description
                    WHERE GroupId = @GroupId
                END";

            var parameters = new[]
            {
                new SqlParameter("@GroupId", group.GroupId),
                new SqlParameter("@GroupName", group.GroupName),
                new SqlParameter("@CreatorId", group.CreatorId),
                new SqlParameter("@CreateTime", group.CreateTime),
                new SqlParameter("@Description", group.Description ?? (object)DBNull.Value)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<List<Group>> GetGroupsAsync(string userId)
        {
            const string sql = @"
                SELECT g.GroupId, g.GroupName, g.CreatorId, g.CreateTime, g.Description
                FROM Groups g
                INNER JOIN GroupMembers gm ON g.GroupId = gm.GroupId
                WHERE gm.UserId = @UserId
                ORDER BY g.GroupName";

            var parameters = new[]
            {
                new SqlParameter("@UserId", userId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            return ConvertDataTableToGroups(dt);
        }

        public async Task<Group> GetGroupByIdAsync(string groupId)
        {
            const string sql = @"
                SELECT GroupId, GroupName, CreatorId, CreateTime, Description
                FROM Groups
                WHERE GroupId = @GroupId";

            var parameters = new[]
            {
                new SqlParameter("@GroupId", groupId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return ConvertDataRowToGroup(dt.Rows[0]);
        }

        public async Task<List<Group>> SearchGroupsAsync(string keyword)
        {
            const string sql = @"
                SELECT GroupId, GroupName, CreatorId, CreateTime, Description
                FROM Groups
                WHERE GroupName LIKE @Keyword OR GroupId LIKE @Keyword
                ORDER BY GroupName";

            var parameters = new[]
            {
                new SqlParameter("@Keyword", $"%{keyword}%")
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            return ConvertDataTableToGroups(dt);
        }

        public async Task SaveGroupMessageAsync(GroupMessage message)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM GroupMessages WHERE MessageId = @MessageId)
                BEGIN
                    INSERT INTO GroupMessages (MessageId, GroupId, SenderId, Content, SendTime, MessageType)
                    VALUES (@MessageId, @GroupId, @SenderId, @Content, @SendTime, @MessageType)
                END";

            var parameters = new[]
            {
                new SqlParameter("@MessageId", message.MessageId),
                new SqlParameter("@GroupId", message.GroupId),
                new SqlParameter("@SenderId", message.SenderId),
                new SqlParameter("@Content", message.Content ?? (object)DBNull.Value),
                new SqlParameter("@SendTime", message.SendTime),
                new SqlParameter("@MessageType", message.MessageType)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<List<GroupMessage>> GetGroupHistoryAsync(string groupId, int limit = 50)
        {
            const string sql = @"
                SELECT TOP (@Limit) MessageId, GroupId, SenderId, Content, SendTime, MessageType
                FROM GroupMessages
                WHERE GroupId = @GroupId
                ORDER BY SendTime DESC";

            var parameters = new[]
            {
                new SqlParameter("@Limit", limit),
                new SqlParameter("@GroupId", groupId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            return ConvertDataTableToGroupMessages(dt);
        }

        public async Task SaveGroupMemberAsync(GroupMember member)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM GroupMembers WHERE GroupId = @GroupId AND UserId = @UserId)
                BEGIN
                    INSERT INTO GroupMembers (GroupId, UserId, Nickname, Role, JoinTime)
                    VALUES (@GroupId, @UserId, @Nickname, @Role, @JoinTime)
                END
                ELSE
                BEGIN
                    UPDATE GroupMembers 
                    SET Nickname = @Nickname, 
                        Role = @Role
                    WHERE GroupId = @GroupId AND UserId = @UserId
                END";

            var parameters = new[]
            {
                new SqlParameter("@GroupId", member.GroupId),
                new SqlParameter("@UserId", member.UserId),
                new SqlParameter("@Nickname", member.Nickname ?? (object)DBNull.Value),
                new SqlParameter("@Role", member.Role),
                new SqlParameter("@JoinTime", member.JoinTime)
            };

            await DbHelper.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task<List<GroupMember>> GetGroupMembersAsync(string groupId)
        {
            const string sql = @"
                SELECT GroupId, UserId, Nickname, Role, JoinTime
                FROM GroupMembers
                WHERE GroupId = @GroupId
                ORDER BY Role DESC, Nickname";

            var parameters = new[]
            {
                new SqlParameter("@GroupId", groupId)
            };

            var dt = await DbHelper.ExecuteQueryAsync(sql, parameters);
            return ConvertDataTableToGroupMembers(dt);
        }

        public async Task<bool> IsGroupMemberAsync(string groupId, string userId)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM GroupMembers 
                WHERE GroupId = @GroupId AND UserId = @UserId";

            var parameters = new[]
            {
                new SqlParameter("@GroupId", groupId),
                new SqlParameter("@UserId", userId)
            };

            var result = await DbHelper.ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> RemoveGroupMemberAsync(string groupId, string userId)
        {
            const string sql = @"
                DELETE FROM GroupMembers 
                WHERE GroupId = @GroupId AND UserId = @UserId";

            var parameters = new[]
            {
                new SqlParameter("@GroupId", groupId),
                new SqlParameter("@UserId", userId)
            };

            var rows = await DbHelper.ExecuteNonQueryAsync(sql, parameters);
            return rows > 0;
        }

        public async Task<int> GetGroupMemberCountAsync(string groupId)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM GroupMembers 
                WHERE GroupId = @GroupId";

            var parameters = new[]
            {
                new SqlParameter("@GroupId", groupId)
            };

            var result = await DbHelper.ExecuteScalarAsync(sql, parameters);
            return Convert.ToInt32(result);
        }

        private List<Group> ConvertDataTableToGroups(DataTable dt)
        {
            var groups = new List<Group>();
            foreach (DataRow row in dt.Rows)
            {
                groups.Add(ConvertDataRowToGroup(row));
            }
            return groups;
        }

        private Group ConvertDataRowToGroup(DataRow row)
        {
            return new Group
            {
                GroupId = row["GroupId"].ToString(),
                GroupName = row["GroupName"].ToString(),
                CreatorId = row["CreatorId"].ToString(),
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                Description = row["Description"]?.ToString()
            };
        }

        private List<GroupMessage> ConvertDataTableToGroupMessages(DataTable dt)
        {
            var messages = new List<GroupMessage>();
            foreach (DataRow row in dt.Rows)
            {
                messages.Add(new GroupMessage
                {
                    MessageId = row["MessageId"].ToString(),
                    GroupId = row["GroupId"].ToString(),
                    SenderId = row["SenderId"].ToString(),
                    Content = row["Content"]?.ToString(),
                    SendTime = Convert.ToDateTime(row["SendTime"]),
                    MessageType = Convert.ToInt32(row["MessageType"])
                });
            }
            return messages;
        }

        private List<GroupMember> ConvertDataTableToGroupMembers(DataTable dt)
        {
            var members = new List<GroupMember>();
            foreach (DataRow row in dt.Rows)
            {
                members.Add(new GroupMember
                {
                    GroupId = row["GroupId"].ToString(),
                    UserId = row["UserId"].ToString(),
                    Nickname = row["Nickname"]?.ToString(),
                    Role = Convert.ToInt32(row["Role"]),
                    JoinTime = Convert.ToDateTime(row["JoinTime"])
                });
            }
            return members;
        }
        
    }
}