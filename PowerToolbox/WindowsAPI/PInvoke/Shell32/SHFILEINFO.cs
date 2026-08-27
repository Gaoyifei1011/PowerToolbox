using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 包含有关文件对象的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct SHFILEINFO
    {
        /// <summary>
        /// 表示文件的图标的句柄。 当不再需要此句柄时，你负责销毁此句柄，DestroyIcon。
        /// </summary>
        internal nint hIcon;

        /// <summary>
        /// 系统映像列表中的图标图像的索引。
        /// </summary>
        internal int iIcon;

        /// <summary>
        /// 一个值数组，指示文件对象的属性。 有关这些值的信息，请参阅 IShellFolder：：GetAttributesOf 方法。
        /// </summary>
        internal uint dwAttributes;

        /// <summary>
        /// 包含 Windows Shell 中显示的文件名称的字符串，或包含表示文件的图标的文件的路径和文件名。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szDisplayName;

        /// <summary>
        /// 描述文件类型的字符串。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        internal string szTypeName;
    }
}
