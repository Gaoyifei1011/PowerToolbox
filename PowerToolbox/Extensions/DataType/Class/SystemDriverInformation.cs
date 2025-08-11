using System;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 系统驱动条目信息
    /// </summary>
    public class SystemDriverInformation
    {
        /// <summary>
        /// 驱动 ID 号
        /// </summary>
        public Guid DeviceGuid { get; set; }

        /// <summary>
        /// 驱动描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 驱动文件路径
        /// </summary>
        public string InfPath { get; set; }

        /// <summary>
        /// 驱动日期
        /// </summary>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary>
        public Version Version { get; set; }
    }
}
