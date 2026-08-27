namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 指示计时器要执行的操作的标志。
    /// </summary>
    internal enum PDTIMER : uint
    {
        /// <summary>
        /// 将计时器重置为零。 将从调用此方法时开始计算进度。
        /// </summary>
        PDTIMER_RESET = 0x00000001,

        /// <summary>
        /// 进度已暂停。
        /// </summary>
        PDTIMER_PAUSE = 0x00000002,

        /// <summary>
        /// 已恢复进度。
        /// </summary>
        PDTIMER_RESUME = 0x00000003
    }
}
