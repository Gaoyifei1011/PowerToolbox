using System;
using WUApiLib;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 更新历史记录信息
    /// </summary>
    internal class UpdateHistoryInformation
    {
        /// <summary>
        /// 更新历史记录条目
        /// </summary>
        internal IUpdateHistoryEntry2 UpdateHistoryEntry { get; set; }

        /// <summary>
        /// 更新的客户端应用程序的标识符
        /// </summary>
        internal string ClientApplicationID { get; set; }

        /// <summary>
        /// 更新的日期和时间
        /// </summary>
        internal DateTimeOffset Date { get; set; }

        /// <summary>
        /// 更新返回的 HRESULT 值
        /// </summary>
        internal int HResult { get; set; }

        /// <summary>
        /// 更新操作的结果
        /// </summary>
        internal OperationResultCode OperationResultCode { get; set; }

        /// <summary>
        /// 更新支持信息的超链接
        /// </summary>
        internal string SupportUrl { get; set; }

        /// <summary>
        /// 更新标题
        /// </summary>
        internal string Title { get; set; }

        /// <summary>
        /// 更新的标识符
        /// </summary>
        internal string UpdateID { get; set; }
    }
}
