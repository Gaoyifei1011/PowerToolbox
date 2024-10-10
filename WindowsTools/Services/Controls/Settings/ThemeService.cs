﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using WindowsTools.Extensions.DataType.Constant;
using WindowsTools.Services.Root;

namespace WindowsTools.Services.Controls.Settings
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static readonly string settingsKey = ConfigKey.ThemeKey;

        private static KeyValuePair<string, string> defaultAppTheme;

        private static KeyValuePair<string, string> _appTheme;

        public static KeyValuePair<string, string> AppTheme
        {
            get { return _appTheme; }

            private set
            {
                if (!Equals(_appTheme, value))
                {
                    _appTheme = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AppTheme)));
                }
            }
        }

        public static List<KeyValuePair<string, string>> ThemeList { get; private set; }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static void InitializeTheme()
        {
            ThemeList = ResourceService.ThemeList;

            defaultAppTheme = ThemeList.Find(item => item.Key.Equals(nameof(ElementTheme.Default), StringComparison.OrdinalIgnoreCase));

            AppTheme = GetTheme();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetTheme()
        {
            string theme = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                SetTheme(defaultAppTheme);
                return defaultAppTheme;
            }

            KeyValuePair<string, string> selectedTheme = ThemeList.Find(item => item.Key.Equals(theme));

            return selectedTheme.Key is null ? defaultAppTheme : ThemeList.Find(item => item.Key.Equals(theme));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static void SetTheme(KeyValuePair<string, string> theme)
        {
            AppTheme = theme;

            LocalSettingsService.SaveSetting(settingsKey, theme.Key);
        }
    }
}
