﻿using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.ComTypes
{
    /// <summary>
    /// IBackgroundCopyCallback 接口的实现
    /// </summary>
    public class BackgroundCopyCallback : IBackgroundCopyCallback
    {
        public Guid DownloadID { get; set; } = Guid.Empty;

        /// <summary>
        /// 下载状态发生变化时触发的事件
        /// </summary>
        public event Action<BackgroundCopyCallback, IBackgroundCopyJob, BG_JOB_STATE> StatusChanged;

        public void JobTransferred([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob)
        {
            pJob.GetState(out BG_JOB_STATE state);
            StatusChanged?.Invoke(this, pJob, state);
        }

        public void JobError([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, [MarshalAs(UnmanagedType.Interface)] IBackgroundCopyError pError)
        {
            pJob.GetState(out BG_JOB_STATE state);
            StatusChanged?.Invoke(this, pJob, state);
        }

        public void JobModification([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, uint dwReserved)
        {
            pJob.GetState(out BG_JOB_STATE state);
            StatusChanged?.Invoke(this, pJob, state);
        }
    }
}
