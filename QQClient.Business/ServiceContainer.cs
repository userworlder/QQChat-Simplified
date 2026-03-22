using System;
using System.Collections.Generic;

namespace QQClient.Business
{
    /// <summary>
    /// 简单的依赖注入容器（IoC容器）
    /// 用于注册和获取服务实例，实现服务之间的解耦
    /// </summary>
    public static class ServiceContainer
    {
        // 存储已注册的服务，键为服务类型，值为服务实例
        // 使用 Dictionary<Type, object> 因为不同类型的服务实例无法用统一类型存储
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// 注册服务实例
        /// </summary>
        /// <typeparam name="T">服务接口类型（或类类型）</typeparam>
        /// <param name="service">要注册的服务实例</param>
        public static void Register<T>(T service) where T : class
        {
            // 使用 lock 保证多线程安全（因为可能有多个线程同时注册/解析服务）
            lock (_services)
            {
                // 如果已存在相同类型的服务，则覆盖（便于更新）
                if (_services.ContainsKey(typeof(T)))
                {
                    _services[typeof(T)] = service;
                }
                else
                {
                    _services.Add(typeof(T), service);
                }
            }
        }

        /// <summary>
        /// 解析（获取）已注册的服务实例
        /// </summary>
        /// <typeparam name="T">要获取的服务接口类型</typeparam>
        /// <returns>注册的服务实例</returns>
        /// <exception cref="InvalidOperationException">如果指定类型的服务未注册</exception>
        /// T：泛型：目的是让一个方法可以适用于多种类型，同时保持类型安全。
        public static T Resolve<T>() where T : class
        {
            lock (_services)
            {
                // 尝试获取服务实例
                if (_services.TryGetValue(typeof(T), out var service))
                {
                    // 强制转换为目标类型（因为存储时是 object，实际类型一定兼容）
                    return (T)service;
                }
            }
            // 未找到时抛出异常，提示调用者先注册服务
            throw new InvalidOperationException($"未注册服务类型：{typeof(T)}");
        }

        /// <summary>
        /// 检查指定类型的服务是否已注册
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>是否已注册</returns>
        public static bool IsRegistered<T>() where T : class
        {
            lock (_services)
            {
                return _services.ContainsKey(typeof(T));
            }
        }
    }
}