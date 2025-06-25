﻿using System;

namespace PowerToolbox.WindowsAPI.ComTypes
{
    /// <summary>
    /// IBackgroundCopyCallback 接口的实现
    /// </summary>
    public class BackgroundCopyCallback : IBackgroundCopyCallback
    {
        public string DownloadID { get; set; }

        /// <summary>
        /// 下载状态发生变化时触发的事件
        /// </summary>
        public event Action<BackgroundCopyCallback, IBackgroundCopyJob, BG_JOB_STATE> StatusChanged;

        public int JobTransferred(IBackgroundCopyJob pJob)
        {
            pJob.GetState(out BG_JOB_STATE state);
            StatusChanged?.Invoke(this, pJob, state);
            return 0;
        }

        public int JobError(IBackgroundCopyJob pJob, IBackgroundCopyError pError)
        {
            pJob.GetState(out BG_JOB_STATE state);
            StatusChanged?.Invoke(this, pJob, state);
            return 0;
        }

        public int JobModification(IBackgroundCopyJob pJob, uint dwReserved)
        {
            pJob.GetState(out BG_JOB_STATE state);
            StatusChanged?.Invoke(this, pJob, state);
            return 0;
        }
    }
}
