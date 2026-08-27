using System;

namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    [Flags]
    internal enum FileMapAccess
    {
        /// <summary>
        /// 映射文件的复制写入视图。 必须使用 PAGE_READONLY、PAGE_EXECUTE_READ、PAGE_WRITECOPY、PAGE_EXECUTE_WRITECOPY、PAGE_READWRITE或 PAGE_EXECUTE_READWRITE 保护来创建文件映射对象。
        /// 当进程写入到写入时，系统会将原始页面复制到一个新页面，该页面是专用于进程的。 新页面由分页文件提供支持。 对新页面的保护会从写入副本更改为读/写。
        /// 指定写入时复制访问时，系统将对整个视图承担进程提交费用，因为调用过程可能会写入视图中的每一页，从而使所有页面都成为私有页面。 新页面的内容永远不会写回到原始文件，在取消映射视图时丢失。
        /// </summary>
        FILE_MAP_COPY = 0x00000001,

        /// <summary>
        /// 映射文件的读/写视图。 必须使用 PAGE_READWRITE 或 PAGE_EXECUTE_READWRITE 保护创建文件映射对象。与 MapViewOfFile一起使用时，（FILE_MAP_WRITE | FILE_MAP_READ）和 FILE_MAP_ALL_ACCESS 等效于 FILE_MAP_WRITE。
        /// </summary>
        FILE_MAP_WRITE = 0x00000002,

        /// <summary>
        /// 映射文件的只读视图。 尝试写入文件视图会导致访问冲突。必须使用 PAGE_READONLY、PAGE_READWRITE、PAGE_EXECUTE_READ或 PAGE_EXECUTE_READWRITE 保护创建文件映射对象。
        /// </summary>
        FILE_MAP_READ = 0x00000004,

        /// <summary>
        /// 文件的可执行视图已映射（映射的内存可以作为代码运行）。 必须使用 PAGE_EXECUTE_READ、PAGE_EXECUTE_WRITECOPY或 PAGE_EXECUTE_READWRITE 保护创建文件映射对象。
        /// </summary>
        FILE_MAP_EXECUTE = 0x00000020,

        /// <summary>
        /// 从 Windows 10 版本 1703 开始，此标志指定应使用 大型页面支持映射视图。 视图的大小必须是 GetLargePageMinimum 函数报告的大型页面大小的倍数，并且必须使用 SEC_LARGE_PAGES 选项创建文件映射对象。 如果为 lpBaseAddress提供非 null 值，则该值必须是 GetLargePageMinimum的倍数。
        /// </summary>
        FILE_MAP_LARGE_PAGES = 0x20000000,

        /// <summary>
        /// 映射文件的读/写视图。 必须使用 PAGE_READWRITE 或 PAGE_EXECUTE_READWRITE 保护创建文件映射对象。与 MapViewOfFile 函数一起使用时，FILE_MAP_ALL_ACCESS 等效于 FILE_MAP_WRITE。
        /// </summary>
        FILE_MAP_ALL_ACCESS = 0x000F0000 | FILE_MAP_COPY | FILE_MAP_WRITE | FILE_MAP_READ | 0x0008 | 0x0010
    }
}
