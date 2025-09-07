using Microsoft.UI.Xaml.Controls;
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

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 文件名称页面
    /// </summary>
    public sealed partial class FileNamePage : Page, INotifyPropertyChanged
    {
        private readonly string AutoString = ResourceService.FileNameResource.GetString("Auto");
        private readonly string DragOverContentString = ResourceService.FileNameResource.GetString("DragOverContent");
        private readonly string SelectFileString = ResourceService.FileNameResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FileNameResource.GetString("SelectFolder");
        private readonly string TotalString = ResourceService.FileNameResource.GetString("Total");
        private readonly object fileNameLock = new();

        private bool _isChecked = false;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

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

        private string _renameRule = "<#>";

        public string RenameRule
        {
            get { return _renameRule; }

            set
            {
                if (!string.Equals(_renameRule, value))
                {
                    _renameRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenameRule)));
                }
            }
        }

        private string _startNumber = "1";

        public string StartNumber
        {
            get { return _startNumber; }

            set
            {
                if (!string.Equals(_startNumber, value))
                {
                    _startNumber = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartNumber)));
                }
            }
        }

        private string _extensionName;

        public string ExtensionName
        {
            get { return _extensionName; }

            set
            {
                if (!string.Equals(_extensionName, value))
                {
                    _extensionName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExtensionName)));
                }
            }
        }

        private string _lookUpString = string.Empty;

        public string LookUpString
        {
            get { return _lookUpString; }

            set
            {
                if (!string.Equals(_lookUpString, value))
                {
                    _lookUpString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LookUpString)));
                }
            }
        }

        private string _replaceString = string.Empty;

        public string ReplaceString
        {
            get { return _replaceString; }

            set
            {
                if (!string.Equals(_replaceString, value))
                {
                    _replaceString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplaceString)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedNumberFormat;

        public KeyValuePair<string, string> SelectedNumberFormat
        {
            get { return _selectedNumberFormat; }

            set
            {
                if (!Equals(_selectedNumberFormat, value))
                {
                    _selectedNumberFormat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNumberFormat)));
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

        private List<KeyValuePair<string, string>> NumberFormatList { get; } = [];

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        private ObservableCollection<OldAndNewNameModel> FileNameCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FileNamePage()
        {
            InitializeComponent();
            NumberFormatList.Add(new KeyValuePair<string, string>("Auto", AutoString));
            NumberFormatList.Add(new KeyValuePair<string, string>("0", "0"));
            NumberFormatList.Add(new KeyValuePair<string, string>("00", "00"));
            NumberFormatList.Add(new KeyValuePair<string, string>("000", "000"));
            NumberFormatList.Add(new KeyValuePair<string, string>("0000", "0000"));
            NumberFormatList.Add(new KeyValuePair<string, string>("00000", "00000"));
            NumberFormatList.Add(new KeyValuePair<string, string>("000000", "000000"));
            NumberFormatList.Add(new KeyValuePair<string, string>("0000000", "0000000"));
            SelectedNumberFormat = NumberFormatList[0];
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override void OnDragOver(global::Windows.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);
            args.AcceptedOperation = DataPackageOperation.Copy;
            args.DragUIOverride.IsCaptionVisible = true;
            args.DragUIOverride.IsContentVisible = false;
            args.DragUIOverride.IsGlyphVisible = true;
            args.DragUIOverride.Caption = DragOverContentString;
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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            List<OldAndNewNameModel> fileNameList = await Task.Run(() =>
            {
                List<OldAndNewNameModel> fileNameList = [];

                foreach (IStorageItem storageItem in storageItemList)
                {
                    try
                    {
                        FileInfo fileInfo = new(storageItem.Path);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        fileNameList.Add(new()
                        {
                            OriginalFileName = storageItem.Name,
                            OriginalFilePath = storageItem.Path,
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(OnDrop), 2, e);
                        continue;
                    }
                }

                return fileNameList;
            });

            AddToFileNamePage(fileNameList);
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
        /// 向下移动
        /// </summary>
        private void OnMoveDownExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
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

        /// <summary>
        /// 向上移动
        /// </summary>
        private void OnMoveUpExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewNameModel oldAndNewName)
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

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：文件名称页面——挂载的事件

        /// <summary>
        /// 当文本框中的内容发生更改时发生的事件。
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.TextBox textBox && textBox.Tag is string tag)
            {
                if (tag is "RenameRule")
                {
                    RenameRule = textBox.Text;
                }
                else if (tag is "StartNumber")
                {
                    StartNumber = textBox.Text;
                }
                else if (tag is "ExtensionName")
                {
                    ExtensionName = textBox.Text;
                }
                else if (tag is "LookUpString")
                {
                    LookUpString = textBox.Text;
                }
                else if (tag is "ReplaceString")
                {
                    ReplaceString = textBox.Text;
                }
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
        private void OnNumberFormatClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> numberFormat)
            {
                SelectedNumberFormat = numberFormat;
            }
        }

        /// <summary>
        /// 查看改名规则
        /// </summary>
        private void OnViewNameChangeExampleClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.Content is MainPage mainPage && mainPage.GetFrameContent() is FileManagerPage fileManagerPage)
            {
                fileManagerPage.ShowUseInstruction();
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
                List<OldAndNewNameModel> fileNameList = await Task.Run(() =>
                {
                    List<OldAndNewNameModel> fileNameList = [];

                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
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
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(OnSelectFileClicked), 1, e);
                            continue;
                        }
                    }

                    return fileNameList;
                });

                openFileDialog.Dispose();
                AddToFileNamePage(fileNameList);
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(OnSelectFolderClicked), 1, e);
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(OnSelectFolderClicked), 2, e);
                        }
                    });

                    openFolderDialog.Dispose();
                    AddToFileNamePage(directoryNameList);
                    AddToFileNamePage(fileNameList);
                }
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
            ExtensionName = string.Empty;
        }

        /// <summary>
        /// 查看修改失败的文件错误信息
        /// </summary>
        private async void OnViewErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new OperationFailedDialog(OperationFailedList));
        }

        #endregion 第三部分：文件名称页面——挂载的事件

        /// <summary>
        /// 添加到文件名称页面
        /// </summary>
        public void AddToFileNamePage(List<OldAndNewNameModel> filenameList)
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
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            return !string.IsNullOrEmpty(RenameRule) || !string.IsNullOrEmpty(StartNumber) || IsChecked || !string.IsNullOrEmpty(LookUpString) || !string.IsNullOrEmpty(ReplaceString);
        }

        /// <summary>
        /// 预览修改后的文件名称
        /// </summary>
        private void PreviewChangedFileName()
        {
            int startIndex = 0;
            if (!string.IsNullOrEmpty(StartNumber))
            {
                int.TryParse(StartNumber, out startIndex);
            }

            lock (fileNameLock)
            {
                int endIndex = FileNameCollection.Count + startIndex;
                int numberLength = Convert.ToString(endIndex).Length;

                foreach (OldAndNewNameModel oldAndNewNameItem in FileNameCollection)
                {
                    string tempNewFileName = oldAndNewNameItem.OriginalFileName;
                    // 根据改名规则替换
                    if (!string.IsNullOrEmpty(RenameRule))
                    {
                        try
                        {
                            string tempFileName = RenameRule;
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
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FileNamePage), nameof(PreviewChangedFileName), 1, e);
                            tempNewFileName = oldAndNewNameItem.OriginalFileName;
                        }
                    }

                    // 修改文件扩展名
                    if (IsChecked)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(tempNewFileName);
                        tempNewFileName = fileName + ExtensionName;
                    }

                    // 查找并替换字符串
                    if (!string.IsNullOrEmpty(LookUpString) && tempNewFileName.Contains(LookUpString))
                    {
                        tempNewFileName = tempNewFileName.Replace(LookUpString, ReplaceString);
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
            List<OperationFailedModel> operationFailedList = await Task.Run(() =>
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
            foreach (OperationFailedModel operationFailedItem in operationFailedList)
            {
                OperationFailedList.Add(operationFailedItem);
            }

            IsOperationFailed = OperationFailedList.Count is not 0;
            int count = FileNameCollection.Count;

            lock (fileNameLock)
            {
                FileNameCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.File, count - OperationFailedList.Count, OperationFailedList.Count));
        }
    }
}
