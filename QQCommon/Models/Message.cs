using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public class Message
    {
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }  // 用户ID或群ID
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
        public bool IsRead { get; set; }
        public int MessageType { get; set; }    // 1=文本，2=图片，3=文件
    }
    // 客户端信息类
    public class ClientInfo
    {
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; set; }
        public string RemoteEndPoint { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public DateTime ConnectedTime { get; set; }
        public DateTime LastActivityTime { get; set; }
        public DateTime LastHeartbeatTime { get; set; }

        public ClientInfo()
        {
            LastActivityTime = DateTime.Now;
            LastHeartbeatTime = DateTime.Now;
        }
    }

    // 客户端信息简表（用于状态查询）
    public class ClientInfoBrief
    {
        public string Username { get; set; }
        public string RemoteEndPoint { get; set; }
        public DateTime ConnectedTime { get; set; }
        public DateTime LastActivityTime { get; set; }
    }

    // 服务器状态
    public class ServerStatus
    {
        public bool IsRunning { get; set; }
        public int ClientCount { get; set; }
        public DateTime StartTime { get; set; }
        public List<ClientInfoBrief> Clients { get; set; }
    }
}
