using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
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
    internal sealed partial class AllToolsPage : Page
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AdvancedSystemOptionsDescriptionString = ResourceService.AllToolsResource.GetString("AdvancedSystemOptionsDescription");
        private readonly string AdvancedSystemOptionsString = ResourceService.AllToolsResource.GetString("AdvancedSystemOptions");
        private readonly string ContextMenuManagerDescriptionString = ResourceService.AllToolsResource.GetString("ContextMenuManagerDescription");
        private readonly string ContextMenuManagerString = ResourceService.AllToolsResource.GetString("ContextMenuManager");
        private readonly string DataVerifyEncryptDescriptionString = ResourceService.AllToolsResource.GetString("DataVerifyEncryptDescription");
        private readonly string DataVerifyEncryptString = ResourceService.AllToolsResource.GetString("DataVerifyEncrypt");
        private readonly string DownloadManagerDescriptionString = ResourceService.AllToolsResource.GetString("DownloadManagerDescription");
        private readonly string DownloadManagerString = ResourceService.AllToolsResource.GetString("DownloadManager");
        private readonly string DriverManagerDescriptionString = ResourceService.AllToolsResource.GetString("DriverManagerDescription");
        private readonly string DriverManagerString = ResourceService.AllToolsResource.GetString("DriverManager");
        private readonly string FileCertificateDescriptionString = ResourceService.AllToolsResource.GetString("FileCertificateDescription");
        private readonly string FileCertificateString = ResourceService.AllToolsResource.GetString("FileCertificate");
        private readonly string FileToolsString = ResourceService.AllToolsResource.GetString("FileTools");
        private readonly string FileManagerDescriptionString = ResourceService.AllToolsResource.GetString("FileManagerDescription");
        private readonly string FileManagerString = ResourceService.AllToolsResource.GetString("FileManager");
        private readonly string FileUnlockDescriptionString = ResourceService.AllToolsResource.GetString("FileUnlockDescription");
        private readonly string FileUnlockString = ResourceService.AllToolsResource.GetString("FileUnlock");
        private readonly string IconExtractDescriptionString = ResourceService.AllToolsResource.GetString("IconExtractDescription");
        private readonly string IconExtractString = ResourceService.AllToolsResource.GetString("IconExtract");
        private readonly string LoafDescriptionString = ResourceService.AllToolsResource.GetString("LoafDescription");
        private readonly string LoafString = ResourceService.AllToolsResource.GetString("Loaf");
        private readonly string LoopbackManagerDescriptionString = ResourceService.AllToolsResource.GetString("LoopbackManagerDescription");
        private readonly string LoopbackManagerString = ResourceService.AllToolsResource.GetString("LoopbackManager");
        private readonly string PersonalizeToolsString = ResourceService.AllToolsResource.GetString("PersonalizeTools");
        private readonly string PriExtractDescriptionString = ResourceService.AllToolsResource.GetString("PriExtractDescription");
        private readonly string PriExtractString = ResourceService.AllToolsResource.GetString("PriExtract");
        private readonly string RelaxToolsString = ResourceService.AllToolsResource.GetString("RelaxTools");
        private readonly string ResourceToolsString = ResourceService.AllToolsResource.GetString("ResourceTools");
        private readonly string ThemeSwitchDescriptionString = ResourceService.AllToolsResource.GetString("ThemeSwitchDescription");
        private readonly string ThemeSwitchString = ResourceService.AllToolsResource.GetString("ThemeSwitch");
        private readonly string ScheduledTaskManagerDescriptionString = ResourceService.AllToolsResource.GetString("ScheduledTaskManagerDescription");
        private readonly string ScheduledTaskManagerString = ResourceService.AllToolsResource.GetString("ScheduledTaskManager");
        private readonly string ShellMenuDescriptionString = ResourceService.AllToolsResource.GetString("ShellMenuDescription");
        private readonly string ShellMenuString = ResourceService.AllToolsResource.GetString("ShellMenu");
        private readonly string SystemInformationDescriptionString = ResourceService.AllToolsResource.GetString("SystemInformationDescription");
        private readonly string SystemInformationString = ResourceService.AllToolsResource.GetString("SystemInformation");
        private readonly string SystemToolsString = ResourceService.AllToolsResource.GetString("SystemTools");
        private readonly string UpdateManagerDescriptionString = ResourceService.AllToolsResource.GetString("UpdateManagerDescription");
        private readonly string UpdateManagerString = ResourceService.AllToolsResource.GetString("UpdateManager");
        private readonly string WinFRDescriptionString = ResourceService.AllToolsResource.GetString("WinFRDescription");
        private readonly string WinFRString = ResourceService.AllToolsResource.GetString("WinFR");
        private readonly string WinSATDescriptionString = ResourceService.AllToolsResource.GetString("WinSATDescription");
        private readonly string WinSATString = ResourceService.AllToolsResource.GetString("WinSAT");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private List<AllToolsItemGroupModel> AllToolsGroupList { get; } = [];

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal AllToolsPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 点击条目时进入条目对应的页面
        /// </summary>
        private void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is AllToolsItemModel allToolsItem && MainWindow.Current.GetSelectedItem(allToolsItem.NavigationPage, MainWindow.Current.NavigationViewItemMenuItemsCollection) is NavigationViewItemModel navigationViewItem)
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

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            AllToolsGroupList.Add(new()
            {
                GroupName = RelaxToolsString,
                AllToolsItemList =
                [
                    new()
                    {
                        Title = LoafString,
                        Description = LoafDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/Loaf.png",
                        NavigationPage = typeof(LoafPage)
                    }
                ]
            });
            AllToolsGroupList.Add(new()
            {
                GroupName = FileToolsString,
                AllToolsItemList =
                [
                    new()
                    {
                        Title = FileManagerString,
                        Description = FileManagerDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/FileManager.png",
                        NavigationPage = typeof(FileManagerPage)
                    },
                    new()
                    {
                        Title = FileCertificateString,
                        Description = FileCertificateDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/FileCertificate.png",
                        NavigationPage = typeof(FileCertificatePage)
                    },
                    new()
                    {
                        Title = FileUnlockString,
                        Description = FileUnlockDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/FileUnlock.png",
                        NavigationPage = typeof(FileUnlockPage)
                    },

                ]
            });
            AllToolsGroupList.Add(new()
            {
                GroupName = ResourceToolsString,
                AllToolsItemList =
                [
                    new()
                    {
                        Title = DataVerifyEncryptString,
                        Description = DataVerifyEncryptDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/DataVerifyEncrypt.png",
                        NavigationPage = typeof(DataVerifyEncryptPage)
                    },
                    new()
                    {
                        Title = DownloadManagerString,
                        Description = DownloadManagerDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/DownloadManager.png",
                        NavigationPage = typeof(DownloadManagerPage)
                    },
                    new()
                    {
                        Title = IconExtractString,
                        Description = IconExtractDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/IconExtract.png",
                        NavigationPage = typeof(IconExtractPage)
                    },
                    new()
                    {
                        Title = PriExtractString,
                        Description = PriExtractDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/PriExtract.png",
                        NavigationPage = typeof(PriExtractPage)
                    }
                ]
            });
            AllToolsGroupList.Add(new()
            {
                GroupName = PersonalizeToolsString,
                AllToolsItemList =
                [
                    new()
                    {
                        Title = ThemeSwitchString,
                        Description = ThemeSwitchDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/ThemeSwitch.png",
                        NavigationPage = typeof(ThemeSwitchPage)
                    },
                    new()
                    {
                        Title = ShellMenuString,
                        Description = ShellMenuDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/ShellMenu.png",
                        NavigationPage = typeof(ShellMenuPage)
                    },
                    new()
                    {
                        Title = ContextMenuManagerString,
                        Description = ContextMenuManagerDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/ContextMenuManager.png",
                        NavigationPage = typeof(ContextMenuManagerPage)
                    }
                ]
            });
            AllToolsGroupList.Add(new()
            {
                GroupName = SystemToolsString,
                AllToolsItemList =
                [
                    new()
                    {
                        Title = LoopbackManagerString,
                        Description = LoopbackManagerDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/LoopbackManager.png",
                        NavigationPage = typeof(LoopbackManagerPage)
                    },
                    new()
                    {
                        Title = ScheduledTaskManagerString,
                        Description = ScheduledTaskManagerDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/ScheduledTaskManager.png",
                        NavigationPage = typeof(ScheduledTaskManagerPage)
                    },
                    new()
                    {
                        Title = DriverManagerString,
                        Description = DriverManagerDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/DriverManager.png",
                        NavigationPage = typeof(DriverManagerPage)
                    },
                    new()
                    {
                        Title = UpdateManagerString,
                        Description = UpdateManagerDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/UpdateManager.png",
                        NavigationPage = typeof(UpdateManagerPage)
                    },
                    new()
                    {
                        Title = AdvancedSystemOptionsString,
                        Description = AdvancedSystemOptionsDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/AdvancedSystemOptions.png",
                        NavigationPage = typeof(AdvancedSystemOptionsPage)
                    },
                    new()
                    {
                        Title = WinFRString,
                        Description = WinFRDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/WinFR.png",
                        NavigationPage = typeof(WinFRPage)
                    },
                    new()
                    {
                        Title = WinSATString,
                        Description = WinSATDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/WinSAT.png",
                        NavigationPage = typeof(WinSATPage)
                    },
                    new()
                    {
                        Title = SystemInformationString,
                        Description = SystemInformationDescriptionString,
                        ImagePath = "ms-appx:///Assets/ControlIcon/SystemInformation.png",
                        NavigationPage = typeof(SystemInformationPage)
                    }
                ]
            });
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
