namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 指定文件映射对象的页面保护。
    /// </summary>
    internal enum PAGE_PROTECTION
    {
        /// <summary>
        /// 允许映射视图进行只读或写入时复制访问。 尝试写入特定区域会导致访问冲突。hFile 参数指定的文件句柄必须使用 GENERIC_READ 访问权限创建。
        /// </summary>
        PAGE_READONLY = 0x02,

        /// <summary>
        /// 允许映射视图进行只读、写入复制或读/写访问。hFile 参数指定的文件句柄必须使用 GENERIC_READ 和 GENERIC_WRITE 访问权限创建。
        /// </summary>
        PAGE_READWRITE = 0x04,

        /// <summary>
        /// 允许映射视图进行只读或写入时复制访问。 此值等效于 PAGE_READONLY。hFile 参数指定的文件句柄必须使用 GENERIC_READ 访问权限创建。
        /// </summary>
        PAGE_WRITECOPY = 0x08,

        /// <summary>
        /// 允许为只读、写入时复制或执行访问权限映射视图。hFile 参数指定的文件句柄必须使用 GENERIC_READ 和 GENERIC_EXECUTE 访问权限创建。
        /// </summary>
        PAGE_EXECUTE_READ = 0x20,

        /// <summary>
        /// 允许为只读、写入复制、读/写或执行访问权限映射视图。hFile 参数指定的文件句柄必须使用 GENERIC_READ、GENERIC_WRITE和 GENERIC_EXECUTE 访问权限创建。
        /// </summary>
        PAGE_EXECUTE_READWRITE = 0x40,

        /// <summary>
        /// 允许为只读、写入时复制或执行访问权限映射视图。 此值等效于 PAGE_EXECUTE_READ。hFile 参数指定的文件句柄必须使用 GENERIC_READ 和 GENERIC_EXECUTE 访问权限创建。
        /// </summary>
        PAGE_EXECUTE_WRITECOPY = 0x80,
    }
}
