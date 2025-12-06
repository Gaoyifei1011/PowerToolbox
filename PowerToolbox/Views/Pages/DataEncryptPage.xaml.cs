using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.Encrypt;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// 数据加密页面
    /// </summary>
    public sealed partial class DataEncryptPage : Page, INotifyPropertyChanged
    {
        private readonly string AESString = ResourceService.DataEncryptResource.GetString("AES");
        private readonly string ANSIX923String = ResourceService.DataEncryptResource.GetString("ANSIX923");
        private readonly string Base64String = ResourceService.DataEncryptResource.GetString("Base64");
        private readonly string BlowfishString = ResourceService.DataEncryptResource.GetString("Blowfish");
        private readonly string CaesarCipherString = ResourceService.DataEncryptResource.GetString("CaesarCipher");
        private readonly string CBCString = ResourceService.DataEncryptResource.GetString("CBC");
        private readonly string CFBString = ResourceService.DataEncryptResource.GetString("CFB");
        private readonly string ChaCha20String = ResourceService.DataEncryptResource.GetString("ChaCha20");
        private readonly string ContentEncryptFailedString = ResourceService.DataEncryptResource.GetString("ContentEncryptFailed");
        private readonly string ContentEncryptSuccessfullyString = ResourceService.DataEncryptResource.GetString("ContentEncryptSuccessfully");
        private readonly string ContentEmptyString = ResourceService.DataEncryptResource.GetString("ContentEmpty");
        private readonly string ContentInitializeString = ResourceService.DataEncryptResource.GetString("ContentInitialize");
        private readonly string CTSString = ResourceService.DataEncryptResource.GetString("CTS");
        private readonly string DESString = ResourceService.DataEncryptResource.GetString("DES");
        private readonly string DragOverContentString = ResourceService.DataEncryptResource.GetString("DragOverContent");
        private readonly string ECBString = ResourceService.DataEncryptResource.GetString("ECB");
        private readonly string ECCString = ResourceService.DataEncryptResource.GetString("ECC");
        private readonly string EncryptingString = ResourceService.DataEncryptResource.GetString("Encrypting");
        private readonly string EncryptTypeNotSelectedString = ResourceService.DataEncryptResource.GetString("EncryptTypeNotSelected");
        private readonly string FileEncryptFailedString = ResourceService.DataEncryptResource.GetString("FileEncryptFailed");
        private readonly string FileEncryptSuccessfullyString = ResourceService.DataEncryptResource.GetString("FileEncryptSuccessfully");
        private readonly string FileInitializeString = ResourceService.DataEncryptResource.GetString("FileInitialize");
        private readonly string FileNotExistedString = ResourceService.DataEncryptResource.GetString("FileNotExisted");
        private readonly string FileNotSelectedString = ResourceService.DataEncryptResource.GetString("FileNotSelected");
        private readonly string ISO10126String = ResourceService.DataEncryptResource.GetString("ISO10126");
        private readonly string LargeContentString = ResourceService.DataEncryptResource.GetString("LargeContent");
        private readonly string MorseCodeString = ResourceService.DataEncryptResource.GetString("MorseCode");
        private readonly string NoMultiFileString = ResourceService.DataEncryptResource.GetString("NoMultiFile");
        private readonly string NonePaddingString = ResourceService.DataEncryptResource.GetString("NonePadding");
        private readonly string OFBString = ResourceService.DataEncryptResource.GetString("OFB");
        private readonly string PKCS7String = ResourceService.DataEncryptResource.GetString("PKCS7");
        private readonly string RabbitString = ResourceService.DataEncryptResource.GetString("Rabbit");
        private readonly string RC2String = ResourceService.DataEncryptResource.GetString("RC2");
        private readonly string RC4String = ResourceService.DataEncryptResource.GetString("RC4");
        private readonly string RC5String = ResourceService.DataEncryptResource.GetString("RC5");
        private readonly string RC6String = ResourceService.DataEncryptResource.GetString("RC6");
        private readonly string RijndaelString = ResourceService.DataEncryptResource.GetString("Rijndael");
        private readonly string RSAString = ResourceService.DataEncryptResource.GetString("RSA");
        private readonly string SelectFileString = ResourceService.DataEncryptResource.GetString("SelectFile");
        private readonly string SM2String = ResourceService.DataEncryptResource.GetString("SM2");
        private readonly string SM4String = ResourceService.DataEncryptResource.GetString("SM4");
        private readonly string StringLengthString = ResourceService.DataEncryptResource.GetString("StringLength");
        private readonly string TripleDESString = ResourceService.DataEncryptResource.GetString("TripleDES");
        private readonly string UTF8String = ResourceService.DataEncryptResource.GetString("UTF8");
        private readonly string XORString = ResourceService.DataEncryptResource.GetString("XOR");
        private readonly string ZerosString = ResourceService.DataEncryptResource.GetString("Zeros");
        private int selectEncryptIndex = -1;
        private string selectedEncryptFile = string.Empty;
        private string selectedEncryptContent = string.Empty;
        private string encryptedLocalFile = string.Empty;

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

        private string _encryptFile;

        public string EncryptFile
        {
            get { return _encryptFile; }

            set
            {
                if (!Equals(_encryptFile, value))
                {
                    _encryptFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptFile)));
                }
            }
        }

        private string _encryptContent;

        public string EncryptContent
        {
            get { return _encryptContent; }

            set
            {
                if (!Equals(_encryptContent, value))
                {
                    _encryptContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptContent)));
                }
            }
        }

        private InfoBarSeverity _resultSeverity;

        private InfoBarSeverity ResultServerity
        {
            get { return _resultSeverity; }

            set
            {
                if (!Equals(_resultSeverity, value))
                {
                    _resultSeverity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultServerity)));
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

        private DataEncryptTypeModel _selectedDataEncryptType;

        public DataEncryptTypeModel SelectedDataEncryptType
        {
            get { return _selectedDataEncryptType; }

            set
            {
                if (!Equals(_selectedDataEncryptType, value))
                {
                    _selectedDataEncryptType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDataEncryptType)));
                }
            }
        }

        private string _encryptKeyText = string.Empty;

        public string EncryptKeyText
        {
            get { return _encryptKeyText; }

            set
            {
                if (!string.Equals(_encryptKeyText, value))
                {
                    _encryptKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptKeyText)));
                }
            }
        }

        private string _initializationVectorText = string.Empty;

        public string InitializationVectorText
        {
            get { return _initializationVectorText; }

            set
            {
                if (!string.Equals(_initializationVectorText, value))
                {
                    _initializationVectorText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InitializationVectorText)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedEncryptKeyStringType;

        public KeyValuePair<string, string> SelectedEncryptKeyStringType
        {
            get { return _selectedEncryptKeyStringType; }

            set
            {
                if (!Equals(_selectedEncryptKeyStringType, value))
                {
                    _selectedEncryptKeyStringType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedEncryptKeyStringType)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedInitializationVectorStringType;

        public KeyValuePair<string, string> SelectedInitializationVectorStringType
        {
            get { return _selectedInitializationVectorStringType; }

            set
            {
                if (!Equals(_selectedInitializationVectorStringType, value))
                {
                    _selectedInitializationVectorStringType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedInitializationVectorStringType)));
                }
            }
        }

        private KeyValuePair<CipherMode, string> _selectedEncryptedBlockCipherMode;

        public KeyValuePair<CipherMode, string> SelectedEncryptedBlockCipherMode
        {
            get { return _selectedEncryptedBlockCipherMode; }

            set
            {
                if (!Equals(_selectedEncryptedBlockCipherMode, value))
                {
                    _selectedEncryptedBlockCipherMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedEncryptedBlockCipherMode)));
                }
            }
        }

        private KeyValuePair<PaddingMode, string> _selectedPaddingMode;

        public KeyValuePair<PaddingMode, string> SelectedPaddingMode
        {
            get { return _selectedPaddingMode; }

            set
            {
                if (!Equals(_selectedPaddingMode, value))
                {
                    _selectedPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPaddingMode)));
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

        private bool _isEncrypting;

        public bool IsEncrypting
        {
            get { return _isEncrypting; }

            set
            {
                if (!Equals(_isEncrypting, value))
                {
                    _isEncrypting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEncrypting)));
                }
            }
        }

        private string _encryptType;

        public string EncryptType
        {
            get { return _encryptType; }

            set
            {
                if (!string.Equals(_encryptType, value))
                {
                    _encryptType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptType)));
                }
            }
        }

        private string _encryptResult;

        public string EncryptResult
        {
            get { return _encryptResult; }

            set
            {
                if (!string.Equals(_encryptResult, value))
                {
                    _encryptResult = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptResult)));
                }
            }
        }

        private bool _isLargeContent;

        public bool IsLargeContent
        {
            get { return _isLargeContent; }

            set
            {
                if (!Equals(_isLargeContent, value))
                {
                    _isLargeContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLargeContent)));
                }
            }
        }

        private List<DataEncryptTypeModel> DataEncryptTypeList { get; } = [];

        private List<KeyValuePair<string, string>> EncryptKeyStringTypeList { get; } = [];

        private List<KeyValuePair<string, string>> InitializationVectorStringTypeList { get; } = [];

        private List<KeyValuePair<CipherMode, string>> EncryptedBlockCipherModeList { get; } = [];

        private List<KeyValuePair<PaddingMode, string>> PaddingModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataEncryptPage()
        {
            InitializeComponent();
            EncryptKeyStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            EncryptKeyStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CBC, CBCString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.ECB, ECBString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.OFB, OFBString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CFB, CFBString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CTS, CTSString));
            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.None, NonePaddingString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.PKCS7, PKCS7String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.Zeros, ZerosString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ANSIX923, ANSIX923String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ISO10126, ISO10126String));
            SelectedPaddingMode = PaddingModeList[0];
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.AES,
                Name = AESString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.Blowfish,
                Name = BlowfishString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.CaesarCipher,
                Name = CaesarCipherString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.ChaCha20,
                Name = ChaCha20String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.DES,
                Name = DESString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.ECC,
                Name = ECCString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.MorseCode,
                Name = MorseCodeString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.Rabbit,
                Name = RabbitString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC2,
                Name = RC2String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC4,
                Name = RC4String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC5,
                Name = RC5String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RC6,
                Name = RC6String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.Rijndael,
                Name = RijndaelString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.RSA,
                Name = RSAString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.SM2,
                Name = SM2String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.SM4,
                Name = SM4String
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.TripleDES,
                Name = TripleDESString
            });
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.XOR,
                Name = XORString
            });
            SelectedDataEncryptType = DataEncryptTypeList[0];
        }

        #region 第一部分：数据校验页面——挂载的事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        private async void OnDataDragOver(object sender, Microsoft.UI.Xaml.DragEventArgs args)
        {
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                if (IsEncrypting)
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = EncryptingString;
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnDragOver), 1, e);
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(IconExtractPage), nameof(OnDrop), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                EncryptFile = filePath;
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
                if (SelectedIndex is 0 && ResultServerity is InfoBarSeverity.Informational)
                {
                    ResultMessage = FileInitializeString;
                }
                else if (SelectedIndex is 1 && ResultServerity is InfoBarSeverity.Informational)
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
                EncryptFile = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 校验内容发生改变时触发的事件
        /// </summary>
        private void OnEncryptContentTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                EncryptContent = textBox.Text;
            }
        }

        /// <summary>
        /// 数据加密类型选中项发生改变时触发的事件
        /// </summary>
        private void OnEncryptSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is DataEncryptTypeModel dataEncryptType)
            {
                SelectedDataEncryptType = dataEncryptType;
                EncryptKeyText = string.Empty;
                InitializationVectorText = string.Empty;
            }
        }

        /// <summary>
        /// 加密密钥内容发生改变时触发的事件
        /// </summary>
        private void OnEncryptKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                EncryptKeyText = textBox.Text;
            }
        }

        /// <summary>
        /// 初始化向量内容改变时触发的事件
        /// </summary>
        private void OnInitializationVectorTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                InitializationVectorText = textBox.Text;
            }
        }

        /// <summary>
        /// 加密块密码模式发生变化时触发的事件
        /// </summary>
        private void OnEncryptedBlockCipherModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<CipherMode, string> encryptedBlockCipherMode)
            {
                SelectedEncryptedBlockCipherMode = encryptedBlockCipherMode;
            }
        }

        /// <summary>
        /// 填充模式发生变化时触发的事件
        /// </summary>
        private void OnPaddingModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<PaddingMode, string> encryptedPaddingMode)
            {
                SelectedPaddingMode = encryptedPaddingMode;
            }
        }

        /// <summary>
        /// 输出格式为大写字母
        /// </summary>
        private void OnUseUpperCaseChecked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(EncryptResult))
            {
                EncryptResult = EncryptResult.ToUpperInvariant();
            }
        }

        /// <summary>
        /// 输出格式为小写字母
        /// </summary>
        private void OnUseUpperCaseUnchecked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(EncryptResult))
            {
                EncryptResult = EncryptResult.ToLowerInvariant();
            }
        }

        /// <summary>
        /// 加密密钥字符串编码模式发生变化时触发的事件
        /// </summary>
        private void OnEncryptKeyStringTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> encryptKeyStringType)
            {
                SelectedEncryptKeyStringType = encryptKeyStringType;
            }
        }

        /// <summary>
        /// 初始化向量字符串编码模式发生变化时触发的事件
        /// </summary>
        private void OnInitializationVectorStringTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> initializationVectorStringType)
            {
                SelectedInitializationVectorStringType = initializationVectorStringType;
            }
        }

        /// <summary>
        /// 开始数据加密
        /// </summary>
        private async void OnStartEncryptClicked(object sender, RoutedEventArgs args)
        {
            selectEncryptIndex = SelectedIndex;
            selectedEncryptFile = EncryptFile;
            selectedEncryptContent = EncryptContent;
            EncryptResult = string.Empty;
            encryptedLocalFile = string.Empty;
            IsLargeContent = false;
            if (selectEncryptIndex is 0 && string.IsNullOrEmpty(selectedEncryptFile))
            {
                ResultServerity = InfoBarSeverity.Error;
                if (string.IsNullOrEmpty(selectedEncryptFile))
                {
                    ResultMessage = FileNotSelectedString;
                    return;
                }
                else if (!File.Exists(selectedEncryptFile))
                {
                    ResultMessage = FileNotExistedString;
                    return;
                }
                return;
            }
            else if (selectEncryptIndex is 1 && string.IsNullOrEmpty(selectedEncryptContent))
            {
                ResultServerity = InfoBarSeverity.Error;
                ResultMessage = ContentEmptyString;
            }

            if (SelectedDataEncryptType is null)
            {
                ResultServerity = InfoBarSeverity.Error;
                ResultMessage = EncryptTypeNotSelectedString;
                return;
            }
            IsEncrypting = true;
            ResultServerity = InfoBarSeverity.Informational;
            ResultMessage = EncryptingString;

            (string encryptedData, string key, string keyStringType, string initializationVector, string initializationVectorStringType) = await Task.Run(() =>
            {
                return GetEncryptedData(SelectedDataEncryptType.DataEncryptType, selectEncryptIndex, selectedEncryptContent, selectedEncryptFile);
            });

            if (string.IsNullOrEmpty(encryptedData))
            {
                ResultServerity = InfoBarSeverity.Error;
                if (selectEncryptIndex is 0)
                {
                    ResultMessage = FileEncryptFailedString;
                }
                else if (selectEncryptIndex is 1)
                {
                    ResultMessage = ContentEncryptFailedString;
                }
            }
            else
            {
                ResultServerity = InfoBarSeverity.Success;
                if (string.IsNullOrEmpty(EncryptKeyText) && !string.IsNullOrEmpty(key))
                {
                    EncryptKeyText = key;
                    if (!string.IsNullOrEmpty(keyStringType))
                    {
                        SelectedEncryptKeyStringType = EncryptKeyStringTypeList.Find(item => string.Equals(item.Key, keyStringType));
                    }
                }
                if (string.IsNullOrEmpty(InitializationVectorText) && !string.IsNullOrEmpty(initializationVector))
                {
                    InitializationVectorText = initializationVector;
                    if (!string.IsNullOrEmpty(initializationVectorStringType))
                    {
                        SelectedInitializationVectorStringType = InitializationVectorStringTypeList.Find(item => string.Equals(item.Key, initializationVectorStringType));
                    }
                }
                if (encryptedData.Length > 1024)
                {
                    EncryptResult = LargeContentString;
                    IsLargeContent = true;

                    await Task.Run(() =>
                    {
                        try
                        {
                            encryptedLocalFile = Path.GetTempFileName();
                            File.AppendAllText(encryptedLocalFile, UseUpperCase ? encryptedData.ToUpperInvariant() : encryptedData.ToLowerInvariant());
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnStartEncryptClicked), 1, e);
                        }
                    });
                }
                else
                {
                    EncryptResult = UseUpperCase ? encryptedData.ToUpperInvariant() : encryptedData.ToLowerInvariant();
                    IsLargeContent = false;
                }
                if (selectEncryptIndex is 0)
                {
                    ResultMessage = FileEncryptSuccessfullyString;
                }
                else if (selectEncryptIndex is 1)
                {
                    ResultMessage = ContentEncryptSuccessfullyString;
                }
            }
            IsEncrypting = false;
        }

        /// <summary>
        /// 查看本地文件
        /// </summary>
        private async void OnViewLocalFileClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(encryptedLocalFile))
                    {
                        Process.Start(encryptedLocalFile);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataVertifyPage), nameof(OnViewLocalFileClicked), 1, e);
                }
            });
        }

        #endregion 第一部分：数据校验页面——挂载的事件

        /// <summary>
        /// 获取校验后的数据
        /// </summary>
        /// TODO：未完成
        private (string, string, string, string, string) GetEncryptedData(DataEncryptType dataEncryptType, int selectedEncryptIndex, string contentData, string encryptFile)
        {
            string encryptedData = string.Empty;
            string key = string.Empty;
            string keyStringType = string.Empty;
            string initializationVector = string.Empty;
            string initializationVectorStringType = string.Empty;

            switch (dataEncryptType)
            {
                case DataEncryptType.AES:
                    {
                        try
                        {
                            Aes aes = Aes.Create();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                aes.GenerateKey();
                                key = Convert.ToBase64String(aes.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    aes.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    aes.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                aes.GenerateIV();
                                initializationVector = Convert.ToBase64String(aes.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    aes.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    aes.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            aes.Mode = SelectedEncryptedBlockCipherMode.Key;
                            aes.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.AES) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.Blowfish:
                    {
                        try
                        {
                            Blowfish blowfish = new();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                blowfish.GenerateKey();
                                key = Convert.ToBase64String(blowfish.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    blowfish.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    blowfish.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                blowfish.GenerateIV();
                                initializationVector = Convert.ToBase64String(blowfish.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    blowfish.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    blowfish.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            blowfish.Mode = SelectedEncryptedBlockCipherMode.Key;
                            blowfish.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = blowfish.CreateEncryptor(blowfish.Key, blowfish.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.Blowfish) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.CaesarCipher:
                    {
                        if (selectedEncryptIndex is 0)
                        {
                            //TOOD：显示通知：凯撒密码仅支持字符串加密
                        }
                        else
                        {
                            try
                            {
                                // TODO：偏移量变量需要更新单独的设置选项
                                encryptedData = CaesarCipher.CaesarEncrypt(contentData, 15);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.CaesarCipher) + 1, e);
                            }
                        }
                        break;
                    }
                case DataEncryptType.ChaCha20:
                    {
                        break;
                    }
                case DataEncryptType.DES:
                    {
                        try
                        {
                            DES des = DES.Create();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                des.GenerateKey();
                                key = Convert.ToBase64String(des.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    des.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    des.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                des.GenerateIV();
                                initializationVector = Convert.ToBase64String(des.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    des.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    des.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            des.Mode = SelectedEncryptedBlockCipherMode.Key;
                            des.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = des.CreateEncryptor(des.Key, des.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.DES) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.ECC:
                    {
                        break;
                    }
                case DataEncryptType.MorseCode:
                    {
                        if (selectedEncryptIndex is 0)
                        {
                            //TOOD：显示通知：摩尔斯密码仅支持字符串加密
                        }
                        else
                        {
                            try
                            {
                                // 隐藏选项
                                encryptedData = MorseCode.Encode(contentData.ToUpperInvariant());
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.CaesarCipher) + 1, e);
                            }
                        }
                        break;
                    }
                case DataEncryptType.Rabbit:
                    {
                        break;
                    }
                case DataEncryptType.RC2:
                    {
                        try
                        {
                            RC2 rc2 = RC2.Create();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                rc2.GenerateKey();
                                key = Convert.ToBase64String(rc2.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rc2.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rc2.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                rc2.GenerateIV();
                                initializationVector = Convert.ToBase64String(rc2.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc2.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc2.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            rc2.Mode = SelectedEncryptedBlockCipherMode.Key;
                            rc2.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = rc2.CreateEncryptor(rc2.Key, rc2.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.RC2) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RC4:
                    {
                        try
                        {
                            RC4 rc4 = RC4.Create();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                rc4.GenerateKey();
                                key = Convert.ToBase64String(rc4.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rc4.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rc4.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                rc4.GenerateIV();
                                initializationVector = Convert.ToBase64String(rc4.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc4.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc4.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            rc4.Mode = SelectedEncryptedBlockCipherMode.Key;
                            rc4.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = rc4.CreateEncryptor(rc4.Key, rc4.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.RC4) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RC5:
                    {
                        break;
                    }
                case DataEncryptType.RC6:
                    {
                        break;
                    }
                case DataEncryptType.Rijndael:
                    {
                        try
                        {
                            Rijndael rijndael = Rijndael.Create();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                rijndael.GenerateKey();
                                key = Convert.ToBase64String(rijndael.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rijndael.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rijndael.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                rijndael.GenerateIV();
                                initializationVector = Convert.ToBase64String(rijndael.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rijndael.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rijndael.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            rijndael.Mode = SelectedEncryptedBlockCipherMode.Key;
                            rijndael.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.Rijndael) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RSA:
                    {
                        break;
                    }
                case DataEncryptType.SM2:
                    {
                        break;
                    }
                case DataEncryptType.SM4:
                    {
                        try
                        {
                            SM4 sm4 = new();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                sm4.GenerateKey();
                                key = Convert.ToBase64String(sm4.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    sm4.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    sm4.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                sm4.GenerateIV();
                                initializationVector = Convert.ToBase64String(sm4.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    sm4.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    sm4.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            sm4.Mode = SelectedEncryptedBlockCipherMode.Key;
                            sm4.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = sm4.CreateEncryptor(sm4.Key, sm4.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.SM4) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.TripleDES:
                    {
                        try
                        {
                            TripleDES tripleDes = TripleDES.Create();
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                tripleDes.GenerateKey();
                                key = Convert.ToBase64String(tripleDes.Key);
                                keyStringType = EncryptKeyStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    tripleDes.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    tripleDes.Key = Convert.FromBase64String(EncryptKeyText);
                                    keyStringType = EncryptKeyStringTypeList[1].Key;
                                }
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                tripleDes.GenerateIV();
                                initializationVector = Convert.ToBase64String(tripleDes.IV);
                                initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                            }
                            else
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    tripleDes.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[0].Key;
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    tripleDes.IV = Convert.FromBase64String(InitializationVectorText);
                                    initializationVectorStringType = InitializationVectorStringTypeList[1].Key;
                                }
                            }

                            tripleDes.Mode = SelectedEncryptedBlockCipherMode.Key;
                            tripleDes.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = tripleDes.CreateEncryptor(tripleDes.Key, tripleDes.IV);
                            MemoryStream memoryStream = new();
                            CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                fileStream.CopyTo(cryptoStream);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = Encoding.UTF8.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.TripleDES) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.XOR:
                    {
                        break;
                    }
            }

            return ValueTuple.Create(encryptedData, key, keyStringType, initializationVector, initializationVectorStringType);
        }

        /// <summary>
        /// 获取要校验的类型
        /// </summary>
        private Visibility GetDataEncryptType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取加密结果
        /// </summary>
        private Visibility GetEncryptResult(InfoBarSeverity resultSeverity)
        {
            return resultSeverity is InfoBarSeverity.Success ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
