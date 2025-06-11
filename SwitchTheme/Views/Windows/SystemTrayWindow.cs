using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using SwitchTheme.Helpers.Root;
using SwitchTheme.Services.Controls.Settings;
using SwitchTheme.Services.Root;
using SwitchTheme.Views.Pages;
using SwitchTheme.WindowsAPI.ComTypes;
using SwitchTheme.WindowsAPI.PInvoke.User32;

// ���� CA1806��CA1822 ����
#pragma warning disable CA1806,CA1822

namespace SwitchTheme.Views.Windows
{
    /// <summary>
    /// ���̳���������
    /// </summary>
    public class SystemTrayWindow : Form
    {
        private readonly string AppTheme = ResourceService.SystemTrayResource.GetString("AppTheme");
        private readonly string Dark = ResourceService.SystemTrayResource.GetString("Dark");
        private readonly string Light = ResourceService.SystemTrayResource.GetString("Light");
        private readonly string SystemTheme = ResourceService.SystemTrayResource.GetString("SystemTheme");
        private readonly string Title = ResourceService.SystemTrayResource.GetString("Title");

        private readonly Container container = new();
        private readonly DesktopWindowXamlSource desktopWindowXamlSource = new();

        private readonly System.Timers.Timer timer = new()
        {
            Interval = 1000,
            Enabled = true
        };

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
            IDesktopWindowXamlSourceNative2 desktopWindowXamlSourceNative = desktopWindowXamlSource as IDesktopWindowXamlSourceNative2;
            desktopWindowXamlSourceNative.AttachToWindow(Handle);
            desktopWindowXamlSource.TakeFocusRequested += OnTakeFocusRequested;

            Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            StartPosition = FormStartPosition.CenterScreen;
            Text = Title;

            RightToLeft = LanguageService.RightToLeft;
            RightToLeftLayout = LanguageService.RightToLeft is RightToLeft.Yes;

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYDATA, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
            }

            SystemTrayService.RightClick += OnRightClick;
            timer.Elapsed += OnElapsed;

            Task.Run(() =>
            {
                ElementTheme systemTheme = GetSystemTheme();
                ElementTheme appTheme = GetAppTheme();

                string notifyIconTitle = string.Join(Environment.NewLine, Title, string.Format(SystemTheme, systemTheme is ElementTheme.Light ? Light : Dark), string.Format(AppTheme, appTheme is ElementTheme.Light ? Light : Dark));
                SystemTrayService.UpdateTitle(notifyIconTitle);
            });
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
            timer.Elapsed -= OnElapsed;
            SystemTrayService.RightClick -= OnRightClick;
            desktopWindowXamlSource.Dispose();
            timer.Dispose();

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYDATA, ChangeFilterFlags.MSGFLT_REMOVE);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
            }

            Current = null;
            (global::Windows.UI.Xaml.Application.Current as SystemTrayApp).Dispose();
        }

        #endregion ��һ���֣�������������Ҫ���ص��¼�

        #region �ڶ����֣��Զ����¼�

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
        /// ʱ�����Ŵ������¼�
        /// </summary>
        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            TimeSpan currentTime = new(DateTime.Now.Hour, DateTime.Now.Minute, 0);

            // �������Զ��л�����
            if (AutoSwitchThemeService.AutoSwitchThemeEnableValue)
            {
                // �Զ��л�ϵͳ����
                if (AutoSwitchThemeService.AutoSwitchSystemThemeValue)
                {
                    // ����ʱ��С��ҹ��ʱ��
                    if (AutoSwitchThemeService.SystemThemeLightTime < AutoSwitchThemeService.SystemThemeDarkTime)
                    {
                        // ���ڰ���ʱ���ҹ��ʱ�䣬�л�ǳɫ����
                        if (currentTime > AutoSwitchThemeService.SystemThemeLightTime && currentTime < AutoSwitchThemeService.SystemThemeDarkTime)
                        {
                            bool isModified = false;

                            if (GetSystemTheme() is ElementTheme.Dark)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 1);
                                isModified = true;
                            }

                            if (RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence") is 1)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", 0);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                        // �л���ɫ����
                        else
                        {
                            bool isModified = false;

                            if (GetSystemTheme() is ElementTheme.Light)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
                                isModified = true;
                            }

                            if (AutoSwitchThemeService.IsShowColorInDarkThemeValue && RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence") is 0)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", 1);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                    }
                    // ����ʱ�����ҹ��ʱ��
                    else
                    {
                        // ���ڰ���ʱ���ҹ��ʱ�䣬�л���ɫ����
                        if (currentTime > AutoSwitchThemeService.SystemThemeDarkTime && currentTime < AutoSwitchThemeService.SystemThemeLightTime)
                        {
                            bool isModified = false;

                            if (GetAppTheme() is ElementTheme.Light)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
                                isModified = true;
                            }

                            if (AutoSwitchThemeService.IsShowColorInDarkThemeValue && RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence") is 0)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", 1);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                        // �л�ǳɫ����
                        else
                        {
                            bool isModified = false;

                            if (GetSystemTheme() is ElementTheme.Dark)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 1);
                                isModified = true;
                            }

                            if (RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence") is 1)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", 0);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                    }
                }

                // �Զ��л�Ӧ������
                if (AutoSwitchThemeService.AutoSwitchAppThemeValue)
                {
                    // ����ʱ��С��ҹ��ʱ��
                    if (AutoSwitchThemeService.AppThemeLightTime < AutoSwitchThemeService.AppThemeDarkTime)
                    {
                        // ���ڰ���ʱ���ҹ��ʱ�䣬�л�ǳɫ����
                        if (currentTime > AutoSwitchThemeService.AppThemeLightTime && currentTime < AutoSwitchThemeService.AppThemeDarkTime)
                        {
                            bool isModified = false;

                            if (GetAppTheme() is ElementTheme.Dark)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                        // �л���ɫ����
                        else
                        {
                            bool isModified = false;

                            if (GetAppTheme() is ElementTheme.Light)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                    }
                    // ����ʱ�����ҹ��ʱ��
                    else
                    {
                        // ���ڰ���ʱ���ҹ��ʱ�䣬�л���ɫ����
                        if (currentTime > AutoSwitchThemeService.AppThemeDarkTime && currentTime < AutoSwitchThemeService.AppThemeLightTime)
                        {
                            bool isModified = false;

                            if (GetAppTheme() is ElementTheme.Light)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                        // �л�ǳɫ����
                        else
                        {
                            bool isModified = false;

                            if (GetAppTheme() is ElementTheme.Dark)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                    }
                }
            }
            else
            {
                Close();
            }

            ElementTheme systemTheme = GetSystemTheme();
            ElementTheme appTheme = GetAppTheme();

            string notifyIconTitle = string.Join(Environment.NewLine, Title, string.Format(SystemTheme, systemTheme is ElementTheme.Light ? Light : Dark), string.Format(AppTheme, appTheme is ElementTheme.Light ? Light : Dark));
            SystemTrayService.UpdateTitle(notifyIconTitle);
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
                // ������Ӧ�ó�����յ�����Ϣ
                case (int)WindowMessage.WM_COPYDATA:
                    {
                        COPYDATASTRUCT copyDataStruct = Marshal.PtrToStructure<COPYDATASTRUCT>(m.LParam);

                        if (string.Equals(copyDataStruct.lpData, "Auto switch theme settings changed"))
                        {
                            Task.Run(AutoSwitchThemeService.InitializeAutoSwitchTheme);
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// ��ȡϵͳ������ʽ
        /// </summary>
        private ElementTheme GetSystemTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }

        /// <summary>
        /// ��ȡӦ��������ʽ
        /// </summary>
        private ElementTheme GetAppTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}
