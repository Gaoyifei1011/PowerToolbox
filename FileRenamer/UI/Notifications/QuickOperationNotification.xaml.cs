using FileRenamer.Extensions.DataType.Enums;
using FileRenamer.Views.CustomControls.Notifications;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FileRenamer.UI.Notifications
{
    /// <summary>
    /// ��ݲ���Ӧ����֪ͨ
    /// </summary>
    public sealed partial class QuickOperationNotification : InAppNotification, INotifyPropertyChanged
    {
        private QuickOperationType _operationType;

        public QuickOperationType OperationType
        {
            get { return _operationType; }

            set
            {
                _operationType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationType)));
            }
        }

        private bool _isPinnedSuccessfully = false;

        public bool IsPinnedSuccessfully
        {
            get { return _isPinnedSuccessfully; }

            set
            {
                _isPinnedSuccessfully = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPinnedSuccessfully)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public QuickOperationNotification(QuickOperationType operationType, [Optional, DefaultParameterValue(false)] bool isPinnedSuccessfully)
        {
            InitializeComponent();
            OperationType = operationType;
            IsPinnedSuccessfully = isPinnedSuccessfully;
        }

        public bool ControlLoad(QuickOperationType operationType, bool isPinnedSuccessfully, string controlName)
        {
            if (controlName is "DesktopShortcutSuccess" && operationType is QuickOperationType.DesktopShortcut && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "DesktopShortcutFailed" && operationType is QuickOperationType.DesktopShortcut && !isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenSuccess" && operationType is QuickOperationType.StartScreen && isPinnedSuccessfully)
            {
                return true;
            }
            else if (controlName is "StartScreenFailed" && operationType is QuickOperationType.StartScreen && !isPinnedSuccessfully)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
