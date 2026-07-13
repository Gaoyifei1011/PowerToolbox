using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 桌面图标显示数据模型
    /// </summary>
    public class DesktopIconDisplayModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 图标是否显示
        /// </summary>
        private bool _isIconVisible;

        public bool IsIconVisible
        {
            get { return _isIconVisible; }

            set
            {
                if (!Equals(_isIconVisible, value))
                {
                    _isIconVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIconVisible)));
                }
            }
        }

        /// <summary>
        /// 桌面图标显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 桌面图标标签
        /// </summary>
        public string IconTag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
