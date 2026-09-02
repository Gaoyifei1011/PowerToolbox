using Microsoft.UI.Xaml.Media;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 桌面图标数据模型
    /// </summary>
    internal sealed class DesktopIconSettingsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 图标注册表路径
        /// </summary>
        internal string IconRegistryKeyPath { get; set; }

        /// <summary>
        /// 图标位置路径
        /// </summary>
        internal string IconLocationPath { get; set; }

        /// <summary>
        /// 图标标签
        /// </summary>
        internal string IconTag { get; set; }

        /// <summary>
        /// 图标索引
        /// </summary>
        internal int IconIndex { get; set; }

        /// <summary>
        /// 图标显示名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 文件图标
        /// </summary>
        private ImageSource _iconImage;

        internal ImageSource IconImage
        {
            get { return _iconImage; }

            set
            {
                if (!Equals(_iconImage, value))
                {
                    _iconImage = value;
                    PropertyChanged?.Invoke(this, new(nameof(IconImage)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
