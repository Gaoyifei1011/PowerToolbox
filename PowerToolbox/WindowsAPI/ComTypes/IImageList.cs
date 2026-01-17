using Microsoft.UI.Xaml.Shapes;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开操作图像列表并与之交互的方法。
    /// </summary>
    [ComImport, Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImageList
    {
        /// <summary>
        /// 向图像列表添加图像。
        /// </summary>
        /// <param name="hbmImage">包含图像或图像的位图的句柄。 图像数是从位图的宽度推断出来的。</param>
        /// <param name="hbmMask">包含掩码的位图的句柄。 如果未将蒙板用于图像列表，则忽略此参数。</param>
        /// <param name="pi">此方法返回时，包含指向第一个新图像的索引的指针。 如果 方法无法成功添加新映像，则此值为 -1。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Add(nint hbmImage, nint hbmMask, out int pi);

        /// <summary>
        /// 将图像替换为图标或光标。
        /// </summary>
        /// <param name="i">一个 int 类型的值，该值包含要替换的图像的索引。 如果 i 为 -1，则 函数会将图像添加到列表的末尾。</param>
        /// <param name="hicon">图标或光标的句柄，其中包含新图像的位图和掩码。</param>
        /// <param name="pi">指向 int 的指针，如果返回成功，则包含图像的索引;否则为 -1。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int ReplaceIcon(int i, nint hicon, out int pi);

        /// <summary>
        /// 将指定图像添加到用作覆盖掩码的图像列表中。
        /// </summary>
        /// <param name="iImage">int 类型的值，包含图像列表中图像的从零开始的索引。 此索引标识要用作覆盖掩码的图像。</param>
        /// <param name="iOverlay">int 类型的值，包含覆盖掩码的从 1 开始的索引。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetOverlayImage(int iImage, int iOverlay);

        /// <summary>
        /// 将图像列表中的图像替换为新图像。
        /// </summary>
        /// <param name="i">一个 int 类型的值，该值包含要替换的图像的索引。</param>
        /// <param name="hbmImage">包含图像的位图的句柄。</param>
        /// <param name="hbmMask">包含掩码的位图的句柄。 如果未将蒙板用于图像列表，则忽略此参数。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Replace(int i, nint hbmImage, nint hbmMask);

        /// <summary>
        /// 将一个或多个图像添加到图像列表，从指定的位图生成掩码。
        /// </summary>
        /// <param name="hbmImage">包含一个或多个图像的位图的句柄。 图像数是从位图的宽度推断出来的。</param>
        /// <param name="crMask">用于生成掩码的颜色。 指定位图中此颜色的每个像素都更改为黑色，掩码中的相应位设置为 1。 如果此参数为CLR_DEFAULT，则使用 (0,0) 的像素颜色作为掩码。</param>
        /// <param name="pi">指向 int 的指针，该指针包含返回时第一个新图像的索引（如果成功），否则为 -1。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int AddMasked(nint hbmImage, int crMask, out int pi);

        /// <summary>
        /// 在指定的设备上下文中绘制图像列表项。
        /// </summary>
        /// <param name="pimldp">指向包含绘图参数的 IMAGELISTDRAWPARAMS 结构的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Draw(nint pimldp);

        /// <summary>
        /// 从图像列表中移除图像。
        /// </summary>
        /// <param name="i">一个 int 类型的值，该值包含要删除的图像的索引。 如果此参数为 -1，则 方法将删除所有图像。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Remove(int i);

        /// <summary>
        /// 从图像和图像列表中的掩码创建图标。
        /// </summary>
        /// <param name="i">包含图像索引的 int 类型的值。</param>
        /// <param name="flags">指定绘图样式的标志的组合。 有关值列表，请参阅 IImageList：:D raw。</param>
        /// <param name="picon">指向包含图标句柄（如果成功）的 int 的指针;否则为 NULL 。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetIcon(int i, uint flags, out nint picon);

        /// <summary>
        /// 获取有关图像的信息。
        /// </summary>
        /// <param name="i">包含图像索引的 int 类型的值。</param>
        /// <param name="pImageInfo">指向 IMAGEINFO 结构的指针，该结构接收有关图像的信息。 此结构中的信息可以直接操作图像的位图。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetImageInfo(int i, out nint pImageInfo);

        /// <summary>
        /// 从给定图像列表中复制图像。
        /// </summary>
        /// <param name="iDst">int 类型的值，包含复制操作的目标映像的从零开始的索引。</param>
        /// <param name="punkSrc">指向源图像列表的 IUnknown 接口的指针。</param>
        /// <param name="iSrc">int 类型的值，包含复制操作源图像的从零开始的索引。</param>
        /// <param name="uFlags">一个 值，该值指定要进行的复制操作的类型。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Copy(int iDst, [MarshalAs(UnmanagedType.Interface)] IImageList punkSrc, int iSrc, int uFlags);

        /// <summary>
        ///
        /// </summary>
        /// <param name="i1">一个 int 类型的值，该值包含第一个现有图像的索引。</param>
        /// <param name="punk2">指向包含第二个图像的图像列表的 IUnknown 接口的指针。</param>
        /// <param name="i2">一个 int 类型的值，该值包含第二个现有图像的索引。</param>
        /// <param name="dx">一个 int 类型的值，该值包含第二个图像相对于第一个图像的偏移量的 x 分量。</param>
        /// <param name="dy">int 类型的值，该值包含第二个图像相对于第一个图像的偏移量的 y 分量。</param>
        /// <param name="riid">新映像列表的接口的 IID。</param>
        /// <param name="ppv">指向新图像列表接口的原始指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Merge(int i1, [MarshalAs(UnmanagedType.Interface)] IImageList punk2, int i2, int dx, int dy, ref Guid riid, out nint ppv);

        /// <summary>
        /// 克隆现有映像列表。
        /// </summary>
        /// <param name="riid">新映像列表的 IID。</param>
        /// <param name="ppv">指向新图像列表接口的指针的地址。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int Clone(ref Guid riid, out nint ppv);

        /// <summary>
        /// 获取图像的边框。
        /// </summary>
        /// <param name="i">包含图像索引的 int 类型的值。</param>
        /// <param name="prc">指向 RECT 的指针，该方法返回时包含边框。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetImageRect(int i, ref RECT prc);

        /// <summary>
        /// 获取图像列表中图像的尺寸。 图像列表中的所有图像具有相同的尺寸。
        /// </summary>
        /// <param name="cx">指向接收每个图像的宽度（以像素为单位）的 int 的指针。</param>
        /// <param name="cy">指向接收每个图像的高度（以像素为单位）的 int 的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetIconSize(out int cx, out int cy);

        /// <summary>
        /// 设置图像列表中图像的尺寸，并从列表中删除所有图像。
        /// </summary>
        /// <param name="cx">int 类型的值，包含图像列表中图像的宽度（以像素为单位）。 图像列表中的所有图像具有相同的尺寸。</param>
        /// <param name="cy">int 类型的值，该值包含图像列表中图像的高度（以像素为单位）。 图像列表中的所有图像具有相同的尺寸。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetIconSize(int cx, int cy);

        /// <summary>
        /// 获取图像列表中的图像数。
        /// </summary>
        /// <param name="pi">指向 int 的指针，该方法返回时包含图像数。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetImageCount(out int pi);

        /// <summary>
        /// 调整现有图像列表的大小。
        /// </summary>
        /// <param name="uNewCount">一个 值，该值指定图像列表的新大小。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetImageCount(int uNewCount);

        /// <summary>
        /// 设置图像列表的背景色。 仅当向图像列表添加图标或使用 IImageList：：AddMasked 方法添加黑白位图时，此方法才可用。 如果没有蒙板，则会绘制整个图像，并且背景色不可见。
        /// </summary>
        /// <param name="clrBk">要设置的背景色。 如果此参数设置为 CLR_NONE，则使用 掩码以透明方式绘制图像。</param>
        /// <param name="pclr">指向返回时包含上一背景色的 COLORREF 的指针，否则CLR_NONE。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetBkColor(int clrBk, out int pclr);

        /// <summary>
        /// 获取图像列表的当前背景色。
        /// </summary>
        /// <param name="pclr">指向在方法返回时包含背景色的 COLORREF 的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetBkColor(out int pclr);

        /// <summary>
        /// 开始拖动图像。
        /// </summary>
        /// <param name="iTrack">一个 int 类型的值，该值包含要拖动的图像的索引。</param>
        /// <param name="dxHotspot">int 类型的值，该值包含相对于图像左上角的拖动位置的 x 分量。</param>
        /// <param name="dyHotspot">int 类型的值，该值包含相对于图像左上角的拖动位置的 y 分量。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);

        /// <summary>
        /// 结束拖动操作。
        /// </summary>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int EndDrag();

        /// <summary>
        /// 在拖动操作期间锁定指定窗口的更新，并在窗口中的指定位置显示拖动图像。
        /// </summary>
        /// <param name="hwndLock">拥有拖动图像的窗口的句柄。</param>
        /// <param name="x">显示拖动图像的 x 坐标。 坐标相对于窗口的左上角，而不是工作区。</param>
        /// <param name="y">显示拖动图像的 y 坐标。 坐标相对于窗口的左上角，而不是工作区。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int DragEnter(nint hwndLock, int x, int y);

        /// <summary>
        /// 解锁指定窗口并隐藏拖动图像，使窗口能够更新。
        /// </summary>
        /// <param name="hwndLock">包含拖动图像的窗口的句柄。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int DragLeave(nint hwndLock);

        /// <summary>
        /// 移动在拖放操作期间拖动的图像。 通常调用此函数以响应 WM_MOUSEMOVE 消息。
        /// </summary>
        /// <param name="x">一个 int 类型的值，该值包含显示拖动图像的 x 坐标。 坐标相对于窗口的左上角，而不是工作区。</param>
        /// <param name="y">一个 int 类型的值，该值包含显示拖动图像的 y 坐标。 坐标相对于窗口的左上角，而不是工作区。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int DragMove(int x, int y);

        /// <summary>
        /// 通过将指定的图像（通常是鼠标光标图像）与当前拖动图像相结合，创建新的拖动图像。
        /// </summary>
        /// <param name="punk">指向访问图像列表接口的 IUnknown 接口的指针，该接口包含要与拖动图像合并的新图像。</param>
        /// <param name="iDrag">一个 int 类型的值，该值包含要与拖动图像合并的新图像的索引。</param>
        /// <param name="dxHotspot">一个 int 类型的值，该值包含新图像中热点的 x 分量。</param>
        /// <param name="dyHotspot">一个 int 类型的值，该值包含新图像中热点的 x 分量。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int SetDragCursorImage([MarshalAs(UnmanagedType.Interface)] IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

        /// <summary>
        /// 显示或隐藏正在拖动的图像。
        /// </summary>
        /// <param name="fShow">一个 值，该值指定是显示还是隐藏正在拖动的图像。 指定 TRUE 以显示图像，指定 FALSE 以隐藏图像。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int DragShowNolock([MarshalAs(UnmanagedType.Bool)] bool fShow);

        /// <summary>
        /// 获取用于拖动图像的临时图像列表。 此函数还将检索当前拖动位置和拖动图像相对于拖动位置的偏移量。
        /// </summary>
        /// <param name="ppt">指向接收当前拖动位置的 POINT 结构的指针。 可以为 NULL。</param>
        /// <param name="pptHotspot">指向 POINT 结构的指针，该结构接收拖动图像相对于拖动位置的偏移量。 可以为 NULL。</param>
        /// <param name="riid">映像列表的 IID。</param>
        /// <param name="ppv">如果成功，则为指向图像列表接口的指针的地址;否则为 NULL 。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetDragImage(out Point ppt, out Point pptHotspot, ref Guid riid, out nint ppv);

        /// <summary>
        /// 获取图像的标志。
        /// </summary>
        /// <param name="i">int 类型的值，包含需要检索其标志的图像的索引。</param>
        /// <param name="dwFlags">指向方法返回时包含标志的 DWORD 的指针。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetItemFlags(int i, out int dwFlags);

        /// <summary>
        /// 从用作覆盖掩码的图像列表中检索指定的图像。
        /// </summary>
        /// <param name="iOverlay">一个 int 类型的值，该值包含覆盖掩码的从 1 开始的索引。</param>
        /// <param name="piIndex">指向接收从零开始的索引的 int 的指针映像列表中的图像。 此索引标识用作覆盖掩码的图像。</param>
        /// <returns>如果该方法成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetOverlayImage(int iOverlay, out int piIndex);
    }
}
