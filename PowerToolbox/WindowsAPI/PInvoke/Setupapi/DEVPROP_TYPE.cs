namespace PowerToolbox.WindowsAPI.PInvoke.Setupapi
{
    /// <summary>
    /// 请求的设备实例属性的 property-data-type 标识符
    /// </summary>
    internal enum DEVPROP_TYPE
    {
        /// <summary>
        /// 在 Windows Vista 和更高版本的 Windows 中，DEVPROP_TYPE_EMPTY标识符表示一个特殊的基数据类型标识符，该标识符指示属性不存在。
        /// </summary>
        DEVPROP_TYPE_EMPTY = 0,

        /// <summary>
        /// 在 Windows Vista 和更高版本的 Windows 中，DEVPROP_TYPE_NULL标识符表示指示存在设备属性的特殊基数据类型标识符。 但是， 属性没有与 属性关联的值。
        /// </summary>
        DEVPROP_TYPE_NULL = 1,

        /// <summary>
        /// 在 Windows Vista 和更高版本的 Windows 中，DEVPROP_TYPE_SBYTE标识符表示基数据类型标识符，该标识符指示数据类型是 SBYTE 类型的带符号整数。
        /// </summary>
        DEVPROP_TYPE_SBYTE = 2,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_BYTE标识符表示基数据类型标识符，该标识符指示数据类型是 BYTE 类型的无符号整数。
        /// </summary>
        DEVPROP_TYPE_BYTE = 3,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_INT16标识符表示基数据类型标识符，该标识符指示数据类型是 SHORT 类型带符号整数。
        /// </summary>
        DEVPROP_TYPE_INT16 = 4,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_UINT16标识符表示基数据类型标识符，该标识符指示数据类型是 USHORT 类型的无符号整数。
        /// </summary>
        DEVPROP_TYPE_UINT16 = 5,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_INT32标识符表示基数据类型标识符，该标识符指示数据类型是 LONG 类型带符号整数。
        /// </summary>
        DEVPROP_TYPE_INT32 = 6,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_UINT32标识符表示基数据类型标识符，该标识符指示数据类型是 ULONG 类型的无符号整数。
        /// </summary>
        DEVPROP_TYPE_UINT32 = 7,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_INT64标识符表示基数据类型标识符，该标识符指示数据类型是 LONG64 类型的有符号整数。
        /// </summary>
        DEVPROP_TYPE_INT64 = 8,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_INT64标识符表示基数据类型标识符，该标识符指示数据类型是 ULONG64 类型的无符号整数。
        /// </summary>
        DEVPROP_TYPE_UINT64 = 9,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_INT64标识符表示基数据类型标识符，该标识符指示数据类型是 FLOAT 类型的 IEEE 浮点数。
        /// </summary>
        DEVPROP_TYPE_FLOAT = 10,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_DOUBLE标识符表示基数据类型标识符，该标识符指示数据类型是双类型 IEEE 浮点数。
        /// </summary>
        DEVPROP_TYPE_DOUBLE = 11,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_DECIMAL标识符表示基数据类型标识符，该标识符指示数据类型是 DECIMAL 类型的值。
        /// </summary>
        DEVPROP_TYPE_DECIMAL = 12,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_GUID标识符表示基数据类型标识符，该标识符指示数据类型是 GUID 类型的全局唯一标识符 (GUID) 。

        /// </summary>
        DEVPROP_TYPE_GUID = 13,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_CURRENCY标识符表示基数据类型标识符，该标识符指示数据类型是 CURRENCY 类型的值。
        /// </summary>
        DEVPROP_TYPE_CURRENCY = 14,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_DATE 属性类型表示基数据类型标识符，该标识符指示数据类型是一个 DOUBLE 类型的值，该值指定自 1899 年 12 月 31 日以来的天数。 例如，1900 年 1 月 1 日为 1.0;1900年1月2日，为2.0：等等。
        /// </summary>
        DEVPROP_TYPE_DATE = 15,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_FILETIME 属性类型表示基数据类型标识符，该标识符指示数据类型是 FILETIME 类型的值。
        /// </summary>
        DEVPROP_TYPE_FILETIME = 16,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_BOOLEAN属性类型表示基数据类型标识符，该标识符指示数据类型是DEVPROP_BOOLEAN类型的布尔值。
        /// </summary>
        DEVPROP_TYPE_BOOLEAN = 17,

        /// <summary>
        /// 在 Windows Vista 和更高版本的 Windows 中，DEVPROP_TYPE_STRING 属性类型表示基数据类型标识符，该标识符指示数据类型是以 NULL 结尾的 Unicode 字符串。
        /// </summary>
        DEVPROP_TYPE_STRING = 18,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_SECURITY_DESCRIPTOR标识符表示基数据类型标识符，该标识符指示数据类型是可变长度、自相对SECURITY_DESCRIPTOR类型的安全描述符。
        /// </summary>
        DEVPROP_TYPE_SECURITY_DESCRIPTOR = 19,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING标识符表示基数据类型标识符，该标识符指示数据类型是以 NULL 结尾的 Unicode 字符串，其中包含安全描述符定义语言 (SDDL) 格式的安全描述符。
        /// </summary>
        DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING = 20,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_DEVPROPKEY标识符表示基数据类型标识符，该标识符指示数据类型是 DEVPROPKEY 类型的设备属性键。
        /// </summary>
        DEVPROP_TYPE_DEVPROPKEY = 21,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_DEVPROPTYPE标识符表示基数据类型标识符，该标识符指示数据类型是 DEVPROPTYPE 类型的值。
        /// </summary>
        DEVPROP_TYPE_DEVPROPTYPE = 22,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_ERROR标识符表示 WINERROR.H 中定义的 Microsoft Win32 错误代码值的基数据类型标识符。
        /// </summary>
        DEVPROP_TYPE_ERROR = 23,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_NTSTATUS标识符表示 Ntstatus.h 中定义的 NTSTATUS 状态代码值的基数据类型标识符。
        /// </summary>
        DEVPROP_TYPE_NTSTATUS = 24,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_STRING_INDIRECT标识符表示包含间接字符串引用的以 NULL 结尾的 Unicode 字符串的基数据类型标识符。
        /// </summary>
        DEVPROP_TYPE_STRING_INDIRECT = 25,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPEMOD_ARRAY标识符表示属性数据类型修饰符，该修饰符可与 base-data-type 标识符 组合，以创建表示 base-data-type 值数组的属性数据类型标识符。
        /// </summary>
        DEVPROP_TYPEMOD_ARRAY = 4096,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPE_BINARY标识符表示基数据类型标识符，该标识符指示数据类型是 BYTE 类型无符号值的数组。
        /// </summary>
        DEVPROP_TYPE_BINARY = 4099,

        /// <summary>
        /// 在 Windows Vista 及更高版本的 Windows 中，DEVPROP_TYPEMOD_LIST标识符表示属性数据类型修饰符，该修饰符只能与基数据类型标识符组合DEVPROP_TYPE_STRING，DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING创建属性数据类型标识符，该标识符表示以 NULL 结尾的 Unicode 字符串REG_MULTI_SZ列表。
        /// </summary>
        DEVPROP_TYPEMOD_LIST = 8192,

        /// <summary>
        /// 在 Windows Vista 和更高版本的 Windows 中，DEVPROP_TYPE_STRING_LIST 属性类型表示基数据类型标识符，该标识符指示数据类型是 unicode 字符串REG_MULTI_SZ类型的列表。
        /// </summary>
        DEVPROP_TYPE_STRING_LIST = 8210,
    }
}
