using System.Collections.Generic;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 语言数据模型
    /// </summary>
    public sealed partial class LanguageModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 语言信息
        /// </summary>
        public KeyValuePair<string, string> LanguageInfo { get; set; }

        /// <summary>
        /// 语言项是否已经被选择
        /// </summary>
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
