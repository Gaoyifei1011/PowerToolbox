namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    public enum EWX
    {
        /// <summary>
        /// 关闭调用 ExitWindowsEx 函数的进程登录会话中运行的所有进程。 然后，它会注销用户。
        /// 此标志只能由在交互式用户的登录会话中运行的进程使用。
        /// </summary>
        EWX_LOGOFF = 0,

        /// <summary>
        /// 关闭系统并关闭电源。 系统必须支持关机功能。
        /// 调用进程必须具有SE_SHUTDOWN_NAME特权。 有关更多信息，请参见下面的“备注”部分。
        /// </summary>
        EWX_POWEROFF = 0x00000008,

        /// <summary>
        /// 关闭系统，然后重启系统。
        /// 调用进程必须具有SE_SHUTDOWN_NAME特权。 有关更多信息，请参见下面的“备注”部分。
        /// </summary>
        EWX_REBOOT = 0x00000002,

        /// <summary>
        /// 关闭系统，然后重启系统，以及已注册使用 RegisterApplicationRestart 函数重启的任何应用程序。 这些应用程序接收 WM_QUERYENDSESSION 消息， lParam 设置为 ENDSESSION_CLOSEAPP 值。 有关详细信息，请参阅 应用程序指南。
        /// </summary>
        EWX_RESTARTAPPS = 0x00000040,

        /// <summary>
        /// 将系统关闭到可以安全关闭电源的点。 所有文件缓冲区都已刷新到磁盘，并且所有正在运行的进程都已停止。
        /// 调用进程必须具有SE_SHUTDOWN_NAME 特权。
        /// 指定此标志不会关闭电源，即使系统支持关机功能。 必须指定EWX_POWEROFF才能执行此操作。
        /// </summary>
        EWX_SHUTDOWN = 0x00000001,

        /// <summary>
        /// 可以通过将 EWX_HYBRID_SHUTDOWN 标志与 EWX_SHUTDOWN 标志组合来准备系统以加快启动速度。
        /// </summary>
        EWX_HYBRID_SHUTDOWN = 0x00400000,

        /// <summary>
        /// 如果启用了终端服务，则此标志不起作用。 否则，系统不会发送 WM_QUERYENDSESSION 消息。 这可能会导致应用程序丢失数据。 因此，应仅在紧急情况下使用此标志。
        /// </summary>
        EWX_FORCE = 0x00000004,

        /// <summary>
        /// 如果进程在超时间隔内不响应 WM_QUERYENDSESSION 或 WM_ENDSESSION 消息，则强制终止。 有关详细信息，请参阅“备注”部分。
        /// </summary>
        EWX_FORCEIFHUNG = 0x00000010,
    }
}
