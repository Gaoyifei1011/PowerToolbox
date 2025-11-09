using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.FirewallAPI
{
    /// <summary>
    /// SID_AND_ATTRIBUTES 结构表示 SID) 及其属性安全标识符。 SID 用于唯一标识用户或组。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SID_AND_ATTRIBUTES
    {
        /// <summary>
        /// 指向 SID 结构的指针。
        /// </summary>
        public nint Sid;

        /// <summary>
        /// 指定 SID 的属性。 此值最多包含 32 个一位标志。 其含义取决于 SID 的定义和用法。
        /// </summary>
        public uint Attributes;
    }
}
