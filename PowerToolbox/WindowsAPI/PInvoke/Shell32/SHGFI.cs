namespace PowerToolbox.WindowsAPI.PInvoke.Shell32
{
    public enum SHGFI : uint
    {
        /// <summary>
        /// 修改 SHGFI_ICON，导致函数检索文件的大型图标。 还必须设置 SHGFI_ICON 标志。
        /// </summary>
        SHGFI_LARGEICON = 0x000000000,

        /// <summary>
        /// 修改 SHGFI_ICON，导致函数检索文件的小图标。 还用于修改 SHGFI_SYSICONINDEX，导致函数将句柄返回到包含小图标图像的系统映像列表。 还必须设置 SHGFI_ICON 和/或 SHGFI_SYSICONINDEX 标志。
        /// </summary>
        SHGFI_SMALLICON = 0x000000001,

        /// <summary>
        /// 修改 SHGFI_ICON，导致函数检索文件的打开图标。 还用于修改 SHGFI_SYSICONINDEX，导致函数将句柄返回到包含文件小打开图标的系统映像列表。 容器对象显示一个打开的图标，指示容器处于打开状态。 还必须设置 SHGFI_ICON 和/或 SHGFI_SYSICONINDEX 标志。
        /// </summary>
        SHGFI_OPENICON = 0x000000002,

        /// <summary>
        /// 修改 SHGFI_ICON，导致函数检索 Shell 大小的图标。 如果未指定此标志，函数将根据系统指标值调整图标的大小。 还必须设置 SHGFI_ICON 标志。
        /// </summary>
        SHGFI_SHELLICONSIZE = 0x000000004,

        /// <summary>
        /// 指示 pszPath 是 ITEMIDLIST 结构的地址，而不是路径名称。
        /// </summary>
        SHGFI_PIDL = 0x000000008,

        /// <summary>
        /// 指示函数不应尝试访问由 pszPath指定的文件。 相反，它应该像 pszPath 指定的文件与传入 dwFileAttributes中的文件属性一样。 此标志不能与 SHGFI_ATTRIBUTES、SHGFI_EXETYPE或 SHGFI_PIDL 标志结合使用。
        /// </summary>
        SHGFI_USEFILEATTRIBUTES = 0x000000010,

        /// <summary>
        /// 版本 5.0。 将适当的覆盖应用于文件的图标。 还必须设置 SHGFI_ICON 标志。
        /// </summary>
        SHGFI_ADDOVERLAYS = 0x000000020,

        /// <summary>
        /// 版本 5.0。 返回覆盖图标的索引。 覆盖索引的值在由 psfi指定的结构的 iIcon 成员的上八位中返回。 此标志还需要设置 SHGFI_ICON。
        /// </summary>
        SHGFI_OVERLAYINDEX = 0x000000040,

        /// <summary>
        /// 检索表示系统映像列表中的图标的文件和图标索引的句柄。 句柄将复制到由 psfi指定的结构的 hIcon 成员，并将索引复制到 iIcon 成员。
        /// </summary>
        SHGFI_ICON = 0x000000100,

        /// <summary>
        /// 检索该文件的显示名称，该文件在 Windows 资源管理器中显示的名称。 该名称将复制到 szDisplayNamepsfi中指定的结构的成员。 返回的显示名称使用长文件名（如果有）而不是文件名的 8.3 形式。 请注意，显示名称可能会受到设置的影响，例如是否显示扩展。
        /// </summary>
        SHGFI_DISPLAYNAME = 0x000000200,

        /// <summary>
        /// 检索描述文件类型的字符串。 字符串将复制到 szTypeNamepsfi中指定的结构的成员。
        /// </summary>
        SHGFI_TYPENAME = 0x000000400,

        /// <summary>
        /// 检索项属性。 属性将复制到 dwAttributespsfi 参数中指定的结构的成员。 这些属性与从 IShellFolder：：GetAttributesOf获取的属性相同。
        /// </summary>
        SHGFI_ATTRIBUTES = 0x000000800,

        /// <summary>
        /// 检索包含表示由 pszPath指定的文件的图标的文件的名称，如文件的图标处理程序的 IExtractIcon：：GetIconLocation 方法返回。 此外，检索该文件中的图标索引。 包含图标的文件的名称将复制到由 psfi指定的结构的 szDisplayName 成员。 图标的索引将复制到该结构的 iIcon 成员。
        /// </summary>
        SHGFI_ICONLOCATION = 0x000001000,

        /// <summary>
        /// 如果 pszPath 标识可执行文件，则检索可执行文件的类型。 信息打包到返回值中。 不能使用任何其他标志指定此标志。
        /// </summary>
        SHGFI_EXETYPE = 0x000002000,

        /// <summary>
        /// 检索系统映像列表图标的索引。 如果成功，索引将复制到 psfi的 iIcon 成员。 返回值是系统映像列表的句柄。 只有成功将索引复制到 iIcon 的图像才有效。 尝试访问系统映像列表中的其他映像将导致未定义的行为。
        /// </summary>
        SHGFI_SYSICONINDEX = 0x000004000,

        /// <summary>
        /// 修改 SHGFI_ICON，导致函数将链接覆盖添加到文件的图标。 还必须设置 SHGFI_ICON 标志。
        /// </summary>
        SHGFI_LINKOVERLAY = 0x000008000,

        /// <summary>
        /// 修改 SHGFI_ICON，导致函数将文件的图标与系统突出显示颜色混合。 还必须设置 SHGFI_ICON 标志。
        /// </summary>
        SHGFI_SELECTED = 0x000010000,

        /// <summary>
        /// 修改 SHGFI_ATTRIBUTES 以指示 dwAttributesSHFILEINFO 结构（psfi）包含所需的特定属性。 这些属性将传递给 IShellFolder：：GetAttributesOf。 如果未指定此标志，0xFFFFFFFF将传递给 IShellFolder：：GetAttributesOf，请求所有属性。 不能使用 SHGFI_ICON 标志指定此标志。
        /// </summary>
        SHGFI_ATTR_SPECIFIED = 0x000020000,
    }
}
