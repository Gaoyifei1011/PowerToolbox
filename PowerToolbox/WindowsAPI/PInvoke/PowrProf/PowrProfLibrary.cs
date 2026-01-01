using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace PowerToolbox.WindowsAPI.PInvoke.PowrProf
{
    /// <summary>
    /// PowrProf.dll 函数库
    /// </summary>
    public static class PowrProfLibrary
    {
        private const string PowrProf = "powrProf.dll";

        /// <summary>
        /// 通过关闭电源来暂停系统。 根据 休眠 参数，系统将进入暂停 (睡眠) 状态或休眠 (S4) 。
        /// </summary>
        /// <param name="bHibernate">如果此参数为 TRUE，则系统进入休眠状态。 如果参数为 FALSE，则系统挂起。</param>
        /// <param name="bForce">此参数不起作用。</param>
        /// <param name="bWakeupEventsDisabled">如果此参数为 TRUE，则系统将禁用所有唤醒事件。 如果参数为 FALSE，则任何系统唤醒事件仍保持启用状态。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(PowrProf, CharSet = CharSet.Unicode, EntryPoint = "SetSuspendState", PreserveSig = true, SetLastError = false)]
        public static extern bool SetSuspendState(bool bHibernate, bool bForce, bool bWakeupEventsDisabled);
    }
}
