using System;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    [Flags]
    internal enum DESKTOP_SLIDESHOW_OPTIONS
    {
        /// <summary>
        /// 已启用 Shuffle;图像按随机顺序显示。
        /// </summary>
        DSO_SHUFFLEIMAGES = 0x1,
    }
}
