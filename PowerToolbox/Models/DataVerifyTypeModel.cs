using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据校验类型数据模型
    /// </summary>
    internal sealed class DataVerifyTypeModel
    {
        /// <summary>
        /// 数据校验类型名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 数据校验类型
        /// </summary>
        internal DataVerifyType DataVerifyType { get; set; }
    }
}
