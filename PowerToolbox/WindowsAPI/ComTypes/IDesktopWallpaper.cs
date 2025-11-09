using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供用于管理桌面壁纸的方法。
    /// </summary>
    [ComImport, Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDesktopWallpaper
    {
        /// <summary>
        /// 设置桌面壁纸。
        /// </summary>
        /// <param name="monitorID">监视器的 ID。 可以通过 GetMonitorDevicePathAt 获取此值。 将此值设置为 NULL 以在所有监视器上设置壁纸图像。</param>
        /// <param name="wallpaper">壁纸图像文件的完整路径。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.LPWStr)] string wallpaper);

        /// <summary>
        /// 获取当前桌面壁纸。
        /// </summary>
        /// <param name="monitorID">
        /// 监视器的 ID。 可以通过 GetMonitorDevicePathAt 获取此值。
        /// 此值可以设置为 NULL。 在这种情况下，如果单个壁纸图像显示在系统的所有监视器上，该方法将成功返回。 如果此值设置为 NULL ，并且不同的监视器显示不同的壁纸或幻灯片放映正在运行，则该方法在 壁纸 参数中返回S_FALSE和空字符串。
        /// </param>
        /// <param name="wallpaper">
        /// 指向缓冲区的指针的地址，当此方法成功返回时，该缓冲区接收壁纸图像文件的路径。 请注意，此图像当前可能显示在系统的所有监视器上，而不仅仅是 在 monitorID 参数中指定的监视器上。
        /// 如果未显示壁纸图像或监视器显示纯色，则此字符串将为空。 如果 方法失败，字符串也将为空。
        /// </param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.LPWStr)] out string wallpaper);

        /// <summary>
        /// 检索其中一个系统监视器的唯一 ID。
        /// </summary>
        /// <param name="monitorIndex">监视器的编号。 调用 GetMonitorDevicePathCount 以确定监视器的总数。</param>
        /// <param name="monitorID">指向缓冲区地址的指针，当此方法成功返回时，该缓冲区接收监视器的 ID。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetMonitorDevicePathAt(uint monitorIndex, [MarshalAs(UnmanagedType.LPWStr)] out string monitorID);

        /// <summary>
        /// 检索与系统关联的监视器数。
        /// </summary>
        /// <param name="count">指向当此方法成功返回时接收监视器数的值的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetMonitorDevicePathCount(out uint count);

        /// <summary>
        /// 检索指定监视器的显示矩形。
        /// </summary>
        /// <param name="monitorID">要查询的监视器的 ID。 可以通过 GetMonitorDevicePathAt 获取此值。</param>
        /// <param name="displayRect">指向 RECT 结构的指针，此方法成功返回时，以屏幕坐标接收 由 monitorID 指定的监视器的显示矩形。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorID, out RECT displayRect);

        /// <summary>
        /// 设置未显示图像或禁用桌面背景时桌面上可见的颜色。 当桌面壁纸未填满整个屏幕时，此颜色也用作边框。
        /// </summary>
        /// <param name="color">一个 COLORREF 值，该值指定背景 RGB 颜色值。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetBackgroundColor([MarshalAs(UnmanagedType.U4)] uint color);

        /// <summary>
        /// 检索未显示图像或禁用桌面背景时桌面上可见的颜色。 当桌面壁纸未填满整个屏幕时，此颜色也用作边框。
        /// </summary>
        /// <param name="color">指向 COLORREF 值的指针，此方法成功返回时，将接收 RGB 颜色值。 如果此方法失败，此值将设置为 0。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetBackgroundColor(out uint color);

        /// <summary>
        /// 设置桌面壁纸图像的显示选项，确定图像应居中、平铺还是拉伸。
        /// </summary>
        /// <param name="position">DESKTOP_WALLPAPER_POSITION 枚举值之一，用于指定图像在系统监视器上的显示方式。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetPosition(DESKTOP_WALLPAPER_POSITION position);

        /// <summary>
        /// 检索桌面背景图像的当前显示值。
        /// </summary>
        /// <param name="position">指向一个值的指针，当此方法成功返回时，将接收指定图像在系统监视器上的显示方式的 DESKTOP_WALLPAPER_POSITION 枚举值之一。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetPosition(out DESKTOP_WALLPAPER_POSITION position);

        /// <summary>
        /// 指定要用于桌面壁纸幻灯片放映的图像。
        /// </summary>
        /// <param name="items">指向包含幻灯片放映图像的 IShellItemArray 的指针。 此数组可以包含存储在同一容器中的单个项 (存储在文件夹) 中的文件，也可以包含单个项，即容器本身 (包含图像) 的文件夹。 数组的任何其他配置都将导致此方法失败。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetSlideshow(nint items);

        /// <summary>
        /// 获取存储幻灯片放映图像的目录的路径。
        /// </summary>
        /// <param name="items">指向 IShellItemArray 对象的指针的地址，当此方法成功返回时，该对象接收构成幻灯片放映的项。 此数组可以包含存储在同一容器中的单个项，也可以包含作为容器本身的单个项。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSlideshow(out nint items);

        /// <summary>
        /// 设置桌面壁纸幻灯片放映设置，以便进行随机播放和计时。
        /// </summary>
        /// <param name="options">设置为 0 可禁用随机或以下值。</param>
        /// <param name="slideshowTick">图像转换之间的时间（以毫秒为单位）。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetSlideshowOptions(DESKTOP_SLIDESHOW_OPTIONS options, uint slideshowTick);

        /// <summary>
        /// 获取用于随机播放和计时的当前桌面壁纸幻灯片放映设置。
        /// </summary>
        /// <param name="options">指向一个值的指针，当此方法成功返回时，该值接收 0 以指示已禁用随机播放或以下值。</param>
        /// <param name="slideshowTick">指向一个值的指针，当此方法成功返回时，该值接收图像转换之间的间隔（以毫秒为单位）。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetSlideshowOptions(DESKTOP_SLIDESHOW_OPTIONS options, out uint slideshowTick);

        /// <summary>
        /// 将指定监视器上的壁纸切换到幻灯片放映中的下一个图像。
        /// </summary>
        /// <param name="monitorID">要更改壁纸图像的监视器的 ID。 可以通过 GetMonitorDevicePathAt 方法获取此 ID。 如果此参数设置为 NULL，则使用计划接下来更改的监视器。</param>
        /// <param name="direction">幻灯片放映应前进的方向。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int AdvanceSlideshow([MarshalAs(UnmanagedType.LPWStr)] string monitorID, DESKTOP_SLIDESHOW_DIRECTION direction);

        /// <summary>
        /// 获取幻灯片放映的当前状态。
        /// </summary>
        /// <param name="state">指向DESKTOP_SLIDESHOW_STATE值的指针，当此方法成功返回时，该值接收以下一个或多个标志。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetStatus(out DESKTOP_SLIDESHOW_STATE state);

        /// <summary>
        /// 启用或禁用桌面背景。
        /// </summary>
        /// <param name="enable">如果为 TRUE ，则启用桌面背景; 如果为 FALSE ，则禁用它。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Enable([MarshalAs(UnmanagedType.Bool)] out bool enable);
    }
}
