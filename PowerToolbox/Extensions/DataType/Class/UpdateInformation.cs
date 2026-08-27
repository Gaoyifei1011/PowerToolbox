using System.Collections.Generic;
using WUApiLib;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 更新条目信息
    /// </summary>
    internal class UpdateInformation
    {
        /// <summary>
        /// 更新条目
        /// </summary>
        internal IUpdate5 Update { get; set; }

        /// <summary>
        /// 更新描述内容
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// 与更新关联的 Microsoft 软件许可条款的完整本地化文本
        /// </summary>
        internal string EulaText { get; set; }

        /// <summary>
        /// 是否为测试版本的更新
        /// </summary>
        internal bool IsBeta { get; set; }

        /// <summary>
        /// 更新是否已经被隐藏
        /// </summary>
        internal bool IsHidden { get; set; }

        /// <summary>
        /// 更新是否已经被安装
        /// </summary>
        internal bool IsInstalled { get; set; }

        /// <summary>
        /// 更新是否必须安装
        /// </summary>
        internal bool IsMandatory { get; set; }

        /// <summary>
        /// 更新是否可以被卸载
        /// </summary>
        internal bool IsCanUninstall { get; set; }

        /// <summary>
        /// 更新最大下载大小
        /// </summary>
        internal decimal MaxDownloadSize { get; set; }

        /// <summary>
        /// 更新最小下载大小
        /// </summary>
        internal decimal MinDownloadSize { get; set; }

        /// <summary>
        /// 更新的严重等级
        /// </summary>
        internal string MsrcSeverity { get; set; }

        /// <summary>
        /// 更新建议安装的 CPU 速度
        /// </summary>
        internal int RecommendedCpuSpeed { get; set; }

        /// <summary>
        /// 更新建议安装的可用空间大小
        /// </summary>
        internal int RecommendedHardDiskSpace { get; set; }

        /// <summary>
        /// 更新建议安装的物理内存大小
        /// </summary>
        internal int RecommendedMemory { get; set; }

        /// <summary>
        /// 更新是否需要重启系统来完成安装或卸载更新
        /// </summary>
        internal bool RebootRequired { get; set; }

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
        internal UpdateType UpdateType { get; set; }

        /// <summary>
        /// 更新的标识符
        /// </summary>
        internal string UpdateID { get; set; }

        /// <summary>
        /// 与更新关联的 CVE ID 集合
        /// </summary>
        internal List<string> CveIDList { get; set; } = [];

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
    }
}
