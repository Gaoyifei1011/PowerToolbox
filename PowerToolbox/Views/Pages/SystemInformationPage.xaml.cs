using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 系统信息页面
    /// </summary>
    public sealed partial class SystemInformationPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SystemInformationPage()
        {
            InitializeComponent();
        }
    }
}
