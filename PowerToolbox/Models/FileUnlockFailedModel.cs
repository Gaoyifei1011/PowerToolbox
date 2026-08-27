using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 解锁失败信息模型
    /// </summary>
    internal class FileUnlockFailedModel
    {
        /// <summary>
        /// 解锁失败的文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 解锁失败的文件路径
        /// </summary>
        internal string FilePath { get; set; }

        /// <summary>
        /// 解锁失败的进程名称
        /// </summary>
        internal string ProcessName { get; set; }

        /// <summary>
        /// 解锁失败的进程 ID
        /// </summary>
        internal string ProcessId { get; set; }

        /// <summary>
        /// 解锁失败的进程路径
        /// </summary>
        internal string ProcessPath { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        internal Exception Exception { get; set; }
    }
}
