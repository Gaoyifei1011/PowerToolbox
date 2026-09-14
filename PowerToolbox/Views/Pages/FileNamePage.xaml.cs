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

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件名称页面
    /// </summary>
    internal sealed partial class FileNamePage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AutoString = ResourceService.FileNameResource.GetString("Auto");
        private readonly string DragOverContentString = ResourceService.FileNameResource.GetString("DragOverContent");
        private readonly string ModifyingNowString = ResourceService.FileNameResource.GetString("ModifyingNow");
        private readonly string SelectFileString = ResourceService.FileNameResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileNameResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileNameResource.GetString("Total");
        private readonly object fileNameLock = new();

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isChecked;

        private bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsChecked)));
                }
            }
        }

        private bool _isModifyingNow;

        internal bool IsModifyingNow
        {
            get { return _isModifyingNow; }

            set
            {
                if (!Equals(_isModifyingNow, value))
                {
                    _isModifyingNow = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifyingNow)));
                }
            }
        }

        private string _renameRule;

        private string RenameRule
        {
            get { return _renameRule; }

            set
            {
                if (!string.Equals(_renameRule, value))
                {
                    _renameRule = value;
                    PropertyChanged?.Invoke(this, new(nameof(RenameRule)));
                }
            }
        }

        private string _startNumber;

        private string StartNumber
        {
            get { return _startNumber; }

            set
            {
                if (!string.Equals(_startNumber, value))
                {
                    _startNumber = value;
                    PropertyChanged?.Invoke(this, new(nameof(StartNumber)));
                }
            }
        }

        private string _extensionName;

        private string ExtensionName
        {
            get { return _extensionName; }

            set
            {
                if (!string.Equals(_extensionName, value))
                {
                    _extensionName = value;
                    PropertyChanged?.Invoke(this, new(nameof(ExtensionName)));
                }
            }
        }

        private string _lookUpText;

        private string LookUpText
        {
            get { return _lookUpText; }

            set
            {
                if (!string.Equals(_lookUpText, value))
                {
                    _lookUpText = value;
                    PropertyChanged?.Invoke(this, new(nameof(LookUpText)));
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

        private ComboBoxItemModel _selectedNumberFormat;

        private ComboBoxItemModel SelectedNumberFormat
        {
            get { return _selectedNumberFormat; }

            set
            {
                if (!Equals(_selectedNumberFormat, value))
                {
                    _selectedNumberFormat = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedNumberFormat)));
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

        private List<ComboBoxItemModel> NumberFormatList { get; } = [];

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        private WinRTObservableCollection<OldAndNewNameModel> FileNameCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal FileNamePage()
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
                    List<OldAndNewNameModel> fileNameList = await GetNeedConvertFileListAsync(fileList);
                    if (fileNameList is not null && fileNameList.Count > 0)
                    {
                        AddToFileNamePage(fileNameList);
                        IsOperationFailed = false;
                        OperationFailedList.Clear();
                    }
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
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
            {
                lock (fileNameLock)
                {
                    FileNameCollection.Remove(oldAndNewName);
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
                lock (fileNameLock)
                {
                    int index = FileNameCollection.IndexOf(oldAndNewName);

                    if (index >= 0 && index < FileNameCollection.Count - 1)
                    {
                        OldAndNewNameModel upOldAndNewName = FileNameCollection[index];
                        OldAndNewNameModel downOldAndNewName = FileNameCollection[index + 1];
                        FileNameCollection[index] = downOldAndNewName;
                        FileNameCollection[index + 1] = upOldAndNewName;
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
                lock (fileNameLock)
                {
                    int index = FileNameCollection.IndexOf(oldAndNewName);

                    if (index > 0)
                    {
                        OldAndNewNameModel upOldAndNewName = FileNameCollection[index - 1];
                        OldAndNewNameModel downOldAndNewName = FileNameCollection[index];
                        FileNameCollection[index - 1] = downOldAndNewName;
                        FileNameCollection[index] = upOldAndNewName;
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
                    await PrepareChangeFileNameAsync(RenameRule, StartNumber, IsChecked, LookUpText, ReplaceText, ExtensionName, false);
                }
                else if (sender.Modifiers is VirtualKeyModifiers.Control)
                {
                    await PrepareChangeFileNameAsync(RenameRule, StartNumber, IsChecked, LookUpText, ReplaceText, ExtensionName, true);
                }
                args.Handled = true;
            }
        }

        /// <summary>
        /// 改名规则文本框中的内容发生更改时发生的事件
        /// </summary>
        private void OnRenameRuleTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                RenameRule = textBox.Text;
            }
        }

        /// <summary>
        /// 起始编号文本框中的内容发生更改时发生的事件
        /// </summary>
        private void OnStartNumberTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                StartNumber = textBox.Text;
            }
        }

        /// <summary>
        /// 扩展名文本框中的内容发生更改时发生的事件
        /// </summary>
        private void OnExtensionNameTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                ExtensionName = textBox.Text;
            }
        }

        /// <summary>
        /// 查找文本框中的内容发生更改时发生的事件
        /// </summary>
        private void OnLookUpTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                LookUpText = textBox.Text;
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
            lock (fileNameLock)
            {
                FileNameCollection.Clear();
                IsOperationFailed = false;
                OperationFailedList.Clear();
            }
        }

        /// <summary>
        /// 选择编号格式
        /// </summary>
        private void OnNumberFormatSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedNumberFormat, comboBox.SelectedItem))
            {
                SelectedNumberFormat = comboBox.SelectedItem is ComboBoxItemModel numberFormat ? numberFormat : null;
            }
        }

        /// <summary>
        /// 查看改名规则
        /// </summary>
        private void OnViewNameChangeExampleClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is FileManagerPage fileManagerPage)
            {
                fileManagerPage.ShowUseInstruction();
            }
        }

        /// <summary>
        /// 预览修改的内容
        /// </summary>
        private async void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            await PrepareChangeFileNameAsync(RenameRule, StartNumber, IsChecked, LookUpText, ReplaceText, ExtensionName, false);
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private async void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            await PrepareChangeFileNameAsync(RenameRule, StartNumber, IsChecked, LookUpText, ReplaceText, ExtensionName, true);
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
                List<OldAndNewNameModel> fileNameList = await GetNeedConvertFileListAsync([.. openFileDialog.FileNames]);
                if (fileNameList is not null && fileNameList.Count > 0)
                {
                    AddToFileNamePage(fileNameList);
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
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                IsOperationFailed = false;
                OperationFailedList.Clear();
                if (!string.IsNullOrEmpty(openFolderDialog.SelectedPath))
                {
                    (List<OldAndNewNameModel> directoryNameList, List<OldAndNewNameModel> fileNameList) = await GetFileAndDirectoryAsync(openFolderDialog.SelectedPath);

                    if (directoryNameList is not null && directoryNameList.Count > 0)
                    {
                        AddToFileNamePage(directoryNameList);
                    }

                    if (fileNameList is not null && fileNameList.Count > 0)
                    {
                        AddToFileNamePage(fileNameList);
                    }
                }
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 取消选中时触发的事件
        /// </summary>
        private void OnUnchecked(object sender, RoutedEventArgs args)
        {
            ExtensionName = string.Empty;
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedList));
        }

        #endregion 第六部分：挂载事件处理

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            RenameRule = "<#>";
            StartNumber = "1";
            LookUpText = string.Empty;
            ReplaceText = string.Empty;

            NumberFormatList.Add(new() { SelectedValue = "Auto", DisplayMember = AutoString });
            NumberFormatList.Add(new() { SelectedValue = "0", DisplayMember = "0" });
            NumberFormatList.Add(new() { SelectedValue = "00", DisplayMember = "00" });
            NumberFormatList.Add(new() { SelectedValue = "000", DisplayMember = "000" });
            NumberFormatList.Add(new() { SelectedValue = "0000", DisplayMember = "0000" });
            NumberFormatList.Add(new() { SelectedValue = "00000", DisplayMember = "00000" });
            NumberFormatList.Add(new() { SelectedValue = "000000", DisplayMember = "000000" });
            NumberFormatList.Add(new() { SelectedValue = "0000000", DisplayMember = "0000000" });
            SelectedNumberFormat = NumberFormatList[0];
        }

        /// <summary>
        /// 添加到文件名称页面
        /// </summary>
        internal void AddToFileNamePage(List<OldAndNewNameModel> filenameList)
        {
            lock (fileNameLock)
            {
                foreach (OldAndNewNameModel oldAndNewNameItem in filenameList)
                {
                    FileNameCollection.Add(oldAndNewNameItem);
                }
            }
        }

        /// <summary>
        /// 准备修改文件名称
        /// </summary>
        private async Task PrepareChangeFileNameAsync(string renameRule, string startNumber, bool isChecked, string lookUpText, string replaceText, string extensionName, bool needChange)
        {
            if (!string.IsNullOrEmpty(renameRule) || !string.IsNullOrEmpty(startNumber) || isChecked || !string.IsNullOrEmpty(lookUpText) || !string.IsNullOrEmpty(replaceText))
            {
                IsOperationFailed = false;
                OperationFailedList.Clear();
                int count = 0;

                lock (fileNameLock)
                {
                    count = FileNameCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileName(startNumber, renameRule, isChecked, lookUpText, replaceText, extensionName);
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
        private void PreviewChangedFileName(string startNumber, string renameRule, bool isChecked, string lookUpText, string replaceText, string extensionName)
        {
            int startIndex = 0;
            if (!string.IsNullOrEmpty(startNumber))
            {
                int.TryParse(startNumber, out startIndex);
            }

            lock (fileNameLock)
            {
                int endIndex = FileNameCollection.Count + startIndex;
                int numberLength = Convert.ToString(endIndex).Length;

                foreach (OldAndNewNameModel oldAndNewNameItem in FileNameCollection)
                {
                    string tempNewFileName = oldAndNewNameItem.OriginalFileName;
                    // 根据改名规则替换
                    if (!string.IsNullOrEmpty(renameRule))
                    {
                        try
                        {
                            string tempFileName = renameRule;
                            if (tempFileName.Contains("<#>"))
                            {
                                string formattedIndex = string.Empty;
                                if (Equals(SelectedNumberFormat, NumberFormatList[0]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(numberLength, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[1]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(1, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[2]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(2, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[3]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(3, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[4]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(4, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[5]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(5, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[6]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(6, '0');
                                }
                                else if (Equals(SelectedNumberFormat, NumberFormatList[7]))
                                {
                                    formattedIndex = Convert.ToString(startIndex).PadLeft(7, '0');
                                }

                                tempFileName = tempFileName.Replace("<#>", formattedIndex);
                                startIndex++;
                            }
                            if (tempFileName.Contains("<$>"))
                            {
                                tempFileName = tempFileName.Replace("<$>", DateTimeOffset.Now.ToString("yyyy-MM-dd"));
                            }
                            if (tempFileName.Contains("<&>"))
                            {
                                tempFileName = tempFileName.Replace("<&>", oldAndNewNameItem.OriginalFileName);
                            }
                            if (tempFileName.Contains("<N>"))
                            {
                                if ((new FileInfo(oldAndNewNameItem.OriginalFilePath).Attributes & System.IO.FileAttributes.Directory) is not 0)
                                {
                                    DirectoryInfo directoryInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<N>", directoryInfo.LastWriteTime.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    FileInfo fileInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<N>", fileInfo.LastWriteTime.ToString("yyyy-MM-dd"));
                                }
                            }
                            if (tempFileName.Contains("<C>"))
                            {
                                if ((new FileInfo(oldAndNewNameItem.OriginalFilePath).Attributes & System.IO.FileAttributes.Directory) is not 0)
                                {
                                    DirectoryInfo directoryInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<C>", directoryInfo.CreationTime.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    FileInfo fileInfo = new(oldAndNewNameItem.OriginalFilePath);
                                    tempFileName = tempFileName.Replace("<C>", fileInfo.CreationTime.ToString("yyyy-MM-dd"));
                                }
                            }
                            tempNewFileName = tempFileName + Path.GetExtension(oldAndNewNameItem.OriginalFileName);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(PreviewChangedFileName), 1, e);
                            tempNewFileName = oldAndNewNameItem.OriginalFileName;
                        }
                    }

                    // 修改文件扩展名
                    if (isChecked)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(tempNewFileName);
                        if (!string.IsNullOrEmpty(extensionName))
                        {
                            tempNewFileName = fileName + extensionName;
                        }
                    }

                    // 查找并替换字符串
                    if (!string.IsNullOrEmpty(lookUpText) && tempNewFileName.Contains(lookUpText) && !string.IsNullOrEmpty(replaceText))
                    {
                        tempNewFileName = tempNewFileName.Replace(lookUpText, replaceText);
                    }

                    oldAndNewNameItem.NewFileName = tempNewFileName;
                    oldAndNewNameItem.NewFilePath = oldAndNewNameItem.OriginalFilePath.Replace(oldAndNewNameItem.OriginalFileName, oldAndNewNameItem.NewFileName);
                }
            }
        }

        /// <summary>
        /// 更改文件名称
        /// </summary>
        private async Task ChangeFileNameAsync()
        {
            IsModifyingNow = true;
            lock (fileNameLock)
            {
                foreach (OldAndNewNameModel oldAndNewName in FileNameCollection)
                {
                    oldAndNewName.IsModifyingNow = true;
                }
            }
            List<OperationFailedModel> operationFailedList = await GetOperationFailedListAsync();

            IsModifyingNow = false;
            lock (fileNameLock)
            {
                foreach (OldAndNewNameModel oldAndNewName in FileNameCollection)
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

            int count = FileNameCollection.Count;
            IsOperationFailed = OperationFailedList.Count is not 0;

            lock (fileNameLock)
            {
                FileNameCollection.Clear();
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(GetDragDropSelectedFilesAsync), 1, e);
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
                List<OldAndNewNameModel> fileNameList = [];

                foreach (string file in fileList)
                {
                    try
                    {
                        FileInfo fileInfo = new(file);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        fileNameList.Add(new()
                        {
                            OriginalFileName = Path.GetFileName(file),
                            OriginalFilePath = file,
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(GetNeedConvertFileListAsync), 1, e);
                        continue;
                    }
                }

                return fileNameList;
            });
        }

        /// <summary>
        /// 获取文件夹的所有子文件夹和所有文件
        /// </summary>
        private async Task<(List<OldAndNewNameModel>, List<OldAndNewNameModel>)> GetFileAndDirectoryAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return ValueTuple.Create<List<OldAndNewNameModel>, List<OldAndNewNameModel>>([], []);
            }

            return await Task.Run(() =>
            {
                List<OldAndNewNameModel> directoryNameList = [];
                List<OldAndNewNameModel> fileNameList = [];
                DirectoryInfo currentFolder = new(folderPath);

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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(GetFileAndDirectoryAsync), 1, e);
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(GetFileAndDirectoryAsync), 2, e);
                }
                return ValueTuple.Create(directoryNameList, fileNameList);
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

                lock (fileNameLock)
                {
                    foreach (OldAndNewNameModel oldAndNewNameItem in FileNameCollection)
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
    }
}
