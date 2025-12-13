using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
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
    public sealed partial class PriExtractPage : Page, INotifyPropertyChanged
    {
        private readonly string AllLanguageString = ResourceService.PriExtractResource.GetString("AllLanguage");
        private readonly string DragOverContentString = ResourceService.PriExtractResource.GetString("DragOverContent");
        private readonly string EmbeddedDataString = ResourceService.PriExtractResource.GetString("EmbeddedData");
        private readonly string FilePathString = ResourceService.PriExtractResource.GetString("FilePath");
        private readonly string FilterConditionString = ResourceService.PriExtractResource.GetString("FilterCondition");
        private readonly string GetResultsString = ResourceService.PriExtractResource.GetString("GetResults");
        private readonly string NoMultiFileString = ResourceService.PriExtractResource.GetString("NoMultiFile");
        private readonly string NoOtherExtensionNameFileString = ResourceService.PriExtractResource.GetString("NoOtherExtensionNameFile");
        private readonly string NoSelectedFileString = ResourceService.PriExtractResource.GetString("NoSelectedFile");
        private readonly string ProcessingNowString = ResourceService.PriExtractResource.GetString("ProcessNowString");
        private readonly string SelectedFolderString = ResourceService.PriExtractResource.GetString("SelectedFolder");
        private readonly string SelectFileString = ResourceService.PriExtractResource.GetString("SelectFile");
        private readonly string SelectFolderString = ResourceService.PriExtractResource.GetString("SelectFolder");
        private readonly string StringString = ResourceService.PriExtractResource.GetString("String");
        private bool isStringAllSelect = false;
        private bool isFilePathAllSelect = false;
        private bool isEmbeddedDataAllSelect = false;
        private bool isLoadCompleted = false;
        private string stringFileName;
        private string filePathFileName;

        private bool _isExtractSaveSame;

        public bool IsExtractSaveSame
        {
            get { return _isExtractSaveSame; }

            set
            {
                if (!Equals(_isExtractSaveSame, value))
                {
                    _isExtractSaveSame = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveSame)));
                }
            }
        }

        private bool _isExtractSaveString;

        public bool IsExtractSaveString
        {
            get { return _isExtractSaveString; }

            set
            {
                if (!Equals(_isExtractSaveString, value))
                {
                    _isExtractSaveString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveString)));
                }
            }
        }

        private bool _isExtractSaveFilePath;

        public bool IsExtractSaveFilePath
        {
            get { return _isExtractSaveFilePath; }

            set
            {
                if (!Equals(_isExtractSaveFilePath, value))
                {
                    _isExtractSaveFilePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveFilePath)));
                }
            }
        }

        private bool _isExtractSaveEmbeddedData;

        public bool IsExtractSaveEmbeddedData
        {
            get { return _isExtractSaveEmbeddedData; }

            set
            {
                if (!Equals(_isExtractSaveEmbeddedData, value))
                {
                    _isExtractSaveEmbeddedData = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExtractSaveEmbeddedData)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedLanguage;

        public KeyValuePair<string, string> SelectedLanguage
        {
            get { return _selectedLanguage; }

            set
            {
                if (!Equals(_selectedLanguage, value))
                {
                    _selectedLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLanguage)));
                }
            }
        }

        private bool _isProcessing;

        public bool IsProcessing
        {
            get { return _isProcessing; }

            set
            {
                if (!Equals(_isProcessing, value))
                {
                    _isProcessing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessing)));
                }
            }
        }

        private string _getResults;

        public string GetResults
        {
            get { return _getResults; }

            set
            {
                if (!string.Equals(_getResults, value))
                {
                    _getResults = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GetResults)));
                }
            }
        }

        private string _selectedSaveFolder;

        public string SelectedSaveFolder
        {
            get { return _selectedSaveFolder; }

            set
            {
                if (!string.Equals(_selectedSaveFolder, value))
                {
                    _selectedSaveFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSaveFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedResourceCandidateKind;

        public KeyValuePair<string, string> SelectedResourceCandidateKind
        {
            get { return _selectedResourceCandidateKind; }

            set
            {
                if (!Equals(_selectedResourceCandidateKind, value))
                {
                    _selectedResourceCandidateKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedResourceCandidateKind)));
                }
            }
        }

        private bool _hasStringResource;

        public bool HasStringResource
        {
            get { return _hasStringResource; }

            set
            {
                if (!Equals(_hasStringResource, value))
                {
                    _hasStringResource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasStringResource)));
                }
            }
        }

        private string _stringSearchText;

        public string StringSearchText
        {
            get { return _stringSearchText; }

            set
            {
                if (!string.Equals(_stringSearchText, value))
                {
                    _stringSearchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringSearchText)));
                }
            }
        }

        private bool _hasFilePathResource;

        public bool HasFilePathResource
        {
            get { return _hasFilePathResource; }

            set
            {
                if (!Equals(_hasFilePathResource, value))
                {
                    _hasFilePathResource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasFilePathResource)));
                }
            }
        }

        private string _filePathSearchText;

        public string FilePathSearchText
        {
            get { return _filePathSearchText; }

            set
            {
                if (!string.Equals(_filePathSearchText, value))
                {
                    _filePathSearchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePathSearchText)));
                }
            }
        }

        private bool _hasEmbeddedDataResource;

        public bool HasEmbeddedDataResource
        {
            get { return _hasEmbeddedDataResource; }

            set
            {
                if (!Equals(_hasEmbeddedDataResource, value))
                {
                    _hasEmbeddedDataResource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasEmbeddedDataResource)));
                }
            }
        }

        private string _embeddedDataSearchText;

        public string EmbeddedDataSearchText
        {
            get { return _embeddedDataSearchText; }

            set
            {
                if (!string.Equals(_embeddedDataSearchText, value))
                {
                    _embeddedDataSearchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmbeddedDataSearchText)));
                }
            }
        }

        private List<KeyValuePair<string, string>> ResourceCandidateKindList { get; } = [];

        private List<StringModel> StringList { get; } = [];

        private List<FilePathModel> FilePathList { get; } = [];

        private List<EmbeddedDataModel> EmbeddedDataList { get; } = [];

        private WinRTObservableCollection<LanguageModel> LanguageCollection { get; } = [];

        private WinRTObservableCollection<StringModel> StringCollection { get; } = [];

        private WinRTObservableCollection<FilePathModel> FilePathCollection { get; } = [];

        private WinRTObservableCollection<EmbeddedDataModel> EmbeddedDataCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public PriExtractPage()
        {
            InitializeComponent();
            GetResults = NoSelectedFileString;
            ResourceCandidateKindList.Add(new KeyValuePair<string, string>("String", StringString));
            ResourceCandidateKindList.Add(new KeyValuePair<string, string>("FilePath", FilePathString));
            ResourceCandidateKindList.Add(new KeyValuePair<string, string>("EmbeddedData", EmbeddedDataString));
            SelectedResourceCandidateKind = ResourceCandidateKindList[0];
            Shell32Library.SHGetKnownFolderPath(new("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, 0, out string downloadFolder);
            SelectedSaveFolder = downloadFolder;
        }

        #region 第一部分：重写父类事件

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
                    IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                    if (dragItemsList.Count is 1)
                    {
                        string extensionName = Path.GetExtension(dragItemsList[0].Name);

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
                DataPackageView dataPackageView = args.DataView;
                IReadOnlyList<IStorageItem> filesList = await Task.Run(async () =>
                {
                    try
                    {
                        if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                        {
                            return await dataPackageView.GetStorageItemsAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(OnDrop), 1, e);
                    }

                    return null;
                });

                if (filesList is not null && filesList.Count is 1)
                {
                    filePath = filesList[0].Path;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(OnDrop), 2, e);
                return;
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                await ParseResourceFileAsync(filePath);
            }
            else
            {
                IsProcessing = false;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 选择语言
        /// </summary>
        private async void OnLanguageExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (LanguageFlyout.IsOpen)
            {
                LanguageFlyout.Hide();
            }

            if (args.Parameter is LanguageModel language && isLoadCompleted)
            {
                foreach (LanguageModel languageItem in LanguageCollection)
                {
                    languageItem.IsChecked = false;
                    if (string.Equals(language.LanguageInfo.Key, languageItem.LanguageInfo.Key))
                    {
                        SelectedLanguage = languageItem.LanguageInfo;
                        languageItem.IsChecked = true;
                    }
                }

                await GetFilteredStringAsync();
            }
        }

        /// <summary>
        /// 复制字符串到剪贴板
        /// </summary>
        private async void OnStringExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is StringModel stringItem)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制文件路径到剪贴板
        /// </summary>
        private async void OnFilePathExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is FilePathModel filePath)
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(string.Format("Key:{0}, FilePath:{1}", filePath.Key, filePath.AbsolutePath));
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
                    await Task.Run(() =>
                    {
                        try
                        {
                            File.WriteAllBytes(Path.Combine(openFolderDialog.SelectedPath, Path.GetFileName(embeddedData.Key)), embeddedData.EmbeddedData);

                            nint pidlList = Shell32Library.ILCreateFromPath(Path.Combine(openFolderDialog.SelectedPath, Path.GetFileName(embeddedData.Key)));
                            if (pidlList is not 0)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
                                Shell32Library.ILFree(pidlList);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(OnEmbeddedDataExecuteRequested), 1, e);
                        }
                    });

                    openFolderDialog.Dispose();
                }
                else
                {
                    openFolderDialog.Dispose();
                }

                IsProcessing = false;
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：包资源索引提取——挂载的事件

        /// <summary>
        /// 单击时修改字符串项选中值
        /// </summary>
        private void OnStringItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is StringModel stringItem)
            {
                stringItem.IsSelected = !stringItem.IsSelected;
            }
        }

        /// <summary>
        /// 单击时修改文件路径项选中值
        /// </summary>
        private void OnFilePathItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is FilePathModel filePath)
            {
                filePath.IsSelected = !filePath.IsSelected;
            }
        }

        /// <summary>
        /// 单击时修改嵌入的数据项选中值
        /// </summary>
        private void OnEmbeddedDataItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is EmbeddedDataModel embeddedData)
            {
                embeddedData.IsSelected = !embeddedData.IsSelected;
            }
        }

        /// <summary>
        /// 字符串列表全选和全部不选
        /// </summary>
        private void OnStringSelectClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (!isStringAllSelect)
            {
                isStringAllSelect = true;

                foreach (StringModel stringItem in StringCollection)
                {
                    stringItem.IsSelected = true;
                }
            }
            else
            {
                isStringAllSelect = false;

                foreach (StringModel stringItem in StringCollection)
                {
                    stringItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 文件路径列表全选和全部不选
        /// </summary>
        private void OnFilePathSelectClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (!isFilePathAllSelect)
            {
                isFilePathAllSelect = true;

                foreach (FilePathModel filePathItem in FilePathCollection)
                {
                    filePathItem.IsSelected = true;
                }
            }
            else
            {
                isFilePathAllSelect = false;

                foreach (FilePathModel filePathItem in FilePathCollection)
                {
                    filePathItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 嵌入数据列表全选和全部不选
        /// </summary>
        private void OnEmbeddedDataSelectClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (!isEmbeddedDataAllSelect)
            {
                isEmbeddedDataAllSelect = true;

                foreach (EmbeddedDataModel embeddedDataItem in EmbeddedDataCollection)
                {
                    embeddedDataItem.IsSelected = true;
                }
            }
            else
            {
                isEmbeddedDataAllSelect = false;

                foreach (EmbeddedDataModel embeddedDataItem in EmbeddedDataCollection)
                {
                    embeddedDataItem.IsSelected = false;
                }
            }
        }

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
        /// 语言选择菜单打开时自动定位到选中项
        /// </summary>
        private void OnOpened(object sender, object args)
        {
            foreach (LanguageModel languageItem in LanguageCollection)
            {
                if (languageItem.IsChecked)
                {
                    LanguageListView.ScrollIntoView(languageItem);
                    break;
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
        private void OnSelectedResourceCandidateKindClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> resourceCandidateKind)
            {
                SelectedResourceCandidateKind = resourceCandidateKind;
            }
        }

        /// <summary>
        /// 搜索字符串
        /// </summary>
        private async void OnStringQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await GetFilteredStringAsync();
        }

        /// <summary>
        /// 字符串内容发生变化时触发的事件
        /// </summary>
        private async void OnStringTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            StringSearchText = sender.Text;
            if (string.IsNullOrEmpty(StringSearchText))
            {
                await GetFilteredStringAsync();
            }
        }

        /// <summary>
        /// 搜索文件路径
        /// </summary>
        private async void OnFilePathQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await GetFilteredFilePathAsync();
        }

        /// <summary>
        /// 文件内容发生变化时触发的事件
        /// </summary>
        private async void OnFilePathTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            FilePathSearchText = sender.Text;
            if (string.IsNullOrEmpty(FilePathSearchText))
            {
                await GetFilteredFilePathAsync();
            }
        }

        /// <summary>
        /// 搜索嵌入的数据
        /// </summary>
        private async void OnEmbeddedDataQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await GetFilteredEmbeddedDataAsync();
        }

        /// <summary>
        /// 嵌入的数据发生变化时触发的事件
        /// </summary>
        private async void OnEmbeddedDataTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            EmbeddedDataSearchText = sender.Text;
            if (string.IsNullOrEmpty(EmbeddedDataSearchText))
            {
                await GetFilteredEmbeddedDataAsync();
            }
        }

        /// <summary>
        /// 复制选中的字符串
        /// </summary>
        private async void OnCopySelectedStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<StringModel> selectedStringList = [.. StringCollection.Where(item => item.IsSelected)];
            if (selectedStringList.Count > 0)
            {
                StringBuilder copyStringBuilder = new();
                foreach (StringModel stringItem in selectedStringList)
                {
                    copyStringBuilder.AppendLine(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(copyStringBuilder));
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
            List<FilePathModel> selectedFilePathList = [.. FilePathCollection.Where(item => item.IsSelected)];
            if (selectedFilePathList.Count > 0)
            {
                StringBuilder copyFilePathBuilder = new();
                foreach (FilePathModel filePathItem in selectedFilePathList)
                {
                    copyFilePathBuilder.AppendLine(string.Format("Key:{0}, AbsolutePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(copyFilePathBuilder));
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
            List<EmbeddedDataModel> selectedEmbeddedDataList = [.. EmbeddedDataCollection.Where(item => item.IsSelected)];
            if (selectedEmbeddedDataList.Count > 0)
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
                    await Task.Run(() =>
                    {
                        try
                        {
                            foreach (EmbeddedDataModel embeddedDataItem in selectedEmbeddedDataList)
                            {
                                File.WriteAllBytes(Path.Combine(openFolderDialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), embeddedDataItem.EmbeddedData);
                            }

                            Process.Start(openFolderDialog.SelectedPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(OnExportSelectedEmbeddedDataClicked), 1, e);
                        }
                    });

                    openFolderDialog.Dispose();
                    IsProcessing = false;
                }
                else
                {
                    IsProcessing = false;
                    openFolderDialog.Dispose();
                }
            }
        }

        /// <summary>
        /// 复制所有的字符串
        /// </summary>
        private async void OnCopyAllStringClicked(object sender, RoutedEventArgs args)
        {
            IsProcessing = true;
            List<StringModel> copyAllStringList = [.. StringCollection];
            if (copyAllStringList.Count > 0)
            {
                StringBuilder copyStringBuilder = new();
                foreach (StringModel stringItem in copyAllStringList)
                {
                    copyStringBuilder.AppendLine(string.Format("Key:{0}, Content:{1}", stringItem.Key, stringItem.Content));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(copyStringBuilder));
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
            List<FilePathModel> copyAllFilePathList = [.. FilePathCollection];
            if (copyAllFilePathList.Count > 0)
            {
                StringBuilder copyFilePathBuilder = new();
                foreach (FilePathModel filePathItem in copyAllFilePathList)
                {
                    copyFilePathBuilder.AppendLine(string.Format("Key:{0}, AbsolutePath:{1}", filePathItem.Key, filePathItem.AbsolutePath));
                }
                bool copyResult = CopyPasteHelper.CopyToClipboard(Convert.ToString(copyFilePathBuilder));
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
            OpenFolderDialog openFolderDialog = new((nint)MainWindow.Current.AppWindow.Id.Value)
            {
                Description = SelectFolderString,
                RootFolder = Environment.SpecialFolder.Desktop
            };

            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
            {
                IsProcessing = true;
                List<EmbeddedDataModel> exportAllEmbeddedDataList = [.. EmbeddedDataCollection];
                if (exportAllEmbeddedDataList.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            foreach (EmbeddedDataModel embeddedDataItem in exportAllEmbeddedDataList)
                            {
                                File.WriteAllBytes(Path.Combine(openFolderDialog.SelectedPath, Path.GetFileName(embeddedDataItem.Key)), embeddedDataItem.EmbeddedData);
                            }

                            Process.Start(openFolderDialog.SelectedPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(OnExportAllEmbeddedDataClicked), 1, e);
                        }
                    });
                }

                openFolderDialog.Dispose();
                IsProcessing = false;
            }
            else
            {
                openFolderDialog.Dispose();
            }
        }

        #endregion 第三部分：包资源索引提取——挂载的事件

        /// <summary>
        /// 解析 PRI 资源文件
        /// </summary>
        public async Task ParseResourceFileAsync(string filePath)
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
            List<string> languageList = [];

            bool result = await Task.Run(() =>
            {
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
                        tocList.Add(new TocEntry()
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
                                                                if (IsExtractSaveFilePath)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, filePathFileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", key, absolutePath, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 1, e);
                                                                    }
                                                                }

                                                                FilePathList.Add(new FilePathModel()
                                                                {
                                                                    Key = key,
                                                                    IsSelected = false,
                                                                    AbsolutePath = string.IsNullOrEmpty(absolutePath) ? string.Empty : absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // ASCII 格式字符串内容
                                                        case ResourceValueType.AsciiString:
                                                            {
                                                                string content = Encoding.ASCII.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (IsExtractSaveString)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", key, content, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 2, e);
                                                                    }
                                                                }

                                                                StringList.Add(new StringModel()
                                                                {
                                                                    Key = key,
                                                                    Content = string.IsNullOrEmpty(content) ? string.Empty : content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                    IsSelected = false
                                                                });
                                                                break;
                                                            }
                                                        // UTF8 格式路径内容
                                                        case ResourceValueType.Utf8Path:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.UTF8.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (IsExtractSaveFilePath)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, filePathFileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", key, absolutePath, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 3, e);
                                                                    }
                                                                }

                                                                FilePathList.Add(new FilePathModel()
                                                                {
                                                                    Key = key,
                                                                    IsSelected = false,
                                                                    AbsolutePath = string.IsNullOrEmpty(absolutePath) ? string.Empty : absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // UTF8 格式字符串内容
                                                        case ResourceValueType.Utf8String:
                                                            {
                                                                string content = Encoding.UTF8.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (IsExtractSaveString)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", key, content, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 4, e);
                                                                    }
                                                                }

                                                                StringList.Add(new StringModel()
                                                                {
                                                                    Key = key,
                                                                    Content = string.IsNullOrEmpty(content) ? string.Empty : content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                    IsSelected = false
                                                                });
                                                                break;
                                                            }
                                                        // Unicode 格式路径内容
                                                        case ResourceValueType.UnicodePath:
                                                            {
                                                                string absolutePath = Path.Combine(Path.GetDirectoryName(filePath), Encoding.Unicode.GetString(data).TrimEnd('\0'));

                                                                // 自动保存文件路径
                                                                if (IsExtractSaveFilePath)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, filePathFileName), string.Format("Key: {0} - AbsolutePath:{1}{2}", key, absolutePath, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 5, e);
                                                                    }
                                                                }

                                                                FilePathList.Add(new FilePathModel()
                                                                {
                                                                    Key = key,
                                                                    IsSelected = false,
                                                                    AbsolutePath = string.IsNullOrEmpty(absolutePath) ? string.Empty : absolutePath,
                                                                });
                                                                break;
                                                            }
                                                        // Unicode 格式字符串内容
                                                        case ResourceValueType.UnicodeString:
                                                            {
                                                                string content = Encoding.Unicode.GetString(data).TrimEnd('\0');

                                                                // 自动保存字符串
                                                                if (IsExtractSaveString)
                                                                {
                                                                    try
                                                                    {
                                                                        File.AppendAllText(Path.Combine(SelectedSaveFolder, stringFileName), string.Format("Key: {0} - Content:{1}{2}", key, content, Environment.NewLine));
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 6, e);
                                                                    }
                                                                }

                                                                StringList.Add(new StringModel()
                                                                {
                                                                    Key = key,
                                                                    Content = string.IsNullOrEmpty(content) ? string.Empty : content,
                                                                    Language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : string.Empty,
                                                                    IsSelected = false
                                                                });
                                                                break;
                                                            }
                                                        case ResourceValueType.EmbeddedData:
                                                            {
                                                                // 自动保存资源文件嵌入数据
                                                                if (IsExtractSaveEmbeddedData)
                                                                {
                                                                    try
                                                                    {
                                                                        File.WriteAllBytes(Path.Combine(SelectedSaveFolder, Path.GetFileName(key)), data);
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 7, e);
                                                                    }
                                                                }

                                                                EmbeddedDataList.Add(new EmbeddedDataModel()
                                                                {
                                                                    Key = key,
                                                                    EmbeddedData = data,
                                                                    IsSelected = false,
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
                    languageList.AddRange(StringList.Select(item => item.Language).Distinct());
                    languageList.Sort();
                    StringList.Sort();
                    FilePathList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));
                    EmbeddedDataList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));

                    priBinaryReader.Dispose();
                    priFileStream.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(PriExtractPage), nameof(ParseResourceFileAsync), 8, e);
                    return false;
                }
            });

            if (result)
            {
                LanguageCollection.Add(new LanguageModel()
                {
                    IsChecked = true,
                    LanguageInfo = new KeyValuePair<string, string>("AllLanguage", AllLanguageString)
                });

                // 显示获取到的所有内容
                foreach (string languageItem in languageList)
                {
                    CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageItem);
                    LanguageCollection.Add(new LanguageModel()
                    {
                        IsChecked = false,
                        LanguageInfo = new KeyValuePair<string, string>(cultureInfo.Name, string.Format("{0}[{1}]", cultureInfo.DisplayName, languageItem))
                    });
                }

                SelectedLanguage = LanguageCollection[0].LanguageInfo;
                HasStringResource = StringList.Count > 0;
                HasFilePathResource = FilePathList.Count > 0;
                HasEmbeddedDataResource = EmbeddedDataList.Count > 0;
                await GetFilteredStringAsync();
                await GetFilteredFilePathAsync();
                await GetFilteredEmbeddedDataAsync();
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
        /// 获取过滤后符合条件的字符串
        /// </summary>
        private async Task GetFilteredStringAsync()
        {
            isStringAllSelect = false;
            foreach (StringModel stringItem in StringList)
            {
                stringItem.IsSelected = false;
            }

            // 所有语言
            List<StringModel> filteredStringList = await Task.Run(() =>
            {
                List<StringModel> filteredStringList = [];

                if (Equals(SelectedLanguage, LanguageCollection[0].LanguageInfo))
                {
                    if (string.IsNullOrEmpty(StringSearchText))
                    {
                        foreach (StringModel stringItem in StringList)
                        {
                            filteredStringList.Add(stringItem);
                        }
                    }
                    else
                    {
                        foreach (StringModel stringItem in StringList)
                        {
                            if (stringItem.Key.Contains(StringSearchText) || stringItem.Content.Contains(StringSearchText))
                            {
                                filteredStringList.Add(stringItem);
                            }
                        }
                    }
                }
                // 某一语言
                else
                {
                    List<StringModel> coincidentStringList = [.. StringList.Where(item => string.Equals(item.Language, SelectedLanguage.Key, StringComparison.OrdinalIgnoreCase))];

                    if (string.IsNullOrEmpty(StringSearchText))
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
                            if (stringItem.Key.Contains(StringSearchText) || stringItem.Content.Contains(StringSearchText))
                            {
                                filteredStringList.Add(stringItem);
                            }
                        }
                    }
                }

                return filteredStringList;
            });

            StringCollection.Clear();
            foreach (StringModel stringItem in filteredStringList)
            {
                StringCollection.Add(stringItem);
            }
        }

        /// <summary>
        /// 获取过滤后符合条件的文件路径
        /// </summary>
        private async Task GetFilteredFilePathAsync()
        {
            isFilePathAllSelect = false;
            foreach (FilePathModel filePathItem in FilePathList)
            {
                filePathItem.IsSelected = false;
            }

            List<FilePathModel> filteredFilePathList = await Task.Run(() =>
            {
                List<FilePathModel> filteredFilePathList = [];

                if (string.IsNullOrEmpty(FilePathSearchText))
                {
                    foreach (FilePathModel filePathItem in FilePathList)
                    {
                        filteredFilePathList.Add(filePathItem);
                    }
                }
                else
                {
                    foreach (FilePathModel filePathItem in FilePathList)
                    {
                        if (filePathItem.Key.Contains(FilePathSearchText) || filePathItem.AbsolutePath.Contains(FilePathSearchText))
                        {
                            filteredFilePathList.Add(filePathItem);
                        }
                    }
                }

                return filteredFilePathList;
            });

            FilePathCollection.Clear();
            foreach (FilePathModel filePathItem in filteredFilePathList)
            {
                FilePathCollection.Add(filePathItem);
            }
        }

        /// <summary>
        /// 获取过滤后符合条件的嵌入的数据
        /// </summary>
        private async Task GetFilteredEmbeddedDataAsync()
        {
            isEmbeddedDataAllSelect = false;
            foreach (EmbeddedDataModel embeddedDataItem in EmbeddedDataList)
            {
                embeddedDataItem.IsSelected = false;
            }

            List<EmbeddedDataModel> filteredEmbeddedDataList = await Task.Run(() =>
            {
                List<EmbeddedDataModel> filteredEmbeddedDataList = [];

                if (string.IsNullOrEmpty(EmbeddedDataSearchText))
                {
                    foreach (EmbeddedDataModel embeddedDataItem in EmbeddedDataList)
                    {
                        filteredEmbeddedDataList.Add(embeddedDataItem);
                    }
                }
                else
                {
                    foreach (EmbeddedDataModel embeddedDataItem in EmbeddedDataList)
                    {
                        if (embeddedDataItem.Key.Contains(EmbeddedDataSearchText))
                        {
                            filteredEmbeddedDataList.Add(embeddedDataItem);
                        }
                    }
                }

                return filteredEmbeddedDataList;
            });

            EmbeddedDataCollection.Clear();
            foreach (EmbeddedDataModel embeddedDataItem in filteredEmbeddedDataList)
            {
                EmbeddedDataCollection.Add(embeddedDataItem);
            }
        }
    }
}
