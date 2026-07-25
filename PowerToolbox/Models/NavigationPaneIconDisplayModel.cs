using System.ComponentModel;

namespace PowerToolbox.Models
{
    public class NavigationPaneIconDisplayModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 导航窗格图标是否显示
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
        /// 导航窗格图标显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 导航窗格图标标签
        /// </summary>
        public string IconTag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
