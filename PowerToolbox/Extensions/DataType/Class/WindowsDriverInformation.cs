using System;
using WUApiLib;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// Windows 驱动条目信息
    /// </summary>
    internal sealed class WindowsDriverInformation
    {
        /// <summary>
        /// 驱动条目
        /// </summary>
        internal IWindowsDriverUpdate5 WindowsDriverUpdate { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的匹配设备的问题号
        /// </summary>
        internal long DeviceProblemNumber { get; set; }

        /// <summary>
        /// 驱动程序更新的类
        /// </summary>
        internal string DriverClass { get; set; }

        /// <summary>
        /// Windows 驱动程序更新必须匹配才能安装的硬件 ID 或兼容 ID
        /// </summary>
        internal string DriverHardwareID { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的制造商的语言固定名称
        /// </summary>
        internal string DriverManufacturer { get; set; }

        /// <summary>
        /// Windows 驱动程序更新所针对的设备的语言固定模型名称
        /// </summary>
        internal string DriverModel { get; set; }

        /// <summary>
        /// Windows 驱动程序更新提供程序的语言固定名称
        /// </summary>
        internal string DriverProvider { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的驱动程序版本日期
        /// </summary>
        internal DateTimeOffset DriverVerDate { get; set; }
    }
}
