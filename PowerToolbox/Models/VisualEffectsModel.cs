using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 视觉效果数据模型
    /// </summary>
    public class VisualEffectsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 开启 / 禁用视觉效果
        /// </summary>
        private bool _isVisualEnabled;

        public bool IsVisualEnabled
        {
            get { return _isVisualEnabled; }

            set
            {
                if (!Equals(_isVisualEnabled, value))
                {
                    _isVisualEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsVisualEnabled)));
                }
            }
        }

        /// <summary>
        /// 视觉效果名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 视觉效果标签
        /// </summary>
        public string VisualTag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
