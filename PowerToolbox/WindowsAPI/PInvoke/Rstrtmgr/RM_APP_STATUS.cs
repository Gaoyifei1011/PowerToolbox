namespace PowerToolbox.WindowsAPI.PInvoke.Rstrtmgr
{
    /// <summary>
    /// 描述重启管理器所针对的应用程序的当前状态。
    /// </summary>
    internal enum RM_APP_STATUS
    {
        /// <summary>
        /// 应用程序处于未由任何其他枚举状态描述的状态。
        /// </summary>
        RmStatusUnknown = 0x0,

        /// <summary>
        /// 应用程序当前正在运行。
        /// </summary>
        RmStatusRunning = 0x1,

        /// <summary>
        /// 重启管理器已停止应用程序。
        /// </summary>
        RmStatusStopped = 0x2,

        /// <summary>
        /// 重启管理器外部的操作已停止应用程序。
        /// </summary>
        RmStatusStoppedOther = 0x4,

        /// <summary>
        /// 重启管理器已重启应用程序。
        /// </summary>
        RmStatusRestarted = 0x8,

        /// <summary>
        /// 重启管理器在停止应用程序时遇到错误。
        /// </summary>
        RmStatusErrorOnStop = 0x10,

        /// <summary>
        /// 重启应用程序时，重启管理器遇到错误。
        /// </summary>
        RmStatusErrorOnRestart = 0x20,

        /// <summary>
        /// 筛选器屏蔽了关闭。
        /// </summary>
        RmStatusShutdownMasked = 0x40,

        /// <summary>
        /// 重新启动被筛选器屏蔽。
        /// </summary>
        RmStatusRestartMasked = 0x80
    }
}
