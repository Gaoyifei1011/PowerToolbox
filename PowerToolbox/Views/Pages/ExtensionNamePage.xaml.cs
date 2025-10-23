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
    public sealed partial class ExtensionNamePage : Page, INotifyPropertyChanged
    {
        private readonly string DragOverContentString = ResourceService.ExtensionNameResource.GetString("DragOverContent");
        private readonly string ModifyingNowString = ResourceService.ExtensionNameResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.ExtensionNameResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.ExtensionNameResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.ExtensionNameResource.GetString("Total");
        private readonly object extensionNameLock = new();

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

        private ExtensionNameSelectedKind _selectedKind = ExtensionNameSelectedKind.None;

        public ExtensionNameSelectedKind SelectedKind
        {
            get { return _selectedKind; }

            set
            {
                if (!Equals(_selectedKind, value))
                {
                    _selectedKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedKind)));
                }
            }
        }

        private string _changeToText;

        public string ChangeToText
        {
            get { return _changeToText; }

            set
            {
                if (!string.Equals(_changeToText, value))
                {
                    _changeToText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeToText)));
                }
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private string _replaceText;

        public string ReplaceText
        {
            get { return _replaceText; }

            set
            {
                if (!string.Equals(_replaceText, value))
                {
                    _replaceText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplaceText)));
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

        public event PropertyChangedEventHandler PropertyChanged;

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        private WinRTObservableCollection<OldAndNewNameModel> ExtensionNameCollection { get; } = [];

        public ExtensionNamePage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            List<OldAndNewNameModel> extensionNameList = await Task.Run(() =>
            {
                List<OldAndNewNameModel> extensionNameList = [];

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
                            extensionNameList.Add(new()
                            {
                                OriginalFileName = storageItem.Name,
                                OriginalFilePath = storageItem.Path
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(OnDrop), 2, e);
                        continue;
                    }
                }

                return extensionNameList;
            });

            AddToExtensionNamePage(extensionNameList);
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
                ExtensionNameCollection.Remove(oldAndNewName);
            }
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        private void OnMoveDownExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
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

        /// <summary>
        /// 向上移动
        /// </summary>
        private void OnMoveUpExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
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

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：扩展名称页面——挂载的事件

        /// <summary>
        /// 当文本框中的内容发生更改时发生的事件。
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox && textBox.Tag is string tag)
            {
                if (tag is "ChangeToText")
                {
                    ChangeToText = textBox.Text;
                }
                else if (tag is "SearchText")
                {
                    SearchText = textBox.Text;
                }
                else if (tag is "ReplaceText")
                {
                    ReplaceText = textBox.Text;
                }
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
            bool checkResult = CheckOperationState();
            if (checkResult)
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
                List<OldAndNewNameModel> extensionNameList = await Task.Run(() =>
                {
                    List<OldAndNewNameModel> extensionNameList = [];

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
                                extensionNameList.Add(new()
                                {
                                    OriginalFileName = fileInfo.Name,
                                    OriginalFilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(OnSelectFileClicked), 1, e);
                            continue;
                        }
                    }

                    return extensionNameList;
                });

                openFileDialog.Dispose();
                AddToExtensionNamePage(extensionNameList);
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
                    List<OldAndNewNameModel> fileNameList = [];

                    await Task.Run(() =>
                    {
                        DirectoryInfo currentFolder = new(openFolderDialog.SelectedPath);

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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ExtensionNamePage), nameof(OnSelectFolderClicked), 1, e);
                        }
                    });

                    AddToExtensionNamePage(fileNameList);
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

        #endregion 第三部分：扩展名称页面——挂载的事件

        /// <summary>
        /// 添加到扩展名称页面
        /// </summary>
        public void AddToExtensionNamePage(List<OldAndNewNameModel> extensionNameList)
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
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            return SelectedKind is not ExtensionNameSelectedKind.None;
        }

        /// <summary>
        /// 预览修改后的文件名称
        /// </summary>
        private void PreviewChangedFileName()
        {
            switch (SelectedKind)
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
                                    oldAndNewNameItem.NewFileName = fileName + "." + ChangeToText;
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
                                    if (extensionName.Contains(SearchText))
                                    {
                                        extensionName = extensionName.Replace(SearchText, ReplaceText);
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
            foreach (OldAndNewNameModel oldAndNewName in ExtensionNameCollection)
            {
                oldAndNewName.IsModifyingNow = true;
            }

            List<OperationFailedModel> operationFailedList = await Task.Run(() =>
            {
                List<OperationFailedModel> operationFailedList = [];

                lock (extensionNameLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in ExtensionNameCollection)
                    {
                        if (!string.IsNullOrEmpty(oldAndNewNameItem.OriginalFileName) && !string.IsNullOrEmpty(oldAndNewNameItem.OriginalFilePath))
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

                return operationFailedList;
            });

            IsModifyingNow = false;
            foreach (OldAndNewNameModel oldAndNewName in ExtensionNameCollection)
            {
                oldAndNewName.IsModifyingNow = false;
            }
            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedList.Add(operationFailedItem);
            }

            IsOperationFailed = OperationFailedList.Count is not 0;
            int count = ExtensionNameCollection.Count;

            lock (extensionNameLock)
            {
                ExtensionNameCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.File, count - OperationFailedList.Count, OperationFailedList.Count));
        }
    }
}
