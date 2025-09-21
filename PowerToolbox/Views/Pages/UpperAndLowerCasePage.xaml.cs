using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 大写小写页面
    /// </summary>
    public sealed partial class UpperAndLowerCasePage : Page, INotifyPropertyChanged
    {
        private readonly string DragOverContentString = ResourceService.UpperAndLowerCaseResource.GetString("DragOverContent");
        private readonly string ModifyingNowString = ResourceService.UpperAndLowerCaseResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.UpperAndLowerCaseResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.UpperAndLowerCaseResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.UpperAndLowerCaseResource.GetString("Total");
        private readonly object upperAndLowerCaseLock = new();

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

        private UpperAndLowerSelectedKind _selectedType = UpperAndLowerSelectedKind.None;

        public UpperAndLowerSelectedKind SelectedType
        {
            get { return _selectedType; }

            set
            {
                if (!Equals(_selectedType, value))
                {
                    _selectedType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
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

        public ObservableCollection<OldAndNewNameModel> UpperAndLowerCaseCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public UpperAndLowerCasePage()
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
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpperAndLowerCasePage), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            List<OldAndNewNameModel> upperAndLowerCaseList = await Task.Run(() =>
            {
                List<OldAndNewNameModel> upperAndLowerCaseList = [];

                foreach (IStorageItem storageItem in storageItemList)
                {
                    try
                    {
                        FileInfo fileInfo = new(storageItem.Path);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        upperAndLowerCaseList.Add(new()
                        {
                            OriginalFileName = storageItem.Name,
                            OriginalFilePath = storageItem.Path,
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpperAndLowerCasePage), nameof(OnDrop), 2, e);
                        continue;
                    }
                }

                return upperAndLowerCaseList;
            });

            AddtoUpperAndLowerCasePage(upperAndLowerCaseList);
            IsOperationFailed = false;
            OperationFailedList.Clear();
        }

        /// <summary>
        /// 按下 Enter 键发生的事件（预览修改内容）
        /// 按下 Ctrl + Enter 键发生的事件（修改内容）
        /// </summary>
        protected override async void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    IsOperationFailed = false;
                    OperationFailedList.Clear();
                    int count = 0;

                    lock (upperAndLowerCaseLock)
                    {
                        count = UpperAndLowerCaseCollection.Count;
                    }

                    if (count is 0)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileName();
                    }
                }
                else
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NoOperation));
                }
            }
            else if (args.Key is VirtualKey.Control && args.Key is VirtualKey.Enter)
            {
                args.Handled = true;
                bool checkResult = CheckOperationState();
                if (checkResult)
                {
                    IsOperationFailed = false;
                    OperationFailedList.Clear();
                    int count = 0;

                    lock (upperAndLowerCaseLock)
                    {
                        count = UpperAndLowerCaseCollection.Count;
                    }

                    if (count is 0)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileName();
                        await ChangeFileNameAsync();
                    }
                }
                else
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NoOperation));
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
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
            {
                UpperAndLowerCaseCollection.Remove(oldAndNewName);
            }
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        private void OnMoveDownExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
            {
                int index = UpperAndLowerCaseCollection.IndexOf(oldAndNewName);

                if (index >= 0 && index < UpperAndLowerCaseCollection.Count - 1)
                {
                    OldAndNewNameModel upOldAndNewName = UpperAndLowerCaseCollection[index];
                    OldAndNewNameModel downOldAndNewName = UpperAndLowerCaseCollection[index + 1];
                    UpperAndLowerCaseCollection[index] = downOldAndNewName;
                    UpperAndLowerCaseCollection[index + 1] = upOldAndNewName;
                }
            }
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        private void OnMoveUpExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
            {
                int index = UpperAndLowerCaseCollection.IndexOf(oldAndNewName);

                if (index > 0)
                {
                    OldAndNewNameModel upOldAndNewName = UpperAndLowerCaseCollection[index - 1];
                    OldAndNewNameModel downOldAndNewName = UpperAndLowerCaseCollection[index];
                    UpperAndLowerCaseCollection[index - 1] = downOldAndNewName;
                    UpperAndLowerCaseCollection[index] = upOldAndNewName;
                }
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：大写小写页面——挂载的事件

        /// <summary>
        /// 选中时触发的事件
        /// </summary>
        private void OnChecked(object sender, RoutedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.CheckBox checkBox && checkBox.Tag is UpperAndLowerSelectedKind upperAndLowerSelectedKind)
            {
                SelectedType = upperAndLowerSelectedKind;
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (upperAndLowerCaseLock)
            {
                IsOperationFailed = false;
                UpperAndLowerCaseCollection.Clear();
                OperationFailedList.Clear();
            }
        }

        /// <summary>
        /// 预览修改的内容
        /// </summary>
        private async void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                IsOperationFailed = false;
                OperationFailedList.Clear();
                int count = 0;

                lock (upperAndLowerCaseLock)
                {
                    count = UpperAndLowerCaseCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName();
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NoOperation));
            }
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private async void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            bool checkResult = CheckOperationState();
            if (checkResult)
            {
                IsOperationFailed = false;
                OperationFailedList.Clear();
                int count = 0;

                lock (upperAndLowerCaseLock)
                {
                    count = UpperAndLowerCaseCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName();
                    await ChangeFileNameAsync();
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NoOperation));
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
                List<OldAndNewNameModel> upperAndLowerCaseList = await Task.Run(() =>
                {
                    List<OldAndNewNameModel> upperAndLowerCaseList = [];

                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }

                            upperAndLowerCaseList.Add(new()
                            {
                                OriginalFileName = fileInfo.Name,
                                OriginalFilePath = fileInfo.FullName
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpperAndLowerCasePage), nameof(OnSelectFileClicked), 1, e);
                            continue;
                        }
                    }

                    return upperAndLowerCaseList;
                });

                openFileDialog.Dispose();
                AddtoUpperAndLowerCasePage(upperAndLowerCaseList);
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
                    List<OldAndNewNameModel> directoryNameList = [];
                    List<OldAndNewNameModel> fileNameList = [];

                    await Task.Run(() =>
                    {
                        DirectoryInfo currentFolder = new(openFolderDialog.SelectedPath);

                        try
                        {
                            foreach (DirectoryInfo directoryInfo in currentFolder.GetDirectories())
                            {
                                if ((directoryInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                directoryNameList.Add(new()
                                {
                                    OriginalFileName = directoryInfo.Name,
                                    OriginalFilePath = directoryInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpperAndLowerCasePage), nameof(OnSelectFolderClicked), 1, e);
                        }

                        try
                        {
                            foreach (FileInfo fileInfo in currentFolder.GetFiles())
                            {
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                fileNameList.Add(new()
                                {
                                    OriginalFileName = fileInfo.Name,
                                    OriginalFilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(UpperAndLowerCasePage), nameof(OnSelectFolderClicked), 2, e);
                        }
                    });

                    AddtoUpperAndLowerCasePage(directoryNameList);
                    AddtoUpperAndLowerCasePage(fileNameList);
                }

                openFolderDialog.Dispose();
            }
            else
            {
                openFolderDialog.Dispose();
            }
        }

        /// <summary>
        /// 取消选中时触发的事件
        /// </summary>
        private void OnUnchecked(object sender, RoutedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.CheckBox checkBox && checkBox.Tag is UpperAndLowerSelectedKind upperAndLowerSelectedKind)
            {
                if (Equals(SelectedType, upperAndLowerSelectedKind))
                {
                    SelectedType = UpperAndLowerSelectedKind.None;
                }
            }
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedList));
        }

        #endregion 第三部分：大写小写页面——挂载的事件

        /// <summary>
        /// 添加到大写小写页面
        /// </summary>
        public void AddtoUpperAndLowerCasePage(List<OldAndNewNameModel> upperAndLowerCaseList)
        {
            lock (upperAndLowerCaseLock)
            {
                foreach (OldAndNewNameModel oldAndNewNameItem in upperAndLowerCaseList)
                {
                    UpperAndLowerCaseCollection.Add(oldAndNewNameItem);
                }
            }
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            return SelectedType is not UpperAndLowerSelectedKind.None;
        }

        /// <summary>
        /// 预览修改后的文件名称
        /// </summary>
        private void PreviewChangedFileName()
        {
            switch (SelectedType)
            {
                case UpperAndLowerSelectedKind.AllUppercase:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.ToUpper();
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.FileNameUppercase:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName).ToUpper();
                                    string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                                    oldAndNewNameItem.NewFileName = fileName + extensionName;
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ExtensionNameUppercase:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                    string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName).ToUpper();
                                    oldAndNewNameItem.NewFileName = fileName + extensionName;
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.DeleteSpace:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.Replace(" ", string.Empty);
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.AllLowercase:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.ToLower();
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.FileNameLowercase:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName).ToLower();
                                    string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                                    oldAndNewNameItem.NewFileName = fileName + extensionName;
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ExtensionNameLowercase:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                    string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName).ToLower();
                                    oldAndNewNameItem.NewFileName = fileName + extensionName;
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case UpperAndLowerSelectedKind.ReplaceSpace:
                    {
                        lock (upperAndLowerCaseLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    oldAndNewNameItem.NewFileName = oldAndNewNameItem.OriginalFileName.Replace(" ", "_");
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 更改文件名称
        /// </summary>
        private async Task ChangeFileNameAsync()
        {
            IsModifyingNow = true;
            foreach (OldAndNewNameModel oldAndNewName in UpperAndLowerCaseCollection)
            {
                oldAndNewName.IsModifyingNow = true;
            }

            List<OperationFailedModel> operationFailedList = await Task.Run(() =>
            {
                List<OperationFailedModel> operationFailedList = [];

                lock (upperAndLowerCaseLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in UpperAndLowerCaseCollection)
                    {
                        if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName) && !string.IsNullOrEmpty(oldAndNewNameItem.OriginalFilePath))
                        {
                            if ((new FileInfo(oldAndNewNameItem.OriginalFilePath).Attributes & System.IO.FileAttributes.Directory) is not 0)
                            {
                                try
                                {
                                    Directory.Move(oldAndNewNameItem.OriginalFilePath, oldAndNewNameItem.NewFilePath);
                                }
                                catch (Exception e)
                                {
                                    operationFailedList.Add(new OperationFailedModel()
                                    {
                                        FileName = oldAndNewNameItem.OriginalFileName,
                                        FilePath = oldAndNewNameItem.OriginalFilePath,
                                        Exception = e
                                    });
                                }
                            }
                            else
                            {
                                try
                                {
                                    File.Move(oldAndNewNameItem.OriginalFilePath, oldAndNewNameItem.NewFilePath);
                                }
                                catch (Exception e)
                                {
                                    operationFailedList.Add(new OperationFailedModel()
                                    {
                                        FileName = oldAndNewNameItem.OriginalFileName,
                                        FilePath = oldAndNewNameItem.OriginalFilePath,
                                        Exception = e
                                    });
                                }
                            }
                        }
                    }
                }

                return operationFailedList;
            });

            IsModifyingNow = false;
            foreach (OldAndNewNameModel oldAndNewName in UpperAndLowerCaseCollection)
            {
                oldAndNewName.IsModifyingNow = false;
            }
            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedList.Add(operationFailedItem);
            }

            int count = UpperAndLowerCaseCollection.Count;
            IsOperationFailed = OperationFailedList.Count is not 0;

            lock (upperAndLowerCaseLock)
            {
                UpperAndLowerCaseCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.File, count - OperationFailedList.Count, OperationFailedList.Count));
        }
    }
}
