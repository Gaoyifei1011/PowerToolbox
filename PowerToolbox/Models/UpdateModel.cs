using PowerToolbox.Extensions.DataType.Class;
using System.Collections.Generic;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// Windows 更新数据模型
    /// </summary>
    internal sealed class UpdateModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 更新信息
        /// </summary>
        internal UpdateInformation UpdateInformation { get; set; }

        /// <summary>
        /// 驱动信息
        /// </summary>
        internal WindowsDriverInformation WindowsDriverInformation { get; set; }

        /// <summary>
        /// 更新历史记录信息
        /// </summary>
        internal UpdateHistoryInformation UpdateHistoryInformation { get; set; }

        /// <summary>
        /// 更新的客户端应用程序的标识符
        /// </summary>
        internal string ClientApplicationID { get; set; }

        /// <summary>
        /// 更新的日期和时间
        /// </summary>
        internal string Date { get; set; }

        /// <summary>
        /// 更新描述内容
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// 与更新关联的 Microsoft 软件许可条款的完整本地化文本
        /// </summary>
        internal string EulaText { get; set; }

        /// <summary>
        /// 历史更新记录结果
        /// </summary>
        internal string HistoryUpdateResult { get; set; }

        /// <summary>
        /// 是否为测试版本的更新
        /// </summary>
        internal string IsBeta { get; set; }

        /// <summary>
        /// 更新是否必须安装
        /// </summary>
        internal string IsMandatory { get; set; }

        /// <summary>
        /// 更新最大下载大小
        /// </summary>
        internal string MaxDownloadSize { get; set; }

        /// <summary>
        /// 更新最小下载大小
        /// </summary>
        internal string MinDownloadSize { get; set; }

        /// <summary>
        /// 更新的严重等级
        /// </summary>
        internal string MsrcSeverity { get; set; }

        /// <summary>
        /// 更新建议安装的 CPU 速度
        /// </summary>
        internal string RecommendedCpuSpeed { get; set; }

        /// <summary>
        /// 更新建议安装的可用空间大小
        /// </summary>
        internal string RecommendedHardDiskSpace { get; set; }

        /// <summary>
        /// 更新建议安装的物理内存大小
        /// </summary>
        internal string RecommendedMemory { get; set; }

        /// <summary>
        /// 更新的本地化发行说明
        /// </summary>
        internal string ReleaseNotes { get; set; }

        /// <summary>
        /// 更新支持的链接
        /// </summary>
        internal string SupportURL { get; set; }

        /// <summary>
        /// 更新标题
        /// </summary>
        internal string Title { get; set; }

        /// <summary>
        /// 更新类型
        /// </summary>
        internal string UpdateType { get; set; }

        /// <summary>
        /// 更新的标识符
        /// </summary>
        internal string UpdateID { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的匹配设备的问题号
        /// </summary>
        internal string DeviceProblemNumber { get; set; }

        /// <summary>
        /// 驱动程序更新的类
        /// </summary>
        internal string DriverClass { get; set; }

        /// <summary>
        /// Windows 驱动程序更新必须匹配才能安装的硬件 ID 或兼容 ID
        /// </summary>
        internal string DriverHardwareID { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的制造商的语言固定名称
        /// </summary>
        internal string DriverManufacturer { get; set; }

        /// <summary>
        /// Windows 驱动程序更新所针对的设备的语言固定模型名称
        /// </summary>
        internal string DriverModel { get; set; }

        /// <summary>
        /// Windows 驱动程序更新提供程序的语言固定名称
        /// </summary>
        internal string DriverProvider { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的驱动程序版本日期
        /// </summary>
        internal string DriverVerDate { get; set; }

        /// <summary>
        /// 与更新关联的 CVE ID 集合
        /// </summary>
        internal List<string> CveIDList { get; } = [];

        /// <summary>
        /// 与更新关联的 Microsoft 知识库文章 ID 的集合。
        /// </summary>
        internal List<string> KBArticleIDList { get; } = [];

        /// <summary>
        /// 更新支持的语言列表
        /// </summary>
        internal List<string> SupportedLanguageList { get; } = [];

        /// <summary>
        /// 有关更新的详细信息的超链接
        /// </summary>
        internal List<string> MoreInfoList { get; } = [];

        /// <summary>
        /// 当前更新是否正在更新中
        /// </summary>
        private bool _isUpdating;

        internal bool IsUpdating
        {
            get { return _isUpdating; }

            set
            {
                if (!Equals(_isUpdating, value))
                {
                    _isUpdating = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdating)));
                }
            }
        }

        /// <summary>
        /// 是否在准备安装当前更新
        /// </summary>
        private bool _isUpdatePreparing;

        internal bool IsUpdatePreparing
        {
            get { return _isUpdatePreparing; }

            set
            {
                if (!Equals(_isUpdatePreparing, value))
                {
                    _isUpdatePreparing = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdatePreparing)));
                }
            }
        }

        /// <summary>
        /// 更新安装是否已取消
        /// </summary>
        private bool _isUpdateCanceled;

        internal bool IsUpdateCanceled
        {
            get { return _isUpdateCanceled; }

            set
            {
                if (!Equals(_isUpdateCanceled, value))
                {
                    _isUpdateCanceled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdateCanceled)));
                }
            }
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        private string _updateProgress;

        internal string UpdateProgress
        {
            get { return _updateProgress; }

            set
            {
                if (!string.Equals(_updateProgress, value))
                {
                    _updateProgress = value;
                    PropertyChanged?.Invoke(this, new(nameof(UpdateProgress)));
                }
            }
        }

        private double _updatePercentage;

        internal double UpdatePercentage
        {
            get { return _updatePercentage; }

            set
            {
                if (!Equals(_updatePercentage, value))
                {
                    _updatePercentage = value;
                    PropertyChanged?.Invoke(this, new(nameof(UpdatePercentage)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
