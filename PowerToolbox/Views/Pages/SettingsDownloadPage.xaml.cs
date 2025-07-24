﻿using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// 抑制 CA1806，IDE0060 警告
#pragma warning disable CA1806,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 设置下载管理页面
    /// </summary>
    public sealed partial class SettingsDownloadPage : Page, INotifyPropertyChanged
    {
        private readonly string DoEngineAria2String = ResourceService.SettingsDownloadResource.GetString("DoEngineAria2");
        private readonly string DoEngineBitsString = ResourceService.SettingsDownloadResource.GetString("DoEngineBits");
        private readonly string DoEngineDoString = ResourceService.SettingsDownloadResource.GetString("DoEngineDo");
        private readonly string SelectFolderString = ResourceService.SettingsDownloadResource.GetString("SelectFolder");

        private string _downloadFolder = DownloadOptionsService.DownloadFolder;

        public string DownloadFolder
        {
            get { return _downloadFolder; }

            set
            {
                if (!Equals(_downloadFolder, value))
                {
                    _downloadFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _doEngineMode;

        public KeyValuePair<string, string> DoEngineMode
        {
            get { return _doEngineMode; }

            set
            {
                if (!Equals(_doEngineMode, value))
                {
                    _doEngineMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoEngineMode)));
                }
            }
        }

        private List<KeyValuePair<string, string>> DoEngineModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsDownloadPage()
        {
            InitializeComponent();
            DoEngineModeList.Add(new KeyValuePair<string, string>(DownloadOptionsService.DoEngineModeList[0], DoEngineDoString));
            DoEngineModeList.Add(new KeyValuePair<string, string>(DownloadOptionsService.DoEngineModeList[1], DoEngineBitsString));
            DoEngineModeList.Add(new KeyValuePair<string, string>(DownloadOptionsService.DoEngineModeList[2], DoEngineAria2String));
            DoEngineMode = DoEngineModeList.Find(item => string.Equals(item.Key, DownloadOptionsService.DoEngineMode, StringComparison.OrdinalIgnoreCase));
        }

        #region 第一部分：设置下载管理页面——挂载的事件

        /// <summary>
        /// 打开下载文件存放目录
        /// </summary>
        private void OnDownloadOpenFolderClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(DownloadFolder);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsAdvancedPage), nameof(OnDownloadOpenFolderClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 修改下载文件存放目录
        /// </summary>
        private void OnDownloadChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            Shell32Library.SHGetKnownFolderPath(new("F1B32785-6FBA-4FCF-9D55-7B8E7F157091"), KNOWN_FOLDER_FLAG.KF_FLAG_FORCE_APP_DATA_REDIRECTION, nint.Zero, out string localAppdataPath);
                            DownloadFolder = localAppdataPath;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Download":
                        {
                            Shell32Library.SHGetKnownFolderPath(new("374DE290-123F-4565-9164-39C4925E467B"), KNOWN_FOLDER_FLAG.KF_FLAG_DEFAULT, nint.Zero, out string downloadFolder);
                            DownloadFolder = downloadFolder;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Custom":
                        {
                            OpenFolderDialog openFolderDialog = new()
                            {
                                Description = SelectFolderString,
                                RootFolder = Environment.SpecialFolder.Desktop
                            };
                            DialogResult dialogResult = openFolderDialog.ShowDialog();
                            if (dialogResult is DialogResult.OK || dialogResult is DialogResult.Yes)
                            {
                                DownloadFolder = openFolderDialog.SelectedPath;
                                DownloadOptionsService.SetFolder(DownloadFolder);
                            }
                            openFolderDialog.Dispose();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 打开传递优化设置
        /// </summary>
        private void OnOpenDeliveryOptimizationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("ms-settings:delivery-optimization");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsAdvancedPage), nameof(OnOpenDeliveryOptimizationClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 下载引擎说明
        /// </summary>
        private void OnLearnDoEngineClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.Content is MainPage mainPage && mainPage.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
        }

        /// <summary>
        /// 下载引擎方式设置
        /// </summary>

        private void OnDoEngineModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> doEngineMode)
            {
                DoEngineMode = doEngineMode;
                DownloadOptionsService.SetDoEngineMode(DoEngineMode.Key);
            }
        }

        /// <summary>
        /// 打开 Aria2 配置文件
        /// </summary>
        private void OnConfigurationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                if (!File.Exists(Aria2Service.Aria2ConfPath))
                {
                    Aria2Service.InitializeAria2Conf();
                }

                try
                {
                    Process.Start(Aria2Service.Aria2ConfPath);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(SettingsAdvancedPage), nameof(OnConfigurationClicked), 1, e);
                }
            });
        }

        #endregion 第一部分：设置下载管理页面——挂载的事件
    }
}
