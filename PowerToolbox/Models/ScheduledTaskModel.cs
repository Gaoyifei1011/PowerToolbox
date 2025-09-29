using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 计划任务数据模型
    /// </summary>
    public class ScheduledTaskModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 计划任务是否已经被选择
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
