using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.DataType.Methods;
using PowerToolbox.Extensions.PriExtract;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 包资源索引提取页面
    /// </summary>
    internal sealed partial class PriExtractPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AllLanguageString = ResourceService.PriExtractResource.GetString("AllLanguage");
        private readonly string DragOverContentString = ResourceService.PriExtractResource.GetString("DragOverContent");
        private readonly string EmbeddedDataString = ResourceService.PriExtractResource.GetString("EmbeddedData");
        private readonly string FilePathString = ResourceService.PriExtractResource.GetString("FilePath");
        private readonly string FilterConditionString = ResourceService.PriExtractResource.GetString("FilterCondition");
        private readonly string GetResultsString = ResourceService.PriExtractResource.GetString("GetResults");
        private readonly string NoMultiFileString = ResourceService.PriExtractResource.GetString("NoMultiFile");
        private readonly string NoOtherExtensionNameFileString = ResourceService.PriExtractResource.GetString("NoOtherExtensionNameFile");
        private readonly string NoSelectedFileString = ResourceService.PriExtractResource.GetString("NoSelectedFile");
        private readonly string SelectedFolderString = ResourceService.PriExtractResource.GetString("SelectedFolder");
        private readonly string SelectFileString = ResourceService.PriExtractResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.PriExtractResource.GetString("SelectFolder");
        private readonly string StringString = ResourceService.PriExtractResource.GetString("String");
        private bool isLoadCompleted = false;
        private string stringFileName;
        private string filePathFileName;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private bool _isExtractSaveSame;

        private bool IsExtractSaveSame
        {
            get { return _isExtractSaveSame; }

            set
            {
                if (!Equals(_isExtractSaveSame, value))
                {
                    _isExtractSaveSame = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsExtractSaveSame)));
                }
            }
        }

        private bool _isExtractSaveString;

        private bool IsExtractSaveString
        {
            get { return _isExtractSaveString; }

            set
            {
                if (!Equals(_isExtractSaveString, value))
                {
                    _isExtractSaveString = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsExtractSaveString)));
                }
            }
        }

        private bool _isExtractSaveFilePath;

        private bool IsExtractSaveFilePath
        {
            get { return _isExtractSaveFilePath; }

            set
            {
                if (!Equals(_isExtractSaveFilePath, value))
                {
                    _isExtractSaveFilePath = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsExtractSaveFilePath)));
                }
            }
        }

        private bool _isExtractSaveEmbeddedData;

        private bool IsExtractSaveEmbeddedData
        {
            get { return _isExtractSaveEmbeddedData; }

            set
            {
                if (!Equals(_isExtractSaveEmbeddedData, value))
                {
                    _isExtractSaveEmbeddedData = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsExtractSaveEmbeddedData)));
                }
            }
        }

        private ComboBoxItemModel _selectedLanguage;

        private ComboBoxItemModel SelectedLanguage
        {
            get { return _selectedLanguage; }

            set
            {
                if (!Equals(_selectedLanguage, value))
                {
                    _selectedLanguage = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedLanguage)));
                }
            }
        }

        private bool _isProcessing;

        internal bool IsProcessing
        {
            get { return _isProcessing; }

            private set
            {
                if (!Equals(_isProcessing, value))
                {
                    _isProcessing = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsProcessing)));
                }
            }
        }

        private string _getResults;

        private string GetResults
        {
            get { return _getResults; }

            set
            {
                if (!string.Equals(_getResults, value))
                {
                    _getResults = value;
                    PropertyChanged?.Invoke(this, new(nameof(GetResults)));
                }
            }
        }

        private string _selectedSaveFolder;

        private string SelectedSaveFolder
        {
            get { return _selectedSaveFolder; }

            set
            {
                if (!string.Equals(_selectedSaveFolder, value))
                {
                    _selectedSaveFolder = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedSaveFolder)));
                }
            }
        }

        private ComboBoxItemModel _selectedResourceCandidateKind;

        private ComboBoxItemModel SelectedResourceCandidateKind
        {
            get { return _selectedResourceCandidateKind; }

            set
            {
                if (!Equals(_selectedResourceCandidateKind, value))
                {
                    _selectedResourceCandidateKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedResourceCandidateKind)));
                }
            }
        }

        private bool _hasStringResource;

        private bool HasStringResource
        {
            get { return _hasStringResource; }

            set
            {
                if (!Equals(_hasStringResource, value))
                {
                    _hasStringResource = value;
                    PropertyChanged?.Invoke(this, new(nameof(HasStringResource)));
                }
            }
        }

        private string _stringSearchText;

        private string StringSearchText
        {
            get { return _stringSearchText; }

            set
            {
                if (!string.Equals(_stringSearchText, value))
                {
                    _stringSearchText = value;
                    PropertyChanged?.Invoke(this, new(nameof(StringSearchText)));
                }
            }
        }

        private bool _hasFilePathResource;

        private bool HasFilePathResource
        {
            get { return _hasFilePathResource; }

            set
            {
                if (!Equals(_hasFilePathResource, value))
                {
                    _hasFilePathResource = value;
                    PropertyChanged?.Invoke(this, new(nameof(HasFilePathResource)));
                }
            }
        }

        private string _filePathSearchText;

        private string FilePathSearchText
        {
            get { return _filePathSearchText; }

            set
            {
                if (!string.Equals(_filePathSearchText, value))
                {
                    _filePathSearchText = value;
                    PropertyChanged?.Invoke(this, new(nameof(FilePathSearchText)));
                }
            }
        }

        private bool _hasEmbeddedDataResource;

        private bool HasEmbeddedDataResource
        {
            get { return _hasEmbeddedDataResource; }

            set
            {
                if (!Equals(_hasEmbeddedDataResource, value))
                {
                    _hasEmbeddedDataResource = value;
                    PropertyChanged?.Invoke(this, new(nameof(HasEmbeddedDataResource)));
                }
            }
        }

        private string _embeddedDataSearchText;

        private string EmbeddedDataSearchText
        {
            get { return _embeddedDataSearchText; }

            set
            {
                if (!string.Equals(_embeddedDataSearchText, value))
                {
                    _embeddedDataSearchText = value;
                    PropertyChanged?.Invoke(this, new(nameof(EmbeddedDataSearchText)));
                }
            }
        }

        private List<ComboBoxItemModel> ResourceCandidateKindList { get; } = [];

        private List<StringModel> StringList { get; } = [];

        private List<FilePathModel> FilePathList { get; } = [];

        private List<EmbeddedDataModel> EmbeddedDataList { get; } = [];

        private WinRTObservableCollection<ComboBoxItemModel> LanguageCollection { get; } = [];

        private WinRTObservableCollection<StringModel> StringCollection { get; } = [];

        private WinRTObservableCollection<FilePathModel> FilePathCollection { get; } = [];

        private WinRTObservableCollection<EmbeddedDataModel> EmbeddedDataCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal PriExtractPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        protected override async void OnDragOver(Microsoft.UI.Xaml.DragEventArgs args)
        {
            base.OnDragOver(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                if (IsProcessing)
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = NoOtherExtensionNameFileString;
                }
                else
                {
                    if (await GetDragDropSelectedFilesAsync(args.DataView) is List<string> fileList && fileList.Count is 1)
                    {
                        string extensionName = Path.GetExtension(fileList[0]);

                        if (string.Equals(extensionName, ".pri", StringComparison.OrdinalIgnoreCase))
                        {
                            args.AcceptedOperation = DataPackageOperation.Copy;
                            args.DragUIOverride.IsCaptionVisible = true;
                            args.DragUIOverride.IsContentVisible = false;
                            args.DragUIOverride.IsGlyphVisible = true;
                            args.DragUIOverride.Caption = DragOverContentString;
                        }
                        else
                        {
                            args.AcceptedOperation = DataPackageOperation.None;
                            args.DragUIOverride.IsCaptionVisible = true;
                            args.DragUIOverride.IsContentVisible = false;
                            args.DragUIOverride.IsGlyphVisible = true;
                            args.DragUIOverride.Caption = NoOtherExtensionNameFileString;
                        }
                    }
                    else
                    {
                        args.AcceptedOperation = DataPackageOperation.None;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = NoMultiFileString;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(OnDragOver), 1, e);
                return;
            }
            finally
            {
                args.Handled = true;
                dragOperationDeferral.Complete();
            }
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(Microsoft.UI.Xaml.DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();
            string filePath = string.Empty;

            try
            {
                if (await GetDragDropSelectedFilesAsync(args.DataView) is List<string> fileList && fileList.Count is 1)
                {
                    filePath = fileList[0];
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                await ParseResourceFileAsync(filePath);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 复制字符串到剪贴板
        /// </summary>
        private async void OnStringExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is StringModel stringItem && await GetStringCopyListAsync([stringItem]) is string copyString)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyString);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制文件路径到剪贴板
        /// </summary>
        private async void OnFilePathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is FilePathModel filePathItem && await GetFilePathCopyListAsync([filePathItem]) is string copyFilePath)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyFilePath);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 导出嵌入数据
        /// </summary>
        private async void OnEmbeddedDataExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is EmbeddedDataModel embeddedData)
            {
                IsProcessing = true;
                OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };

                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    await SaveEmbeddedDataListAsync(openFolderDialog.SelectedPath, [embeddedData]);
                    await OpenEmbeddedDataSavedFolderPathAsync(openFolderDialog.SelectedPath, embeddedData.Key);
                    openFolderDialog.Dispose();
                }
                else
                {
                    openFolderDialog.Dispose();
                }

                IsProcessing = false;
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 提取时同时保存单选框点击时触发的事件
        /// </summary>
        private void OnExtractSaveSameClicked(object sender, RoutedEventArgs args)
        {
            if (!IsExtractSaveSame)
            {
                IsExtractSaveString = false;
                IsExtractSaveFilePath = false;
                IsExtractSaveEmbeddedData = false;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[0]))
            {
                PriExtractStringListView.SelectAll();
            }
            else if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[1]))
            {
                PriExtractFilePathListView.SelectAll();
            }
            else if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[2]))
            {
                PriExtractEmbeddedDataListView.SelectAll();
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[0]))
            {
                PriExtractStringListView.DeselectRange(new(0, (uint)PriExtractStringListView.Items.Count));
            }
            else if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[1]))
            {
                PriExtractFilePathListView.DeselectRange(new(0, (uint)PriExtractFilePathListView.Items.Count));
            }
            else if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[2]))
            {
                PriExtractEmbeddedDataListView.DeselectRange(new(0, (uint)PriExtractEmbeddedDataListView.Items.Count));
            }
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[0]))
            {
                List<object> selectedItemList = [.. PriExtractStringListView.SelectedItems];

                foreach (object item in PriExtractStringListView.Items)
                {
                    if (selectedItemList.Contains(item))
                    {
                        PriExtractStringListView.SelectedItems.Remove(item);
                    }
                    else
                    {
                        PriExtractStringListView.SelectedItems.Add(item);
                    }
                }
            }
            else if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[1]))
            {
                List<object> selectedItemList = [.. PriExtractFilePathListView.SelectedItems];

                foreach (object item in PriExtractFilePathListView.Items)
                {
                    if (selectedItemList.Contains(item))
                    {
                        PriExtractFilePathListView.SelectedItems.Remove(item);
                    }
                    else
                    {
                        PriExtractFilePathListView.SelectedItems.Add(item);
                    }
                }
            }
            else if (Equals(SelectedResourceCandidateKind, ResourceCandidateKindList[2]))
            {
                List<object> selectedItemList = [.. PriExtractEmbeddedDataListView.SelectedItems];

                foreach (object item in PriExtractEmbeddedDataListView.Items)
                {
                    if (selectedItemList.Contains(item))
                    {
                        PriExtractEmbeddedDataListView.SelectedItems.Remove(item);
                    }
                    else
                    {
                        PriExtractEmbeddedDataListView.SelectedItems.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 自动保存时选择保存的文件夹
        /// </summary>
        private void OnSelectSaveFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                SelectedSaveFolder = openFolderDialog.SelectedPath;
            }
            openFolderDialog.Dispose();
        }

        /// <summary>
        /// 语言设置选项修改后触发的事件
        /// </summary>
        private async void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (isLoadCompleted && sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedLanguage, comboBox.SelectedItem))
            {
                SelectedLanguage = comboBox.SelectedItem is ComboBoxItemModel language ? language : null;

                if (SelectedLanguage is not null)
                {
                    PriExtractStringListView.SelectedItems.Clear();
                    if (await GetFilteredStringAsync(StringSearchText, Convert.ToString(SelectedLanguage.SelectedValue), StringList) is List<StringModel> filteredStringList)
                    {
                        UpdateStringResult(filteredStringList);
                    }
                }
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private async void OnSelectFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = FilterConditionString,
                Title = SelectFileString
            };

            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                await ParseResourceFileAsync(openFileDialog.FileName);
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 选择要显示的封装的资源类型数据的格式
        /// </summary>
        private void OnResourceCandidateKindSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedResourceCandidateKind, comboBox.SelectedItem))
            {
                SelectedResourceCandidateKind = comboBox.SelectedItem is ComboBoxItemModel resourceCandidateKind ? resourceCandidateKind : null;
            }
        }

        /// <summary>
        /// 搜索字符串
        /// </summary>
        private async void OnStringQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            PriExtractStringListView.SelectedItems.Clear();
            if (await GetFilteredStringAsync(StringSearchText, Convert.ToString(SelectedLanguage.SelectedValue), StringList) is List<StringModel> filteredStringList)
            {
                UpdateStringResult(filteredStringList);
            }
        }

        /// <summary>
        /// 字符串内容发生变化时触发的事件
        /// </summary>
        private async void OnStringTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            StringSearchText = sender.Text;
            if (string.IsNullOrEmpty(StringSearchText))
            {
                PriExtractStringListView.SelectedItems.Clear();
                if (await GetFilteredStringAsync(StringSearchText, Convert.ToString(SelectedLanguage.SelectedValue), StringList) is List<StringModel> filteredStringList)
                {
                    UpdateStringResult(filteredStringList);
                }
            }
        }

        /// <summary>
        /// 搜索文件路径
        /// </summary>
        private async void OnFilePathQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            PriExtractFilePathListView.SelectedItems.Clear();
            if (await GetFilteredFilePathAsync(FilePathSearchText, FilePathList) is List<FilePathModel> filteredFilePathList)
            {
                UpdateFilePathResult(filteredFilePathList);
            }
        }

        /// <summary>
        /// 文件内容发生变化时触发的事件
        /// </summary>
        private async void OnFilePathTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            FilePathSearchText = sender.Text;
            if (string.IsNullOrEmpty(FilePathSearchText))
            {
                PriExtractFilePathListView.SelectedItems.Clear();
                if (await GetFilteredFilePathAsync(FilePathSearchText, FilePathList) is List<FilePathModel> filteredFilePathList)
                {
                    UpdateFilePathResult(filteredFilePathList);
                }
            }
        }

        /// <summary>
        /// 搜索嵌入的数据
        /// </summary>
        private async void OnEmbeddedDataQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            PriExtractEmbeddedDataListView.SelectedItems.Clear();
            if (await GetFilteredEmbeddedDataAsync(EmbeddedDataSearchText, EmbeddedDataList) is List<EmbeddedDataModel> filteredEmbeddedDataList)
            {
                UpdateEmbeddedDataResult(filteredEmbeddedDataList);
            }
        }

        /// <summary>
        /// 嵌入的数据发生变化时触发的事件
        /// </summary>
        private async void OnEmbeddedDataTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            EmbeddedDataSearchText = sender.Text;
            if (string.IsNullOrEmpty(EmbeddedDataSearchText))
            {
                PriExtractEmbeddedDataListView.SelectedItems.Clear();
                if (await GetFilteredEmbeddedDataAsync(EmbeddedDataSearchText, EmbeddedDataList) is List<EmbeddedDataModel> filteredEmbeddedDataList)
                {
                    UpdateEmbeddedDataResult(filteredEmbeddedDataList);
                }
            }
        }

        /// <summary>
        /// 复制选中的字符串
        /// </summary>
        private async void OnCopySelectedStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<StringModel> selectedStringList = [];
            foreach (object selectedItem in PriExtractStringListView.SelectedItems)
            {
                if (selectedItem is StringModel stringItem)
                {
                    selectedStringList.Add(stringItem);
                }
            }

            if (selectedStringList.Count > 0 && await GetStringCopyListAsync(selectedStringList) is string copyString)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyString);
                IsProcessing = false;
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
            else
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// 复制选中的文件路径
        /// </summary>
        private async void OnCopySelectedFilePathClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<FilePathModel> selectedFilePathList = [];
            foreach (object selectedItem in PriExtractFilePathListView.SelectedItems)
            {
                if (selectedItem is FilePathModel filePathItem)
                {
                    selectedFilePathList.Add(filePathItem);
                }
            }

            if (selectedFilePathList.Count > 0 && await GetFilePathCopyListAsync(selectedFilePathList) is string copyFilePath)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyFilePath);
                IsProcessing = false;
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
            else
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// 导出选中的嵌入数据
        /// </summary>
        private async void OnExportSelectedEmbeddedDataClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<EmbeddedDataModel> selectedEmbeddedDataList = [];
            foreach (object selectedItem in PriExtractEmbeddedDataListView.SelectedItems)
            {
                if (selectedItem is EmbeddedDataModel embeddedDataItem)
                {
                    selectedEmbeddedDataList.Add(embeddedDataItem);
                }
            }

            if (selectedEmbeddedDataList.Count > 0)
            {
                OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
                {
                    Description = SelectFolderString,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                DialogResult dialogResult = openFolderDialog.ShowDialog();
                if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                {
                    await SaveEmbeddedDataListAsync(openFolderDialog.SelectedPath, selectedEmbeddedDataList);
                    await OpenEmbeddedDataSavedFolderPathAsync(openFolderDialog.SelectedPath, null);
                }
                openFolderDialog.Dispose();
            }
            IsProcessing = false;
        }

        /// <summary>
        /// 复制所有的字符串
        /// </summary>
        private async void OnCopyAllStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            if (await GetStringCopyListAsync([.. StringCollection]) is string copyString)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyString);
                IsProcessing = false;
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
            else
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// 复制所有的文件路径
        /// </summary>
        private async void OnCopyAllFilePathClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            if (await GetFilePathCopyListAsync([.. FilePathCollection]) is string copyFilePath)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(copyFilePath);
                IsProcessing = false;
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
            else
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// 导出所有的嵌入数据
        /// </summary>
        private async void OnExportAllEmbeddedDataClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };

            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if ((dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes) && EmbeddedDataCollection.ToList() is List<EmbeddedDataModel> exportEmbeddedDataList && exportEmbeddedDataList.Count > 0)
            {
                await SaveEmbeddedDataListAsync(openFolderDialog.SelectedPath, exportEmbeddedDataList);
                await OpenEmbeddedDataSavedFolderPathAsync(openFolderDialog.SelectedPath, null);
            }
            openFolderDialog.Dispose();
            IsProcessing = false;
        }

        #endregion 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            GetResults = NoSelectedFileString;
            ResourceCandidateKindList.Add(new() { SelectedValue = "String", DisplayMember = StringString });
            ResourceCandidateKindList.Add(new() { SelectedValue = "FilePath", DisplayMember = FilePathString });
            ResourceCandidateKindList.Add(new() { SelectedValue = "EmbeddedData", DisplayMember = EmbeddedDataString });
            SelectedResourceCandidateKind = ResourceCandidateKindList[0];
            Shell32Library.SHGetKnownFolderPath(new("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, 0, out string downloadFolder);
            SelectedSaveFolder = downloadFolder;
        }

        /// <summary>
        /// 解析 PRI 资源文件
        /// </summary>
        internal async Task ParseResourceFileAsync(string filePath)
        {
            isLoadCompleted = false;
            IsProcessing = true;
            StringSearchText = string.Empty;
            FilePathSearchText = string.Empty;
            EmbeddedDataSearchText = string.Empty;
            HasStringResource = false;
            HasFilePathResource = false;
            HasEmbeddedDataResource = false;
            StringList.Clear();
            FilePathList.Clear();
            EmbeddedDataList.Clear();
            LanguageCollection.Clear();
            StringCollection.Clear();
            FilePathCollection.Clear();
            EmbeddedDataCollection.Clear();
            (bool result, List<string> languageList, List<StringModel> stringList, List<FilePathModel> filePathList, List<EmbeddedDataModel> embeddedDataList) = await GetResourceFileContentAsync(filePath, IsExtractSaveString, IsExtractSaveFilePath, IsExtractSaveEmbeddedData, SelectedSaveFolder);

            if (result)
            {
                StringList.AddRange(stringList);
                FilePathList.AddRange(filePathList);
                EmbeddedDataList.AddRange(embeddedDataList);
                LanguageCollection.Add(new() { SelectedValue = "AllLanguage", DisplayMember = AllLanguageString });

                // 显示获取到的所有内容
                foreach (string languageItem in languageList)
                {
                    CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageItem);
                    LanguageCollection.Add(new() { SelectedValue = cultureInfo.Name, DisplayMember = string.Format("{0}[{1}]", cultureInfo.DisplayName, languageItem) });
                }

                SelectedLanguage = LanguageCollection[0];
                HasStringResource = StringList.Count > 0;
                HasFilePathResource = FilePathList.Count > 0;
                HasEmbeddedDataResource = EmbeddedDataList.Count > 0;
                PriExtractStringListView.SelectedItems.Clear();
                PriExtractFilePathListView.SelectedItems.Clear();
                PriExtractEmbeddedDataListView.SelectedItems.Clear();
                if (await GetFilteredStringAsync(StringSearchText, Convert.ToString(SelectedLanguage.SelectedValue), StringList) is List<StringModel> filteredStringList)
                {
                    UpdateStringResult(filteredStringList);
                }
                if (await GetFilteredFilePathAsync(FilePathSearchText, FilePathList) is List<FilePathModel> filteredFilePathList)
                {
                    UpdateFilePathResult(filteredFilePathList);
                }
                if (await GetFilteredEmbeddedDataAsync(EmbeddedDataSearchText, EmbeddedDataList) is List<EmbeddedDataModel> filteredEmbeddedDataList)
                {
                    UpdateEmbeddedDataResult(filteredEmbeddedDataList);
                }
                IsProcessing = false;
                GetResults = string.Format(GetResultsString, Path.GetFileName(filePath), StringList.Count + FilePathList.Count + EmbeddedDataList.Count);
                isLoadCompleted = true;
            }
            else
            {
                IsProcessing = false;
                GetResults = string.Format(GetResultsString, Path.GetFileName(filePath), 0);
                isLoadCompleted = true;
            }
        }

        /// <summary>
        /// 获取 PRI 资源文件的所有内容
        /// </summary>
        private async Task<(bool, List<string>, List<StringModel>, List<FilePathModel>, List<EmbeddedDataModel>)> GetResourceFileContentAsync(string filePath, bool isExtractSaveString, bool isExtractSaveFilePath, bool isExtractSaveEmbeddedData, string saveFolder)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                List<string> languageList = [];
                List<StringModel> stringList = [];
                List<FilePathModel> filePathList = [];
                List<EmbeddedDataModel> embeddedDataList = [];
                stringFileName = string.Format("{0} - {1}.txt", Path.GetFileName(filePath), "Strings");
                filePathFileName = string.Format("{0} - {1}.txt", Path.GetFileName(filePath), "FilePath");

                try
                {
                    // 尝试读取文件二进制流
                    using FileStream priFileStream = File.OpenRead(filePath);

                    // 读取并检查文件类型
                    BinaryReader priBinaryReader = new(priFileStream, Encoding.ASCII, true);
                    long fileStartOffset = priBinaryReader.BaseStream.Position;
                    string priType = new(priBinaryReader.ReadChars(8));

                    if (priType is not "mrm_pri0" && priType is not "mrm_pri1" && priType is not "mrm_pri2" && priType is not "mrm_prif")
                    {
                        throw new InvalidDataException("Data does not start with a PRI file header.");
                    }

                    priBinaryReader.ExpectUInt16(0);
                    priBinaryReader.ExpectUInt16(1);
                    uint totalFileSize = priBinaryReader.ReadUInt32();
                    uint tocOffset = priBinaryReader.ReadUInt32();
                    uint sectionStartOffset = priBinaryReader.ReadUInt32();
                    uint numSections = priBinaryReader.ReadUInt16();
                    priBinaryReader.ExpectUInt16(0xFFFF);
                    priBinaryReader.ExpectUInt32(0);
                    priBinaryReader.BaseStream.Seek(fileStartOffset + totalFileSize - 16, SeekOrigin.Begin);
                    priBinaryReader.ExpectUInt32(0xDEFFFADE);
                    priBinaryReader.ExpectUInt32(totalFileSize);
                    priBinaryReader.ExpectString(priType);
                    priBinaryReader.BaseStream.Seek(tocOffset, SeekOrigin.Begin);

                    // 读取内容列表
                    List<TocEntry> tocList = new((int)numSections);

                    for (int index = 0; index < numSections; index++)
                    {
                        tocList.Add(new()
                        {
                            SectionIdentifier = new(priBinaryReader.ReadChars(16)),
                            Flags = priBinaryReader.ReadUInt16(),
                            SectionFlags = priBinaryReader.ReadUInt16(),
                            SectionQualifier = priBinaryReader.ReadUInt32(),
                            SectionOffset = priBinaryReader.ReadUInt32(),
                            SectionLength = priBinaryReader.ReadUInt32(),
                        });
                    }

                    // 读取分段列表
                    object[] sectionArray = new object[numSections];

                    for (int index = 0; index < sectionArray.Length; index++)
                    {
                        if (sectionArray[index] is null)
                        {
                            priBinaryReader.BaseStream.Seek(sectionStartOffset + tocList[index].SectionOffset, SeekOrigin.Begin);

                            switch (tocList[index].SectionIdentifier)
                            {
                                case "[mrm_pridescex]\0":
                                    {
                                        PriDescriptorSection section = new("[mrm_pridescex]\0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_hschema]  \0":
                                    {
                                        HierarchicalSchemaSection section = new("[mrm_hschema]  \0", priBinaryReader, false);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_hschemaex] ":
                                    {
                                        HierarchicalSchemaSection section = new("[mrm_hschemaex] ", priBinaryReader, true);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_decn_info]\0":
                                    {
                                        DecisionInfoSection section = new("[mrm_decn_info]\0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_res_map__]\0":
                                    {
                                        ResourceMapSection section = new("[mrm_res_map__]\0", priBinaryReader, false, ref sectionArray);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_res_map2_]\0":
                                    {
                                        ResourceMapSection section = new("[mrm_res_map2_]\0", priBinaryReader, true, ref sectionArray);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_dataitem] \0":
                                    {
                                        DataItemSection section = new("[mrm_dataitem] \0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[mrm_rev_map]  \0":
                                    {
                                        ReverseMapSection section = new("[mrm_rev_map]  \0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                case "[def_file_list]\0":
                                    {
                                        ReferencedFileSection section = new("[def_file_list]\0", priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                                default:
                                    {
                                        UnknownSection section = new(null, priBinaryReader);
                                        sectionArray[index] = section;
                                        break;
                                    }
                            }
                        }
                    }

                    // 根据分段列表获取相应的内容
                    List<PriDescriptorSection> priDescriptorSectionList = [.. sectionArray.OfType<PriDescriptorSection>()];

                    foreach (PriDescriptorSection priDescriptorSection in priDescriptorSectionList)
                    {
                        foreach (int resourceMapIndex in priDescriptorSection.ResourceMapSectionsList)
                        {
                            if (sectionArray[resourceMapIndex] is ResourceMapSection resourceMapSection)
                            {
                                if (resourceMapSection.HierarchicalSchemaReference is not null)
                                {
                                    continue;
                                }

                                DecisionInfoSection decisionInfoSection = sectionArray[resourceMapSection.DecisionInfoSectionIndex] as DecisionInfoSection;

                                foreach (CandidateSet candidateSet in resourceMapSection.CandidateSetsDict.Values)
                                {
                                    if (sectionArray[candidateSet.ResourceMapSectionAndIndex.SchemaSectionIndex] is HierarchicalSchemaSection hierarchicalSchemaSection)
                                    {
                                        ResourceMapScopeAndItem resourceMapScopeAndItem = hierarchicalSchemaSection.ItemsList[candidateSet.ResourceMapSectionAndIndex.ResourceMapItemIndex];

                                        string key = string.Empty;

                                        if (resourceMapScopeAndItem.Name is not null && resourceMapScopeAndItem.Parent is not null)
                                        {
                                            key = Path.Combine(resourceMapScopeAndItem.Parent.Name, resourceMapScopeAndItem.Name);
                                        }
                                        else if (resourceMapScopeAndItem.Name is not null)
                                        {
                                            key = resourceMapScopeAndItem.Name;
                                        }

                                        if (string.IsNullOrEmpty(key))
                                        {
                                            continue;
                                        }

                                        foreach (Candidate candidate in candidateSet.CandidatesList)
                                        {
                                            string value = string.Empty;

                                            if (candidate.SourceFileIndex is null)
                                            {
                                                ByteSpan byteSpan = null;

                                                if (!candidate.DataItemSectionAndIndex.Equals(default))
                                                {
                                                    DataItemSection dataItemSection = sectionArray[candidate.DataItemSectionAndIndex.DataItemSection] as DataItemSection;
                                                    byteSpan = dataItemSection is not null ? dataItemSection.DataItemsList[candidate.DataItemSectionAndIndex.DataItemIndex] : candidate.Data;
                                                }

                                                if (byteSpan is not null)
                                                {
                                                    priFileStream.Seek(byteSpan.Offset, SeekOrigin.Begin);
                                                    using BinaryReader binaryReader = new(priFileStream, Encoding.Default, true);
                                                    byte[] data = binaryReader.ReadBytes((int)byteSpan.Length);
                                                    binaryReader.Dispose();

                                                    switch (candidate.Type)
                                                    {
                                                        // ASCII 格式路径内容
                                                        case ResourceValueType.AsciiPath:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.ASCII.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (isExtractSaveFilePath && !string.IsNullOrEmpty(saveFolder))
                                                                {
                                                                    SvaeFilePathData(saveFolder, filePathFileName, key, absolutePath);
                                                                }

                                                                filePathList.Add(new()
                                                                {
                                                                    Key = key,
                                                                    AbsolutePath = string.IsNullOrEmpty(absolutePath) ? string.Empty : absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // ASCII 格式字符串内容
                                                        case ResourceValueType.AsciiString:
                                                            {
                                                                string content = Encoding.ASCII.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (isExtractSaveString && !string.IsNullOrEmpty(saveFolder))
                                                                {
                                                                    SaveStringData(saveFolder, stringFileName, key, content);
                                                                }

                                                                stringList.Add(new()
                                                                {
                                                                    Key = key,
                                                                    Content = string.IsNullOrEmpty(content) ? string.Empty : content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                });
                                                                break;
                                                            }
                                                        // UTF8 格式路径内容
                                                        case ResourceValueType.Utf8Path:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.UTF8.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (isExtractSaveFilePath && !string.IsNullOrEmpty(saveFolder))
                                                                {
                                                                    SvaeFilePathData(saveFolder, filePathFileName, key, absolutePath);
                                                                }

                                                                filePathList.Add(new()
                                                                {
                                                                    Key = key,
                                                                    AbsolutePath = string.IsNullOrEmpty(absolutePath) ? string.Empty : absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // UTF8 格式字符串内容
                                                        case ResourceValueType.Utf8String:
                                                            {
                                                                string content = Encoding.UTF8.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (isExtractSaveString && !string.IsNullOrEmpty(saveFolder))
                                                                {
                                                                    SaveStringData(saveFolder, stringFileName, key, content);
                                                                }

                                                                stringList.Add(new()
                                                                {
                                                                    Key = key,
                                                                    Content = string.IsNullOrEmpty(content) ? string.Empty : content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                });
                                                                break;
                                                            }
                                                        // Unicode 格式路径内容
                                                        case ResourceValueType.UnicodePath:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.Unicode.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (isExtractSaveFilePath && !string.IsNullOrEmpty(saveFolder))
                                                                {
                                                                    SvaeFilePathData(saveFolder, filePathFileName, key, absolutePath);
                                                                }

                                                                filePathList.Add(new()
                                                                {
                                                                    Key = key,
                                                                    AbsolutePath = string.IsNullOrEmpty(absolutePath) ? string.Empty : absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // Unicode 格式字符串内容
                                                        case ResourceValueType.UnicodeString:
                                                            {
                                                                string content = Encoding.Unicode.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (isExtractSaveString && !string.IsNullOrEmpty(saveFolder))
                                                                {
                                                                    SaveStringData(saveFolder, stringFileName, key, content);
                                                                }

                                                                stringList.Add(new()
                                                                {
                                                                    Key = key,
                                                                    Content = string.IsNullOrEmpty(content) ? string.Empty : content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                });
                                                                break;
                                                            }
                                                        case ResourceValueType.EmbeddedData:
                                                            {
                                                                // 自动保存资源文件嵌入数据
                                                                if (isExtractSaveEmbeddedData && !string.IsNullOrEmpty(saveFolder))
                                                                {
                                                                    SaveEmbeddedData(saveFolder, Path.GetFileName(key), data);
                                                                }

                                                                embeddedDataList.Add(new()
                                                                {
                                                                    Key = key,
                                                                    EmbeddedData = data,
                                                                });
                                                                break;
                                                            }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 根据分段列表得到的内容进行归纳分类
                    languageList.AddRange(stringList.Select(item => item.Language).Distinct());
                    languageList.Sort();
                    stringList.Sort();
                    filePathList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));
                    embeddedDataList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));

                    priBinaryReader.Dispose();
                    priFileStream.Dispose();
                    return ValueTuple.Create(true, languageList, stringList, filePathList, embeddedDataList);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 8, e);
                    return ValueTuple.Create(false, languageList, stringList, filePathList, embeddedDataList);
                }
            });
        }

        /// <summary>
        /// 获取过滤后符合条件的字符串
        /// </summary>
        private async Task<List<StringModel>> GetFilteredStringAsync(string stringSearchText, string selectedLanguage, List<StringModel> stringList)
        {
            if (string.IsNullOrEmpty(selectedLanguage) || stringList is null || stringList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<StringModel> filteredStringList = [];

                if (string.Equals(selectedLanguage, "AllLanguage"))
                {
                    if (string.IsNullOrEmpty(stringSearchText))
                    {
                        foreach (StringModel stringItem in stringList)
                        {
                            filteredStringList.Add(stringItem);
                        }
                    }
                    else
                    {
                        foreach (StringModel stringItem in stringList)
                        {
                            if (stringItem.Key.Contains(stringSearchText) || stringItem.Content.Contains(stringSearchText))
                            {
                                filteredStringList.Add(stringItem);
                            }
                        }
                    }
                }
                // 某一语言
                else
                {
                    List<StringModel> coincidentStringList = [.. stringList.Where(item => string.Equals(item.Language, Convert.ToString(selectedLanguage), StringComparison.OrdinalIgnoreCase))];

                    if (string.IsNullOrEmpty(stringSearchText))
                    {
                        foreach (StringModel stringItem in coincidentStringList)
                        {
                            filteredStringList.Add(stringItem);
                        }
                    }
                    else
                    {
                        foreach (StringModel stringItem in coincidentStringList)
                        {
                            if (stringItem.Key.Contains(stringSearchText) || stringItem.Content.Contains(stringSearchText))
                            {
                                filteredStringList.Add(stringItem);
                            }
                        }
                    }
                }

                return filteredStringList;
            });
        }

        /// <summary>
        /// 获取过滤后符合条件的文件路径
        /// </summary>
        private async Task<List<FilePathModel>> GetFilteredFilePathAsync(string filePathSearchText, List<FilePathModel> filePathList)
        {
            if (filePathList is null || filePathList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<FilePathModel> filteredFilePathList = [];

                if (string.IsNullOrEmpty(filePathSearchText))
                {
                    foreach (FilePathModel filePathItem in filePathList)
                    {
                        filteredFilePathList.Add(filePathItem);
                    }
                }
                else
                {
                    foreach (FilePathModel filePathItem in filePathList)
                    {
                        if (filePathItem.Key.Contains(filePathSearchText) || filePathItem.AbsolutePath.Contains(filePathSearchText))
                        {
                            filteredFilePathList.Add(filePathItem);
                        }
                    }
                }

                return filteredFilePathList;
            });
        }

        /// <summary>
        /// 获取过滤后符合条件的嵌入的数据
        /// </summary>
        private async Task<List<EmbeddedDataModel>> GetFilteredEmbeddedDataAsync(string embeddedDataSearchText, List<EmbeddedDataModel> embeddedDataList)
        {
            if (embeddedDataList is null || embeddedDataList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<EmbeddedDataModel> filteredEmbeddedDataList = [];

                if (string.IsNullOrEmpty(embeddedDataSearchText))
                {
                    foreach (EmbeddedDataModel embeddedDataItem in embeddedDataList)
                    {
                        filteredEmbeddedDataList.Add(embeddedDataItem);
                    }
                }
                else
                {
                    foreach (EmbeddedDataModel embeddedDataItem in embeddedDataList)
                    {
                        if (embeddedDataItem.Key.Contains(embeddedDataSearchText))
                        {
                            filteredEmbeddedDataList.Add(embeddedDataItem);
                        }
                    }
                }

                return filteredEmbeddedDataList;
            });
        }

        /// <summary>
        /// 更新字符串结果
        /// </summary>
        private void UpdateStringResult(List<StringModel> filteredStringList)
        {
            StringCollection.Clear();
            if (filteredStringList is not null && filteredStringList.Count > 0)
            {
                foreach (StringModel stringItem in filteredStringList)
                {
                    StringCollection.Add(stringItem);
                }
            }
        }

        /// <summary>
        /// 更新文件路径结果
        /// </summary>
        private void UpdateFilePathResult(List<FilePathModel> filteredFilePathList)
        {
            FilePathCollection.Clear();
            if (filteredFilePathList is not null && filteredFilePathList.Count > 0)
            {
                foreach (FilePathModel filePathItem in filteredFilePathList)
                {
                    FilePathCollection.Add(filePathItem);
                }
            }
        }

        /// <summary>
        /// 更新嵌入的数据结果
        /// </summary>
        private void UpdateEmbeddedDataResult(List<EmbeddedDataModel> filteredEmbeddedDataList)
        {
            EmbeddedDataCollection.Clear();
            if (filteredEmbeddedDataList is not null && filteredEmbeddedDataList.Count > 0)
            {
                foreach (EmbeddedDataModel embeddedDataItem in filteredEmbeddedDataList)
                {
                    EmbeddedDataCollection.Add(embeddedDataItem);
                }
            }
        }

        /// <summary>
        /// 获取要准备复制的字符串内容
        /// </summary>
        private async Task<string> GetStringCopyListAsync(List<StringModel> copyStringList)
        {
            if (copyStringList is null || copyStringList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    StringBuilder copyStringBuilder = new();
                    foreach (StringModel stringItem in copyStringList)
                    {
                        copyStringBuilder.AppendLine(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                    }
                    return Convert.ToString(copyStringBuilder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(GetStringCopyListAsync), 1, e);
                    return null;
                }
            });
        }

        /// <summary>
        /// 获取要准备复制的文件路径内容
        /// </summary>
        private async Task<string> GetFilePathCopyListAsync(List<FilePathModel> copyFilePathList)
        {
            if (copyFilePathList is null || copyFilePathList.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    StringBuilder copyFilePathBuilder = new();
                    foreach (FilePathModel filePathItem in copyFilePathList)
                    {
                        copyFilePathBuilder.AppendLine(string.Format("Key:{0}, AbsolutePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                    }
                    return Convert.ToString(copyFilePathBuilder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(GetFilePathCopyListAsync), 1, e);
                    return null;
                }
            });
        }

        /// <summary>
        /// 保存字符串数据
        /// </summary>
        private void SaveStringData(string saveFolder, string fileName, string key, string content)
        {
            if (string.IsNullOrEmpty(saveFolder) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(content))
            {
                return;
            }

            try
            {
                File.AppendAllText(Path.Combine(saveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", key, content, Environment.NewLine));
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(SaveStringData), 1, e);
            }
        }

        /// <summary>
        /// 保存文件路径数据
        /// </summary>
        private void SvaeFilePathData(string saveFolder, string fileName, string key, string absolutePath)
        {
            if (string.IsNullOrEmpty(saveFolder) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(absolutePath))
            {
                return;
            }

            try
            {
                File.AppendAllText(Path.Combine(saveFolder, fileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", key, absolutePath, Environment.NewLine));
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(SvaeFilePathData), 1, e);
            }
        }

        /// <summary>
        /// 保存嵌入文件数据
        /// </summary>
        private void SaveEmbeddedData(string saveFolder, string fileName, byte[] data)
        {
            if (string.IsNullOrEmpty(saveFolder) || string.IsNullOrEmpty(fileName) || data is null)
            {
                return;
            }

            try
            {
                File.WriteAllBytes(Path.Combine(saveFolder, fileName), data);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(SaveEmbeddedData), 7, e);
            }
        }

        /// <summary>
        /// 保存嵌入数据列表到本地文件中
        /// </summary>
        private async Task SaveEmbeddedDataListAsync(string folderPath, List<EmbeddedDataModel> embeddedDataList)
        {
            if (string.IsNullOrEmpty(folderPath) && embeddedDataList is null || embeddedDataList.Count is 0)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    foreach (EmbeddedDataModel embeddedDataItem in embeddedDataList)
                    {
                        File.WriteAllBytes(Path.Combine(folderPath, Path.GetFileName(embeddedDataItem.Key)), embeddedDataItem.EmbeddedData);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(SaveEmbeddedDataListAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 打开保存嵌入的数据所对应的文件路径
        /// </summary>
        private async Task OpenEmbeddedDataSavedFolderPathAsync(string folderPath, string embeddedDataKey)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(embeddedDataKey))
                    {
                        Process.Start(folderPath);
                    }
                    else
                    {
                        nint pidlList = Shell32Library.ILCreateFromPath(Path.Combine(folderPath, Path.GetFileName(embeddedDataKey)));
                        if (pidlList is not 0)
                        {
                            Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
                            Shell32Library.ILFree(pidlList);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(OnEmbeddedDataExecuteRequested), 1, e);
                }
            });
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(GetDragDropSelectedFilesAsync), 1, e);
                }

                return fileList;
            });
        }
    }
}
