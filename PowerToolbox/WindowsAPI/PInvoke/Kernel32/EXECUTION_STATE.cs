using System;

namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 线程的执行要求。
    /// </summary>
    [Flags]
    internal enum EXECUTION_STATE : uint
    {
        /// <summary>
        /// 通过重置系统空闲计时器强制系统处于工作状态。
        /// </summary>
        ES_SYSTEM_REQUIRED = 0x00000001,

        /// <summary>
        /// Forces the display to be on by resetting the display idle timer.
        /// </summary>
        ES_DISPLAY_REQUIRED = 0x00000002,

        /// <summary>
        /// 不支持此值。 如果 ES_USER_PRESENT 与其他 esFlags 值组合使用，则调用将失败，并且不会设置任何指定的状态。
        /// </summary>
        ES_USER_PRESENT = 0x00000004,

        /// <summary>
        /// 启用离开模式。 必须使用 ES_CONTINUOUS 指定此值。
        /// 离开模式只能由媒体录制和媒体分发应用程序使用，这些应用程序必须在计算机似乎处于睡眠状态时在台式计算机上执行关键后台处理。 请参阅“备注”。
        /// </summary>
        ES_AWAYMODE_REQUIRED = 0x00000040,

        /// <summary>
        /// 通知系统正在设置的状态应保持有效，直到使用 ES_CONTINUOUS 的下一次调用和清除其他状态标志之一。
        /// </summary>
        ES_CONTINUOUS = 0x80000000
    }
}
