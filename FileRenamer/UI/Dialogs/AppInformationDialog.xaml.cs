using FileRenamer.Views.CustomControls.DialogsAndFlyouts;

namespace FileRenamer.UI.Dialogs.About
{
    /// <summary>
    /// Ӧ����Ϣ�Ի�����ͼ
    /// </summary>
    public sealed partial class AppInformationDialog : ExtendedContentDialog
    {
        public AppInformationDialog()
        {
            InitializeComponent();
            ViewModel.InitializeAppInformation();
        }
    }
}
