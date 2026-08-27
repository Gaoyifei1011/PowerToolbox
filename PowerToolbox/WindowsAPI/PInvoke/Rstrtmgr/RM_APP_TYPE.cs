namespace PowerToolbox.WindowsAPI.PInvoke.Rstrtmgr
{
    /// <summary>
    /// 指定由 RM_PROCESS_INFO 结构描述的应用程序的类型。
    /// </summary>
    internal enum RM_APP_TYPE
    {
        /// <summary>
        /// 应用程序不能归类为任何其他类型。 此类型的应用程序只能通过强制关闭来关闭。
        /// </summary>
        RmUnknownApp = 0,

        /// <summary>
        /// Windows 应用程序作为显示顶级窗口的独立进程运行。
        /// </summary>
        RmMainWindow = 1,

        /// <summary>
        /// 不作为独立进程运行且不显示顶级窗口的 Windows 应用程序。
        /// </summary>
        RmOtherWindow = 2,

        /// <summary>
        /// 该应用程序是 Windows 服务。
        /// </summary>
        RmService = 3,

        /// <summary>
        /// 应用程序是 Windows 资源管理器。
        /// </summary>
        RmExplorer = 4,

        /// <summary>
        /// 该应用程序是一个独立的控制台应用程序。
        /// </summary>
        RmConsole = 5,

        /// <summary>
        /// 需要重启系统才能完成安装，因为无法关闭进程。 由于以下原因，无法关闭进程。 该过程可能是一个关键过程。 当前用户可能没有关闭进程的权限。 进程可能属于启动重启管理器的主安装程序。
        /// </summary>
        RmCritical = 1000
    }
}
