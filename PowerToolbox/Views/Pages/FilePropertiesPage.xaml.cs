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
using System.Text;
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
    /// 文件属性页面
    /// </summary>
    public sealed partial class FilePropertiesPage : Page, INotifyPropertyChanged
    {
        private readonly string ArchiveString = ResourceService.FilePropertiesResource.GetString("Archive");
        private readonly string CreateDateString = ResourceService.FilePropertiesResource.GetString("CreateDate");
        private readonly string DragOverContentString = ResourceService.FilePropertiesResource.GetString("DragOverContent");
        private readonly string HideString = ResourceService.FilePropertiesResource.GetString("Hide");
        private readonly string ModifyDateString = ResourceService.FilePropertiesResource.GetString("ModifyDate");
        private readonly string ReadOnlyString = ResourceService.FilePropertiesResource.GetString("ReadOnly");
        private readonly string SelectFileString = ResourceService.FilePropertiesResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FilePropertiesResource.GetString("SelectFolder");
        private readonly string SystemString = ResourceService.FilePropertiesResource.GetString("System");
        private readonly string TotalString = ResourceService.FilePropertiesResource.GetString("Total");
        private readonly object filePropertiesLock = new();

        private bool _isReadOnlyChecked = false;

        public bool IsReadOnlyChecked
        {
            get { return _isReadOnlyChecked; }

            set
            {
                if (!Equals(_isReadOnlyChecked, value))
                {
                    _isReadOnlyChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReadOnlyChecked)));
                }
            }
        }

        private bool _isArchiveChecked = false;

        public bool IsArchiveChecked
        {
            get { return _isArchiveChecked; }

            set
            {
                if (!Equals(_isArchiveChecked, value))
                {
                    _isArchiveChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsArchiveChecked)));
                }
            }
        }

        private bool _isCreateDateChecked = false;

        public bool IsCreateDateChecked
        {
            get { return _isCreateDateChecked; }

            set
            {
                if (!Equals(_isCreateDateChecked, value))
                {
                    _isCreateDateChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCreateDateChecked)));
                }
            }
        }

        private bool _isHideChecked = false;

        public bool IsHideChecked
        {
            get { return _isHideChecked; }

            set
            {
                if (!Equals(_isHideChecked, value))
                {
                    _isHideChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHideChecked)));
                }
            }
        }

        private bool _isSystemChecked = false;

        public bool IsSystemChecked
        {
            get { return _isSystemChecked; }

            set
            {
                if (!Equals(_isSystemChecked, value))
                {
                    _isSystemChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemChecked)));
                }
            }
        }

        private bool _isModifyDateChecked = false;

        public bool IsModifyDateChecked
        {
            get { return _isModifyDateChecked; }

            set
            {
                if (!Equals(_isModifyDateChecked, value))
                {
                    _isModifyDateChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModifyDateChecked)));
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

        private DateTimeOffset _createDate = DateTimeOffset.Now;

        public DateTimeOffset CreateDate
        {
            get { return _createDate; }

            set
            {
                if (!Equals(_createDate, value))
                {
                    _createDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreateDate)));
                }
            }
        }

        private TimeSpan _createTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan CreateTime
        {
            get { return _createTime; }

            set
            {
                if (!Equals(_createTime, value))
                {
                    _createTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreateTime)));
                }
            }
        }

        private DateTimeOffset _modifyDate = DateTimeOffset.Now;

        public DateTimeOffset ModifyDate
        {
            get { return _modifyDate; }

            set
            {
                if (!Equals(_modifyDate, value))
                {
                    _modifyDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyDate)));
                }
            }
        }

        private TimeSpan _modifyTime = DateTimeOffset.Now.TimeOfDay;

        public TimeSpan ModifyTime
        {
            get { return _modifyTime; }

            set
            {
                if (!Equals(_modifyTime, value))
                {
                    _modifyTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModifyTime)));
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

        private ObservableCollection<OldAndNewPropertiesModel> FilePropertiesCollection { get; } = [];

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public FilePropertiesPage()
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
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            List<OldAndNewPropertiesModel> filePropertiesList = await Task.Run(() =>
            {
                List<OldAndNewPropertiesModel> filePropertiesList = [];

                foreach (IStorageItem storageItem in storageItemList)
                {
                    try
                    {
                        FileInfo fileInfo = new(storageItem.Path);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        filePropertiesList.Add(new OldAndNewPropertiesModel()
                        {
                            FileName = storageItem.Name,
                            FilePath = storageItem.Path,
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(OnDrop), 2, e);
                        continue;
                    }
                }

                return filePropertiesList;
            });

            AddToFilePropertiesPage(filePropertiesList);
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

                    lock (filePropertiesLock)
                    {
                        count = FilePropertiesCollection.Count;
                    }

                    if (count is 0)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileAttributes();
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

                    lock (filePropertiesLock)
                    {
                        count = FilePropertiesCollection.Count;
                    }

                    if (count is 0)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                    }
                    else
                    {
                        PreviewChangedFileAttributes();
                        await ChangeFileAttributesAsync();
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
            if (args.Parameter is OldAndNewPropertiesModel oldAndNewProperties)
            {
                int index = FilePropertiesCollection.IndexOf(oldAndNewProperties);

                if (index >= 0 && index < FilePropertiesCollection.Count - 1)
                {
                    OldAndNewPropertiesModel upOldAndNewProperties = FilePropertiesCollection[index];
                    OldAndNewPropertiesModel downOldAndNewProperties = FilePropertiesCollection[index + 1];
                    FilePropertiesCollection[index] = downOldAndNewProperties;
                    FilePropertiesCollection[index + 1] = upOldAndNewProperties;
                }
            }
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        private void OnMoveUpExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewPropertiesModel oldAndNewProperties)
            {
                int index = FilePropertiesCollection.IndexOf(oldAndNewProperties);

                if (index > 0)
                {
                    OldAndNewPropertiesModel upOldAndNewProperties = FilePropertiesCollection[index - 1];
                    OldAndNewPropertiesModel downOldAndNewProperties = FilePropertiesCollection[index];
                    FilePropertiesCollection[index - 1] = downOldAndNewProperties;
                    FilePropertiesCollection[index] = upOldAndNewProperties;
                }
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：文件属性页面——挂载的事件

        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClearListClicked(object sender, RoutedEventArgs args)
        {
            lock (filePropertiesLock)
            {
                IsOperationFailed = false;
                FilePropertiesCollection.Clear();
                OperationFailedList.Clear();
            }
        }

        /// <summary>
        /// 日期更改时触发的事件
        /// </summary>
        private void OnDateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            if (sender is DatePicker datePicker && datePicker.Tag is string tag)
            {
                if (string.Equals(tag, nameof(CreateDate)))
                {
                    CreateDate = args.NewDate;
                }
                else if (string.Equals(tag, nameof(ModifyDate)))
                {
                    ModifyDate = args.NewDate;
                }
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

                lock (filePropertiesLock)
                {
                    count = FilePropertiesCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileAttributes();
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

                lock (filePropertiesLock)
                {
                    count = FilePropertiesCollection.Count;
                }

                if (count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ListEmpty));
                }
                else
                {
                    PreviewChangedFileAttributes();
                    await ChangeFileAttributesAsync();
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
                List<OldAndNewPropertiesModel> filePropertiesList = await Task.Run(() =>
                {
                    List<OldAndNewPropertiesModel> filePropertiesList = [];

                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            FileInfo fileInfo = new(fileName);
                            if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                            {
                                continue;
                            }

                            filePropertiesList.Add(new OldAndNewPropertiesModel()
                            {
                                FileName = fileInfo.Name,
                                FilePath = fileInfo.FullName,
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(OnSelectFileClicked), 1, e);
                            continue;
                        }
                    }

                    return filePropertiesList;
                });

                openFileDialog.Dispose();
                AddToFilePropertiesPage(filePropertiesList);
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
                    List<OldAndNewPropertiesModel> directoryNameList = [];
                    List<OldAndNewPropertiesModel> fileNameList = [];

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

                                directoryNameList.Add(new OldAndNewPropertiesModel()
                                {
                                    FileName = directoryInfo.Name,
                                    FilePath = directoryInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(OnSelectFolderClicked), 1, e);
                        }

                        try
                        {
                            foreach (FileInfo fileInfo in currentFolder.GetFiles())
                            {
                                if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                fileNameList.Add(new OldAndNewPropertiesModel()
                                {
                                    FileName = fileInfo.Name,
                                    FilePath = fileInfo.FullName
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(OnSelectFolderClicked), 2, e);
                        }
                    });

                    openFolderDialog.Dispose();
                    AddToFilePropertiesPage(directoryNameList);
                    AddToFilePropertiesPage(fileNameList);
                }
            }
            else
            {
                openFolderDialog.Dispose();
            }
        }

        /// <summary>
        /// 时间更改时触发的事件
        /// </summary>
        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker timePicker && timePicker.Tag is string tag)
            {
                if (string.Equals(tag, nameof(CreateTime)))
                {
                    CreateTime = args.NewTime;
                }
                else if (string.Equals(tag, nameof(ModifyTime)))
                {
                    ModifyTime = args.NewTime;
                }
            }
        }

        /// <summary>
        /// 取消选中时触发的事件
        /// </summary>
        private void OnUnchecked(object sender, RoutedEventArgs args)
        {
            if (sender is global::Windows.UI.Xaml.Controls.CheckBox checkBox && checkBox.Tag is string tag)
            {
                if (string.Equals(tag, nameof(CreateDate)))
                {
                    CreateDate = DateTimeOffset.Now;
                    CreateTime = DateTimeOffset.Now.TimeOfDay;
                }
                else if (string.Equals(tag, nameof(ModifyDate)))
                {
                    ModifyDate = DateTimeOffset.Now;
                    ModifyTime = DateTimeOffset.Now.TimeOfDay;
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

        #endregion 第三部分：文件属性页面——挂载的事件

        /// <summary>
        /// 添加到文件属性页面
        /// </summary>
        public void AddToFilePropertiesPage(List<OldAndNewPropertiesModel> filePropertiesList)
        {
            lock (filePropertiesLock)
            {
                foreach (OldAndNewPropertiesModel oldAndNewPropertiesItem in filePropertiesList)
                {
                    FilePropertiesCollection.Add(oldAndNewPropertiesItem);
                }
            }
        }

        /// <summary>
        /// 检查用户是否指定了操作过程
        /// </summary>
        private bool CheckOperationState()
        {
            return IsReadOnlyChecked || IsArchiveChecked || IsCreateDateChecked || IsHideChecked || IsSystemChecked || IsModifyDateChecked;
        }

        /// <summary>
        /// 修改文件属性
        /// </summary>
        private void PreviewChangedFileAttributes()
        {
            StringBuilder stringBuilder = new();
            if (IsReadOnlyChecked)
            {
                stringBuilder.Append(ReadOnlyString);
                stringBuilder.Append(' ');
            }
            if (IsArchiveChecked)
            {
                stringBuilder.Append(ArchiveString);
                stringBuilder.Append(' ');
            }
            if (IsHideChecked)
            {
                stringBuilder.Append(HideString);
                stringBuilder.Append(' ');
            }
            if (IsSystemChecked)
            {
                stringBuilder.Append(SystemString);
                stringBuilder.Append(' ');
            }
            if (IsCreateDateChecked)
            {
                stringBuilder.Append(CreateDateString);
                stringBuilder.Append(' ');
            }
            if (IsModifyDateChecked)
            {
                stringBuilder.Append(ModifyDateString);
                stringBuilder.Append(' ');
            }

            lock (filePropertiesLock)
            {
                foreach (OldAndNewPropertiesModel oldAndNewPropertiesItem in FilePropertiesCollection)
                {
                    oldAndNewPropertiesItem.FileProperties = Convert.ToString(stringBuilder);
                }
            }
        }

        /// <summary>
        /// 更改文件属性
        /// </summary>
        private async Task ChangeFileAttributesAsync()
        {
            IsModifyingNow = true;
            List<OperationFailedModel> operationFailedList = await Task.Run(() =>
            {
                List<OperationFailedModel> operationFailedList = [];

                lock (filePropertiesLock)
                {
                    foreach (OldAndNewPropertiesModel oldAndNewPropertiesItem in FilePropertiesCollection)
                    {
                        if (!string.IsNullOrEmpty(oldAndNewPropertiesItem.FileName) && !string.IsNullOrEmpty(oldAndNewPropertiesItem.FilePath))
                        {
                            try
                            {
                                System.IO.FileAttributes fileAttributes = File.GetAttributes(oldAndNewPropertiesItem.FilePath);
                                if (IsReadOnlyChecked) fileAttributes |= System.IO.FileAttributes.ReadOnly;
                                if (IsArchiveChecked) fileAttributes |= System.IO.FileAttributes.Archive;
                                if (IsHideChecked) fileAttributes |= System.IO.FileAttributes.Hidden;
                                if (IsSystemChecked) fileAttributes |= System.IO.FileAttributes.System;
                                File.SetAttributes(oldAndNewPropertiesItem.FilePath, fileAttributes);

                                if (IsCreateDateChecked)
                                {
                                    File.SetCreationTime(oldAndNewPropertiesItem.FilePath, CreateDate.Date + CreateTime);
                                }
                                if (IsModifyDateChecked)
                                {
                                    File.SetLastWriteTime(oldAndNewPropertiesItem.FilePath, ModifyDate.Date + ModifyTime);
                                }
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new OperationFailedModel()
                                {
                                    FileName = oldAndNewPropertiesItem.FileName,
                                    FilePath = oldAndNewPropertiesItem.FilePath,
                                    Exception = e
                                });
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
            int count = FilePropertiesCollection.Count;

            lock (filePropertiesLock)
            {
                FilePropertiesCollection.Clear();
            }

            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.File, count - OperationFailedList.Count, OperationFailedList.Count));
        }
    }
}
