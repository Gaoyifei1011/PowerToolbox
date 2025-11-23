using PowerToolbox.WindowsAPI.PInvoke.Advapi32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 将挂钩信息传递给当前挂钩链中的下一个挂钩过程。 挂钩过程可以在处理挂钩信息之前或之后调用此函数。
        /// </summary>
        /// <param name="idHook">传递给当前挂钩过程的类型。</param>
        /// <param name="nCode">传递给当前挂钩过程的挂钩代码。 下一个挂钩过程使用此代码来确定如何处理挂钩信息。</param>
        /// <param name="wParam">传递给当前挂钩过程的 wParam 值。 此参数的含义取决于与当前挂钩链关联的挂钩类型。</param>
        /// <param name="lParam">传递给当前挂钩过程的 lParam 值。 此参数的含义取决于与当前挂钩链关联的挂钩类型。</param>
        /// <returns>此值由链中的下一个挂钩过程返回。 当前挂钩过程还必须返回此值。 返回值的含义取决于挂钩类型。 有关详细信息，请参阅各个挂钩过程的说明。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "CallNextHookEx", PreserveSig = true, SetLastError = false)]
        public static extern nint CallNextHookEx(nint idHook, int nCode, nuint wParam, nint lParam);

        /// <summary>
        /// 在用户界面特权隔离 (UIPI) 消息筛选器中添加或删除消息。
        /// </summary>
        /// <param name="msg">要向筛选器添加或从中删除的消息。</param>
        /// <param name="flags">要执行的操作。</param>
        /// <returns>如果成功，则为 TRUE;否则为 FALSE。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "ChangeWindowMessageFilter", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeWindowMessageFilter(WindowMessage message, ChangeFilterFlags dwFlag);

        /// <summary>
        /// 注销交互式用户，关闭系统，或关闭并重启系统。 它将 WM_QUERYENDSESSION 消息发送到所有应用程序，以确定它们是否可以终止。
        /// </summary>
        /// <param name="uFlags">关闭类型。</param>
        /// <param name="dwReason">
        /// 启动关闭的原因。 此参数必须是 系统关闭原因代码之一。
        /// 如果此参数为零，则不会设置SHTDN_REASON_FLAG_PLANNED原因代码，因此默认操作是未定义的关闭，该关闭操作记录为“找不到此原因的标题”。 默认情况下，它也是计划外关闭。 根据系统配置方式，计划外关闭会触发创建包含系统状态信息的文件，这可能会延迟关闭。 因此，不要对此参数使用零。</param>
        /// <returns>如果该函数成功，则返回值为非零值。 由于函数以异步方式执行，因此非零返回值指示已启动关闭。 它并不指示关闭是否成功。 系统、用户或其他应用程序可能会中止关闭。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "ExitWindowsEx", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExitWindowsEx(EWX uFlags, SHTDN_REASON dwReason);

        /// <summary>
        /// 返回指定窗口的每英寸点数 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取其相关信息的窗口。</param>
        /// <returns>窗口的 DPI，取决于窗口 DPI_AWARENESS 。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetDpiForWindow", PreserveSig = true, SetLastError = false)]
        public static extern int GetDpiForWindow(nint hwnd);

        /// <summary>
        /// 检索指定虚拟键的状态。 状态指定键是向上、向下还是切换， (打开、关闭—每次按下键时交替) 。
        /// </summary>
        /// <param name="nVirtKey">虚拟密钥。 如果所需的虚拟键是字母或数字 (A 到 Z、a 到 z 或 0 到 9) ，则必须将 nVirtKey 设置为该字符的 ASCII 值。 对于其他密钥，它必须是虚拟密钥代码。
        /// 如果使用非英语键盘布局，则使用值在 ASCII A 到 Z 和 0 到 9 范围内的虚拟键来指定大多数字符键。 例如，对于德语键盘布局，ASCII O (0x4F) 值虚拟键是指“o”键，而VK_OEM_1表示“o with umlaut”键。
        /// </param>
        /// <returns>
        /// 返回值指定指定虚拟密钥的状态，如下所示：
        /// 如果高阶位为 1，则键关闭;否则，它已启动。
        /// 如果低序位为 1，则切换键。 如果某个键（如 CAPS LOCK 键）处于打开状态，则会将其切换。 如果低序位为 0，则键处于关闭状态并取消键。 切换键的指示灯(，如果键盘上的任何) 在切换键时将亮起，在取消切换键时处于关闭状态。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetKeyState", PreserveSig = true, SetLastError = false)]
        public static extern short GetKeyState(Keys nVirtKey);

        /// <summary>
        /// 检索指定窗口的边框的尺寸。 尺寸以相对于屏幕左上角的屏幕坐标提供。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="lpRect">指向 RECT 结构的指针，该结构接收窗口左上角和右下角的屏幕坐标。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetWindowRect", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 32 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值在 0 到额外窗口内存的字节数中，减去 4 个;例如，如果指定了 12 个或更多字节的额外内存，则值 8 将是第三个 32 位整数的索引。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetWindowLongW", PreserveSig = true, SetLastError = false)]
        public static extern int GetWindowLong(nint hWnd, WindowLongIndexFlags nIndex);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 64 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去 LONG_PTR的大小。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetWindowLongPtrW", PreserveSig = true, SetLastError = false)]
        public static extern int GetWindowLongPtr(nint hWnd, WindowLongIndexFlags nIndex);

        /// <summary>
        /// 合成键击。 系统可以使用这种合成的击键来生成 WM_KEYUP 或 WM_KEYDOWN 消息。 键盘驱动程序的中断处理程序调用 keybd_event 函数。
        /// </summary>
        /// <param name="bVk">虚拟密钥代码。 代码必须是 1 到 254 范围内的值。</param>
        /// <param name="bScan">密钥的硬件扫描代码。</param>
        /// <param name="dwFlags">控制函数操作的各个方面。 此参数可使用以下一个或多个值。</param>
        /// <param name="dwExtraInfo">与键笔划关联的附加值。</param>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "keybd_event", PreserveSig = true, SetLastError = false)]
        public static extern void keybd_event(Keys bVk, byte bScan, KEYEVENTFLAGS dwFlags, nuint dwExtraInfo);

        /// <summary>
        /// 锁定工作站的显示器。 锁定工作站可防止未经授权的使用。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。 由于函数以异步方式执行，因此非零返回值指示操作已启动。 它并不指示工作站是否已成功锁定。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "LockWorkStation", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LockWorkStation();

        /// <summary>
        /// MapWindowPoints 函数将 (映射) 一组点从相对于一个窗口的坐标空间转换为相对于另一个窗口的坐标空间。
        /// </summary>
        /// <param name="hWndFrom">从中转换点的窗口的句柄。 如果此参数为 NULL 或HWND_DESKTOP，则假定这些点位于屏幕坐标中。</param>
        /// <param name="hWndTo">指向要向其转换点的窗口的句柄。 如果此参数为 NULL 或HWND_DESKTOP，则点将转换为屏幕坐标。</param>
        /// <param name="lpPoints">指向 POINT 结构的数组的指针，该数组包含要转换的点集。 这些点以设备单位为单位。 此参数还可以指向 RECT 结构，在这种情况下， cPoints 参数应设置为 2。</param>
        /// <param name="cPoints">lpPoints 参数指向的数组中的 POINT 结构数。</param>
        /// <returns>如果函数成功，则返回值的低序字是添加到每个源点的水平坐标以计算每个目标点的水平坐标的像素数。 (除此之外，如果正对 hWndFrom 和 hWndTo 之一进行镜像，则每个生成的水平坐标乘以 -1.) 高序字是添加到每个源点垂直坐标的像素数，以便计算每个目标点的垂直坐标。
        /// 如果函数失败，则返回值为零。 在调用此方法之前调用 SetLastError ，以将错误返回值与合法的“0”返回值区分开来。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "MapWindowPoints", PreserveSig = true, SetLastError = false)]
        public static extern int MapWindowPoints(nint hWndFrom, nint hWndTo, ref Point lpPoints, uint cPoints);

        /// <summary>
        /// 创建从指定文件中提取的图标的句柄数组。
        /// </summary>
        /// <param name="lpszFile">要从中提取图标的文件的路径和名称。</param>
        /// <param name="nIconIndex">要提取的第一个图标的从零开始的索引。 例如，如果此值为零，则函数提取指定文件中的第一个图标。</param>
        /// <param name="cxIcon">所需的水平图标大小。</param>
        /// <param name="cyIcon">所需的垂直图标大小。 </param>
        /// <param name="phicon">指向返回的图标句柄数组的指针。</param>
        /// <param name="piconid">指向图标返回的资源标识符的指针，该图标最适合当前显示设备。 如果标识符不可用于此格式，则返回的标识符0xFFFFFFFF。 如果无法以其他方式获取标识符，则返回的标识符为 0。</param>
        /// <param name="nIcons">要从文件中提取的图标数。 仅当从 .exe 和 .dll 文件中提取时，此参数才有效。</param>
        /// <param name="flags">指定控制此函数的标志。 这些标志是 LoadImage 函数使用的LR_* 标志。</param>
        /// <returns>
        /// 如果 phicon 参数为 NULL 并且此函数成功，则返回值是文件中的图标数。 如果函数失败，则返回值为 0。如果 phicon 参数不为 NULL 且函数成功，则返回值是提取的图标数。 否则，如果未找到该文件，则返回值0xFFFFFFFF。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "PrivateExtractIconsW", PreserveSig = true, SetLastError = false)]
        public static extern int PrivateExtractIcons([MarshalAs(UnmanagedType.LPWStr)] string lpszFile, int nIconIndex, int cxIcon, int cyIcon, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] nint[] phicon, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] piconid, int nIcons, int flags);

        /// <summary>
        /// 将指定的消息发送到一个或多个窗口。
        /// </summary>
        /// <param name="hWnd">
        /// 窗口过程的句柄将接收消息。
        /// 如果此参数 HWND_BROADCAST（HWND）0xffff），则会将消息发送到系统中的所有顶级窗口，包括已禁用或不可见的未所有者窗口。 在每个窗口超时之前，该函数不会返回。因此，总等待时间可以高达 uTimeout 乘以顶级窗口数的值。
        /// </param>
        /// <param name="Msg">要发送的消息。</param>
        /// <param name="wParam">任何其他特定于消息的信息。</param>
        /// <param name="lParam">任何其他特定于消息的信息。</param>
        /// <param name="fuFlags">此函数的行为。</param>
        /// <param name="uTimeout">超时期限的持续时间（以毫秒为单位）。 如果消息是广播消息，则每个窗口都可以使用全职超时期限。 例如，如果指定了 5 秒超时期限，并且有三个无法处理消息的顶级窗口，则最多可以延迟 15 秒。</param>
        /// <param name="result">消息处理的结果。 此参数的值取决于指定的消息。</param>
        /// <returns>
        /// 如果函数成功，则返回值为非零。 SendMessageTimeout 在使用 HWND_BROADCAST 时不会提供有关各个窗口超时的信息。
        /// 如果函数失败或超时，则返回值为 0。 请注意，函数并不总是在失败时调用 setLastError 。 如果失败的原因对你很重要，请先调用 SetLastError（ERROR_SUCCESS），然后再调用 SendMessageTimeout。 如果函数返回 0，GetLastError 返回ERROR_SUCCESS，则将其视为泛型故障。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SendMessageTimeoutW", PreserveSig = true, SetLastError = false)]
        public static extern int SendMessageTimeout(nint hWnd, WindowMessage Msg, nuint wParam, nint lParam, SMTO fuFlags, uint uTimeout, out nint result);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。SendMessage 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
        /// </summary>
        /// <param name="hWnd">
        /// 窗口过程的句柄将接收消息。 如果此参数 HWND_BROADCAST ( (HWND) 0xffff) ，则会将消息发送到系统中的所有顶级窗口，
        /// 包括已禁用或不可见的未所有者窗口、重叠窗口和弹出窗口;但消息不会发送到子窗口。消息发送受 UIPI 的约束。
        /// 进程的线程只能将消息发送到较低或等于完整性级别的线程的消息队列。
        /// </param>
        /// <param name="wMsg">要发送的消息。</param>
        /// <param name="wParam">其他的消息特定信息。</param>
        /// <param name="lParam">其他的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果;这取决于发送的消息。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW", PreserveSig = true, SetLastError = false)]
        public static extern nint SendMessage(nint hWnd, WindowMessage wMsg, nuint wParam, nint lParam);

        /// <summary>
        /// 当仅针对当前正在运行的进程创建没有父级或所有者的窗口时，更改默认布局。
        /// </summary>
        /// <param name="dwDefaultLayout">默认进程布局。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetProcessDefaultLayout", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessDefaultLayout(uint dwDefaultLayout);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowLongW", PreserveSig = true, SetLastError = false)]
        public static extern nint SetWindowLong(nint hWnd, WindowLongIndexFlags nIndex, nint dwNewLong);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowLongPtrW", PreserveSig = true, SetLastError = false)]
        public static extern nint SetWindowLongPtr(nint hWnd, WindowLongIndexFlags nIndex, nint dwNewLong);

        /// <summary>
        /// 更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。
        /// </summary>
        /// <param name="hWnd">更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。</param>
        /// <param name="hWndInsertAfter">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="X">以客户端坐标表示的窗口左侧的新位置。 </param>
        /// <param name="Y">窗口顶部的新位置，以客户端坐标表示。</param>
        /// <param name="cx">窗口的新宽度（以像素为单位）。</param>
        /// <param name="cy">窗口的新高度（以像素为单位）。</param>
        /// <param name="uFlags">窗口大小调整和定位标志。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowPos", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        /// <summary>
        /// 将应用程序定义的挂钩过程安装到挂钩链中。 你将安装挂钩过程来监视系统的某些类型的事件。 这些事件与特定线程或与调用线程位于同一桌面中的所有线程相关联。
        /// </summary>
        /// <param name="idHook">要安装的挂钩过程的类型。</param>
        /// <param name="lpfn">指向挂钩过程的指针。 如果 dwThreadId 参数为零或指定由其他进程创建的线程的标识符， 则 lpfn 参数必须指向 DLL 中的挂钩过程。 否则， lpfn 可以指向与当前进程关联的代码中的挂钩过程。</param>
        /// <param name="hMod">DLL 的句柄，其中包含 lpfn 参数指向的挂钩过程。 如果 dwThreadId 参数指定当前进程创建的线程，并且挂钩过程位于与当前进程关联的代码中，则必须将 hMod 参数设置为 NULL。</param>
        /// <param name="dwThreadId">要与挂钩过程关联的线程的标识符。 对于桌面应用，如果此参数为零，则挂钩过程与调用线程在同一桌面中运行的所有现有线程相关联。 对于 Windows 应用商店应用，请参阅“备注”部分。</param>
        /// <returns>如果函数成功，则返回值是挂钩过程的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowsHookExW", PreserveSig = true, SetLastError = false)]
        public static extern nint SetWindowsHookEx(HOOKTYPE idHook, HOOKPROC lpfn, nint hMod, int dwThreadId);

        /// <summary>
        /// 删除 SetWindowsHookEx 函数安装在挂钩链中的挂钩过程。
        /// </summary>
        /// <param name="idHook">要移除的挂钩的句柄。 此参数是由先前调用 SetWindowsHookEx 获取的挂钩句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "UnhookWindowsHookEx", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(nint idHook);
    }
}
