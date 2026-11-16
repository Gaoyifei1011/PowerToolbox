using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据解密类型数据模型
    /// </summary>
    internal sealed class DataDecryptTypeModel
    {
        /// <summary>
        /// 数据解密类型名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 数据解密类型
        /// </summary>
        internal DataDecryptType DataDecryptType { get; set; }
    }
}
