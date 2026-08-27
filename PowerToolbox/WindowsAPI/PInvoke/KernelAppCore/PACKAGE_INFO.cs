using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 表示包标识信息，其中包括包标识符、全名和安装位置。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct PACKAGE_INFO
    {
        /// <summary>
        /// 保留值；请勿使用。
        /// </summary>
        internal uint reserved;

        /// <summary>
        /// 包的属性。
        /// </summary>
        internal uint flags;

        /// <summary>
        /// 包的位置。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string path;

        /// <summary>
        /// 包全名。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string packageFullName;

        /// <summary>
        /// 包系列名称。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)] internal string packageFamilyName;

        /// <summary>
        /// 包标识符 (ID) 。
        /// </summary>
        internal PACKAGE_ID packageId;
    }
}
