using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.Hashing;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// 抑制 CA1822，CA2022，IDE0060 警告
#pragma warning disable CA1822,CA2022,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据校验页面
    /// </summary>
    public sealed partial class DataVerifyPage : Page, INotifyPropertyChanged
    {
        private readonly string ASCIIString = ResourceService.DataVerifyResource.GetString("ASCII");
        private readonly string BigEndianUnicodeString = ResourceService.DataVerifyResource.GetString("BigEndianUnicode");
        private readonly string Blake2bString = ResourceService.DataVerifyResource.GetString("Blake2b");
        private readonly string Blake3String = ResourceService.DataVerifyResource.GetString("Blake3");
        private readonly string ContentEmptyString = ResourceService.DataVerifyResource.GetString("ContentEmpty");
        private readonly string ContentInitializeString = ResourceService.DataVerifyResource.GetString("ContentInitialize");
        private readonly string ContentVerifyFailedString = ResourceService.DataVerifyResource.GetString("ContentVerifyFailed");
        private readonly string ContentVerifyPartSuccessfullyString = ResourceService.DataVerifyResource.GetString("ContentVerifyPartSuccessfully");
        private readonly string ContentVerifyWholeSuccessfullyString = ResourceService.DataVerifyResource.GetString("ContentVerifyWholeSuccessfully");
        private readonly string CRC32String = ResourceService.DataVerifyResource.GetString("CRC32");
        private readonly string CRC64String = ResourceService.DataVerifyResource.GetString("CRC64");
        private readonly string CustomString = ResourceService.DataVerifyResource.GetString("Custom");
        private readonly string DragOverContentString = ResourceService.DataVerifyResource.GetString("DragOverContent");
        private readonly string ED2KString = ResourceService.DataVerifyResource.GetString("ED2K");
        private readonly string FileInitializeString = ResourceService.DataVerifyResource.GetString("FileInitialize");
        private readonly string FileNotExistedString = ResourceService.DataVerifyResource.GetString("FileNotExisted");
        private readonly string FileNotSelectedString = ResourceService.DataVerifyResource.GetString("FileNotSelected");
        private readonly string FileVerifyFailedString = ResourceService.DataVerifyResource.GetString("FileVerifyFailed");
        private readonly string FileVerifyPartSuccessfullyString = ResourceService.DataVerifyResource.GetString("FileVerifyPartSuccessfully");
        private readonly string FileVerifyWholeSuccessfullyString = ResourceService.DataVerifyResource.GetString("FileVerifyWholeSuccessfully");
        private readonly string GB18030String = ResourceService.DataVerifyResource.GetString("GB18030");
        private readonly string GB2312String = ResourceService.DataVerifyResource.GetString("GB2312");
        private readonly string GBKString = ResourceService.DataVerifyResource.GetString("GBK");
        private readonly string Has160String = ResourceService.DataVerifyResource.GetString("Has160");
        private readonly string ISO88591String = ResourceService.DataVerifyResource.GetString("ISO88591");
        private readonly string MD2String = ResourceService.DataVerifyResource.GetString("MD2");
        private readonly string MD4String = ResourceService.DataVerifyResource.GetString("MD4");
        private readonly string MD5String = ResourceService.DataVerifyResource.GetString("MD5");
        private readonly string NoMultiFileString = ResourceService.DataVerifyResource.GetString("NoMultiFile");
        private readonly string RIPEMD160String = ResourceService.DataVerifyResource.GetString("RIPEMD160");
        private readonly string SelectFileString = ResourceService.DataVerifyResource.GetString("SelectFile");
        private readonly string SHA1String = ResourceService.DataVerifyResource.GetString("SHA1");
        private readonly string SHA224String = ResourceService.DataVerifyResource.GetString("SHA224");
        private readonly string SHA256String = ResourceService.DataVerifyResource.GetString("SHA256");
        private readonly string SHA3224String = ResourceService.DataVerifyResource.GetString("SHA3224");
        private readonly string SHA3256String = ResourceService.DataVerifyResource.GetString("SHA3256");
        private readonly string SHA3384String = ResourceService.DataVerifyResource.GetString("SHA3384");
        private readonly string SHA3512String = ResourceService.DataVerifyResource.GetString("SHA3512");
        private readonly string SHA384String = ResourceService.DataVerifyResource.GetString("SHA384");
        private readonly string SHA512String = ResourceService.DataVerifyResource.GetString("SHA512");
        private readonly string Shake128String = ResourceService.DataVerifyResource.GetString("Shake128");
        private readonly string Shake256String = ResourceService.DataVerifyResource.GetString("Shake256");
        private readonly string SM3String = ResourceService.DataVerifyResource.GetString("SM3");
        private readonly string TextEncodingInvalidString = ResourceService.DataVerifyResource.GetString("TextEncodingInvalid");
        private readonly string TIGERString = ResourceService.DataVerifyResource.GetString("TIGER");
        private readonly string TIGER2String = ResourceService.DataVerifyResource.GetString("TIGER2");
        private readonly string UnicodeString = ResourceService.DataVerifyResource.GetString("Unicode");
        private readonly string UnknownErrorString = ResourceService.DataVerifyResource.GetString("UnknownError");
        private readonly string UTF32String = ResourceService.DataVerifyResource.GetString("UTF32");
        private readonly string UTF7String = ResourceService.DataVerifyResource.GetString("UTF7");
        private readonly string UTF8String = ResourceService.DataVerifyResource.GetString("UTF8");
        private readonly string VerifyingString = ResourceService.DataVerifyResource.GetString("Verifying");
        private readonly string VerifyTypeNotSelectedString = ResourceService.DataVerifyResource.GetString("VerifyTypeNotSelected");
        private readonly string WhirlpoolString = ResourceService.DataVerifyResource.GetString("Whirlpool");
        private readonly string XXH32String = ResourceService.DataVerifyResource.GetString("XXH32");
        private readonly string XXH64String = ResourceService.DataVerifyResource.GetString("XXH64");
        private readonly string XXH128String = ResourceService.DataVerifyResource.GetString("XXH128");
        private int selectVerifyIndex = -1;
        private string selectedVerifyFile = string.Empty;
        private string selectedVerifyContent = string.Empty;

        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                }
            }
        }

        private string _verifyFile;

        public string VerifyFile
        {
            get { return _verifyFile; }

            set
            {
                if (!Equals(_verifyFile, value))
                {
                    _verifyFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerifyFile)));
                }
            }
        }

        private string _verifyContent;

        public string VerifyContent
        {
            get { return _verifyContent; }

            set
            {
                if (!Equals(_verifyContent, value))
                {
                    _verifyContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerifyContent)));
                }
            }
        }

        private InfoBarSeverity _resultSeverity;

        private InfoBarSeverity ResultSeverity
        {
            get { return _resultSeverity; }

            set
            {
                if (!Equals(_resultSeverity, value))
                {
                    _resultSeverity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultSeverity)));
                }
            }
        }

        private string _resultMessage;

        public string ResultMessage
        {
            get { return _resultMessage; }

            set
            {
                if (!string.Equals(_resultMessage, value))
                {
                    _resultMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultMessage)));
                }
            }
        }

        private bool _isAllSelected;

        public bool IsAllSelected
        {
            get { return _isAllSelected; }

            set
            {
                if (!Equals(_isAllSelected, value))
                {
                    _isAllSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAllSelected)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedTextEncodingType;

        public KeyValuePair<string, string> SelectedTextEncodingType
        {
            get { return _selectedTextEncodingType; }

            set
            {
                if (!Equals(_selectedTextEncodingType, value))
                {
                    _selectedTextEncodingType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTextEncodingType)));
                }
            }
        }

        private string _textEncodingCustomTypeText;

        public string TextEncodingCustomTypeText
        {
            get { return _textEncodingCustomTypeText; }

            set
            {
                if (!Equals(_textEncodingCustomTypeText, value))
                {
                    _textEncodingCustomTypeText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextEncodingCustomTypeText)));
                }
            }
        }

        private bool _useUpperCase;

        public bool UseUpperCase
        {
            get { return _useUpperCase; }

            set
            {
                if (!Equals(_useUpperCase, value))
                {
                    _useUpperCase = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseUpperCase)));
                }
            }
        }

        private bool _isVerifying;

        public bool IsVerifying
        {
            get { return _isVerifying; }

            set
            {
                if (!Equals(_isVerifying, value))
                {
                    _isVerifying = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVerifying)));
                }
            }
        }

        private List<KeyValuePair<string, string>> TextEncodingTypeList { get; } = [];

        private List<DataVerifyTypeModel> DataVerifyTypeList { get; } = [];

        private WinRTObservableCollection<DataVerifyResultModel> DataVerifyResultCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataVerifyPage()
        {
            InitializeComponent();
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.ASCII), ASCIIString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.BigEndianUnicode), BigEndianUnicodeString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("ISO-8859-1", ISO88591String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("GB18030", GB18030String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("GB2312", GB2312String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("GBK", GBKString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.Unicode), UnicodeString));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF32), UTF32String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF7), UTF7String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            TextEncodingTypeList.Add(new KeyValuePair<string, string>("Custom", CustomString));
            SelectedTextEncodingType = TextEncodingTypeList[9];
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = Blake2bString,
                DataVerifyType = DataVerifyType.Blake2b
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = Blake3String,
                DataVerifyType = DataVerifyType.Blake3
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = CRC32String,
                DataVerifyType = DataVerifyType.CRC_32
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = CRC64String,
                DataVerifyType = DataVerifyType.CRC_64
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = ED2KString,
                DataVerifyType = DataVerifyType.ED2K
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = Has160String,
                DataVerifyType = DataVerifyType.HAS160
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = MD2String,
                DataVerifyType = DataVerifyType.MD2
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = MD4String,
                DataVerifyType = DataVerifyType.MD4
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = MD5String,
                DataVerifyType = DataVerifyType.MD5
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = RIPEMD160String,
                DataVerifyType = DataVerifyType.RIPEMD_160
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA1String,
                DataVerifyType = DataVerifyType.SHA_1
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA224String,
                DataVerifyType = DataVerifyType.SHA_224
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA256String,
                DataVerifyType = DataVerifyType.SHA_256
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA384String,
                DataVerifyType = DataVerifyType.SHA_384
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA512String,
                DataVerifyType = DataVerifyType.SHA_512
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA3224String,
                DataVerifyType = DataVerifyType.SHA3_224
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA3256String,
                DataVerifyType = DataVerifyType.SHA3_256
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA3384String,
                DataVerifyType = DataVerifyType.SHA3_384
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SHA3512String,
                DataVerifyType = DataVerifyType.SHA3_512
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = Shake128String,
                DataVerifyType = DataVerifyType.Shake128
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = Shake256String,
                DataVerifyType = DataVerifyType.Shake256
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = SM3String,
                DataVerifyType = DataVerifyType.SM3
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = TIGERString,
                DataVerifyType = DataVerifyType.TIGER
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = TIGER2String,
                DataVerifyType = DataVerifyType.TIGER2
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = WhirlpoolString,
                DataVerifyType = DataVerifyType.WHIRLPOOL
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = XXH32String,
                DataVerifyType = DataVerifyType.XXH32
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = XXH64String,
                DataVerifyType = DataVerifyType.XXH64
            });
            DataVerifyTypeList.Add(new DataVerifyTypeModel()
            {
                Name = XXH128String,
                DataVerifyType = DataVerifyType.XXH128
            });
        }

        #region 第一部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 数据校验类型选中项发生改变时触发的事件
        /// </summary>
        private void OnDataVerifyCheckExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            IsAllSelected = DataVerifyTypeList.All(item => item.IsSelected);
        }

        #endregion 第一部分：ExecuteCommand 命令调用时挂载的事件

        #region 第二部分：数据校验页面——挂载的事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        private async void OnDataDragOver(object sender, Microsoft.UI.Xaml.DragEventArgs args)
        {
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                if (IsVerifying)
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = VerifyingString;
                }
                else
                {
                    IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                    if (dragItemsList.Count is 1)
                    {
                        string extensionName = Path.GetExtension(dragItemsList[0].Name);

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
                        args.DragUIOverride.Caption = NoMultiFileString;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(OnDragOver), 1, e);
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
        private async void OnDataDrop(object sender, Microsoft.UI.Xaml.DragEventArgs args)
        {
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(OnDrop), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                VerifyFile = filePath;
            }
        }

        /// <summary>
        /// 选中项发生改变时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is RadioButtons radioButtons && radioButtons.SelectedIndex >= 0)
            {
                SelectedIndex = radioButtons.SelectedIndex;
                if (SelectedIndex is 0 && ResultSeverity is InfoBarSeverity.Informational)
                {
                    ResultMessage = FileInitializeString;
                }
                else if (SelectedIndex is 1 && ResultSeverity is InfoBarSeverity.Informational)
                {
                    ResultMessage = ContentInitializeString;
                }
            }
        }

        /// <summary>
        /// 打开本地文件
        /// </summary>
        private void OnOpenLocalFileClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Title = SelectFileString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                VerifyFile = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 校验内容发生改变时触发的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                VerifyContent = textBox.Text;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DataVerifyTypeModel dataVerifyTypeItem in DataVerifyTypeList)
            {
                dataVerifyTypeItem.IsSelected = true;
            }

            IsAllSelected = true;
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (DataVerifyTypeModel dataVerifyTypeItem in DataVerifyTypeList)
            {
                dataVerifyTypeItem.IsSelected = false;
            }

            IsAllSelected = false;
        }

        /// <summary>
        /// 选择文字编码类型
        /// </summary>
        private void OnTextEncodingTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> textEncodingType)
            {
                SelectedTextEncodingType = textEncodingType;
            }
        }

        /// <summary>
        /// 文字自定义编码类型内容发生变化时触发的事件
        /// </summary>
        private void OnTextEncodingCustomTypeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                TextEncodingCustomTypeText = textBox.Text;
            }
        }

        /// <summary>
        /// 输出格式为大写字母
        /// </summary>
        private void OnUseUpperCaseChecked(object sender, RoutedEventArgs args)
        {
            foreach (DataVerifyResultModel dataVerifyResultItem in DataVerifyResultCollection)
            {
                dataVerifyResultItem.Result = dataVerifyResultItem.Result.ToUpperInvariant();
            }
        }

        /// <summary>
        /// 输出格式为小写字母
        /// </summary>
        private void OnUseUpperCaseUnchecked(object sender, RoutedEventArgs args)
        {
            foreach (DataVerifyResultModel dataVerifyResultItem in DataVerifyResultCollection)
            {
                dataVerifyResultItem.Result = dataVerifyResultItem.Result.ToLowerInvariant();
            }
        }

        /// <summary>
        /// 开始数据校验
        /// </summary>
        private async void OnStartVerifyClicked(object sender, RoutedEventArgs args)
        {
            selectVerifyIndex = SelectedIndex;
            selectedVerifyFile = VerifyFile;
            selectedVerifyContent = VerifyContent;
            if (selectVerifyIndex is 0 && string.IsNullOrEmpty(selectedVerifyFile))
            {
                ResultSeverity = InfoBarSeverity.Error;
                if (string.IsNullOrEmpty(selectedVerifyFile))
                {
                    ResultMessage = FileNotSelectedString;
                }
                else if (!File.Exists(selectedVerifyFile))
                {
                    ResultMessage = FileNotExistedString;
                }
                else
                {
                    ResultMessage = UnknownErrorString;
                }
                return;
            }
            else if (selectVerifyIndex is 1 && string.IsNullOrEmpty(selectedVerifyContent))
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = ContentEmptyString;
                return;
            }

            Encoding textEncoding = await Task.Run(() =>
            {
                Encoding textEncoding = null;
                if (Equals(SelectedTextEncodingType, TextEncodingTypeList[0]))
                {
                    textEncoding = Encoding.ASCII;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[1]))
                {
                    textEncoding = Encoding.BigEndianUnicode;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[6]))
                {
                    textEncoding = Encoding.Unicode;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[7]))
                {
                    textEncoding = Encoding.UTF32;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[8]))
                {
                    textEncoding = Encoding.UTF7;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[9]))
                {
                    textEncoding = Encoding.UTF8;
                }
                else if (Equals(SelectedTextEncodingType, TextEncodingTypeList[10]))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(TextEncodingCustomTypeText))
                        {
                            textEncoding = int.TryParse(TextEncodingCustomTypeText, out int textEncodingCustomType) ? Encoding.GetEncoding(textEncodingCustomType) : Encoding.GetEncoding(TextEncodingCustomTypeText);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(OnStartVerifyClicked), 1, e);
                    }
                }
                else
                {
                    textEncoding = Encoding.GetEncoding(SelectedTextEncodingType.Key);
                }
                return textEncoding;
            });

            if (textEncoding is null)
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = TextEncodingInvalidString;
                return;
            }

            List<DataVerifyTypeModel> selectedDataVerifyTypeList = [.. DataVerifyTypeList.Where(item => item.IsSelected)];
            if (selectedDataVerifyTypeList.Count is 0)
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = VerifyTypeNotSelectedString;
                return;
            }
            IsVerifying = true;
            ResultSeverity = InfoBarSeverity.Informational;
            ResultMessage = VerifyingString;
            DataVerifyResultCollection.Clear();
            List<DataVerifyResultModel> dataVerifyResultList = await Task.Run(async () =>
            {
                byte[] contentData = null;
                List<DataVerifyResultModel> dataVerifyResultList = [];

                try
                {
                    if (selectVerifyIndex is 0)
                    {
                        FileStream fileStream = File.OpenRead(selectedVerifyFile);
                        contentData = new byte[(int)fileStream.Length];
                        fileStream.Read(contentData, 0, contentData.Length);
                        fileStream.Dispose();
                    }
                    else if (selectVerifyIndex is 1)
                    {
                        contentData = textEncoding.GetBytes(selectedVerifyContent);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(OnStartVerifyClicked), 2, e);
                }

                if (contentData is not null && (selectVerifyIndex is 0 || selectVerifyIndex is 1))
                {
                    List<Task> verifyingTaskList = [];
                    object verifyingLock = new();
                    foreach (DataVerifyTypeModel dataVerifyTypeItem in selectedDataVerifyTypeList)
                    {
                        verifyingTaskList.Add(Task.Run(() =>
                        {
                            string verifyResultContent = GetVerifiedData(dataVerifyTypeItem.DataVerifyType, selectVerifyIndex, contentData);
                            if (!string.IsNullOrEmpty(verifyResultContent))
                            {
                                lock (verifyingLock)
                                {
                                    dataVerifyResultList.Add(new DataVerifyResultModel()
                                    {
                                        Name = dataVerifyTypeItem.Name,
                                        Result = verifyResultContent
                                    });
                                }
                            }
                        }));
                    }
                    await Task.WhenAll(verifyingTaskList);
                }

                dataVerifyResultList.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));
                return dataVerifyResultList;
            });

            if (UseUpperCase)
            {
                foreach (DataVerifyResultModel dataVerifyResultItem in dataVerifyResultList)
                {
                    dataVerifyResultItem.Result = dataVerifyResultItem.Result.ToUpperInvariant();
                    DataVerifyResultCollection.Add(dataVerifyResultItem);
                }
            }
            else
            {
                foreach (DataVerifyResultModel dataVerifyResultItem in dataVerifyResultList)
                {
                    dataVerifyResultItem.Result = dataVerifyResultItem.Result.ToLowerInvariant();
                    DataVerifyResultCollection.Add(dataVerifyResultItem);
                }
            }

            if (DataVerifyResultCollection.Count > 0)
            {
                ResultSeverity = InfoBarSeverity.Success;
                if (Equals(selectedDataVerifyTypeList.Count, DataVerifyResultCollection.Count))
                {
                    if (selectVerifyIndex is 0)
                    {
                        ResultMessage = string.Format(FileVerifyWholeSuccessfullyString, DataVerifyResultCollection.Count);
                    }
                    else if (selectVerifyIndex is 1)
                    {
                        ResultMessage = string.Format(ContentVerifyWholeSuccessfullyString, DataVerifyResultCollection.Count);
                    }
                }
                else
                {
                    if (selectVerifyIndex is 0)
                    {
                        ResultMessage = string.Format(FileVerifyPartSuccessfullyString, DataVerifyResultCollection.Count, selectedDataVerifyTypeList.Count - DataVerifyResultCollection.Count);
                    }
                    else if (selectVerifyIndex is 1)
                    {
                        ResultMessage = string.Format(ContentVerifyPartSuccessfullyString, DataVerifyResultCollection.Count, selectedDataVerifyTypeList.Count - DataVerifyResultCollection.Count);
                    }
                }
            }
            else
            {
                ResultSeverity = InfoBarSeverity.Error;
                if (selectVerifyIndex is 0)
                {
                    ResultMessage = FileVerifyFailedString;
                }
                else if (selectVerifyIndex is 1)
                {
                    ResultMessage = ContentVerifyFailedString;
                }
            }
            IsVerifying = false;
        }

        #endregion 第二部分：数据校验页面——挂载的事件

        /// <summary>
        /// 获取校验后的数据
        /// </summary>
        private string GetVerifiedData(DataVerifyType dataVerifyType, int selectedVerifyIndex, byte[] contentData)
        {
            string verifiedData = string.Empty;

            switch (dataVerifyType)
            {
                case DataVerifyType.Blake2b:
                    {
                        try
                        {
                            Blake2b blake2b = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = blake2b.ComputeHash(contentData);
                            }
                            blake2b.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.Blake2b) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.Blake3:
                    {
                        try
                        {
                            Blake3 blake3 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = blake3.ComputeHash(contentData);
                            }
                            blake3.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.Blake3) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.CRC_32:
                    {
                        try
                        {
                            Crc32 crc32 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = crc32.ComputeHash(contentData);
                            }
                            crc32.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.CRC_32) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.CRC_64:
                    {
                        try
                        {
                            Crc64 crc64 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = crc64.ComputeHash(contentData);
                            }
                            crc64.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = string.Format("{0:x}", BitConverter.ToUInt64(hashBytes, 0));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.CRC_64) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.ED2K:
                    {
                        try
                        {
                            ED2K ed2k = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = ed2k.ComputeHash(contentData);
                            }
                            ed2k.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.ED2K) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.HAS160:
                    {
                        try
                        {
                            Has160 has160 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = has160.ComputeHash(contentData);
                            }
                            has160.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.HAS160) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.MD2:
                    {
                        try
                        {
                            MD2 md2 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = md2.ComputeHash(contentData);
                            }
                            md2.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.MD2) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.MD4:
                    {
                        try
                        {
                            MD4 md4 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = md4.ComputeHash(contentData);
                            }
                            md4.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.MD4) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.MD5:
                    {
                        try
                        {
                            MD5 md5 = MD5.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = md5.ComputeHash(contentData);
                            }
                            md5.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.MD5) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.RIPEMD_160:
                    {
                        try
                        {
                            RIPEMD160 ripemd160 = RIPEMD160.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = ripemd160.ComputeHash(contentData);
                            }
                            ripemd160.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.RIPEMD_160) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA_1:
                    {
                        try
                        {
                            SHA1 sha1 = SHA1.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha1.ComputeHash(contentData);
                            }
                            sha1.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA_1) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA_224:
                    {
                        try
                        {
                            Sha224 sha224 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha224.ComputeHash(contentData);
                            }
                            sha224.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA_224) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA_256:
                    {
                        try
                        {
                            SHA256 sha256 = SHA256.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha256.ComputeHash(contentData);
                            }
                            sha256.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA_256) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA_384:
                    {
                        try
                        {
                            SHA384 sha384 = SHA384.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha384.ComputeHash(contentData);
                            }
                            sha384.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA_384) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA_512:
                    {
                        try
                        {
                            SHA512 sha512 = SHA512.Create();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha512.ComputeHash(contentData);
                            }
                            sha512.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA_512) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA3_224:
                    {
                        try
                        {
                            Sha3_224 sha3_224 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_224.ComputeHash(contentData);
                            }
                            sha3_224.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA3_224) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA3_256:
                    {
                        try
                        {
                            Sha3_256 sha3_256 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_256.ComputeHash(contentData);
                            }
                            sha3_256.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA3_256) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA3_384:
                    {
                        try
                        {
                            Sha3_384 sha3_384 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_384.ComputeHash(contentData);
                            }
                            sha3_384.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA3_384) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SHA3_512:
                    {
                        try
                        {
                            Sha3_512 sha3_512 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sha3_512.ComputeHash(contentData);
                            }
                            sha3_512.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SHA3_512) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.Shake128:
                    {
                        try
                        {
                            Shake128 shake128 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = shake128.ComputeHash(contentData);
                            }
                            shake128.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.Shake128) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.Shake256:
                    {
                        try
                        {
                            Shake256 shake256 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = shake256.ComputeHash(contentData);
                            }
                            shake256.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.Shake256) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.SM3:
                    {
                        try
                        {
                            SM3 sm3 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = sm3.ComputeHash(contentData);
                            }
                            sm3.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.SM3) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.TIGER:
                    {
                        try
                        {
                            Tiger tiger = new(1);
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = tiger.ComputeHash(contentData);
                            }
                            tiger.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.TIGER) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.TIGER2:
                    {
                        try
                        {
                            Tiger tiger = new(128);
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = tiger.ComputeHash(contentData);
                            }
                            tiger.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.TIGER2) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.WHIRLPOOL:
                    {
                        try
                        {
                            Whirlpool whirlpool = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = whirlpool.ComputeHash(contentData);
                            }
                            whirlpool.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.WHIRLPOOL) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.XXH32:
                    {
                        try
                        {
                            XxHash32 xxHash32 = new();
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = xxHash32.ComputeHash(contentData);
                            }
                            xxHash32.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.XXH32) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.XXH64:
                    {
                        try
                        {
                            XXHash64 xxhash64 = new(0);
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                hashBytes = xxhash64.ComputeHash(contentData);
                            }
                            xxhash64.Dispose();

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.XXH64) + 1, e);
                        }
                        break;
                    }
                case DataVerifyType.XXH128:
                    {
                        try
                        {
                            XxHash128 xxhash128 = new(0);
                            byte[] hashBytes = null;
                            if (contentData is not null)
                            {
                                xxhash128.Append(contentData);
                                hashBytes = xxhash128.GetCurrentHash();
                            }

                            if (hashBytes is not null)
                            {
                                verifiedData = Convert.ToString(BitConverter.ToString(hashBytes).Replace("-", string.Empty));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVerifyPage), nameof(GetVerifiedData), Convert.ToInt32(DataVerifyType.XXH128) + 1, e);
                        }
                        break;
                    }
            }

            return verifiedData;
        }

        /// <summary>
        /// 获取要校验的类型
        /// </summary>
        private Visibility GetDataVerifyType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取对应的文字编码类型
        /// </summary>
        private Visibility GetTextEncodingType(string textEncodingType, string comparedTextEncodingType)
        {
            return string.Equals(textEncodingType, comparedTextEncodingType) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取校验结果列表显示类型
        /// </summary>
        private Visibility GetVerifyResult(bool isVerifying, int verifyCount)
        {
            return isVerifying ? Visibility.Collapsed : verifyCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
