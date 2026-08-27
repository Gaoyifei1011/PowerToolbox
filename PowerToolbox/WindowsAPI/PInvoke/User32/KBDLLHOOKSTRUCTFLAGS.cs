using System;

namespace PowerToolbox.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 扩展键标志、事件注入标志、上下文代码和转换状态标志。
    /// </summary>
    [Flags]
    internal enum KBDLLHOOKSTRUCTFLAGS
    {
        /// <summary>
        /// 测试扩展键标志。
        /// </summary>
        LLKHF_EXTENDED = 0x01,

        /// <summary>
        /// 从 (以较低完整性级别) 标志运行的进程测试事件注入。
        /// </summary>
        LLKHF_LOWER_IL_INJECTED = 0x02,

        /// <summary>
        /// 从(任何进程) 标志测试事件注入。
        /// </summary>
        LLKHF_INJECTED = 0x10,

        /// <summary>
        /// 测试上下文代码。
        /// </summary>
        LLKHF_ALTDOWN = 0x20,

        /// <summary>
        /// 测试转换状态标志。
        /// </summary>
        LLKHF_UP = 0x80
    }
}
