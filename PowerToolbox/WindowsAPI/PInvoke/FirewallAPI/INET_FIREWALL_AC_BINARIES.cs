using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.FirewallAPI
{
    /// <summary>
    /// 包含应用容器中运行的应用程序的二进制路径。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct INET_FIREWALL_AC_BINARIES
    {
        /// <summary>
        /// 二进制文件成员中的路径数。
        /// </summary>
        internal uint count;

        /// <summary>
        /// 应用容器中运行的应用程序的路径。
        /// </summary>
        internal nint binaries;
    }
}
