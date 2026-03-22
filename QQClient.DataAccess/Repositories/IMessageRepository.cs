using System.Collections.Generic;
using System.Threading.Tasks;
using QQCommon.Models;

namespace QQClient.DataAccess.Repositories
{
    //消息数据访问接口
    public interface IMessageRepository
    {
        /// 保存单条消息到本地数据库
        Task SaveMessageAsync(Message message);

        //获取与指定好友的历史消息（按时间倒序，取最近 limit 条）
        Task<List<Message>> GetHistoryAsync(string userId, string friendId, int limit = 50);

        //标记与指定好友的未读消息为已读
        Task MarkAsReadAsync(string userId, string friendId);
    }
}