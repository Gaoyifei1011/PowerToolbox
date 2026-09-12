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
using System.Text;
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
    /// 文件属性页面
    /// </summary>
    internal sealed partial class FilePropertiesPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string ArchiveString = ResourceService.FilePropertiesResource.GetString("Archive");
        private readonly string CreateDateString = ResourceService.FilePropertiesResource.GetString("CreateDate");
        private readonly string DragOverContentString = ResourceService.FilePropertiesResource.GetString("DragOverContent");
        private readonly string HideString = ResourceService.FilePropertiesResource.GetString("Hide");
        private readonly string ModifyDateString = ResourceService.FilePropertiesResource.GetString("ModifyDate");
        private readonly string ModifyingNowString = ResourceService.FilePropertiesResource.GetString("ModifyingNow");
        private readonly string ReadOnlyString = ResourceService.FilePropertiesResource.GetString("ReadOnly");
        private readonly string SelectFileString = ResourceService.FilePropertiesResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.FilePropertiesResource.GetString("SelectFolder");
        private readonly string SystemString = ResourceService.FilePropertiesResource.GetString("System");
        private readonly string TotalString = ResourceService.FilePropertiesResource.GetString("Total");
        private readonly object filePropertiesLock = new();

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isReadOnlyChecked;

        private bool IsReadOnlyChecked
        {
            get { return _isReadOnlyChecked; }

            set
            {
                if (!Equals(_isReadOnlyChecked, value))
                {
                    _isReadOnlyChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsReadOnlyChecked)));
                }
            }
        }

        private bool _isArchiveChecked;

        private bool IsArchiveChecked
        {
            get { return _isArchiveChecked; }

            set
            {
                if (!Equals(_isArchiveChecked, value))
                {
                    _isArchiveChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsArchiveChecked)));
                }
            }
        }

        private bool _isCreateDateChecked;

        private bool IsCreateDateChecked
        {
            get { return _isCreateDateChecked; }

            set
            {
                if (!Equals(_isCreateDateChecked, value))
                {
                    _isCreateDateChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCreateDateChecked)));
                }
            }
        }

        private bool _isHideChecked;

        private bool IsHideChecked
        {
            get { return _isHideChecked; }

            set
            {
                if (!Equals(_isHideChecked, value))
                {
                    _isHideChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsHideChecked)));
                }
            }
        }

        private bool _isSystemChecked;

        private bool IsSystemChecked
        {
            get { return _isSystemChecked; }

            set
            {
                if (!Equals(_isSystemChecked, value))
                {
                    _isSystemChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSystemChecked)));
                }
            }
        }

        private bool _isModifyDateChecked;

        private bool IsModifyDateChecked
        {
            get { return _isModifyDateChecked; }

            set
            {
                if (!Equals(_isModifyDateChecked, value))
                {
                    _isModifyDateChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsModifyDateChecked)));
                }
            }
        }

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

        private DateTimeOffset _createDate = DateTimeOffset.Now;

        private DateTimeOffset CreateDate
        {
            get { return _createDate; }

            set
            {
                if (!Equals(_createDate, value))
                {
                    _createDate = value;
                    PropertyChanged?.Invoke(this, new(nameof(CreateDate)));
                }
            }
        }

        private TimeSpan _createTime = DateTimeOffset.Now.TimeOfDay;

        private TimeSpan CreateTime
        {
            get { return _createTime; }

            set
            {
                if (!Equals(_createTime, value))
                {
                    _createTime = value;
                    PropertyChanged?.Invoke(this, new(nameof(CreateTime)));
                }
            }
        }

        private DateTimeOffset _modifyDate = DateTimeOffset.Now;

        private DateTimeOffset ModifyDate
        {
            get { return _modifyDate; }

            set
            {
                if (!Equals(_modifyDate, value))
                {
                    _modifyDate = value;
                    PropertyChanged?.Invoke(this, new(nameof(ModifyDate)));
                }
            }
        }

        private TimeSpan _modifyTime = DateTimeOffset.Now.TimeOfDay;

        private TimeSpan ModifyTime
        {
            get { return _modifyTime; }

            set
            {
                if (!Equals(_modifyTime, value))
                {
                    _modifyTime = value;
                    PropertyChanged?.Invoke(this, new(nameof(ModifyTime)));
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

        private WinRTObservableCollection<OldAndNewPropertiesModel> FilePropertiesCollection { get; } = [];

        private List<OperationFailedModel> OperationFailedList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal FilePropertiesPage()
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
            try
            {
                List<string> fileList = await GetDragDropSelectedFilesAsync(args.DataView);

                if (fileList is not null && fileList.Count > 0)
                {
                    List<OldAndNewPropertiesModel> filePropertiesList = await GetNeedConvertFileListAsync(fileList);
                    if (filePropertiesList is not null && filePropertiesList.Count > 0)
                    {
                        AddToFilePropertiesPage(filePropertiesList);
                        IsOperationFailed = false;
                        OperationFailedList.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(UpperAndLowerCasePage), nameof(OnDrop), 1, e);
            }
            finally
            {
                args.Handled = true;
                dragOperationDeferral.Complete();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 删除当前项
        /// </summary>
        private void OnDeleteExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewPropertiesModel oldAndNewProperties)
            {
                lock (filePropertiesLock)
                {
                    FilePropertiesCollection.Remove(oldAndNewProperties);
                }
            }
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        private void OnMoveDownExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewPropertiesModel oldAndNewProperties)
            {
                lock (filePropertiesLock)
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
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        private void OnMoveUpExecuteRequested(object sender, Extensions.DataType.Class.ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is OldAndNewPropertiesModel oldAndNewProperties)
            {
                lock (filePropertiesLock)
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
        }

        #endregion 第五部分：挂载事件处理

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
                    await PrepareChangeFilePropertiesAsync(IsReadOnlyChecked, IsArchiveChecked, IsCreateDateChecked, IsHideChecked, IsSystemChecked, IsModifyDateChecked, CreateDate, CreateTime, ModifyDate, ModifyTime, false);
                }
                else if (sender.Modifiers is VirtualKeyModifiers.Control)
                {
                    await PrepareChangeFilePropertiesAsync(IsReadOnlyChecked, IsArchiveChecked, IsCreateDateChecked, IsHideChecked, IsSystemChecked, IsModifyDateChecked, CreateDate, CreateTime, ModifyDate, ModifyTime, true);
                }
                args.Handled = true;
            }
        }

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
        /// 创建日期更改时触发的事件
        /// </summary>
        private void OnCreateDateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            CreateDate = args.NewDate;
        }

        /// <summary>
        /// 修改日期更改时触发的事件
        /// </summary>
        private void OnModifyDateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            ModifyDate = args.NewDate;
        }

        /// <summary>
        /// 预览修改的内容
        /// </summary>
        private async void OnPreviewClicked(object sender, RoutedEventArgs args)
        {
            await PrepareChangeFilePropertiesAsync(IsReadOnlyChecked, IsArchiveChecked, IsCreateDateChecked, IsHideChecked, IsSystemChecked, IsModifyDateChecked, CreateDate, CreateTime, ModifyDate, ModifyTime, false);
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        private async void OnModifyClicked(object sender, RoutedEventArgs args)
        {
            await PrepareChangeFilePropertiesAsync(IsReadOnlyChecked, IsArchiveChecked, IsCreateDateChecked, IsHideChecked, IsSystemChecked, IsModifyDateChecked, CreateDate, CreateTime, ModifyDate, ModifyTime, true);
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
                List<OldAndNewPropertiesModel> filePropertiesList = await GetNeedConvertFileListAsync([.. openFileDialog.FileNames]);
                if (filePropertiesList is not null && filePropertiesList.Count > 0)
                {
                    openFileDialog.Dispose();
                    AddToFilePropertiesPage(filePropertiesList);
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
                    (List<OldAndNewPropertiesModel> directoryNameList, List<OldAndNewPropertiesModel> fileNameList) = await GetFileAndDirectoryAsync(openFolderDialog.SelectedPath);

                    if (directoryNameList is not null && directoryNameList.Count > 0)
                    {
                        AddToFilePropertiesPage(directoryNameList);
                    }

                    if (fileNameList is not null && fileNameList.Count > 0)
                    {
                        AddToFilePropertiesPage(fileNameList);
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
        /// 创建日期自定义选择框取消选中时触发的事件
        /// </summary>
        private void OnCreateUnchecked(object sender, RoutedEventArgs args)
        {
            CreateDate = DateTimeOffset.Now;
            CreateTime = DateTimeOffset.Now.TimeOfDay;
        }

        /// <summary>
        /// 修改日期自定义选择框取消选中时触发的事件
        /// </summary>
        private void OnModifyUnchecked(object sender, RoutedEventArgs args)
        {
            ModifyDate = DateTimeOffset.Now;
            ModifyTime = DateTimeOffset.Now.TimeOfDay;
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
        /// 添加到文件属性页面
        /// </summary>
        internal void AddToFilePropertiesPage(List<OldAndNewPropertiesModel> filePropertiesList)
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
        /// 准备修改文件属性
        /// </summary>
        private async Task PrepareChangeFilePropertiesAsync(bool isReadOnlyChecked, bool isArchiveChecked, bool isCreateDateChecked, bool isHideChecked, bool isSystemChecked, bool isModifyDateChecked, DateTimeOffset createDate, TimeSpan createTime, DateTimeOffset modifyDate, TimeSpan modifyTime, bool needChange)
        {
            if (isReadOnlyChecked || isArchiveChecked || isCreateDateChecked || isHideChecked || isSystemChecked || isModifyDateChecked)
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
                    PreviewChangedFileAttributes(isReadOnlyChecked, isArchiveChecked, isCreateDateChecked, isHideChecked, isSystemChecked, isModifyDateChecked);
                    if (needChange)
                    {
                        await ChangeFileAttributesAsync(isReadOnlyChecked, isArchiveChecked, isCreateDateChecked, isHideChecked, isSystemChecked, isModifyDateChecked, createDate, createTime, modifyDate, modifyTime);
                    }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NoOperation));
            }
        }

        /// <summary>
        /// 修改文件属性
        /// </summary>
        private void PreviewChangedFileAttributes(bool isReadOnlyChecked, bool isArchiveChecked, bool isCreateDateChecked, bool isHideChecked, bool isSystemChecked, bool isModifyDateChecked)
        {
            StringBuilder stringBuilder = new();
            if (isReadOnlyChecked)
            {
                stringBuilder.Append(ReadOnlyString);
                stringBuilder.Append(' ');
            }
            if (isArchiveChecked)
            {
                stringBuilder.Append(ArchiveString);
                stringBuilder.Append(' ');
            }
            if (isHideChecked)
            {
                stringBuilder.Append(HideString);
                stringBuilder.Append(' ');
            }
            if (isSystemChecked)
            {
                stringBuilder.Append(SystemString);
                stringBuilder.Append(' ');
            }
            if (isCreateDateChecked)
            {
                stringBuilder.Append(CreateDateString);
                stringBuilder.Append(' ');
            }
            if (isModifyDateChecked)
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
        private async Task ChangeFileAttributesAsync(bool isReadOnlyChecked, bool isArchiveChecked, bool isCreateDateChecked, bool isHideChecked, bool isSystemChecked, bool isModifyDateChecked, DateTimeOffset createDate, TimeSpan createTime, DateTimeOffset modifyDate, TimeSpan modifyTime)
        {
            IsModifyingNow = true;
            lock (filePropertiesLock)
            {
                foreach (OldAndNewPropertiesModel oldAndNewProperties in FilePropertiesCollection)
                {
                    oldAndNewProperties.IsModifyingNow = true;
                }
            }
            List<OperationFailedModel> operationFailedList = await GetOperationFailedListAsync(isReadOnlyChecked, isArchiveChecked, isCreateDateChecked, isHideChecked, isSystemChecked, isModifyDateChecked, createDate, createTime, modifyDate, modifyTime);

            IsModifyingNow = false;
            lock (filePropertiesLock)
            {
                foreach (OldAndNewPropertiesModel oldAndNewProperties in FilePropertiesCollection)
                {
                    oldAndNewProperties.IsModifyingNow = false;
                }
            }
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(GetDragDropSelectedFilesAsync), 1, e);
                }

                return fileList;
            });
        }

        /// <summary>
        /// 获取要待转换的文件列表
        /// </summary>
        private async Task<List<OldAndNewPropertiesModel>> GetNeedConvertFileListAsync(List<string> fileList)
        {
            if (fileList is null || fileList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<OldAndNewPropertiesModel> upperAndLowerCaseList = [];

                foreach (string file in fileList)
                {
                    try
                    {
                        FileInfo fileInfo = new(file);
                        if ((fileInfo.Attributes & System.IO.FileAttributes.Hidden) is System.IO.FileAttributes.Hidden)
                        {
                            continue;
                        }

                        upperAndLowerCaseList.Add(new()
                        {
                            FileName = Path.GetFileName(file),
                            FilePath = file,
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(GetNeedConvertFileListAsync), 1, e);
                        continue;
                    }
                }

                return upperAndLowerCaseList;
            });
        }

        /// <summary>
        /// 获取文件夹的所有子文件夹和所有文件
        /// </summary>
        private async Task<(List<OldAndNewPropertiesModel>, List<OldAndNewPropertiesModel>)> GetFileAndDirectoryAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return ValueTuple.Create<List<OldAndNewPropertiesModel>, List<OldAndNewPropertiesModel>>([], []);
            }

            return await Task.Run(() =>
            {
                List<OldAndNewPropertiesModel> directoryNameList = [];
                List<OldAndNewPropertiesModel> fileNameList = [];
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
                            FileName = directoryInfo.Name,
                            FilePath = directoryInfo.FullName
                        });
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(GetFileAndDirectoryAsync), 1, e);
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
                            FileName = fileInfo.Name,
                            FilePath = fileInfo.FullName
                        });
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(FilePropertiesPage), nameof(GetFileAndDirectoryAsync), 2, e);
                }
                return ValueTuple.Create(directoryNameList, fileNameList);
            });
        }

        /// <summary>
        /// 获取失败操作列表
        /// </summary>
        private async Task<List<OperationFailedModel>> GetOperationFailedListAsync(bool isReadOnlyChecked, bool isArchiveChecked, bool isCreateDateChecked, bool isHideChecked, bool isSystemChecked, bool isModifyDateChecked, DateTimeOffset createDate, TimeSpan createTime, DateTimeOffset modifyDate, TimeSpan modifyTime)
        {
            return await Task.Run(() =>
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
                                if (isReadOnlyChecked) fileAttributes |= System.IO.FileAttributes.ReadOnly;
                                if (isArchiveChecked) fileAttributes |= System.IO.FileAttributes.Archive;
                                if (isHideChecked) fileAttributes |= System.IO.FileAttributes.Hidden;
                                if (isSystemChecked) fileAttributes |= System.IO.FileAttributes.System;
                                File.SetAttributes(oldAndNewPropertiesItem.FilePath, fileAttributes);

                                if (isCreateDateChecked)
                                {
                                    File.SetCreationTime(oldAndNewPropertiesItem.FilePath, createDate.Date + createTime);
                                }
                                if (isModifyDateChecked)
                                {
                                    File.SetLastWriteTime(oldAndNewPropertiesItem.FilePath, modifyDate.Date + modifyTime);
                                }
                            }
                            catch (Exception e)
                            {
                                operationFailedList.Add(new()
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
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
