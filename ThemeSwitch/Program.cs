using Microsoft.Win32;
using System;
using System.Device.Location;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ThemeSwitch.Extensions.DataType.Class;
using ThemeSwitch.Extensions.DataType.Enums;
using ThemeSwitch.Helpers.Root;
using ThemeSwitch.Services.Controls.Settings;
using ThemeSwitch.Services.Position;
using ThemeSwitch.Services.Root;
using ThemeSwitch.WindowsAPI.PInvoke.User32;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace ThemeSwitch
{
    public static class Program
    {
        private static readonly System.Timers.Timer timer = new()
        {
            Interval = 10000,
            Enabled = true
        };

        public static ManualResetEvent ManualResetEvent { get; set; }

        /// <summary>
        /// 主题切换后台程序
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (!RuntimeHelper.IsMSIX)
            {
                try
                {
                    Process.Start("powertoolbox:");
                }
                catch (Exception)
                { }
                return;
            }

            Mutex mutex = new(true, nameof(ThemeSwitch), out bool createdNew);

            if (createdNew)
            {
                InitializeProgramResources();
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                if (AutoThemeSwitchService.AutoThemeSwitchEnableValue)
                {
                    if (ManualResetEvent is null)
                    {
                        timer.Elapsed += OnElapsed;
                        if (!timer.Enabled)
                        {
                            timer.Start();
                        }
                        ManualResetEvent = new(false);
                        ManualResetEvent.WaitOne();
                        ManualResetEvent.Dispose();
                        ManualResetEvent = null;
                    }
                    else
                    {
                        mutex.Dispose();
                        return;
                    }
                }
                else
                {
                    mutex.Dispose();
                    return;
                }
            }
            else
            {
                mutex.Dispose();
                return;
            }
        }

        /// <summary>
        /// 处理应用非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(TraceEventType.Warning, nameof(ThemeSwitch), nameof(Program), nameof(OnUnhandledException), 1, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 时间流逝触发的事件
        /// </summary>
        private static async void OnElapsed(object sender, ElapsedEventArgs args)
        {
            AutoThemeSwitchService.InitializeOrUpdateAutoThemeSwitch();

            // 已启用自动切换主题
            if (AutoThemeSwitchService.AutoThemeSwitchEnableValue)
            {
                // 固定时间段
                if (string.Equals(AutoThemeSwitchService.AutoThemeSwitchTypeValue, AutoThemeSwitchService.AutoThemeSwitchTypeList[0]))
                {
                    // 如果地理位置服务开启，关闭地理位置服务
                    if (DevicePositionService.IsInitialized)
                    {
                        await UnInitializeDeviceServiceAsync();
                    }

                    TimeSpan currentTime = new(DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute, 0);

                    // 自动切换系统主题
                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                    {
                        SetSystemTheme(currentTime, AutoThemeSwitchService.SystemThemeLightTime, AutoThemeSwitchService.SystemThemeDarkTime, AutoThemeSwitchService.IsShowColorInDarkThemeValue);
                    }

                    // 自动切换应用主题
                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                    {
                        SetAppTheme(currentTime, AutoThemeSwitchService.AppThemeLightTime, AutoThemeSwitchService.AppThemeDarkTime);
                    }
                }
                // 日落日出
                else if (Equals(AutoThemeSwitchService.AutoThemeSwitchTypeValue, AutoThemeSwitchService.AutoThemeSwitchTypeList[1]))
                {
                    if (DevicePositionService.IsInitialized)
                    {
                        if (DevicePositionService.IsLoaded)
                        {
                            TimeSpan currentTime = new(DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute, 0);
                            SunTimes sunTime = SunRiseSetHelper.CalculateSunriseSunset(DevicePositionService.Latitude, DevicePositionService.Longitude, DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day);

                            // 正常地区时间
                            if (sunTime.HasSunset && sunTime.HasSunrise)
                            {
                                TimeSpan standardSunriseTime = new(sunTime.SunriseHour, sunTime.SunriseMinute, 0);
                                TimeSpan standardSunsetTime = new(sunTime.SunsetHour, sunTime.SunsetMinute, 0);
                                TimeSpan calculatedSunriseTime = standardSunriseTime + TimeSpan.FromMinutes(AutoThemeSwitchService.SunriseOffset);
                                TimeSpan calculatedSunsetTime = standardSunsetTime + TimeSpan.FromMinutes(AutoThemeSwitchService.SunsetOffset);
                                TimeSpan sunriseTime = new(calculatedSunriseTime.Hours, calculatedSunriseTime.Minutes, 0);
                                TimeSpan sunsetTime = new(calculatedSunsetTime.Hours, calculatedSunsetTime.Minutes, 0);

                                // 其他地区时间
                                if (sunriseTime < sunsetTime)
                                {
                                    // 自动切换系统主题
                                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                                    {
                                        SetSystemTheme(currentTime, sunriseTime, sunsetTime, AutoThemeSwitchService.IsShowColorInDarkThemeValue);
                                    }

                                    // 自动切换应用主题
                                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                                    {
                                        SetAppTheme(currentTime, sunriseTime, sunsetTime);
                                    }
                                }
                                else
                                {
                                    // 自动切换系统主题
                                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                                    {
                                        SetSystemTheme(currentTime, standardSunriseTime, standardSunsetTime, AutoThemeSwitchService.IsShowColorInDarkThemeValue);
                                    }

                                    // 自动切换应用主题
                                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                                    {
                                        SetAppTheme(currentTime, standardSunriseTime, standardSunsetTime);
                                    }
                                }
                            }
                            // 极地地区时间
                            else
                            {
                                // 北半球极地时间
                                if (DevicePositionService.Latitude > 0)
                                {
                                    // 北极极昼
                                    if (sunTime.IsPolarDay)
                                    {
                                        bool isModified = false;

                                        // 自动切换系统主题
                                        if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemLightTheme())
                                        {
                                            isModified = true;
                                        }

                                        // 自动切换应用主题
                                        if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppLightTheme())
                                        {
                                            isModified = true;
                                        }

                                        if (isModified)
                                        {
                                            User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                        }
                                    }
                                    // 北极极夜
                                    else
                                    {
                                        bool isModified = false;

                                        // 自动切换系统主题
                                        if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemDarkTheme(AutoThemeSwitchService.IsShowColorInDarkThemeValue))
                                        {
                                            isModified = true;
                                        }

                                        // 自动切换应用主题
                                        if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppDarkTheme())
                                        {
                                            isModified = true;
                                        }

                                        if (isModified)
                                        {
                                            User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                        }
                                    }
                                }
                                // 南半球极地时间
                                else
                                {
                                    // 南极极昼
                                    if (sunTime.IsPolarDay)
                                    {
                                        bool isModified = false;

                                        // 自动切换系统主题
                                        if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemLightTheme())
                                        {
                                            isModified = true;
                                        }

                                        // 自动切换应用主题
                                        if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppLightTheme())
                                        {
                                            isModified = true;
                                        }

                                        if (isModified)
                                        {
                                            User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                        }
                                    }
                                    // 南极极夜
                                    else
                                    {
                                        bool isModified = false;

                                        // 自动切换系统主题
                                        if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemDarkTheme(AutoThemeSwitchService.IsShowColorInDarkThemeValue))
                                        {
                                            isModified = true;
                                        }

                                        // 自动切换应用主题
                                        if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppDarkTheme())
                                        {
                                            isModified = true;
                                        }

                                        if (isModified)
                                        {
                                            User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            TimeSpan currentTime = new(DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute, 0);

                            // 自动切换系统主题
                            if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                            {
                                SetSystemTheme(currentTime, AutoThemeSwitchService.SystemThemeLightTime, AutoThemeSwitchService.SystemThemeDarkTime, AutoThemeSwitchService.IsShowColorInDarkThemeValue);
                            }

                            // 自动切换应用主题
                            if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                            {
                                SetAppTheme(currentTime, AutoThemeSwitchService.AppThemeLightTime, AutoThemeSwitchService.AppThemeDarkTime);
                            }
                        }
                    }
                    else
                    {
                        await InitializeDeviceServiceAsync();
                    }
                }
            }
            else
            {
                ManualResetEvent?.Set();
            }
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LogService.Initialize();
            AutoThemeSwitchService.InitializeOrUpdateAutoThemeSwitch();
        }

        /// <summary>
        /// 设置系统主题
        /// </summary>
        private static void SetSystemTheme(TimeSpan currentTime, TimeSpan lightTime, TimeSpan darkTime, bool isShowColorInDarkThemeValue)
        {
            // 白天时间小于夜间时间
            if (lightTime < darkTime)
            {
                // 介于白天时间和夜间时间，切换浅色主题
                if (currentTime > lightTime && currentTime < darkTime)
                {
                    bool isModified = SetSystemLightTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换深色主题
                else
                {
                    bool isModified = SetSystemDarkTheme(isShowColorInDarkThemeValue);

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
            }
            // 白天时间大于夜间时间
            else
            {
                // 介于白天时间和夜间时间，切换深色主题
                if (currentTime > darkTime && currentTime < lightTime)
                {
                    bool isModified = SetSystemDarkTheme(isShowColorInDarkThemeValue);

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换浅色主题
                else
                {
                    bool isModified = SetSystemLightTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
            }
        }

        /// <summary>
        /// 设置应用主题
        /// </summary>
        private static void SetAppTheme(TimeSpan currentTime, TimeSpan lightTime, TimeSpan darkTime)
        {
            // 白天时间小于夜间时间
            if (lightTime < darkTime)
            {
                // 介于白天时间和夜间时间，切换浅色主题
                if (currentTime > lightTime && currentTime < darkTime)
                {
                    bool isModified = SetAppLightTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换深色主题
                else
                {
                    bool isModified = SetAppDarkTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
            }
            // 白天时间大于夜间时间
            else
            {
                // 介于白天时间和夜间时间，切换深色主题
                if (currentTime > darkTime && currentTime < lightTime)
                {
                    bool isModified = SetAppDarkTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换浅色主题
                else
                {
                    bool isModified = SetAppLightTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
            }
        }

        /// <summary>
        /// 设置系统浅色主题
        /// </summary>
        private static bool SetSystemLightTheme()
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

            return isModified;
        }

        /// <summary>
        /// 设置系统深色主题
        /// </summary>
        private static bool SetSystemDarkTheme(bool isShowColorInDarkThemeValue)
        {
            bool isModified = false;

            if (GetSystemTheme() is ElementTheme.Light)
            {
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0);
                isModified = true;
            }

            if (isShowColorInDarkThemeValue && RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence") is 0)
            {
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", 1);
                isModified = true;
            }

            return isModified;
        }

        /// <summary>
        /// 设置应用浅色主题
        /// </summary>
        private static bool SetAppLightTheme()
        {
            bool isModified = false;

            if (GetAppTheme() is ElementTheme.Dark)
            {
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
                isModified = true;
            }

            return isModified;
        }

        /// <summary>
        /// 设置应用深色主题
        /// </summary>
        private static bool SetAppDarkTheme()
        {
            bool isModified = false;

            if (GetAppTheme() is ElementTheme.Light)
            {
                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0);
                isModified = true;
            }

            return isModified;
        }

        /// <summary>
        /// 初始化设备位置服务
        /// </summary>
        private static async Task InitializeDeviceServiceAsync()
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
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(Program), nameof(InitializeDeviceServiceAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 卸载设备位置服务
        /// </summary>
        private static async Task UnInitializeDeviceServiceAsync()
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
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(Program), nameof(UnInitializeDeviceServiceAsync), 1, e);
                }
            });
        }

        /// <summary>
        /// 位置或状态发生变化时触发的事件
        /// </summary>
        private static async void OnStatusOrPositionChanged()
        {
            switch (DevicePositionService.GeoPositionStatus)
            {
                case GeoPositionStatus.Ready:
                    {
                        TimeSpan currentTime = new(DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute, 0);
                        SunTimes sunTime = SunRiseSetHelper.CalculateSunriseSunset(DevicePositionService.Latitude, DevicePositionService.Longitude, DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day);

                        // 正常地区时间
                        if (!sunTime.IsPolarDay && !sunTime.IsPolarNight)
                        {
                            TimeSpan standardSunriseTime = new(sunTime.SunriseHour, sunTime.SunriseMinute, 0);
                            TimeSpan standardSunsetTime = new(sunTime.SunsetHour, sunTime.SunsetMinute, 0);
                            TimeSpan calculatedSunriseTime = standardSunriseTime + TimeSpan.FromMinutes(AutoThemeSwitchService.SunriseOffset);
                            TimeSpan calculatedSunsetTime = standardSunsetTime + TimeSpan.FromMinutes(AutoThemeSwitchService.SunsetOffset);
                            TimeSpan sunriseTime = new(calculatedSunriseTime.Hours, calculatedSunriseTime.Minutes, 0);
                            TimeSpan sunsetTime = new(calculatedSunsetTime.Hours, calculatedSunsetTime.Minutes, 0);

                            // 其他地区时间
                            if (sunriseTime < sunsetTime)
                            {
                                // 自动切换系统主题
                                if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                                {
                                    SetSystemTheme(currentTime, sunriseTime, sunsetTime, AutoThemeSwitchService.IsShowColorInDarkThemeValue);
                                }

                                // 自动切换应用主题
                                if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                                {
                                    SetAppTheme(currentTime, sunriseTime, sunsetTime);
                                }
                            }
                            else
                            {
                                // 自动切换系统主题
                                if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                                {
                                    SetSystemTheme(currentTime, standardSunriseTime, standardSunsetTime, AutoThemeSwitchService.IsShowColorInDarkThemeValue);
                                }

                                // 自动切换应用主题
                                if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                                {
                                    SetAppTheme(currentTime, standardSunriseTime, standardSunsetTime);
                                }
                            }
                        }
                        // 极地地区时间
                        else
                        {
                            // 北半球极地时间
                            if (DevicePositionService.Latitude > 0)
                            {
                                // 北极极昼
                                if (sunTime.IsPolarDay)
                                {
                                    bool isModified = false;

                                    // 自动切换系统主题
                                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemLightTheme())
                                    {
                                        isModified = true;
                                    }

                                    // 自动切换应用主题
                                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppLightTheme())
                                    {
                                        isModified = true;
                                    }

                                    if (isModified)
                                    {
                                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                    }
                                }
                                // 北极极夜
                                else
                                {
                                    bool isModified = false;

                                    // 自动切换系统主题
                                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemDarkTheme(AutoThemeSwitchService.IsShowColorInDarkThemeValue))
                                    {
                                        isModified = true;
                                    }

                                    // 自动切换应用主题
                                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppDarkTheme())
                                    {
                                        isModified = true;
                                    }

                                    if (isModified)
                                    {
                                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                    }
                                }
                            }
                            // 南半球极地时间
                            else
                            {
                                // 南极极昼
                                if (sunTime.IsPolarDay)
                                {
                                    bool isModified = false;

                                    // 自动切换系统主题
                                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemLightTheme())
                                    {
                                        isModified = true;
                                    }

                                    // 自动切换应用主题
                                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppLightTheme())
                                    {
                                        isModified = true;
                                    }

                                    if (isModified)
                                    {
                                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                    }
                                }
                                // 南极极夜
                                else
                                {
                                    bool isModified = false;

                                    // 自动切换系统主题
                                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue && SetSystemDarkTheme(AutoThemeSwitchService.IsShowColorInDarkThemeValue))
                                    {
                                        isModified = true;
                                    }

                                    // 自动切换应用主题
                                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue && SetAppDarkTheme())
                                    {
                                        isModified = true;
                                    }

                                    if (isModified)
                                    {
                                        User32Library.SendMessageTimeout(0xffff, WindowMessage.WM_SETTINGCHANGE, 0, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                                    }
                                }
                            }
                        }
                        break;
                    }
            }

            // 如果定位服务被禁用，直接卸载当前服务，并使用固定时间段数据
            if (DevicePositionService.Permission is GeoPositionPermission.Denied || DevicePositionService.GeoPositionStatus is GeoPositionStatus.Disabled)
            {
                try
                {
                    TimeSpan currentTime = new(DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute, 0);

                    // 自动切换系统主题
                    if (AutoThemeSwitchService.AutoSwitchSystemThemeValue)
                    {
                        SetSystemTheme(currentTime, AutoThemeSwitchService.SystemThemeLightTime, AutoThemeSwitchService.SystemThemeDarkTime, AutoThemeSwitchService.IsShowColorInDarkThemeValue);
                    }

                    // 自动切换应用主题
                    if (AutoThemeSwitchService.AutoSwitchAppThemeValue)
                    {
                        SetAppTheme(currentTime, AutoThemeSwitchService.AppThemeLightTime, AutoThemeSwitchService.AppThemeDarkTime);
                    }

                    DevicePositionService.StatusOrPositionChanged -= OnStatusOrPositionChanged;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(Program), nameof(OnStatusOrPositionChanged), 1, e);
                }
            }
        }

        /// <summary>
        /// 获取系统主题样式
        /// </summary>
        private static ElementTheme GetSystemTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }

        /// <summary>
        /// 获取应用主题样式
        /// </summary>
        private static ElementTheme GetAppTheme()
        {
            return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme") ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}
