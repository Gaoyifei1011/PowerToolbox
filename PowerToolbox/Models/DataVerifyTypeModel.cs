using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 数据校验类型数据模型
    /// </summary>
    public class DataVerifyTypeModel
    {
        /// <summary>
        /// 数据校验类型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据校验类型
        /// </summary>
        public DataVerifyType DataVerifyType { get; set; }
    }
}
