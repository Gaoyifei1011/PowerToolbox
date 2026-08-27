using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.KernelAppCore
{
    /// <summary>
    /// 表示包标识信息，例如名称、版本和发布者。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct PACKAGE_ID
    {
        /// <summary>
        /// 保留值；请勿使用。
        /// </summary>
        internal uint reserved;

        /// <summary>
        /// 包的处理器体系结构。
        /// </summary>
        internal PROCESSOR_ARCHITECTURE processorArchitecture;

        /// <summary>
        /// 包的版本。
        /// </summary>
        internal PACKAGE_VERSION version;

        /// <summary>
        /// 包的名称。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string name;

        /// <summary>
        /// 包的发布者。 如果包没有发布者，则此成员为 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string publisher;

        /// <summary>
        /// 资源标识符 (包的 ID) 。 如果包没有资源 ID，则此成员为 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string resourceId;

        /// <summary>
        /// 发布者标识符 (包的 ID) 。 如果包没有发布者 ID，则此成员为 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string publisherId;
    }
}
