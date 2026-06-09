using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项页面
    /// </summary>
    public sealed partial class AdvancedSystemOptionsPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AdvancedSystemOptionsPage()
        {
            InitializeComponent();
        }
    }
}
