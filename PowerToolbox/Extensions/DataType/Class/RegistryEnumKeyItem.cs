using System.Collections.Generic;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 注册表枚举遍历项
    /// </summary>
    internal sealed class RegistryEnumKeyItem
    {
        /// <summary>
        /// 枚举项名称
        /// </summary>
        internal string RootKey { get; set; }

        /// <summary>
        /// 子项信息
        /// </summary>
        internal List<RegistryEnumKeyItem> SubRegistryKeyList { get; set; } = [];
    }
}
