﻿using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Services.Root;
using System;
using System.Threading.Tasks;

namespace FileRenamer.Services.Controls.Settings.Common
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public static class NotificationService
    {
        private static string SettingsKey { get; } = ConfigKey.NotificationKey;

        private static bool DefaultAppNotification { get; } = true;

        public static bool AppNotification { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用通知显示值
        /// </summary>
        public static async Task InitializeNotificationAsync()
        {
            AppNotification = await GetNotificationAsync();
        }

        /// <summary>
        /// 获取设置存储的应用通知显示值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetNotificationAsync()
        {
            bool? appNotification = await ConfigService.ReadSettingAsync<bool?>(SettingsKey);

            if (!appNotification.HasValue)
            {
                return DefaultAppNotification;
            }

            return Convert.ToBoolean(appNotification);
        }

        /// <summary>
        /// 应用通知显示发生修改时修改设置存储的使用说明按钮显示值
        /// </summary>
        public static async Task SetNotificationAsync(bool appNotification)
        {
            AppNotification = appNotification;

            await ConfigService.SaveSettingAsync(SettingsKey, appNotification);
        }
    }
}
