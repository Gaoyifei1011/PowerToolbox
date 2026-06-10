using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 包含有关物理内存和虚拟内存（包括扩展内存）的当前状态的信息。 GlobalMemoryStatusEx 函数在此结构中存储信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MEMORYSTATUSEX
    {
        /// <summary>
        /// 结构大小（以字节为单位）。 在调用之前，必须设置此成员 GlobalMemoryStatusEx.
        /// </summary>
        public uint dwLength;

        /// <summary>
        /// 一个介于 0 和 100 之间的数字，指定正在使用的物理内存的近似百分比 (0 表示不使用内存，100 表示) 已满内存使用。
        /// </summary>
        public uint dwMemoryLoad;

        /// <summary>
        /// 实际物理内存量（以字节为单位）。
        /// </summary>
        public ulong ullTotalPhys;

        /// <summary>
        /// 当前可用的物理内存量（以字节为单位）。 这是可以立即重复使用的物理内存量，而无需先将其内容写入磁盘。 它是备用列表、可用列表和零列表的大小之和。
        /// </summary>
        public ulong ullAvailPhys;

        /// <summary>
        /// 系统或当前进程的当前已提交内存限制，以字节为单位，以较小者为准。 若要获取系统范围的已提交内存限制，请调用 GetPerformanceInfo。
        /// </summary>
        public ulong ullTotalPageFile;

        /// <summary>
        /// 当前进程可以提交的最大内存量（以字节为单位）。 此值等于或小于系统范围的可用提交值。 若要计算系统范围的可用提交值，请调用 GetPerformanceInfo，并从 CommitLimit 的值中减去 CommitTotal 的值。
        /// </summary>
        public ulong ullAvailPageFile;

        /// <summary>
        /// 调用进程的虚拟地址空间的用户模式部分的大小（以字节为单位）。 此值取决于进程类型、处理器类型和操作系统的配置。 例如，对于 x86 处理器上的大多数 32 位进程，此值约为 2 GB，对于在启用了 4 GB 优化 的系统上运行的大地址感知的 32 位进程，此值约为 3 GB。
        /// </summary>
        public ulong ullTotalVirtual;

        /// <summary>
        /// 当前位于调用进程的虚拟地址空间的用户模式部分中的未保留和未提交的内存量（以字节为单位）。
        /// </summary>
        public ulong ullAvailVirtual;

        /// <summary>
        /// 保留。 此值始终为 0。
        /// </summary>
        public ulong ullAvailExtendedVirtual;
    }
}
