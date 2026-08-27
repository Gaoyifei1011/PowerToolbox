namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 指定桌面壁纸的显示方式。
    /// </summary>
    internal enum DESKTOP_WALLPAPER_POSITION
    {
        /// <summary>
        /// 将图像居中;不要拉伸。 这相当于 IActiveDesktop 中的WPSTYLE_CENTER样式。
        /// </summary>
        DWPOS_CENTER = 0,

        /// <summary>
        /// 在所有监视器上平铺图像。 这相当于 IActiveDesktop 中的WPSTYLE_TILE样式。
        /// </summary>
        DWPOS_TILE = 1,

        /// <summary>
        /// 拉伸图像以完全适合监视器。 这相当于 IActiveDesktop中的WPSTYLE_STRETCH样式。
        /// </summary>
        DWPOS_STRETCH = 2,

        /// <summary>
        /// 将图像拉伸到监视器的高度或宽度，而无需更改其纵横比或裁剪图像。 这可能会导致图像的两侧或上方和下方出现 彩色信箱条 。 这相当于 IActiveDesktop 中的WPSTYLE_KEEPASPECT样式。
        /// </summary>
        DWPOS_FIT = 3,

        /// <summary>
        /// 拉伸图像以填充屏幕，根据需要裁剪图像以避免使用信箱栏。 这相当于 IActiveDesktop 中的WPSTYLE_CROPTOFIT样式。
        /// </summary>
        DWPOS_FILL = 4,

        /// <summary>
        /// 跨连接到系统的所有监视器跨单个映像。 此标志没有 等效的 IActiveDesktop 。
        /// </summary>
        DWPOS_SPAN = 5
    }
}
