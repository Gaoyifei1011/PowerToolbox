using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 桌面图标显示数据模型
    /// </summary>
    internal class DesktopIconDisplayModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 图标是否显示
        /// </summary>
        private bool _isIconVisible;

        internal bool IsIconVisible
        {
            get { return _isIconVisible; }

            set
            {
                if (!Equals(_isIconVisible, value))
                {
                    _isIconVisible = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsIconVisible)));
                }
            }
        }

        /// <summary>
        /// 桌面图标显示名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 桌面图标标签
        /// </summary>
        internal string IconTag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
