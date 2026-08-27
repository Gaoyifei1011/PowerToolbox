using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 包含有关系统电源状态的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct SYSTEM_POWER_STATUS
    {
        /// <summary>
        /// 交流电源状态。
        /// </summary>
        internal byte ACLineStatus;

        /// <summary>
        /// 电池充电状态。如果电池未充电，并且电池容量介于低到高之间，则值为零。
        /// </summary>
        internal byte BatteryFlag;

        /// <summary>
        /// 剩余电池电量的百分比。 此成员可以是 0 到 100 范围内的值;如果状态未知，则为 255。
        /// </summary>
        internal byte BatteryLifePercent;

        /// <summary>
        /// 节电模式的状态。 若要参与节能，请在节电模式打开时避免资源密集型任务。 若要在此值更改时收到通知，请使用电源设置 GUID 调用 RegisterPowerSettingNotification 函数，GUID_POWER_SAVING_STATUS。
        /// </summary>
        internal byte SystemStatusFlag;

        /// <summary>
        /// 剩余电池使用时间的秒数;如果剩余秒未知或设备连接到交流电源，则为 –1。
        /// </summary>
        internal int BatteryLifeTime;

        /// <summary>
        /// 充满电时的电池使用时间的秒数;如果电池完整使用时间未知或设备连接到交流电源，则为 –1。
        /// </summary>
        internal int BatteryFullLifeTime;
    }
}
