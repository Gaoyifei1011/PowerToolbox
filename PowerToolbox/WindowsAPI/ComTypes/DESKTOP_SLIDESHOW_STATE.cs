using System;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// 幻灯片放映的当前状态
    /// </summary>
    [Flags]
    internal enum DESKTOP_SLIDESHOW_STATE
    {
        /// <summary>
        /// 已启用幻灯片放映。
        /// </summary>
        DSS_ENABLED = 0x1,

        /// <summary>
        /// 当前已配置幻灯片放映。
        /// </summary>
        DSS_SLIDESHOW = 0x2,

        /// <summary>
        /// 远程会话已暂时禁用幻灯片放映。
        /// </summary>
        DSS_DISABLED_BY_REMOTE_SESSION = 0x4,
    }
}
