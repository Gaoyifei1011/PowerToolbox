using System;

namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 以下一个或多个值的组合，这些值指示其他结构成员的内容和有效性
    /// </summary>
    [Flags]
    internal enum ShellExecuteMaskFlags : uint
    {
        /// <summary>
        /// 默认值
        /// </summary>
        SEE_MASK_DEFAULT = 0x00000000,

        /// <summary>
        /// 使用 lpClass 成员提供的类名。 如果同时设置了 SEE_MASK_CLASSKEY 和 SEE_MASK_CLASSNAME，则使用类键。
        /// </summary>
        SEE_MASK_CLASSNAME = 0x00000001,

        /// <summary>
        /// 使用 hkeyClass 成员提供的类键。 如果同时设置了 SEE_MASK_CLASSKEY 和 SEE_MASK_CLASSNAME，则使用类键。
        /// </summary>
        SEE_MASK_CLASSKEY = 0x00000003,

        /// <summary>
        /// 使用 lpIDList 参数
        /// </summary>
        SEE_MASK_IDLIST = 0x00000004,

        /// <summary>
        /// 使用所选项的快捷菜单处理程序的 IContextMenu 接口。 使用 lpFile 按其文件系统路径标识项，使用 lpIDList 按其 PIDL 标识项。 此标志允许应用程序使用 ShellExecuteEx 从快捷菜单扩展（而不是注册表中列出的静态谓词）调用谓词。
        /// </summary>
        SEE_MASK_INVOKEIDLIST = 0x0000000C,

        /// <summary>
        /// 使用 hIcon 成员提供的图标。 此标志不能与 SEE_MASK_HMONITOR 组合使用。
        /// </summary>
        SEE_MASK_ICON = 0x00000010,

        /// <summary>
        /// 使用 dwHotKey 成员提供的键盘快捷方式。
        /// </summary>
        SEE_MASK_HOTKEY = 0x00000020,

        /// <summary>
        /// 使用 指示 hProcess 成员接收进程句柄。 此句柄通常用于允许应用程序了解使用 ShellExecuteExecuteEx 创建的进程何时终止。 在某些情况下，例如通过 DDE 会话满足执行要求时，不会返回任何句柄。 调用应用程序负责在不再需要句柄时将其关闭。
        /// </summary>
        SEE_MASK_NOCLOSEPROCESS = 0x00000040,

        /// <summary>
        /// 验证共享并连接到驱动器号。 这样就可以重新连接断开连接的网络驱动器。 lpFile 成员是网络上文件的 UNC 路径。
        /// </summary>
        SEE_MASK_CONNECTNETDRV = 0x00000080,

        /// <summary>
        /// 等待执行操作完成，然后返回。 此标志应由使用可能导致异步激活的 ShellExecute 表单（例如 DDE）的调用方使用，并创建可能在后台线程上运行的进程。 (注意：如果调用方线程模型不是 Apartment， 则默认情况下 ShellExecuteExecuteEx 在后台线程上运行。) 从已在后台线程上运行的进程调用 ShellExecuteEx 应 始终传递此标志。 此外，调用 ShellExecuteEx 后 立即退出的应用程序应指定此标志。
        /// 如果在后台线程上执行执行操作，并且调用方未指定SEE_MASK_ASYNCOK标志，则调用线程将等到新进程启动后再返回。 这通常意味着已调用 CreateProcess 、DDE 通信已完成，或者自定义执行委托已通知 ShellExecuteExecuteEx 已完成。 如果指定了SEE_MASK_WAITFORINPUTIDLE标志，则 ShellExecuteExecuteEx 调用 WaitForInputIdle 并等待新进程空闲，然后返回，最大超时时间为 1 分钟。
        /// </summary>
        SEE_MASK_NOASYNC = 0x00000100,

        /// <summary>
        /// 与 SEE_MASK_NOASYNC 相同，最好使用该选项。
        /// </summary>
        SEE_MASK_FLAG_DDEWAIT = SEE_MASK_NOASYNC,

        /// <summary>
        /// 展开 lpDirectory 或 lpFile 成员提供的字符串中指定的任何环境变量。
        /// </summary>
        SEE_MASK_DOENVSUBST = 0x00000200,

        /// <summary>
        /// 不要显示任何用户界面 (UI) 包括错误对话框、安全警告或其他用户界面，这些用户界面通常不会显示此选项。
        /// </summary>
        SEE_MASK_FLAG_NO_UI = 0x00000400,

        /// <summary>
        /// 使用此标志指示 Unicode 应用程序。
        /// </summary>
        SEE_MASK_UNICODE = 0x00004000,

        /// <summary>
        /// 使用为新进程继承父级的控制台，而不是让其创建新控制台。 这与将 CREATE_NEW_CONSOLE 标志与 CreateProcess 配合使用相反。
        /// </summary>
        SEE_MASK_NO_CONSOLE = 0x00008000,

        /// <summary>
        /// 可以在后台线程上执行执行，调用应立即返回，而无需等待后台线程完成。 请注意，在某些情况下， ShellExecuteEx 会忽略此标志，并等待进程完成，然后再返回。
        /// </summary>
        SEE_MASK_ASYNCOK = 0x00100000,

        /// <summary>
        /// 在多监视器系统上指定监视器时使用此标志。 监视器在 hMonitor 成员中指定。 此标志不能与SEE_MASK_ICON组合使用。
        /// </summary>
        SEE_MASK_HMONITOR = 0x00200000,

        /// <summary>
        /// 请勿执行区域检查。 此标志允许 ShellExecuteExecuteEx 绕过 IAttachmentExecute 到位的区域检查。
        /// </summary>
        SEE_MASK_NOZONECHECKS = 0x00800000,

        /// <summary>
        /// 未使用。
        /// </summary>
        SEE_MASK_NOQUERYCLASSSTORE = 0x01000000,

        /// <summary>
        /// 创建新进程后，等待进程变为空闲状态，然后返回，超时为一分钟。 有关更多详细信息，请参阅 WaitForInputIdle 。
        /// </summary>
        SEE_MASK_WAITFORINPUTIDLE = 0x02000000,

        /// <summary>
        /// 指示用户启动的启动，该启动可跟踪常用程序和其他行为。
        /// </summary>
        SEE_MASK_FLAG_LOG_USAGE = 0x04000000,

        /// <summary>
        /// hInstApp 成员用于指定实现 IServiceProvider 的对象的 IUnknown。 此对象将用作网站指针。 站点指针用于向 ShellExecute 函数、处理程序绑定进程和调用的谓词处理程序提供服务。
        /// 可以提供 ICreatingProcess 以允许调用方更改所创建进程的一些参数。
        /// Windows 8 及更高版本中支持此标志。
        /// 指定此选项后，调用将在调用线程上同步运行。
        /// </summary>
        SEE_MASK_FLAG_HINST_IS_SITE = 0x08000000
    }
}
