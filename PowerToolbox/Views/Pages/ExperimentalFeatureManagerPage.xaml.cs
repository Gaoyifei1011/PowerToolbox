using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 实验功能管理界面
    /// </summary>
    public sealed partial class ExperimentalFeatureManagerPage : Page, INotifyPropertyChanged
    {
        private string _experimentaFeatureDescription;

        public string ExperimentalFeatureDescription
        {
            get { return _experimentaFeatureDescription; }

            set
            {
                if (!string.Equals(_experimentaFeatureDescription, value))
                {
                    _experimentaFeatureDescription = value;
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
