using System;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 系统驱动条目信息
    /// </summary>
    internal sealed class SystemDriverInformation
    {
        /// <summary>
        /// 驱动 ID 号
        /// </summary>
        internal Guid DeviceGuid { get; set; }

        /// <summary>
        /// 驱动描述信息
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// 驱动文件路径
        /// </summary>
        internal string InfPath { get; set; }

        /// <summary>
        /// 驱动日期
        /// </summary>
        internal DateTimeOffset Date { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary>
        internal Version Version { get; set; }
    }
}
