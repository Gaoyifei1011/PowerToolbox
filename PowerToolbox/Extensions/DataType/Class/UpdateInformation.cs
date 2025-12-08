using System.Collections.Generic;
using WUApiLib;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 更新条目信息
    /// </summary>
    public class UpdateInformation
    {
        /// <summary>
        /// 更新条目
        /// </summary>
        public IUpdate5 Update { get; set; }

        /// <summary>
        /// 更新描述内容
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 与更新关联的 Microsoft 软件许可条款的完整本地化文本
        /// </summary>
        public string EulaText { get; set; }

        /// <summary>
        /// 是否为测试版本的更新
        /// </summary>
        public bool IsBeta { get; set; }

        /// <summary>
        /// 更新是否已经被隐藏
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// 更新是否已经被安装
        /// </summary>
        public bool IsInstalled { get; set; }

        /// <summary>
        /// 更新是否必须安装
        /// </summary>
        public bool IsMandatory { get; set; }

        /// <summary>
        /// 更新是否可以被卸载
        /// </summary>
        public bool IsCanUninstall { get; set; }

        /// <summary>
        /// 更新最大下载大小
        /// </summary>
        public decimal MaxDownloadSize { get; set; }

        /// <summary>
        /// 更新最小下载大小
        /// </summary>
        public decimal MinDownloadSize { get; set; }

        /// <summary>
        /// 更新的严重等级
        /// </summary>
        public string MsrcSeverity { get; set; }

        /// <summary>
        /// 更新建议安装的 CPU 速度
        /// </summary>
        public int RecommendedCpuSpeed { get; set; }

        /// <summary>
        /// 更新建议安装的可用空间大小
        /// </summary>
        public int RecommendedHardDiskSpace { get; set; }

        /// <summary>
        /// 更新建议安装的物理内存大小
        /// </summary>
        public int RecommendedMemory { get; set; }

        /// <summary>
        /// 更新是否需要重启系统来完成安装或卸载更新
        /// </summary>
        public bool RebootRequired { get; set; }

        /// <summary>
        /// 更新的本地化发行说明
        /// </summary>
        public string ReleaseNotes { get; set; }

        /// <summary>
        /// 更新支持的链接
        /// </summary>
        public string SupportURL { get; set; }

        /// <summary>
        /// 更新标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 更新类型
        /// </summary>
        public UpdateType UpdateType { get; set; }

        /// <summary>
        /// 更新的标识符
        /// </summary>
        public string UpdateID { get; set; }

        /// <summary>
        /// 与更新关联的 CVE ID 集合
        /// </summary>
        public List<string> CveIDList { get; set; } = [];

        /// <summary>
        /// 与更新关联的 Microsoft 知识库文章 ID 的集合。
        /// </summary>
        public List<string> KBArticleIDList { get; } = [];

        /// <summary>
        /// 更新支持的语言列表
        /// </summary>
        public List<string> SupportedLanguageList { get; } = [];

        /// <summary>
        /// 有关更新的详细信息的超链接
        /// </summary>
        public List<string> MoreInfoList { get; } = [];
    }
}
