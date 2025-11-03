using Microsoft.Win32;
using System;
using System.Device.Location;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using ThemeSwitch.Services.Root;
using ThemeSwitchService.Extensions.DataType.Class;
using ThemeSwitchService.Extensions.DataType.Enums;
using ThemeSwitchService.Helpers.Root;
using ThemeSwitchService.Services.Controls.Settings;
using ThemeSwitchService.Services.Position;
using ThemeSwitchService.WindowsAPI.PInvoke.User32;

// 抑制 CA1806，CA1822 警告
#pragma warning disable CA1806,CA1822

namespace ThemeSwitchService
{
    /// <summary>
    /// 主题切换服务
    /// </summary>
    public class ThemeSwitchRootService : ServiceBase
    {
        private readonly Timer timer = new()
        {
            Interval = 10000,
            Enabled = true
        };

        public ThemeSwitchRootService()
        {
            timer.Elapsed += OnElapsed;
            AutoThemeSwitchService.InitializeOrUpdateAutoThemeSwitch();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            if (!timer.Enabled)
            {
                timer.Start();
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();

            if (timer.Enabled)
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// 暂停服务
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();

            if (timer.Enabled)
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// 继续运行服务
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();

            if (!timer.Enabled)
            {
                timer.Start();
            }
        }

        /// <summary>
        /// 系统关闭时触发的事件
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();

            if (timer.Enabled)
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// 时间流逝触发的事件
        /// </summary>
        private async void OnElapsed(object sender, ElapsedEventArgs args)
        {
            AutoThemeSwitchService.InitializeOrUpdateAutoThemeSwitch();

            // 已启用自动切换主题
            if (AutoThemeSwitchService.AutoThemeSwitchEnableValue)
            {
                // 固定时间段
                if (Equals(AutoThemeSwitchService.AutoThemeSwitchTypeValue, AutoThemeSwitchService.AutoThemeSwitchTypeList[0]))
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
                else if (Equals(AutoThemeSwitchService.AutoThemeSwitchEnableValue, AutoThemeSwitchService.AutoThemeSwitchTypeList[1]))
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
                                            User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                                            User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                                            User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                                            User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
        }

        /// <summary>
        /// 设置系统主题
        /// </summary>
        private void SetSystemTheme(TimeSpan currentTime, TimeSpan lightTime, TimeSpan darkTime, bool isShowColorInDarkThemeValue)
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
                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换深色主题
                else
                {
                    bool isModified = SetSystemDarkTheme(isShowColorInDarkThemeValue);

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
                if (currentTime > darkTime && currentTime < lightTime)
                {
                    bool isModified = SetSystemDarkTheme(isShowColorInDarkThemeValue);

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换浅色主题
                else
                {
                    bool isModified = SetSystemLightTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
            }
        }

        /// <summary>
        /// 设置应用主题
        /// </summary>
        private void SetAppTheme(TimeSpan currentTime, TimeSpan lightTime, TimeSpan darkTime)
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
                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换深色主题
                else
                {
                    bool isModified = SetAppDarkTheme();

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
                if (currentTime > darkTime && currentTime < lightTime)
                {
                    bool isModified = SetAppDarkTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
                // 切换浅色主题
                else
                {
                    bool isModified = SetAppLightTheme();

                    if (isModified)
                    {
                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
                    }
                }
            }
        }

        /// <summary>
        /// 设置系统浅色主题
        /// </summary>
        private bool SetSystemLightTheme()
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
        private bool SetSystemDarkTheme(bool isShowColorInDarkThemeValue)
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
        private bool SetAppLightTheme()
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
        private bool SetAppDarkTheme()
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
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitchService), nameof(ThemeSwitchRootService), nameof(InitializeDeviceServiceAsync), 1, e);
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
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitchService), nameof(ThemeSwitchRootService), nameof(UnInitializeDeviceServiceAsync), 1, e);
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
                                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                                        User32Library.SendMessageTimeout(new IntPtr(0xffff), WindowMessage.WM_SETTINGCHANGE, UIntPtr.Zero, Marshal.StringToHGlobalUni("ImmersiveColorSet"), SMTO.SMTO_ABORTIFHUNG, 50, out _);
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
                    LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitchService), nameof(ThemeSwitchRootService), nameof(OnStatusOrPositionChanged), 1, e);
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
    }
}
