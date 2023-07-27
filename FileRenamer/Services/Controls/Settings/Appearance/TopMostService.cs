﻿using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Services.Root;
using System;
using System.Threading.Tasks;

namespace FileRenamer.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用窗口置顶设置服务
    /// </summary>
    public static class TopMostService
    {
        private static string SettingsKey { get; } = ConfigKey.TopMostKey;

        private static bool DefaultTopMostValue { get; } = false;

        public static bool TopMostValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的窗口置顶值
        /// </summary>
        public static async Task InitializeTopMostValueAsync()
        {
            TopMostValue = await GetTopMostValueAsync();
        }

        /// <summary>
        /// 获取设置存储的窗口置顶值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetTopMostValueAsync()
        {
            bool? topMostValue = await ConfigService.ReadSettingAsync<bool?>(SettingsKey);

            if (!topMostValue.HasValue)
            {
                return DefaultTopMostValue;
            }

            return Convert.ToBoolean(topMostValue);
        }

        /// <summary>
        /// 窗口置顶值发生修改时修改设置存储的窗口置顶值
        /// </summary>
        public static async Task SetTopMostValueAsync(bool topMostValue)
        {
            TopMostValue = topMostValue;

            await ConfigService.SaveSettingAsync(SettingsKey, topMostValue);
        }

        /// <summary>
        /// 设置应用的窗口置顶状态
        /// </summary>
        public static void SetAppTopMost()
        {
            Program.MainWindow.TopMost = TopMostValue;
        }
    }
}
