using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace ThemeSwitchService.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "user32.dll";

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
    }
}
