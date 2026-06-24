using System.Runtime.InteropServices;

namespace PowerToolbox.WindowsAPI.PInvoke.PowrProf
{
    /// <summary>
    /// 包含 IOCTL_BATTERY_QUERY_STATUS报告的电池容量的粒度。 当 InformationLevel 设置为 BatteryGranularityInformation 时，将从IOCTL_BATTERY_QUERY_INFORMATION返回BATTERY_REPORTING_SCALE结构的可变长度数组。 当粒度取决于电池的当前容量时，将返回多个条目。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BATTERY_REPORTING_SCALE
    {
        /// <summary>
        /// IOCTL_BATTERY_QUERY_STATUS返回的容量读数的粒度（以毫瓦时 (mWh) 为单位）。 随着电池放电和充电降低读数范围，粒度可能会随时间而变化。
        /// </summary>
        public uint Granularity;

        /// <summary>
        /// 粒度的容量上限。 Granularity 的值对于IOCTL_BATTERY_QUERY_STATUS报告的容量有效，这些容量小于或等于此容量 (mWh) ，但大于或等于上一个数组元素中给定的容量;如果这是第一个数组元素，则为零。
        /// </summary>
        public uint Capacity;
    }
}
