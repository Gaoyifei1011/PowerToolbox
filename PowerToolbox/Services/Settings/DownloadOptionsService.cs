using PowerToolbox.Extensions.DataType.Constant;
using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace PowerToolbox.Services.Settings
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public static class DownloadOptionsService
    {
        private static readonly string downloadFolderKey = ConfigKey.DownloadFolderKey;
        private static readonly string doEngineModeKey = ConfigKey.DoEngineModeKey;

        private static string defaultDoEngineMode;
        private static string defaultDownloadFolder;

        public static string DownloadFolder { get; private set; }

        public static string DoEngineMode { get; private set; }

        public static List<string> DoEngineModeList { get; } = ["DeliveryOptimization", "Bits", "Aria2"];

        /// <summary>
        /// 应用在初始化前获取设置存储的下载目录设置值，并创建默认下载目录
        /// </summary>
        public static void InitializeDownloadOptions()
        {
            Shell32Library.SHGetKnownFolderPath(new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"), KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, 0, out string downloadFolder);
            defaultDoEngineMode = DoEngineModeList[0];
            defaultDownloadFolder = downloadFolder;
            DownloadFolder = GetFolder();
            DoEngineMode = GetDoEngineMode();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private static string GetFolder()
        {
            string folder = LocalSettingsService.ReadSetting<string>(downloadFolderKey);

            try
            {
                if (string.IsNullOrEmpty(folder))
                {
                    SetFolder(defaultDownloadFolder);
                    return defaultDownloadFolder;
                }
                else
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    return folder;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Warning, nameof(PowerToolbox), nameof(DownloadOptionsService), nameof(GetFolder), 1, e);
                SetFolder(defaultDownloadFolder);
                return defaultDownloadFolder;
            }
        }

        /// <summary>
        /// 获取设置存储的下载引擎方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetDoEngineMode()
        {
            string doEngineMode = LocalSettingsService.ReadSetting<string>(doEngineModeKey);

            if (doEngineMode is null)
            {
                SetDoEngineMode(defaultDoEngineMode);
                return DoEngineModeList.Find(item => string.Equals(item, defaultDoEngineMode, StringComparison.OrdinalIgnoreCase));
            }

            string selectedDoEngine = DoEngineModeList.Find(item => string.Equals(item, doEngineMode, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(selectedDoEngine) ? defaultDoEngineMode : selectedDoEngine;
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public static void SetFolder(string downloadFolder)
        {
            DownloadFolder = downloadFolder;
            LocalSettingsService.SaveSetting(downloadFolderKey, downloadFolder);
        }

        /// <summary>
        /// 应用下载引擎发生修改时修改设置存储的下载引擎方式值
        /// </summary>
        public static void SetDoEngineMode(string doEngineMode)
        {
            DoEngineMode = doEngineMode;
            LocalSettingsService.SaveSetting(doEngineModeKey, doEngineMode);
        }
    }
}
