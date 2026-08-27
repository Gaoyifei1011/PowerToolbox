using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.FirewallAPI
{
    /// <summary>
    /// 包含有关应用容器功能的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct INET_FIREWALL_AC_CAPABILITIES
    {
        /// <summary>
        /// SID 的安全标识符数。
        /// </summary>
        internal uint count;

        /// <summary>
        /// 与应用容器功能相关的安全信息。
        /// </summary>
        internal nint capabilities;
    }
}
