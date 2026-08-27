using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using TaskScheduler;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 计划任务数据模型
    /// </summary>
    internal class ScheduledTaskModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 计划任务是否已启用
        /// </summary>
        private bool _isEnabled;

        internal bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (!Equals(_isEnabled, value))
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsEnabled)));
                }
            }
        }

        /// <summary>
        /// 计划任务是否正在处理中
        /// </summary>
        private bool _isProcessing;

        internal bool IsProcessing
        {
            get { return _isProcessing; }

            set
            {
                if (!Equals(_isProcessing, value))
                {
                    _isProcessing = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsProcessing)));
                }
            }
        }

        /// <summary>
        /// 计划任务图标
        /// </summary>
        internal ImageSource TaskIcon { get; set; }

        /// <summary>
        /// 计划任务名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 计划任务的作者
        /// </summary>
        internal string Author { get; set; }

        /// <summary>
        /// 计划任务描述
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// 计划任务路径
        /// </summary>
        internal string Path { get; set; }

        /// <summary>
        /// 计划任务状态
        /// </summary>
        private string _state;

        internal string State
        {
            get { return _state; }

            set
            {
                if (!Equals(_state, value))
                {
                    _state = value;
                    PropertyChanged?.Invoke(this, new(nameof(State)));
                }
            }
        }

        /// <summary>
        /// 计划任务上次运行的时间
        /// </summary>
        private string _lastRunTime;

        internal string LastRunTime
        {
            get { return _lastRunTime; }

            set
            {
                if (!Equals(_lastRunTime, value))
                {
                    _lastRunTime = value;
                    PropertyChanged?.Invoke(this, new(nameof(LastRunTime)));
                }
            }
        }

        /// <summary>
        /// 上次运行计划任务时返回的结果
        /// </summary>
        private string _lastTaskResult;

        internal string LastTaskResult
        {
            get { return _lastTaskResult; }

            set
            {
                if (!Equals(_lastTaskResult, value))
                {
                    _lastTaskResult = value;
                    PropertyChanged?.Invoke(this, new(nameof(LastTaskResult)));
                }
            }
        }

        /// <summary>
        /// 计划任务下次运行时间
        /// </summary>
        private string _nextRunTime;

        internal string NextRunTime
        {
            get { return _nextRunTime; }

            set
            {
                if (!Equals(_nextRunTime, value))
                {
                    _nextRunTime = value;
                    PropertyChanged?.Invoke(this, new(nameof(NextRunTime)));
                }
            }
        }

        /// <summary>
        /// 计划任务程序路径
        /// </summary>
        internal string ProcessPath { get; set; }

        /// <summary>
        /// 计划任务启动参数
        /// </summary>
        internal string ProcessArguments { get; set; }

        /// <summary>
        /// 计划任务版本
        /// </summary>
        internal string Version { get; set; }

        /// <summary>
        /// 已注册的计划任务
        /// </summary>
        internal IRegisteredTask RegisteredTask { get; set; }

        /// <summary>
        /// 计划任务所属的文件夹
        /// </summary>
        internal ITaskFolder TaskFolder { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
