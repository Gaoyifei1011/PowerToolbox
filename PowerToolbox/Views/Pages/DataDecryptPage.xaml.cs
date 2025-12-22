using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
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
    /// 数据解密页面
    /// </summary>
    public sealed partial class DataDecryptPage : Page, INotifyPropertyChanged
    {
        private readonly string AESString = ResourceService.DataDecryptResource.GetString("AES");
        private readonly string ANSIX923String = ResourceService.DataDecryptResource.GetString("ANSIX923");
        private readonly string Base64String = ResourceService.DataDecryptResource.GetString("Base64");
        private readonly string CaesarCipherString = ResourceService.DataDecryptResource.GetString("CaesarCipher");
        private readonly string CBCString = ResourceService.DataDecryptResource.GetString("CBC");
        private readonly string CFBString = ResourceService.DataDecryptResource.GetString("CFB");
        private readonly string ChaCha20String = ResourceService.DataDecryptResource.GetString("ChaCha20");
        private readonly string ContentInitializeString = ResourceService.DataDecryptResource.GetString("ContentInitialize");
        private readonly string CTSString = ResourceService.DataDecryptResource.GetString("CTS");
        private readonly string DecryptingString = ResourceService.DataDecryptResource.GetString("Decrypting");
        private readonly string DecryptKey162432SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey162432Size");
        private readonly string DecryptKey1624SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey1624Size");
        private readonly string DecryptKey16SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey16Size");
        private readonly string DecryptKey32SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey32Size");
        private readonly string DecryptKey8SizeString = ResourceService.DataDecryptResource.GetString("DecryptKey8Size");
        private readonly string DecryptKeyEmptyString = ResourceService.DataDecryptResource.GetString("DecryptKeyEmpty");
        private readonly string DESString = ResourceService.DataDecryptResource.GetString("DES");
        private readonly string DragOverContentString = ResourceService.DataDecryptResource.GetString("DragOverContent");
        private readonly string ECBString = ResourceService.DataDecryptResource.GetString("ECB");
        private readonly string FileInitializeString = ResourceService.DataDecryptResource.GetString("FileInitialize");
        private readonly string InitializationVector12SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector12Size");
        private readonly string InitializationVector16SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector16Size");
        private readonly string InitializationVector8SizeString = ResourceService.DataDecryptResource.GetString("InitializationVector8Size");
        private readonly string InitializationVectorEmptyString = ResourceService.DataDecryptResource.GetString("InitializationVectorEmpty");
        private readonly string ISO10126String = ResourceService.DataDecryptResource.GetString("ISO10126");
        private readonly string MorseCodeString = ResourceService.DataDecryptResource.GetString("MorseCode");
        private readonly string NoMultiFileString = ResourceService.DataDecryptResource.GetString("NoMultiFile");
        private readonly string NonePaddingString = ResourceService.DataDecryptResource.GetString("NonePadding");
        private readonly string OaepString = ResourceService.DataDecryptResource.GetString("Oaep");
        private readonly string OFBString = ResourceService.DataDecryptResource.GetString("OFB");
        private readonly string Pkcs1String = ResourceService.DataDecryptResource.GetString("Pkcs1");
        private readonly string Pkcs7String = ResourceService.DataDecryptResource.GetString("Pkcs7");
        private readonly string RabbitString = ResourceService.DataDecryptResource.GetString("Rabbit");
        private readonly string RC2String = ResourceService.DataDecryptResource.GetString("RC2");
        private readonly string RC4String = ResourceService.DataDecryptResource.GetString("RC4");
        private readonly string RC5String = ResourceService.DataDecryptResource.GetString("RC5");
        private readonly string RC6String = ResourceService.DataDecryptResource.GetString("RC6");
        private readonly string RijndaelString = ResourceService.DataDecryptResource.GetString("Rijndael");
        private readonly string RSAString = ResourceService.DataDecryptResource.GetString("RSA");
        private readonly string SelectFileString = ResourceService.DataDecryptResource.GetString("SelectFile");
        private readonly string SM4String = ResourceService.DataDecryptResource.GetString("SM4");
        private readonly string StringLengthString = ResourceService.DataDecryptResource.GetString("StringLength");
        private readonly string TripleDESString = ResourceService.DataDecryptResource.GetString("TripleDES");
        private readonly string UTF8String = ResourceService.DataDecryptResource.GetString("UTF8");
        private readonly string XORString = ResourceService.DataDecryptResource.GetString("XOR");
        private readonly string ZerosString = ResourceService.DataDecryptResource.GetString("Zeros");

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

        private string _decryptFile;

        public string DecryptFile
        {
            get { return _decryptFile; }

            set
            {
                if (!Equals(_decryptFile, value))
                {
                    _decryptFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptFile)));
                }
            }
        }

        private string _decryptContent;

        public string DecryptContent
        {
            get { return _decryptContent; }

            set
            {
                if (!Equals(_decryptContent, value))
                {
                    _decryptContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptContent)));
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

        private string _decryptFailedInformation;

        public string DecryptFailedInformation
        {
            get { return _decryptFailedInformation; }

            set
            {
                if (!string.Equals(_decryptFailedInformation, value))
                {
                    _decryptFailedInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptFailedInformation)));
                }
            }
        }

        private DataDecryptTypeModel _selectedDataDecryptType;

        public DataDecryptTypeModel SelectedDataDecryptType
        {
            get { return _selectedDataDecryptType; }

            set
            {
                if (!Equals(_selectedDataDecryptType, value))
                {
                    _selectedDataDecryptType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDataDecryptType)));
                }
            }
        }

        private bool _hasDecryptOptions;

        public bool HasDecryptOptions
        {
            get { return _hasDecryptOptions; }

            set
            {
                if (!Equals(_hasDecryptOptions, value))
                {
                    _hasDecryptOptions = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptOptions)));
                }
            }
        }

        private bool _hasDecryptKey;

        public bool HasDecryptKey
        {
            get { return _hasDecryptKey; }

            set
            {
                if (!Equals(_hasDecryptKey, value))
                {
                    _hasDecryptKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptKey)));
                }
            }
        }

        private string _decryptKeyPHText = string.Empty;

        public string DecryptKeyPHText
        {
            get { return _decryptKeyPHText; }

            set
            {
                if (!string.Equals(_decryptKeyPHText, value))
                {
                    _decryptKeyPHText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptKeyPHText)));
                }
            }
        }

        private string _decryptKeyText = string.Empty;

        public string DecryptKeyText
        {
            get { return _decryptKeyText; }

            set
            {
                if (!string.Equals(_decryptKeyText, value))
                {
                    _decryptKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptKeyText)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedDecryptKeyStringType;

        public KeyValuePair<string, string> SelectedDecryptKeyStringType
        {
            get { return _selectedDecryptKeyStringType; }

            set
            {
                if (!Equals(_selectedDecryptKeyStringType, value))
                {
                    _selectedDecryptKeyStringType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDecryptKeyStringType)));
                }
            }
        }

        private bool _hasInitializationVector;

        public bool HasInitializationVector
        {
            get { return _hasInitializationVector; }

            set
            {
                if (!Equals(_hasInitializationVector, value))
                {
                    _hasInitializationVector = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasInitializationVector)));
                }
            }
        }

        private string _initializationVectorPHText = string.Empty;

        public string InitializationVectorPHText
        {
            get { return _initializationVectorPHText; }

            set
            {
                if (!string.Equals(_initializationVectorPHText, value))
                {
                    _initializationVectorPHText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InitializationVectorPHText)));
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

        private bool _hasDecryptedBlockCipherMode;

        public bool HasDecryptedBlockCipherMode
        {
            get { return _hasDecryptedBlockCipherMode; }

            set
            {
                if (!Equals(_hasDecryptedBlockCipherMode, value))
                {
                    _hasDecryptedBlockCipherMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptedBlockCipherMode)));
                }
            }
        }

        private KeyValuePair<CipherMode, string> _selectedDecryptedBlockCipherMode;

        public KeyValuePair<CipherMode, string> SelectedDecryptedBlockCipherMode
        {
            get { return _selectedDecryptedBlockCipherMode; }

            set
            {
                if (!Equals(_selectedDecryptedBlockCipherMode, value))
                {
                    _selectedDecryptedBlockCipherMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDecryptedBlockCipherMode)));
                }
            }
        }

        private bool _hasPaddingMode;

        public bool HasPaddingMode
        {
            get { return _hasPaddingMode; }

            set
            {
                if (!Equals(_hasPaddingMode, value))
                {
                    _hasPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasPaddingMode)));
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

        private bool _hasOffset;

        public bool HasOffset
        {
            get { return _hasOffset; }

            set
            {
                if (!Equals(_hasOffset, value))
                {
                    _hasOffset = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasOffset)));
                }
            }
        }

        private int _offset;

        public int Offset
        {
            get { return _offset; }

            set
            {
                if (!Equals(_offset, value))
                {
                    _offset = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offset)));
                }
            }
        }

        private bool _hasDecryptPublicKey;

        public bool HasDecryptPublicKey
        {
            get { return _hasDecryptPublicKey; }

            set
            {
                if (!Equals(_hasDecryptPublicKey, value))
                {
                    _hasDecryptPublicKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptPublicKey)));
                }
            }
        }

        private string _decryptPublicKeyText;

        public string DecryptPublicKeyText
        {
            get { return _decryptPublicKeyText; }

            set
            {
                if (!Equals(_decryptPublicKeyText, value))
                {
                    _decryptPublicKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptPublicKeyText)));
                }
            }
        }

        private bool _hasDecryptPrivateKey;

        public bool HasDecryptPrivateKey
        {
            get { return _hasDecryptPrivateKey; }

            set
            {
                if (!Equals(_hasDecryptPrivateKey, value))
                {
                    _hasDecryptPrivateKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDecryptPrivateKey)));
                }
            }
        }

        private string _decryptPrivateKeyText;

        public string DecryptPrivateKeyText
        {
            get { return _decryptPrivateKeyText; }

            set
            {
                if (!Equals(_decryptPrivateKeyText, value))
                {
                    _decryptPrivateKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptPrivateKeyText)));
                }
            }
        }

        private bool _hasRSAEncryptionPaddingMode;

        public bool HasRSAEncryptionPaddingMode
        {
            get { return _hasRSAEncryptionPaddingMode; }

            set
            {
                if (!Equals(_hasRSAEncryptionPaddingMode, value))
                {
                    _hasRSAEncryptionPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasRSAEncryptionPaddingMode)));
                }
            }
        }

        private KeyValuePair<RSAEncryptionPaddingMode, string> _selectedRSAEncryptionPaddingMode;

        public KeyValuePair<RSAEncryptionPaddingMode, string> SelectedRSAEncryptionPaddingMode
        {
            get { return _selectedRSAEncryptionPaddingMode; }

            set
            {
                if (!Equals(_selectedRSAEncryptionPaddingMode, value))
                {
                    _selectedRSAEncryptionPaddingMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRSAEncryptionPaddingMode)));
                }
            }
        }

        private bool _hasRSADecryptionOtherOptions;

        public bool HasRSADecryptionOtherOptions
        {
            get { return _hasRSADecryptionOtherOptions; }

            set
            {
                if (!Equals(_hasRSADecryptionOtherOptions, value))
                {
                    _hasRSADecryptionOtherOptions = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasRSADecryptionOtherOptions)));
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

        private bool _isDecrypting;

        public bool IsDecrypting
        {
            get { return _isDecrypting; }

            set
            {
                if (!Equals(_isDecrypting, value))
                {
                    _isDecrypting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDecrypting)));
                }
            }
        }

        private string _decryptResult;

        public string DecryptResult
        {
            get { return _decryptResult; }

            set
            {
                if (!string.Equals(_decryptResult, value))
                {
                    _decryptResult = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecryptResult)));
                }
            }
        }

        private List<DataDecryptTypeModel> DataDecryptTypeList { get; } = [];

        private List<KeyValuePair<string, string>> DecryptKeyStringTypeList { get; } = [];

        private List<KeyValuePair<string, string>> InitializationVectorStringTypeList { get; } = [];

        private List<KeyValuePair<CipherMode, string>> DecryptedBlockCipherModeList { get; } = [];

        private List<KeyValuePair<PaddingMode, string>> PaddingModeList { get; } = [];

        private List<KeyValuePair<RSAEncryptionPaddingMode, string>> RSAEncryptionPaddingModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataDecryptPage()
        {
            InitializeComponent();
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.AES,
                Name = AESString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.CaesarCipher,
                Name = CaesarCipherString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.ChaCha20,
                Name = ChaCha20String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.DES,
                Name = DESString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.MorseCode,
                Name = MorseCodeString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.Rabbit,
                Name = RabbitString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC2,
                Name = RC2String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC4,
                Name = RC4String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC5,
                Name = RC5String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RC6,
                Name = RC6String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.Rijndael,
                Name = RijndaelString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.RSA,
                Name = RSAString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.SM4,
                Name = SM4String
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.TripleDES,
                Name = TripleDESString
            });
            DataDecryptTypeList.Add(new DataDecryptTypeModel()
            {
                DataDecryptType = DataDecryptType.XOR,
                Name = XORString
            });
            SelectedDataDecryptType = DataDecryptTypeList[0];
            DecryptKeyStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            DecryptKeyStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CBC, CBCString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.ECB, ECBString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.OFB, OFBString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CFB, CFBString));
            DecryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CTS, CTSString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.None, NonePaddingString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.PKCS7, Pkcs7String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.Zeros, ZerosString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ANSIX923, ANSIX923String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ISO10126, ISO10126String));
            RSAEncryptionPaddingModeList.Add(new KeyValuePair<RSAEncryptionPaddingMode, string>(RSAEncryptionPaddingMode.Pkcs1, Pkcs1String));
            RSAEncryptionPaddingModeList.Add(new KeyValuePair<RSAEncryptionPaddingMode, string>(RSAEncryptionPaddingMode.Oaep, OaepString));
        }

        #region 第一部分：数据解密页面——挂载的事件

        /// <summary>
        /// 设置拖动的数据的可视表示形式
        /// </summary>
        private async void OnDataDragOver(object sender, Microsoft.UI.Xaml.DragEventArgs args)
        {
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                if (IsDecrypting)
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = DecryptingString;
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnDragOver), 1, e);
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnDrop), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataDecryptPage), nameof(OnDrop), 2, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (File.Exists(filePath))
            {
                DecryptFile = filePath;
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
                if (!IsDecrypting && SelectedIndex is 0 && ResultSeverity is InfoBarSeverity.Informational)
                {
                    ResultMessage = FileInitializeString;
                }
                else if (!IsDecrypting && SelectedIndex is 1 && ResultSeverity is InfoBarSeverity.Informational)
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
                DecryptFile = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 解密内容发生改变时触发的事件
        /// </summary>
        private void OnDecryptContentTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DecryptContent = textBox.Text;
            }
        }

        /// <summary>
        /// 复制错误原因
        /// </summary>
        private async void OnCopyErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            if (ViewErrorInformationFlyout.IsOpen)
            {
                ViewErrorInformationFlyout.Hide();
            }

            if (!string.IsNullOrEmpty(DecryptFailedInformation))
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(DecryptFailedInformation);

                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            if (ViewErrorInformationFlyout.IsOpen)
            {
                ViewErrorInformationFlyout.Hide();
            }
        }

        /// <summary>
        /// 数据解密类型选中项发生改变时触发的事件
        /// </summary>
        private void OnDecryptSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is DataDecryptTypeModel dataDecryptType)
            {
                SelectedDataDecryptType = dataDecryptType;
                DecryptKeyText = string.Empty;
                InitializationVectorText = string.Empty;

                switch (SelectedDataDecryptType.DataDecryptType)
                {
                    case DataDecryptType.AES:
                        {
                            DecryptKeyPHText = DecryptKey162432SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.CaesarCipher:
                        {
                            SelectedIndex = 1;
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = false;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = true;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.ChaCha20:
                        {
                            DecryptKeyPHText = DecryptKey32SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector12SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = true;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.DES:
                        {
                            DecryptKeyPHText = DecryptKey8SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.MorseCode:
                        {
                            SelectedIndex = 1;
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = false;
                            HasDecryptKey = false;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.Rabbit:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = true;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC2:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC4:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC5:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RC6:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.Rijndael:
                        {
                            DecryptKeyPHText = DecryptKey162432SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.RSA:
                        {
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = false;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPublicKey = true;
                            HasDecryptPrivateKey = true;
                            HasRSAEncryptionPaddingMode = true;
                            HasRSADecryptionOtherOptions = true;
                            break;
                        }
                    case DataDecryptType.SM4:
                        {
                            DecryptKeyPHText = DecryptKey16SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.TripleDES:
                        {
                            DecryptKeyPHText = DecryptKey1624SizeString;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasDecryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    case DataDecryptType.XOR:
                        {
                            SelectedIndex = 1;
                            DecryptKeyPHText = string.Empty;
                            DecryptKeyText = string.Empty;
                            SelectedDecryptKeyStringType = DecryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedDecryptedBlockCipherMode = DecryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            DecryptPublicKeyText = string.Empty;
                            DecryptPrivateKeyText = string.Empty;
                            HasDecryptOptions = true;
                            HasDecryptKey = true;
                            HasInitializationVector = false;
                            HasDecryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasDecryptPublicKey = false;
                            HasDecryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSADecryptionOtherOptions = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 解密密钥字符串编码模式发生变化时触发的事件
        /// </summary>
        private void OnDecryptKeyStringTypeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> decryptKeyStringType)
            {
                SelectedDecryptKeyStringType = decryptKeyStringType;
            }
        }

        /// <summary>
        /// 解密密钥内容发生改变时触发的事件
        /// </summary>
        private void OnDecryptKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DecryptKeyText = textBox.Text;
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
        /// 解密块密码模式发生变化时触发的事件
        /// </summary>
        private void OnDecryptedBlockCipherModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<CipherMode, string> decryptedBlockCipherMode)
            {
                SelectedDecryptedBlockCipherMode = decryptedBlockCipherMode;
                if (SelectedDataDecryptType.DataDecryptType is DataDecryptType.AES || SelectedDataDecryptType.DataDecryptType is DataDecryptType.DES || SelectedDataDecryptType.DataDecryptType is DataDecryptType.RC2 ||
                    SelectedDataDecryptType.DataDecryptType is DataDecryptType.RC5 || SelectedDataDecryptType.DataDecryptType is DataDecryptType.RC6 || SelectedDataDecryptType.DataDecryptType is DataDecryptType.Rijndael ||
                    SelectedDataDecryptType.DataDecryptType is DataDecryptType.SM4 || SelectedDataDecryptType.DataDecryptType is DataDecryptType.TripleDES)
                {
                    HasInitializationVector = SelectedDecryptedBlockCipherMode.Key is not CipherMode.ECB;
                }
            }
        }

        /// <summary>
        /// 填充模式发生变化时触发的事件
        /// </summary>
        private void OnPaddingModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<PaddingMode, string> decryptedPaddingMode)
            {
                SelectedPaddingMode = decryptedPaddingMode;
            }
        }

        /// <summary>
        /// 偏移量值发生变化时触发的事件
        /// </summary>
        private void OnOffsetValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue is not double.NaN)
            {
                Offset = Convert.ToInt32(args.NewValue);
            }
        }

        /// <summary>
        /// 解密公钥内容发生变化时触发的事件
        /// </summary>
        private void OnDecryptPublicKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DecryptPublicKeyText = textBox.Text;
            }
        }

        /// <summary>
        /// 解密私钥内容发生变化时触发的事件
        /// </summary>
        private void OnDecryptPrivateKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                DecryptPrivateKeyText = textBox.Text;
            }
        }

        /// <summary>
        /// RSA 非对称加密填充模式发生变化时触发的事件
        /// </summary>
        private void OnRSAEncryptionPaddingModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<RSAEncryptionPaddingMode, string> encryptedRSAEncryptionPaddingMode)
            {
                SelectedRSAEncryptionPaddingMode = encryptedRSAEncryptionPaddingMode;
            }
        }

        /// <summary>
        /// 复制 RSA 加密算法公钥到剪贴板
        /// </summary>
        private async void OnCopyRSAPublicKeyClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(DecryptPublicKeyText))
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(DecryptPublicKeyText);

                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制 RSA 加密算法私钥到剪贴板
        /// </summary>
        private async void OnCopyRSAPrivateKeyClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(DecryptPrivateKeyText))
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(DecryptPrivateKeyText);

                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 输出格式为大写字母
        /// </summary>
        private void OnUseUpperCaseChecked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(DecryptResult))
            {
                DecryptResult = DecryptResult.ToUpperInvariant();
            }
        }

        /// <summary>
        /// 输出格式为小写字母
        /// </summary>
        private void OnUseUpperCaseUnchecked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(DecryptResult))
            {
                DecryptResult = DecryptResult.ToLowerInvariant();
            }
        }

        /// <summary>
        /// 开始数据解密
        /// </summary>
        private async void OnStartDecryptClicked(object sender, RoutedEventArgs args)
        {
            // TODO：未完成
        }

        #endregion 第一部分：数据解密页面——挂载的事件

        /// <summary>
        /// 获取要解密的类型
        /// </summary>
        private Visibility GetDataDecryptType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetDecryptInfoButtonHitTestState(InfoBarSeverity resultSeverity, bool isDecrypting)
        {
            return resultSeverity is InfoBarSeverity.Error && !isDecrypting;
        }

        private Visibility GetDecryptInfoButtonVisibility(InfoBarSeverity resultSeverity)
        {
            return resultSeverity is InfoBarSeverity.Informational || resultSeverity is InfoBarSeverity.Error ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetDecryptInfoProgressRingActiveState(InfoBarSeverity resultSeverity, bool isDecrypting)
        {
            return resultSeverity is InfoBarSeverity.Informational && isDecrypting;
        }

        private Visibility GetDecryptFailedInfoButtonState(InfoBarSeverity resultSeverity, bool isDecrypting, string decryptFailedInformation)
        {
            return resultSeverity is InfoBarSeverity.Error && !isDecrypting && !string.IsNullOrEmpty(decryptFailedInformation) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
