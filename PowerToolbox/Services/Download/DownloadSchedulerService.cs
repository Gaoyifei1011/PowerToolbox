using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PowerToolbox.Services.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public static class DownloadSchedulerService
    {
        private static bool isInitialized;
        private static string doEngineMode;

        public static SemaphoreSlim DownloadSchedulerSemaphoreSlim { get; private set; } = new SemaphoreSlim(1, 1);

        public static List<DownloadSchedulerModel> DownloadSchedulerList { get; } = [];

        public static event Action<DownloadSchedulerModel> DownloadProgress;

        /// <summary>
        /// 下载状态发生改变时触发的事件
        /// </summary>
        private static void OnDownloadProgress(DownloadProgress downloadProgress)
        {
            // 处于等待中（新添加下载任务或者已经恢复下载）
            if (downloadProgress.DownloadProgressState is DownloadProgressState.Queued)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    // 下载任务已经存在，更新下载状态
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (string.Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            return;
                        }
                    }

                    // 不存在则添加任务
                    DownloadSchedulerModel downloadScheduler = new()
                    {
                        DownloadID = downloadProgress.DownloadID,
                        DownloadProgressState = downloadProgress.DownloadProgressState,
                        FileName = downloadProgress.FileName,
                        FilePath = downloadProgress.FilePath,
                        CompletedSize = downloadProgress.CompletedSize,
                        TotalSize = downloadProgress.TotalSize,
                        DownloadSpeed = downloadProgress.DownloadSpeed,
                    };

                    DownloadSchedulerList.Add(downloadScheduler);
                    DownloadProgress?.Invoke(new DownloadSchedulerModel()
                    {
                        DownloadID = downloadScheduler.DownloadID,
                        FileName = downloadScheduler.FileName,
                        FilePath = downloadScheduler.FilePath,
                        DownloadProgressState = downloadScheduler.DownloadProgressState,
                        CompletedSize = downloadScheduler.CompletedSize,
                        TotalSize = downloadScheduler.TotalSize,
                        DownloadSpeed = downloadScheduler.DownloadSpeed,
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadSchedulerService), nameof(OnDownloadProgress), 1, e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务正在下载中
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Downloading)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (string.Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            downloadSchedulerItem.DownloadSpeed = downloadProgress.CompletedSize - downloadSchedulerItem.CompletedSize;
                            downloadSchedulerItem.CompletedSize = downloadProgress.CompletedSize;
                            downloadSchedulerItem.TotalSize = downloadProgress.TotalSize;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadSchedulerService), nameof(OnDownloadProgress), 2, e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已暂停
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Paused)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (string.Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadSchedulerService), nameof(OnDownloadProgress), 3, e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已失败
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Failed)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (string.Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            downloadSchedulerItem.CompletedSize = 1;
                            downloadProgress.TotalSize = 1;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            DownloadSchedulerList.Remove(downloadSchedulerItem);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadSchedulerService), nameof(OnDownloadProgress), 4, e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已完成
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Finished)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (string.Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            downloadSchedulerItem.DownloadSpeed = downloadProgress.CompletedSize - downloadSchedulerItem.CompletedSize;
                            downloadSchedulerItem.CompletedSize = downloadProgress.CompletedSize;
                            downloadSchedulerItem.TotalSize = downloadProgress.TotalSize;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            DownloadSchedulerList.Remove(downloadSchedulerItem);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadSchedulerService), nameof(OnDownloadProgress), 5, e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已删除
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Deleted)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (string.Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            DownloadSchedulerList.Remove(downloadSchedulerItem);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadSchedulerService), nameof(OnDownloadProgress), 6, e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
        }

        /// <summary>
        /// 初始化后台下载调度器
        /// 先检查当前网络状态信息，加载暂停任务信息，然后初始化下载监控任务
        /// </summary>
        public static void InitializeDownloadScheduler()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                // 获取当前下载引擎
                doEngineMode = DownloadOptionsService.DoEngineMode;

                // 初始化下载服务
                if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
                {
                    DeliveryOptimizationService.DownloadProgress += OnDownloadProgress;
                }
                else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
                {
                    BitsService.Initialize();
                    BitsService.DownloadProgress += OnDownloadProgress;
                }
                else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
                {
                    Aria2Service.InitializeAria2Conf();
                    Aria2Service.Initialize();
                    Aria2Service.DownloadProgress += OnDownloadProgress;
                }
            }
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public static void CloseDownloadScheduler()
        {
            if (isInitialized)
            {
                isInitialized = false;

                DownloadSchedulerSemaphoreSlim?.Dispose();
                DownloadSchedulerSemaphoreSlim = null;

                // 注销下载服务
                try
                {
                    if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
                    {
                        DeliveryOptimizationService.TerminateDownload();
                        DeliveryOptimizationService.DownloadProgress -= OnDownloadProgress;
                    }
                    else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
                    {
                        BitsService.TerminateDownload();
                        BitsService.DownloadProgress -= OnDownloadProgress;
                    }
                    else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
                    {
                        Aria2Service.Release();
                        Aria2Service.DownloadProgress -= OnDownloadProgress;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadSchedulerService), nameof(CloseDownloadScheduler), 1, e);
                }
            }
        }

        /// <summary>
        /// 创建下载任务
        /// </summary>
        public static void CreateDownload(string fileLink, string filePath)
        {
            if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.CreateDownload(fileLink, filePath);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.CreateDownload(fileLink, filePath);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.CreateDownload(fileLink, filePath);
            }
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public static void ContinueDownload(string downloadID)
        {
            if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.ContinueDownload(downloadID);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.ContinueDownload(downloadID);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.ContinueDownload(downloadID);
            }
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public static void PauseDownload(string downloadID)
        {
            if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.PauseDownload(downloadID);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.PauseDownload(downloadID);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.PauseDownload(downloadID);
            }
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public static void DeleteDownload(string downloadID)
        {
            if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.DeleteDownload(downloadID);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.DeleteDownload(downloadID);
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.DeleteDownload(downloadID);
            }
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.TerminateDownload();
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.TerminateDownload();
            }
            else if (string.Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.Release();
            }
        }
    }
}
