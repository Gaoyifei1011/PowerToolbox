using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.PowrProf
{
    /// <summary>
    /// 包含有关系统电源功能的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_POWER_CAPABILITIES
    {
        /// <summary>
        /// 如果此成员为 TRUE，则存在系统电源按钮。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool PowerButtonPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则存在系统睡眠按钮。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool SleepButtonPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则有一个盖子开关。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool LidPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则操作系统支持 睡眠状态 S1。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS1;

        /// <summary>
        /// 如果此成员为 TRUE，则操作系统支持 睡眠状态 S2。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS2;

        /// <summary>
        /// 如果此成员为 TRUE，则操作系统支持 睡眠状态 S3。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS3;

        /// <summary>
        /// 如果此成员为 TRUE，则操作系统支持 睡眠状态 S4 (休眠) 。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS4;

        /// <summary>
        /// 如果此成员为 TRUE，则操作系统支持 关机状态 S5 (软关闭) 。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemS5;

        /// <summary>
        /// 如果此成员为 TRUE，则存在系统休眠文件。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool HiberFilePresent;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持唤醒功能。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool FullWake;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持视频显示调暗功能。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool VideoDimPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持 APM BIOS 电源管理功能。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool ApmPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则 UPS) (不间断电源。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool UpsPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持热区域。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool ThermalControl;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持处理器限制。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool ProcessorThrottle;

        /// <summary>
        /// 支持的最低系统处理器限制级别，以百分比表示。
        /// </summary>
        public byte ProcessorMinThrottle;

        /// <summary>
        /// 支持的最大系统处理器限制级别，以百分比表示。
        /// </summary>
        public byte ProcessorMaxThrottle;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持 混合睡眠状态。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool FastSystemS4;

        [MarshalAs(UnmanagedType.U1)]
        public bool Hiberboot;

        /// <summary>
        /// 如果此成员为 TRUE，则平台支持 ACPI 唤醒警报设备。 有关唤醒警报设备的更多详细信息，请参阅 ACPI 规范第 9.18 部分。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool WakeAlarmPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持 S0 低功耗空闲模型。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool AoAc;

        /// <summary>
        /// 如果此成员为 TRUE，则系统支持允许移除固定磁盘设备的电源。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool DiskSpinDown;

        public byte HiberFileType;

        [MarshalAs(UnmanagedType.U1)]
        public bool AoAcConnectivitySupported;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] spare3;

        /// <summary>
        /// 如果此成员为 TRUE，则系统中有一个或多个电池。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemBatteriesPresent;

        /// <summary>
        /// 如果此成员为 TRUE，则系统电池是短期的。 短期电池用于不间断电源 (UPS) 。
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool BatteriesAreShortTerm;

        /// <summary>
        /// 一个BATTERY_REPORTING_SCALE结构，其中包含有关如何报告系统电池指标的信息。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public BATTERY_REPORTING_SCALE[] BatteryScale;

        /// <summary>
        /// 最低 系统睡眠状态 (Sx) ，当系统使用交流电源时，它将生成唤醒事件。 此成员必须是 SYSTEM_POWER_STATE 枚举类型值之一。
        /// </summary>
        public SYSTEM_POWER_STATE AcOnLineWake;

        /// <summary>
        /// 最低 系统睡眠状态 (Sx) ，它将通过盖子开关生成唤醒事件。 此成员必须是 SYSTEM_POWER_STATE 枚举类型值之一。
        /// </summary>
        public SYSTEM_POWER_STATE SoftLidWake;

        /// <summary>
        /// 最低 系统睡眠状态 (Sx) 硬件支持，这些硬件将通过实时时钟 (RTC) 生成唤醒事件。 此成员必须是 SYSTEM_POWER_STATE 枚举类型值之一。
        /// 若要使用 RTC 唤醒计算机，操作系统还必须支持在 RTC 生成唤醒事件时从计算机处于的睡眠状态唤醒。 因此，RTC 唤醒事件可以唤醒计算机的有效最低睡眠状态是操作系统支持的最低睡眠状态，该状态等于或高于 RtcWake 的值。 若要确定操作系统支持的睡眠状态，检查 SystemS1、SystemS2、SystemS3 和 SystemS4 成员。
        /// </summary>
        public SYSTEM_POWER_STATE RtcWake;

        /// <summary>
        /// 支持唤醒事件的最小允许 系统电源状态 。 此成员必须是 SYSTEM_POWER_STATE 枚举类型值之一。 请注意，当系统上安装不同的设备驱动程序时，此状态可能会更改。
        /// </summary>
        public SYSTEM_POWER_STATE MinDeviceWakeState;

        /// <summary>
        /// 如果应用程序使用 LT_LOWEST_LATENCY 调用 RequestWakeupLatency，则使用的默认系统电源状态。 此成员必须是 SYSTEM_POWER_STATE 枚举类型值之一。
        /// </summary>
        public SYSTEM_POWER_STATE DefaultLowLatencyWake;
    }
}
