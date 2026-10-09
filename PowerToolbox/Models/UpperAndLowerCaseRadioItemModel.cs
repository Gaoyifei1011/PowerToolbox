using PowerToolbox.Extensions.DataType.Enums;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 大写小写页面中单选框数据模型
    /// </summary>
    internal class UpperAndLowerCaseRadioItemModel
    {
        /// <summary>
        /// 大写小写页面中单选框选中要操作的类型
        /// </summary>
        internal UpperAndLowerKind UpperAndLowerKind { get; set; }

        /// <summary>
        /// 大写小写页面中单选框选选项名称
        /// </summary>
        internal string DisplayName { get; set; }
    }
}
