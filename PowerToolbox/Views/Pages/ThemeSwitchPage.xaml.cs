using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 主题切换页面
    /// </summary>
    public sealed partial class ThemeSwitchPage : Page, INotifyPropertyChanged
    {
        private readonly string DarkString = ResourceService.ThemeSwitchResource.GetString("Dark");
        private readonly string LightString = ResourceService.ThemeSwitchResource.GetString("Light");
        private readonly Guid CLSID_DesktopWallpaper = new("C2CF3110-460E-4fC1-B9D0-8A1C0C9CC4BD");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private bool isInitialized;
        private IDesktopWallpaper desktopWallpaper;

        private Brush _systemAppBackground;

        public Brush SystemAppBackground
        {
            get { return _systemAppBackground; }

            set
            {
                if (!Equals(_systemAppBackground, value))
                {
                    _systemAppBackground = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemAppBackground)));
                }
            }
        }

        private ElementTheme _systemAppTheme;

        public ElementTheme SystemAppTheme
        {
            get { return _systemAppTheme; }

            set
            {
                if (!Equals(_systemAppTheme, value))
                {
                    _systemAppTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemAppTheme)));
                }
            }
        }

        private ImageSource _systemAppImage;

        public ImageSource SystemAppImage
        {
            get { return _systemAppImage; }

            set
            {
                if (!Equals(_systemAppImage, value))
                {
                    _systemAppImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemAppImage)));
                }
            }
        }

        private bool _isManualCloseInfoBar;

        public bool IsManualCloseInfoBar
        {
            get { return _isManualCloseInfoBar; }

            set
            {
                if (!Equals(_isManualCloseInfoBar, value))
                {
                    _isManualCloseInfoBar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsManualCloseInfoBar)));
                }
            }
        }

        private bool _isThemeSwitchNotificationEnabled;

        public bool IsThemeSwitchNotificationEnabled
        {
            get { return _isThemeSwitchNotificationEnabled; }

            set
            {
                if (!Equals(_isThemeSwitchNotificationEnabled, value))
                {
                    _isThemeSwitchNotificationEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsThemeSwitchNotificationEnabled)));
                }
            }
        }

        private KeyValuePair<ElementTheme, string> _selectedSystemThemeStyle;

        public KeyValuePair<ElementTheme, string> SelectedSystemThemeStyle
        {
            get { return _selectedSystemThemeStyle; }

            set
            {
                if (!Equals(_selectedSystemThemeStyle, value))
                {
                    _selectedSystemThemeStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSystemThemeStyle)));
                }
            }
        }

        private KeyValuePair<ElementTheme, string> _selectedAppThemeStyle;

        public KeyValuePair<ElementTheme, string> SelectedAppThemeStyle
        {
            get { return _selectedAppThemeStyle; }

            set
            {
                if (!Equals(_selectedAppThemeStyle, value))
                {
                    _selectedAppThemeStyle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAppThemeStyle)));
                }
            }
        }

        private bool _isShowThemeColorInStartAndTaskbar;

        public bool IsShowThemeColorInStartAndTaskbar
        {
            get { return _isShowThemeColorInStartAndTaskbar; }

            set
            {
                if (!Equals(_isShowThemeColorInStartAndTaskbar, value))
                {
                    _isShowThemeColorInStartAndTaskbar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowThemeColorInStartAndTaskbar)));
                }
            }
        }

        private bool _isShowThemeColorInStartAndTaskbarEnabled;

        public bool IsShowThemeColorInStartAndTaskbarEnabled
        {
            get { return _isShowThemeColorInStartAndTaskbarEnabled; }

            set
            {
                if (!Equals(_isShowThemeColorInStartAndTaskbarEnabled, value))
                {
                    _isShowThemeColorInStartAndTaskbarEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowThemeColorInStartAndTaskbarEnabled)));
                }
            }
        }

        private bool _isAutoThemeSwitchEnableValue = AutoThemeSwitchService.AutoThemeSwitchEnableValue;

        public bool IsAutoThemeSwitchEnableValue
        {
            get { return _isAutoThemeSwitchEnableValue; }

            set
            {
                if (!Equals(_isAutoThemeSwitchEnableValue, value))
                {
                    _isAutoThemeSwitchEnableValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoThemeSwitchEnableValue)));
                }
            }
        }

        private bool _isAutoSwitchSystemThemeValue = AutoThemeSwitchService.AutoSwitchSystemThemeValue;

        public bool IsAutoSwitchSystemThemeValue
        {
            get { return _isAutoSwitchSystemThemeValue; }

            set
            {
                if (!Equals(_isAutoSwitchSystemThemeValue, value))
                {
                    _isAutoSwitchSystemThemeValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoSwitchSystemThemeValue)));
                }
            }
        }

        private bool _isAutoSwitchAppThemeValue = AutoThemeSwitchService.AutoSwitchAppThemeValue;

        public bool IsAutoSwitchAppThemeValue
        {
            get { return _isAutoSwitchAppThemeValue; }

            set
            {
                if (!Equals(_isAutoSwitchAppThemeValue, value))
                {
                    _isAutoSwitchAppThemeValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoSwitchAppThemeValue)));
                }
            }
        }

        private bool _isShowColorInDarkThemeValue = AutoThemeSwitchService.IsShowColorInDarkThemeValue;

        public bool IsShowColorInDarkThemeValue
        {
            get { return _isShowColorInDarkThemeValue; }

            set
            {
                if (!Equals(_isShowColorInDarkThemeValue, value))
                {
                    _isShowColorInDarkThemeValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShowColorInDarkThemeValue)));
                }
            }
        }

        private TimeSpan _systemThemeLightTime = AutoThemeSwitchService.SystemThemeLightTime;

        public TimeSpan SystemThemeLightTime
        {
            get { return _systemThemeLightTime; }

            set
            {
                if (!Equals(_systemThemeLightTime, value))
                {
                    _systemThemeLightTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemThemeLightTime)));
                }
            }
        }

        private TimeSpan _systemThemeDarkTime = AutoThemeSwitchService.SystemThemeDarkTime;

        public TimeSpan SystemThemeDarkTime
        {
            get { return _systemThemeDarkTime; }

            set
            {
                if (!Equals(_systemThemeDarkTime, value))
                {
                    _systemThemeDarkTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SystemThemeDarkTime)));
                }
            }
        }

        private TimeSpan _appThemeLightTime = AutoThemeSwitchService.AppThemeLightTime;

        public TimeSpan AppThemeLightTime
        {
            get { return _appThemeLightTime; }

            set
            {
                if (!Equals(_appThemeLightTime, value))
                {
                    _appThemeLightTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppThemeLightTime)));
                }
            }
        }

        private TimeSpan _appThemeDarkTime = AutoThemeSwitchService.AppThemeDarkTime;

        public TimeSpan AppThemeDarkTime
        {
            get { return _appThemeDarkTime; }

            set
            {
                if (!Equals(_appThemeDarkTime, value))
                {
                    _appThemeDarkTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppThemeDarkTime)));
                }
            }
        }

        private List<KeyValuePair<ElementTheme, string>> SystemThemeStyleList { get; } = [];

        private List<KeyValuePair<ElementTheme, string>> AppThemeStyleList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ThemeSwitchPage()
        {
            InitializeComponent();

            SystemThemeStyleList.Add(new KeyValuePair<ElementTheme, string>(ElementTheme.Light, LightString));
            SystemThemeStyleList.Add(new KeyValuePair<ElementTheme, string>(ElementTheme.Dark, DarkString));
            AppThemeStyleList.Add(new KeyValuePair<ElementTheme, string>(ElementTheme.Light, LightString));
            AppThemeStyleList.Add(new KeyValuePair<ElementTheme, string>(ElementTheme.Dark, DarkString));
            SelectedSystemThemeStyle = SystemThemeStyleList[0];
            SelectedAppThemeStyle = AppThemeStyleList[0];
            RegistryHelper.NotifyKeyValueChanged += OnNotifyKeyValueChanged;
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (!isInitialized)
            {
                isInitialized = true;
                await Task.Run(() =>
                {
                    desktopWallpaper = (IDesktopWallpaper)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_DesktopWallpaper));
                    RegistryHelper.MonitorRegistryValueChange(Registry.CurrentUser, @"Control Panel\Desktop");
                    RegistryHelper.MonitorRegistryValueChange(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Wallpapers");
                    RegistryHelper.MonitorRegistryValueChange(Registry.CurrentUser, @"Control Panel\Colors");
                });
            }

            await InitializeSystemThemeSettingsAsync();
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：修改主题页面——挂载的事件

        /// <summary>
        /// 打开系统个性化
        /// </summary>
        private void OnOpenPersonalizeClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:colors");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnOpenPersonalizeClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 启用开机自启任务
        /// </summary>
        private async void OnEnableStartupTaskClicked(object sender, RoutedEventArgs args)
        {
            bool isStartupTaskEnabled = await Task.Run(async () =>
            {
                StartupTask startupTask = await StartupTask.GetAsync("ThemeSwitch");
                StartupTaskState startupTaskState = await startupTask.RequestEnableAsync();
                return startupTaskState is StartupTaskState.Enabled || startupTaskState is StartupTaskState.Disabled;
            });

            if (AutoThemeSwitchService.AutoThemeSwitchEnableValue)
            {
                IsThemeSwitchNotificationEnabled = !isStartupTaskEnabled;

                if (IsThemeSwitchNotificationEnabled)
                {
                    await MainWindow.Current.ShowDialogAsync(new OpenStartupTaskFailedDialog());
                }
            }
            else
            {
                IsThemeSwitchNotificationEnabled = false;
            }
        }

        /// <summary>
        /// 刷新主题样式设置值
        /// </summary>
        private async void OnRefreshClicked(SplitButton sender, SplitButtonClickEventArgs args)
        {
            await InitializeSystemThemeSettingsAsync();
        }

        /// <summary>
        /// 手动关闭信息栏提示
        /// </summary>
        private void OnCloseInfoBarClicked(InfoBar sender, object args)
        {
            IsManualCloseInfoBar = true;
        }

        /// <summary>
        /// 修改系统主题样式
        /// </summary>
        private void OnSystemThemeStyleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<ElementTheme, string> systemThemeStyle)
            {
                SelectedSystemThemeStyle = systemThemeStyle;
                int systemTheme = 0;

                if (Equals(SelectedSystemThemeStyle, SystemThemeStyleList[0]))
                {
                    systemTheme = 1;
                    IsShowThemeColorInStartAndTaskbarEnabled = false;
                    IsShowThemeColorInStartAndTaskbar = false;
                }
                else if (Equals(SelectedSystemThemeStyle, SystemThemeStyleList[1]))
                {
                    systemTheme = 0;
                    IsShowThemeColorInStartAndTaskbarEnabled = true;
                }

                Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", systemTheme);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", IsShowThemeColorInStartAndTaskbar);
                    User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                });
            }
        }

        /// <summary>
        /// 修改应用主题样式
        /// </summary>
        private void OnAppThemeStyleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<ElementTheme, string> appTheme)
            {
                SelectedAppThemeStyle = appTheme;
                int apptheme = 0;

                if (Equals(SelectedAppThemeStyle, AppThemeStyleList[0]))
                {
                    apptheme = 1;
                }
                else if (Equals(SelectedSystemThemeStyle, SystemThemeStyleList[1]))
                {
                    apptheme = 0;
                }

                Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", apptheme);
                    User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                });
            }
        }

        /// <summary>
        /// 在“开始菜单”和任务栏中显示主题色
        /// </summary>
        private void OnShowThemeColorInStartAndTaskbarToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsShowThemeColorInStartAndTaskbar = toggleSwitch.IsOn;

                Task.Run(() =>
                {
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", IsShowThemeColorInStartAndTaskbar);
                    User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                });
            }
        }

        /// <summary>
        /// 保存自动修改主题设置值
        /// </summary>
        private async void OnSaveClicked(SplitButton sender, SplitButtonClickEventArgs args)
        {
            if (IsAutoSwitchSystemThemeValue && Equals(SystemThemeLightTime, SystemThemeDarkTime))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ThemeChangeSameTime));
                return;
            }
            else if (IsAutoSwitchAppThemeValue && Equals(AppThemeLightTime, AppThemeDarkTime))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ThemeChangeSameTime));
                return;
            }

            await Task.Run(() =>
            {
                AutoThemeSwitchService.SetAutoThemeSwitchEnableValue(IsAutoThemeSwitchEnableValue);
                AutoThemeSwitchService.SetAutoSwitchSystemThemeValue(IsAutoSwitchSystemThemeValue);
                AutoThemeSwitchService.SetAutoSwitchAppThemeValue(IsAutoSwitchAppThemeValue);
                AutoThemeSwitchService.SetIsShowColorInDarkThemeValue(IsShowColorInDarkThemeValue);
                AutoThemeSwitchService.SetSystemThemeLightTime(SystemThemeLightTime);
                AutoThemeSwitchService.SetSystemThemeDarkTime(SystemThemeDarkTime);
                AutoThemeSwitchService.SetAppThemeLightTime(AppThemeLightTime);
                AutoThemeSwitchService.SetAppThemeDarkTime(AppThemeDarkTime);

                if (IsAutoThemeSwitchEnableValue)
                {
                    try
                    {
                        bool isExisted = false;
                        Process[] processArray = Process.GetProcessesByName("ThemeSwitch");

                        foreach (Process process in processArray)
                        {
                            if (process.Id is not 0 && !process.MainWindowHandle.Equals(IntPtr.Zero))
                            {
                                isExisted = true;
                                string message = "Auto switch theme settings changed";

                                COPYDATASTRUCT copyDataStruct = new()
                                {
                                    dwData = IntPtr.Zero,
                                    cbData = Encoding.Unicode.GetBytes(message).Length + 1,
                                    lpData = message,
                                };

                                IntPtr copyDataStructPtr = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());
                                Marshal.StructureToPtr(copyDataStruct, copyDataStructPtr, false);
                                User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_COPYDATA, UIntPtr.Zero, copyDataStructPtr);
                                Marshal.FreeHGlobal(copyDataStructPtr);
                                break;
                            }
                        }

                        if (!isExisted)
                        {
                            ProcessStartInfo startInfo = new()
                            {
                                UseShellExecute = true,
                                WorkingDirectory = Environment.CurrentDirectory,
                                FileName = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "ThemeSwitch.exe"),
                                Verb = "open"
                            };
                            Process.Start(startInfo);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnSaveClicked), 1, e);
                    }
                }
                else
                {
                    Process[] processArray = Process.GetProcessesByName("ThemeSwitch");

                    foreach (Process process in processArray)
                    {
                        if (process.Id is not 0 && !process.MainWindowHandle.Equals(IntPtr.Zero))
                        {
                            string message = "Auto switch theme settings changed";

                            COPYDATASTRUCT copyDataStruct = new()
                            {
                                dwData = IntPtr.Zero,
                                cbData = Encoding.Unicode.GetBytes(message).Length + 1,
                                lpData = message,
                            };

                            IntPtr copyDataStructPtr = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());
                            Marshal.StructureToPtr(copyDataStruct, copyDataStructPtr, false);
                            User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_COPYDATA, UIntPtr.Zero, copyDataStructPtr);
                            Marshal.FreeHGlobal(copyDataStructPtr);
                            break;
                        }
                    }
                }
            });

            IsThemeSwitchNotificationEnabled = AutoThemeSwitchService.AutoThemeSwitchEnableValue && !await Task.Run(GetStartupTaskEnabledAsync);
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ThemeSwitchSaveResult));
        }

        /// <summary>
        /// 恢复默认值
        /// </summary>
        private async void OnRestoreDefaultClicked(object sender, RoutedEventArgs args)
        {
            await Task.Run(() =>
            {
                AutoThemeSwitchService.SetAutoThemeSwitchEnableValue(AutoThemeSwitchService.DefaultAutoThemeSwitchEnableValue);
                AutoThemeSwitchService.SetAutoSwitchSystemThemeValue(AutoThemeSwitchService.DefaultAutoSwitchSystemThemeValue);
                AutoThemeSwitchService.SetAutoSwitchAppThemeValue(AutoThemeSwitchService.DefaultAutoSwitchAppThemeValue);
                AutoThemeSwitchService.SetIsShowColorInDarkThemeValue(AutoThemeSwitchService.DefaultIsShowColorInDarkThemeValue);
                AutoThemeSwitchService.SetSystemThemeLightTime(AutoThemeSwitchService.DefaultSystemThemeLightTime);
                AutoThemeSwitchService.SetSystemThemeDarkTime(AutoThemeSwitchService.DefaultSystemThemeDarkTime);
                AutoThemeSwitchService.SetAppThemeLightTime(AutoThemeSwitchService.DefaultAppThemeLightTime);
                AutoThemeSwitchService.SetAppThemeDarkTime(AutoThemeSwitchService.DefaultAppThemeDarkTime);

                Process[] processArray = Process.GetProcessesByName("ThemeSwitch");

                foreach (Process process in processArray)
                {
                    if (process.Id is not 0 && !process.MainWindowHandle.Equals(IntPtr.Zero))
                    {
                        string message = "Auto switch theme settings changed";

                        COPYDATASTRUCT copyDataStruct = new()
                        {
                            dwData = IntPtr.Zero,
                            cbData = Encoding.Unicode.GetBytes(message).Length + 1,
                            lpData = message,
                        };

                        IntPtr copyDataStructPtr = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());
                        Marshal.StructureToPtr(copyDataStruct, copyDataStructPtr, false);
                        User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_COPYDATA, UIntPtr.Zero, copyDataStructPtr);
                        Marshal.FreeHGlobal(copyDataStructPtr);
                        break;
                    }
                }
            });

            IsAutoThemeSwitchEnableValue = AutoThemeSwitchService.DefaultAutoThemeSwitchEnableValue;
            IsAutoSwitchSystemThemeValue = AutoThemeSwitchService.DefaultAutoSwitchSystemThemeValue;
            IsAutoSwitchAppThemeValue = AutoThemeSwitchService.DefaultAutoSwitchAppThemeValue;
            IsShowColorInDarkThemeValue = AutoThemeSwitchService.DefaultIsShowColorInDarkThemeValue;
            SystemThemeLightTime = AutoThemeSwitchService.DefaultSystemThemeLightTime;
            SystemThemeDarkTime = AutoThemeSwitchService.DefaultSystemThemeDarkTime;
            AppThemeLightTime = AutoThemeSwitchService.DefaultAppThemeLightTime;
            AppThemeDarkTime = AutoThemeSwitchService.DefaultAppThemeDarkTime;
            IsThemeSwitchNotificationEnabled = AutoThemeSwitchService.AutoThemeSwitchEnableValue && !await Task.Run(GetStartupTaskEnabledAsync);
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ThemeSwitchRestoreResult));
        }

        /// <summary>
        /// 启动自动切换主题程序
        /// </summary>
        private void OnOpenAutoSwitchProgramClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                bool isExisted = false;
                Process[] processArray = Process.GetProcessesByName("ThemeSwitch");

                foreach (Process process in processArray)
                {
                    if (process.Id is not 0 && !process.MainWindowHandle.Equals(IntPtr.Zero))
                    {
                        isExisted = true;
                        string message = "Auto switch theme settings changed";

                        COPYDATASTRUCT copyDataStruct = new()
                        {
                            dwData = IntPtr.Zero,
                            cbData = Encoding.Unicode.GetBytes(message).Length + 1,
                            lpData = message,
                        };

                        IntPtr copyDataStructPtr = Marshal.AllocHGlobal(Marshal.SizeOf<COPYDATASTRUCT>());
                        Marshal.StructureToPtr(copyDataStruct, copyDataStructPtr, false);
                        User32Library.SendMessage(process.MainWindowHandle, WindowMessage.WM_COPYDATA, UIntPtr.Zero, copyDataStructPtr);
                        Marshal.FreeHGlobal(copyDataStructPtr);
                        break;
                    }
                }

                if (!isExisted)
                {
                    ProcessStartInfo startInfo = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "ThemeSwitch.exe"),
                        Verb = "open",
                    };
                    Process.Start(startInfo);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnSaveClicked), 2, e);
            }
        }

        /// <summary>
        /// 是否启用自动切换主题
        /// </summary>
        private void OnAutoThemeSwitchEnableToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAutoThemeSwitchEnableValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 修改选定的自动修改系统主题设置选项
        /// </summary>
        private void OnAutoSwitchSystemThemeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAutoSwitchSystemThemeValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 显示设置时间控件
        /// </summary>
        private void OnShowSetTimeFlyoutClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 修改选定的自动修改系统主题设置选项
        /// </summary>
        private void OnAutoSwitchAppThemeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAutoSwitchAppThemeValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 切换系统深色主题时显示主题色
        /// </summary>
        private void OnShowColorInDarkThemeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsShowColorInDarkThemeValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            if (sender is Button button && button.Tag is string tag && !string.IsNullOrEmpty(tag))
            {
                if (string.Equals(tag, "SystemThemeSetTimeFlyout", StringComparison.OrdinalIgnoreCase) && SystemThemeSetTimeFlyout.IsOpen)
                {
                    SystemThemeSetTimeFlyout.Hide();
                }
                else if (string.Equals(tag, "AppThemeSetTimeFlyout", StringComparison.OrdinalIgnoreCase) && AppThemeSetTimeFlyout.IsOpen)
                {
                    AppThemeSetTimeFlyout.Hide();
                }
            }
        }

        /// <summary>
        /// 修改选定的自动修改系统浅色主题时设定时间
        /// </summary>
        private void OnSystemThemeLightTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                SystemThemeLightTime = args.NewTime;
            }
        }

        /// <summary>
        /// 修改选定的自动修改系统浅色主题时设定时间
        /// </summary>
        private void OnSystemThemeDarkTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                SystemThemeDarkTime = args.NewTime;
            }
        }

        /// <summary>
        /// 修改选定的自动修改应用浅色主题时设定时间
        /// </summary>
        private void OnAppThemeLightTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                AppThemeLightTime = args.NewTime;
            }
        }

        /// <summary>
        /// 修改选定的自动修改应用浅色主题时设定时间
        /// </summary>
        private void OnAppThemeDarkTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            if (sender is TimePicker)
            {
                AppThemeDarkTime = args.NewTime;
            }
        }

        #endregion 第二部分：修改主题页面——挂载的事件

        #region 第二部分：自定义事件

        /// <summary>
        /// 注册表内容发生变更时触发的事件
        /// </summary>
        private void OnNotifyKeyValueChanged(object sender, string key)
        {
            if (string.Equals(key, @"Control Panel\Desktop") || string.Equals(key, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Wallpapers") || string.Equals(key, @"Control Panel\Colors"))
            {
                synchronizationContext.Post(async (_) =>
                {
                    await InitializeSystemThemeSettingsAsync();
                }, null);

                // 注册的变化通知在使用一次后就消失了，需要重新注册
                RegistryHelper.MonitorRegistryValueChange(Registry.CurrentUser, key);
            }
        }

        #endregion 第二部分：自定义事件

        /// <summary>
        /// 初始化系统主题设置内容
        /// </summary>
        public async Task InitializeSystemThemeSettingsAsync()
        {
            Dictionary<string, object> wallpaperDict = await Task.Run(() =>
            {
                Dictionary<string, object> wallpaperDict = [];
                if (desktopWallpaper.GetWallpaper(System.Windows.Forms.Screen.PrimaryScreen.DeviceName, out string wallpaper) is 0 && File.Exists(wallpaper))
                {
                    wallpaperDict.Add("Wallpaper", wallpaper);
                    wallpaperDict.Add("Color", Colors.Black);
                }
                else
                {
                    wallpaperDict.Add("Wallpaper", string.Empty);
                }

                if (!wallpaperDict.ContainsKey("Color"))
                {
                    if (desktopWallpaper.GetBackgroundColor(out uint color) is 0)
                    {
                        System.Drawing.Color classicColor = System.Drawing.ColorTranslator.FromWin32((int)color);
                        wallpaperDict.Add("Color", Color.FromArgb(classicColor.A, classicColor.R, classicColor.G, classicColor.B));
                    }
                    else
                    {
                        wallpaperDict.Add("Color", Colors.Black);
                    }
                }

                return wallpaperDict;
            });

            string wallpaper = Convert.ToString(wallpaperDict["Wallpaper"]);

            if (!string.IsNullOrEmpty(wallpaper))
            {
                try
                {
                    SystemAppImage = new BitmapImage
                    {
                        UriSource = new Uri(wallpaper)
                    };
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(InitializeSystemThemeSettingsAsync), 1, e);
                }
            }
            else
            {
                SystemAppImage = null;
            }

            SystemAppBackground = new SolidColorBrush((Color)wallpaperDict["Color"]);

            ElementTheme systemTheme = await Task.Run(GetSystemTheme);
            SelectedSystemThemeStyle = SystemThemeStyleList.Find(item => Equals(item.Key, systemTheme));

            ElementTheme appTheme = await Task.Run(GetAppTheme);
            SystemAppTheme = appTheme;
            SelectedAppThemeStyle = AppThemeStyleList.Find(item => Equals(item.Key, appTheme));

            IsShowThemeColorInStartAndTaskbarEnabled = Equals(SelectedSystemThemeStyle, SystemThemeStyleList[1]);
            bool showThemeColorInStartAndTaskbar = await Task.Run(GetShowThemeColorInStartAndTaskbar);
            IsShowThemeColorInStartAndTaskbar = showThemeColorInStartAndTaskbar;
            IsThemeSwitchNotificationEnabled = AutoThemeSwitchService.AutoThemeSwitchEnableValue && !await Task.Run(GetStartupTaskEnabledAsync);
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
        /// 获取在“开始菜单”和任务栏中显示主题色设置值
        /// </summary>
        private bool GetShowThemeColorInStartAndTaskbar()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence");
        }

        /// <summary>
        /// 获取信息栏显示状态
        /// </summary>
        private Visibility GetInfoBarState(bool isManualCloseInfoBar, bool isThemeSwitchNotificationEnabled)
        {
            return !isManualCloseInfoBar && isThemeSwitchNotificationEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取启动任务状态
        /// </summary>
        private async Task<bool> GetStartupTaskEnabledAsync()
        {
            StartupTask startupTask = await StartupTask.GetAsync("ThemeSwitch");
            return startupTask is not null ? startupTask.State is StartupTaskState.Enabled || startupTask.State is StartupTaskState.EnabledByPolicy : await Task.FromResult(true);
        }
    }
}
