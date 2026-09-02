namespace PowerToolbox.Models
{
    /// <summary>
    /// 恢复模式建议数据模型
    /// </summary>
    internal sealed class RecoveryModeSuggestionModel
    {
        /// <summary>
        /// 文件系统
        /// </summary>
        internal string FileSystem { get; set; }

        /// <summary>
        /// 使用状况
        /// </summary>
        internal string Circumstances { get; set; }

        /// <summary>
        /// 建议模式
        /// </summary>
        internal string RecommendedMode { get; set; }
    }
}
