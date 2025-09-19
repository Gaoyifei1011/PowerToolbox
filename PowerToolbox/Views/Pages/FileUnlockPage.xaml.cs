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
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件解锁页面
    /// </summary>
    public sealed partial class FileUnlockPage : Page, INotifyPropertyChanged
    {
        private readonly string DragOverContentString = ResourceService.FileUnlockResource.GetString("DragOverContent");
        private readonly string FileString = ResourceService.FileUnlockResource.GetString("File");
        private readonly string FolderString = ResourceService.FileUnlockResource.GetString("Folder");
        private readonly string ModifyingNowString = ResourceService.FileUnlockResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.FileUnlockResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileUnlockResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileUnlockResource.GetString("Total");
        private CancellationTokenSource cancellationTokenSource;

        private bool _isModifyingNow;

        public bool IsModifyingNow
        {
            get { return _isModifyingNow; }

            set
            {
                if (!Equals(_isModifyingNow, value))
                {
                    _isModifyingNow = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModifyingNow)));
                }
            }
        }

        private bool _isOperationCancelling;

        public bool IsOperationCancelling
        {
            get { return _isOperationCancelling; }

            set
            {
                if (!Equals(_isOperationCancelling, value))
                {
                    _isOperationCancelling = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOperationCancelling)));
                }
            }
        }

        private bool _isOperationFailed;

        public bool IsOperationFailed
        {
            get { return _isOperationFailed; }

            set
            {
                if (!Equals(_isOperationFailed, value))
                {
                    _isOperationFailed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOperationFailed)));
                }
            }
        }

        private List<FileUnlockFailedModel> FileUnlockFailedList { get; } = [];

        private ObservableCollection<FileUnlockModel> FileUnlockCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileUnlockPage()
        {
            InitializeComponent();
        }

        #region 第一部分：第一部分：重写父类事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override void OnDragOver(global::Windows.UI.Xaml.DragEventArgs args)
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
        protected override async void OnDrop(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();
            List<IStorageItem> storageItemList = [];
            try
            {
                DataPackageView dataPackageView = args.DataView;
                if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                {
                    storageItemList.AddRange(await Task.Run(async () =>
                    {
                        return await dataPackageView.GetStorageItemsAsync();
                    }));
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            List<FileUnlockModel> fileUnlockList = await Task.Run(() =>
            {
                List<FileUnlockModel> fileUnlockList = [];

                foreach (IStorageItem storageItem in storageItemList)
                {
                    try
                    {
                        FileInfo fileInfo = new(storageItem.Path);
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
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnDrop), 2, e);
                    }
                }

                return fileUnlockList;
            });

            await AddToFileUnlockPageAsync(fileUnlockList);
            IsOperationFailed = false;
            FileUnlockFailedList.Clear();
        }

        /// <summary>
        /// 按下 Enter 键发生的事件（预览修改内容）
        /// </summary>
        protected override async void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key is VirtualKey.Enter && !IsModifyingNow)
            {
                args.Handled = true;
                IsOperationFailed = false;
                FileUnlockFailedList.Clear();
                await RemoveUnlockAsync();
            }
        }

        #endregion 第一部分：第一部分：重写父类事件

        #region 第三部分：文件解锁页面——挂载的事件

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
                List<FileUnlockModel> fileUnlockList = await Task.Run(() =>
                {
                    List<FileUnlockModel> fileUnlockList = [];

                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnSelectFileClicked), 1, e);
                        }
                    }

                    return fileUnlockList;
                });

                openFileDialog.Dispose();
                await AddToFileUnlockPageAsync(fileUnlockList);
            }
            else
            {
                openFileDialog.Dispose();
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        private async void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                IsOperationFailed = false;
                FileUnlockFailedList.Clear();
                if (!string.IsNullOrEmpty(openFolderDialog.SelectedPath))
                {
                    List<FileUnlockModel> fileUnlockList = await Task.Run(() =>
                    {
                        List<FileUnlockModel> fileUnlockList = [];

                        try
                        {
                            FileInfo fileInfo = new(openFolderDialog.SelectedPath);
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnSelectFolderClicked), 1, e);
                        }

                        return fileUnlockList;
                    });
                }
            }
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
            Task.Run(() =>
            {
                try
                {
                    Process.Start("taskmgr.exe");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnOpenTaskManagerClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 查看解锁失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new FileUnlockFailedDialog(FileUnlockFailedList));
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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnCancelUnlockClicked), 1, e);
            }
        }

        #endregion 第三部分：文件解锁页面——挂载的事件

        /// <summary>
        /// 添加到文件解锁页面
        /// </summary>
        public async Task AddToFileUnlockPageAsync(List<FileUnlockModel> fileUnlockList)
        {
            await Task.Run(() =>
            {
                foreach (FileUnlockModel fileUnlockItem in fileUnlockList)
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(AddToFileUnlockPageAsync), 1, e);
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

            foreach (FileUnlockModel fileUnlockItem in fileUnlockList)
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
                        await Task.Run(() =>
                        {
                            // 查找进程
                            List<Process> processList = [];
                            uint handle = 0;
                            try
                            {
                                Guid keyGuid = Guid.NewGuid();
                                int result = RstrtmgrLibrary.RmStartSession(out handle, 0, Convert.ToString(keyGuid));

                                if (result is 0)
                                {
                                    uint pnProcInfo = 0;
                                    string[] resources = [subFile];

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
                                                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(RemoveUnlockAsync), 1, e);
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
                                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(RemoveUnlockAsync), 2, e);
                            }
                            finally
                            {
                                RstrtmgrLibrary.RmEndSession(handle);
                            }

                            // 结束进程
                            foreach (Process process in processList)
                            {
                                try
                                {
                                    process?.Kill();
                                }
                                catch (Exception e)
                                {
                                    unlockSuccessfully = false;
                                    try
                                    {
                                        FileUnlockFailedList.Add(new FileUnlockFailedModel()
                                        {
                                            Exception = e,
                                            FileName = subFile,
                                            FilePath = subFile,
                                            ProcessName = Path.GetFileName(process.MainModule.FileVersionInfo.FileName),
                                            ProcessId = Convert.ToString(process.Id),
                                            ProcessPath = process.MainModule.FileName,
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(RemoveUnlockAsync), 3, e);
                                    }
                                }
                                finally
                                {
                                    process?.Dispose();
                                }
                            }
                        });

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
        }
    }
}
