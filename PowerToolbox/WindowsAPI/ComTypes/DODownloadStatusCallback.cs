using System;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDODownloadStatusCallback 接口的实现
    /// </summary>
    internal class DODownloadStatusCallback : IDODownloadStatusCallback
    {
        internal string DownloadID { get; set; }

        /// <summary>
        /// 下载状态发生变化时触发的事件
        /// </summary>
        internal event Action<DODownloadStatusCallback, IDODownload, DO_DOWNLOAD_STATUS> StatusChanged;

        public int OnStatusChange(IDODownload download, DO_DOWNLOAD_STATUS status)
        {
            StatusChanged?.Invoke(this, download, status);
            return 0;
        }
    }
}
