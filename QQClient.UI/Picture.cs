using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;  //Image所需的库
namespace QQClient.UI
{
    public static class ImageHelper
    {
        // 加载图片，就这么简单！
        public static Image Load(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"QQClient.UI.picture.{name}"))
            {
                return stream != null ? Image.FromStream(stream) : null;
            }
        }
    }
}
