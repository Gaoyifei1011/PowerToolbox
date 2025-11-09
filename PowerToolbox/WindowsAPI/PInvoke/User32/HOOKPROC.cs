using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 与 SetWindowsHookEx 函数一起使用的应用程序定义的或库定义的回调函数。 在调用 SendMessage 函数后，系统会调用此函数。 挂钩过程可以检查消息;它不能修改它。
    /// HOOKPROC 类型定义指向此回调函数的指针。 CallWndRetProc 是应用程序定义的或库定义的函数名称的占位符。
    /// </summary>
    /// <param name="nCode">钩子代码传递给当前的钩子过程。下一个钩子过程使用此代码来确定如何处理挂钩信息。</param>
    /// <param name="wParam">指定消息是否由当前进程发送。 如果消息由当前进程发送，则为非零;否则为 NULL。</param>
    /// <param name="lParam">指向 CWPRETSTRUCT 结构的指针，该结构包含有关消息的详细信息。</param>
    /// <returns>
    /// 如果 nCode 小于零，则挂钩过程必须返回 CallNextHookEx 函数返回的值。
    ///  如果 nCode 大于或等于零，强烈建议调用 CallNextHookEx 函数 并返回它返回的值; 否则，已安装 WH_CALLWNDPROCRET 挂钩的其他应用程序将不会收到挂钩通知，并可能因此行为不正确。 如果挂钩过程不调用 CallNextHookEx，则返回值应为零。
    /// </returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint HOOKPROC(int nCode, nuint wParam, nint lParam);
}
