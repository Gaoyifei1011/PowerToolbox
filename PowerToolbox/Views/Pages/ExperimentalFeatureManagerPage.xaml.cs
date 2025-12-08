using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 实验功能管理界面
    /// </summary>
    public sealed partial class ExperimentalFeatureManagerPage : Page, INotifyPropertyChanged
    {
        private string _experimentalFeatureDescription;

        public string ExperimentalFeatureDescription
        {
            get { return _experimentalFeatureDescription; }

            set
            {
                if (!string.Equals(_experimentalFeatureDescription, value))
                {
                    _experimentalFeatureDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExperimentalFeatureDescription)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ExperimentalFeatureManagerPage()
        {
            InitializeComponent();
        }
    }
}
