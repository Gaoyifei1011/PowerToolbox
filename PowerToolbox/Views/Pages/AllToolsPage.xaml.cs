using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 所有工具页面
    /// </summary>
    public sealed partial class AllToolsPage : Page
    {
        private List<ControlItemModel> RelaxToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("Loaf"),
                Description = ResourceService.AllToolsResource.GetString("LoafDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/Loaf.png",
                Tag = "Loaf"
            }
        ];

        // 文件工具列表
        private List<ControlItemModel> FileToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileManager"),
                Description = ResourceService.AllToolsResource.GetString("FileManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileManager.png",
                Tag = "FileManager"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileCertificate"),
                Description = ResourceService.AllToolsResource.GetString("FileCertificateDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileCertificate.png",
                Tag = "FileCertificate"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileUnlock"),
                Description = ResourceService.AllToolsResource.GetString("FileUnlockDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileUnlock.png",
                Tag = "FileUnlock"
            }
        ];

        // 资源工具列表
        private List<ControlItemModel> ResourceToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("DataEncryptVerify"),
                Description = ResourceService.AllToolsResource.GetString("DataEncryptVerifyDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/DataEncryptVerify.png",
                Tag = "DataEncryptVerify"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("DownloadManager"),
                Description = ResourceService.AllToolsResource.GetString("DownloadManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/DownloadManager.png",
                Tag = "DownloadManager"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("IconExtract"),
                Description = ResourceService.AllToolsResource.GetString("IconExtractDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/IconExtract.png",
                Tag = "IconExtract"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("PriExtract"),
                Description = ResourceService.AllToolsResource.GetString("PriExtractDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/PriExtract.png",
                Tag = "PriExtract"
            }
        ];

        // 个性化工具列表
        private List<ControlItemModel> PersonalizeToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ThemeSwitch"),
                Description = ResourceService.AllToolsResource.GetString("ThemeSwitchDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ThemeSwitch.png",
                Tag = "ThemeSwitch"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ShellMenu"),
                Description = ResourceService.AllToolsResource.GetString("ShellMenuDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ShellMenu.png",
                Tag = "ShellMenu"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ContextMenuManager"),
                Description = ResourceService.AllToolsResource.GetString("ContextMenuManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ContextMenuManager.png",
                Tag = "ContextMenuManager"
            },
            //new ControlItemModel()
            //{
            //    Title = ResourceService.AllToolsResource.GetString("ExperimentalFeatureManager"),
            //    Description = ResourceService.AllToolsResource.GetString("ExperimentalFeatureManagerDescription"),
            //    ImagePath = "ms-appx:///Assets/ControlIcon/ExperimentalFeatureManager.png",
            //    Tag = "ExperimentalFeatureManager"
            //}
        ];

        // 系统工具列表
        private List<ControlItemModel> SystemToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("LoopbackManager"),
                Description = ResourceService.AllToolsResource.GetString("LoopbackManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/LoopbackManager.png",
                Tag = "LoopbackManager"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ScheduledTaskManager"),
                Description = ResourceService.AllToolsResource.GetString("ScheduledTaskManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ScheduledTaskManager.png",
                Tag = "ScheduledTaskManager"
            },
            //new ControlItemModel()
            //{
            //    Title = ResourceService.AllToolsResource.GetString("Hosts"),
            //    Description = ResourceService.AllToolsResource.GetString("HostsDescription"),
            //    ImagePath = "ms-appx:///Assets/ControlIcon/Hosts.png",
            //    Tag = "Hosts"
            //},
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("DriverManager"),
                Description = ResourceService.AllToolsResource.GetString("DriverManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/DriverManager.png",
                Tag = "DriverManager"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("UpdateManager"),
                Description = ResourceService.AllToolsResource.GetString("UpdateManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/UpdateManager.png",
                Tag = "UpdateManager"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("WinFR"),
                Description = ResourceService.AllToolsResource.GetString("WinFRDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/WinFR.png",
                Tag = "WinFR"
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("WinSAT"),
                Description = ResourceService.AllToolsResource.GetString("WinSATDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/WinSAT.png",
                Tag = "WinSAT"
            }
        ];

        public AllToolsPage()
        {
            InitializeComponent();
        }

        #region 第一部分：所有工具页面——挂载的事件

        /// <summary>
        /// 点击条目时进入条目对应的页面
        /// </summary>
        private void OnItemClick(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is ControlItemModel controlItem && MainWindow.Current.NavigationItemList.Find(item => string.Equals(item.NavigationTag, controlItem.Tag, StringComparison.OrdinalIgnoreCase)) is NavigationModel navigationItem)
            {
                if (Equals(navigationItem.NavigationPage, typeof(ShellMenuPage)))
                {
                    MainWindow.Current.NavigateTo(navigationItem.NavigationPage, "ShellMenu");
                }
                else
                {
                    MainWindow.Current.NavigateTo(navigationItem.NavigationPage);
                }
            }
        }

        #endregion 第一部分：所有工具页面——挂载的事件
    }
}
