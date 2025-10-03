using System.ComponentModel;
using TaskScheduler;
using Windows.UI.Xaml.Media;

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

        /// <summary>
        /// 计划任务是否已启用
        /// </summary>
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (!Equals(_isEnabled, value))
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        /// <summary>
        /// 计划任务图标
        /// </summary>
        public ImageSource TaskIcon { get; set; }

        /// <summary>
        /// 计划任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 计划任务的作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 计划任务描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 计划任务路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 计划任务状态
        /// </summary>
        private string _state;

        public string State
        {
            get { return _state; }

            set
            {
                if (!Equals(_state, value))
                {
                    _state = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
                }
            }
        }

        /// <summary>
        /// 计划任务上次运行的时间
        /// </summary>
        private string _lastRunTime;

        public string LastRunTime
        {
            get { return _lastRunTime; }

            set
            {
                if (!Equals(_lastRunTime, value))
                {
                    _lastRunTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastRunTime)));
                }
            }
        }

        /// <summary>
        /// 上次运行计划任务时返回的结果
        /// </summary>
        private string _lastTaskResult;

        public string LastTaskResult
        {
            get { return _lastTaskResult; }

            set
            {
                if (!Equals(_lastTaskResult, value))
                {
                    _lastTaskResult = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastTaskResult)));
                }
            }
        }

        /// <summary>
        /// 计划任务下次运行时间
        /// </summary>
        private string _nextRunTime;

        public string NextRunTime
        {
            get { return _nextRunTime; }

            set
            {
                if (!Equals(_nextRunTime, value))
                {
                    _nextRunTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextRunTime)));
                }
            }
        }

        /// <summary>
        /// 计划任务程序路径
        /// </summary>
        public string ProcessPath { get; set; }

        /// <summary>
        /// 计划任务启动参数
        /// </summary>
        public string ProcessArguments { get; set; }

        /// <summary>
        /// 计划任务版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 已注册的计划任务
        /// </summary>
        public IRegisteredTask RegisteredTask { get; set; }

        /// <summary>
        /// 计划任务所属的文件夹
        /// </summary>
        public ITaskFolder TaskFolder { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
