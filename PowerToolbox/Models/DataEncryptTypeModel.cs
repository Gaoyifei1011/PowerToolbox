using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据加密类型数据模型
    /// </summary>
    internal sealed class DataEncryptTypeModel
    {
        /// <summary>
        /// 数据加密类型名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 数据加密类型
        /// </summary>
        internal DataEncryptType DataEncryptType { get; set; }
    }
}
