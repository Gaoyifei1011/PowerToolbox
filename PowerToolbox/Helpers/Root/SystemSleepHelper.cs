using PowerToolbox.WindowsAPI.PInvoke.Kernel32;

namespace PowerToolbox.Helpers.Root
{
    /// <summary>
    /// 包含控制屏幕关闭以及系统休眠相关的方法。
    /// </summary>
    internal static class SystemSleepHelper
    {
        /// <summary>
        /// 设置此线程此时开始一直将处于运行状态，此时计算机不应该进入睡眠状态。
        /// 此线程退出后，设置将失效。
        /// 如果需要恢复，请调用 RestoreForCurrentThread 方法。
        /// </summary>
        /// <param name="keepDisplayOn">
        /// 表示是否应该同时保持屏幕不关闭。
        /// 对于游戏、视频和演示相关的任务需要保持屏幕不关闭；而对于后台服务、下载和监控等任务则不需要。
        /// </param>
        internal static void PreventForCurrentThread(bool keepDisplayOn = true)
        {
            if (keepDisplayOn)
            {
                Kernel32Library.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
            }
            else
            {
                Kernel32Library.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            }
        }

        /// <summary>
        /// 恢复此线程的运行状态，操作系统现在可以正常进入睡眠状态和关闭屏幕。
        /// </summary>
        internal static void RestoreForCurrentThread()
        {
            Kernel32Library.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        /// <summary>
        /// 重置系统睡眠或者关闭屏幕的计时器，这样系统睡眠或者屏幕能够继续持续工作设定的超时时间。
        /// </summary>
        /// <param name="keepDisplayOn">
        /// 表示是否应该同时保持屏幕不关闭。
        /// 对于游戏、视频和演示相关的任务需要保持屏幕不关闭；而对于后台服务、下载和监控等任务则不需要。
        /// </param>
        internal static void ResetIdle(bool keepDisplayOn = true)
        {
            if (keepDisplayOn)
            {
                Kernel32Library.SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
            }
            else
            {
                Kernel32Library.SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            }
        }
    }
}
