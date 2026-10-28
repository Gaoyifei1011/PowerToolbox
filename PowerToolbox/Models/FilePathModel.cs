namespace PowerToolbox.Models
{
    /// <summary>
    /// 包文件索引文件路径数据模型
    /// </summary>
    internal sealed class FilePathModel
    {
        /// <summary>
        /// 文件路径对应的键
        /// </summary>
        internal string Key { get; set; }

        /// <summary>
        /// 文件路径对应的绝对路径
        /// </summary>
        internal string AbsolutePath { get; set; }
    }
}
