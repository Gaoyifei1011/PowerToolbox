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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数字签名页面
    /// </summary>
    public sealed partial class FileCertificatePage : Page, INotifyPropertyChanged
    {
        private readonly string DragOverContentString = ResourceService.FileCertificateResource.GetString("DragOverContent");
        private readonly string ModifyingNowString = ResourceService.FileCertificateResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.FileCertificateResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileCertificateResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileCertificateResource.GetString("Total");
        private readonly object fileCertificateLock = new();

        private bool _isModifyingNow = false;

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

        private ObservableCollection<CertificateResultModel> FileCertificateCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileCertificatePage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

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
            finally
            {
                dragOperationDeferral.Complete();
            }

            List<CertificateResultModel> fileCertificateList = await Task.Run(() =>
            {
                List<CertificateResultModel> fileCertificateList = [];

                foreach (IStorageItem storageItem in storageItemList)
                {
                    try
                    {
                        FileInfo fileInfo = new(storageItem.Path);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is 0)
                        {
                            fileCertificateList.Add(new CertificateResultModel()
                            {
                                FileName = storageItem.Name,
                                FilePath = storageItem.Path
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileCertificatePage), nameof(OnDrop), 1, e);
                        continue;
                    }
                }

                return fileCertificateList;
            });

            AddToFileCertificatePage(fileCertificateList);
            IsOperationFailed = false;
            OperationFailedList.Clear();
        }

        /// <summary>
        /// 按下 Enter 键发生的事件（预览修改内容）
        /// </summary>
        protected override async void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
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
                    await RemoveFileCertificatesAsync();
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 删除当前项
        /// </summary>
        private void OnDeleteExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CertificateResultModel certificateResult)
            {
                FileCertificateCollection.Remove(certificateResult);
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：文件证书页面——挂载的事件

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
                await RemoveFileCertificatesAsync();
            }
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
                List<CertificateResultModel> fileCertificateList = await Task.Run(() =>
                {
                    List<CertificateResultModel> fileCertificateList = [];

                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }

                            if ((fileInfo.Attributes & System.IO.FileAttributes.Directory) is 0)
                            {
                                fileCertificateList.Add(new CertificateResultModel()
                                {
                                    FileName = fileInfo.Name,
                                    FilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileCertificatePage), nameof(OnSelectFileClicked), 1, e);
                            continue;
                        }
                    }

                    return fileCertificateList;
                });

                openFileDialog.Dispose();
                AddToFileCertificatePage(fileCertificateList);
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
                    List<CertificateResultModel> fileNameList = await Task.Run(() =>
                    {
                        List<CertificateResultModel> fileNameList = [];
                        DirectoryInfo currentFolder = new(openFolderDialog.SelectedPath);

                        try
                        {
                            foreach (FileInfo fileInfo in currentFolder.GetFiles())
                            {
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                fileNameList.Add(new CertificateResultModel()
                                {
                                    FileName = fileInfo.Name,
                                    FilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileCertificatePage), nameof(OnSelectFolderClicked), 1, e);
                        }

                        return fileNameList;
                    });

                    AddToFileCertificatePage(fileNameList);
                }

                openFolderDialog.Dispose();
            }
            else
            {
                openFolderDialog.Dispose();
            }
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedList));
        }

        #endregion 第三部分：文件证书页面——挂载的事件

        /// <summary>
        /// 添加到数字签名页面
        /// </summary>
        public void AddToFileCertificatePage(List<CertificateResultModel> fileCertificateList)
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
        /// 移除文件证书
        /// </summary>
        private async Task RemoveFileCertificatesAsync()
        {
            IsModifyingNow = true;
            foreach (CertificateResultModel certificateResult in FileCertificateCollection)
            {
                certificateResult.IsModifyingNow = true;
            }

            List<OperationFailedModel> operationFailedList = await Task.Run(() =>
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
                                    operationFailedList.Add(new OperationFailedModel()
                                    {
                                        FileName = certificateResultItem.FileName,
                                        FilePath = certificateResultItem.FilePath,
                                        Exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error())
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
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

            IsModifyingNow = false;
            foreach (CertificateResultModel certificateResult in FileCertificateCollection)
            {
                certificateResult.IsModifyingNow = false;
            }
            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedList.Add(operationFailedItem);
            }

            IsOperationFailed = OperationFailedList.Count is not 0;
            int count = FileCertificateCollection.Count;

            lock (fileCertificateLock)
            {
                FileCertificateCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.File, count - OperationFailedList.Count, OperationFailedList.Count));
        }
    }
}
