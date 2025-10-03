using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PowerToolbox.Services.Download
{
    /// <summary>
    /// 后台智能传输服务
    /// </summary>
    public static class BitsService
    {
        private static readonly string displayName = "PowerToolbox";
        private static readonly object bitsLock = new();
        private static readonly Guid CLSID_BackgroundCopyManager = new("4991D34B-80A1-4291-83B6-3328366B9097");

        private static IBackgroundCopyManager backgroundCopyManager;

        private static Dictionary<string, (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback)> BitsDict { get; } = [];

        public static event Action<DownloadProgress> DownloadProgress;

        /// <summary>
        /// 初始化后台智能传输服务
        /// </summary>
        public static void Initialize()
        {
            if (backgroundCopyManager is null)
            {
                Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        backgroundCopyManager = (IBackgroundCopyManager)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_BackgroundCopyManager));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(BitsService), nameof(Initialize), 1, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            Task.Factory.StartNew((param) =>
            {
                lock (bitsLock)
                {
                    foreach (KeyValuePair<string, (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback)> bits in BitsDict)
                    {
                        bits.Value.backgroundCopyJob.Cancel();
                    }
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static void CreateDownload(string url, string saveFilePath)
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    if (backgroundCopyManager is not null)
                    {
                        backgroundCopyManager.CreateJob(displayName, BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out Guid downloadID, out IBackgroundCopyJob downloadJob);
                        downloadJob.AddFile(url, saveFilePath);
                        downloadJob.SetNotifyFlags(BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_FILE_RANGES_TRANSFERRED | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_ERROR | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_MODIFICATION);
                        BackgroundCopyCallback backgroundCopyCallback = new()
                        {
                            DownloadID = Convert.ToString(downloadID)
                        };
                        backgroundCopyCallback.StatusChanged += OnStatusChanged;
                        downloadJob.SetNotifyInterface(new UnknownWrapper(backgroundCopyCallback).WrappedObject);
                        downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                        lock (bitsLock)
                        {
                            if (!BitsDict.ContainsKey(backgroundCopyCallback.DownloadID))
                            {
                                BitsDict.Add(backgroundCopyCallback.DownloadID, ValueTuple.Create(saveFilePath, downloadJob, backgroundCopyCallback));
                            }
                        }

                        DownloadProgress?.Invoke(new DownloadProgress()
                        {
                            DownloadID = backgroundCopyCallback.DownloadID,
                            DownloadProgressState = DownloadProgressState.Queued,
                            FileName = Path.GetFileName(saveFilePath),
                            FilePath = saveFilePath,
                            DownloadSpeed = 0,
                            CompletedSize = 0,
                            TotalSize = 0,
                        });

                        downloadJob.Resume();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(BitsService), nameof(CreateDownload), 1, e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public static void ContinueDownload(string downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                lock (bitsLock)
                {
                    if (BitsDict.TryGetValue(downloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                    {
                        int continueResult = downloadValue.backgroundCopyJob.Resume();

                        if (continueResult is 0)
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = downloadID,
                                DownloadProgressState = DownloadProgressState.Queued,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = 0,
                                TotalSize = 0,
                            });
                        }
                    }
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public static void PauseDownload(string downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                lock (bitsLock)
                {
                    if (BitsDict.TryGetValue(downloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                    {
                        int pauseResult = downloadValue.backgroundCopyJob.Suspend();

                        if (pauseResult is 0)
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = downloadID,
                                DownloadProgressState = DownloadProgressState.Paused,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = 0,
                                TotalSize = 0,
                            });
                        }
                    }
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static void DeleteDownload(string downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                lock (bitsLock)
                {
                    if (BitsDict.TryGetValue(downloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                    {
                        int deleteResult = downloadValue.backgroundCopyJob.Cancel();

                        if (deleteResult is 0)
                        {
                            try
                            {
                                downloadValue.backgroundCopyCallback.StatusChanged -= OnStatusChanged;
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(BitsService), nameof(DeleteDownload), 1, e);
                            }

                            BitsDict.Remove(downloadID);
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = downloadID,
                                DownloadProgressState = DownloadProgressState.Deleted,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = 0,
                                TotalSize = 0,
                            });
                        }
                    }
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>
        /// 下载状态发生变化触发的事件
        /// </summary>
        private static void OnStatusChanged(BackgroundCopyCallback callback, IBackgroundCopyJob downloadJob, BG_JOB_STATE state)
        {
            // 下载文件中
            if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                if (BitsDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                {
                    DownloadProgress?.Invoke(new DownloadProgress()
                    {
                        DownloadID = callback.DownloadID,
                        DownloadProgressState = DownloadProgressState.Downloading,
                        FileName = Path.GetFileName(downloadValue.saveFilePath),
                        FilePath = downloadValue.saveFilePath,
                        DownloadSpeed = 0,
                        CompletedSize = progress.BytesTransferred,
                        TotalSize = progress.BytesTotal,
                    });
                }
            }
            // 下载完成
            else if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    downloadJob.Complete();

                    lock (bitsLock)
                    {
                        if (BitsDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Finished,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = progress.BytesTransferred,
                                TotalSize = progress.BytesTotal,
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(BitsService), nameof(OnStatusChanged), 1, e);
                }
            }
            // 下载错误
            else if (state is BG_JOB_STATE.BG_JOB_STATE_ERROR || state is BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    lock (bitsLock)
                    {
                        if (BitsDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Failed,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = progress.BytesTransferred,
                                TotalSize = progress.BytesTotal,
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(BitsService), nameof(OnStatusChanged), 2, e);
                }
            }
        }
    }
}
