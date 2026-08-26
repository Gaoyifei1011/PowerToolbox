using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 包文件索引文件路径数据模型
    /// </summary>
    public sealed class FilePathModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 是否已选择
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 文件路径对应的键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 文件路径对应的绝对路径
        /// </summary>
        public string AbsolutePath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
