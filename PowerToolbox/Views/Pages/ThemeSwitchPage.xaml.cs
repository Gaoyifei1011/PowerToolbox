using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Services.Position;
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
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
        private readonly string AutoThemeSwitchTypeFixedTimeString = ResourceService.ThemeSwitchResource.GetString("AutoThemeSwitchTypeFixedTime");
        private readonly string AutoThemeSwitchTypeSunriseSunsetString = ResourceService.ThemeSwitchResource.GetString("AutoThemeSwitchTypeSunriseSunset");
        private readonly string DarkString = ResourceService.ThemeSwitchResource.GetString("Dark");
        private readonly string LightString = ResourceService.ThemeSwitchResource.GetString("Light");
        private readonly string NotAvailableString = ResourceService.ThemeSwitchResource.GetString("NotAvailable");
        private readonly string PolarDayString = ResourceService.ThemeSwitchResource.GetString("PolarDay");
        private readonly string PolarNightString = ResourceService.ThemeSwitchResource.GetString("PolarNight");
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

        private KeyValuePair<string, string> _selectedAutoThemeSwitchType;

        public KeyValuePair<string, string> SelectedAutoThemeSwitchType
        {
            get { return _selectedAutoThemeSwitchType; }

            set
            {
                if (!Equals(_selectedAutoThemeSwitchType, value))
                {
                    _selectedAutoThemeSwitchType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAutoThemeSwitchType)));
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

        private bool _isGettingPosition;

        public bool IsGettingPosition
        {
            get { return _isGettingPosition; }

            set
            {
                if (!Equals(_isGettingPosition, value))
                {
                    _isGettingPosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGettingPosition)));
                }
            }
        }

        private int _sunriseOffset = AutoThemeSwitchService.SunriseOffset;

        public int SunriseOffset
        {
            get { return _sunriseOffset; }

            set
            {
                if (!Equals(_sunriseOffset, value))
                {
                    _sunriseOffset = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SunriseOffset)));
                }
            }
        }

        private int _sunsetOffset = AutoThemeSwitchService.SunsetOffset;

        public int SunsetOffset
        {
            get { return _sunsetOffset; }

            set
            {
                if (!Equals(_sunsetOffset, value))
                {
                    _sunsetOffset = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SunsetOffset)));
                }
            }
        }

        private string _longitude;

        public string Longitude
        {
            get { return _longitude; }

            set
            {
                if (!string.Equals(_longitude, value))
                {
                    _longitude = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Longitude)));
                }
            }
        }

        private string _latitude;

        public string Latitude
        {
            get { return _latitude; }

            set
            {
                if (!string.Equals(_latitude, value))
                {
                    _latitude = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Latitude)));
                }
            }
        }

        private string _sunriseTime;

        public string SunriseTime
        {
            get { return _sunriseTime; }

            set
            {
                if (!string.Equals(_sunriseTime, value))
                {
                    _sunriseTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SunriseTime)));
                }
            }
        }

        private string _sunsetTime;

        public string SunsetTime
        {
            get { return _sunsetTime; }

            set
            {
                if (!string.Equals(_sunsetTime, value))
                {
                    _sunsetTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SunsetTime)));
                }
            }
        }

        private bool _isPolarNightRegion;

        public bool IsPolarNightRegion
        {
            get { return _isPolarNightRegion; }

            set
            {
                if (!Equals(_isPolarNightRegion, value))
                {
                    _isPolarNightRegion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPolarNightRegion)));
                }
            }
        }

        private bool _isPolarDayRegion;

        public bool IsPolarDayRegion
        {
            get { return _isPolarDayRegion; }

            set
            {
                if (!Equals(_isPolarDayRegion, value))
                {
                    _isPolarDayRegion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPolarDayRegion)));
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

        private List<KeyValuePair<string, string>> AutoThemeSwitchTypeList { get; } = [];

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
            AutoThemeSwitchTypeList.Add(new KeyValuePair<string, string>(AutoThemeSwitchService.AutoThemeSwitchTypeList[0], AutoThemeSwitchTypeFixedTimeString));
            AutoThemeSwitchTypeList.Add(new KeyValuePair<string, string>(AutoThemeSwitchService.AutoThemeSwitchTypeList[1], AutoThemeSwitchTypeSunriseSunsetString));
            SelectedSystemThemeStyle = SystemThemeStyleList[0];
            SelectedAppThemeStyle = AppThemeStyleList[0];
            SelectedAutoThemeSwitchType = AutoThemeSwitchTypeList[0];
            Longitude = NotAvailableString;
            Latitude = NotAvailableString;
            SunriseTime = NotAvailableString;
            SunsetTime = NotAvailableString;
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
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
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

        /// <summary>
        /// 应用程序即将关闭时发生的事件
        /// </summary>
        private async void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                if (IsAutoThemeSwitchEnableValue && Equals(SelectedAutoThemeSwitchType, AutoThemeSwitchTypeList[1]) && DevicePositionService.IsInitialized)
                {
                    await UnInitializeDeviceServiceAsync();
                    Latitude = NotAvailableString;
                    Longitude = NotAvailableString;
                    SunriseTime = NotAvailableString;
                    SunsetTime = NotAvailableString;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DownloadManagerPage), nameof(OnApplicationExit), 1, e);
            }
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
                    User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                    User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                    User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                AutoThemeSwitchService.SetAutoThemeSwitchTypeValue(SelectedAutoThemeSwitchType.Key);
                AutoThemeSwitchService.SetAutoSwitchSystemThemeValue(IsAutoSwitchSystemThemeValue);
                AutoThemeSwitchService.SetAutoSwitchAppThemeValue(IsAutoSwitchAppThemeValue);
                AutoThemeSwitchService.SetIsShowColorInDarkThemeValue(IsShowColorInDarkThemeValue);
                AutoThemeSwitchService.SetSystemThemeLightTime(SystemThemeLightTime);
                AutoThemeSwitchService.SetSystemThemeDarkTime(SystemThemeDarkTime);
                AutoThemeSwitchService.SetAppThemeLightTime(AppThemeLightTime);
                AutoThemeSwitchService.SetAppThemeDarkTime(AppThemeDarkTime);
                AutoThemeSwitchService.SetSunriseOffset(SunriseOffset);
                AutoThemeSwitchService.SetSunsetOffset(SunsetOffset);

                if (IsAutoThemeSwitchEnableValue)
                {
                    try
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
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnSaveClicked), 1, e);
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
                AutoThemeSwitchService.SetAutoThemeSwitchTypeValue(AutoThemeSwitchService.DefaultAutoThemeSwitchTypeValue);
                AutoThemeSwitchService.SetAutoSwitchSystemThemeValue(AutoThemeSwitchService.DefaultAutoSwitchSystemThemeValue);
                AutoThemeSwitchService.SetAutoSwitchAppThemeValue(AutoThemeSwitchService.DefaultAutoSwitchAppThemeValue);
                AutoThemeSwitchService.SetIsShowColorInDarkThemeValue(AutoThemeSwitchService.DefaultIsShowColorInDarkThemeValue);
                AutoThemeSwitchService.SetSystemThemeLightTime(AutoThemeSwitchService.DefaultSystemThemeLightTime);
                AutoThemeSwitchService.SetSystemThemeDarkTime(AutoThemeSwitchService.DefaultSystemThemeDarkTime);
                AutoThemeSwitchService.SetAppThemeLightTime(AutoThemeSwitchService.DefaultAppThemeLightTime);
                AutoThemeSwitchService.SetAppThemeDarkTime(AutoThemeSwitchService.DefaultAppThemeDarkTime);
                AutoThemeSwitchService.SetSunriseOffset(AutoThemeSwitchService.DefaultSunriseOffset);
                AutoThemeSwitchService.SetSunsetOffset(AutoThemeSwitchService.DefaultSunriseOffset);
            });

            IsAutoThemeSwitchEnableValue = AutoThemeSwitchService.DefaultAutoThemeSwitchEnableValue;
            SelectedAutoThemeSwitchType = AutoThemeSwitchTypeList[AutoThemeSwitchService.AutoThemeSwitchTypeList.FindIndex(item => string.Equals(item, AutoThemeSwitchService.DefaultAutoThemeSwitchTypeValue))];
            IsAutoSwitchSystemThemeValue = AutoThemeSwitchService.DefaultAutoSwitchSystemThemeValue;
            IsAutoSwitchAppThemeValue = AutoThemeSwitchService.DefaultAutoSwitchAppThemeValue;
            IsShowColorInDarkThemeValue = AutoThemeSwitchService.DefaultIsShowColorInDarkThemeValue;
            SystemThemeLightTime = AutoThemeSwitchService.DefaultSystemThemeLightTime;
            SystemThemeDarkTime = AutoThemeSwitchService.DefaultSystemThemeDarkTime;
            AppThemeLightTime = AutoThemeSwitchService.DefaultAppThemeLightTime;
            AppThemeDarkTime = AutoThemeSwitchService.DefaultAppThemeDarkTime;
            SunriseOffset = AutoThemeSwitchService.DefaultSunriseOffset;
            SunsetOffset = AutoThemeSwitchService.DefaultSunsetOffset;
            IsThemeSwitchNotificationEnabled = AutoThemeSwitchService.AutoThemeSwitchEnableValue && !await Task.Run(GetStartupTaskEnabledAsync);
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ThemeSwitchRestoreResult));
        }

        /// <summary>
        /// 启动自动切换主题程序
        /// </summary>
        private void OnOpenAutoSwitchProgramClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
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
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnSaveClicked), 2, e);
                }
            });
        }

        /// <summary>
        /// 是否启用自动切换主题
        /// </summary>
        private async void OnAutoThemeSwitchEnableToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAutoThemeSwitchEnableValue = toggleSwitch.IsOn;
            }

            if (Equals(SelectedAutoThemeSwitchType, AutoThemeSwitchTypeList[1]))
            {
                if (IsAutoThemeSwitchEnableValue)
                {
                    if (!DevicePositionService.IsInitialized)
                    {
                        await InitializeDeviceServiceAsync();
                    }
                }
                else
                {
                    if (DevicePositionService.IsInitialized)
                    {
                        await UnInitializeDeviceServiceAsync();
                        Latitude = NotAvailableString;
                        Longitude = NotAvailableString;
                        SunriseTime = NotAvailableString;
                        SunsetTime = NotAvailableString;
                    }
                }
            }
        }

        /// <summary>
        /// 自动切换主题类型发生改变时触发的事件
        /// </summary>
        private async void OnAutoThemeSwitchTypeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> autoThemeSwitchType)
            {
                SelectedAutoThemeSwitchType = autoThemeSwitchType;
            }

            if (IsAutoThemeSwitchEnableValue)
            {
                if (Equals(SelectedAutoThemeSwitchType, AutoThemeSwitchTypeList[1]))
                {
                    if (!DevicePositionService.IsInitialized)
                    {
                        await InitializeDeviceServiceAsync();
                    }
                }
                else
                {
                    if (DevicePositionService.IsInitialized)
                    {
                        await UnInitializeDeviceServiceAsync();
                        Latitude = NotAvailableString;
                        Longitude = NotAvailableString;
                        SunriseTime = NotAvailableString;
                        SunsetTime = NotAvailableString;
                    }
                }
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
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseAppThemeSetTimeFlyoutClicked(object sender, RoutedEventArgs args)
        {
            AppThemeSetTimeFlyout.Hide();
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
        private void OnCloseSystemThemeSetTimeFlyoutClicked(object sender, RoutedEventArgs args)
        {
            SystemThemeSetTimeFlyout.Hide();
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

        /// <summary>
        /// 打开系统位置设置
        /// </summary>
        private void OnSystemPositionSettingsClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                Process.Start("ms-settings:privacy-location");
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnSystemPositionSettingsClicked), 1, e);
            }
        }

        /// <summary>
        /// 获取设备位置
        /// </summary>
        private async void OnLocatePositionClicked(object sender, RoutedEventArgs args)
        {
            IsGettingPosition = true;
            if (IsAutoThemeSwitchEnableValue && Equals(SelectedAutoThemeSwitchType, AutoThemeSwitchTypeList[1]))
            {
                if (DevicePositionService.IsInitialized)
                {
                    if (DevicePositionService.IsLoaded)
                    {
                        Latitude = Convert.ToString(DevicePositionService.Latitude);
                        Longitude = Convert.ToString(DevicePositionService.Longitude);
                        SunTimes sunTime = SunRiseSetHelper.CalculateSunriseSunset(DevicePositionService.Latitude, DevicePositionService.Longitude, DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day);

                        // 正常地区时间
                        if (!sunTime.IsPolarDay && !sunTime.IsPolarNight)
                        {
                            IsPolarDayRegion = false;
                            IsPolarNightRegion = false;
                            TimeSpan standardSunriseTime = new(sunTime.SunriseHour, sunTime.SunriseMinute, 0);
                            TimeSpan standardSunsetTime = new(sunTime.SunsetHour, sunTime.SunsetMinute, 0);
                            TimeSpan calculatedSunriseTime = standardSunriseTime + TimeSpan.FromMinutes(SunriseOffset);
                            TimeSpan calculatedSunsetTime = standardSunsetTime + TimeSpan.FromMinutes(SunsetOffset);
                            TimeSpan sunriseTime = new(calculatedSunriseTime.Hours, calculatedSunriseTime.Minutes, 0);
                            TimeSpan sunsetTime = new(calculatedSunsetTime.Hours, calculatedSunsetTime.Minutes, 0);

                            if (sunriseTime < sunsetTime)
                            {
                                SunriseTime = sunriseTime.ToString(@"hh\:mm");
                                SunsetTime = sunsetTime.ToString(@"hh\:mm");
                            }
                            else
                            {
                                SunriseOffset = 0;
                                SunsetOffset = 0;
                                SunriseTime = standardSunriseTime.ToString(@"hh\:mm");
                                SunsetTime = standardSunsetTime.ToString(@"hh\:mm");
                            }
                        }
                        // 极地地区时间
                        else
                        {
                            SunriseOffset = 0;
                            SunsetOffset = 0;

                            // 北半球极地时间
                            if (DevicePositionService.Latitude > 0)
                            {
                                // 北极极昼
                                if (sunTime.IsPolarDay)
                                {
                                    IsPolarDayRegion = false;
                                    IsPolarNightRegion = true;
                                    SunsetTime = PolarDayString;
                                }
                                else
                                {
                                    IsPolarDayRegion = true;
                                    IsPolarNightRegion = false;
                                    SunriseTime = PolarNightString;
                                }
                            }
                            // 南半球极地时间
                            else
                            {
                                // 南极极昼
                                if (sunTime.IsPolarDay)
                                {
                                    IsPolarDayRegion = false;
                                    IsPolarNightRegion = true;
                                    SunsetTime = PolarDayString;
                                }
                                else
                                {
                                    IsPolarDayRegion = true;
                                    IsPolarNightRegion = false;
                                    SunriseTime = PolarNightString;
                                }
                            }
                        }
                    }
                    else
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DevicePositionLoadFailed));
                    }
                }
                else
                {
                    await InitializeDeviceServiceAsync();
                }
            }
            IsGettingPosition = false;
        }

        /// <summary>
        /// 日出偏移时间发生变化后触发的事件
        /// </summary>
        private void OnSunriseOffsetValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int sunriseOffset = 0;
            if (args.NewValue is not double.NaN)
            {
                try
                {
                    sunriseOffset = Convert.ToInt32(args.NewValue);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnSunriseOffsetValueChanged), 1, e);
                }
            }

            if (IsAutoThemeSwitchEnableValue && Equals(SelectedAutoThemeSwitchType, AutoThemeSwitchTypeList[1]) && DevicePositionService.IsInitialized && DevicePositionService.IsLoaded)
            {
                Latitude = Convert.ToString(DevicePositionService.Latitude);
                Longitude = Convert.ToString(DevicePositionService.Longitude);
                SunTimes sunTime = SunRiseSetHelper.CalculateSunriseSunset(DevicePositionService.Latitude, DevicePositionService.Longitude, DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day);

                // 正常地区时间
                if (!sunTime.IsPolarDay && !sunTime.IsPolarNight)
                {
                    IsPolarDayRegion = false;
                    IsPolarNightRegion = false;
                    TimeSpan standardSunriseTime = new(sunTime.SunriseHour, sunTime.SunriseMinute, 0);
                    TimeSpan standardSunsetTime = new(sunTime.SunsetHour, sunTime.SunsetMinute, 0);
                    TimeSpan calculatedSunriseTime = standardSunriseTime + TimeSpan.FromMinutes(sunriseOffset);
                    TimeSpan calculatedSunsetTime = standardSunsetTime + TimeSpan.FromMinutes(SunsetOffset);
                    TimeSpan sunriseTime = new(calculatedSunriseTime.Hours, calculatedSunriseTime.Minutes, 0);
                    TimeSpan sunsetTime = new(calculatedSunsetTime.Hours, calculatedSunsetTime.Minutes, 0);

                    if (sunriseTime < sunsetTime)
                    {
                        SunriseOffset = sunriseOffset;
                        SunriseTime = sunriseTime.ToString(@"hh\:mm");
                    }
                    else
                    {
                        SunriseOffset = 0;
                        SunriseTime = standardSunriseTime.ToString(@"hh\:mm");
                    }
                }
                // 极地地区时间
                else
                {
                    SunriseOffset = 0;
                    SunsetOffset = 0;

                    // 北半球极地时间
                    if (DevicePositionService.Latitude > 0)
                    {
                        // 北极极昼
                        if (sunTime.IsPolarDay)
                        {
                            IsPolarDayRegion = false;
                            IsPolarNightRegion = true;
                            SunsetTime = PolarDayString;
                        }
                        else
                        {
                            IsPolarDayRegion = true;
                            IsPolarNightRegion = false;
                            SunriseTime = PolarNightString;
                        }
                    }
                    // 南半球极地时间
                    else
                    {
                        // 南极极昼
                        if (sunTime.IsPolarDay)
                        {
                            IsPolarDayRegion = false;
                            IsPolarNightRegion = true;
                            SunsetTime = PolarDayString;
                        }
                        else
                        {
                            IsPolarDayRegion = true;
                            IsPolarNightRegion = false;
                            SunriseTime = PolarNightString;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 日落偏移时间发生变化后触发的事件
        /// </summary>
        private void OnSunsetOffsetValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int sunsetOffset = 0;
            if (args.NewValue is not double.NaN)
            {
                try
                {
                    sunsetOffset = Convert.ToInt32(args.NewValue);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnSunsetOffsetValueChanged), 1, e);
                    sunsetOffset = 0;
                }
            }
            else
            {
                sunsetOffset = 0;
            }

            if (IsAutoThemeSwitchEnableValue && Equals(SelectedAutoThemeSwitchType, AutoThemeSwitchTypeList[1]) && DevicePositionService.IsInitialized && DevicePositionService.IsLoaded)
            {
                Latitude = Convert.ToString(DevicePositionService.Latitude);
                Longitude = Convert.ToString(DevicePositionService.Longitude);
                SunTimes sunTime = SunRiseSetHelper.CalculateSunriseSunset(DevicePositionService.Latitude, DevicePositionService.Longitude, DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day);

                // 正常地区时间
                if (!sunTime.IsPolarDay && !sunTime.IsPolarNight)
                {
                    IsPolarDayRegion = false;
                    IsPolarNightRegion = false;
                    TimeSpan standardSunriseTime = new(sunTime.SunriseHour, sunTime.SunriseMinute, 0);
                    TimeSpan standardSunsetTime = new(sunTime.SunsetHour, sunTime.SunsetMinute, 0);
                    TimeSpan calculatedSunriseTime = standardSunriseTime + TimeSpan.FromMinutes(SunriseOffset);
                    TimeSpan calculatedSunsetTime = standardSunsetTime + TimeSpan.FromMinutes(sunsetOffset);
                    TimeSpan sunriseTime = new(calculatedSunriseTime.Hours, calculatedSunriseTime.Minutes, 0);
                    TimeSpan sunsetTime = new(calculatedSunsetTime.Hours, calculatedSunsetTime.Minutes, 0);

                    if (sunriseTime < sunsetTime)
                    {
                        SunsetOffset = sunsetOffset;
                        SunsetTime = sunsetTime.ToString(@"hh\:mm");
                    }
                    else
                    {
                        SunsetOffset = 0;
                        SunsetTime = standardSunsetTime.ToString(@"hh\:mm");
                    }
                }
                // 极地地区时间
                else
                {
                    SunriseOffset = 0;
                    SunsetOffset = 0;

                    // 北半球极地时间
                    if (DevicePositionService.Latitude > 0)
                    {
                        // 北极极昼
                        if (sunTime.IsPolarDay)
                        {
                            IsPolarDayRegion = false;
                            IsPolarNightRegion = true;
                            SunsetTime = PolarDayString;
                        }
                        else
                        {
                            IsPolarDayRegion = true;
                            IsPolarNightRegion = false;
                            SunriseTime = PolarNightString;
                        }
                    }
                    // 南半球极地时间
                    else
                    {
                        // 南极极昼
                        if (sunTime.IsPolarDay)
                        {
                            IsPolarDayRegion = false;
                            IsPolarNightRegion = true;
                            SunsetTime = PolarDayString;
                        }
                        else
                        {
                            IsPolarDayRegion = true;
                            IsPolarNightRegion = false;
                            SunriseTime = PolarNightString;
                        }
                    }
                }
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

            SelectedAutoThemeSwitchType = AutoThemeSwitchTypeList.Find(item => string.Equals(item.Key, AutoThemeSwitchService.AutoThemeSwitchTypeValue, StringComparison.OrdinalIgnoreCase));
            IsShowThemeColorInStartAndTaskbarEnabled = Equals(SelectedSystemThemeStyle, SystemThemeStyleList[1]);
            bool showThemeColorInStartAndTaskbar = await Task.Run(GetShowThemeColorInStartAndTaskbar);
            IsShowThemeColorInStartAndTaskbar = showThemeColorInStartAndTaskbar;
            IsThemeSwitchNotificationEnabled = AutoThemeSwitchService.AutoThemeSwitchEnableValue && !await Task.Run(GetStartupTaskEnabledAsync);

            if (IsAutoThemeSwitchEnableValue && Equals(SelectedAutoThemeSwitchType, AutoThemeSwitchTypeList[1]) && !DevicePositionService.IsInitialized)
            {
                await InitializeDeviceServiceAsync();
            }
        }

        /// <summary>
        /// 初始化设备位置服务
        /// </summary>
        private async Task InitializeDeviceServiceAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (!DevicePositionService.IsInitialized)
                    {
                        DevicePositionService.Initialize();
                        DevicePositionService.StatusOrPositionChanged += OnStatusOrPositionChanged;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(InitializeDeviceServiceAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 卸载设备位置服务
        /// </summary>
        private async Task UnInitializeDeviceServiceAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (DevicePositionService.IsInitialized)
                    {
                        DevicePositionService.UnInitialize();
                        DevicePositionService.StatusOrPositionChanged -= OnStatusOrPositionChanged;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(UnInitializeDeviceServiceAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 位置或状态发生变化时触发的事件
        /// </summary>
        private async void OnStatusOrPositionChanged()
        {
            switch (DevicePositionService.GeoPositionStatus)
            {
                case GeoPositionStatus.Ready:
                    {
                        synchronizationContext.Post((_) =>
                        {
                            Latitude = Convert.ToString(DevicePositionService.Latitude);
                            Longitude = Convert.ToString(DevicePositionService.Longitude);
                            SunTimes sunTime = SunRiseSetHelper.CalculateSunriseSunset(DevicePositionService.Latitude, DevicePositionService.Longitude, DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day);

                            // 正常地区时间
                            if (!sunTime.IsPolarDay && !sunTime.IsPolarNight)
                            {
                                IsPolarDayRegion = false;
                                IsPolarNightRegion = false;
                                TimeSpan standardSunriseTime = new(sunTime.SunriseHour, sunTime.SunriseMinute, 0);
                                TimeSpan standardSunsetTime = new(sunTime.SunsetHour, sunTime.SunsetMinute, 0);
                                TimeSpan calculatedSunriseTime = standardSunriseTime + TimeSpan.FromMinutes(SunriseOffset);
                                TimeSpan calculatedSunsetTime = standardSunsetTime + TimeSpan.FromMinutes(SunsetOffset);
                                TimeSpan sunriseTime = new(calculatedSunriseTime.Hours, calculatedSunriseTime.Minutes, 0);
                                TimeSpan sunsetTime = new(calculatedSunsetTime.Hours, calculatedSunsetTime.Minutes, 0);

                                if (sunriseTime < sunsetTime)
                                {
                                    SunriseTime = sunriseTime.ToString(@"hh\:mm");
                                    SunsetTime = sunsetTime.ToString(@"hh\:mm");
                                }
                                else
                                {
                                    SunriseOffset = 0;
                                    SunsetOffset = 0;
                                    SunriseTime = standardSunriseTime.ToString(@"hh\:mm");
                                    SunsetTime = standardSunsetTime.ToString(@"hh\:mm");
                                }
                            }
                            // 极地地区时间
                            else
                            {
                                SunriseOffset = 0;
                                SunsetOffset = 0;

                                // 北半球极地时间
                                if (DevicePositionService.Latitude > 0)
                                {
                                    // 北极极昼
                                    if (sunTime.IsPolarDay)
                                    {
                                        IsPolarDayRegion = false;
                                        IsPolarNightRegion = true;
                                        SunsetTime = PolarDayString;
                                    }
                                    else
                                    {
                                        IsPolarDayRegion = true;
                                        IsPolarNightRegion = false;
                                        SunriseTime = PolarNightString;
                                    }
                                }
                                // 南半球极地时间
                                else
                                {
                                    // 南极极昼
                                    if (sunTime.IsPolarDay)
                                    {
                                        IsPolarDayRegion = false;
                                        IsPolarNightRegion = true;
                                        SunsetTime = PolarDayString;
                                    }
                                    else
                                    {
                                        IsPolarDayRegion = true;
                                        IsPolarNightRegion = false;
                                        SunriseTime = PolarNightString;
                                    }
                                }
                            }
                        }, null);
                        break;
                    }
                case GeoPositionStatus.Initializing:
                    {
                        synchronizationContext.Post((_) =>
                        {
                            Latitude = NotAvailableString;
                            Longitude = NotAvailableString;
                            SunriseTime = NotAvailableString;
                            SunsetTime = NotAvailableString;
                        }, null);
                        break;
                    }
                case GeoPositionStatus.NoData:
                    {
                        synchronizationContext.Post((_) =>
                        {
                            Latitude = NotAvailableString;
                            Longitude = NotAvailableString;
                            SunriseTime = NotAvailableString;
                            SunsetTime = NotAvailableString;
                        }, null);
                        break;
                    }
                case GeoPositionStatus.Disabled:
                    {
                        synchronizationContext.Post((_) =>
                        {
                            Latitude = NotAvailableString;
                            Longitude = NotAvailableString;
                            SunriseTime = NotAvailableString;
                            SunsetTime = NotAvailableString;
                        }, null);
                        break;
                    }
                default:
                    {
                        synchronizationContext.Post((_) =>
                        {
                            Latitude = NotAvailableString;
                            Longitude = NotAvailableString;
                            SunriseTime = NotAvailableString;
                            SunsetTime = NotAvailableString;
                        }, null);
                        break;
                    }
            }

            // 如果定位服务被禁用，直接卸载当前服务
            if (DevicePositionService.Permission is GeoPositionPermission.Denied || DevicePositionService.GeoPositionStatus is GeoPositionStatus.Disabled)
            {
                try
                {
                    DevicePositionService.StatusOrPositionChanged -= OnStatusOrPositionChanged;
                    synchronizationContext.Post(async (_) =>
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DevicePositionInitializeFailed));
                    }, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ThemeSwitchPage), nameof(OnStatusOrPositionChanged), 1, e);
                }
            }
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

        /// <summary>
        /// 获取自动切换系统主题时间显示状态值
        /// </summary>
        private Visibility GetAutoSwitchSystemThemeTimeState(string selectedAutoThemeSwitchType, bool isAutoSwitchSystemThemeValue)
        {
            return Equals(selectedAutoThemeSwitchType, AutoThemeSwitchTypeList[0].Key) && isAutoSwitchSystemThemeValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取自动切换应用主题时间显示状态值
        /// </summary>
        private Visibility GetAutoSwitchAppThemeTimeState(string selectedAutoThemeSwitchType, bool isAutoSwitchAppThemeValue)
        {
            return Equals(selectedAutoThemeSwitchType, AutoThemeSwitchTypeList[0].Key) && isAutoSwitchAppThemeValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取自动切换主题类型值
        /// </summary>
        private Visibility GetAutoThemeSwitchTypeState(string selectedAutoThemeSwitchType, string comparedAutoThemeSwitchType)
        {
            return string.Equals(selectedAutoThemeSwitchType, comparedAutoThemeSwitchType) ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool GetIsNotPolarRegion(bool isPolarDayRegion, bool isPolarNightRegion)
        {
            return !(isPolarDayRegion || isPolarNightRegion);
        }
    }
}
