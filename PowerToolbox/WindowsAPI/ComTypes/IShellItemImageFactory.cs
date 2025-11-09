using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 公开用于返回 Shell 项图标或缩略图的方法。 如果请求的项没有可用的缩略图或图标，可以从 Shell 提供每个类的图标。
    /// </summary>
    [ComImport, Guid("BCC18B79-BA16-442F-80C4-8A59C30C463B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItemImageFactory
    {
        /// <summary>
        /// 获取表示 IShellItem的 HBITMAP。 默认行为是加载缩略图。 如果当前 IShellItem没有缩略图，它将检索项图标的 HBITMAP。 如果未缓存缩略图或图标，则会提取该缩略图或图标。
        /// </summary>
        /// <param name="size">一个结构，指定要接收的图像的大小。</param>
        /// <param name="flags">收缩位图标志</param>
        /// <param name="phbm">指向此方法成功返回的值的指针，它接收检索到的位图的句柄。 调用方有责任在不再需要资源时通过 DeleteObject 释放检索到的资源。</param>
        /// <returns>如果此方法成功，则返回 S_OK。 否则，它将返回 HRESULT 错误代码。</returns>
        [PreserveSig]
        int GetImage(Size size, SIIGBF flags, out nint phbm);
    }
}
