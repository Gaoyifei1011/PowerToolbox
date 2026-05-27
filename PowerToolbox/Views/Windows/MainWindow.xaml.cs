using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.Backdrop;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Download;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Settings;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.Pages;
using PowerToolbox.WindowsAPI.PInvoke.Comctl32;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using PowerToolbox.WindowsAPI.PInvoke.Uxtheme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly string RunningAdministratorString = ResourceService.WindowResource.GetString("RunningAdministrator");
        private readonly string TitleString = ResourceService.WindowResource.GetString("Title");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private readonly OverlappedPresenter overlappedPresenter;
        private readonly SUBCLASSPROC mainWindowSubClassProc;
        private readonly ContentIsland contentIsland;
        private readonly InputKeyboardSource inputKeyboardSource;
        private readonly InputPointerSource inputPointerSource;
        private bool isProgrammaticExpan;

        public new static MainWindow Current { get; private set; }

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }

            set
            {
                if (!string.Equals(_windowTitle, value))
                {
                    _windowTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));
                }
            }
        }

        private SystemBackdrop _windowSystemBackdrop;

        public SystemBackdrop WindowSystemBackdrop
        {
            get { return _windowSystemBackdrop; }

            set
            {
                if (!Equals(_windowSystemBackdrop, value))
                {
                    _windowSystemBackdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowSystemBackdrop)));
                }
            }
        }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                }
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                if (!Equals(_isBackEnabled, value))
                {
                    _isBackEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBackEnabled)));
                }
            }
        }

        private NavigationViewItemModel _selectedItem;

        public NavigationViewItemModel SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            new KeyValuePair<string, Type>("AllTools",typeof(AllToolsPage)),
            new KeyValuePair<string, Type>("Relaxation", null),
            new KeyValuePair<string, Type>("Loaf", typeof(LoafPage)),
            new KeyValuePair<string, Type>("File",null),
            new KeyValuePair<string, Type>("FileManager",typeof(FileManagerPage)),
            new KeyValuePair<string, Type>("FileCertificate",typeof(FileCertificatePage)),
            new KeyValuePair<string, Type>("FileUnlock",typeof(FileUnlockPage)),
            new KeyValuePair<string, Type>("Resource",null),
            new KeyValuePair<string, Type>("DataVerifyEncrypt",typeof(DataVerifyEncryptPage)),
            new KeyValuePair<string, Type>("DownloadManager",typeof(DownloadManagerPage)),
            new KeyValuePair<string, Type>("IconExtract",typeof(IconExtractPage)),
            new KeyValuePair<string, Type>("PriExtract",typeof(PriExtractPage)),
            new KeyValuePair<string, Type>("Personalize",null),
            new KeyValuePair<string, Type>("ThemeSwitch",typeof(ThemeSwitchPage)),
            new KeyValuePair<string, Type>("ShellMenu",typeof(ShellMenuPage)),
            new KeyValuePair<string, Type>("ContextMenuManager",typeof(ContextMenuManagerPage)),
            new KeyValuePair<string, Type>("ExperimentalFeatureManager",typeof(ExperimentalFeatureManagerPage)),
            new KeyValuePair<string, Type>("System",null),
            new KeyValuePair<string, Type>("LoopbackManager",typeof(LoopbackManagerPage)),
            new KeyValuePair<string, Type>("ScheduledTaskManager",typeof(ScheduledTaskManagerPage)),
            new KeyValuePair<string, Type>("Hosts",typeof(HostsPage)),
            new KeyValuePair<string, Type>("DriverManager",typeof(DriverManagerPage)),
            new KeyValuePair<string, Type>("UpdateManager",typeof(UpdateManagerPage)),
            new KeyValuePair<string, Type>("WinFR",typeof(WinFRPage)),
            new KeyValuePair<string, Type>("WinSAT",typeof(WinSATPage)),
            new KeyValuePair<string, Type>("Settings",typeof(SettingsPage)),
        ];

        public WinRTObservableCollection<NavigationViewItemModel> NavigationViewItemMenuItemsCollection { get; } = [];

        public WinRTObservableCollection<NavigationViewItemModel> NavigationViewItemFooterMenuItemsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            Current = this;
            InitializeComponent();

            // 窗口部分初始化
            WindowTitle = RuntimeHelper.IsElevated ? TitleString + RunningAdministratorString : TitleString;
            overlappedPresenter = AppWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentIsland = ContentIsland.FindAllForCompositor(Compositor)[0];
            inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
            inputPointerSource = InputPointerSource.GetForIsland(contentIsland);

            // 挂载相应的事件
            AlwaysShowBackdropService.PropertyChanged += OnServicePropertyChanged;
            ThemeService.PropertyChanged += OnServicePropertyChanged;
            BackdropService.PropertyChanged += OnServicePropertyChanged;
            TopMostService.PropertyChanged += OnServicePropertyChanged;
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;
            inputPointerSource.PointerReleased += OnPointerReleased;

            // 标题栏和右键菜单设置
            SetClassicMenuTheme((Content as FrameworkElement).ActualTheme);

            // 为应用主窗口添加窗口过程
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass((nint)AppWindow.Id.Value, mainWindowSubClassProc, 0, 0);

            SetWindowTheme();
            SetSystemBackdrop();
            SetTopMost();

            // 默认直接显示到窗口中间
            User32Library.GetWindowRect((nint)AppWindow.Id.Value, out RECT rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            User32Library.SetWindowPos((nint)AppWindow.Id.Value, 0, (System.Windows.Forms.SystemInformation.WorkingArea.Width - width) / 2, (System.Windows.Forms.SystemInformation.WorkingArea.Height - height) / 2, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER);

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
                Shell32Library.DragAcceptFiles((nint)AppWindow.Id.Value, true);
            }

            NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/AllTools.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("AllTools"),
                NavigationTag = "AllTools",
                ParentTag = null,
                NavigationPage = typeof(AllToolsPage),
                VisibleState = Visibility.Visible
            });
            NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Seperator,
                NavigationIcon = null,
                NavigationTitle = null,
                NavigationTag = null,
                ParentTag = null,
                NavigationPage = null,
                VisibleState = Visibility.Visible
            });
            NavigationViewItemModel relaxationItem = new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/Relaxation.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("Relaxation"),
                NavigationTag = "Relaxation",
                ParentTag = null,
                NavigationPage = null,
                VisibleState = Visibility.Visible
            };
            relaxationItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/Loaf.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("Loaf"),
                NavigationTag = "Loaf",
                ParentTag = "Relaxation",
                NavigationPage = typeof(LoafPage),
                VisibleState = Visibility.Visible
            });
            NavigationViewItemMenuItemsCollection.Add(relaxationItem);
            NavigationViewItemModel fileItem = new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/File.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("File"),
                NavigationTag = "File",
                ParentTag = null,
                NavigationPage = null,
                VisibleState = Visibility.Visible
            };
            fileItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/FileManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("FileManager"),
                NavigationTag = "FileManager",
                ParentTag = "File",
                NavigationPage = typeof(FileManagerPage),
                VisibleState = Visibility.Visible
            });
            fileItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/FileCertificate.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("FileCertificate"),
                NavigationTag = "FileCertificate",
                ParentTag = "File",
                NavigationPage = typeof(FileCertificatePage),
                VisibleState = Visibility.Visible
            });
            fileItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/FileUnlock.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("FileUnlock"),
                NavigationTag = "FileUnlock",
                ParentTag = "File",
                NavigationPage = typeof(FileUnlockPage),
                VisibleState = Visibility.Visible
            });
            NavigationViewItemMenuItemsCollection.Add(fileItem);
            NavigationViewItemModel resourceItem = new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/Resource.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("Resource"),
                NavigationTag = "Resource",
                ParentTag = null,
                NavigationPage = null,
                VisibleState = Visibility.Visible
            };
            resourceItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/DataVerifyEncrypt.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("DataVerifyEncrypt"),
                NavigationTag = "DataVerifyEncrypt",
                ParentTag = "Resource",
                NavigationPage = typeof(DataVerifyEncryptPage),
                VisibleState = Visibility.Visible
            });
            resourceItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/DownloadManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("DownloadManager"),
                NavigationTag = "DownloadManager",
                ParentTag = "Resource",
                NavigationPage = typeof(DownloadManagerPage),
                VisibleState = Visibility.Visible
            });
            resourceItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/IconExtract.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("IconExtract"),
                NavigationTag = "IconExtract",
                ParentTag = "Resource",
                NavigationPage = typeof(IconExtractPage),
                VisibleState = Visibility.Visible
            });
            resourceItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/PriExtract.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("PriExtract"),
                NavigationTag = "PriExtract",
                ParentTag = "Resource",
                NavigationPage = typeof(PriExtractPage),
                VisibleState = Visibility.Visible
            });
            NavigationViewItemMenuItemsCollection.Add(resourceItem);
            NavigationViewItemModel personalizeItem = new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/Personalize.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("Personalize"),
                NavigationTag = "Personalize",
                ParentTag = null,
                NavigationPage = null,
                VisibleState = Visibility.Visible
            };
            personalizeItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/ThemeSwitch.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("ThemeSwitch"),
                NavigationTag = "ThemeSwitch",
                ParentTag = "Personalize",
                NavigationPage = typeof(ThemeSwitchPage),
                VisibleState = Visibility.Visible
            });
            personalizeItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/ShellMenu.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("ShellMenu"),
                NavigationTag = "ShellMenu",
                ParentTag = "Personalize",
                NavigationPage = typeof(ShellMenuPage),
                VisibleState = Visibility.Visible
            });
            personalizeItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/ContextMenuManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("ContextMenuManager"),
                NavigationTag = "ContextMenuManager",
                ParentTag = "Personalize",
                NavigationPage = typeof(ContextMenuManagerPage),
                VisibleState = Visibility.Visible
            });
            NavigationViewItemMenuItemsCollection.Add(personalizeItem);
            NavigationViewItemModel experimentalFeatureManagerItem = new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/ExperimentalFeatureManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("ExperimentalFeatureManager"),
                NavigationTag = "ExperimentalFeatureManager",
                ParentTag = null,
                NavigationPage = null,
                VisibleState = Visibility.Collapsed
            };
            NavigationViewItemMenuItemsCollection.Add(experimentalFeatureManagerItem);
            NavigationViewItemModel systemItem = new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/System.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("System"),
                NavigationTag = "System",
                ParentTag = null,
                NavigationPage = null,
                VisibleState = Visibility.Visible
            };
            systemItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/LoopbackManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("LoopbackManager"),
                NavigationTag = "LoopbackManager",
                ParentTag = "System",
                NavigationPage = typeof(LoopbackManagerPage),
                VisibleState = Visibility.Visible
            });
            systemItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/ScheduledTaskManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("ScheduledTaskManager"),
                NavigationTag = "ScheduledTaskManager",
                ParentTag = "System",
                NavigationPage = typeof(ScheduledTaskManagerPage),
                VisibleState = Visibility.Visible
            });
            systemItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/Hosts.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("Hosts"),
                NavigationTag = "Hosts",
                ParentTag = "System",
                NavigationPage = typeof(HostsPage),
                VisibleState = Visibility.Collapsed
            });
            systemItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/DriverManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("DriverManager"),
                NavigationTag = "DriverManager",
                ParentTag = "System",
                NavigationPage = typeof(DriverManagerPage),
                VisibleState = Visibility.Visible
            });
            systemItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/UpdateManager.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("UpdateManager"),
                NavigationTag = "UpdateManager",
                ParentTag = "System",
                NavigationPage = typeof(UpdateManagerPage),
                VisibleState = Visibility.Visible
            });
            systemItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/WinFR.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("WinFR"),
                NavigationTag = "WinFR",
                ParentTag = "System",
                NavigationPage = typeof(WinFRPage),
                VisibleState = Visibility.Visible
            });
            systemItem.NavigationViewItemMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/WinSAT.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("WinSAT"),
                NavigationTag = "WinSAT",
                ParentTag = "System",
                NavigationPage = typeof(WinSATPage),
                VisibleState = Visibility.Visible
            });
            NavigationViewItemMenuItemsCollection.Add(systemItem);
            NavigationViewItemFooterMenuItemsCollection.Add(new NavigationViewItemModel()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/ControlIcon/Settings.png") } },
                NavigationTitle = ResourceService.WindowResource.GetString("Settings"),
                NavigationTag = "Settings",
                ParentTag = null,
                NavigationPage = typeof(SettingsPage),
                VisibleState = Visibility.Visible
            });
        }

        #region 第一部分：窗口辅助类挂载的事件

        /// 处理键盘系统按键事件
        /// </summary>
        private async void OnSystemKeyDown(InputKeyboardSource sender, KeyEventArgs args)
        {
            if (args.VirtualKey is VirtualKey.F10 && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        /// <summary>
        /// 处理鼠标事件
        /// </summary>
        private async void OnPointerReleased(InputPointerSource sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.Properties.PointerUpdateKind is PointerUpdateKind.RightButtonReleased && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        #endregion 第一部分：窗口辅助类挂载的事件

        #region 第二部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage((nint)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage((nint)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage((nint)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage((nint)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage((nint)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage((nint)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_CLOSE, 0);
        }

        #endregion 第二部分：窗口右键菜单事件

        #region 第三部分：窗口内容挂载的事件

        /// <summary>
        /// 应用主题变化时设置标题栏按钮的颜色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(sender.ActualTheme);
            SetClassicMenuTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 按下 Alt + BackSpace 键时，导航控件返回到上一页
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Back && args.KeyStatus.IsMenuKeyDown)
            {
                NavigationFrom();
            }
        }

        #endregion 第三部分：窗口内容挂载的事件

        #region 第四部分：导航控件及其内容挂载的事件

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackClicked(object sender, RoutedEventArgs args)
        {
            NavigationFrom();
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            // 设置标题栏主题
            SetTitleBarTheme((Content as FrameworkElement).ActualTheme);
            SelectedItem = NavigationViewItemMenuItemsCollection[0];
            NavigateTo(typeof(AllToolsPage));
            IsBackEnabled = CanGoBack();
            SetPopupControlTheme(WindowTheme);
        }

        /// <summary>
        /// 当导航栏菜单中的选中项发生改变时触发的事件
        /// </summary>
        private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not null && !Equals(SelectedItem, args.SelectedItem))
            {
                SelectedItem = args.SelectedItem as NavigationViewItemModel;

                // 对应的页面为空，选中项修改为已经选择的页面
                if (SelectedItem.NavigationPage is null)
                {
                    Type currentPageType = GetCurrentPageType();
                    NavigationViewItemModel selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemMenuItemsCollection);
                    if (selectedNavigationViewItem is not null)
                    {
                        SelectedItem = selectedNavigationViewItem;
                    }
                    else
                    {
                        selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemFooterMenuItemsCollection);
                        SelectedItem = selectedNavigationViewItem is not null ? selectedNavigationViewItem : null;
                    }
                }
                // 切换到选中项对应的页面
                else
                {
                    Type currentPageType = GetCurrentPageType();
                    if (Equals(SelectedItem.NavigationPage, typeof(ShellMenuPage)))
                    {
                        NavigateTo(SelectedItem.NavigationPage, "ShellMenu");
                    }
                    else
                    {
                        NavigateTo(SelectedItem.NavigationPage);
                    }
                }
            }
        }

        /// <summary>
        /// 当树中的节点开始展开时发生时的事件
        /// </summary>
        private async void OnExpanding(NavigationView sender, NavigationViewItemExpandingEventArgs args)
        {
            Type currentPageType = GetCurrentPageType();
            if(isProgrammaticExpan)
            {
                isProgrammaticExpan = false;
                await Task.Delay(5);
            }

            // 切换到选中页面对应的项
            SelectedItem = null;
            NavigationViewItemModel selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemMenuItemsCollection);
            if (selectedNavigationViewItem is not null)
            {
                SelectedItem = selectedNavigationViewItem;
            }
            else
            {
                selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemFooterMenuItemsCollection);
                SelectedItem = selectedNavigationViewItem is not null ? selectedNavigationViewItem : null;
            }
        }

        /// <summary>
        /// 当树中的节点开始折叠时发生时的事件
        /// </summary>
        private void OnCollapsed(NavigationView sender, NavigationViewItemCollapsedEventArgs args)
        {
            Type currentPageType = GetCurrentPageType();

            // 切换到选中页面对应的项
            SelectedItem = null;
            NavigationViewItemModel selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemMenuItemsCollection);
            if (selectedNavigationViewItem is not null)
            {
                SelectedItem = selectedNavigationViewItem;
            }
            else
            {
                selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemFooterMenuItemsCollection);
                SelectedItem = selectedNavigationViewItem is not null ? selectedNavigationViewItem : null;
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private async void OnNavigated(object sender, NavigationEventArgs args)
        {
            try
            {
                Type currentPageType = GetCurrentPageType();

                // 切换到选中页面对应的项
                NavigationViewItemModel selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemMenuItemsCollection);
                // 显示未打开的父项
                if (ShowParentNavigationViewItem(selectedNavigationViewItem))
                {
                    await Task.Delay(5);
                }

                SelectedItem = null;
                if (selectedNavigationViewItem is not null)
                {
                    SelectedItem = selectedNavigationViewItem;
                }
                else
                {
                    selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemFooterMenuItemsCollection);
                    SelectedItem = selectedNavigationViewItem is not null ? selectedNavigationViewItem : null;
                }

                IsBackEnabled = CanGoBack();

                // 如果导航到更新页面、 Hosts 文件编辑器页面、文件恢复页面，而且是非管理员模式，显示提示对话框
                if ((Equals(currentPageType, typeof(UpdateManagerPage)) || Equals(currentPageType, typeof(HostsPage)) || Equals(currentPageType, typeof(WinFRPage))) && !RuntimeHelper.IsElevated)
                {
                    await ShowDialogAsync(new NeedElevatedDialog());
                    NavigationFrom();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(OnNavigated), 1, e);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(TraceEventType.Warning, nameof(PowerToolbox), nameof(MainWindow), nameof(OnNavigationFailed), 1, args.Exception);
            (Application.Current as MainApp).Dispose();
        }

        #endregion 第四部分：导航控件及其内容挂载的事件

        #region 第五部分：自定义事件

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            synchronizationContext.Post((_) =>
            {
                if (string.Equals(args.PropertyName, nameof(ThemeService.AppTheme)))
                {
                    SetWindowTheme();
                }
                if (string.Equals(args.PropertyName, nameof(BackdropService.AppBackdrop)))
                {
                    SetSystemBackdrop();
                }
                if (string.Equals(args.PropertyName, nameof(TopMostService.TopMostValue)))
                {
                    SetTopMost();
                }
            }, null);
        }

        #endregion 第五部分：自定义事件

        #region 第六部分：窗口及内容属性设置

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public void SetWindowTheme()
        {
            WindowTheme = string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]) ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark : Enum.TryParse(ThemeService.AppTheme, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;
        }

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        private void SetSystemBackdrop()
        {
            if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[1]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[2]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.BaseAlt);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[3]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Default);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[4]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[5]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Thin);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else
            {
                WindowSystemBackdrop = null;
                VisualStateManager.GoToState(MainPage, "BackgroundDefault", false);
            }
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private void SetClassicMenuTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            if (theme is ElementTheme.Light)
            {
                titleBar.PreferredTheme = TitleBarTheme.Light;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                titleBar.PreferredTheme = TitleBarTheme.Dark;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        /// <summary>
        /// 设置所有弹出控件主题
        /// </summary>
        private void SetPopupControlTheme(ElementTheme elementTheme)
        {
            foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
            {
                popup.RequestedTheme = elementTheme;

                if (popup.Child is FlyoutPresenter flyoutPresenter)
                {
                    flyoutPresenter.RequestedTheme = elementTheme;
                }

                if (popup.Child is Grid grid && grid.Name is "OuterOverflowContentRootV2")
                {
                    grid.RequestedTheme = elementTheme;
                }
            }
        }

        /// <summary>
        /// 设置窗口的置顶状态
        /// </summary>
        private void SetTopMost()
        {
            overlappedPresenter.IsAlwaysOnTop = TopMostService.TopMostValue;
        }

        #endregion 第六部分：窗口及内容属性设置

        #region 第七部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private nint MainWindowSubClassProc(nint hWnd, WindowMessage Msg, nuint wParam, nint lParam, uint uIdSubclass, nint dwRefData)
        {
            switch (Msg)
            {
                // 窗口位置发生变化时触发的消息
                case WindowMessage.WM_MOVE:
                    {
                        if (TitlebarMenuFlyout.IsOpen)
                        {
                            TitlebarMenuFlyout.Hide();
                        }

                        if (overlappedPresenter is not null)
                        {
                            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
                        }
                        break;
                    }
                // 窗口大小发生变化时触发的消息
                case WindowMessage.WM_SIZE:
                    {
                        if (TitlebarMenuFlyout.IsOpen)
                        {
                            TitlebarMenuFlyout.Hide();
                        }

                        if (overlappedPresenter is not null)
                        {
                            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
                        }

                        if (MainPage.IsLoaded)
                        {
                            double dpi = Convert.ToDouble(User32Library.GetDpiForWindow((nint)AppWindow.Id.Value)) / 96;
                            overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1000 * dpi);
                            overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(600 * dpi);
                        }
                        break;
                    }
                // 窗口激活状态发生变化时触发的消息
                case WindowMessage.WM_ACTIVATE:
                    {
                        try
                        {
                            if (WindowSystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                            {
                                materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue || wParam is not 0;
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(MainWindowSubClassProc), 1, e);
                        }
                        break;
                    }
                // 窗口关闭时触发的消息
                case WindowMessage.WM_CLOSE:
                    {
                        synchronizationContext.Post(async (_) =>
                        {
                            int count = 0;
                            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();
                            try
                            {
                                count = DownloadSchedulerService.DownloadSchedulerList.Count;
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(MainWindowSubClassProc), 2, e);
                            }
                            finally
                            {
                                DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                            }

                            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
                            if (count > 0)
                            {
                                Activate();

                                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                                ContentDialogResult contentDialogResult = await ShowDialogAsync(new ClosingWindowDialog());

                                if (contentDialogResult is ContentDialogResult.Primary)
                                {
                                    if (RuntimeHelper.IsElevated)
                                    {
                                        User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_REMOVE);
                                        User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
                                    }

                                    AlwaysShowBackdropService.PropertyChanged -= OnServicePropertyChanged;
                                    ThemeService.PropertyChanged -= OnServicePropertyChanged;
                                    BackdropService.PropertyChanged -= OnServicePropertyChanged;
                                    TopMostService.PropertyChanged -= OnServicePropertyChanged;
                                    inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
                                    inputPointerSource.PointerReleased -= OnPointerReleased;
                                    DownloadSchedulerService.TerminateDownload();
                                    Comctl32Library.RemoveWindowSubclass((nint)AppWindow.Id.Value, mainWindowSubClassProc, 0);
                                    (Application.Current as MainApp).Dispose();
                                }
                                else if (contentDialogResult is ContentDialogResult.Secondary)
                                {
                                    if (GetFrameContent() is not DownloadManagerPage)
                                    {
                                        NavigateTo(typeof(DownloadManagerPage));
                                    }
                                }
                            }
                            else
                            {
                                if (RuntimeHelper.IsElevated)
                                {
                                    User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_REMOVE);
                                    User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
                                }

                                AlwaysShowBackdropService.PropertyChanged -= OnServicePropertyChanged;
                                ThemeService.PropertyChanged -= OnServicePropertyChanged;
                                BackdropService.PropertyChanged -= OnServicePropertyChanged;
                                TopMostService.PropertyChanged -= OnServicePropertyChanged;
                                inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
                                inputPointerSource.PointerReleased -= OnPointerReleased;
                                Comctl32Library.RemoveWindowSubclass((nint)AppWindow.Id.Value, mainWindowSubClassProc, 0);
                                (Application.Current as MainApp).Dispose();
                            }
                        }, null);
                        return 0;
                    }
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (TitlebarMenuFlyout.IsOpen)
                        {
                            TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam is 2 && Content is not null && Content.XamlRoot is not null)
                        {
                            System.Drawing.Point cursorPos = new((int)LOWORD((uint)lParam), (int)HIWORD((uint)lParam));
                            User32Library.MapWindowPoints(0, hWnd, ref cursorPos, 2); ;
                            double dpi = Convert.ToDouble(User32Library.GetDpiForWindow((nint)AppWindow.Id.Value)) / 96;

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = Environment.OSVersion.Version.Build > 22000 ? new Point(cursorPos.X / dpi, cursorPos.Y / dpi) : new Point(cursorPos.X, cursorPos.Y)
                            };

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
                // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        SetWindowTheme();
                        SetClassicMenuTheme(WindowTheme);

                        synchronizationContext.Post((_) =>
                        {
                            SetPopupControlTheme(WindowTheme);
                        }, null);

                        if (GetFrameContent() is ThemeSwitchPage themeSwitchPage)
                        {
                            synchronizationContext.Post(async (_) =>
                            {
                                await themeSwitchPage.InitializeSystemThemeSettingsAsync();
                            }, null);
                        }
                        break;
                    }
                // 窗口 DPI 发生变化后触发的消息
                case WindowMessage.WM_DPICHANGED:
                    {
                        overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1000 * Convert.ToDouble(wParam) / 96);
                        overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(600 * Convert.ToDouble(wParam) / 96);
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new Point(0, 15),
                                ShowMode = FlyoutShowMode.Standard
                            };
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        else if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU)
                        {
                            if (lParam is (int)System.Windows.Forms.Keys.Space)
                            {
                                FlyoutShowOptions options = new()
                                {
                                    Position = new Point(0, 45),
                                    ShowMode = FlyoutShowMode.Standard
                                };
                                TitlebarMenuFlyout.ShowAt(null, options);
                                return 0;
                            }
                        }
                        break;
                    }
                // 提升权限时允许应用接收拖放消息
                case WindowMessage.WM_DROPFILES:
                    {
                        Task.Run(() =>
                        {
                            List<string> filesList = [];
                            char[] dragFileCharArray = new char[260];
                            uint filesCount = Shell32Library.DragQueryFile(wParam, 0xffffffffu, null, 0);

                            for (uint index = 0; index < filesCount; index++)
                            {
                                Array.Clear(dragFileCharArray, 0, dragFileCharArray.Length);
                                if (Shell32Library.DragQueryFile(wParam, index, dragFileCharArray, (uint)dragFileCharArray.Length) > 0)
                                {
                                    filesList.Add(new string(dragFileCharArray).Replace("\0", string.Empty));
                                }
                            }

                            Shell32Library.DragQueryPoint(wParam, out System.Drawing.Point point);
                            Shell32Library.DragFinish(wParam);

                            synchronizationContext.Post(async (_) =>
                            {
                                await SendReceivedFilesListAsync(filesList);
                            }, null);
                        });

                        break;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第七部分：窗口过程

        #region 第八部分：窗口导航方法

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null)
        {
            try
            {
                 // 导航到该项目对应的页面
                if(!Equals(GetCurrentPageType(),navigationPageType))
                {
                    (MainNavigationView.Content as Frame).Navigate(navigationPageType, parameter);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 页面向后导航
        /// </summary>
        public void NavigationFrom()
        {
            if (GetFrameContent() is ShellMenuPage shellMenuPage && shellMenuPage.BreadCollection.Count is 2)
            {
                shellMenuPage.NavigateTo(shellMenuPage.PageList[0], null, false);
                return;
            }

            if ((MainNavigationView.Content as Frame).CanGoBack)
            {
                (MainNavigationView.Content as Frame).GoBack();
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return (MainNavigationView.Content as Frame).CurrentSourcePageType;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        public object GetFrameContent()
        {
            return (MainNavigationView.Content as Frame).Content;
        }

        /// <summary>
        /// 检查当前页面是否能向后导航
        /// </summary>
        public bool CanGoBack()
        {
            return (MainNavigationView.Content as Frame).CanGoBack;
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        public NavigationViewItemModel GetSelectedItem(Type currentPageType, WinRTObservableCollection<NavigationViewItemModel> navigationViewItemMenuItemCollection)
        {
            foreach (NavigationViewItemModel navigationViewItem in navigationViewItemMenuItemCollection)
            {
                if(Equals(navigationViewItem.NavigationPage, currentPageType))
                {
                    return navigationViewItem;
                }

                // 递归遍历
                if(GetSelectedItem(currentPageType,navigationViewItem.NavigationViewItemMenuItemsCollection) is NavigationViewItemModel searchedNavigationViewItem)
                {
                    return searchedNavigationViewItem;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取选中的父项
        /// </summary>
        private NavigationViewItemModel GetParentNavigationViewItem(NavigationViewItemModel searchNavigationViewItem)
        {
            foreach (NavigationViewItemModel naviationViewItem in NavigationViewItemMenuItemsCollection)
            {
                if (string.Equals(naviationViewItem.NavigationTag, searchNavigationViewItem.ParentTag))
                {
                    return naviationViewItem;
                }
            }

            foreach (NavigationViewItemModel naviationViewItem in NavigationViewItemFooterMenuItemsCollection)
            {
                if (string.Equals(naviationViewItem.NavigationTag, searchNavigationViewItem.ParentTag))
                {
                    return naviationViewItem;
                }
            }

            return null;
        }

        /// <summary>
        /// 显示未打开的父项
        /// </summary>
        private bool ShowParentNavigationViewItem(NavigationViewItemModel selectedNavigationViewItem)
        {
            // 如果选中的是子项，而父项没有展开，则自动展开父项中所有的子项
            if (selectedNavigationViewItem is not null && !string.IsNullOrEmpty(selectedNavigationViewItem.ParentTag))
            {
                NavigationViewItemModel parentNavigationViewModelItem = GetParentNavigationViewItem(selectedNavigationViewItem);
                if (MainNavigationView.ContainerFromMenuItem(parentNavigationViewModelItem) is NavigationViewItem parentNavigationViewItem)
                {
                    MainNavigationView.Expand(parentNavigationViewItem);
                    isProgrammaticExpan = true;
                    return true;
                }
            }

            return false;
        }

        #endregion 第八部分：窗口导航方法

        #region 第九部分：显示对话框和应用通知

        /// <summary>
        /// 显示内容对话框
        /// </summary>
        public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog contentDialog)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            bool isDialogOpening = false;
            if (contentDialog is not null && Content is not null)
            {
                foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
                {
                    if (popup.Child is ContentDialog)
                    {
                        isDialogOpening = true;
                        break;
                    }
                }

                if (!isDialogOpening)
                {
                    try
                    {
                        contentDialog.XamlRoot = Content.XamlRoot;
                        dialogResult = await contentDialog.ShowAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(ShowDialogAsync), 1, e);
                    }
                }
            }

            return dialogResult;
        }

        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public async Task ShowNotificationAsync(TeachingTip teachingTip, int duration = 2000)
        {
            if (teachingTip is not null && Content is Page page && page.Content is Grid grid)
            {
                try
                {
                    grid.Children.Add(teachingTip);

                    teachingTip.IsOpen = true;
                    await Task.Delay(duration);
                    teachingTip.IsOpen = false;

                    // 应用内通知关闭动画显示耗费 300 ms
                    await Task.Delay(300);
                    grid.Children.Remove(teachingTip);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(ShowNotificationAsync), 1, e);
                }
            }
        }

        #endregion 第九部分：显示对话框和应用通知

        /// <summary>
        /// 将提权模式下拖放获得到的文件列表发送到各个页面
        /// </summary>
        public async Task SendReceivedFilesListAsync(List<string> filesList)
        {
            object currentFrameContent = GetFrameContent();
            if (currentFrameContent is DataVerifyEncryptPage dataVerifyEncryptPage)
            {
                object currentDataVerifyEncryptContent = dataVerifyEncryptPage.GetFrameContent();
                if (currentDataVerifyEncryptContent is DataEncryptPage dataEncryptPage)
                {
                    if (!dataEncryptPage.IsEncrypting && filesList.Count is 1)
                    {
                        dataEncryptPage.EncryptFile = filesList[0];
                    }
                }
                else if (currentDataVerifyEncryptContent is DataVerifyPage dataVerifyPage)
                {
                    if (!dataVerifyPage.IsVerifying && filesList.Count is 1)
                    {
                        dataVerifyPage.VerifyFile = filesList[0];
                    }
                }
            }
            else if (currentFrameContent is FileManagerPage fileManagerPage)
            {
                object currentFileManagerContent = fileManagerPage.GetFrameContent();
                if (currentFileManagerContent is FileNamePage fileNamePage)
                {
                    if (!fileNamePage.IsModifyingNow)
                    {
                        List<OldAndNewNameModel> fileNameList = await Task.Run(() =>
                        {
                            List<OldAndNewNameModel> fileNameList = [];

                            foreach (string file in filesList)
                            {
                                FileInfo fileInfo = new(file);
                                if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                fileNameList.Add(new()
                                {
                                    OriginalFileName = Path.GetFileName(file),
                                    OriginalFilePath = file,
                                });
                            }

                            return fileNameList;
                        });

                        fileNamePage.AddToFileNamePage(fileNameList);
                    }
                }
                else if (currentFileManagerContent is ExtensionNamePage extensionNamePage)
                {
                    if (!extensionNamePage.IsModifyingNow)
                    {
                        List<OldAndNewNameModel> extensionNameList = await Task.Run(() =>
                        {
                            List<OldAndNewNameModel> extensionNameList = [];

                            foreach (string file in filesList)
                            {
                                FileInfo fileInfo = new(file);
                                if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                if ((new FileInfo(fileInfo.FullName).Attributes & FileAttributes.Directory) is 0)
                                {
                                    extensionNameList.Add(new()
                                    {
                                        OriginalFileName = fileInfo.Name,
                                        OriginalFilePath = fileInfo.FullName
                                    });
                                }
                            }

                            return extensionNameList;
                        });

                        extensionNamePage.AddToExtensionNamePage(extensionNameList);
                    }
                }
                else if (currentFileManagerContent is UpperAndLowerCasePage upperAndLowerCasePage)
                {
                    if (!upperAndLowerCasePage.IsModifyingNow)
                    {
                        List<OldAndNewNameModel> upperAndLowerCaseList = await Task.Run(() =>
                        {
                            List<OldAndNewNameModel> upperAndLowerCaseList = [];

                            foreach (string file in filesList)
                            {
                                FileInfo fileInfo = new(file);
                                if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                upperAndLowerCaseList.Add(new()
                                {
                                    OriginalFileName = Path.GetFileName(file),
                                    OriginalFilePath = file,
                                });
                            }

                            return upperAndLowerCaseList;
                        });

                        upperAndLowerCasePage.AddToUpperAndLowerCasePage(upperAndLowerCaseList);
                    }
                }
                else if (currentFileManagerContent is FilePropertiesPage filePropertiesPage)
                {
                    if (!filePropertiesPage.IsModifyingNow)
                    {
                        List<OldAndNewPropertiesModel> filePropertiesList = await Task.Run(() =>
                        {
                            List<OldAndNewPropertiesModel> filePropertiesList = [];

                            foreach (string file in filesList)
                            {
                                FileInfo fileInfo = new(file);
                                if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                                {
                                    continue;
                                }

                                filePropertiesList.Add(new OldAndNewPropertiesModel()
                                {
                                    FileName = Path.GetFileName(file),
                                    FilePath = file,
                                });
                            }

                            return filePropertiesList;
                        });

                        filePropertiesPage.AddToFilePropertiesPage(filePropertiesList);
                    }
                }
            }
            else if (currentFrameContent is FileCertificatePage fileCertificatePage)
            {
                if (!fileCertificatePage.IsModifyingNow)
                {
                    List<CertificateResultModel> fileCertificateList = await Task.Run(() =>
                    {
                        List<CertificateResultModel> fileCertificateList = [];

                        foreach (string file in filesList)
                        {
                            FileInfo fileInfo = new(file);
                            if ((fileInfo.Attributes & FileAttributes.Hidden) is FileAttributes.Hidden)
                            {
                                continue;
                            }

                            if ((new FileInfo(fileInfo.FullName).Attributes & FileAttributes.Directory) is 0)
                            {
                                fileCertificateList.Add(new CertificateResultModel()
                                {
                                    FileName = fileInfo.Name,
                                    FilePath = fileInfo.FullName
                                });
                            }
                        }

                        return fileCertificateList;
                    });

                    fileCertificatePage.AddToFileCertificatePage(fileCertificateList);
                }
            }
            else if (currentFrameContent is IconExtractPage iconExtractPage)
            {
                if (iconExtractPage.GetIsNotParsingOrSaving(iconExtractPage.IconExtractResultKind, iconExtractPage.IsSaving) && filesList.Count is 1 && (string.Equals(Path.GetExtension(filesList[0]), ".exe") || string.Equals(Path.GetExtension(filesList[0]), ".dll")))
                {
                    if (Equals(iconExtractPage.SelectedGetIconType, iconExtractPage.GetIconTypeList[0]))
                    {
                        await iconExtractPage.ParseIconFileAsync(filesList[0]);
                    }
                    else if (Equals(iconExtractPage.SelectedGetIconType, iconExtractPage.GetIconTypeList[1]))
                    {
                        await iconExtractPage.ParseIconFileAsync(filesList[1]);
                    }
                }
            }
            else if (currentFrameContent is PriExtractPage priExtractPage)
            {
                if (!priExtractPage.IsProcessing && filesList.Count is 1 && string.Equals(Path.GetExtension(filesList[0]), ".pri"))
                {
                    await priExtractPage.ParseResourceFileAsync(filesList[0]);
                }
            }
            else if (currentFrameContent is FileUnlockPage fileUnlockPage)
            {
                if (!fileUnlockPage.IsModifyingNow)
                {
                    List<FileUnlockModel> fileUnlockList = await Task.Run(() =>
                    {
                        List<FileUnlockModel> fileUnlockList = [];

                        foreach (string file in filesList)
                        {
                            try
                            {
                                FileInfo fileInfo = new(file);
                                FileUnlockModel fileUnlock = new()
                                {
                                    FileFolderName = fileInfo.Name,
                                    FileFolderPath = fileInfo.FullName,
                                    IsDirectory = (fileInfo.Attributes & FileAttributes.Directory) is FileAttributes.Directory
                                };

                                fileUnlockList.Add(fileUnlock);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(MainWindow), nameof(SendReceivedFilesListAsync), 1, e);
                            }
                        }

                        return fileUnlockList;
                    });

                    await fileUnlockPage.AddToFileUnlockPageAsync(fileUnlockList);
                }
            }
        }

        private uint HIWORD(uint dword)
        {
            return (dword >> 16) & 0xffff;
        }

        private uint LOWORD(uint dword)
        {
            return dword & 0xffff;
        }
    }
}
