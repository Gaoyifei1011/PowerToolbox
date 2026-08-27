using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 操作失败信息数据模型
    /// </summary>
    internal sealed class OperationFailedModel
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        internal string FilePath { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        internal Exception Exception { get; set; }
    }
}
