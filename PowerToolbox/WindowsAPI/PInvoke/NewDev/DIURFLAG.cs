using System;

namespace PowerToolbox.WindowsAPI.PInvoke.NewDev
{
    [Flags]
    internal enum DIURFLAG
    {
        /// <summary>
        /// 安装的任何设备中删除驱动程序包，但不从驱动程序存储中删除驱动程序包。
        /// </summary>
        NO_REMOVE_INF = 0x00000001,

        UNCONFIGURE_INF = 0x00000002,
    }
}
