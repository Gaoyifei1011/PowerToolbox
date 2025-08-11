using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Shell;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 自定义扩展菜单列表页面
    /// </summary>
    public sealed partial class ShellMenuListPage : Page, INotifyPropertyChanged
    {
        private DateTimeOffset lastUpdateTime;
        private ShellMenuItemModel selectedItem;

        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (!Equals(_isLoading, value))
                {
                    _isLoading = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                }
            }
        }

        private bool _isAddMenuEnabled;

        public bool IsAddMenuEnabled
        {
            get { return _isAddMenuEnabled; }

            set
            {
                if (!Equals(_isAddMenuEnabled, value))
                {
                    _isAddMenuEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAddMenuEnabled)));
                }
            }
        }

        private bool _isRemoveMenuEnabled;

        public bool IsRemoveMenuEnabled
        {
            get { return _isRemoveMenuEnabled; }

            set
            {
                if (!Equals(_isRemoveMenuEnabled, value))
                {
                    _isRemoveMenuEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRemoveMenuEnabled)));
                }
            }
        }

        private bool _isEditMenuEnabled;

        public bool IsEditMenuEnabled
        {
            get { return _isEditMenuEnabled; }

            set
            {
                if (!Equals(_isEditMenuEnabled, value))
                {
                    _isEditMenuEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEditMenuEnabled)));
                }
            }
        }

        private bool _isMoveUpEnabled;

        public bool IsMoveUpEnabled
        {
            get { return _isMoveUpEnabled; }

            set
            {
                if (!Equals(_isMoveUpEnabled, value))
                {
                    _isMoveUpEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMoveUpEnabled)));
                }
            }
        }

        private bool _isMoveDownEnabled;

        public bool IsMoveDownEnabled
        {
            get { return _isMoveDownEnabled; }

            set
            {
                if (!Equals(_isMoveDownEnabled, value))
                {
                    _isMoveDownEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMoveDownEnabled)));
                }
            }
        }

        private ObservableCollection<ShellMenuItemModel> ShellMenuItemCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellMenuListPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            await GetShellMenuItemAsync();
            ActualThemeChanged += OnActualThemeChanged;
        }

        /// <summary>
        /// 离开该页面时触发的事件
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);
            try
            {
                ActualThemeChanged -= OnActualThemeChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ShellMenuListPage), nameof(OnNavigatedFrom), 1, e);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：自定义扩展菜单页面——挂载的事件

        /// <summary>
        /// 当前应用主题发生变化时对应的事件
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            foreach (ShellMenuItemModel shellMenuItem in ShellMenuItemCollection)
            {
                EnumModifyShellMenuItemTheme(shellMenuItem);
            }
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        private async void OnAddMenuItemClicked(object sender, RoutedEventArgs args)
        {
            Guid menuGuid = Guid.NewGuid();
            string menuKey = string.Empty;
            int menuIndex = -1;

            // 数据已经过时，需要更新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            await Task.Run(() =>
            {
                // 检查添加项是否为根菜单项
                if (selectedItem is null)
                {
                    menuKey = Path.Combine(@"Software\PowerToolbox\ShellMenu", Convert.ToString(menuGuid));
                    menuIndex = 0;
                }
                else if (selectedItem.MenuType is MenuType.FirstLevelMenu)
                {
                    menuKey = Path.Combine(selectedItem.MenuKey, Convert.ToString(menuGuid));
                    menuIndex = selectedItem.SubMenuItemCollection.Count;
                }
            });

            if (MainWindow.Current.Content is MainPage mainPage && mainPage.GetFrameContent() is ShellMenuPage shellMenuPage)
            {
                shellMenuPage.NavigateTo(shellMenuPage.PageList[1], new List<object>
                {
                    ShellEditKind.AddMenu,
                    new ShellMenuItem()
                    {
                        MenuGuid = menuGuid,
                        MenuKey = menuKey,
                        MenuIndex = menuIndex
                    }
                }, true);
            }
        }

        /// <summary>
        /// 移除菜单项
        /// </summary>
        private async void OnRemoveMenuItemClicked(object sender, RoutedEventArgs args)
        {
            // 数据已经过时，需要更新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            if (selectedItem is not null)
            {
                // 移除指定的菜单项
                await Task.Run(() =>
                {
                    ShellMenuService.RemoveShellMenuItem(selectedItem.MenuKey);
                });

                ShellMenuItemModel parentItem = await EnumRemoveItemAsync(selectedItem, null, ShellMenuItemCollection);

                // 删除的父项为空，说明被删除项为根菜单项
                if (parentItem is null)
                {
                    selectedItem = null;
                    IsAddMenuEnabled = true;
                    IsRemoveMenuEnabled = false;
                    IsEditMenuEnabled = false;
                    IsMoveUpEnabled = false;
                    IsMoveDownEnabled = false;
                }
                else
                {
                    parentItem.IsSelected = true;
                    selectedItem = parentItem;
                    IsAddMenuEnabled = true;
                    IsRemoveMenuEnabled = true;
                    IsEditMenuEnabled = true;
                    EnumModifySelectedItem(selectedItem, ShellMenuItemCollection);
                }

                ShellMenuService.UpdateLastUpdateTime();
                lastUpdateTime = ShellMenuService.GetLastUpdateTime();
            }
        }

        /// <summary>
        /// 清空菜单项
        /// </summary>
        private async void OnClearMenuClicked(object sender, RoutedEventArgs args)
        {
            // 数据已经过时，需要更新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            if (ShellMenuItemCollection.Count > 0)
            {
                string rootMenuKey = ShellMenuItemCollection[0].MenuKey;

                // 清空所有菜单项信息
                await Task.Run(() =>
                {
                    ShellMenuService.RemoveShellMenuItem(rootMenuKey);
                });

                ShellMenuItemCollection.Clear();
                selectedItem = null;
                IsAddMenuEnabled = true;
                IsRemoveMenuEnabled = false;
                IsEditMenuEnabled = false;
                IsMoveUpEnabled = false;
                IsMoveDownEnabled = false;
            }

            ShellMenuService.UpdateLastUpdateTime();
            lastUpdateTime = ShellMenuService.GetLastUpdateTime();
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetShellMenuItemAsync();
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        private async void OnEditMenuClicked(object sender, RoutedEventArgs args)
        {
            // 数据已经过时，需要更新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            if (MainWindow.Current.Content is MainPage mainPage && mainPage.GetFrameContent() is ShellMenuPage shellMenuPage)
            {
                shellMenuPage.NavigateTo(shellMenuPage.PageList[1], new List<object>
                {
                    ShellEditKind.EditMenu,
                    selectedItem
                }, true);
            }
        }

        /// <summary>
        /// 向上移动菜单项
        /// </summary>
        private async void OnMoveUpClicked(object sender, RoutedEventArgs args)
        {
            // 数据已经过时，需要更新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            await EnumMoveUpShellMenuItemAsync(selectedItem, ShellMenuItemCollection);
            ShellMenuService.UpdateLastUpdateTime();
            lastUpdateTime = ShellMenuService.GetLastUpdateTime();
        }

        /// <summary>
        /// 向下移动菜单项
        /// </summary>
        private async void OnMoveDownClicked(object sender, RoutedEventArgs args)
        {
            // 数据已经过时，需要更新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            await EnumMoveDownShellMenuItemAsync(selectedItem, ShellMenuItemCollection);
            ShellMenuService.UpdateLastUpdateTime();
            lastUpdateTime = ShellMenuService.GetLastUpdateTime();
        }

        /// <summary>
        /// 打开菜单设置
        /// </summary>
        private void OnMenuSettingsClicked(object sender, RoutedEventArgs args)
        {
            (MainWindow.Current.Content as MainPage).NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// 点击选中项触发的事件
        /// </summary>
        private void OnItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is ShellMenuItemModel shellMenuItem)
            {
                EnumModifySelectedItem(shellMenuItem, ShellMenuItemCollection);
            }
        }

        #endregion 第二部分：自定义扩展菜单页面——挂载的事件

        #region 第四部分：递归遍历

        /// <summary>
        /// 枚举并递归菜单项信息
        /// </summary>
        private ShellMenuItemModel EnumShellMenuItem(ShellMenuItem menuItem, MenuType menuType)
        {
            // 读取遍历到的当前项的信息
            ShellMenuItemModel shellMenuItem = new()
            {
                MenuKey = menuItem.MenuKey,
                IsSelected = menuType is MenuType.FirstLevelMenu,
                MenuType = menuType,
                MenuTitleText = menuItem.MenuTitleText,
                MenuGuid = menuItem.MenuGuid,
                DefaultIconPath = menuItem.DefaultIconPath,
                LightThemeIconPath = menuItem.LightThemeIconPath,
                DarkThemeIconPath = menuItem.DarkThemeIconPath,
                MenuProgramPathText = menuItem.MenuProgramPath,
                MenuParameter = menuItem.MenuParameter,
                FolderBackground = menuItem.FolderBackground,
                IsAlwaysRunAsAdministrator = menuItem.IsAlwaysRunAsAdministrator,
                FolderDesktop = menuItem.FolderDesktop,
                FolderDirectory = menuItem.FolderDirectory,
                FolderDrive = menuItem.FolderDrive,
                MenuFileMatchRule = menuItem.MenuFileMatchRule,
                MenuFileMatchFormatText = menuItem.MenuFileMatchFormatText,
                MenuIndex = menuItem.MenuIndex,
                UseIcon = menuItem.UseIcon,
                UseThemeIcon = menuItem.UseThemeIcon,
                UseProgramIcon = menuItem.UseProgramIcon,
                MenuIcon = new BitmapImage()
            };
            if (shellMenuItem.UseIcon)
            {
                // 使用应用程序图标
                if (shellMenuItem.UseProgramIcon)
                {
                    try
                    {
                        if (File.Exists(shellMenuItem.MenuProgramPathText) && string.Equals(Path.GetExtension(shellMenuItem.MenuProgramPathText), ".exe"))
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.MenuProgramPathText);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            shellMenuItem.MenuIcon = bitmapImage;
                            memoryStream.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ShellMenuListPage), nameof(EnumShellMenuItem), 1, e);
                    }
                }
                else
                {
                    // 使用主题菜单图标
                    if (shellMenuItem.UseThemeIcon)
                    {
                        // 浅色主题图标
                        if (ActualTheme is ElementTheme.Light && File.Exists(shellMenuItem.LightThemeIconPath))
                        {
                            try
                            {
                                Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.LightThemeIconPath);
                                MemoryStream memoryStream = new();
                                icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                shellMenuItem.MenuIcon = bitmapImage;
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ShellMenuListPage), nameof(EnumShellMenuItem), 2, e);
                            }
                        }
                        // 深色主题图标
                        else if (ActualTheme is ElementTheme.Dark && File.Exists(shellMenuItem.DarkThemeIconPath))
                        {
                            try
                            {
                                Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.DarkThemeIconPath);
                                MemoryStream memoryStream = new();
                                icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                shellMenuItem.MenuIcon = bitmapImage;
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ShellMenuListPage), nameof(EnumShellMenuItem), 3, e);
                            }
                        }
                    }
                    else
                    {
                        // 默认图标
                        if (File.Exists(shellMenuItem.DefaultIconPath))
                        {
                            try
                            {
                                Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.DefaultIconPath);
                                MemoryStream memoryStream = new();
                                icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                                shellMenuItem.MenuIcon = bitmapImage;
                                memoryStream.Dispose();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ShellMenuListPage), nameof(EnumShellMenuItem), 4, e);
                            }
                        }
                    }
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItem subMenuItem in menuItem.SubShellMenuItem)
            {
                if (menuType is MenuType.FirstLevelMenu)
                {
                    shellMenuItem.SubMenuItemCollection.Add(EnumShellMenuItem(subMenuItem, MenuType.SecondLevelMenu));
                }
            }

            return shellMenuItem;
        }

        /// <summary>
        /// 删除指定项
        /// </summary>
        private async Task<ShellMenuItemModel> EnumRemoveItemAsync(ShellMenuItemModel selectedItem, ShellMenuItemModel parentItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            bool isRemoved = false;
            ShellMenuItemModel removedParentItem = null;

            // 递归遍历列表，删除遍历到的当前项
            for (int index = 0; index < shellMenuItemCollection.Count; index++)
            {
                // 删除遍历到的当前项
                if (selectedItem is not null && string.Equals(selectedItem.MenuKey, shellMenuItemCollection[index].MenuKey))
                {
                    shellMenuItemCollection.RemoveAt(index);
                    isRemoved = true;
                    break;
                }
            }

            // 删除当前项后，对列表中已有的项重新排序
            if (isRemoved)
            {
                await Task.Run(() =>
                {
                    for (int index = 0; index < shellMenuItemCollection.Count; index++)
                    {
                        shellMenuItemCollection[index].MenuIndex = index;
                    }

                    // 修改菜单顺序
                    foreach (ShellMenuItemModel shellMenuItem in shellMenuItemCollection)
                    {
                        ShellMenuItem selectedMenuItem = new()
                        {
                            MenuGuid = shellMenuItem.MenuGuid,
                            MenuIndex = shellMenuItem.MenuIndex,
                            MenuTitleText = shellMenuItem.MenuTitleText,
                            UseIcon = shellMenuItem.UseIcon,
                            UseProgramIcon = shellMenuItem.UseProgramIcon,
                            UseThemeIcon = shellMenuItem.UseThemeIcon,
                            DefaultIconPath = shellMenuItem.DefaultIconPath,
                            LightThemeIconPath = shellMenuItem.LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItem.DarkThemeIconPath,
                            MenuProgramPath = shellMenuItem.MenuProgramPathText,
                            MenuParameter = shellMenuItem.MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItem.IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItem.FolderBackground,
                            FolderDesktop = shellMenuItem.FolderDesktop,
                            FolderDirectory = shellMenuItem.FolderDirectory,
                            FolderDrive = shellMenuItem.FolderDrive,
                            MenuFileMatchRule = shellMenuItem.MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItem.MenuFileMatchFormatText,
                        };

                        // 保存菜单新顺序
                        ShellMenuService.SaveShellMenuItem(shellMenuItem.MenuKey, selectedMenuItem);
                    }
                });

                removedParentItem = parentItem;
                return removedParentItem;
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel subMenuItem in shellMenuItemCollection)
            {
                if (await EnumRemoveItemAsync(selectedItem, subMenuItem, subMenuItem.SubMenuItemCollection) is ShellMenuItemModel searchedParentItem)
                {
                    removedParentItem = searchedParentItem;
                }
            }

            return removedParentItem;
        }

        /// <summary>
        /// 枚举并修改选中项
        /// </summary>
        private void EnumModifySelectedItem(ShellMenuItemModel shellMenuItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            // 递归遍历列表，修改遍历到的当前项
            for (int index = 0; index < shellMenuItemCollection.Count; index++)
            {
                // 修改遍历到的当前项
                shellMenuItemCollection[index].IsSelected = false;
                if (shellMenuItem is not null && string.Equals(shellMenuItem.MenuKey, shellMenuItemCollection[index].MenuKey))
                {
                    shellMenuItem.IsSelected = true;
                    IsEditMenuEnabled = true;
                    IsAddMenuEnabled = shellMenuItem.MenuType is MenuType.FirstLevelMenu;

                    if (shellMenuItemCollection.Count is 1)
                    {
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = false;
                    }
                    else
                    {
                        // 第一项，不可向上移动
                        if (shellMenuItem.MenuIndex is 0)
                        {
                            IsMoveUpEnabled = false;
                            IsMoveDownEnabled = true;
                        }
                        // 最后一项，不可向下移动
                        else if (Equals(shellMenuItem.MenuIndex, shellMenuItemCollection.Count - 1))
                        {
                            IsMoveUpEnabled = true;
                            IsMoveDownEnabled = false;
                        }
                        // 不是首项也不是最后一项，可以向上移动，也可以向下移动
                        else
                        {
                            IsMoveUpEnabled = true;
                            IsMoveDownEnabled = true;
                        }
                    }
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel subMenuItem in shellMenuItemCollection)
            {
                EnumModifySelectedItem(shellMenuItem, subMenuItem.SubMenuItemCollection);
            }
        }

        /// <summary>
        /// 当应用显示主题发生修改时，枚举并递归修改带主题的菜单项
        /// </summary>
        private void EnumModifyShellMenuItemTheme(ShellMenuItemModel shellMenuItem)
        {
            // 修改遍历到的当前项
            if (shellMenuItem.UseIcon && !shellMenuItem.UseProgramIcon && shellMenuItem.UseThemeIcon)
            {
                shellMenuItem.MenuIcon = new BitmapImage();

                if (ActualTheme is ElementTheme.Light)
                {
                    if (File.Exists(shellMenuItem.LightThemeIconPath))
                    {
                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.LightThemeIconPath);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            shellMenuItem.MenuIcon = bitmapImage;
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ShellMenuListPage), nameof(EnumModifyShellMenuItemTheme), 1, e);
                        }
                    }
                }
                else if (ActualTheme is ElementTheme.Dark)
                {
                    if (File.Exists(shellMenuItem.DarkThemeIconPath))
                    {
                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(shellMenuItem.DarkThemeIconPath);
                            MemoryStream memoryStream = new();
                            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                            shellMenuItem.MenuIcon = bitmapImage;
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(ShellMenuListPage), nameof(EnumModifyShellMenuItemTheme), 2, e);
                        }
                    }
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel subMenuItem in shellMenuItem.SubMenuItemCollection)
            {
                EnumModifyShellMenuItemTheme(subMenuItem);
            }
        }

        /// <summary>
        /// 枚举并递归获取选中项，向上移动
        /// </summary>
        private async Task EnumMoveUpShellMenuItemAsync(ShellMenuItemModel selectedItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            // 递归遍历列表，修改选中项顺序
            for (int index = 1; index < shellMenuItemCollection.Count; index++)
            {
                if (string.Equals(selectedItem.MenuKey, shellMenuItemCollection[index].MenuKey))
                {
                    shellMenuItemCollection[index].MenuIndex = index - 1;
                    shellMenuItemCollection[index - 1].MenuIndex = index;

                    // 修改已保存的数据
                    await Task.Run(() =>
                    {
                        // 修改顺序后的菜单项信息
                        ShellMenuItem swappedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index - 1].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index - 1].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index - 1].MenuTitleText,
                            UseIcon = shellMenuItemCollection[index - 1].UseIcon,
                            UseProgramIcon = shellMenuItemCollection[index - 1].UseProgramIcon,
                            UseThemeIcon = shellMenuItemCollection[index - 1].UseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index - 1].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index - 1].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index - 1].DarkThemeIconPath,
                            MenuProgramPath = shellMenuItemCollection[index - 1].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index - 1].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index - 1].IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItemCollection[index - 1].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index - 1].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index - 1].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index - 1].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index - 1].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index - 1].MenuFileMatchFormatText,
                        };

                        // 选中项菜单信息
                        ShellMenuItem selectedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index].MenuTitleText,
                            UseIcon = shellMenuItemCollection[index].UseIcon,
                            UseProgramIcon = shellMenuItemCollection[index].UseProgramIcon,
                            UseThemeIcon = shellMenuItemCollection[index].UseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index].DarkThemeIconPath,
                            MenuProgramPath = shellMenuItemCollection[index].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index].IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItemCollection[index].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index].MenuFileMatchFormatText
                        };

                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index - 1].MenuKey, swappedMenuItem);
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index].MenuKey, selectedMenuItem);
                    });

                    // 修改已保存的数据
                    (shellMenuItemCollection[index], shellMenuItemCollection[index - 1]) = (shellMenuItemCollection[index - 1], shellMenuItemCollection[index]);

                    // 上移后是第一项
                    if ((index + 2).Equals(shellMenuItemCollection.Count))
                    {
                        IsMoveUpEnabled = false;
                        IsMoveDownEnabled = true;
                    }
                    break;
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel shellMenuItem in shellMenuItemCollection)
            {
                await EnumMoveUpShellMenuItemAsync(selectedItem, shellMenuItem.SubMenuItemCollection);
            }
        }

        /// <summary>
        /// 枚举并递归获取选中项，向下移动
        /// </summary>
        private async Task EnumMoveDownShellMenuItemAsync(ShellMenuItemModel selectedItem, ObservableCollection<ShellMenuItemModel> shellMenuItemCollection)
        {
            // 递归遍历列表，修改选中项顺序
            for (int index = shellMenuItemCollection.Count - 1; index >= 0; index--)
            {
                if (string.Equals(selectedItem.MenuKey, shellMenuItemCollection[index].MenuKey))
                {
                    shellMenuItemCollection[index].MenuIndex = index + 1;
                    shellMenuItemCollection[index + 1].MenuIndex = index;

                    await Task.Run(() =>
                    {
                        // 保存修改顺序后的菜单项信息
                        ShellMenuItem swappedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index + 1].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index + 1].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index + 1].MenuTitleText,
                            UseIcon = shellMenuItemCollection[index + 1].UseIcon,
                            UseProgramIcon = shellMenuItemCollection[index + 1].UseProgramIcon,
                            UseThemeIcon = shellMenuItemCollection[index + 1].UseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index + 1].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index + 1].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index + 1].DarkThemeIconPath,
                            MenuProgramPath = shellMenuItemCollection[index + 1].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index + 1].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index + 1].IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItemCollection[index + 1].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index + 1].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index + 1].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index + 1].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index + 1].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index + 1].MenuFileMatchFormatText
                        };

                        // 选中项菜单信息
                        ShellMenuItem selectedMenuItem = new()
                        {
                            MenuGuid = shellMenuItemCollection[index].MenuGuid,
                            MenuIndex = shellMenuItemCollection[index].MenuIndex,
                            MenuTitleText = shellMenuItemCollection[index].MenuTitleText,
                            UseIcon = shellMenuItemCollection[index].UseIcon,
                            UseProgramIcon = shellMenuItemCollection[index].UseProgramIcon,
                            UseThemeIcon = shellMenuItemCollection[index].UseThemeIcon,
                            DefaultIconPath = shellMenuItemCollection[index].DefaultIconPath,
                            LightThemeIconPath = shellMenuItemCollection[index].LightThemeIconPath,
                            DarkThemeIconPath = shellMenuItemCollection[index].DarkThemeIconPath,
                            MenuProgramPath = shellMenuItemCollection[index].MenuProgramPathText,
                            MenuParameter = shellMenuItemCollection[index].MenuParameter,
                            IsAlwaysRunAsAdministrator = shellMenuItemCollection[index].IsAlwaysRunAsAdministrator,
                            FolderBackground = shellMenuItemCollection[index].FolderBackground,
                            FolderDesktop = shellMenuItemCollection[index].FolderDesktop,
                            FolderDirectory = shellMenuItemCollection[index].FolderDirectory,
                            FolderDrive = shellMenuItemCollection[index].FolderDrive,
                            MenuFileMatchRule = shellMenuItemCollection[index].MenuFileMatchRule,
                            MenuFileMatchFormatText = shellMenuItemCollection[index].MenuFileMatchFormatText,
                        };

                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index + 1].MenuKey, swappedMenuItem);
                        ShellMenuService.SaveShellMenuItem(shellMenuItemCollection[index].MenuKey, selectedMenuItem);
                    });

                    // 修改已保存的数据
                    (shellMenuItemCollection[index], shellMenuItemCollection[index + 1]) = (shellMenuItemCollection[index + 1], shellMenuItemCollection[index]);

                    // 上移后是最后一项
                    if ((index + 2).Equals(shellMenuItemCollection.Count))
                    {
                        IsMoveUpEnabled = true;
                        IsMoveDownEnabled = false;
                    }
                    break;
                }
            }

            // 递归遍历子项
            foreach (ShellMenuItemModel shellMenuItem in shellMenuItemCollection)
            {
                await EnumMoveDownShellMenuItemAsync(selectedItem, shellMenuItem.SubMenuItemCollection);
            }
        }

        #endregion 第四部分：递归遍历

        /// <summary>
        /// 获取自定义菜单项
        /// </summary>
        private async Task GetShellMenuItemAsync()
        {
            IsLoading = true;
            lastUpdateTime = ShellMenuService.GetLastUpdateTime();
            ShellMenuItemCollection.Clear();

            // 获取所有菜单项信息
            ShellMenuItem rootShellMenuItem = await Task.Run(ShellMenuService.GetShellMenuItem);
            IsAddMenuEnabled = true;
            IsMoveUpEnabled = false;
            IsMoveDownEnabled = false;

            if (rootShellMenuItem is not null)
            {
                ShellMenuItemCollection.Add(EnumShellMenuItem(rootShellMenuItem, MenuType.FirstLevelMenu));

                if (ShellMenuItemCollection.Count is 0)
                {
                    selectedItem = null;
                    IsRemoveMenuEnabled = false;
                    IsEditMenuEnabled = false;
                }
                else
                {
                    ShellMenuItemCollection[0].IsSelected = true;
                    selectedItem = ShellMenuItemCollection[0];
                    IsRemoveMenuEnabled = true;
                    IsEditMenuEnabled = true;
                }
            }
            else
            {
                selectedItem = null;
                IsRemoveMenuEnabled = false;
                IsEditMenuEnabled = false;
            }
            IsLoading = false;

            foreach (ShellMenuItemModel shellMenuItem in ShellMenuItemCollection)
            {
                EnumModifyShellMenuItemTheme(shellMenuItem);
            }
        }
    }
}
