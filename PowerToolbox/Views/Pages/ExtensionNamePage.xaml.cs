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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// 扩展名称页面
    /// </summary>
    internal sealed partial class ExtensionNamePage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DragOverContentString = ResourceService.ExtensionNameResource.GetString("DragOverContent");
        private readonly string ModifyingNowString = ResourceService.ExtensionNameResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.ExtensionNameResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.ExtensionNameResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.ExtensionNameResource.GetString("Total");
        private readonly object extensionNameLock = new();

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isModifyingNow;

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

        private ExtensionNameSelectedKind _selectedKind;

        private ExtensionNameSelectedKind SelectedKind
        {
            get { return _selectedKind; }

            set
            {
                if (!Equals(_selectedKind, value))
                {
                    _selectedKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedKind)));
                }
            }
        }

        private string _changeToText;

        private string ChangeToText
        {
            get { return _changeToText; }

            set
            {
                if (!string.Equals(_changeToText, value))
                {
                    _changeToText = value;
                    PropertyChanged?.Invoke(this, new(nameof(ChangeToText)));
                }
            }
        }

        private string _searchText;

        private string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new(nameof(SearchText)));
                }
            }
        }

        private string _replaceText;

        private string ReplaceText
        {
            get { return _replaceText; }

            set
            {
                if (!string.Equals(_replaceText, value))
                {
                    _replaceText = value;
                    PropertyChanged?.Invoke(this, new(nameof(ReplaceText)));
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

        public event PropertyChangedEventHandler PropertyChanged;

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        private WinRTObservableCollection<OldAndNewNameModel> ExtensionNameCollection { get; } = [];

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal ExtensionNamePage()
        {
            InitializeComponent();
            InitializeData();
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
                List<string> fileList = await GetDragDropSelectedFilesAsync(args.DataView);

                if (fileList is not null && fileList.Count > 0)
                {
                    List<OldAndNewNameModel> extensionNameList = await GetNeedConvertFileListAsync(fileList);
                    if (extensionNameList is not null && extensionNameList.Count > 0)
                    {
                        AddToExtensionNamePage(extensionNameList);
                        IsOperationFailed = false;
                        OperationFailedList.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(OnDrop), 1, e);
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
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
            {
                lock (extensionNameLock)
                {
                    ExtensionNameCollection.Remove(oldAndNewName);
                }
            }
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        private void OnMoveDownExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
            {
                lock (extensionNameLock)
                {
                    int index = ExtensionNameCollection.IndexOf(oldAndNewName);

                    if (index >= 0 && index < ExtensionNameCollection.Count - 1)
                    {
                        OldAndNewNameModel upOldAndNewName = ExtensionNameCollection[index];
                        OldAndNewNameModel downOldAndNewName = ExtensionNameCollection[index + 1];
                        ExtensionNameCollection[index] = downOldAndNewName;
                        ExtensionNameCollection[index + 1] = upOldAndNewName;
                    }
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
                lock (extensionNameLock)
                {
                    int index = ExtensionNameCollection.IndexOf(oldAndNewName);

                    if (index > 0)
                    {
                        OldAndNewNameModel upOldAndNewName = ExtensionNameCollection[index - 1];
                        OldAndNewNameModel downOldAndNewName = ExtensionNameCollection[index];
                        ExtensionNameCollection[index - 1] = downOldAndNewName;
                        ExtensionNameCollection[index] = upOldAndNewName;
                    }
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
                if (sender.Modifiers is VirtualKeyModifiers.None)
                {
                    await PrepareChangeFileNameAsync(SelectedKind, ChangeToText, SearchText, ReplaceText, false);
                }
                else if (sender.Modifiers is VirtualKeyModifiers.Control)
                {
                    await PrepareChangeFileNameAsync(SelectedKind, ChangeToText, SearchText, ReplaceText, true);
                }
                args.Handled = true;
            }
        }

        /// <summary>
        /// 改为文本框中的内容发生更改时发生的事件
        /// </summary>
        private void OnChangeToTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                ChangeToText = textBox.Text;
            }
        }

        /// <summary>
        /// 选中时触发的事件
        /// </summary>
        private void OnChecked(object sender, RoutedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.CheckBox checkBox && checkBox.Tag is ExtensionNameSelectedKind extensionNameSelectedKind)
            {
                SelectedKind = extensionNameSelectedKind;
            }
        }

        /// <summary>
        /// 查找文本框中的内容发生更改时发生的事件
        /// </summary>
        private void OnSearchTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                SearchText = textBox.Text;
            }
        }

        /// <summary>
        /// 替换文本框中的内容发生更改时发生的事件
        /// </summary>
        private void OnReplaceTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                ReplaceText = textBox.Text;
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (extensionNameLock)
            {
                IsOperationFailed = false;
                ExtensionNameCollection.Clear();
                OperationFailedList.Clear();
            }
        }

        /// <summary>
        /// 预览修改的内容
        /// </summary>
        private async void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            await PrepareChangeFileNameAsync(SelectedKind, ChangeToText, SearchText, ReplaceText, false);
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private async void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            await PrepareChangeFileNameAsync(SelectedKind, ChangeToText, SearchText, ReplaceText, true);
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
                List<OldAndNewNameModel> extensionNameList = await GetNeedConvertFileListAsync([.. openFileDialog.FileNames]);
                if (extensionNameList is not null && extensionNameList.Count > 0)
                {
                    openFileDialog.Dispose();
                    AddToExtensionNamePage(extensionNameList);
                }
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
                if (!string.IsNullOrEmpty(openFolderDialog.SelectedPath))
                {
                    List<OldAndNewNameModel> fileNameList = await GetFileAsync(openFolderDialog.SelectedPath);

                    if (fileNameList is not null && fileNameList.Count > 0)
                    {
                        AddToExtensionNamePage(fileNameList);
                    }
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
            if (sender is Microsoft.UI.Xaml.Controls.CheckBox checkBox && checkBox.Tag is ExtensionNameSelectedKind extensionNameSelectedKind)
            {
                if (Equals(SelectedKind, extensionNameSelectedKind))
                {
                    SelectedKind = ExtensionNameSelectedKind.None;
                }

                if (extensionNameSelectedKind is ExtensionNameSelectedKind.IsSameExtensionName)
                {
                    ChangeToText = string.Empty;
                }
                else if (extensionNameSelectedKind is ExtensionNameSelectedKind.IsFindAndReplaceExtensionName)
                {
                    SearchText = string.Empty;
                    ReplaceText = string.Empty;
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

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            SelectedKind = ExtensionNameSelectedKind.None;
        }

        /// <summary>
        /// 添加到扩展名称页面
        /// </summary>
        internal void AddToExtensionNamePage(List<OldAndNewNameModel> extensionNameList)
        {
            lock (extensionNameLock)
            {
                foreach (OldAndNewNameModel oldAndNewNameItem in extensionNameList)
                {
                    ExtensionNameCollection.Add(oldAndNewNameItem);
                }
            }
        }

        /// <summary>
        /// 准备修改文件名称
        /// </summary>
        private async Task PrepareChangeFileNameAsync(ExtensionNameSelectedKind selectedKind, string changeToText, string searchText, string replaceText, bool needChange)
        {
            if (selectedKind is not ExtensionNameSelectedKind.None)
            {
                IsOperationFailed = false;
                OperationFailedList.Clear();
                int count = 0;

                lock (extensionNameLock)
                {
                    count = ExtensionNameCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName(selectedKind, changeToText, searchText, replaceText);
                    if (needChange)
                    {
                        await ChangeFileNameAsync();
                    }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NoOperation));
            }
        }

        /// <summary>
        /// 预览修改后的文件名称
        /// </summary>
        private void PreviewChangedFileName(ExtensionNameSelectedKind selectedKind, string changeToText, string searchText, string replaceText)
        {
            switch (selectedKind)
            {
                case ExtensionNameSelectedKind.IsSameExtensionName:
                    {
                        lock (extensionNameLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in ExtensionNameCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                    if (!string.IsNullOrEmpty(changeToText))
                                    {
                                        oldAndNewNameItem.NewFileName = fileName + "." + changeToText;
                                    }
                                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                                }
                            }
                        }
                        break;
                    }
                case ExtensionNameSelectedKind.IsFindAndReplaceExtensionName:
                    {
                        lock (extensionNameLock)
                        {
                            foreach (OldAndNewNameModel oldAndNewNameItem in ExtensionNameCollection)
                            {
                                if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName))
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(oldAndNewNameItem.OriginalFileName);
                                    string extensionName = Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                                    if (extensionName.Contains(searchText) && !string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(replaceText))
                                    {
                                        extensionName = extensionName.Replace(searchText, replaceText);
                                    }
                                    oldAndNewNameItem.NewFileName = fileName + extensionName;
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
            lock (extensionNameLock)
            {
                foreach (OldAndNewNameModel oldAndNewName in ExtensionNameCollection)
                {
                    oldAndNewName.IsModifyingNow = true;
                }
            }

            List<OperationFailedModel> operationFailedList = await GetOperationFailedListAsync();

            IsModifyingNow = false;
            lock (extensionNameLock)
            {
                foreach (OldAndNewNameModel oldAndNewName in ExtensionNameCollection)
                {
                    oldAndNewName.IsModifyingNow = false;
                }
            }
            if (operationFailedList is not null && operationFailedList.Count > 0)
            {
                foreach (OperationFailedModel operationFailedItem in operationFailedList)
                {
                    OperationFailedList.Add(operationFailedItem);
                }
            }

            int count = ExtensionNameCollection.Count;
            IsOperationFailed = OperationFailedList.Count is not 0;

            lock (extensionNameLock)
            {
                ExtensionNameCollection.Clear();
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(GetDragDropSelectedFilesAsync), 1, e);
                }

                return fileList;
            });
        }

        /// <summary>
        /// 获取要待转换的文件列表
        /// </summary>
        private async Task<List<OldAndNewNameModel>> GetNeedConvertFileListAsync(List<string> fileList)
        {
            if (fileList is null || fileList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<OldAndNewNameModel> extensionNameList = [];

                foreach (string file in fileList)
                {
                    try
                    {
                        FileInfo fileInfo = new(file);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        extensionNameList.Add(new()
                        {
                            OriginalFileName = Path.GetFileName(file),
                            OriginalFilePath = file,
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(GetNeedConvertFileListAsync), 1, e);
                        continue;
                    }
                }

                return extensionNameList;
            });
        }

        /// <summary>
        /// 获取文件夹的所有子文件夹和所有文件
        /// </summary>
        private async Task<List<OldAndNewNameModel>> GetFileAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return [];
            }

            return await Task.Run(() =>
            {
                List<OldAndNewNameModel> fileNameList = [];
                DirectoryInfo currentFolder = new(folderPath);

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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(GetFileAsync), 1, e);
                }
                return fileNameList;
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

                lock (extensionNameLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in ExtensionNameCollection)
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
                                    operationFailedList.Add(new()
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
                                    operationFailedList.Add(new()
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
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
