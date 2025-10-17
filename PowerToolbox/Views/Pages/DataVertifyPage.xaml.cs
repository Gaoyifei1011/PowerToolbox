using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据校验页面
    /// </summary>
    public sealed partial class DataVertifyPage : Page, INotifyPropertyChanged
    {
        private readonly string AICHString = ResourceService.DataVertifyResource.GetString("AICH");
        private readonly string BLAKE2spString = ResourceService.DataVertifyResource.GetString("BLAKE2sp");
        private readonly string BLAKE3String = ResourceService.DataVertifyResource.GetString("BLAKE3");
        private readonly string BTIHString = ResourceService.DataVertifyResource.GetString("BTIH");
        private readonly string ContentInitializeString = ResourceService.DataVertifyResource.GetString("ContentInitialize");
        private readonly string ContentVertifyFailedString = ResourceService.DataVertifyResource.GetString("ContentVertifyFailed");
        private readonly string ContentVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyPartSuccessfully");
        private readonly string ContentVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyWholeSuccessfully");
        private readonly string CRC32String = ResourceService.DataVertifyResource.GetString("CRC32");
        private readonly string CRC64String = ResourceService.DataVertifyResource.GetString("CRC64");
        private readonly string ED2KString = ResourceService.DataVertifyResource.GetString("ED2K");
        private readonly string EDONR224String = ResourceService.DataVertifyResource.GetString("EDONR224");
        private readonly string EDONR256String = ResourceService.DataVertifyResource.GetString("EDONR256");
        private readonly string EDONR384String = ResourceService.DataVertifyResource.GetString("EDONR384");
        private readonly string EDONR512String = ResourceService.DataVertifyResource.GetString("EDONR512");
        private readonly string FileInitializeString = ResourceService.DataVertifyResource.GetString("FileInitialize");
        private readonly string FileVertifyFailedString = ResourceService.DataVertifyResource.GetString("FileVertifyFailed");
        private readonly string FileVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyPartSuccessfully");
        private readonly string FileVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyWholeSuccessfully");
        private readonly string GOST12256String = ResourceService.DataVertifyResource.GetString("GOST12256");
        private readonly string GOST12512String = ResourceService.DataVertifyResource.GetString("GOST12512");
        private readonly string GOST94String = ResourceService.DataVertifyResource.GetString("GOST94");
        private readonly string GOST94CryptoProString = ResourceService.DataVertifyResource.GetString("GOST94CryptoPro");
        private readonly string HAS160String = ResourceService.DataVertifyResource.GetString("HAS160");
        private readonly string MD2String = ResourceService.DataVertifyResource.GetString("MD2");
        private readonly string MD4String = ResourceService.DataVertifyResource.GetString("MD4");
        private readonly string MD5String = ResourceService.DataVertifyResource.GetString("MD5");
        private readonly string RIPEMD160String = ResourceService.DataVertifyResource.GetString("RIPEMD160");
        private readonly string SHA1String = ResourceService.DataVertifyResource.GetString("SHA1");
        private readonly string SHA224String = ResourceService.DataVertifyResource.GetString("SHA224");
        private readonly string SHA256String = ResourceService.DataVertifyResource.GetString("SHA256");
        private readonly string SHA3224String = ResourceService.DataVertifyResource.GetString("SHA3224");
        private readonly string SHA3256String = ResourceService.DataVertifyResource.GetString("SHA3256");
        private readonly string SHA3384String = ResourceService.DataVertifyResource.GetString("SHA3384");
        private readonly string SHA3512String = ResourceService.DataVertifyResource.GetString("SHA3512");
        private readonly string SHA384String = ResourceService.DataVertifyResource.GetString("SHA384");
        private readonly string SHA512String = ResourceService.DataVertifyResource.GetString("SHA512");
        private readonly string SM3String = ResourceService.DataVertifyResource.GetString("SM3");
        private readonly string SNEFRU128String = ResourceService.DataVertifyResource.GetString("SNEFRU128");
        private readonly string SNEFRU256String = ResourceService.DataVertifyResource.GetString("SNEFRU256");
        private readonly string TIGERString = ResourceService.DataVertifyResource.GetString("TIGER");
        private readonly string TIGER2String = ResourceService.DataVertifyResource.GetString("TIGER2");
        private readonly string TTHSString = ResourceService.DataVertifyResource.GetString("TTHS");
        private readonly string VertifyingString = ResourceService.DataVertifyResource.GetString("Vertifying");
        private readonly string WHIRLPOOLString = ResourceService.DataVertifyResource.GetString("WHIRLPOOL");
        private readonly string XXH128String = ResourceService.DataVertifyResource.GetString("XXH128");
        private readonly string XXH3String = ResourceService.DataVertifyResource.GetString("XXH3");
        private readonly string XXH32String = ResourceService.DataVertifyResource.GetString("XXH32");
        private readonly string XXH64String = ResourceService.DataVertifyResource.GetString("XXH64");

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

        private string _vertifyFile;

        public string VertifyFile
        {
            get { return _vertifyFile; }

            set
            {
                if (!Equals(_vertifyFile, value))
                {
                    _vertifyFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VertifyFile)));
                }
            }
        }

        private string _vertifyContent;

        public string VertifyContent
        {
            get { return _vertifyContent; }

            set
            {
                if (!Equals(_vertifyContent, value))
                {
                    _vertifyContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VertifyContent)));
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

        private bool _isVertifying;

        public bool IsVertifying
        {
            get { return _isVertifying; }

            set
            {
                if (!Equals(_isVertifying, value))
                {
                    _isVertifying = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVertifying)));
                }
            }
        }

        private List<DataVertifyTypeModel> DataVertifyTypeList { get; } = [];

        private ObservableCollection<DataEncryptVertifyResultModel> DataVertifyResultCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DataVertifyPage()
        {
            InitializeComponent();
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = AICHString,
                DataVertifyType = DataVertifyType.AICH
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = BLAKE2spString,
                DataVertifyType = DataVertifyType.BLAKE2sp
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = BLAKE3String,
                DataVertifyType = DataVertifyType.BLAKE3
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = BTIHString,
                DataVertifyType = DataVertifyType.BTIH
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = CRC32String,
                DataVertifyType = DataVertifyType.CRC_32
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = CRC64String,
                DataVertifyType = DataVertifyType.CRC_64
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = ED2KString,
                DataVertifyType = DataVertifyType.ED2K
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = EDONR224String,
                DataVertifyType = DataVertifyType.EDON_R_224
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = EDONR256String,
                DataVertifyType = DataVertifyType.EDON_R_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = EDONR384String,
                DataVertifyType = DataVertifyType.EDON_R_384
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = EDONR512String,
                DataVertifyType = DataVertifyType.EDON_R_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST12256String,
                DataVertifyType = DataVertifyType.GOST12_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST12512String,
                DataVertifyType = DataVertifyType.GOST12_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST94String,
                DataVertifyType = DataVertifyType.GOST94
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = GOST94CryptoProString,
                DataVertifyType = DataVertifyType.GOST94CryptoPro
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = HAS160String,
                DataVertifyType = DataVertifyType.HAS_160
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = MD2String,
                DataVertifyType = DataVertifyType.MD2
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = MD4String,
                DataVertifyType = DataVertifyType.MD4
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = MD5String,
                DataVertifyType = DataVertifyType.MD5
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = RIPEMD160String,
                DataVertifyType = DataVertifyType.RIPEMD_160
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA1String,
                DataVertifyType = DataVertifyType.SHA_1
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA224String,
                DataVertifyType = DataVertifyType.SHA_224
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA256String,
                DataVertifyType = DataVertifyType.SHA_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA384String,
                DataVertifyType = DataVertifyType.SHA_384
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA512String,
                DataVertifyType = DataVertifyType.SHA_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3224String,
                DataVertifyType = DataVertifyType.SHA3_224
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3256String,
                DataVertifyType = DataVertifyType.SHA3_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3384String,
                DataVertifyType = DataVertifyType.SHA3_384
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SHA3512String,
                DataVertifyType = DataVertifyType.SHA3_512
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SM3String,
                DataVertifyType = DataVertifyType.SM3
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SNEFRU128String,
                DataVertifyType = DataVertifyType.SNEFRU_128
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = SNEFRU256String,
                DataVertifyType = DataVertifyType.SNEFRU_256
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = TIGERString,
                DataVertifyType = DataVertifyType.TIGER
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = TIGER2String,
                DataVertifyType = DataVertifyType.TIGER2
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = TTHSString,
                DataVertifyType = DataVertifyType.TTH
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = WHIRLPOOLString,
                DataVertifyType = DataVertifyType.WHIRLPOOL
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH32String,
                DataVertifyType = DataVertifyType.XXH32
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH64String,
                DataVertifyType = DataVertifyType.XXH64
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH3String,
                DataVertifyType = DataVertifyType.XXH3
            });
            DataVertifyTypeList.Add(new DataVertifyTypeModel()
            {
                Name = XXH128String,
                DataVertifyType = DataVertifyType.XXH128
            });
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
        /// TODO：未完成
        private void OnOpenLocalFileClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 校验内容发生改变时触发的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                VertifyContent = textBox.Text;
            }
        }

        /// <summary>
        /// 开始数据校验
        /// </summary>
        /// TODO：未完成
        private void OnStartVertifyClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 获取要校验的类型
        /// </summary>
        private Visibility GetDataVertifyType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取校验结果列表显示类型
        /// </summary>
        private Visibility GetVertifyResult(bool isVertifying, int vertifyCount)
        {
            return isVertifying ? Visibility.Collapsed : vertifyCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
