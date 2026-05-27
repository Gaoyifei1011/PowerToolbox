using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
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
        // 休闲工具列表
        private List<ControlItemModel> RelaxToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("Loaf"),
                Description = ResourceService.AllToolsResource.GetString("LoafDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/Loaf.png",
                NavigationPage = typeof(LoafPage)
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
                NavigationPage = typeof(FileManagerPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileCertificate"),
                Description = ResourceService.AllToolsResource.GetString("FileCertificateDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileCertificate.png",
                NavigationPage = typeof(FileCertificatePage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("FileUnlock"),
                Description = ResourceService.AllToolsResource.GetString("FileUnlockDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/FileUnlock.png",
                NavigationPage = typeof(FileUnlockPage)
            }
        ];

        // 资源工具列表
        private List<ControlItemModel> ResourceToolsList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("DataVerifyEncrypt"),
                Description = ResourceService.AllToolsResource.GetString("DataVerifyEncryptDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/DataVerifyEncrypt.png",
                NavigationPage = typeof(DataVerifyEncryptPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("DownloadManager"),
                Description = ResourceService.AllToolsResource.GetString("DownloadManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/DownloadManager.png",
                NavigationPage = typeof(DownloadManagerPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("IconExtract"),
                Description = ResourceService.AllToolsResource.GetString("IconExtractDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/IconExtract.png",
                NavigationPage = typeof(IconExtractPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("PriExtract"),
                Description = ResourceService.AllToolsResource.GetString("PriExtractDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/PriExtract.png",
                NavigationPage = typeof(PriExtractPage)
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
                NavigationPage = typeof(ThemeSwitchPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ShellMenu"),
                Description = ResourceService.AllToolsResource.GetString("ShellMenuDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ShellMenu.png",
                NavigationPage = typeof(ShellMenuPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ContextMenuManager"),
                Description = ResourceService.AllToolsResource.GetString("ContextMenuManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ContextMenuManager.png",
                NavigationPage = typeof(ContextMenuManagerPage)
            },
            //new ControlItemModel()
            //{
            //    Title = ResourceService.AllToolsResource.GetString("ExperimentalFeatureManager"),
            //    Description = ResourceService.AllToolsResource.GetString("ExperimentalFeatureManagerDescription"),
            //    ImagePath = "ms-appx:///Assets/ControlIcon/ExperimentalFeatureManager.png",
            //    NavigationPage = typeof(ExperimentalFeatureManagerPage)
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
                NavigationPage = typeof(LoopbackManagerPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("ScheduledTaskManager"),
                Description = ResourceService.AllToolsResource.GetString("ScheduledTaskManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/ScheduledTaskManager.png",
                NavigationPage = typeof(ScheduledTaskManagerPage)
            },
            //new ControlItemModel()
            //{
            //    Title = ResourceService.AllToolsResource.GetString("Hosts"),
            //    Description = ResourceService.AllToolsResource.GetString("HostsDescription"),
            //    ImagePath = "ms-appx:///Assets/ControlIcon/Hosts.png",
            //    NavigationPage = typeof(HostsPage)
            //},
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("DriverManager"),
                Description = ResourceService.AllToolsResource.GetString("DriverManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/DriverManager.png",
                NavigationPage = typeof(DriverManagerPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("UpdateManager"),
                Description = ResourceService.AllToolsResource.GetString("UpdateManagerDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/UpdateManager.png",
                NavigationPage = typeof(UpdateManagerPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("WinFR"),
                Description = ResourceService.AllToolsResource.GetString("WinFRDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/WinFR.png",
                NavigationPage = typeof(WinFRPage)
            },
            new ControlItemModel()
            {
                Title = ResourceService.AllToolsResource.GetString("WinSAT"),
                Description = ResourceService.AllToolsResource.GetString("WinSATDescription"),
                ImagePath = "ms-appx:///Assets/ControlIcon/WinSAT.png",
                NavigationPage = typeof(WinSATPage)
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
            if (args.ClickedItem is ControlItemModel controlItem && MainWindow.Current.GetSelectedItem(controlItem.NavigationPage, MainWindow.Current.NavigationViewItemMenuItemsCollection) is NavigationViewItemModel navigationViewItem)
            {
                if (Equals(navigationViewItem.NavigationPage, typeof(ShellMenuPage)))
                {
                    MainWindow.Current.NavigateTo(navigationViewItem.NavigationPage, "ShellMenu");
                }
                else
                {
                    MainWindow.Current.NavigateTo(navigationViewItem.NavigationPage);
                }
            }
        }

        #endregion 第一部分：所有工具页面——挂载的事件
    }
}
