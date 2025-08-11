using System;
using WUApiLib;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// Windows 驱动条目信息
    /// </summary>
    public class WindowsDriverInformation
    {
        /// <summary>
        /// 驱动条目
        /// </summary>
        public IWindowsDriverUpdate5 WindowsDriverUpdate { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的匹配设备的问题号
        /// </summary>
        public long DeviceProblemNumber { get; set; }

        /// <summary>
        /// 驱动程序更新的类
        /// </summary>
        public string DriverClass { get; set; }

        /// <summary>
        /// Windows 驱动程序更新必须匹配才能安装的硬件 ID 或兼容 ID
        /// </summary>
        public string DriverHardwareID { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的制造商的语言固定名称
        /// </summary>
        public string DriverManufacturer { get; set; }

        /// <summary>
        /// Windows 驱动程序更新所针对的设备的语言固定模型名称
        /// </summary>
        public string DriverModel { get; set; }

        /// <summary>
        /// Windows 驱动程序更新提供程序的语言固定名称
        /// </summary>
        public string DriverProvider { get; set; }

        /// <summary>
        /// Windows 驱动程序更新的驱动程序版本日期
        /// </summary>
        public DateTimeOffset DriverVerDate { get; set; }
    }
}
