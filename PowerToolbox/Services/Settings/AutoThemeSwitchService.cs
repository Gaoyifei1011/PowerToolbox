using PowerToolbox.Extensions.DataType.Constant;
using PowerToolbox.Services.Root;
using System;
using System.Collections.Generic;

namespace PowerToolbox.Services.Settings
{
    /// <summary>
    /// 自动切换主题服务
    /// </summary>
    public static class AutoThemeSwitchService
    {
        private static readonly string autoThemeSwitchEnableKey = ConfigKey.AutoThemeSwitchEnableKey;
        private static readonly string autoThemeSwitchTypeKey = ConfigKey.AutoThemeSwitchTypeKey;
        private static readonly string autoSwitchSystemThemeKey = ConfigKey.AutoSwitchSystemThemeKey;
        private static readonly string autoSwitchAppThemeKey = ConfigKey.AutoSwitchAppThemeKey;
        private static readonly string isShowColorInDarkThemeKey = ConfigKey.IsShowColorInDarkThemeKey;
        private static readonly string systemThemeLightTimeKey = ConfigKey.SystemThemeLightTimeKey;
        private static readonly string systemThemeDarkTimeKey = ConfigKey.SystemThemeDarkTimeKey;
        private static readonly string appThemeLightTimeKey = ConfigKey.AppThemeLightTimeKey;
        private static readonly string appThemeDarkTimeKey = ConfigKey.AppThemeDarkTimeKey;
        private static readonly string sunriseOffsetKey = ConfigKey.SunriseOffsetKey;
        private static readonly string sunsetOffsetKey = ConfigKey.SunsetOffsetKey;

        public static bool DefaultAutoThemeSwitchEnable { get; } = false;

        public static string DefaultAutoThemeSwitchType { get; private set; }

        public static bool DefaultAutoSwitchSystemTheme { get; } = false;

        public static bool DefaultAutoSwitchAppTheme { get; } = false;

        public static bool DefaultIsShowColorInDarkTheme { get; } = false;

        public static TimeSpan DefaultSystemThemeLightTime { get; } = new(7, 0, 0);

        public static TimeSpan DefaultSystemThemeDarkTime { get; } = new(19, 0, 0);

        public static TimeSpan DefaultAppThemeLightTime { get; } = new(7, 0, 0);

        public static TimeSpan DefaultAppThemeDarkTime { get; } = new(19, 0, 0);

        public static int DefaultSunriseOffset { get; } = 0;

        public static int DefaultSunsetOffset { get; } = 0;

        public static bool AutoThemeSwitchEnable { get; set; }

        public static string AutoThemeSwitchType { get; set; }

        public static bool AutoSwitchSystemTheme { get; set; }

        public static bool AutoSwitchAppTheme { get; set; }

        public static bool IsShowColorInDarkTheme { get; set; }

        public static TimeSpan SystemThemeLightTime { get; set; }

        public static TimeSpan SystemThemeDarkTime { get; set; }

        public static TimeSpan AppThemeLightTime { get; set; }

        public static TimeSpan AppThemeDarkTime { get; set; }

        public static int SunriseOffset { get; set; }

        public static int SunsetOffset { get; set; }

        public static List<string> AutoThemeSwitchTypeList { get; } = ["FixedTime", "SunriseSunset", "DarkMode"];

        /// <summary>
        /// 应用在初始化前获取设置存储的自动切换主题所有选项值
        /// </summary>
        public static void InitializeAutoThemeSwitch()
        {
            DefaultAutoThemeSwitchType = AutoThemeSwitchTypeList[0];
            AutoThemeSwitchEnable = GetAutoThemeSwitchEnable();
            AutoThemeSwitchType = GetAutoThemeSwitchType();
            AutoSwitchSystemTheme = GetAutoSwitchSystemTheme();
            AutoSwitchAppTheme = GetAutoSwitchAppTheme();
            IsShowColorInDarkTheme = GetIsShowColorInDarkTheme();
            SystemThemeLightTime = GetSystemThemeLightTime();
            SystemThemeDarkTime = GetSystemThemeDarkTime();
            AppThemeLightTime = GetAppThemeLightTime();
            AppThemeDarkTime = GetAppThemeDarkTime();
            SunriseOffset = GetSunriseOffset();
            SunsetOffset = GetSunsetOffset();
        }

        /// <summary>
        /// 获取设置存储的自动切换主题启用值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAutoThemeSwitchEnable()
        {
            bool? autoThemeSwitchEnable = LocalSettingsService.ReadSetting<bool?>(autoThemeSwitchEnableKey);

            if (!autoThemeSwitchEnable.HasValue)
            {
                SetAutoThemeSwitchEnable(DefaultAutoThemeSwitchEnable);
                return DefaultAutoThemeSwitchEnable;
            }

            return autoThemeSwitchEnable.Value;
        }

        /// <summary>
        /// 获取设置存储的自动切换系统主题启用值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAutoSwitchSystemTheme()
        {
            bool? autoSwitchSystemTheme = LocalSettingsService.ReadSetting<bool?>(autoSwitchSystemThemeKey);

            if (!autoSwitchSystemTheme.HasValue)
            {
                SetAutoSwitchSystemTheme(DefaultAutoSwitchSystemTheme);
                return DefaultAutoSwitchSystemTheme;
            }

            return autoSwitchSystemTheme.Value;
        }

        /// <summary>
        /// 获取设置存储的自动切换主题类型值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetAutoThemeSwitchType()
        {
            string autoThemeSwitchType = LocalSettingsService.ReadSetting<string>(autoThemeSwitchTypeKey);

            if (string.IsNullOrEmpty(autoThemeSwitchType))
            {
                SetAutoThemeSwitchEnable(DefaultAutoThemeSwitchEnable);
                return DefaultAutoThemeSwitchType;
            }

            return autoThemeSwitchType;
        }

        /// <summary>
        /// 获取设置存储的自动切换应用主题启用值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAutoSwitchAppTheme()
        {
            bool? autoSwitchAppTheme = LocalSettingsService.ReadSetting<bool?>(autoSwitchAppThemeKey);

            if (!autoSwitchAppTheme.HasValue)
            {
                SetAutoSwitchAppTheme(DefaultAutoSwitchAppTheme);
                return DefaultAutoSwitchAppTheme;
            }

            return autoSwitchAppTheme.Value;
        }

        /// <summary>
        /// 获取设置存储的切换系统深色主题时显示主题色值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetIsShowColorInDarkTheme()
        {
            bool? isShowColorInDarkTheme = LocalSettingsService.ReadSetting<bool?>(isShowColorInDarkThemeKey);

            if (!isShowColorInDarkTheme.HasValue)
            {
                SetIsShowColorInDarkTheme(DefaultIsShowColorInDarkTheme);
                return DefaultIsShowColorInDarkTheme;
            }

            return isShowColorInDarkTheme.Value;
        }

        /// <summary>
        /// 获取设置存储的自动切换系统浅色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetSystemThemeLightTime()
        {
            string systemThemeLightTimeString = LocalSettingsService.ReadSetting<string>(systemThemeLightTimeKey);

            if (string.IsNullOrEmpty(systemThemeLightTimeString))
            {
                SetSystemThemeLightTime(DefaultSystemThemeLightTime);
                return DefaultSystemThemeLightTime;
            }
            else
            {
                if (TimeSpan.TryParse(systemThemeLightTimeString, out TimeSpan systemThemeLightTheme))
                {
                    SetSystemThemeLightTime(systemThemeLightTheme);
                    return systemThemeLightTheme;
                }
                else
                {
                    SetSystemThemeLightTime(DefaultSystemThemeLightTime);
                    return DefaultSystemThemeLightTime;
                }
            }
        }

        /// <summary>
        /// 获取设置存储的自动切换系统深色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetSystemThemeDarkTime()
        {
            string systemThemeDarkTimeString = LocalSettingsService.ReadSetting<string>(systemThemeDarkTimeKey);

            if (string.IsNullOrEmpty(systemThemeDarkTimeString))
            {
                SetSystemThemeDarkTime(DefaultSystemThemeDarkTime);
                return DefaultSystemThemeDarkTime;
            }
            else
            {
                if (TimeSpan.TryParse(systemThemeDarkTimeString, out TimeSpan systemThemeLightTheme))
                {
                    SetSystemThemeDarkTime(systemThemeLightTheme);
                    return systemThemeLightTheme;
                }
                else
                {
                    SetSystemThemeDarkTime(DefaultSystemThemeDarkTime);
                    return DefaultSystemThemeDarkTime;
                }
            }
        }

        /// <summary>
        /// 获取设置存储的自动切换应用浅色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetAppThemeLightTime()
        {
            string appThemeLightTimeString = LocalSettingsService.ReadSetting<string>(appThemeLightTimeKey);

            if (string.IsNullOrEmpty(appThemeLightTimeString))
            {
                SetAppThemeLightTime(DefaultAppThemeLightTime);
                return DefaultAppThemeLightTime;
            }
            else
            {
                if (TimeSpan.TryParse(appThemeLightTimeString, out TimeSpan appThemeLightTheme))
                {
                    SetAppThemeLightTime(appThemeLightTheme);
                    return appThemeLightTheme;
                }
                else
                {
                    SetAppThemeLightTime(DefaultAppThemeLightTime);
                    return DefaultAppThemeLightTime;
                }
            }
        }

        /// <summary>
        /// 获取设置存储的自动切换应用深色主题时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static TimeSpan GetAppThemeDarkTime()
        {
            string appThemeDarkTimeString = LocalSettingsService.ReadSetting<string>(appThemeDarkTimeKey);

            if (string.IsNullOrEmpty(appThemeDarkTimeString))
            {
                SetAppThemeDarkTime(DefaultAppThemeDarkTime);
                return DefaultAppThemeDarkTime;
            }
            else
            {
                if (TimeSpan.TryParse(appThemeDarkTimeString, out TimeSpan appThemeLightTheme))
                {
                    SetAppThemeDarkTime(appThemeLightTheme);
                    return appThemeLightTheme;
                }
                else
                {
                    SetAppThemeDarkTime(DefaultAppThemeDarkTime);
                    return DefaultAppThemeDarkTime;
                }
            }
        }

        /// <summary>
        /// 获取设置存储的日出时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static int GetSunriseOffset()
        {
            int? sunriseOffset = LocalSettingsService.ReadSetting<int?>(sunriseOffsetKey);

            if (!sunriseOffset.HasValue)
            {
                SetSunriseOffset(DefaultSunriseOffset);
                return DefaultSunriseOffset;
            }

            return sunriseOffset.Value;
        }

        /// <summary>
        /// 获取设置存储的日落时间值，如果设置没有存储，使用默认值
        /// </summary>
        private static int GetSunsetOffset()
        {
            int? sunsetOffset = LocalSettingsService.ReadSetting<int?>(sunsetOffsetKey);

            if (!sunsetOffset.HasValue)
            {
                SetSunsetOffset(DefaultSunsetOffset);
                return DefaultSunsetOffset;
            }

            return sunsetOffset.Value;
        }

        /// <summary>
        /// 自动切换主题启用值发生修改时修改设置存储的自动切换主题启用值
        /// </summary>
        public static void SetAutoThemeSwitchEnable(bool autoThemeSwitchEnable)
        {
            AutoThemeSwitchEnable = autoThemeSwitchEnable;
            LocalSettingsService.SaveSetting(autoThemeSwitchEnableKey, autoThemeSwitchEnable);
        }

        /// <summary>
        /// 自动切换主题类型值发生修改时修改设置存储的自动切换主题类型值
        /// </summary>
        public static void SetAutoThemeSwitchType(string autoThemeSwitchType)
        {
            AutoThemeSwitchType = autoThemeSwitchType;
            LocalSettingsService.SaveSetting(autoThemeSwitchTypeKey, autoThemeSwitchType);
        }

        /// <summary>
        /// 自动切换系统主题启用值发生修改时修改设置存储的自动切换系统主题启用值
        /// </summary>
        public static void SetAutoSwitchSystemTheme(bool autoSwitchSystemTheme)
        {
            AutoSwitchSystemTheme = autoSwitchSystemTheme;
            LocalSettingsService.SaveSetting(autoSwitchSystemThemeKey, autoSwitchSystemTheme);
        }

        /// <summary>
        /// 自动切换应用主题启用值发生修改时修改设置存储的自动切换系统主题启用值
        /// </summary>
        public static void SetAutoSwitchAppTheme(bool autoSwitchAppTheme)
        {
            AutoSwitchAppTheme = autoSwitchAppTheme;
            LocalSettingsService.SaveSetting(autoSwitchAppThemeKey, autoSwitchAppTheme);
        }

        /// <summary>
        /// 切换系统深色主题时显示主题色值发生修改时修改设置存储的切换系统深色主题时显示主题色值
        /// </summary>
        public static void SetIsShowColorInDarkTheme(bool isShowColorInDarkTheme)
        {
            IsShowColorInDarkTheme = isShowColorInDarkTheme;
            LocalSettingsService.SaveSetting(isShowColorInDarkThemeKey, isShowColorInDarkTheme);
        }

        /// <summary>
        /// 自动切换系统浅色主题时间值发生修改时修改设置存储的自动切换系统浅色主题时间值
        /// </summary>
        public static void SetSystemThemeLightTime(TimeSpan systemThemeLightTime)
        {
            SystemThemeLightTime = systemThemeLightTime;
            LocalSettingsService.SaveSetting(systemThemeLightTimeKey, systemThemeLightTime.ToString());
        }

        /// <summary>
        /// 自动切换系统深色主题时间值发生修改时修改设置存储的自动切换系统深色主题时间值
        /// </summary>
        public static void SetSystemThemeDarkTime(TimeSpan systemThemeDarkTime)
        {
            SystemThemeDarkTime = systemThemeDarkTime;
            LocalSettingsService.SaveSetting(systemThemeDarkTimeKey, systemThemeDarkTime.ToString());
        }

        /// <summary>
        /// 自动切换系统浅色主题时间值发生修改时修改设置存储的自动切换系统浅色主题时间值
        /// </summary>
        public static void SetAppThemeLightTime(TimeSpan appThemeLightTime)
        {
            AppThemeLightTime = appThemeLightTime;
            LocalSettingsService.SaveSetting(appThemeLightTimeKey, appThemeLightTime.ToString());
        }

        /// <summary>
        /// 自动切换系统深色主题时间值发生修改时修改设置存储的自动切换系统深色主题时间值
        /// </summary>
        public static void SetAppThemeDarkTime(TimeSpan appThemeDarkTime)
        {
            AppThemeDarkTime = appThemeDarkTime;
            LocalSettingsService.SaveSetting(appThemeDarkTimeKey, appThemeDarkTime.ToString());
        }

        /// <summary>
        /// 日出时间值发生修改时修改设置存储的日出时间值
        /// </summary>
        public static void SetSunriseOffset(int sunriseOffset)
        {
            SunriseOffset = sunriseOffset;
            LocalSettingsService.SaveSetting(sunriseOffsetKey, sunriseOffset);
        }

        /// <summary>
        /// 日落时间值发生修改时修改设置存储的日落时间值
        /// </summary>
        public static void SetSunsetOffset(int sunsetOffset)
        {
            SunsetOffset = sunsetOffset;
            LocalSettingsService.SaveSetting(sunsetOffsetKey, sunsetOffset);
        }
    }
}
