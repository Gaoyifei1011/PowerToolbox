using System;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    [Flags]
    internal enum SPIF
    {
        None = 0x00,

        /// <summary>
        /// 将新的系统范围参数设置写入用户配置文件。
        /// </summary>
        SPIF_UPDATEINIFILE = 0x01,

        /// <summary>
        /// 在更新用户配置文件后广播 WM_SETTINGCHANGE 消息。
        /// </summary>
        SPIF_SENDCHANGE = 0x02,

        /// <summary>
        /// 与 SPIF_SENDCHANGE 相同。
        /// </summary>
        SPIF_SENDWININICHANGE = SPIF_SENDCHANGE
    }
}
