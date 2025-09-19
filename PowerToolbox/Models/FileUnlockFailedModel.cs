using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 解锁失败信息模型
    /// </summary>
    public class FileUnlockFailedModel
    {
        /// <summary>
        /// 解锁失败的文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 解锁失败的文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 解锁失败的进程名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 解锁失败的进程 ID
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// 解锁失败的进程路径
        /// </summary>
        public string ProcessPath { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }
    }
}
