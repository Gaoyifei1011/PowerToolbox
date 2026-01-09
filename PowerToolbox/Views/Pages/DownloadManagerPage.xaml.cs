using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic.FileIO;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 下载管理页面
    /// </summary>
    public sealed partial class DownloadManagerPage : Page
    {
        private readonly string DownloadingCountInfoString = ResourceService.DownloadManagerResource.GetString("DownloadingCountInfo");
        private readonly string FileShareString = ResourceService.DownloadManagerResource.GetString("FileShare");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly IDataTransferManagerInterop dataTransferManagerInterop = (IDataTransferManagerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DataTransferManager));
        private bool isInitialized = false;

        private WinRTObservableCollection<DownloadModel> DownloadCollection { get; } = [];

        public DownloadManagerPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                DownloadSchedulerService.DownloadProgress += OnDownloadProgress;
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第一部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                download.IsOperating = true;
                DownloadSchedulerService.ContinueDownload(download.DownloadID);
            }
        }

        /// <summary>
        /// 暂停下载当前任务
        /// </summary>
        private void OnPauseExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                download.IsOperating = true;
                DownloadSchedulerService.PauseDownload(download.DownloadID);
            }
        }

        /// <summary>
        /// 打开下载文件所属目录
        /// </summary>
        private void OnOpenFolderExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            if (File.Exists(filePath))
                            {
                                nint pidlList = Shell32Library.ILCreateFromPath(filePath);
                                if (pidlList is not 0)
                                {
                                    Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
                                    Shell32Library.ILFree(pidlList);
                                }
                            }
                            else
                            {
                                string directoryPath = Path.GetDirectoryName(filePath);

                                if (Directory.Exists(directoryPath))
                                {
                                    Process.Start(directoryPath);
                                }
                                else
                                {
                                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnOpenFolderExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private void OnDeleteExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                if (download.DownloadProgressState is DownloadProgressState.Queued || download.DownloadProgressState is DownloadProgressState.Downloading || download.DownloadProgressState is DownloadProgressState.Paused)
                {
                    DownloadSchedulerService.DeleteDownload(download.DownloadID);
                }
                else
                {
                    DownloadCollection.Remove(download);
                }
            }
        }

        /// <summary>
        /// 删除下载（包括文件）
        /// </summary>
        private async void OnDeleteWithFileExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                if (download.DownloadProgressState is DownloadProgressState.Queued || download.DownloadProgressState is DownloadProgressState.Downloading || download.DownloadProgressState is DownloadProgressState.Paused)
                {
                    DownloadSchedulerService.DeleteDownload(download.DownloadID);
                }
                else if (download.DownloadProgressState is DownloadProgressState.Finished)
                {
                    download.IsOperating = true;
                    (bool result, Exception exception) = await Task.Run(() =>
                    {
                        // 删除文件
                        try
                        {
                            if (File.Exists(download.FilePath))
                            {
                                FileSystem.DeleteFile(download.FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                            }

                            return ValueTuple.Create<bool, Exception>(true, null);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnDeleteWithFileExecuteRequested), 1, e);
                            return ValueTuple.Create(false, e);
                        }
                    });

                    if (result)
                    {
                        DownloadCollection.Remove(download);
                    }
                    else
                    {
                        download.IsOperating = false;
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteFileFailed));
                    }
                }
                else
                {
                    DownloadCollection.Remove(download);
                }
            }
        }

        /// <summary>
        /// 文件共享
        /// </summary>
        private async void OnShareFileExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath && File.Exists(filePath))
            {
                if (DataTransferManager.IsSupported())
                {
                    try
                    {
                        List<StorageFile> fileList = [await StorageFile.GetFileFromPathAsync(filePath)];
                        dataTransferManagerInterop.GetForWindow((nint)MainWindow.Current.AppWindow.Id.Value, new("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C"), out DataTransferManager dataTransferManager);
                        dataTransferManager.DataRequested += (sender, args) => OnDataRequested(sender, args, fileList);
                        dataTransferManagerInterop.ShowShareUIForWindow((nint)MainWindow.Current.AppWindow.Id.Value);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnShareFileExecuteRequested), 1, e);
                    }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FileLost));
            }
        }

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private void OnFileInformationExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath && File.Exists(filePath))
            {
                Task.Run(() =>
                {
                    try
                    {
                        StringCollection stringCollection = [filePath];
                        DataObject data = new();
                        data.SetData("Preferred DropEffect", true, new MemoryStream([5, 0, 0, 0]));
                        data.SetData("Shell IDList Array", true, CreateShellIDList(stringCollection));
                        data.SetFileDropList(stringCollection);
                        Shell32Library.SHMultiFileProperties(data, 0);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnFileInformationExecuteRequested), 1, e);
                    }
                });
            }
        }

        #endregion 第一部分：ExecuteCommand 命令调用时挂载的事件

        #region 第二部分：下载管理页面——挂载的事件

        /// <summary>
        /// 添加任务
        /// </summary>
        private async void OnAddTaskClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new AddDownloadTaskDialog());
        }

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        private void OnContinueAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadModel downloadItem in DownloadCollection)
            {
                if (downloadItem.DownloadProgressState is DownloadProgressState.Paused)
                {
                    downloadItem.IsOperating = true;
                    DownloadSchedulerService.ContinueDownload(downloadItem.DownloadID);
                }
            }
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        private void OnPauseDownloadClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadModel downloadItem in DownloadCollection)
            {
                if (downloadItem.DownloadProgressState is DownloadProgressState.Queued || downloadItem.DownloadProgressState is DownloadProgressState.Downloading)
                {
                    downloadItem.IsOperating = true;
                    DownloadSchedulerService.PauseDownload(downloadItem.DownloadID);
                }
            }
        }

        /// <summary>
        /// 删除全部下载
        /// </summary>
        private void OnDeleteDownloadClicked(object sender, RoutedEventArgs args)
        {
            for (int index = DownloadCollection.Count - 1; index >= 0; index--)
            {
                DownloadModel downloadItem = DownloadCollection[index];
                downloadItem.IsOperating = true;

                if (downloadItem.DownloadProgressState is DownloadProgressState.Queued || downloadItem.DownloadProgressState is DownloadProgressState.Downloading || downloadItem.DownloadProgressState is DownloadProgressState.Paused)
                {
                    DownloadSchedulerService.DeleteDownload(downloadItem.DownloadID);
                }
                else
                {
                    DownloadCollection.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Shell32Library.SHGetKnownFolderPath(new("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, 0, out string downloadFolder);
                    Process.Start(downloadFolder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnOpenFolderClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开使用说明
        /// </summary>
        private async void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            await Task.Delay(300);
            if (!DownloadSplitView.IsPaneOpen)
            {
                DownloadSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        private async void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new DownloadSettingsDialog());
        }

        /// <summary>
        /// 关闭对话框或使用说明
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            DownloadSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 了解更多
        /// </summary>
        private async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage));

            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
        }

        /// <summary>
        /// 打开网络和 Internet 设置
        /// </summary>
        private void OnNetworkInternetClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:network-status");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnNetworkInternetClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        private async void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadSplitView.IsPaneOpen = false;
            await Task.Delay(300);
        }

        #endregion 第二部分：下载管理页面——挂载的事件

        #region 第三部分：下载管理页面——自定义事件

        /// <summary>
        /// 下载状态发生改变时触发的事件
        /// </summary>
        private void OnDownloadProgress(DownloadSchedulerModel downloadScheduler)
        {
            // 处于等待中（新添加下载任务或者已经恢复下载）
            if (downloadScheduler.DownloadProgressState is DownloadProgressState.Queued)
            {
                synchronizationContext.Post((_) =>
                {
                    // 下载任务已经存在，更新下载状态
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.IsOperating = false;
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            return;
                        }
                    }

                    // 不存在则添加任务
                    DownloadModel download = new()
                    {
                        IsOperating = false,
                        DownloadID = downloadScheduler.DownloadID,
                        FileName = downloadScheduler.FileName,
                        FilePath = downloadScheduler.FilePath,
                        DownloadProgressState = downloadScheduler.DownloadProgressState,
                        CompletedSize = downloadScheduler.CompletedSize,
                        TotalSize = downloadScheduler.TotalSize,
                        DownloadSpeed = downloadScheduler.DownloadSpeed
                    };

                    DownloadCollection.Add(download);
                }, null);
            }
            // 下载任务正在下载中
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Downloading)
            {
                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            downloadItem.DownloadSpeed = downloadScheduler.DownloadSpeed;
                            downloadItem.CompletedSize = downloadScheduler.CompletedSize;
                            downloadItem.TotalSize = downloadScheduler.TotalSize;
                            return;
                        }
                    }
                }, null);
            }
            // 下载任务已暂停或已失败
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Paused || downloadScheduler.DownloadProgressState is DownloadProgressState.Failed)
            {
                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.IsOperating = false;
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            return;
                        }
                    }
                }, null);
            }
            // 下载任务已完成
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Finished)
            {
                MemoryStream memoryStream = null;
                try
                {
                    Bitmap thumbnailBitmap = ThumbnailHelper.GetThumbnailBitmap(downloadScheduler.FilePath);

                    if (thumbnailBitmap is not null)
                    {
                        memoryStream = new();
                        thumbnailBitmap.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        thumbnailBitmap.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnDownloadProgress), 1, e);
                }

                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            downloadItem.DownloadSpeed = downloadScheduler.DownloadSpeed;
                            downloadItem.CompletedSize = downloadScheduler.CompletedSize;
                            downloadItem.TotalSize = downloadScheduler.TotalSize;
                            downloadItem.DownloadProgressState = downloadScheduler.DownloadProgressState;

                            if (memoryStream is not null)
                            {
                                try
                                {
                                    BitmapImage bitmapImage = new();
                                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                    downloadItem.IconImage = bitmapImage;
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnDownloadProgress), 2, e);
                                }
                                finally
                                {
                                    memoryStream?.Dispose();
                                }
                            }

                            return;
                        }
                    }
                }, null);
            }
            // 下载任务已删除
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Deleted)
            {
                synchronizationContext.Post((_) =>
                {
                    foreach (DownloadModel downloadItem in DownloadCollection)
                    {
                        if (string.Equals(downloadItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            DownloadCollection.Remove(downloadItem);
                            return;
                        }
                    }
                }, null);
            }
        }

        /// <summary>
        /// 应用程序即将关闭时发生的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                DownloadSchedulerService.DownloadProgress -= OnDownloadProgress;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 在共享操作启动时发生的事件
        /// </summary>
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args, List<StorageFile> fileList)
        {
            DataRequestDeferral dataRequestDeferral = args.Request.GetDeferral();

            try
            {
                args.Request.Data.Properties.Title = FileShareString;
                args.Request.Data.SetStorageItems(fileList);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnDataRequested), 1, e);
            }
            finally
            {
                dataRequestDeferral.Complete();
            }
        }

        #endregion 第三部分：下载管理页面——自定义事件

        private static MemoryStream CreateShellIDList(StringCollection fileNameCollection)
        {
            int pos = 0;
            byte[][] pidls = new byte[fileNameCollection.Count][];
            foreach (object filename in fileNameCollection)
            {
                nint pidl = Shell32Library.ILCreateFromPath(filename.ToString());
                int pidlSize = Shell32Library.ILGetSize(pidl);
                pidls[pos] = new byte[pidlSize];
                Marshal.Copy(pidl, pidls[pos++], 0, pidlSize);
                Shell32Library.ILFree(pidl);
            }

            int pidlOffset = 4 * (fileNameCollection.Count + 2);
            MemoryStream memoryStream = new();
            BinaryWriter binaryWriter = new(memoryStream);
            binaryWriter.Write(fileNameCollection.Count);
            binaryWriter.Write(pidlOffset);
            pidlOffset += 4;
            foreach (byte[] pidl in pidls)
            {
                binaryWriter.Write(pidlOffset);
                pidlOffset += pidl.Length;
            }

            binaryWriter.Write(0);
            foreach (byte[] pidl in pidls)
            {
                binaryWriter.Write(pidl);
            }

            return memoryStream;
        }
    }
}
