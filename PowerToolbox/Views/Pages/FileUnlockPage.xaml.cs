using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
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
        public readonly string FileString = ResourceService.FileUnlockResource.GetString("File");
        public readonly string FolderString = ResourceService.FileUnlockResource.GetString("Folder");
        private readonly string ModifyingNowString = ResourceService.FileUnlockResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.FileUnlockResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileUnlockResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileUnlockResource.GetString("Total");
        private readonly object fileUnlockLock = new();

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

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        private ObservableCollection<FileUnlockModel> FileUnlockCollection { get; } = [];

        private ObservableCollection<ProcessInfoModel> ProcessInfoCollection { get; } = [];

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
                            FileFolderName = storageItem.Name,
                            FileFolderPath = storageItem.Path,
                        };

                        // 选择的是目录
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is System.IO.FileAttributes.Directory)
                        {
                            fileUnlock.FileFolderType = FolderString;
                            string[] subFileArray = Directory.GetFiles(storageItem.Path, "*", SearchOption.AllDirectories);
                            fileUnlock.SubFileList.AddRange(subFileArray);
                        }
                        // 选择的是文件
                        else
                        {
                            fileUnlock.FileFolderType = FileString;
                            fileUnlock.SubFileList.Add(storageItem.Path);
                        }

                        fileUnlock.FileFolderAmount = Convert.ToString(fileUnlock.SubFileList.Count);
                        fileUnlockList.Add(fileUnlock);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileUnlockPage), nameof(OnDrop), 2, e);
                    }
                }

                return fileUnlockList;
            });

            AddToFileUnlockPage(fileUnlockList);
            IsOperationFailed = false;
            OperationFailedList.Clear();
        }

        /// <summary>
        /// 按下 Enter 键发生的事件（预览修改内容）
        /// </summary>
        protected override void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                IsOperationFailed = false;
                OperationFailedList.Clear();
                // TODO：未完成
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
                OperationFailedList.Clear();
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
                            };

                            // 选择的是目录
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is System.IO.FileAttributes.Directory)
                            {
                                fileUnlock.FileFolderType = FolderString;
                                string[] subFileArray = Directory.GetFiles(fileInfo.FullName, "*", SearchOption.AllDirectories);
                                fileUnlock.SubFileList.AddRange(subFileArray);
                            }
                            // 选择的是文件
                            else
                            {
                                fileUnlock.FileFolderType = FileString;
                                fileUnlock.SubFileList.Add(fileInfo.FullName);
                            }

                            fileUnlock.FileFolderAmount = Convert.ToString(fileUnlock.SubFileList.Count);
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
                AddToFileUnlockPage(fileUnlockList);
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
                OperationFailedList.Clear();
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
                            };

                            // 选择的是目录
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is System.IO.FileAttributes.Directory)
                            {
                                fileUnlock.FileFolderType = FolderString;
                                string[] subFileArray = Directory.GetFiles(fileInfo.FullName, "*", SearchOption.AllDirectories);
                                fileUnlock.SubFileList.AddRange(subFileArray);
                            }
                            // 选择的是文件
                            else
                            {
                                fileUnlock.FileFolderType = FileString;
                                fileUnlock.SubFileList.Add(fileInfo.FullName);
                            }

                            fileUnlock.FileFolderAmount = Convert.ToString(fileUnlock.SubFileList.Count);
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
            lock (fileUnlockLock)
            {
                IsOperationFailed = false;
                FileUnlockCollection.Clear();
                OperationFailedList.Clear();
            }
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
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedList));
        }

        /// <summary>
        /// 解除文件占用
        /// </summary>
        private void OnUnlockClicked(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第三部分：文件解锁页面——挂载的事件

        /// <summary>
        /// 添加到文件解锁页面
        /// </summary>
        public void AddToFileUnlockPage(List<FileUnlockModel> fileUnlockList)
        {
            lock (fileUnlockLock)
            {
                foreach (FileUnlockModel fileUnlockItem in fileUnlockList)
                {
                    FileUnlockCollection.Add(fileUnlockItem);
                }
            }
        }
    }
}
