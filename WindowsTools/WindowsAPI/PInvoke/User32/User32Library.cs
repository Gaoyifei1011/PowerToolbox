﻿using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// user32.dll 函数库
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 修改指定窗口的用户界面特权隔离 (UIPI) 消息筛选器。
        /// </summary>
        /// <param name="hWnd">要修改其 UIPI 消息筛选器的窗口的句柄。</param>
        /// <param name="message">消息筛选器允许通过或阻止的消息。</param>
        /// <param name="action">要执行的操作，可以执行以下值</param>
        /// <param name="pChangeFilterStruct">指向 CHANGEFILTERSTRUCT 结构的可选指针。</param>
        /// <returns>如果函数成功，则返回 TRUE;否则，它将返回 FALSE。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "ChangeWindowMessageFilterEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, WindowMessage message, ChangeFilterAction action, in CHANGEFILTERSTRUCT pChangeFilterStruct);

        /// <summary>
        /// 销毁图标并释放图标占用的任何内存。
        /// </summary>
        /// <param name="hIcon">要销毁的图标的句柄。 图标不得处于使用中。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "DestroyIcon", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// 检索一个窗口的句柄，该窗口的类名和窗口名称与指定的字符串匹配。 该函数搜索子窗口，从指定子窗口后面的子窗口开始。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="hWndParent">要搜索其子窗口的父窗口的句柄。如果 hwndParent 为 NULL，则该函数使用桌面窗口作为父窗口。 函数在桌面的子窗口之间搜索。 如果 hwndParent 为HWND_MESSAGE，则函数将搜索所有 仅消息窗口。</param>
        /// <param name="hWndChildAfter">子窗口的句柄。 搜索从 Z 顺序中的下一个子窗口开始。 子窗口必须是 hwndParent 的直接子窗口，而不仅仅是子窗口。 如果 hwndChildAfter 为 NULL，则搜索从 hwndParent 的第一个子窗口开始。请注意，如果 hwndParent 和 hwndChildAfter 均为 NULL，则该函数将搜索所有顶级窗口和仅消息窗口。</param>
        /// <param name="lpszClass">类名或上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类名或类原子。 原子必须置于 lpszClass 的低序单词中;高阶单词必须为零。如果 lpszClass 是字符串，则指定窗口类名。 类名可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，也可以是预定义的控件类名称，也可以是 MAKEINTATOM(0x8000)。 在此后一种情况下，0x8000是菜单类的原子。 </param>
        /// <param name="lpszWindow">窗口名称 (窗口的标题) 。 如果此参数为 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类和窗口名称的窗口的句柄。如果函数失败，则返回值为 NULL。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "FindWindowExW", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

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
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "PrivateExtractIconsW", SetLastError = false)]
        public static extern int PrivateExtractIcons(string lpszFile, int nIconIndex, int cxIcon, int cyIcon, IntPtr[] phicon, int[] piconid, int nIcons, int flags);

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
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW", SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, int wParam, IntPtr lParam);

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。 键盘输入将定向到窗口，并为用户更改各种视觉提示。 系统为创建前台窗口的线程分配的优先级略高于其他线程的优先级。
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄。</param>
        /// <returns>如果将窗口带到前台，则返回值为非零值。如果未将窗口带到前台，则返回值为零。</returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetForegroundWindow", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。
        /// </summary>
        /// <param name="hWnd">更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。</param>
        /// <param name="hWndInsertAfter">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="X">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="Y">窗口顶部的新位置，以客户端坐标表示。</param>
        /// <param name="cx">窗口的新宽度（以像素为单位）。</param>
        /// <param name="cy">窗口的新高度（以像素为单位）。</param>
        /// <param name="uFlags">窗口大小调整和定位标志。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "SetWindowPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
    }
}
