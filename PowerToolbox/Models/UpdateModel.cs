using PowerToolbox.Extensions.DataType.Classes;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// 历史更新记录结果
        /// </summary>
        internal string HistoryUpdateResult { get; set; }

        /// <summary>
        /// 更新支持的链接
        /// </summary>
        internal string SupportURL { get; set; }

        /// <summary>
        /// 更新标题
        /// </summary>
        internal string Title { get; set; }

        /// <summary>
        /// 更新的标识符
        /// </summary>
        internal string UpdateID { get; set; }

        /// <summary>
        /// 更新信息列表
        /// </summary>
        internal WinRTObservableCollection<UpdateInfoModel> UpdateInfoList { get; } = [];

        /// <summary>
        /// 驱动信息列表
        /// </summary>
        internal List<UpdateInfoModel> DriverInfoList { get; } = [];

        /// <summary>
        /// 与更新关联的 CVE ID 集合
        /// </summary>
        internal StringCollection CveIDCollection { get; } = [];

        /// <summary>
        /// 与更新关联的 Microsoft 知识库文章 ID 的集合。
        /// </summary>
        internal StringCollection KBArticleIDCollection { get; } = [];

        /// <summary>
        /// 更新支持的语言列表
        /// </summary>
        internal StringCollection SupportedLanguageCollection { get; } = [];

        /// <summary>
        /// 有关更新的详细信息的超链接
        /// </summary>
        internal StringCollection MoreInfoCollection { get; } = [];

        /// <summary>
        /// 当前更新是否正在更新中
        /// </summary>
        internal bool IsUpdating
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdating)));
                }
            }
        }

        /// <summary>
        /// 是否在准备安装当前更新
        /// </summary>
        internal bool IsUpdatePreparing
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdatePreparing)));
                }
            }
        }

        /// <summary>
        /// 更新安装是否已取消
        /// </summary>
        internal bool IsUpdateCanceled
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdateCanceled)));
                }
            }
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        internal string UpdateProgress
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(UpdateProgress)));
                }
            }
        }

        internal double UpdatePercentage
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(UpdatePercentage)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
