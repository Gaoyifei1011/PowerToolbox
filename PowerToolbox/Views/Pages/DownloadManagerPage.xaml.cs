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
    internal sealed partial class DownloadManagerPage : Page
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DownloadingCountInfoString = ResourceService.DownloadManagerResource.GetString("DownloadingCountInfo");
        private readonly string FileShareString = ResourceService.DownloadManagerResource.GetString("FileShare");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly IDataTransferManagerInterop dataTransferManagerInterop = (IDataTransferManagerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DataTransferManager));
        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private WinRTObservableCollection<DownloadModel> DownloadCollection { get; } = [];

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal DownloadManagerPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await MountDownloadEventAsync();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                download.IsOperating = true;
                ContinueDownload(download);
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
                PauseDownload(download);
            }
        }

        /// <summary>
        /// 打开下载文件所属目录
        /// </summary>
        private void OnOpenFolderExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath)
            {
                OpenFolder(filePath);
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private async void OnDeleteExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadModel download && !string.IsNullOrEmpty(download.DownloadID))
            {
                if (download.DownloadProgressState is DownloadProgressState.Queued || download.DownloadProgressState is DownloadProgressState.Downloading || download.DownloadProgressState is DownloadProgressState.Paused)
                {
                    DownloadSchedulerService.DeleteDownload(download.DownloadID);
                }
                else if (download.DownloadProgressState is DownloadProgressState.Finished)
                {
                    DeleteFileDialog deleteFileDialog = new();
                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(deleteFileDialog);

                    if (contentDialogResult is ContentDialogResult.Primary)
                    {
                        download.IsOperating = true;

                        if (deleteFileDialog.DeleteFileSameTime)
                        {
                            bool result = await DeleteFileAsync(download.FilePath);

                            if (result)
                            {
                                DeleteDownload(download);
                            }
                            else
                            {
                                download.IsOperating = false;
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteFileFailed));
                            }
                        }
                        else
                        {
                            DeleteDownload(download);
                        }
                    }
                }
                else
                {
                    DeleteDownload(download);
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
                        if (fileList is not null && fileList.Count > 0)
                        {
                            ShowShareUI(fileList);
                        }
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
                ViewFileInformation(filePath);
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

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
                    ContinueDownload(downloadItem);
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
                    PauseDownload(downloadItem);
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
                DeleteDownload(downloadItem);
            }
        }

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenDownloadFolder();
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
            OpenNetworkInternetSettings();
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        private async void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadSplitView.IsPaneOpen = false;
            await Task.Delay(300);
        }

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
                    if (ThumbnailHelper.GetThumbnailBitmap(downloadScheduler.FilePath, 256) is Bitmap thumbnailBitmap)
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

        /// <summary>
        /// 应用程序即将关闭时发生的事件
        /// </summary>
        private void OnApplicationExit()
        {
            DismountDownloadEvent();
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 挂载与下载相关的事件
        /// </summary>
        private async Task MountDownloadEventAsync()
        {
            await Task.Run(() =>
            {
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
                DownloadSchedulerService.DownloadProgress += OnDownloadProgress;
            });
        }

        /// <summary>
        /// 卸载与下载相关的事件
        /// </summary>
        private void DismountDownloadEvent()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                DownloadSchedulerService.DownloadProgress -= OnDownloadProgress;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(DismountDownloadEvent), 1, e);
            }
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        private void ContinueDownload(DownloadModel download)
        {
            if (download is null)
            {
                return;
            }

            DownloadSchedulerService.ContinueDownload(download.DownloadID);
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        private void DeleteDownload(DownloadModel download)
        {
            if (download is null)
            {
                return;
            }

            if (download.DownloadProgressState is DownloadProgressState.Queued || download.DownloadProgressState is DownloadProgressState.Downloading || download.DownloadProgressState is DownloadProgressState.Paused)
            {
                DownloadSchedulerService.DeleteDownload(download.DownloadID);
            }
            else
            {
                DownloadCollection.Remove(download);
            }
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        private void PauseDownload(DownloadModel download)
        {
            if (download is null)
            {
                return;
            }

            DownloadSchedulerService.PauseDownload(download.DownloadID);
        }

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private void OpenDownloadFolder()
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OpenDownloadFolder), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开网络和 Internet 设置
        /// </summary>
        private void OpenNetworkInternetSettings()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:network-status");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OpenNetworkInternetSettings), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开文件所在的文件夹
        /// </summary>
        private void OpenFolder(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OpenFolder), 1, e);
                }
            });
        }

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private void ViewFileInformation(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(ViewFileInformation), 1, e);
                }
            });
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        private async Task<bool> DeleteFileAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                // 删除文件
                try
                {
                    if (File.Exists(filePath))
                    {
                        FileSystem.DeleteFile(filePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(DeleteFileAsync), 1, e);
                    return false;
                }
            });
        }

        /// <summary>
        /// 显示分享面板
        /// </summary>
        private void ShowShareUI(List<StorageFile> fileList)
        {
            if (fileList is null || fileList.Count is 0)
            {
                return;
            }

            if (dataTransferManagerInterop is not null)
            {
                dataTransferManagerInterop.GetForWindow((nint)MainWindow.Current.AppWindow.Id.Value, new("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C"), out DataTransferManager dataTransferManager);
                dataTransferManager.DataRequested += (sender, args) => OnDataRequested(sender, args, fileList);
                dataTransferManagerInterop.ShowShareUIForWindow((nint)MainWindow.Current.AppWindow.Id.Value);
            }
        }

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

        #endregion 第七部分：数据操作与业务逻辑
    }
}
