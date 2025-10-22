using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace ThemeSwitch.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "user32.dll";

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
        /// 返回指定窗口的每英寸点数 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取其相关信息的窗口。</param>
        /// <returns>窗口的 DPI，取决于窗口 DPI_AWARENESS 。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetDpiForWindow", PreserveSig = true, SetLastError = false)]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// 检索鼠标光标的位置（以屏幕坐标为单位）。
        /// </summary>
        /// <param name="lpPoint">指向接收光标屏幕坐标的 POINT 结构的指针。</param>
        /// <returns>如果成功，则返回非零值，否则返回零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "GetCursorPos", PreserveSig = true, SetLastError = false)]
        public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);

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
    }
}
