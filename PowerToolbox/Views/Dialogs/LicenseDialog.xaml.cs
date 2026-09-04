using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Text;

namespace PowerToolbox.Views.Dialogs
{
    /// <summary>
    /// 许可证文字内容对话框
    /// </summary>
    internal sealed partial class LicenseDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：属性、列表与事件

        private string _licenseText = Encoding.UTF8.GetString(Strings.Resources.LICENSE);

        private string LicenseText
        {
            get { return _licenseText; }

            set
            {
                if (!Equals(_licenseText, value))
                {
                    _licenseText = value;
                    PropertyChanged?.Invoke(this, new(nameof(LicenseText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、列表与事件

        #region 第二部分：构造函数

        internal LicenseDialog()
        {
            InitializeComponent();
        }

        #endregion 第二部分：构造函数
    }
}
