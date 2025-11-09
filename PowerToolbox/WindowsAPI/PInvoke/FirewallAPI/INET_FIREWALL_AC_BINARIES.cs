using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.FirewallAPI
{
    /// <summary>
    /// 包含应用容器中运行的应用程序的二进制路径。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct INET_FIREWALL_AC_BINARIES
    {
        /// <summary>
        /// 二进制文件成员中的路径数。
        /// </summary>
        public uint count;

        /// <summary>
        /// 应用容器中运行的应用程序的路径。
        /// </summary>
        public nint binaries;
    }
}
