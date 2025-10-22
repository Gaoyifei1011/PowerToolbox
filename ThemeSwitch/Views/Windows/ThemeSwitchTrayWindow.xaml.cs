using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ThemeSwitch.Helpers.Root;
using ThemeSwitch.Services.Controls.Settings;
using ThemeSwitch.Services.Root;
using ThemeSwitch.WindowsAPI.PInvoke.Comctl32;
using ThemeSwitch.WindowsAPI.PInvoke.User32;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace ThemeSwitch.Views.Windows
{
    /// <summary>
    /// 主题切换托盘窗口
    /// </summary>
    public sealed partial class ThemeSwitchTrayWindow : Window, INotifyPropertyChanged
    {
        private readonly string AppThemeString = ResourceService.ThemeSwitchTrayResource.GetString("AppTheme");
        private readonly string DarkString = ResourceService.ThemeSwitchTrayResource.GetString("Dark");
        private readonly string LightString = ResourceService.ThemeSwitchTrayResource.GetString("Light");
        private readonly string SystemThemeString = ResourceService.ThemeSwitchTrayResource.GetString("SystemTheme");
        private readonly string ThemeSwitchString = ResourceService.ThemeSwitchTrayResource.GetString("ThemeSwitch");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly SUBCLASSPROC mainWindowSubClassProc;

        private readonly System.Timers.Timer timer = new()
        {
            Interval = 1000,
            Enabled = true
        };

        public new static ThemeSwitchTrayWindow Current { get; private set; }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ThemeSwitchTrayWindow()
        {
            Current = this;
            InitializeComponent();

            SetWindowLongAuto((IntPtr)AppWindow.Id.Value, WindowLongIndexFlags.GWL_STYLE, new IntPtr(unchecked((int)WindowStyle.WS_POPUPWINDOW)));
            int exStyle = GetWindowLongAuto((IntPtr)AppWindow.Id.Value, WindowLongIndexFlags.GWL_EXSTYLE);
            SetWindowLongAuto((IntPtr)AppWindow.Id.Value, WindowLongIndexFlags.GWL_EXSTYLE, new IntPtr(exStyle | (int)(WindowExStyle.WS_EX_TRANSPARENT | WindowExStyle.WS_EX_LAYERED | WindowExStyle.WS_EX_TOOLWINDOW)));
            User32Library.SetWindowPos((IntPtr)AppWindow.Id.Value, IntPtr.Zero, 0, 0, 0, 0, PowerToolbox.WindowsAPI.PInvoke.User32.SetWindowPosFlags.SWP_FRAMECHANGED | PowerToolbox.WindowsAPI.PInvoke.User32.SetWindowPosFlags.SWP_NOOWNERZORDER);

            // 为应用主窗口添加窗口过程
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass((IntPtr)AppWindow.Id.Value, mainWindowSubClassProc, 0, IntPtr.Zero);

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYDATA, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
            }

            SystemTrayService.RightClick += OnRightClick;
            SystemTrayService.MouseDoubleClick += OnDoubleClick;
            timer.Elapsed += OnElapsed;

            Task.Run(() =>
            {
                ElementTheme systemTheme = GetSystemTheme();
                ElementTheme appTheme = GetAppTheme();

                string notifyIconTitle = string.Join(Environment.NewLine, ThemeSwitchString, string.Format(SystemThemeString, systemTheme is ElementTheme.Light ? LightString : DarkString), string.Format(AppThemeString, appTheme is ElementTheme.Light ? LightString : DarkString));
                SystemTrayService.UpdateTitle(notifyIconTitle);
            });
        }

        #region 第一部分：主题切换托盘窗口——挂载的事件

        /// <summary>
        /// 主题切换托盘窗口初始化触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            WindowTheme = await Task.Run(GetSystemTheme);
        }

        /// <summary>
        /// 打开主程序
        /// </summary>
        private void OnOpenMainProgramClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("powertoolbox:");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(ThemeSwitchTrayWindow), nameof(OnOpenMainProgramClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 切换系统主题
        /// </summary>
        private void OnSwitchSystemThemeClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ElementTheme systemTheme = GetSystemTheme();
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", systemTheme is ElementTheme.Light ? 0 : 1);
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", AutoThemeSwitchService.IsShowColorInDarkThemeValue && systemTheme is ElementTheme.Light ? 1 : 0);
                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
            });
        }

        /// <summary>
        /// 切换应用主题
        /// </summary>
        private void OnSwitchAppThemeClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ElementTheme appTheme = GetAppTheme();
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", appTheme is ElementTheme.Light ? 0 : 1);
                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
            });
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void OnExitClicked(object sender, RoutedEventArgs args)
        {
            (Application.Current as ThemeSwitchApp).Dispose();
        }

        #endregion 第一部分：主题切换托盘窗口——挂载的事件

        #region 第二部分：主题切换托盘窗口——自定义事件

        /// <summary>
        /// 处理托盘鼠标右键单击事件
        /// </summary>
        private void OnRightClick(object sender, System.Windows.Forms.MouseEventArgs args)
        {
            User32Library.GetCursorPos(out System.Drawing.Point cursorPoint);
            double dpi = Convert.ToDouble(User32Library.GetDpiForWindow((IntPtr)AppWindow.Id.Value)) / 96;

            FlyoutShowOptions options = new()
            {
                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
                ShowMode = FlyoutShowMode.Standard,
                Position = InfoHelper.SystemVersion.Build >= 22000
                    ? new global::Windows.Foundation.Point(cursorPoint.X / dpi, cursorPoint.Y / dpi)
                    : new global::Windows.Foundation.Point(cursorPoint.X, cursorPoint.Y)
            };

            Activate();
            ThemeSwitchFlyout.ShowAt(null, options);
        }

        /// <summary>
        /// 处理托盘鼠标右键双击事件
        /// </summary>
        private void OnDoubleClick(object sender, System.Windows.Forms.MouseEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("powertoolbox:");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(ThemeSwitchTrayWindow), nameof(OnDoubleClick), 1, e);
                }
            });
        }

        /// <summary>
        /// 时间流逝触发的事件
        /// </summary>
        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            TimeSpan currentTime = new(DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute, 0);

            // 已启用自动切换主题
            if (AutoThemeSwitchService.AutoThemeSwitchEnableValue)
            {
                // 自动切换系统主题
                if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                {
                    // 白天时间小于夜间时间
                    if (AutoThemeSwitchService.SystemThemeLightTime < AutoThemeSwitchService.SystemThemeDarkTime)
                    {
                        // 介于白天时间和夜间时间，切换浅色主题
                        if (currentTime > AutoThemeSwitchService.SystemThemeLightTime && currentTime < AutoThemeSwitchService.SystemThemeDarkTime)
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
                        // 切换深色主题
                        else
                        {
                            bool isModified = false;

                            if (GetSystemTheme() is ElementTheme.Light)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
                                isModified = true;
                            }

                            if (AutoThemeSwitchService.IsShowColorInDarkThemeValue && RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence") is 0)
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
                    // 白天时间大于夜间时间
                    else
                    {
                        // 介于白天时间和夜间时间，切换深色主题
                        if (currentTime > AutoThemeSwitchService.SystemThemeDarkTime && currentTime < AutoThemeSwitchService.SystemThemeLightTime)
                        {
                            bool isModified = false;

                            if (GetAppTheme() is ElementTheme.Light)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
                                isModified = true;
                            }

                            if (AutoThemeSwitchService.IsShowColorInDarkThemeValue && RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence") is 0)
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", 1);
                                isModified = true;
                            }

                            if (isModified)
                            {
                                User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                            }
                        }
                        // 切换浅色主题
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

                // 自动切换应用主题
                if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                {
                    // 白天时间小于夜间时间
                    if (AutoThemeSwitchService.AppThemeLightTime < AutoThemeSwitchService.AppThemeDarkTime)
                    {
                        // 介于白天时间和夜间时间，切换浅色主题
                        if (currentTime > AutoThemeSwitchService.AppThemeLightTime && currentTime < AutoThemeSwitchService.AppThemeDarkTime)
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
                        // 切换深色主题
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
                    // 白天时间大于夜间时间
                    else
                    {
                        // 介于白天时间和夜间时间，切换深色主题
                        if (currentTime > AutoThemeSwitchService.AppThemeDarkTime && currentTime < AutoThemeSwitchService.AppThemeLightTime)
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
                        // 切换浅色主题
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
                (Application.Current as ThemeSwitchApp).Dispose();
            }

            ElementTheme systemTheme = GetSystemTheme();
            ElementTheme appTheme = GetAppTheme();

            string notifyIconTitle = string.Join(Environment.NewLine, ThemeSwitchString, string.Format(SystemThemeString, systemTheme is ElementTheme.Light ? LightString : DarkString), string.Format(AppThemeString, appTheme is ElementTheme.Light ? LightString : DarkString));
            SystemTrayService.UpdateTitle(notifyIconTitle);
        }

        #endregion 第二部分：主题切换托盘窗口——自定义事件

        #region 第三部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private nint MainWindowSubClassProc(nint hWnd, WindowMessage Msg, nuint wParam, nint lParam, uint uIdSubclass, nint dwRefData)
        {
            switch (Msg)
            {
                // 窗口激活状态发生变化时触发的消息
                case WindowMessage.WM_ACTIVATE:
                    {
                        synchronizationContext.Post((_) =>
                        {
                            if (wParam == UIntPtr.Zero)
                            {
                                ThemeSwitchFlyout.Hide();
                            }
                        }, null);
                        break;
                    }
                // 窗口关闭时触发的消息
                case WindowMessage.WM_CLOSE:
                    {
                        synchronizationContext.Post(async (_) =>
                        {
                            try
                            {
                                timer.Elapsed -= OnElapsed;
                                SystemTrayService.RightClick -= OnRightClick;
                                timer.Dispose();

                                if (RuntimeHelper.IsElevated)
                                {
                                    User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYDATA, ChangeFilterFlags.MSGFLT_REMOVE);
                                    User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
                                }

                                Current = null;
                                (Application.Current as ThemeSwitchApp).Dispose();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(ThemeSwitchTrayWindow), nameof(MainWindowSubClassProc), 1, e);
                            }
                        }, null);
                        break;
                    }
                // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        synchronizationContext.Post(async (_) =>
                        {
                            await UpdateSystemTrayThemeAsync();
                        }, null);
                        break;
                    }
                // 从其他应用程序接收到的消息
                case WindowMessage.WM_COPYDATA:
                    {
                        COPYDATASTRUCT copyDataStruct = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);

                        if (string.Equals(copyDataStruct.lpData, "Auto switch theme settings changed"))
                        {
                            Task.Run(AutoThemeSwitchService.InitializeAutoThemeSwitch);
                        }

                        break;
                    }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第三部分：窗口过程

        /// <summary>
        /// 更新系统托盘主题
        /// </summary>
        public async Task UpdateSystemTrayThemeAsync()
        {
            WindowTheme = await Task.Run(GetSystemTheme);
        }

        /// <summary>
        /// 获取系统主题样式
        /// </summary>
        private ElementTheme GetSystemTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }

        /// <summary>
        /// 获取应用主题样式
        /// </summary>
        private ElementTheme GetAppTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }

        /// <summary>
        /// 获取窗口属性
        /// </summary>
        private static int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
        {
            return IntPtr.Size is 8 ? User32Library.GetWindowLongPtr(hWnd, nIndex) : User32Library.GetWindowLong(hWnd, nIndex);
        }

        /// <summary>
        /// 更改窗口属性
        /// </summary>
        private static IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size is 8 ? User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong) : User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
        }
    }
}
