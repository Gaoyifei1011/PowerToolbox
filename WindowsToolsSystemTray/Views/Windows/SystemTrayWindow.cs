using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using WindowsToolsSystemTray.Helpers.Root;
using WindowsToolsSystemTray.Services.Controls.Settings;
using WindowsToolsSystemTray.Services.Root;
using WindowsToolsSystemTray.Views.Pages;
using WindowsToolsSystemTray.WindowsAPI.ComTypes;
using WindowsToolsSystemTray.WindowsAPI.PInvoke.User32;

namespace WindowsToolsSystemTray.Views.Windows
{
    /// <summary>
    /// ���̳���������
    /// </summary>
    public class SystemTrayWindow : Form
    {
        private readonly Container container = new();
        private readonly DesktopWindowXamlSource desktopWindowXamlSource = new();

        public UIElement Content { get; set; }

        public static SystemTrayWindow Current { get; private set; }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle = (int)(WindowExStyle.WS_EX_TRANSPARENT | WindowExStyle.WS_EX_LAYERED | WindowExStyle.WS_EX_TOOLWINDOW);
                createParams.Style = unchecked((int)WindowStyle.WS_POPUPWINDOW);
                return createParams;
            }
        }

        public SystemTrayWindow()
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            Current = this;
            Content = new SystemTrayPage();

            desktopWindowXamlSource.Content = Content;
            IDesktopWindowXamlSourceNative2 desktopWindowXamlSourceInterop = desktopWindowXamlSource as IDesktopWindowXamlSourceNative2;
            desktopWindowXamlSourceInterop.AttachToWindow(Handle);
            desktopWindowXamlSource.TakeFocusRequested += OnTakeFocusRequested;

            Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            StartPosition = FormStartPosition.CenterScreen;
            Text = ResourceService.SystemTrayResource.GetString("WindowTitle");

            RightToLeft = LanguageService.RightToLeft;
            RightToLeftLayout = LanguageService.RightToLeft is RightToLeft.Yes;

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
            }

            SystemTrayService.RightClick += OnRightClick;
        }

        #region ��һ���֣�������������Ҫ���ص��¼�

        /// <summary>
        /// ������������ռ�õ���Դ
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && container is not null)
            {
                container.Dispose();
            }
        }

        /// <summary>
        /// ���ڲ����ڽ���״̬ʱ�������¼�
        /// </summary>
        protected override void OnDeactivate(EventArgs args)
        {
            base.OnDeactivate(args);

            if ((Content as SystemTrayPage).SystemTrayFlyout.IsOpen)
            {
                (Content as SystemTrayPage).SystemTrayFlyout.Hide();
            }
        }

        /// <summary>
        /// �رմ��ں��ͷ���Դ
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs args)
        {
            base.OnFormClosed(args);
            desktopWindowXamlSource.TakeFocusRequested -= OnTakeFocusRequested;
            SystemTrayService.RightClick -= OnRightClick;
            desktopWindowXamlSource.Dispose();

            Current = null;
            (global::Windows.UI.Xaml.Application.Current as SystemTrayApp).Dispose();
        }

        #endregion ��һ���֣�������������Ҫ���ص��¼�

        #region �ڶ����֣��Զ����¼�

        /// <summary>
        /// ������������Ҽ������¼�
        /// </summary>
        private void OnRightClick(object sender, MouseEventArgs args)
        {
            FlyoutShowOptions options = new()
            {
                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                ShowMode = FlyoutShowMode.Standard,
                Position = InfoHelper.SystemVersion.Build >= 22000
                        ? new global::Windows.Foundation.Point(MousePosition.X / ((double)DeviceDpi / 96), MousePosition.Y / ((double)DeviceDpi / 96))
                        : new global::Windows.Foundation.Point(MousePosition.X, MousePosition.Y)
            };

            Activate();
            (Content as SystemTrayPage).SystemTrayFlyout.ShowAt(Content, options);
        }

        /// <summary>
        /// ����������Ӧ�ó����յ��� DesktopWindowXamlSource ���� (��ȡ���������ʱ�������û�λ�� DesktopWindowXamlSource �е����һ���ɾ۽�Ԫ���ϣ�Ȼ�� Tab) ��
        /// </summary>
        private void OnTakeFocusRequested(DesktopWindowXamlSource sender, DesktopWindowXamlSourceTakeFocusRequestedEventArgs args)
        {
            XamlSourceFocusNavigationReason reason = args.Request.Reason;

            if (reason < XamlSourceFocusNavigationReason.Left)
            {
                sender.NavigateFocus(args.Request);
            }
        }

        #endregion �ڶ����֣��Զ����¼�

        /// <summary>
        /// Ӧ����������Ϣ����
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                // Ӧ���������ø���ϵͳ�����仯ʱ����ϵͳ�������÷����仯ʱ�޸��޸�Ӧ�ñ���ɫ
                case (int)WindowMessage.WM_SETTINGCHANGE:
                    {
                        BeginInvoke(async () =>
                        {
                            await (Content as SystemTrayPage).UpdateSystemTrayThemeAsync();
                        });
                        break;
                    }
            }
        }
    }
}
