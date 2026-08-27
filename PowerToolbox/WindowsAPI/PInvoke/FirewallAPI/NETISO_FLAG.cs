namespace PowerToolbox.WindowsAPI.PInvoke.FirewallAPI
{
    /// <summary>
    /// 指定是否应为应用容器返回二进制文件。
    /// </summary>
    internal enum NETISO_FLAG
    {
        /// <summary>
        /// 指定在返回应用容器之前将计算所有二进制文件。如果调用方需要有关应用容器二进制文件的最新信息和完整信息，则应设置此标志。 如果未设置此标志，则返回的数据可能已过时或不完整。
        /// </summary>
        NETISO_FLAG_FORCE_COMPUTE_BINARIES = 0x1,

        /// <summary>
        /// 用于测试的最大值。
        /// </summary>
        NETISO_FLAG_MAX = 0x2
    }
}
