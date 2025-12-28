using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Extensions.Encrypt;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
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

// 抑制 CA1806，CA1822，CA2022，IDE0060 警告
#pragma warning disable CA1806,CA1822,CA2022,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据加密页面
    /// </summary>
    public sealed partial class DataEncryptPage : Page, INotifyPropertyChanged
    {
        private readonly string AllFilesString = ResourceService.DataEncryptResource.GetString("AllFiles");
        private readonly string AESString = ResourceService.DataEncryptResource.GetString("AES");
        private readonly string ANSIX923String = ResourceService.DataEncryptResource.GetString("ANSIX923");
        private readonly string ASCIIString = ResourceService.DataEncryptResource.GetString("ASCII");
        private readonly string Base64String = ResourceService.DataEncryptResource.GetString("Base64");
        private readonly string BigEndianUnicodeString = ResourceService.DataEncryptResource.GetString("BigEndianUnicode");
        private readonly string CaesarCipherString = ResourceService.DataEncryptResource.GetString("CaesarCipher");
        private readonly string CBCString = ResourceService.DataEncryptResource.GetString("CBC");
        private readonly string CFBString = ResourceService.DataEncryptResource.GetString("CFB");
        private readonly string ChaCha20String = ResourceService.DataEncryptResource.GetString("ChaCha20");
        private readonly string ContentEncryptedDataSaveFailedString = ResourceService.DataEncryptResource.GetString("ContentEncryptedDataSaveFailed");
        private readonly string ContentEncryptedDataSaveSuccessfullyString = ResourceService.DataEncryptResource.GetString("ContentEncryptedDataSaveSuccessfully");
        private readonly string ContentEncryptFailedString = ResourceService.DataEncryptResource.GetString("ContentEncryptFailed");
        private readonly string ContentEncryptedDataSaveFailedToTempFileString = ResourceService.DataEncryptResource.GetString("ContentEncryptedDataSaveFailedToTempFile");
        private readonly string ContentEncryptSuccessfullyString = ResourceService.DataEncryptResource.GetString("ContentEncryptSuccessfully");
        private readonly string ContentEmptyString = ResourceService.DataEncryptResource.GetString("ContentEmpty");
        private readonly string ContentInitializeString = ResourceService.DataEncryptResource.GetString("ContentInitialize");
        private readonly string CTSString = ResourceService.DataEncryptResource.GetString("CTS");
        private readonly string CustomString = ResourceService.DataEncryptResource.GetString("Custom");
        private readonly string DESString = ResourceService.DataEncryptResource.GetString("DES");
        private readonly string DragOverContentString = ResourceService.DataEncryptResource.GetString("DragOverContent");
        private readonly string ECBString = ResourceService.DataEncryptResource.GetString("ECB");
        private readonly string EncryptingString = ResourceService.DataEncryptResource.GetString("Encrypting");
        private readonly string EncryptKey162432SizeString = ResourceService.DataEncryptResource.GetString("EncryptKey162432Size");
        private readonly string EncryptKey1624SizeString = ResourceService.DataEncryptResource.GetString("EncryptKey1624Size");
        private readonly string EncryptKey16SizeString = ResourceService.DataEncryptResource.GetString("EncryptKey16Size");
        private readonly string EncryptKey32SizeString = ResourceService.DataEncryptResource.GetString("EncryptKey32Size");
        private readonly string EncryptKey8SizeString = ResourceService.DataEncryptResource.GetString("EncryptKey8Size");
        private readonly string EncryptKeyEmptyString = ResourceService.DataEncryptResource.GetString("EncryptKeyEmpty");
        private readonly string EncryptKeyInitializeString = ResourceService.DataEncryptResource.GetString("EncryptKeyInitialize");
        private readonly string EncryptPublicKeyEmptyString = ResourceService.DataEncryptResource.GetString("EncryptPublicKeyEmpty");
        private readonly string EncryptTypeMustContentString = ResourceService.DataEncryptResource.GetString("EncryptTypeMustContent");
        private readonly string EncryptTypeNotSelectedString = ResourceService.DataEncryptResource.GetString("EncryptTypeNotSelected");
        private readonly string FileEncryptedDataSaveFailedString = ResourceService.DataEncryptResource.GetString("FileEncryptedDataSaveFailed");
        private readonly string FileEncryptedDataSaveFailedToTempFileString = ResourceService.DataEncryptResource.GetString("FileEncryptedDataSaveFailedToTempFile");
        private readonly string FileEncryptedDataSaveSuccessfullyString = ResourceService.DataEncryptResource.GetString("FileEncryptedDataSaveSuccessfully");
        private readonly string FileEncryptFailedString = ResourceService.DataEncryptResource.GetString("FileEncryptFailed");
        private readonly string FileEncryptSuccessfullyString = ResourceService.DataEncryptResource.GetString("FileEncryptSuccessfully");
        private readonly string FileInitializeString = ResourceService.DataEncryptResource.GetString("FileInitialize");
        private readonly string FileNotExistedString = ResourceService.DataEncryptResource.GetString("FileNotExisted");
        private readonly string FileNotSelectedString = ResourceService.DataEncryptResource.GetString("FileNotSelected");
        private readonly string GB18030String = ResourceService.DataEncryptResource.GetString("GB18030");
        private readonly string GB2312String = ResourceService.DataEncryptResource.GetString("GB2312");
        private readonly string GBKString = ResourceService.DataEncryptResource.GetString("GBK");
        private readonly string InitializationVector12SizeString = ResourceService.DataEncryptResource.GetString("InitializationVector12Size");
        private readonly string InitializationVector16SizeString = ResourceService.DataEncryptResource.GetString("InitializationVector16Size");
        private readonly string InitializationVector8SizeString = ResourceService.DataEncryptResource.GetString("InitializationVector8Size");
        private readonly string InitializationVectorEmptyString = ResourceService.DataEncryptResource.GetString("InitializationVectorEmpty");
        private readonly string ISO10126String = ResourceService.DataEncryptResource.GetString("ISO10126");
        private readonly string ISO88591String = ResourceService.DataEncryptResource.GetString("ISO88591");
        private readonly string LargeContentString = ResourceService.DataEncryptResource.GetString("LargeContent");
        private readonly string MorseCodeString = ResourceService.DataEncryptResource.GetString("MorseCode");
        private readonly string NoMultiFileString = ResourceService.DataEncryptResource.GetString("NoMultiFile");
        private readonly string NonePaddingString = ResourceService.DataEncryptResource.GetString("NonePadding");
        private readonly string OaepString = ResourceService.DataEncryptResource.GetString("Oaep");
        private readonly string OFBString = ResourceService.DataEncryptResource.GetString("OFB");
        private readonly string Pkcs1String = ResourceService.DataEncryptResource.GetString("Pkcs1");
        private readonly string Pkcs7String = ResourceService.DataEncryptResource.GetString("Pkcs7");
        private readonly string RabbitString = ResourceService.DataEncryptResource.GetString("Rabbit");
        private readonly string RC2String = ResourceService.DataEncryptResource.GetString("RC2");
        private readonly string RC4String = ResourceService.DataEncryptResource.GetString("RC4");
        private readonly string RC5String = ResourceService.DataEncryptResource.GetString("RC5");
        private readonly string RC6String = ResourceService.DataEncryptResource.GetString("RC6");
        private readonly string RijndaelString = ResourceService.DataEncryptResource.GetString("Rijndael");
        private readonly string RSAString = ResourceService.DataEncryptResource.GetString("RSA");
        private readonly string SelectedDataEncryptInputContentFormatErrorString = ResourceService.DataEncryptResource.GetString("SelectedDataEncryptInputContentFormatError");
        private readonly string SelectFileString = ResourceService.DataEncryptResource.GetString("SelectFile");
        private readonly string SM4String = ResourceService.DataEncryptResource.GetString("SM4");
        private readonly string StringLengthString = ResourceService.DataEncryptResource.GetString("StringLength");
        private readonly string TextEncodingInvalidString = ResourceService.DataEncryptResource.GetString("TextEncodingInvalid");
        private readonly string TripleDESString = ResourceService.DataEncryptResource.GetString("TripleDES");
        private readonly string UnicodeString = ResourceService.DataEncryptResource.GetString("Unicode");
        private readonly string UnknownErrorString = ResourceService.DataEncryptResource.GetString("UnknownError");
        private readonly string UTF32String = ResourceService.DataEncryptResource.GetString("UTF32");
        private readonly string UTF7String = ResourceService.DataEncryptResource.GetString("UTF7");
        private readonly string UTF8String = ResourceService.DataEncryptResource.GetString("UTF8");
        private readonly string XORString = ResourceService.DataEncryptResource.GetString("XOR");
        private readonly string ZerosString = ResourceService.DataEncryptResource.GetString("Zeros");
        private int selectEncryptIndex = -1;
        private string selectedEncryptFile = string.Empty;
        private string inputtedEncryptContent = string.Empty;
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

        private string _encryptFailedInformation;

        public string EncryptFailedInformation
        {
            get { return _encryptFailedInformation; }

            set
            {
                if (!string.Equals(_encryptFailedInformation, value))
                {
                    _encryptFailedInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptFailedInformation)));
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

        private bool _hasEncryptOptions;

        public bool HasEncryptOptions
        {
            get { return _hasEncryptOptions; }

            set
            {
                if (!Equals(_hasEncryptOptions, value))
                {
                    _hasEncryptOptions = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasEncryptOptions)));
                }
            }
        }

        private bool _hasEncryptKey;

        public bool HasEncryptKey
        {
            get { return _hasEncryptKey; }

            set
            {
                if (!Equals(_hasEncryptKey, value))
                {
                    _hasEncryptKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasEncryptKey)));
                }
            }
        }

        private bool _hasEncryptKeyStringType;

        public bool HasEncryptKeyStringType
        {
            get { return _hasEncryptKeyStringType; }

            set
            {
                if (!Equals(_hasEncryptKeyStringType, value))
                {
                    _hasEncryptKeyStringType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasEncryptKeyStringType)));
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

        private string _encryptKeyPHText = string.Empty;

        public string EncryptKeyPHText
        {
            get { return _encryptKeyPHText; }

            set
            {
                if (!string.Equals(_encryptKeyPHText, value))
                {
                    _encryptKeyPHText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptKeyPHText)));
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

        private bool _hasEncryptedBlockCipherMode;

        public bool HasEncryptedBlockCipherMode
        {
            get { return _hasEncryptedBlockCipherMode; }

            set
            {
                if (!Equals(_hasEncryptedBlockCipherMode, value))
                {
                    _hasEncryptedBlockCipherMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasEncryptedBlockCipherMode)));
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

        private bool _hasEncryptPublicKey;

        public bool HasEncryptPublicKey
        {
            get { return _hasEncryptPublicKey; }

            set
            {
                if (!Equals(_hasEncryptPublicKey, value))
                {
                    _hasEncryptPublicKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasEncryptPublicKey)));
                }
            }
        }

        private string _encryptPublicKeyText;

        public string EncryptPublicKeyText
        {
            get { return _encryptPublicKeyText; }

            set
            {
                if (!Equals(_encryptPublicKeyText, value))
                {
                    _encryptPublicKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptPublicKeyText)));
                }
            }
        }

        private bool _hasEncryptPrivateKey;

        public bool HasEncryptPrivateKey
        {
            get { return _hasEncryptPrivateKey; }

            set
            {
                if (!Equals(_hasEncryptPrivateKey, value))
                {
                    _hasEncryptPrivateKey = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasEncryptPrivateKey)));
                }
            }
        }

        private string _encryptPrivateKeyText;

        public string EncryptPrivateKeyText
        {
            get { return _encryptPrivateKeyText; }

            set
            {
                if (!Equals(_encryptPrivateKeyText, value))
                {
                    _encryptPrivateKeyText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptPrivateKeyText)));
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

        private bool _hasRSAEncryptionOtherOptions;

        public bool HasRSAEncryptionOtherOptions
        {
            get { return _hasRSAEncryptionOtherOptions; }

            set
            {
                if (!Equals(_hasRSAEncryptionOtherOptions, value))
                {
                    _hasRSAEncryptionOtherOptions = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasRSAEncryptionOtherOptions)));
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

        private bool _saveEncryptedDataToLocalFile;

        public bool SaveEncryptedDataToLocalFile
        {
            get { return _saveEncryptedDataToLocalFile; }

            set
            {
                if (!Equals(_saveEncryptedDataToLocalFile, value))
                {
                    _saveEncryptedDataToLocalFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveEncryptedDataToLocalFile)));
                }
            }
        }

        private string _saveEncryptedFilePath = string.Empty;

        public string SaveEncryptedFilePath
        {
            get { return _saveEncryptedFilePath; }

            set
            {
                if (!Equals(_saveEncryptedFilePath, value))
                {
                    _saveEncryptedFilePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveEncryptedFilePath)));
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

        private List<KeyValuePair<RSAEncryptionPaddingMode, string>> RSAEncryptionPaddingModeList { get; } = [];

        private List<KeyValuePair<string, string>> TextEncodingTypeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataEncryptPage()
        {
            InitializeComponent();
            DataEncryptTypeList.Add(new DataEncryptTypeModel()
            {
                DataEncryptType = DataEncryptType.AES,
                Name = AESString
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
            EncryptKeyStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            EncryptKeyStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>(nameof(Encoding.UTF8), UTF8String));
            InitializationVectorStringTypeList.Add(new KeyValuePair<string, string>("Base64", Base64String));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CBC, CBCString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.ECB, ECBString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.OFB, OFBString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CFB, CFBString));
            EncryptedBlockCipherModeList.Add(new KeyValuePair<CipherMode, string>(CipherMode.CTS, CTSString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.None, NonePaddingString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.PKCS7, Pkcs7String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.Zeros, ZerosString));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ANSIX923, ANSIX923String));
            PaddingModeList.Add(new KeyValuePair<PaddingMode, string>(PaddingMode.ISO10126, ISO10126String));
            RSAEncryptionPaddingModeList.Add(new KeyValuePair<RSAEncryptionPaddingMode, string>(RSAEncryptionPaddingMode.Pkcs1, Pkcs1String));
            RSAEncryptionPaddingModeList.Add(new KeyValuePair<RSAEncryptionPaddingMode, string>(RSAEncryptionPaddingMode.Oaep, OaepString));
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
        }

        #region 第一部分：数据加密页面——挂载的事件

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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnDragOver), 1, e);
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnDrop), 1, e);
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
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnDrop), 2, e);
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
                if (!IsEncrypting && SelectedIndex is 0 && ResultSeverity is InfoBarSeverity.Informational)
                {
                    ResultMessage = FileInitializeString;
                }
                else if (!IsEncrypting && SelectedIndex is 1 && ResultSeverity is InfoBarSeverity.Informational)
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
        /// 加密内容发生改变时触发的事件
        /// </summary>
        private void OnEncryptContentTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                EncryptContent = textBox.Text;
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

            if (!string.IsNullOrEmpty(EncryptFailedInformation))
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(EncryptFailedInformation);

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
        /// 数据加密类型选中项发生改变时触发的事件
        /// </summary>
        private void OnEncryptSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is DataEncryptTypeModel dataEncryptType)
            {
                SelectedDataEncryptType = dataEncryptType;
                EncryptKeyText = string.Empty;
                InitializationVectorText = string.Empty;

                switch (SelectedDataEncryptType.DataEncryptType)
                {
                    case DataEncryptType.AES:
                        {
                            EncryptKeyPHText = EncryptKey162432SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.CaesarCipher:
                        {
                            SelectedIndex = 1;
                            EncryptKeyPHText = string.Empty;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = false;
                            HasEncryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasEncryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = true;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.ChaCha20:
                        {
                            EncryptKeyPHText = EncryptKey32SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector12SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = true;
                            HasEncryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.DES:
                        {
                            EncryptKeyPHText = EncryptKey8SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.MorseCode:
                        {
                            SelectedIndex = 1;
                            EncryptKeyPHText = string.Empty;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = false;
                            HasEncryptKey = false;
                            HasEncryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasEncryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.Rabbit:
                        {
                            EncryptKeyPHText = EncryptKey16SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = true;
                            HasEncryptedBlockCipherMode = false;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.RC2:
                        {
                            EncryptKeyPHText = EncryptKey16SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.RC4:
                        {
                            EncryptKeyPHText = EncryptKey16SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = false;
                            HasEncryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.RC5:
                        {
                            EncryptKeyPHText = EncryptKey16SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector8SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.RC6:
                        {
                            EncryptKeyPHText = EncryptKey16SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.Rijndael:
                        {
                            EncryptKeyPHText = EncryptKey162432SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.RSA:
                        {
                            EncryptKeyPHText = string.Empty;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = false;
                            HasEncryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasEncryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasEncryptPublicKey = true;
                            HasEncryptPrivateKey = true;
                            HasRSAEncryptionPaddingMode = true;
                            HasRSAEncryptionOtherOptions = true;
                            break;
                        }
                    case DataEncryptType.SM4:
                        {
                            EncryptKeyPHText = EncryptKey16SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.TripleDES:
                        {
                            EncryptKeyPHText = EncryptKey1624SizeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = InitializationVector16SizeString;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = true;
                            HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                            HasEncryptedBlockCipherMode = true;
                            HasPaddingMode = true;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
                            break;
                        }
                    case DataEncryptType.XOR:
                        {
                            SelectedIndex = 1;
                            EncryptKeyPHText = EncryptKeyInitializeString;
                            EncryptKeyText = string.Empty;
                            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[0];
                            InitializationVectorPHText = string.Empty;
                            InitializationVectorText = string.Empty;
                            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[0];
                            SelectedEncryptedBlockCipherMode = EncryptedBlockCipherModeList[0];
                            SelectedPaddingMode = PaddingModeList[0];
                            SelectedRSAEncryptionPaddingMode = RSAEncryptionPaddingModeList[0];
                            Offset = 0;
                            EncryptPublicKeyText = string.Empty;
                            EncryptPrivateKeyText = string.Empty;
                            HasEncryptOptions = true;
                            HasEncryptKey = true;
                            HasEncryptKeyStringType = false;
                            HasInitializationVector = false;
                            HasEncryptedBlockCipherMode = false;
                            HasPaddingMode = false;
                            HasOffset = false;
                            HasEncryptPublicKey = false;
                            HasEncryptPrivateKey = false;
                            HasRSAEncryptionPaddingMode = false;
                            HasRSAEncryptionOtherOptions = false;
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
        /// 生成随机密钥
        /// </summary>
        private async void OnGenerateRandomKeyClicked(object sender, RoutedEventArgs args)
        {
            SelectedEncryptKeyStringType = EncryptKeyStringTypeList[1];
            EncryptKeyText = await Task.Run(() =>
            {
                string encryptKey = string.Empty;

                try
                {
                    if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.AES)
                    {
                        // 默认密钥支持 16 字节，24 字节和 32 字节，目前使用 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.ChaCha20)
                    {
                        // 默认密钥支持 32 字节
                        byte[] keyBytes = new byte[32];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.DES)
                    {
                        // 默认密钥支持 8 字节
                        byte[] keyBytes = new byte[8];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.Rabbit)
                    {
                        // 默认密钥支持 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC2)
                    {
                        // 默认密钥支持 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC4)
                    {
                        // 默认密钥支持 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC5)
                    {
                        // 默认密钥支持 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC6)
                    {
                        // 默认密钥支持 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.Rijndael)
                    {
                        // 默认密钥支持 16 字节，24 字节和 32 字节，目前使用 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.SM4)
                    {
                        // 默认密钥支持 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.TripleDES)
                    {
                        // 默认密钥支持 16 字节和 24 字节，目前使用 16 字节
                        byte[] keyBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(keyBytes);
                        encryptKey = Convert.ToBase64String(keyBytes);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnGenerateRandomKeyClicked), 1, e);
                }

                return encryptKey;
            });
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
        /// 生成初始化向量
        /// </summary>
        private async void OnGenerateInitializationVectorClicked(object sender, RoutedEventArgs args)
        {
            SelectedInitializationVectorStringType = InitializationVectorStringTypeList[1];
            InitializationVectorText = await Task.Run(() =>
            {
                string initializationVector = string.Empty;

                try
                {
                    if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.AES)
                    {
                        // 初始化向量支持 16 字节
                        byte[] initializationVectorBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.ChaCha20)
                    {
                        // 初始化向量支持 12 字节
                        byte[] initializationVectorBytes = new byte[12];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.DES)
                    {
                        // 初始化向量支持 8 字节
                        byte[] initializationVectorBytes = new byte[8];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.Rabbit)
                    {
                        // 初始化向量支持 8 字节
                        byte[] initializationVectorBytes = new byte[8];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC2)
                    {
                        // 初始化向量支持 8 字节
                        byte[] initializationVectorBytes = new byte[8];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC5)
                    {
                        // 初始化向量支持 8 字节
                        byte[] initializationVectorBytes = new byte[8];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC6)
                    {
                        // 初始化向量支持 16 字节
                        byte[] initializationVectorBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.Rijndael)
                    {
                        // 初始化向量支持 16 字节
                        byte[] initializationVectorBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.SM4)
                    {
                        // 初始化向量支持 16 字节
                        byte[] initializationVectorBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                    else if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.TripleDES)
                    {
                        // 初始化向量支持 16 字节
                        byte[] initializationVectorBytes = new byte[16];
                        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                        randomNumberGenerator.GetBytes(initializationVectorBytes);
                        initializationVector = Convert.ToBase64String(initializationVectorBytes);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnGenerateInitializationVectorClicked), 1, e);
                }

                return initializationVector;
            });
        }

        /// <summary>
        /// 加密块密码模式发生变化时触发的事件
        /// </summary>
        private void OnEncryptedBlockCipherModeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<CipherMode, string> encryptedBlockCipherMode)
            {
                SelectedEncryptedBlockCipherMode = encryptedBlockCipherMode;
                if (SelectedDataEncryptType.DataEncryptType is DataEncryptType.AES || SelectedDataEncryptType.DataEncryptType is DataEncryptType.DES || SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC2 ||
                    SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC5 || SelectedDataEncryptType.DataEncryptType is DataEncryptType.RC6 || SelectedDataEncryptType.DataEncryptType is DataEncryptType.Rijndael ||
                    SelectedDataEncryptType.DataEncryptType is DataEncryptType.SM4 || SelectedDataEncryptType.DataEncryptType is DataEncryptType.TripleDES)
                {
                    HasInitializationVector = SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB;
                }
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
        /// 加密公钥内容发生变化时触发的事件
        /// </summary>
        private void OnEncryptPublicKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                EncryptPublicKeyText = textBox.Text;
            }
        }

        /// <summary>
        /// 加密私钥内容发生变化时触发的事件
        /// </summary>
        private void OnEncryptPrivateKeyTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBox textBox)
            {
                EncryptPrivateKeyText = textBox.Text;
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
        /// 生成 RSA 加密算法密钥对
        /// </summary>
        private async void OnGenerateRandomRSAKeyPairClicked(object sender, RoutedEventArgs args)
        {
            (string publicKey, string privateKey) = await Task.Run(() =>
            {
                string publicKey = string.Empty;
                string privateKey = string.Empty;

                try
                {
                    RSA rsa = RSA.Create();
                    publicKey = rsa.ToXmlString(false);
                    privateKey = rsa.ToXmlString(true);
                    rsa.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnGenerateRandomRSAKeyPairClicked), 1, e);
                }

                return ValueTuple.Create(publicKey, privateKey);
            });

            if (!string.IsNullOrEmpty(publicKey))
            {
                EncryptPublicKeyText = publicKey;
            }

            if (!string.IsNullOrEmpty(privateKey))
            {
                EncryptPrivateKeyText = privateKey;
            }
        }

        /// <summary>
        /// 复制 RSA 加密算法公钥到剪贴板
        /// </summary>
        private async void OnCopyRSAPublicKeyClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(EncryptPublicKeyText))
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(EncryptPublicKeyText);

                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制 RSA 加密算法私钥到剪贴板
        /// </summary>
        private async void OnCopyRSAPrivateKeyClicked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(EncryptPrivateKeyText))
            {
                bool copyResult = CopyPasteHelper.CopyToClipboard(EncryptPrivateKeyText);

                await MainWindow.Current.ShowNotificationAsync(new CopyPasteNotificationTip(copyResult));
            }
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
        /// 保存解密的数据到本地文件中
        /// </summary>
        private void OnSaveEncryptedDataToLocalFileToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SaveEncryptedDataToLocalFile = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 打开文件所对应的文件夹
        /// </summary>
        private void OnSaveEncryptedFilePathClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(SaveEncryptedFilePath))
                    {
                        if (File.Exists(SaveEncryptedFilePath))
                        {
                            nint pidlList = Shell32Library.ILCreateFromPath(SaveEncryptedFilePath);
                            if (pidlList is not 0)
                            {
                                Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, 0, 0);
                                Shell32Library.ILFree(pidlList);
                            }
                        }
                        else
                        {
                            string directoryPath = Path.GetDirectoryName(SaveEncryptedFilePath);

                            if (Directory.Exists(directoryPath))
                            {
                                Process.Start(directoryPath);
                            }
                            else
                            {
                                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnSaveEncryptedFilePathClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 设置文件存储路径
        /// </summary>
        private void OnSetFilePathClicked(object sender, RoutedEventArgs args)
        {
            SaveFileDialog saveFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = string.Format(".*|{0}", AllFilesString),
                DefaultExt = ".*",
                Title = SelectFileString
            };
            if (saveFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                SaveEncryptedFilePath = saveFileDialog.FileName;
            }
            saveFileDialog.Dispose();
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
        /// 开始数据加密
        /// </summary>
        private async void OnStartEncryptClicked(object sender, RoutedEventArgs args)
        {
            selectEncryptIndex = SelectedIndex;
            selectedEncryptFile = EncryptFile;
            inputtedEncryptContent = EncryptContent;
            EncryptFailedInformation = string.Empty;
            EncryptResult = string.Empty;
            encryptedLocalFile = string.Empty;
            IsLargeContent = false;

            // 检查要加密的文件
            if (selectEncryptIndex is 0)
            {
                if (string.IsNullOrEmpty(selectedEncryptFile))
                {
                    ResultSeverity = InfoBarSeverity.Error;
                    ResultMessage = FileNotSelectedString;
                    return;
                }
                else if (!File.Exists(selectedEncryptFile))
                {
                    ResultSeverity = InfoBarSeverity.Error;
                    ResultMessage = FileNotExistedString;
                    return;
                }
            }
            // 检查要加密的字符串
            else if (selectEncryptIndex is 1)
            {
                if (string.IsNullOrEmpty(inputtedEncryptContent))
                {
                    ResultSeverity = InfoBarSeverity.Error;
                    ResultMessage = ContentEmptyString;
                    return;
                }
            }
            else
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = SelectedDataEncryptInputContentFormatErrorString;
                return;
            }

            // 选择加密的算法为空时的提示
            if (SelectedDataEncryptType is null)
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = EncryptTypeNotSelectedString;
                return;
            }
            else
            {
                // 检查对应的加密算法所填充的加密密钥或初始化向量是否为空
                switch (SelectedDataEncryptType.DataEncryptType)
                {
                    case DataEncryptType.AES:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.CaesarCipher:
                        {
                            if (selectEncryptIndex is not 1)
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptTypeMustContentString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.ChaCha20:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.DES:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.MorseCode:
                        {
                            if (selectEncryptIndex is not 1)
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptTypeMustContentString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.Rabbit:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.RC2:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.RC4:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.RC5:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.RC6:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.Rijndael:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.RSA:
                        {
                            if (string.IsNullOrEmpty(EncryptPublicKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptPublicKeyEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.SM4:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.TripleDES:
                        {
                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }

                            if (SelectedEncryptedBlockCipherMode.Key is not CipherMode.ECB && string.IsNullOrEmpty(InitializationVectorText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = InitializationVectorEmptyString;
                                return;
                            }
                            break;
                        }
                    case DataEncryptType.XOR:
                        {
                            if (selectEncryptIndex is not 1)
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptTypeMustContentString;
                                return;
                            }

                            if (string.IsNullOrEmpty(EncryptKeyText))
                            {
                                ResultSeverity = InfoBarSeverity.Error;
                                ResultMessage = EncryptKeyEmptyString;
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            // 获取具体的文字编码选项
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
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnStartEncryptClicked), 1, e);
                    }
                }
                else
                {
                    textEncoding = Encoding.GetEncoding(SelectedTextEncodingType.Key);
                }
                return textEncoding;
            });

            // 文字编码异常时的提示
            if (textEncoding is null)
            {
                ResultSeverity = InfoBarSeverity.Error;
                ResultMessage = TextEncodingInvalidString;
                return;
            }

            // 加密内容
            IsEncrypting = true;
            ResultSeverity = InfoBarSeverity.Informational;
            ResultMessage = EncryptingString;
            (string encryptedData, Exception encryptException) = await Task.Run(() =>
            {
                return GetEncryptedData(SelectedDataEncryptType.DataEncryptType, selectEncryptIndex, inputtedEncryptContent, textEncoding, selectedEncryptFile);
            });

            // 加密失败
            if (encryptException is not null)
            {
                ResultSeverity = InfoBarSeverity.Error;
                if (selectEncryptIndex is 0)
                {
                    ResultMessage = FileEncryptFailedString;
                }
                else if (selectEncryptIndex is 1)
                {
                    ResultMessage = ContentEncryptFailedString;
                }

                EncryptFailedInformation = !string.IsNullOrEmpty(encryptException.Message) ? encryptException.Message : UnknownErrorString;
            }
            // 加密成功
            else
            {
                bool isSavedToSelectedFile = false;
                bool isSavedToTempFile = false;
                Exception exception = null;
                if (SaveEncryptedDataToLocalFile)
                {
                    await Task.Run(() =>
                    {
                        // 保存到选择的本地文件中
                        try
                        {
                            encryptedLocalFile = SaveEncryptedFilePath;
                            File.WriteAllText(encryptedLocalFile, UseUpperCase ? encryptedData.ToUpperInvariant() : encryptedData.ToLowerInvariant());
                            isSavedToSelectedFile = true;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnStartEncryptClicked), 2, e);
                            exception = e;
                        }

                        // 保存到选定的本地文件失败，自动保存到临时文件目录中
                        if (!isSavedToSelectedFile)
                        {
                            try
                            {
                                encryptedLocalFile = Path.GetTempFileName();
                                File.WriteAllText(encryptedLocalFile, UseUpperCase ? encryptedData.ToUpperInvariant() : encryptedData.ToLowerInvariant());
                                isSavedToTempFile = true;
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnStartEncryptClicked), 3, e);
                                exception = e;
                            }
                        }
                    });
                }

                // 加密后的字符串数据太长
                if (encryptedData.Length > 1024)
                {
                    EncryptResult = LargeContentString;
                    IsLargeContent = true;

                    if (!SaveEncryptedDataToLocalFile)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                encryptedLocalFile = Path.GetTempFileName();
                                File.WriteAllText(encryptedLocalFile, UseUpperCase ? encryptedData.ToUpperInvariant() : encryptedData.ToLowerInvariant());
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnStartEncryptClicked), 4, e);
                            }
                        });
                    }
                }
                else
                {
                    EncryptResult = UseUpperCase ? encryptedData.ToUpperInvariant() : encryptedData.ToLowerInvariant();
                    IsLargeContent = false;
                }

                // 显示加密结果
                if (selectEncryptIndex is 0)
                {
                    if (SaveEncryptedDataToLocalFile)
                    {
                        if (isSavedToSelectedFile)
                        {
                            ResultSeverity = InfoBarSeverity.Success;
                            ResultMessage = FileEncryptedDataSaveSuccessfullyString;
                            EncryptFailedInformation = string.Empty;
                        }
                        else
                        {
                            ResultSeverity = isSavedToTempFile ? InfoBarSeverity.Warning : InfoBarSeverity.Error;
                            EncryptFailedInformation = exception is not null && !string.IsNullOrEmpty(exception.Message) ? exception.Message : UnknownErrorString;
                            ResultMessage = isSavedToTempFile ? FileEncryptedDataSaveFailedToTempFileString : FileEncryptedDataSaveFailedString;
                        }
                    }
                    else
                    {
                        ResultSeverity = InfoBarSeverity.Success;
                        ResultMessage = FileEncryptSuccessfullyString;
                        EncryptFailedInformation = string.Empty;
                    }
                }
                else if (selectEncryptIndex is 1)
                {
                    if (SaveEncryptedDataToLocalFile)
                    {
                        if (isSavedToSelectedFile)
                        {
                            ResultSeverity = InfoBarSeverity.Warning;
                            ResultMessage = ContentEncryptedDataSaveSuccessfullyString;
                            EncryptFailedInformation = string.Empty;
                        }
                        else
                        {
                            ResultSeverity = isSavedToTempFile ? InfoBarSeverity.Warning : InfoBarSeverity.Error;
                            EncryptFailedInformation = exception is not null && !string.IsNullOrEmpty(exception.Message) ? exception.Message : UnknownErrorString;
                            ResultMessage = isSavedToTempFile ? ContentEncryptedDataSaveFailedToTempFileString : ContentEncryptedDataSaveFailedString;
                        }
                    }
                    else
                    {
                        ResultSeverity = InfoBarSeverity.Success;
                        ResultMessage = ContentEncryptSuccessfullyString;
                        EncryptFailedInformation = string.Empty;
                    }
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
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(OnViewLocalFileClicked), 1, e);
                }
            });
        }

        #endregion 第一部分：数据加密页面——挂载的事件

        /// <summary>
        /// 获取加密后的数据
        /// </summary>
        private (string, Exception) GetEncryptedData(DataEncryptType dataEncryptType, int selectedEncryptIndex, string contentData, Encoding textEncoding, string encryptFile)
        {
            string encryptedData = string.Empty;
            Exception encryptException = null;

            switch (dataEncryptType)
            {
                case DataEncryptType.AES:
                    {
                        try
                        {
                            Aes aes = Aes.Create();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    aes.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    aes.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    aes.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    aes.IV = Convert.FromBase64String(InitializationVectorText);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.AES) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.CaesarCipher:
                    {
                        if (selectedEncryptIndex is 1)
                        {
                            try
                            {
                                encryptedData = CaesarCipher.CaesarEncrypt(contentData, Offset);
                            }
                            catch (Exception e)
                            {
                                encryptException = e;
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.CaesarCipher) + 1, e);
                            }
                        }
                        break;
                    }
                case DataEncryptType.ChaCha20:
                    {
                        try
                        {
                            byte[] encryptedKey = null;
                            byte[] encryptedIV = null;
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    encryptedKey = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    encryptedKey = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    encryptedIV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    encryptedIV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            byte[] data = null;
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                data = new byte[(int)fileStream.Length];
                                fileStream.Read(data, 0, data.Length);
                                fileStream.Dispose();
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                data = textEncoding.GetBytes(contentData);
                            }

                            if (data is not null)
                            {
                                encryptedData = Convert.ToBase64String(ChaCha20.Encrypt(encryptedKey, encryptedIV, data));
                            }
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.ChaCha20) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.DES:
                    {
                        try
                        {
                            DES des = DES.Create();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    des.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    des.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    des.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    des.IV = Convert.FromBase64String(InitializationVectorText);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.DES) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.MorseCode:
                    {
                        if (selectedEncryptIndex is 1)
                        {
                            try
                            {
                                // 隐藏选项
                                encryptedData = MorseCode.MorseEncode(contentData.ToUpperInvariant());
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.MorseCode) + 1, e);
                            }
                        }
                        break;
                    }
                case DataEncryptType.Rabbit:
                    {
                        try
                        {
                            Rabbit rabbit = new();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rabbit.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rabbit.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rabbit.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rabbit.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rabbit.Mode = CipherMode.CBC;
                            rabbit.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = rabbit.CreateEncryptor(rabbit.Key, rabbit.IV);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.Rabbit) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RC2:
                    {
                        try
                        {
                            RC2 rc2 = RC2.Create();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rc2.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rc2.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc2.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc2.IV = Convert.FromBase64String(InitializationVectorText);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.RC2) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RC4:
                    {
                        try
                        {
                            RC4 rc4 = RC4.Create();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rc4.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rc4.Key = Convert.FromBase64String(EncryptKeyText);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.RC4) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RC5:
                    {
                        try
                        {
                            RC5 rc5 = new();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rc5.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rc5.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc5.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc5.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rc5.Mode = SelectedEncryptedBlockCipherMode.Key;
                            rc5.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = rc5.CreateEncryptor(rc5.Key, rc5.IV);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.RC5) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RC6:
                    {
                        try
                        {
                            RC6 rc6 = new();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rc6.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rc6.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rc6.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rc6.IV = Convert.FromBase64String(InitializationVectorText);
                                }
                            }

                            rc6.Mode = SelectedEncryptedBlockCipherMode.Key;
                            rc6.Padding = SelectedPaddingMode.Key;
                            ICryptoTransform cryptoTransform = rc6.CreateEncryptor(rc6.Key, rc6.IV);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.RC6) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.Rijndael:
                    {
                        try
                        {
                            Rijndael rijndael = Rijndael.Create();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    rijndael.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    rijndael.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    rijndael.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    rijndael.IV = Convert.FromBase64String(InitializationVectorText);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.Rijndael) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.RSA:
                    {
                        try
                        {
                            RSA rsa = RSA.Create();
                            if (selectedEncryptIndex is 0 && File.Exists(selectedEncryptFile))
                            {
                                FileStream fileStream = File.OpenRead(selectedEncryptFile);
                                byte[] data = new byte[(int)fileStream.Length];
                                fileStream.Read(data, 0, data.Length);
                                fileStream.Dispose();
                                rsa.FromXmlString(EncryptPublicKeyText);
                                byte[] encryptedOutput = null;
                                if (SelectedRSAEncryptionPaddingMode.Key is RSAEncryptionPaddingMode.Pkcs1)
                                {
                                    encryptedOutput = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                                }
                                else if (SelectedRSAEncryptionPaddingMode.Key is RSAEncryptionPaddingMode.Oaep)
                                {
                                    encryptedOutput = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
                                }

                                if (encryptedOutput is not null)
                                {
                                    encryptedData = Convert.ToBase64String(encryptedOutput);
                                }
                            }
                            else if (selectedEncryptIndex is 1)
                            {
                                byte[] data = textEncoding.GetBytes(contentData);
                                rsa.FromXmlString(EncryptPublicKeyText);
                                byte[] encryptedOutput = null;
                                if (SelectedRSAEncryptionPaddingMode.Key is RSAEncryptionPaddingMode.Pkcs1)
                                {
                                    encryptedOutput = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                                }
                                else if (SelectedRSAEncryptionPaddingMode.Key is RSAEncryptionPaddingMode.Oaep)
                                {
                                    encryptedOutput = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
                                }

                                if (encryptedOutput is not null)
                                {
                                    encryptedData = Convert.ToBase64String(encryptedOutput);
                                }
                            }
                            rsa.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.RSA) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.SM4:
                    {
                        try
                        {
                            SM4 sm4 = new();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    sm4.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    sm4.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    sm4.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    sm4.IV = Convert.FromBase64String(InitializationVectorText);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.SM4) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.TripleDES:
                    {
                        try
                        {
                            TripleDES tripleDes = TripleDES.Create();
                            if (!string.IsNullOrEmpty(EncryptKeyText))
                            {
                                if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[0]))
                                {
                                    tripleDes.Key = Encoding.UTF8.GetBytes(EncryptKeyText);
                                }
                                else if (Equals(SelectedEncryptKeyStringType, EncryptKeyStringTypeList[1]))
                                {
                                    tripleDes.Key = Convert.FromBase64String(EncryptKeyText);
                                }
                            }

                            if (!string.IsNullOrEmpty(InitializationVectorText))
                            {
                                if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[0]))
                                {
                                    tripleDes.IV = Encoding.UTF8.GetBytes(InitializationVectorText);
                                }
                                else if (Equals(SelectedInitializationVectorStringType, InitializationVectorStringTypeList[1]))
                                {
                                    tripleDes.IV = Convert.FromBase64String(InitializationVectorText);
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
                                byte[] data = textEncoding.GetBytes(contentData);
                                cryptoStream.Write(data, 0, data.Length);
                                cryptoStream.FlushFinalBlock();
                            }
                            encryptedData = Convert.ToBase64String(memoryStream.ToArray());
                            cryptoStream.Dispose();
                            memoryStream.Dispose();
                            cryptoTransform.Dispose();
                        }
                        catch (Exception e)
                        {
                            encryptException = e;
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.TripleDES) + 1, e);
                        }
                        break;
                    }
                case DataEncryptType.XOR:
                    {
                        if (selectedEncryptIndex is 1)
                        {
                            try
                            {
                                encryptedData = XOR.XOREncrypt(contentData, EncryptKeyText);
                            }
                            catch (Exception e)
                            {
                                encryptException = e;
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DataEncryptPage), nameof(GetEncryptedData), Convert.ToInt32(DataEncryptType.XOR) + 1, e);
                            }
                        }
                        break;
                    }
            }
            return ValueTuple.Create(encryptedData, encryptException);
        }

        /// <summary>
        /// 获取要加密的类型
        /// </summary>
        private Visibility GetDataEncryptType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取文字编码选项控件可见状态
        /// </summary>
        private Visibility GetTextEncodingVisibleState(int selectedIndex, DataEncryptType selectedDataEncryptType)
        {
            return selectedIndex is 1 ? selectedDataEncryptType.Equals(DataEncryptTypeList[Convert.ToInt32(DataEncryptType.CaesarCipher)].DataEncryptType) || selectedDataEncryptType.Equals(DataEncryptTypeList[Convert.ToInt32(DataEncryptType.MorseCode)].DataEncryptType) || selectedDataEncryptType.Equals(DataEncryptTypeList[Convert.ToInt32(DataEncryptType.XOR)].DataEncryptType) ? Visibility.Collapsed : Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取对应的文字编码类型
        /// </summary>
        private Visibility GetTextEncodingType(string textEncodingType, string comparedTextEncodingType)
        {
            return string.Equals(textEncodingType, comparedTextEncodingType) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取加密结果
        /// </summary>
        private Visibility GetEncryptResult(InfoBarSeverity resultSeverity)
        {
            return resultSeverity is InfoBarSeverity.Success ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetEncryptInfoButtonHitTestState(InfoBarSeverity resultSeverity, bool isEncrypting)
        {
            return (resultSeverity is InfoBarSeverity.Warning || resultSeverity is InfoBarSeverity.Error) && !isEncrypting;
        }

        private Visibility GetEncryptInfoButtonVisibility(InfoBarSeverity resultSeverity)
        {
            return resultSeverity is InfoBarSeverity.Informational || resultSeverity is InfoBarSeverity.Warning || resultSeverity is InfoBarSeverity.Error ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetEncryptInfoProgressRingActiveState(InfoBarSeverity resultSeverity, bool isEncrypting)
        {
            return resultSeverity is InfoBarSeverity.Informational && isEncrypting;
        }

        private Visibility GetEncryptFailedInfoButtonState(InfoBarSeverity resultSeverity, bool isEncrypting, string encryptFailedInformation)
        {
            return (resultSeverity is InfoBarSeverity.Warning || resultSeverity is InfoBarSeverity.Error) && !isEncrypting && !string.IsNullOrEmpty(encryptFailedInformation) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
