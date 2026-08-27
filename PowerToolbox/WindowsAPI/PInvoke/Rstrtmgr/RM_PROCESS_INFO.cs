using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Rstrtmgr
{
    /// <summary>
    /// 描述要向重启管理器注册的应用程序。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RM_PROCESS_INFO
    {
        /// <summary>
        /// 包含一个 RM_UNIQUE_PROCESS 结构，该结构按应用程序的 PID 和进程开始的时间唯一标识应用程序。
        /// </summary>
        internal RM_UNIQUE_PROCESS Process;

        /// <summary>
        /// 如果进程是一个服务，则此参数返回该服务的长名称。 如果进程不是服务，则此参数返回应用程序的用户友好名称。 如果进程是关键进程，并且安装程序使用提升的权限运行，则此参数返回关键进程的可执行文件的名称。 如果进程是关键进程，并且安装程序作为服务运行，则此参数返回关键进程的长名称。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        internal string strAppName;

        /// <summary>
        /// 如果进程是一个服务，则这是服务的短名称。 如果进程不是服务，则不使用此成员。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        internal string strServiceShortName;

        /// <summary>
        /// 包含一个 RM_APP_TYPE 枚举值，该值将应用程序类型指定为 RmUnknownApp、 RmMainWindow、 RmOtherWindow、 RmService、 RmExplorer 或 RmCritical。
        /// </summary>
        internal RM_APP_TYPE ApplicationType;

        /// <summary>
        /// 包含一个位掩码，用于描述应用程序的当前状态。 请参阅 RM_APP_STATUS 枚举。
        /// </summary>
        internal RM_APP_STATUS AppStatus;

        /// <summary>
        /// 包含进程的终端服务会话 ID。 如果无法确定进程的终端会话，则此成员的值将设置为 RM_INVALID_SESSION (-1) 。 如果进程是服务或系统关键进程，则不使用此成员。
        /// </summary>
        internal uint TSSessionId;

        /// <summary>
        /// 如果重启管理器可以重启应用程序，则为 TRUE;否则为 FALSE。 如果进程是服务，则此成员始终为 TRUE 。 如果进程是关键系统进程，则此成员始终为 FALSE 。
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        internal readonly bool bRestartable;
    }
}
