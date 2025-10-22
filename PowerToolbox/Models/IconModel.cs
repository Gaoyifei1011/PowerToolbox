using Microsoft.UI.Xaml.Media;
using System.IO;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 文件图标数据模型
    /// </summary>
    public sealed class IconModel
    {
        /// <summary>
        /// 文件图标显示的索引
        /// </summary>
        public string DisplayIndex { get; set; }

        /// <summary>
        /// 文件图标数据流
        /// </summary>
        public MemoryStream IconMemoryStream { get; set; }

        /// <summary>
        /// 文件图标
        /// </summary>
        public ImageSource IconImage { get; set; }
    }
}
