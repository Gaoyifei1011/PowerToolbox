using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Ole32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolbox.Services.Download
{
    /// <summary>
    /// 下载服务
    /// </summary>
    public static class DeliveryOptimizationService
    {
        private static readonly string displayName = "PowerToolbox";
        private static readonly object deliveryOptimizationLock = new();
        private static Guid CLSID_DeliveryOptimization = new("5B99FA76-721C-423C-ADAC-56D03C8A8007");

        private static Dictionary<string, (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback)> DeliveryOptimizationDict { get; } = [];

        public static event Action<DownloadProgress> DownloadProgress;

        /// <summary>
        /// 应用关闭时终止所有下载任务
        /// </summary>
        public static void TerminateDownload()
        {
            Task.Factory.StartNew((param) =>
            {
                lock (deliveryOptimizationLock)
                {
                    foreach (KeyValuePair<string, (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback)> deliveryOptimization in DeliveryOptimizationDict)
                    {
                        deliveryOptimization.Value.doDownload.Abort();
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
                    IDOManager doManager = null;

                    // 创建 IDoManager
                    doManager = (IDOManager)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_DeliveryOptimization));
                    doManager.CreateDownload(out IDODownload doDownload);
                    IntPtr pInterface = Marshal.GetComInterfaceForObject<object, IDODownload>(doDownload);
                    Ole32Library.CoSetProxyBlanket(pInterface, uint.MaxValue, uint.MaxValue, unchecked((IntPtr)ulong.MaxValue), 0, 3, IntPtr.Zero, 32);
                    Marshal.Release(pInterface);
                    Marshal.FinalReleaseComObject(doManager);

                    // 添加下载信息
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_DisplayName, displayName);
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_Uri, url);
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_LocalPath, saveFilePath);
                    doDownload.GetProperty(DODownloadProperty.DODownloadProperty_Id, out object id);
                    string downloadID = Convert.ToString(id);
                    DODownloadStatusCallback doDownloadStatusCallback = new()
                    {
                        DownloadID = downloadID
                    };
                    doDownloadStatusCallback.StatusChanged += OnStatusChanged;
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_CallbackInterface, new UnknownWrapper(doDownloadStatusCallback));
                    doDownload.SetProperty(DODownloadProperty.DODownloadProperty_ForegroundPriority, true);

                    lock (deliveryOptimizationLock)
                    {
                        if (!DeliveryOptimizationDict.ContainsKey(downloadID))
                        {
                            DeliveryOptimizationDict.Add(downloadID, ValueTuple.Create(saveFilePath, doDownload, doDownloadStatusCallback));
                        }
                    }

                    DownloadProgress?.Invoke(new DownloadProgress()
                    {
                        DownloadID = doDownloadStatusCallback.DownloadID,
                        DownloadProgressState = DownloadProgressState.Queued,
                        FileName = Path.GetFileName(saveFilePath),
                        FilePath = saveFilePath,
                        DownloadSpeed = 0,
                        CompletedSize = 0,
                        TotalSize = 0,
                    });

                    doDownload.Start(IntPtr.Zero);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DeliveryOptimizationService), nameof(CreateDownload), 1, e);
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
                lock (deliveryOptimizationLock)
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                    {
                        int continueResult = downloadValue.doDownload.Start(IntPtr.Zero);

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
                lock (deliveryOptimizationLock)
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                    {
                        int pauseResult = downloadValue.doDownload.Pause();

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
                lock (deliveryOptimizationLock)
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                    {
                        int deleteResult = downloadValue.doDownload.Abort();

                        if (deleteResult is 0)
                        {
                            downloadValue.doDownloadStatusCallback.StatusChanged -= OnStatusChanged;
                            DeliveryOptimizationDict.Remove(downloadID);
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
        private static void OnStatusChanged(DODownloadStatusCallback callback, IDODownload doDownload, DO_DOWNLOAD_STATUS status)
        {
            // 下载文件中
            if (status.State is DODownloadState.DODownloadState_Transferring)
            {
                if (DeliveryOptimizationDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                {
                    DownloadProgress?.Invoke(new DownloadProgress()
                    {
                        DownloadID = callback.DownloadID,
                        DownloadProgressState = DownloadProgressState.Downloading,
                        FileName = Path.GetFileName(downloadValue.saveFilePath),
                        FilePath = downloadValue.saveFilePath,
                        DownloadSpeed = 0,
                        CompletedSize = status.BytesTransferred,
                        TotalSize = status.BytesTotal,
                    });
                }
            }
            // 下载完成
            else if (status.State is DODownloadState.DODownloadState_Transferred)
            {
                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    doDownload.Finalize();

                    lock (deliveryOptimizationLock)
                    {
                        if (DeliveryOptimizationDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Finished,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = status.BytesTransferred,
                                TotalSize = status.BytesTotal,
                            });

                            DeliveryOptimizationDict.Remove(callback.DownloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DeliveryOptimizationService), nameof(OnStatusChanged), 1, e);
                }
            }

            // 下载错误
            if (status.Error is not 0 || status.ExtendedError is not 0)
            {
                try
                {
                    callback.StatusChanged -= OnStatusChanged;

                    lock (deliveryOptimizationLock)
                    {
                        if (DeliveryOptimizationDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Failed,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = status.BytesTransferred,
                                TotalSize = status.BytesTotal,
                            });

                            DeliveryOptimizationDict.Remove(callback.DownloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DeliveryOptimizationService), nameof(OnStatusChanged), 2, e);
                }
            }
        }
    }
}
