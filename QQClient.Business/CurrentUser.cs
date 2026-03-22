using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQClient.Business
{
    public static class CurrentUser
    {
        public static string UserId { get; set; }
        public static string Username { get; set; }
        public static string Nickname { get; set; }

        public static void Clear()
        {
            UserId = null;
            Username = null;
            Nickname = null;
        }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(UserId);
    }
}
