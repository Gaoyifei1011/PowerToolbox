namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 收缩位图标志
    /// </summary>
    internal enum SIIGBF
    {
        /// <summary>
        /// 根据需要收缩位图以适应，保留其纵横比。
        /// </summary>
        SIIGBF_RESIZETOFIT = 0x00000000,

        /// <summary>
        /// 如果调用方想要拉伸返回的图像本身，则通过呼叫者传递。 例如，如果调用方传递图标大小为 80x80，则可能会返回 96x96 缩略图。 如果调用方希望它们需要拉伸图像，则可以将此操作用作性能优化。 请注意，IShellItemImageFactory 的 Shell 实现执行 GDI 拉伸 blit。 如果调用方希望比通过该机制提供的质量更高的图像拉伸，则应传递此标志并自行执行拉伸。
        /// </summary>
        SIIGBF_BIGGERSIZEOK = 0x00000001,

        /// <summary>
        /// 仅当项已在内存中时返回。 即使缓存了项，也不访问磁盘。 请注意，仅返回已缓存的图标，如果某个项具有尚未缓存的每个实例图标，则可以回退到每类图标。 检索缩略图（即使缓存了缩略图）始终需要访问磁盘，因此不应从 UI 线程调用 GetImage，而无需传递 SIIGBF_MEMORYONLY。
        /// </summary>
        SIIGBF_MEMORYONLY = 0x00000002,

        /// <summary>
        /// 仅返回图标，从不返回缩略图。
        /// </summary>
        SIIGBF_ICONONLY = 0x00000004,

        /// <summary>
        /// 仅返回缩略图，从不返回图标。 请注意，并非所有项目都有缩略图，因此 SIIGBF_THUMBNAILONLY 会导致方法在这些情况下失败。
        /// </summary>
        SIIGBF_THUMBNAILONLY = 0x00000008,

        /// <summary>
        /// 允许访问磁盘，但只允许检索缓存的项。 如果缓存的缩略图可用，则返回该缩略图。 如果没有可用的缓存缩略图，它将返回缓存的每个实例图标，但不提取缩略图或图标。
        /// </summary>
        SIIGBF_INCACHEONLY = 0x00000010,

        /// <summary>
        /// Windows 8中引入的 。 如有必要，将位图裁剪为正方形。
        /// </summary>
        SIIGBF_CROPTOSQUARE = 0x00000020,

        /// <summary>
        /// Windows 8中引入的 。 拉伸位图并将其裁剪为 0.7 纵横比。
        /// </summary>
        SIIGBF_WIDETHUMBNAILS = 0x00000040,

        /// <summary>
        /// Windows 8中引入的 。 如果返回图标，请使用关联的应用的注册背景色绘制背景。
        /// </summary>
        SIIGBF_ICONBACKGROUND = 0x00000080,

        /// <summary>
        /// Windows 8中引入的 。 如有必要，拉伸位图，使高度和宽度适合给定大小。
        /// </summary>
        SIIGBF_SCALEUP = 0x00000100
    }
}
