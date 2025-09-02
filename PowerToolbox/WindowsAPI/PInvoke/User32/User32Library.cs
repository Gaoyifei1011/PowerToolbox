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
        /// 根据所需的客户端矩形大小和提供的 DPI 计算窗口矩形的所需大小。 然后，可以将此窗口矩形传递给 CreateWindowEx 函数，以创建具有所需大小的工作区的窗口。
        /// </summary>
        /// <param name="lpRect">指向 RECT 结构的指针，该结构包含所需工作区的左上角和右下角的坐标。 函数返回时，结构包含窗口左上角和右下角的坐标，以适应所需的工作区。</param>
        /// <param name="dwStyle">要计算其所需大小的窗口的窗口 样式 。 请注意，不能指定 WS_OVERLAPPED 样式。</param>
        /// <param name="bMenu">指示窗口是否具有菜单。</param>
        /// <param name="dwExStyle">要计算其所需大小的窗口的 扩展窗口样式 。</param>
        /// <param name="dpi">用于缩放的 DPI。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "AdjustWindowRectExForDpi", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustWindowRectExForDpi(ref RECT lpRect, WindowStyle dwStyle, bool bMenu, uint dwExStyle, uint dpi);

        /// <summary>
        /// BeginPaint 函数准备用于绘制的指定窗口，并使用有关绘制的信息填充 PAINTSTRUCT 结构。
        /// </summary>
        /// <param name="hwnd">要重新绘制的窗口的句柄。</param>
        /// <param name="lpPaint">指向将接收绘制信息的 PAINTSTRUCT 结构的指针。</param>
        /// <returns>如果函数成功，则返回值是指定窗口的显示设备上下文的句柄。如果函数失败，则返回值为 NULL，指示没有可用的显示设备上下文。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "BeginPaint", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        /// <summary>
        /// 将挂钩信息传递给当前挂钩链中的下一个挂钩过程。 挂钩过程可以在处理挂钩信息之前或之后调用此函数。
        /// </summary>
        /// <param name="idHook">传递给当前挂钩过程的类型。</param>
        /// <param name="nCode">传递给当前挂钩过程的挂钩代码。 下一个挂钩过程使用此代码来确定如何处理挂钩信息。</param>
        /// <param name="wParam">传递给当前挂钩过程的 wParam 值。 此参数的含义取决于与当前挂钩链关联的挂钩类型。</param>
        /// <param name="lParam">传递给当前挂钩过程的 lParam 值。 此参数的含义取决于与当前挂钩链关联的挂钩类型。</param>
        /// <returns>此值由链中的下一个挂钩过程返回。 当前挂钩过程还必须返回此值。 返回值的含义取决于挂钩类型。 有关详细信息，请参阅各个挂钩过程的说明。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "CallNextHookEx", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, UIntPtr wParam, IntPtr lParam);

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
        /// 创建具有扩展窗口样式的重叠、弹出窗口或子窗口;否则，此函数与 InitializeWindow 函数相同。
        /// 有关创建窗口以及 CreateWindowEx 的其他参数的完整说明的详细信息，请参阅 InitializeWindow。
        /// </summary>
        /// <param name="dwExStyle">正在创建的窗口的扩展窗口样式。 </param>
        /// <param name="lpClassName">
        /// 由上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的空终止字符串或类原子。 原子必须位于 <param  name="lpClassName"> 的低序单词中;高序单词必须为零。 如果 <param  name="lpClassName"> 是字符串，则指定窗口类名称。 类名可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，前提是注册该类的模块也是创建窗口的模块。 类名也可以是任何预定义的系统类名称。
        /// </param>
        /// <param name="lpWindowName">
        /// 窗口名称。 如果窗口样式指定标题栏，则 lpWindowName 指向的窗口标题将显示在标题栏中。 使用 InitializeWindow 创建控件（如按钮、复选框和静态控件）时，请使用 lpWindowName 指定控件的文本。 使用 SS_ICON 样式创建静态控件时，请使用 lpWindowName 指定图标名称或标识符。 若要指定标识符，请使用语法“#num”。
        /// </param>
        /// <param name="dwStyle">正在创建的窗口的样式。</param>
        /// <param name="x">
        /// 窗口的初始水平位置。 对于重叠或弹出窗口， x 参数是窗口左上角的初始 x 坐标，以屏幕坐标表示。
        /// 对于子窗口， x 是窗口左上角相对于父窗口工作区左上角的 x 坐标。 如果 x 设置为 CW_USEDEFAULT，系统将选择窗口左上角的默认位置，并忽略 y 参数。
        /// CW_USEDEFAULT 仅适用于重叠窗口;如果为弹出窗口或子窗口指定， 则 x 和 y 参数设置为零。
        /// </param>
        /// <param name="y">
        /// 窗口的初始垂直位置。 对于重叠或弹出窗口， y 参数是窗口左上角的初始 y 坐标，以屏幕坐标表示。
        /// 对于子窗口， y 是子窗口左上角相对于父窗口工作区左上角的初始 y 坐标。 对于列表框 y ，是列表框工作区左上角相对于父窗口工作区左上角的初始 y 坐标。
        /// </param>
        /// <param name="nWidth">
        /// 窗口的宽度（以设备单位为单位）。 对于重叠的窗口， nWidth 是窗口的宽度、屏幕坐标或 CW_USEDEFAULT。
        /// 如果 nWidth 是CW_USEDEFAULT，则系统会为窗口选择默认宽度和高度;默认宽度从初始 x 坐标扩展到屏幕的右边缘;默认高度从初始 y 坐标扩展到图标区域的顶部。 CW_USEDEFAULT 仅适用于重叠窗口;如果为弹出窗口或子窗口指定 了CW_USEDEFAULT ， 则 nWidth 和 nHeight 参数设置为零。
        /// </param>
        /// <param name="nHeight">
        /// 窗口的高度（以设备单位为单位）。 对于重叠窗口， nHeight 是窗口的高度（以屏幕坐标为单位）。
        /// 如果 nWidth 参数设置为 CW_USEDEFAULT，则系统将忽略 nHeight。
        /// </param>
        /// <param name="hWndParent">
        /// 正在创建的窗口的父窗口或所有者窗口的句柄。 若要创建子窗口或拥有的窗口，请提供有效的窗口句柄。 对于弹出窗口，此参数是可选的。
        /// </param>
        /// <param name="hMenu">
        /// 菜单的句柄，或指定子窗口标识符，具体取决于窗口样式。 对于重叠或弹出窗口， hMenu 标识要与窗口一起使用的菜单；如果使用类菜单，则为 NULL 。
        /// 对于子窗口， hMenu 指定子窗口标识符，即对话框控件用来通知其父级事件的整数值。
        /// 应用程序确定子窗口标识符;对于具有相同父窗口的所有子窗口，它必须是唯一的。
        /// </param>
        /// <param name="hInstance">要与窗口关联的模块实例的句柄。</param>
        /// <param name="lpParam">
        /// 指向通过 CREATESTRUCT 结构传递给窗口的值的指针， (lpCreateParams 成员) WM_CREATE 消息的 lpParam 参数所指向的值。 此消息在返回之前由此函数发送到创建的窗口。
        /// </param>
        /// <returns>如果函数成功，则返回值是新窗口的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr CreateWindowEx(WindowExStyle dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WindowStyle dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        /// <summary>
        /// 调用默认窗口过程，为应用程序未处理的任何窗口消息提供默认处理。 此函数可确保处理每个消息。 DefWindowProc 使用窗口过程接收的相同参数调用。
        /// </summary>
        /// <param name="hWnd">接收消息的窗口过程的句柄。</param>
        /// <param name="uMsg">消息。</param>
        /// <param name="wParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <param name="lParam">其他消息信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <returns>返回值是消息处理的结果，取决于消息。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, WindowMessage uMsg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// EndPaint 函数在指定窗口中标记绘制的结束。 每次调用 BeginPaint 函数都需要此函数，但仅在绘制完成后。
        /// </summary>
        /// <param name="hWnd">已重新绘制的窗口的句柄。</param>
        /// <param name="lpPaint">指向 PAINTSTRUCT 结构的指针，该结构包含 BeginPaint 检索的绘画信息。</param>
        /// <returns>返回值始终为非零值。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "EndPaint", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

        /// <summary>
        /// FillRect 函数使用指定的画笔填充矩形。 此函数包括左边框和上边框，但不包括矩形的右边框和下边框。
        /// </summary>
        /// <param name="hdc">设备上下文的句柄。</param>
        /// <param name="lprc">指向 RECT 结构的指针，该结构包含要填充的矩形的逻辑坐标。</param>
        /// <param name="hbr">用于填充矩形的画笔的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "FillRect", PreserveSig = true, SetLastError = false)]
        public static extern int FillRect(IntPtr hdc, RECT lprc, IntPtr hbr);

        /// <summary>
        /// 检索窗口工作区的坐标。 客户端坐标指定工作区的左上角和右下角。 由于客户端坐标相对于窗口工作区的左上角，因此左上角的坐标 (0,0) 。
        /// </summary>
        /// <param name="hWnd">要检索其客户端坐标的窗口的句柄。</param>
        /// <param name="lpRect">指向接收客户端坐标的 RECT 结构的指针。 左侧成员和顶部成员为零。 右侧和底部成员包含窗口的宽度和高度。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetClientRect", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

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
        /// 检索指定的系统指标或系统配置设置，同时考虑提供的 DPI。
        /// </summary>
        /// <param name="nIndex">要检索的系统指标或配置设置。</param>
        /// <param name="dpi">用于缩放指标的 DPI。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetSystemMetricsForDpi", PreserveSig = true, SetLastError = false)]
        public static extern int GetSystemMetricsForDpi(SM nIndex, int dpi);

        /// <summary>
        /// 检索指定窗口的边框的尺寸。 尺寸以相对于屏幕左上角的屏幕坐标提供。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="lpRect">指向 RECT 结构的指针，该结构接收窗口左上角和右下角的屏幕坐标。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetWindowRect", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 32 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值在 0 到额外窗口内存的字节数中，减去 4 个;例如，如果指定了 12 个或更多字节的额外内存，则值 8 将是第三个 32 位整数的索引。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetWindowLongW", PreserveSig = true, SetLastError = false)]
        public static extern int GetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 64 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去 LONG_PTR的大小。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetWindowLongPtrW", PreserveSig = true, SetLastError = false)]
        public static extern int GetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex);

        /// <summary>
        /// 确定指定窗口的可见性状态。
        /// </summary>
        /// <param name="hWnd">要测试的窗口的句柄。</param>
        /// <returns>
        /// 如果指定的窗口、其父窗口、其父窗口等具有 WS_VISIBLE 样式，则返回值为非零。 否则返回值为零。
        /// 由于返回值指定窗口是否具有 WS_VISIBLE 样式，因此即使窗口被其他窗口完全遮挡，也可能为非零。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "IsWindowVisible", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// 确定窗口是否最大化。
        /// </summary>
        /// <param name="hWnd">要测试的窗口的句柄。</param>
        /// <returns>如果窗口已缩放，则返回值为非零值。如果未缩放窗口，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "IsZoomed", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// 合成键击。 系统可以使用这种合成的击键来生成 WM_KEYUP 或 WM_KEYDOWN 消息。 键盘驱动程序的中断处理程序调用 keybd_event 函数。
        /// </summary>
        /// <param name="bVk">虚拟密钥代码。 代码必须是 1 到 254 范围内的值。</param>
        /// <param name="bScan">密钥的硬件扫描代码。</param>
        /// <param name="dwFlags">控制函数操作的各个方面。 此参数可使用以下一个或多个值。</param>
        /// <param name="dwExtraInfo">与键笔划关联的附加值。</param>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "keybd_event", PreserveSig = true, SetLastError = false)]
        public static extern void keybd_event(Keys bVk, byte bScan, KEYEVENTFLAGS dwFlags, UIntPtr dwExtraInfo);

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
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref Point lpPoints, uint cPoints);

        /// <summary>
        /// 更改指定窗口的位置和尺寸。 对于顶级窗口，位置和尺寸是相对于屏幕左上角的。 对于子窗口，它们相对于父窗口工作区的左上角。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="x">窗口左侧的新位置。</param>
        /// <param name="y">窗口顶部的新位置。</param>
        /// <param name="width">窗口的新宽度。</param>
        /// <param name="height">窗口的新高度。</param>
        /// <param name="bRepaint">指示是否重新绘制窗口。 如果此参数为 TRUE，则窗口将收到消息。 如果参数为 FALSE，则不会进行任何类型的重新绘制。 这适用于工作区、非工作区 (包括标题栏和滚动条) ，以及由于移动子窗口而发现父窗口的任何部分。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "MoveWindow", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool bRepaint);

        /// <summary>
        /// Places (在与创建指定窗口的线程关联的消息队列中发布) 消息，并在不等待线程处理消息的情况下返回 。
        /// 若要在与线程关联的消息队列中发布消息，请使用 PostThreadMessage 函数。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，窗口过程是接收消息。从 Windows Vista 开始，消息发布受 UIPI 的约束。 进程的线程只能将消息发布到完整性级别较低或相等的进程中线程的消息队列。</param>
        /// <param name="Msg">要发布的消息。</param>
        /// <param name="wparam">其他的消息特定信息。</param>
        /// <param name="lparam">其他的消息特定信息。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "PostMessage", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, WindowMessage Msg, UIntPtr wparam, IntPtr lparam);

        /// <summary>
        /// PtInRect 函数确定指定的点是否位于指定的矩形内。 如果点位于左侧或顶部，或者位于所有四个边内，则点位于矩形内。 右侧或底部的点被视为矩形外部的点。
        /// </summary>
        /// <param name="lprc">指向包含指定矩形的 RECT 结构的指针。</param>
        /// <param name="pt">包含指定点的 POINT 结构。</param>
        /// <returns>如果指定的点位于矩形内，则返回值为非零值。如果指定的点不在矩形内，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "PtInRect", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PtInRect(ref RECT lprc, Point pt);

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
        public static extern int PrivateExtractIcons([MarshalAs(UnmanagedType.LPWStr)] string lpszFile, int nIconIndex, int cxIcon, int cyIcon, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] phicon, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] piconid, int nIcons, int flags);

        /// <summary>
        /// 注册一个窗口类，以便在调用 CreateWindow 或 CreateWindowEx 函数时使用。
        /// </summary>
        /// <param name="lpwcx">指向 WNDCLASSEX 结构的指针。 在将结构传递给函数之前，必须用相应的类属性填充结构。</param>
        /// <returns>
        /// 如果函数成功，则返回值为唯一标识要注册的类的类原子。 此原子只能由 CreateWindow、CreateWindowEx、GetClassInfo、GetClassInfoEx使用， FindWindow、FindWindowEx和 UnregisterClass 函数和 IActiveIMMap：：FilterClientWindows 方法。
        /// 如果函数失败，则返回值为零。
        /// </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW", PreserveSig = true, SetLastError = false)]
        public static extern int RegisterClassEx(ref WNDCLASSEX lpwcx);

        /// <summary>
        /// 从当前线程中的窗口释放鼠标捕获，并还原正常鼠标输入处理。 捕获鼠标的窗口接收所有鼠标输入，而不考虑光标的位置，但当光标热点位于另一个线程的窗口中时单击鼠标按钮除外。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "ReleaseCapture", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// 注册应用程序以接收特定电源设置事件的电源设置通知。
        /// </summary>
        /// <param name="hRecipient">指示电源设置通知的发送位置的句柄。 对于交互式应用程序， dwFlags 参数应为零， hRecipient 参数应为窗口句柄。 对于服务，dwFlags 参数应为 1，hRecipient 参数应为从 RegisterServiceCtrlHandlerEx 返回的SERVICE_STATUS_HANDLE。</param>
        /// <param name="PowerSettingGuid">要为其发送通知的电源设置的 GUID 。</param>
        /// <param name="Flags">
        /// DEVICE_NOTIFY_WINDOW_HANDLE：使用 wParam 参数为 PBT_POWERSETTINGCHANGE 的WM_POWERBROADCAST消息发送通知。
        /// DEVICE_NOTIFY_SERVICE_HANDLE：通知发送到 HandlerEx 回调函数，其中 dwControl 参数 为 SERVICE_CONTROL_POWEREVENT ， dwEventType为 PBT_POWERSETTINGCHANGE。
        /// </param>
        /// <returns>返回用于取消注册电源通知的通知句柄。 如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "RegisterPowerSettingNotification", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, in Guid PowerSettingGuid, uint Flags);

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
        public static extern int SendMessageTimeout(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, SMTO fuFlags, uint uTimeout, out IntPtr result);

        /// <summary>
        /// 设置分层窗口的不透明度和透明度颜色键。
        /// </summary>
        /// <param name="hWnd">
        /// 分层窗口的句柄。 通过使用 CreateWindowEx 函数创建窗口时指定WS_EX_LAYERED，或者在创建窗口后通过 SetWindowLong 设置WS_EX_LAYERED来创建分层窗口。
        /// Windows 8：顶级窗口和子窗口支持WS_EX_LAYERED样式。 以前的 Windows 版本仅对顶级窗口支持 WS_EX_LAYERED 。
        /// </param>
        /// <param name="crKey">COLORREF 结构，指定组合分层窗口时要使用的透明度颜色键。 窗口以这种颜色绘制的所有像素都是透明的。 若要生成 COLORREF，请使用 RGB 宏。</param>
        /// <param name="alpha">用于描述分层窗口的不透明度的 Alpha 值。 类似于 BLENDFUNCTION 结构的 SourceConstantAlpha 成员。 当 bAlpha 为 0 时，窗口是完全透明的。 当 bAlpha 为 255 时，窗口是不透明的。</param>
        /// <param name="dwFlags">要执行的操作。 </param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetLayeredWindowAttributes", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, int alpha, LWA dwFlags);

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
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 将鼠标捕获设置为属于当前线程的指定窗口。 当鼠标悬停在捕获窗口上时，或者在鼠标悬停在捕获窗口上且按钮仍然向下的情况下按下鼠标按钮时，SetCapture 将捕获鼠标输入。 一次只会有一个窗口捕获鼠标。
        /// 如果鼠标光标位于另一个线程创建的窗口上，则仅当鼠标按钮按下时，系统才会将鼠标输入定向到指定的窗口。
        /// </summary>
        /// <param name="hWnd">当前线程中窗口的句柄，用于捕获鼠标。</param>
        /// <returns>返回值是以前捕获了鼠标的窗口的句柄。 如果没有此类窗口，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetCapture", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowLongW", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowLongPtrW", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong);

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
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        /// <summary>
        /// 将应用程序定义的挂钩过程安装到挂钩链中。 你将安装挂钩过程来监视系统的某些类型的事件。 这些事件与特定线程或与调用线程位于同一桌面中的所有线程相关联。
        /// </summary>
        /// <param name="idHook">要安装的挂钩过程的类型。</param>
        /// <param name="lpfn">指向挂钩过程的指针。 如果 dwThreadId 参数为零或指定由其他进程创建的线程的标识符， 则 lpfn 参数必须指向 DLL 中的挂钩过程。 否则， lpfn 可以指向与当前进程关联的代码中的挂钩过程。</param>
        /// <param name="hMod">DLL 的句柄，其中包含 lpfn 参数指向的挂钩过程。 如果 dwThreadId 参数指定当前进程创建的线程，并且挂钩过程位于与当前进程关联的代码中，则必须将 hMod 参数设置为 NULL。</param>
        /// <param name="dwThreadId">要与挂钩过程关联的线程的标识符。 对于桌面应用，如果此参数为零，则挂钩过程与调用线程在同一桌面中运行的所有现有线程相关联。 对于 Windows 应用商店应用，请参阅“备注”部分。</param>
        /// <returns>如果函数成功，则返回值是挂钩过程的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowsHookExW", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr SetWindowsHookEx(HOOKTYPE idHook, HOOKPROC lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// 当在指定时间内鼠标指针离开窗口或将鼠标悬停在窗口上时，发布消息。
        /// </summary>
        /// <param name="lpEventTrack">指向包含跟踪信息的 TRACKMOUSEEVENT 结构的指针。</param>
        /// <returns>如果函数成功，则返回值为非零 。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "TrackMouseEvent", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TrackMouseEvent(TRACKMOUSEEVENT lpEventTrack);

        /// <summary>
        /// 删除 SetWindowsHookEx 函数安装在挂钩链中的挂钩过程。
        /// </summary>
        /// <param name="idHook">要移除的挂钩的句柄。 此参数是由先前调用 SetWindowsHookEx 获取的挂钩句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "UnhookWindowsHookEx", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        /// <summary>
        /// 检索包含指定点的窗口的句柄。
        /// </summary>
        /// <param name="Point">要检查的点。</param>
        /// <returns>返回值是包含点的窗口的句柄。 如果给定点不存在窗口，则返回值为 NULL。 如果点位于静态文本控件上，则返回值是静态文本控件下窗口的句柄。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "WindowFromPoint", PreserveSig = true, SetLastError = false)]
        public static extern IntPtr WindowFromPoint(Point Point);

        /// <summary>
        /// 取消注册电源设置通知。
        /// </summary>
        /// <param name="handle">从 RegisterPowerSettingNotification 函数返回的句柄。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "UnregisterPowerSettingNotification", PreserveSig = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr handle);
    }
}
