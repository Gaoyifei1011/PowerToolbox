using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 表示包版本信息。
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    internal struct PACKAGE_VERSION
    {
        /// <summary>
        /// 以单个整型值表示的包的完整版本号。
        /// </summary>
        [FieldOffset(0)]
        internal ulong Version;

        [FieldOffset(0)]
        internal DUMMYSTRUCTNAME Parts;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DUMMYSTRUCTNAME
        {
            /// <summary>
            /// 包的修订版本号。
            /// </summary>
            internal ushort Revision;

            /// <summary>
            /// 包的内部版本号。
            /// </summary>
            internal ushort Build;

            /// <summary>
            /// 包的次要版本号。
            /// </summary>
            internal ushort Minor;

            /// <summary>
            /// 包的主版本号。
            /// </summary>
            internal ushort Major;
        }
    }
}
