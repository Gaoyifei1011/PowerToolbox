using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Imagehlp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数字签名页面
    /// </summary>
    internal sealed partial class FileCertificatePage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DragOverContentString = ResourceService.FileCertificateResource.GetString("DragOverContent");
        private readonly string ModifyingNowString = ResourceService.FileCertificateResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.FileCertificateResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileCertificateResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileCertificateResource.GetString("Total");
        private readonly object fileCertificateLock = new();

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isModifyingNow = false;

        internal bool IsModifyingNow
        {
            get { return _isModifyingNow; }

            private set
            {
                if (!Equals(_isModifyingNow, value))
                {
                    _isModifyingNow = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifyingNow)));
                }
            }
        }

        private bool _isOperationFailed;

        private bool IsOperationFailed
        {
            get { return _isOperationFailed; }

            set
            {
                if (!Equals(_isOperationFailed, value))
                {
                    _isOperationFailed = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOperationFailed)));
                }
            }
        }

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        private WinRTObservableCollection<CertificateResultModel> FileCertificateCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal FileCertificatePage()
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
            List<IStorageItem> storageItemList = [];
            try
            {
                if (await GetDragDropSelectedFilesAsync(args.DataView) is List<string> fileList && fileList.Count > 0 && await GetNeedConvertFileListAsync(fileList) is List<CertificateResultModel> fileCertificateList && fileCertificateList.Count > 0)
                {
                    AddToFileCertificatePage(fileCertificateList);
                    IsOperationFailed = false;
                    OperationFailedList.Clear();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(OnDrop), 1, e);
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
        private void OnDeleteExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CertificateResultModel certificateResult)
            {
                lock (fileCertificateLock)
                {
                    FileCertificateCollection.Remove(certificateResult);
                }
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 按下 Enter 键发生的事件（预览修改内容）
        /// 按下 Ctrl + Enter 键发生的事件（修改内容）
        /// </summary>
        private async void OnKeyBoardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (sender.Key is VirtualKey.Enter)
            {
                await ChangeFileAsync();
                args.Handled = true;
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (fileCertificateLock)
            {
                IsOperationFailed = false;
                FileCertificateCollection.Clear();
                OperationFailedList.Clear();
            }
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private async void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            await ChangeFileAsync();
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
                OperationFailedList.Clear();
                if (await GetNeedConvertFileListAsync([.. openFileDialog.FileNames]) is List<CertificateResultModel> fileCertificateList && fileCertificateList.Count > 0)
                {
                    AddToFileCertificatePage(fileCertificateList);
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
                OperationFailedList.Clear();
                if (!string.IsNullOrEmpty(openFolderDialog.SelectedPath) && await GetFileAsync(openFolderDialog.SelectedPath) is List<CertificateResultModel> fileNameList && fileNameList.Count > 0)
                {
                    AddToFileCertificatePage(fileNameList);
                }
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedList));
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 添加到数字签名页面
        /// </summary>
        internal void AddToFileCertificatePage(List<CertificateResultModel> fileCertificateList)
        {
            lock (fileCertificateLock)
            {
                foreach (CertificateResultModel certificateResultItem in fileCertificateList)
                {
                    FileCertificateCollection.Add(certificateResultItem);
                }
            }
        }

        /// <summary>
        /// 修改文件
        /// </summary>
        private async Task ChangeFileAsync()
        {
            IsOperationFailed = false;
            OperationFailedList.Clear();
            int count = 0;

            lock (fileCertificateLock)
            {
                count = FileCertificateCollection.Count;
            }

            if (count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
            }
            else
            {
                await RemoveFileCertificateAsync();
            }
        }

        /// <summary>
        /// 移除文件证书
        /// </summary>
        private async Task RemoveFileCertificateAsync()
        {
            IsModifyingNow = true;
            lock (fileCertificateLock)
            {
                foreach (CertificateResultModel certificateResult in FileCertificateCollection)
                {
                    certificateResult.IsModifyingNow = true;
                }
            }

            List<OperationFailedModel> operationFailedList = await GetOperationFailedListAsync();

            IsModifyingNow = false;
            lock (fileCertificateLock)
            {
                foreach (CertificateResultModel certificateResult in FileCertificateCollection)
                {
                    certificateResult.IsModifyingNow = false;
                }
            }
            if (operationFailedList is not null && operationFailedList.Count > 0)
            {
                foreach (OperationFailedModel operationFailedItem in operationFailedList)
                {
                    OperationFailedList.Add(operationFailedItem);
                }
            }

            IsOperationFailed = OperationFailedList.Count is not 0;
            int count = FileCertificateCollection.Count;

            lock (fileCertificateLock)
            {
                FileCertificateCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.File, count - OperationFailedList.Count, OperationFailedList.Count));
        }

        /// <summary>
        /// 获取拖拽支持选中的文件
        /// </summary>
        private async Task<List<string>> GetDragDropSelectedFilesAsync(DataPackageView dataPackageView)
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileCertificatePage), nameof(GetDragDropSelectedFilesAsync), 1, e);
                }

                return fileList;
            });
        }

        /// <summary>
        /// 获取要待转换的文件列表
        /// </summary>
        private async Task<List<CertificateResultModel>> GetNeedConvertFileListAsync(List<string> fileList)
        {
            if (fileList is null || fileList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<CertificateResultModel> fileCertificateList = [];

                foreach (string file in fileList)
                {
                    try
                    {
                        FileInfo fileInfo = new(file);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is 0)
                        {
                            fileCertificateList.Add(new()
                            {
                                FileName = Path.GetFileName(file),
                                FilePath = file
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileCertificatePage), nameof(GetNeedConvertFileListAsync), 1, e);
                        continue;
                    }
                }

                return fileCertificateList;
            });
        }

        /// <summary>
        /// 获取文件夹的所有文件
        /// </summary>
        private async Task<List<CertificateResultModel>> GetFileAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return [];
            }

            return await Task.Run(() =>
            {
                List<CertificateResultModel> fileCertificateList = [];
                DirectoryInfo currentFolder = new(folderPath);

                try
                {
                    foreach (FileInfo fileInfo in currentFolder.GetFiles())
                    {
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        fileCertificateList.Add(new()
                        {
                            FileName = fileInfo.Name,
                            FilePath = fileInfo.FullName
                        });
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileCertificatePage), nameof(GetFileAsync), 1, e);
                }
                return fileCertificateList;
            });
        }

        /// <summary>
        /// 获取失败操作列表
        /// </summary>
        private async Task<List<OperationFailedModel>> GetOperationFailedListAsync()
        {
            return await Task.Run(() =>
            {
                List<OperationFailedModel> operationFailedList = [];

                lock (fileCertificateLock)
                {
                    foreach (CertificateResultModel certificateResultItem in FileCertificateCollection)
                    {
                        if (!string.IsNullOrEmpty(certificateResultItem.FileName) && !string.IsNullOrEmpty(certificateResultItem.FilePath))
                        {
                            try
                            {
                                using FileStream fileStream = new(certificateResultItem.FilePath, FileMode.Open, FileAccess.ReadWrite);
                                bool result = ImagehlpLibrary.ImageRemoveCertificate(fileStream.SafeFileHandle.DangerousGetHandle(), 0);

                                if (!result)
                                {
                                    operationFailedList.Add(new()
                                    {
                                        FileName = certificateResultItem.FileName,
                                        FilePath = certificateResultItem.FilePath,
                                        Exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error())
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new()
                                {
                                    FileName = certificateResultItem.FileName,
                                    FilePath = certificateResultItem.FilePath,
                                    Exception = e
                                });
                            }
                        }
                    }
                }
                return operationFailedList;
            });
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
