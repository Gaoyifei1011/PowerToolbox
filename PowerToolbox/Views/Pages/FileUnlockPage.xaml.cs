using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using PowerToolbox.Extensions.DataType.Classes;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Rstrtmgr;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件解锁页面
    /// </summary>
    internal sealed partial class FileUnlockPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DragOverContentString = ResourceService.FileUnlockResource.GetString("DragOverContent");
        private readonly string FileString = ResourceService.FileUnlockResource.GetString("File");
        private readonly string FolderString = ResourceService.FileUnlockResource.GetString("Folder");
        private readonly string ModifyingNowString = ResourceService.FileUnlockResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.FileUnlockResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileUnlockResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileUnlockResource.GetString("Total");
        private CancellationTokenSource cancellationTokenSource;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        internal bool IsModifyingNow
        {
            get;

            private set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifyingNow)));
                }
            }
        }

        private bool IsOperationCancelling
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperationCancelling)));
                }
            }
        }

        private bool IsOperationFailed
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperationFailed)));
                }
            }
        }

        private List<FileUnlockFailedModel> FileUnlockFailedList { get; } = [];

        private WinRTObservableCollection<FileUnlockModel> FileUnlockCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal FileUnlockPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override void OnDragOver(Microsoft.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);
            if (IsModifyingNow)
            {
                args.AcceptedOperation = DataPackageOperation.None;
                args.DragUIOverride.IsCaptionVisible = true;
                args.DragUIOverride.IsContentVisible = false;
                args.DragUIOverride.IsGlyphVisible = true;
                args.DragUIOverride.Caption = ModifyingNowString;
            }
            else
            {
                args.AcceptedOperation = DataPackageOperation.Copy;
                args.DragUIOverride.IsCaptionVisible = true;
                args.DragUIOverride.IsContentVisible = false;
                args.DragUIOverride.IsGlyphVisible = true;
                args.DragUIOverride.Caption = DragOverContentString;
            }
            args.Handled = true;
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(Microsoft.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();
            try
            {
                if (await GetDragDropSelectedFileCollectionAsync(args.DataView) is ReadOnlyCollection<string> fileCollection && fileCollection.Count > 0 && await GetNeedUnlockFileCollectionAsync(fileCollection) is ReadOnlyCollection<FileUnlockModel> fileUnlockCollection && fileUnlockCollection.Count > 0)
                {
                    await AddToFileUnlockPageAsync(fileUnlockCollection);
                    IsOperationFailed = false;
                    FileUnlockFailedList.Clear();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnDrop), 1, e);
            }
            finally
            {
                args.Handled = true;
                dragOperationDeferral.Complete();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 删除当前项
        /// </summary>
        private void OnDeleteExecuteRequested(object sender, Extensions.DataType.Classes.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is FileUnlockModel fileUnlock)
            {
                lock (fileUnlock)
                {
                    FileUnlockCollection.Remove(fileUnlock);
                }
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 按下 Enter 键发生的事件（进行文件解锁）
        /// </summary>
        private async void OnKeyBoardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (sender.Key is VirtualKey.Enter && !IsModifyingNow)
            {
                IsOperationFailed = false;
                FileUnlockFailedList.Clear();
                await RemoveUnlockAsync();
            }
            args.Handled = true;
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private async void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Title = SelectFileString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK)
            {
                IsOperationFailed = false;
                FileUnlockFailedList.Clear();
                if (await GetNeedUnlockFileCollectionAsync(new(openFileDialog.FileNames)) is ReadOnlyCollection<FileUnlockModel> fileUnlockCollection && fileUnlockCollection.Count > 0)
                {
                    await AddToFileUnlockPageAsync(fileUnlockCollection);
                }
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private async void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                IsOperationFailed = false;
                FileUnlockFailedList.Clear();
                if (!string.IsNullOrEmpty(openFolderDialog.SelectedPath) && await GetNeedUnlockFileCollectionAsync(new([openFolderDialog.SelectedPath])) is ReadOnlyCollection<FileUnlockModel> fileUnlockCollection && fileUnlockCollection.Count > 0)
                {
                    await AddToFileUnlockPageAsync(fileUnlockCollection);
                }
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            IsOperationFailed = false;
            FileUnlockCollection.Clear();
            FileUnlockFailedList.Clear();
        }

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OnOpenTaskManagerClicked(object sender, RoutedEventArgs args)
        {
            OpenTaskManager();
        }

        /// <summary>
        /// 查看解锁失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new FileUnlockFailedDialog(FileUnlockFailedList.AsReadOnly()));
        }

        /// <summary>
        /// 解除文件占用
        /// </summary>
        private async void OnUnlockClicked(object sender, RoutedEventArgs args)
        {
            await RemoveUnlockAsync();
        }

        /// <summary>
        /// 取消解除文件占用
        /// </summary>
        private void OnCancelUnlockClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                cancellationTokenSource?.Cancel();
                IsOperationCancelling = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnCancelUnlockClicked), 1, e);
            }
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 添加到文件解锁页面
        /// </summary>
        internal async Task AddToFileUnlockPageAsync(ReadOnlyCollection<FileUnlockModel> fileUnlockCollection)
        {
            if (fileUnlockCollection is null || fileUnlockCollection.Count is 0)
            {
                return;
            }

            await Task.Run(() =>
            {
                foreach (FileUnlockModel fileUnlockItem in fileUnlockCollection)
                {
                    // 选择的是目录
                    if (fileUnlockItem.IsDirectory)
                    {
                        fileUnlockItem.FileFolderType = FolderString;

                        try
                        {
                            string[] subFileArray = Directory.GetFiles(fileUnlockItem.FileFolderPath, "*", SearchOption.AllDirectories);
                            fileUnlockItem.SubFileList.AddRange(subFileArray);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(AddToFileUnlockPageAsync), 1, e);
                        }
                    }
                    // 选择的是文件
                    else
                    {
                        fileUnlockItem.FileFolderType = FileString;
                        fileUnlockItem.SubFileList.Add(fileUnlockItem.FileFolderPath);
                    }

                    fileUnlockItem.FileUnlockState = FileUnlockState.NotStarted;
                    fileUnlockItem.FileUnlockFinishedCount = 0;
                    fileUnlockItem.FileUnlockProgressingPercentage = 0;
                    fileUnlockItem.FileFolderAmount = Convert.ToString(fileUnlockItem.SubFileList.Count);
                }
            });

            foreach (FileUnlockModel fileUnlockItem in fileUnlockCollection)
            {
                FileUnlockCollection.Add(fileUnlockItem);
            }
        }

        /// <summary>
        /// 解除占用
        /// </summary>
        private async Task RemoveUnlockAsync()
        {
            IsModifyingNow = true;
            foreach (FileUnlockModel fileUnlockItem in FileUnlockCollection)
            {
                fileUnlockItem.IsModifyingNow = true;
            }

            try
            {
                cancellationTokenSource = new();

                foreach (FileUnlockModel fileUnlockItem in FileUnlockCollection)
                {
                    // 用户取消任务后自动触发取消任务异常
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    // 已经解锁成功的就不要重复进行了
                    if (fileUnlockItem.FileUnlockState is FileUnlockState.Successfully)
                    {
                        continue;
                    }

                    // 开始解锁
                    fileUnlockItem.FileUnlockState = FileUnlockState.Processing;
                    bool unlockSuccessfully = true;

                    foreach (string subFile in fileUnlockItem.SubFileList)
                    {
                        // 查找进程并结束进程
                        if (await GetProcessCollectionAsync(subFile) is ReadOnlyCollection<Process> processCollection && processCollection.Count > 0 && await TerminateProcessAsync(subFile, processCollection) is ReadOnlyCollection<FileUnlockFailedModel> fileUnlockFailedCollection && (fileUnlockFailedCollection is null || fileUnlockFailedCollection.Count is not 0))
                        {
                            unlockSuccessfully = false;
                            FileUnlockFailedList.AddRange(fileUnlockFailedCollection);
                        }

                        fileUnlockItem.FileUnlockFinishedCount++;
                        fileUnlockItem.FileUnlockProgressingPercentage = fileUnlockItem.SubFileList.Count is 0 ? 0 : Convert.ToInt32((double)fileUnlockItem.FileUnlockFinishedCount * 100 / fileUnlockItem.SubFileList.Count);
                    }

                    fileUnlockItem.FileUnlockState = unlockSuccessfully ? FileUnlockState.Successfully : FileUnlockState.Failed;
                }
            }
            // 任务已取消
            catch (OperationCanceledException)
            {
                foreach (FileUnlockModel fileUnlockItem in FileUnlockCollection)
                {
                    if (fileUnlockItem.FileUnlockState is FileUnlockState.Processing)
                    {
                        fileUnlockItem.FileUnlockState = FileUnlockState.Cancelled;
                    }
                    fileUnlockItem.FileUnlockProgressingPercentage = 0;
                    fileUnlockItem.FileUnlockFinishedCount = 0;
                }
            }
            finally
            {
                IsOperationFailed = FileUnlockFailedList.Count is not 0;
                IsOperationCancelling = false;
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }

            IsModifyingNow = false;
            foreach (FileUnlockModel fileUnlockItem in FileUnlockCollection)
            {
                fileUnlockItem.IsModifyingNow = false;
            }
        }

        /// <summary>
        /// 获取拖拽支持选中的文件
        /// </summary>
        private async Task<ReadOnlyCollection<string>> GetDragDropSelectedFileCollectionAsync(DataPackageView dataPackageView)
        {
            if (dataPackageView is null)
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                List<string> fileList = [];

                try
                {
                    if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                    {
                        foreach (IStorageItem storageItem in await dataPackageView.GetStorageItemsAsync())
                        {
                            fileList.Add(storageItem.Path);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(GetDragDropSelectedFileCollectionAsync), 1, e);
                }

                return fileList.AsReadOnly();
            });
        }

        /// <summary>
        /// 获取要待解锁的的文件列表
        /// </summary>
        private async Task<ReadOnlyCollection<FileUnlockModel>> GetNeedUnlockFileCollectionAsync(ReadOnlyCollection<string> fileCollection)
        {
            if (fileCollection is null || fileCollection.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<FileUnlockModel> fileUnlockList = [];

                foreach (string file in fileCollection)
                {
                    try
                    {
                        FileInfo fileInfo = new(file);
                        FileUnlockModel fileUnlock = new()
                        {
                            FileFolderName = fileInfo.Name,
                            FileFolderPath = fileInfo.FullName,
                            IsDirectory = (fileInfo.Attributes & System.IO.FileAttributes.Directory) is System.IO.FileAttributes.Directory
                        };

                        fileUnlockList.Add(fileUnlock);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(GetNeedUnlockFileCollectionAsync), 1, e);
                        continue;
                    }
                }

                return fileUnlockList.AsReadOnly();
            });
        }

        /// <summary>
        /// 获取文件使用的进程列表
        /// </summary>
        private async Task<ReadOnlyCollection<Process>> GetProcessCollectionAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<Process> processList = [];
                uint handle = 0;
                try
                {
                    Guid keyGuid = Guid.NewGuid();
                    int result = RstrtmgrLibrary.RmStartSession(out handle, 0, Convert.ToString(keyGuid));

                    if (result is 0)
                    {
                        uint pnProcInfo = 0;
                        string[] resources = [fileName];

                        result = RstrtmgrLibrary.RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                        if (result is 0)
                        {
                            result = RstrtmgrLibrary.RmGetList(handle, out uint pnProcInfoNeeded, ref pnProcInfo, null, out uint lpdwRebootReasons);

                            if (result is 234)
                            {
                                RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                                pnProcInfo = pnProcInfoNeeded;
                                result = RstrtmgrLibrary.RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, out lpdwRebootReasons);

                                if (result is 0)
                                {
                                    for (int index = 0; index < pnProcInfo; index++)
                                    {
                                        try
                                        {
                                            Process process = Process.GetProcessById(processInfo[index].Process.dwProcessId);
                                            processList.Add(process);
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(RemoveUnlockAsync), 1, e);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(GetProcessCollectionAsync), 1, e);
                }
                finally
                {
                    RstrtmgrLibrary.RmEndSession(handle);
                }
                return processList.AsReadOnly();
            });
        }

        /// <summary>
        /// 结束进程
        /// </summary>
        private async Task<ReadOnlyCollection<FileUnlockFailedModel>> TerminateProcessAsync(string fileName, ReadOnlyCollection<Process> processCollection)
        {
            if (string.IsNullOrEmpty(fileName) || processCollection is null || processCollection.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<FileUnlockFailedModel> fileUnlockFailedList = [];
                foreach (Process process in processCollection)
                {
                    try
                    {
                        process?.Kill();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            fileUnlockFailedList.Add(new()
                            {
                                Exception = e,
                                FileName = fileName,
                                FilePath = fileName,
                                ProcessName = Path.GetFileName(process.MainModule.FileVersionInfo.FileName),
                                ProcessId = Convert.ToString(process.Id),
                                ProcessPath = process.MainModule.FileName,
                            });
                        }
                        catch (Exception sube)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(TerminateProcessAsync), 1, sube);
                        }
                    }
                    finally
                    {
                        process?.Dispose();
                    }
                }
                return fileUnlockFailedList.AsReadOnly();
            });
        }

        /// <summary>
        /// 打开任务管理器
        /// </summary>
        private void OpenTaskManager()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskmgr.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OpenTaskManager), 1, e);
                }
            });
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
