using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 失败计划任务数据模型
    /// </summary>
    public class ScheduledTaskFailedModel
    {
        /// <summary>
        /// 计划任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 计划任务路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }
    }
}
