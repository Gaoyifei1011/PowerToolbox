using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using TaskScheduler;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 计划任务数据模型
    /// </summary>
    internal sealed class ScheduledTaskModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 计划任务是否已启用
        /// </summary>
        internal bool IsEnabled
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsEnabled)));
                }
            }
        }

        /// <summary>
        /// 计划任务是否正在处理中
        /// </summary>
        internal bool IsProcessing
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
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
        internal string State
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(State)));
                }
            }
        }

        /// <summary>
        /// 计划任务上次运行的时间
        /// </summary>
        internal string LastRunTime
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(LastRunTime)));
                }
            }
        }

        /// <summary>
        /// 上次运行计划任务时返回的结果
        /// </summary>
        internal string LastTaskResult
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(LastTaskResult)));
                }
            }
        }

        /// <summary>
        /// 计划任务下次运行时间
        /// </summary>
        internal string NextRunTime
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
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
