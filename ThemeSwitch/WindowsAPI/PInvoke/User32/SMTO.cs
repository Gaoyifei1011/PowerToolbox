namespace ThemeSwitch.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// SendMessageTimeout 函数的行为。
    /// </summary>
    public enum SMTO
    {
        /// <summary>
        /// 在等待函数返回时，不会阻止调用线程处理其他请求。
        /// </summary>
        SMTO_NORMAL = 0x0000,

        /// <summary>
        /// 在函数返回之前，阻止调用线程处理任何其他请求。
        /// </summary>
        SMTO_BLOCK = 0x0001,

        /// <summary>
        /// 如果接收线程似乎未响应或“挂起”，则函数返回时不等待超时时间过。
        /// </summary>
        SMTO_ABORTIFHUNG = 0x0002,

        /// <summary>
        /// 只要接收线程正在处理消息，该函数就不会强制实施超时期限。
        /// </summary>
        SMTO_NOTIMEOUTIFNOTHUNG = 0x0008,

        /// <summary>
        /// 如果正在处理消息时，接收窗口被销毁或自己的线程死亡，则函数应返回 0。
        /// </summary>
        SMTO_ERRORONEXIT = 0x0020
    }
}
