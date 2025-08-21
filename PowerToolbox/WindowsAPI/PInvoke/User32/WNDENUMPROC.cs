using System;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 与 EnumWindows 或 EnumDesktopWindows 函数一起使用的应用程序定义的回调函数。它接收顶级窗口句柄。WNDENUMPROC 类型定义指向此回调函数的指针。EnumWindowsProc 是应用程序定义的函数名称的占位符。
    /// </summary>
    /// <param name="hWnd">顶级窗口的句柄。</param>
    /// <param name="lParam">在 EnumWindows 或 EnumDesktopWindows 中给出的应用程序定义值。</param>
    /// <returns>若要继续枚举，回调函数必须返回 TRUE;若要停止枚举，它必须返回 FALSE。</returns>
    public delegate bool WNDENUMPROC(IntPtr hWnd, IntPtr lParam);
}
