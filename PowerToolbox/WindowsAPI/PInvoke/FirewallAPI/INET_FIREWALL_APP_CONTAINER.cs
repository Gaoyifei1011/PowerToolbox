using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.FirewallAPI
{
    /// <summary>
    /// 包含有关特定应用容器的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct INET_FIREWALL_APP_CONTAINER
    {
        /// <summary>
        /// 应用容器的包标识符
        /// </summary>
        internal nint appContainerSid;

        /// <summary>
        /// 安全标识符 (应用容器所属用户的 SID) 。
        /// </summary>
        internal nint userSid;

        /// <summary>
        /// 应用容器的全局唯一名称。对于 Windows 应用商店应用的应用容器，也称为“包系列名称”。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string appContainerName;

        /// <summary>
        /// 应用容器的友好名称
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string displayName;

        /// <summary>
        /// 应用容器 (其用途的说明、使用该容器的应用程序的目标等 )
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string description;

        /// <summary>
        /// 应用容器的功能。
        /// </summary>
        internal INET_FIREWALL_AC_CAPABILITIES capabilities;

        /// <summary>
        /// 应用容器中运行的应用程序的二进制路径。
        /// </summary>
        internal INET_FIREWALL_AC_BINARIES binaries;

        /// <summary>
        /// 应用容器的工作目录
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string workingDirectory;

        /// <summary>
        /// 应用容器的包全部名称
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string packageFullName;
    }
}
