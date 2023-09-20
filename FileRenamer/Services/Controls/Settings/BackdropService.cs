﻿using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Models;
using FileRenamer.Services.Root;
using System;
using System.Collections.Generic;

namespace FileRenamer.Services.Controls.Settings
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public static class BackdropService
    {
        private static string SettingsKey { get; } = ConfigKey.BackdropKey;

        private static GroupOptionsModel DefaultAppBackdrop { get; set; }

        public static GroupOptionsModel AppBackdrop { get; set; }

        public static List<GroupOptionsModel> BackdropList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public static void InitializeBackdrop()
        {
            BackdropList = ResourceService.BackdropList;

            DefaultAppBackdrop = BackdropList.Find(item => item.SelectedValue.Equals("Default", StringComparison.OrdinalIgnoreCase));

            AppBackdrop = GetBackdrop();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetBackdrop()
        {
            string backdrop = ConfigService.ReadSetting<string>(SettingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                SetBackdrop(DefaultAppBackdrop);
                return BackdropList.Find(item => item.SelectedValue.Equals(DefaultAppBackdrop.SelectedValue, StringComparison.OrdinalIgnoreCase));
            }

            return BackdropList.Find(item => item.SelectedValue.Equals(backdrop, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public static void SetBackdrop(GroupOptionsModel backdrop)
        {
            AppBackdrop = backdrop;

            ConfigService.SaveSetting(SettingsKey, backdrop.SelectedValue);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public static void SetAppBackdrop()
        {
            Program.MainWindow.SetWindowBackdrop();
        }
    }
}
