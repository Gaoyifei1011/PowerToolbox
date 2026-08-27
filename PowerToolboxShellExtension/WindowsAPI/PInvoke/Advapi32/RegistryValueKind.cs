namespace PowerToolboxShellExtension.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 注册表值类型
    /// </summary>
    internal enum RegistryValueKind
    {
        /// <summary>
        /// 没有定义的值类型。
        /// </summary>
        REG_NONE = 0,

        /// <summary>
        /// 以 null 终止的字符串。它是 Unicode 或 ANSI 字符串，具体取决于您使用的是 Unicode 还是 ANSI 函数。
        /// </summary>
        REG_SZ = 1,

        /// <summary>
        /// 一个以 NULL 结尾的字符串，该字符串中包含对环境变量（如 %PATH%）的未展开的引用。当检索值时，该引用将会展开。
        /// </summary>
        REG_EXPAND_SZ = 2,

        /// <summary>
        /// 任意格式的二进制数据。
        /// </summary>
        REG_BINARY = 3,

        /// <summary>
        /// 32 位数字。
        /// </summary>
        REG_DWORD = 4,

        /// <summary>
        /// 采用 little-endian 格式的 32 位数字。 Windows 设计为在 little-endian 计算机体系结构上运行。 因此，此值在 Windows 头文件中定义为 REG_DWORD 。
        /// </summary>
        REG_DWORD_LITTLE_ENDIAN = REG_DWORD,

        /// <summary>
        /// 采用 big-endian 格式的 32 位数字。 某些 UNIX 系统支持 big-endian 体系结构。
        /// </summary>
        REG_DWORD_BIG_ENDIAN = 5,

        /// <summary>
        /// 一个以 null 结尾的 Unicode 字符串，其中包含通过调用具有 REG_OPTION_CREATE_LINK 的 RegCreateKeyEx 函数创建的符号链接的目标路径。
        /// </summary>
        REG_LINK = 6,

        /// <summary>
        /// 以 null 结尾的字符串序列，由空字符串终止， (\0) 。 下面是一个示例： String1\0String2\0String3\0LastString\0\0。 第一个 \0 终止第一个字符串，第二个终止 \0 最后一个字符串，最后一个 \0 终止序列。 请注意，最终终止符必须计入字符串的长度。
        /// </summary>
        REG_MULTI_SZ = 7,

        /// <summary>
        /// 设备驱动程序资源列表。
        /// </summary>
        REG_RESOURCE_LIST = 8,

        /// <summary>
        /// 完整的资源描述符。
        /// </summary>
        REG_FULL_RESOURCE_DESCRIPTOR = 9,

        /// <summary>
        /// 硬件资源列表。
        /// </summary>
        REG_RESOURCE_REQUIREMENTS_LIST = 10,

        /// <summary>
        /// 64 位数字。
        /// </summary>
        REG_QWORD = 11,

        /// <summary>
        /// 采用 little-endian 格式的 64 位数字。 Windows 设计为在 little-endian 计算机体系结构上运行。 因此，此值在 Windows 头文件中定义为 REG_QWORD 。
        /// </summary>
        REG_QWORD_LITTLE_ENDIAN = REG_QWORD
    }
}
