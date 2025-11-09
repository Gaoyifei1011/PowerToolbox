using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PowerToolbox.Helpers.Root
{
    /// <summary>
    /// 文件缩略图辅助类
    /// </summary>
    public static class ThumbnailHelper
    {
        /// <summary>
        /// 获取文件缩略图
        /// </summary>
        public static Bitmap GetThumbnailBitmap(string filePath)
        {
            try
            {
                int result = Shell32Library.SHCreateItemFromParsingName(filePath, null, typeof(IShellItem).GUID, out IShellItem shellItem);

                if (result is 0)
                {
                    result = ((IShellItemImageFactory)shellItem).GetImage(new Size(256, 256), SIIGBF.SIIGBF_RESIZETOFIT, out nint hBitmap);
                    Marshal.ReleaseComObject(shellItem);

                    if (result is 0)
                    {
                        Bitmap bitmap = Image.FromHbitmap(hBitmap);

                        if (Image.GetPixelFormatSize(bitmap.PixelFormat) < 32)
                        {
                            return bitmap;
                        }
                        else
                        {
                            return CreateAlphaBitmap(bitmap, PixelFormat.Format32bppArgb);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThumbnailHelper), nameof(GetThumbnailBitmap), 1, e);
                return null;
            }
        }

        /// <summary>
        /// 创建 Alpha 的 Bitmap
        /// </summary>
        private static Bitmap CreateAlphaBitmap(Bitmap srcBitmap, PixelFormat targetPixelFormat)
        {
            Bitmap bitmap = new(srcBitmap.Width, srcBitmap.Height, targetPixelFormat);
            Rectangle bitmapBound = new(0, 0, srcBitmap.Width, srcBitmap.Height);
            BitmapData srcData = srcBitmap.LockBits(bitmapBound, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);
            bool isAlplaBitmap = false;

            try
            {
                for (int y = 0; y <= srcData.Height - 1; y++)
                {
                    for (int x = 0; x <= srcData.Width - 1; x++)
                    {
                        Color pixelColor = Color.FromArgb(Marshal.ReadInt32(srcData.Scan0, (srcData.Stride * y) + (4 * x)));

                        if (pixelColor.A > 0 & pixelColor.A < 255)
                        {
                            isAlplaBitmap = true;
                        }

                        bitmap.SetPixel(x, y, pixelColor);
                    }
                }
            }
            finally
            {
                srcBitmap.UnlockBits(srcData);
            }

            return isAlplaBitmap ? bitmap : srcBitmap;
        }
    }
}
