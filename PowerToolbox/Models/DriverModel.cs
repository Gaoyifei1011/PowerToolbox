using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 驱动数据模型
    /// </summary>
    internal class DriverModel
    {
        /// <summary>
        /// 驱动名称
        /// </summary>
        internal string DeviceName { get; set; }

        /// <summary>
        /// 驱动 INF 名称
        /// </summary>
        internal string DriverInfName { get; set; }

        /// <summary>
        /// 驱动 OEM INF 名称
        /// </summary>
        internal string DriverOEMInfName { get; set; }

        /// <summary>
        /// 驱动类别
        /// </summary>
        internal string DriverType { get; set; }

        /// <summary>
        /// 驱动制造商
        /// </summary>
        internal string DriverManufacturer { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary>
        internal Version DriverVersion { get; set; }

        /// <summary>
        /// 驱动日期
        /// </summary>
        internal DateTimeOffset DriverDate { get; set; }

        /// <summary>
        /// 驱动大小
        /// </summary>
        internal string DriverSize { get; set; }

        /// <summary>
        /// 驱动路径
        /// </summary>
        internal string DriverLocation { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        internal string SignatureName { get; set; }
    }
}
