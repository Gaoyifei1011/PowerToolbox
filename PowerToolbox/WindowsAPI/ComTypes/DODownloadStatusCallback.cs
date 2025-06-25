﻿using System;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDODownloadStatusCallback 接口的实现
    /// </summary>
    public class DODownloadStatusCallback : IDODownloadStatusCallback
    {
        public string DownloadID { get; set; }

        /// <summary>
        /// 下载状态发生变化时触发的事件
        /// </summary>
        public event Action<DODownloadStatusCallback, IDODownload, DO_DOWNLOAD_STATUS> StatusChanged;

        public int OnStatusChange(IDODownload download, DO_DOWNLOAD_STATUS status)
        {
            StatusChanged?.Invoke(this, download, status);
            return 0;
        }
    }
}
