using QQCommon.Interfaces;
using QQCommon.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QQClient.Business
{
    /// 管理请求-响应的匹配，支持异步等待响应
    public static class RequestManager
    {
        private static readonly ConcurrentDictionary<string, TaskCompletionSource<ChatPacket>> _pendingRequests
            = new ConcurrentDictionary<string, TaskCompletionSource<ChatPacket>>();


        /// 发送请求并等待响应（异步）
        /// <"client">网络客户端（实现 INetworkClient）
        /// <"packet">请求包（必须包含 MessageId）
        /// <"timeoutMs">超时毫秒数
        /// <returns>响应包</returns>
        public static async Task<ChatPacket> SendRequestAsync(INetworkClient client, ChatPacket packet, int timeoutMs = 10000)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (string.IsNullOrEmpty(packet.MessageId))
                throw new ArgumentException("请求包必须包含 MessageId", nameof(packet));

            // 创建 TaskCompletionSource 用于等待响应
            var tcs = new TaskCompletionSource<ChatPacket>();

            // 将 TCS 存储到字典中，等待响应时匹配
            _pendingRequests[packet.MessageId] = tcs;

            try
            {
                // 发送数据包
                client.SendPacket(packet);
                Console.WriteLine($"[RequestManager] 发送请求: Type={packet.Type}, MessageId={packet.MessageId}");

                // 等待响应或超时
                var timeoutTask = Task.Delay(timeoutMs);
                var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    // 超时，移除并抛出异常
                    _pendingRequests.TryRemove(packet.MessageId, out _);
                    throw new TimeoutException($"请求超时: Type={packet.Type}, MessageId={packet.MessageId}");
                }

                // 成功获取响应
                var response = await tcs.Task;
                Console.WriteLine($"[RequestManager] 收到响应: Type={response.Type}, MessageId={response.MessageId}");
                return response;
            }
            catch (Exception ex)
            {
                // 发生异常时清理
                _pendingRequests.TryRemove(packet.MessageId, out _);
                Console.WriteLine($"[RequestManager] 请求异常: {ex.Message}");
                throw;
            }
        }


        /// 处理接收到的响应包，匹配等待中的请求并设置结果

        /// <returns>是否匹配成功</returns>
        public static bool HandleResponse(ChatPacket packet)
        {
            if (packet == null)
                return false;

            if (!string.IsNullOrEmpty(packet.MessageId) && _pendingRequests.TryRemove(packet.MessageId, out var tcs))
            {
                Console.WriteLine($"[RequestManager] 匹配响应: MessageId={packet.MessageId}, Type={packet.Type}");
                tcs.TrySetResult(packet);
                return true;
            }

            return false;
        }


        /// 取消所有等待中的请求（通常在断开连接时调用）

        public static void CancelAllPendingRequests()
        {
            var pending = _pendingRequests.ToArray();
            foreach (var kvp in pending)
            {
                if (_pendingRequests.TryRemove(kvp.Key, out var tcs))
                {
                    tcs.TrySetCanceled();
                    Console.WriteLine($"[RequestManager] 取消等待请求: MessageId={kvp.Key}");
                }
            }
        }


        /// 获取当前等待中的请求数量

        public static int GetPendingCount()
        {
            return _pendingRequests.Count;
        }


        /// 获取所有等待中的请求ID（用于调试）

        public static string[] GetPendingRequestIds()
        {
            return _pendingRequests.Keys.ToArray();
        }
    }
}