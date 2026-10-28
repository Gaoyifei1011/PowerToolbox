namespace PowerToolbox.Models
{
    /// <summary>
    /// 包文件索引嵌入的数据内容数据模型
    /// </summary>
    internal sealed class EmbeddedDataModel
    {
        /// <summary>
        /// 嵌入的数据对应的键
        /// </summary>
        internal string Key { get; set; }

        /// <summary>
        /// 嵌入的数据
        /// </summary>
        internal byte[] EmbeddedData { get; set; }
    }
}
