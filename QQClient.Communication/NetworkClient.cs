using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QQClient.Communication
{
    public class NetworkClient : INetworkClient
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionEventArgs> ConnectionChanged;

        public bool AddFriend(string friendUsername)
        {
            throw new NotImplementedException();
        }

        public bool Connect(string serverIp, int port)
        {
            try
            {
                _tcpClient = new TcpClient();

                // 内部异步转同步，带超时控制
                // ConnectAsync() 是异步方法，不会阻塞线程
                // 它立即返回一个Task对象，表示"正在进行的连接操作"
                var connectTask = _tcpClient.ConnectAsync(serverIp, port);
                if (!connectTask.Wait(5000)) // 等待5秒超时
                {
                    return false; // 连接超时
                }

                _stream = _tcpClient.GetStream();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                // 1. 关闭网络流（释放资源）
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                // 2. 关闭TCP连接
                if (_tcpClient != null)
                {
                    // 如果连接还开着，先优雅关闭
                    if (_tcpClient.Connected)
                    {
                        _tcpClient.Close();
                    }
                    _tcpClient = null;
                }

                // 3. 触发连接状态改变事件（通知UI）
                //OnConnectionChanged(false, "已断开连接");
            }
            catch (Exception ex)
            {
                // 断开连接时的异常通常不需要抛出，记录日志即可
                Console.WriteLine($"断开连接时出错: {ex.Message}");
            }
        }

        public bool Login(string username, string password)
        {
            // 1. 创建登录请求包
            var packet = new ChatPacket
            {
                Type = MessageType.LoginRequest,
                Sender = username,
                Content = password,
                Timestamp = DateTime.Now
            };

            // 2. 发送给服务器
            SendPacket(packet);

            // 3. 接收响应（简化版，实际应该异步处理）
            var response = ReceivePacket();

            // 4. 返回结果
            return response != null
                   && response.Type == MessageType.LoginResponse
                   && response.Content == "SUCCESS";
        }
        private void SendPacket(ChatPacket packet)
        {
            string json = packet.ToJson();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

            // 发送长度
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
            _stream.Write(lengthBytes, 0, lengthBytes.Length);

            // 发送数据（带超时）
            var writeTask = _stream.WriteAsync(data, 0, data.Length);
            if (!writeTask.Wait(5000))
            {
                throw new TimeoutException("发送数据超时");
            }
        }

        private ChatPacket ReceivePacket()
        {
            // 1. 读取长度（带超时）
            // 比如要发"Hello"，长度是5，发送端会先发 [0,0,0,5]
            byte[] lengthBytes = new byte[4];// 创建4字节的数组存放长度信息
            int totalRead = 0;// 已经读取了多少字节
            while (totalRead < 4)
            {
                // 参数1: 存到哪里      lengthBytes
                // 参数2: 从哪个位置开始存 totalRead（断点续传）
                // 参数3: 最多读多少字节  4 - totalRead（还剩多少没读）
                var readTask = _stream.ReadAsync(lengthBytes, totalRead, 4 - totalRead);
                if (!readTask.Wait(5000))
                {
                    throw new TimeoutException("接收数据超时");
                }
                // Result：获取实际读取到的字节数
                int bytesRead = readTask.Result;
                if (bytesRead == 0)
                    throw new EndOfStreamException("连接已关闭");
                totalRead += bytesRead;
            }
            // 4字节都读完了，现在把它转成整数（真正的消息长度）
            // 比如lengthBytes = [0,0,0,5] → dataLength = 5
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            // 2. 读取数据（带超时）
            byte[] buffer = new byte[dataLength];
            totalRead = 0;
            while (totalRead < dataLength)
            {
                var readTask = _stream.ReadAsync(buffer, totalRead, dataLength - totalRead);
                if (!readTask.Wait(5000))
                {
                    throw new TimeoutException("接收数据超时");
                }

                int bytesRead = readTask.Result;
                if (bytesRead == 0)
                    throw new EndOfStreamException("连接已关闭");
                totalRead += bytesRead;
            }
            // 把字节数组转成字符串
            string json = System.Text.Encoding.UTF8.GetString(buffer, 0, dataLength);
            // 把JSON字符串转成ChatPacket对象
            return ChatPacket.FromJson(json);
        }
        public bool Register(string username, string password, string nickname)
        {
            try
            {
                // 1. 创建注册请求包
                var user = new User
                {
                    Username = username,
                    Password = password,  
                    Nickname = string.IsNullOrEmpty(nickname) ? username : nickname,
                    RegisterTime = DateTime.Now
                };

                var packet = new ChatPacket
                {
                    Type = MessageType.RegisterRequest,
                    Sender = username,
                    Content = JsonConvert.SerializeObject(user),  // 把整个用户对象传给服务器
                    Timestamp = DateTime.Now
                };

                // 2. 发送请求
                SendPacket(packet);

                // 3. 接收响应
                var response = ReceivePacket();

                // 4. 处理响应
                if (response != null && response.Type == MessageType.RegisterResponse)
                {
                    return response.Content == "SUCCESS";
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"注册失败: {ex.Message}");
                return false;
            }
        }

        public bool SendMessage(string username,string receiver, string content)
        {
            try
            {
                //  创建聊天消息包
                var packet = new ChatPacket
                {
                    Type = MessageType.ChatMessage,
                    Sender = username,
                    Receiver = receiver,
                    Content = content,
                    Timestamp = DateTime.Now
                };

                //发送消息
                SendPacket(packet);

                // 这里直接返回成功
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送消息失败: {ex.Message}");
                return false;
            }
        }

    }
}
