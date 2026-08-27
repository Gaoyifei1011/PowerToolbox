namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 指定击键的各个方面。
    /// </summary>
    internal enum KEYEVENTFLAGS
    {
        /// <summary>
        /// 按下密钥。
        /// </summary>
        KEYEVENTF_KEYDOWN = 0x0000,

        /// <summary>
        /// 如果指定， 则 wScan 扫描代码由两个字节组成的序列组成，其中第一个字节的值为 0xE0。
        /// </summary>
        KEYEVENTF_EXTENDEDKEY = 0x0001,

        /// <summary>
        /// 释放密钥。
        /// </summary>
        KEYEVENTF_KEYUP = 0x0002,

        /// <summary>
        /// 如果指定，系统会合成 VK_PACKET 击键。 wVk 参数必须为零。
        /// </summary>
        KEYEVENTF_UNICODE = 0x0004,

        /// <summary>
        /// 如果指定， wScan 将标识密钥，并忽略 wVk 。
        /// </summary>
        KEYEVENTF_SCANCODE = 0x0008
    }
}
