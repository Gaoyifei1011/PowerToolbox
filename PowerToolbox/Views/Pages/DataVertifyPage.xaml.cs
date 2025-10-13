using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Services.Root;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 数据校验页面
    /// </summary>
    public sealed partial class DataVertifyPage : Page, INotifyPropertyChanged
    {
        private readonly string ContentInitializeString = ResourceService.DataVertifyResource.GetString("ContentInitialize");
        private readonly string ContentVertifyFailedString = ResourceService.DataVertifyResource.GetString("ContentVertifyFailed");
        private readonly string ContentVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyPartSuccessfully");
        private readonly string ContentVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("ContentVertifyWholeSuccessfully");
        private readonly string FileInitializeString = ResourceService.DataVertifyResource.GetString("FileInitialize");
        private readonly string FileVertifyFailedString = ResourceService.DataVertifyResource.GetString("FileVertifyFailed");
        private readonly string FileVertifyPartSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyPartSuccessfully");
        private readonly string FileVertifyWholeSuccessfullyString = ResourceService.DataVertifyResource.GetString("FileVertifyWholeSuccessfully");

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

        public event PropertyChangedEventHandler PropertyChanged;

        public DataVertifyPage()
        {
            InitializeComponent();
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
        /// 获取要校验的类型
        /// </summary>
        private Visibility GetDataVertifyType(int selectedIndex, int comparedSelectedIndex)
        {
            return Equals(selectedIndex, comparedSelectedIndex) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
