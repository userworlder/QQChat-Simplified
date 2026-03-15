using QQCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQCommon.Models
{
    public static class GlobalClient
    {
        // 静态属性，保存全局唯一的 INetworkClient 实例
        public static INetworkClient Current { get; set; }
        public  static string CurrentUserId {  get; set; }
    }
}
