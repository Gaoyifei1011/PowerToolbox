using Microsoft.UI.Xaml.Media;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 桌面图标数据模型
    /// </summary>
    public class DesktopIconSettingsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 图标注册表路径
        /// </summary>
        public string IconRegistryKeyPath { get; set; }

        /// <summary>
        /// 图标位置路径
        /// </summary>
        public string IconLocationPath { get; set; }

        /// <summary>
        /// 图标标签
        /// </summary>
        public string IconTag { get; set; }

        /// <summary>
        /// 图标索引
        /// </summary>
        public int IconIndex { get; set; }

        /// <summary>
        /// 图标显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 文件图标
        /// </summary>
        private ImageSource _iconImage;

        public ImageSource IconImage
        {
            get { return _iconImage; }

            set
            {
                if (!Equals(_iconImage, value))
                {
                    _iconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconImage)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
