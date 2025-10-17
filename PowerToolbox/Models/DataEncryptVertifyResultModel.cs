namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据加密校验结果数据模型
    /// </summary>
    public class DataEncryptVertifyResultModel
    {
        /// <summary>
        /// 加密 / 校验名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 加密 / 校验后的结果
        /// </summary>
        public string Result { get; set; }
    }
}
