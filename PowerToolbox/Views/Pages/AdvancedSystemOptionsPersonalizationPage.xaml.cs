using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.ComTypes;
using PowerToolbox.WindowsAPI.PInvoke.Shell32;
using PowerToolbox.WindowsAPI.PInvoke.Shlwapi;
using PowerToolbox.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 高级系统选项——个性化页面
    /// </summary>
    internal sealed partial class AdvancedSystemOptionsPersonalizationPage : Page, INotifyPropertyChanged
    {
        private readonly string controlPanelPath = "{5399E694-6CE5-4D6C-8FCE-1D8870FDCBA0}";
        private readonly string homePath = "{F874310E-B6B7-47DC-BC84-B9E6B38F5903}";
        private readonly string libraryPath = "{031E4825-7B94-4DC3-B131-E946B44C8DD5}";
        private readonly string linuxPath = "{B2B4A4D1-2754-4140-A2EB-9A76D9D7CDC6}";
        private readonly string networkPath = "{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}";
        private readonly string photoGalleryPath = "{E88865EA-0E1C-4E20-9AA6-EDCD0212C87C}";
        private readonly string recycleBinPath = "{645FF040-5081-101B-9F08-00AA002F954E}";
        private readonly string thisPCPath = "{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
        private readonly string userFolderPath = "{59031A47-3F72-44A7-89C5-5595FE6B30EE}";
        private readonly string AnimationControlsAndElementsInsideWindowString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("AnimationControlsAndElementsInsideWindow");
        private readonly string BestAppearanceString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("BestAppearance");
        private readonly string BestPerformanceString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("BestPerformance");
        private readonly string CustomString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Custom");
        private readonly string DownloadsString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Downloads");
        private readonly string EmptyString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Empty");
        private readonly string EnablePeekString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("EnablePeek");
        private readonly string FadeinAndOutOrSlideMenuToViewString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("FadeinAndOutOrSlideMenuToView");
        private readonly string FadeinFadeoutOrSlideToolTipInViewString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("FadeinFadeoutOrSlideToolTipInView");
        private readonly string FadeoutMenuAfterClickingString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("FadeoutMenuAfterClicking");
        private readonly string FullString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Full");
        private readonly string HomeString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Home");
        private readonly string RemovableDeviceString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("RemovableDevice");
        private readonly string SaveTaskbarThumbnailPreviewString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("SaveTaskbarThumbnailPreview");
        private readonly string ShowAnimationWhenMaximizingOrMinimizingString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("ShowAnimationWhenMaximizingOrMinimizing");
        private readonly string ShowSemitransparentSelectedRectangleString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("ShowSemitransparentSelectedRectangle");
        private readonly string ShowShadowUnderMousePointerString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("ShowShadowUnderMousePointer");
        private readonly string ShowShadowUnderWindowString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("ShowShadowUnderWindow");
        private readonly string ShowThumbnailString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("ShowThumbnail");
        private readonly string ShowWindowContentsWhileDraggingString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("ShowWindowContentsWhileDragging");
        private readonly string SlideToOpenComboboxString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("SlideToOpenCombobox");
        private readonly string SmoothScreenFontEdgesString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("SmoothScreenFontEdges");
        private readonly string SmoothScrollListboxString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("SmoothScrollListbox");
        private readonly string TaskbarAnimationsString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("TaskbarAnimations");
        private readonly string ThisPCString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("ThisPC");
        private readonly string UserFolderString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("UserFolder");
        private readonly string UseShadowForIconLabelsOnDesktopString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("UseShadowForIconLabelsOnDesktop");
        private readonly string Windows10ClassicMenuString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Windows10ClassicMenu");
        private readonly string Windows11ModernMenuString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Windows11ModernMenu");
        private readonly string Windows10ClassicFileExplorerString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Windows10ClassicFileExplorer");
        private readonly string Windows11ModernFileExplorerString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("Windows11ModernFileExplorer");
        private readonly string WindowsChooseBestSettingsString = ResourceService.AdvancedSystemOptionsPersonalizationResource.GetString("WindowsChooseBestSettings");
        private AdvancedSystemOptionsPage advancedSystemOptionsPage;

        private readonly byte[] layout =
        [
                19, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 32, 0, 0, 0, 16, 0, 1, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 1, 7,
                0, 0, 94, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0
        ];

        private bool _isRebuildingIconCache;

        internal bool IsRebuildingIconCache
        {
            get { return _isRebuildingIconCache; }

            set
            {
                if (!Equals(_isRebuildingIconCache, value))
                {
                    _isRebuildingIconCache = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRebuildingIconCache)));
                }
            }
        }

        private bool _isIconSelected;

        internal bool IsIconSelected
        {
            get { return _isIconSelected; }

            set
            {
                if (!Equals(_isIconSelected, value))
                {
                    _isIconSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsIconSelected)));
                }
            }
        }

        private DesktopIconSettingsModel _selectedIconItem;

        internal DesktopIconSettingsModel SelectedIconItem
        {
            get { return _selectedIconItem; }

            set
            {
                if (!Equals(_selectedIconItem, value))
                {
                    _selectedIconItem = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedIconItem)));
                }
            }
        }

        private ComboBoxItemModel _rightClickMenuStyle;

        internal ComboBoxItemModel RightClickMenuStyle
        {
            get { return _rightClickMenuStyle; }

            set
            {
                if (!Equals(_rightClickMenuStyle, value))
                {
                    _rightClickMenuStyle = value;
                    PropertyChanged?.Invoke(this, new(nameof(RightClickMenuStyle)));
                }
            }
        }

        private ComboBoxItemModel _fileExplorerStyle;

        internal ComboBoxItemModel FileExplorerStyle
        {
            get { return _fileExplorerStyle; }

            set
            {
                if (!Equals(_fileExplorerStyle, value))
                {
                    _fileExplorerStyle = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileExplorerStyle)));
                }
            }
        }

        private bool _isSyncProviderNotificationsEnabled;

        internal bool IsSyncProviderNotificationsEnabled
        {
            get { return _isSyncProviderNotificationsEnabled; }

            set
            {
                if (!Equals(_isSyncProviderNotificationsEnabled, value))
                {
                    _isSyncProviderNotificationsEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSyncProviderNotificationsEnabled)));
                }
            }
        }

        private ComboBoxItemModel _fileExplorerHomePosition;

        internal ComboBoxItemModel FileExplorerHomePosition
        {
            get { return _fileExplorerHomePosition; }

            set
            {
                if (!Equals(_fileExplorerHomePosition, value))
                {
                    _fileExplorerHomePosition = value;
                    PropertyChanged?.Invoke(this, new(nameof(FileExplorerHomePosition)));
                }
            }
        }

        private bool _isShortcutWithoutShortcutTextEnabled;

        internal bool IsShortcutWithoutShortcutTextEnabled
        {
            get { return _isShortcutWithoutShortcutTextEnabled; }

            set
            {
                if (!Equals(_isShortcutWithoutShortcutTextEnabled, value))
                {
                    _isShortcutWithoutShortcutTextEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsShortcutWithoutShortcutTextEnabled)));
                }
            }
        }

        private ComboBoxItemModel _visualEffectsPlan;

        internal ComboBoxItemModel VisualEffectsPlan
        {
            get { return _visualEffectsPlan; }

            set
            {
                if (!Equals(_visualEffectsPlan, value))
                {
                    _visualEffectsPlan = value;
                    PropertyChanged?.Invoke(this, new(nameof(VisualEffectsPlan)));
                }
            }
        }

        private bool _isUpdatingVisualEffects;

        internal bool IsUpdatingVisualEffects
        {
            get { return _isUpdatingVisualEffects; }

            set
            {
                if (!Equals(_isUpdatingVisualEffects, value))
                {
                    _isUpdatingVisualEffects = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsUpdatingVisualEffects)));
                }
            }
        }

        private WinRTObservableCollection<DesktopIconSettingsModel> DesktopIconSettingsCollection { get; } = [];

        private WinRTObservableCollection<DesktopIconDisplayModel> DesktopIconDisplayCollection { get; } = [];

        private WinRTObservableCollection<NavigationPaneIconDisplayModel> NavigationPaneIconDisplayCollection { get; } = [];

        private List<ComboBoxItemModel> RightClickMenuStyleList { get; } = [];

        private List<ComboBoxItemModel> FileExplorerStyleList { get; } = [];

        private List<ComboBoxItemModel> FileExplorerHomePositionList { get; } = [];

        private List<ComboBoxItemModel> VisualEffectsPlanList { get; } = [];

        private List<VisualEffectsModel> VisualEffectsList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        internal AdvancedSystemOptionsPersonalizationPage()
        {
            InitializeComponent();
            RightClickMenuStyleList.Add(new() { DisplayMember = Windows11ModernMenuString, SelectedValue = "Windows11ModernMenu" });
            RightClickMenuStyleList.Add(new() { DisplayMember = Windows10ClassicMenuString, SelectedValue = "Windows10ClassicMenu" });
            FileExplorerStyleList.Add(new() { DisplayMember = Windows11ModernFileExplorerString, SelectedValue = "Windows11ModernFileExplorer" });
            FileExplorerStyleList.Add(new() { DisplayMember = Windows10ClassicFileExplorerString, SelectedValue = "Windows10ClassicFileExplorer" });
            FileExplorerHomePositionList.Add(new() { DisplayMember = ThisPCString, SelectedValue = "ThisPC" });
            FileExplorerHomePositionList.Add(new() { DisplayMember = HomeString, SelectedValue = "Home" });
            FileExplorerHomePositionList.Add(new() { DisplayMember = DownloadsString, SelectedValue = "Downloads" });
            VisualEffectsPlanList.Add(new() { DisplayMember = WindowsChooseBestSettingsString, SelectedValue = "WindowsChooseBestSettings" });
            VisualEffectsPlanList.Add(new() { DisplayMember = BestAppearanceString, SelectedValue = "BestAppearance" });
            VisualEffectsPlanList.Add(new() { DisplayMember = BestPerformanceString, SelectedValue = "BestPerformance" });
            VisualEffectsPlanList.Add(new() { DisplayMember = CustomString, SelectedValue = "Custom" });
            VisualEffectsList.Add(new() { Name = SaveTaskbarThumbnailPreviewString, IsVisualEnabled = false, VisualTag = "SaveTaskbarThumbnailPreview" });
            VisualEffectsList.Add(new() { Name = AnimationControlsAndElementsInsideWindowString, IsVisualEnabled = false, VisualTag = "AnimationControlsAndElementsInsideWindow" });
            VisualEffectsList.Add(new() { Name = FadeinAndOutOrSlideMenuToViewString, IsVisualEnabled = false, VisualTag = "FadeinAndOutOrSlideMenuToView" });
            VisualEffectsList.Add(new() { Name = SlideToOpenComboboxString, IsVisualEnabled = false, VisualTag = "SlideToOpenCombobox" });
            VisualEffectsList.Add(new() { Name = SmoothScrollListboxString, IsVisualEnabled = false, VisualTag = "SmoothScrollListbox" });
            VisualEffectsList.Add(new() { Name = SmoothScreenFontEdgesString, IsVisualEnabled = false, VisualTag = "SmoothScreenFontEdges" });
            VisualEffectsList.Add(new() { Name = EnablePeekString, IsVisualEnabled = false, VisualTag = "EnablePeek" });
            VisualEffectsList.Add(new() { Name = TaskbarAnimationsString, IsVisualEnabled = false, VisualTag = "TaskbarAnimations" });
            VisualEffectsList.Add(new() { Name = ShowWindowContentsWhileDraggingString, IsVisualEnabled = false, VisualTag = "ShowWindowContentsWhileDragging" });
            VisualEffectsList.Add(new() { Name = ShowThumbnailString, IsVisualEnabled = false, VisualTag = "ShowThumbnail" });
            VisualEffectsList.Add(new() { Name = ShowSemitransparentSelectedRectangleString, IsVisualEnabled = false, VisualTag = "ShowSemitransparentSelectedRectangle" });
            VisualEffectsList.Add(new() { Name = ShowShadowUnderWindowString, IsVisualEnabled = false, VisualTag = "ShowShadowUnderWindow" });
            VisualEffectsList.Add(new() { Name = FadeoutMenuAfterClickingString, IsVisualEnabled = false, VisualTag = "FadeoutMenuAfterClicking" });
            VisualEffectsList.Add(new() { Name = FadeinFadeoutOrSlideToolTipInViewString, IsVisualEnabled = false, VisualTag = "FadeinFadeoutOrSlideToolTipInView" });
            VisualEffectsList.Add(new() { Name = ShowShadowUnderMousePointerString, IsVisualEnabled = false, VisualTag = "ShowShadowUnderMousePointer" });
            VisualEffectsList.Add(new() { Name = UseShadowForIconLabelsOnDesktopString, IsVisualEnabled = false, VisualTag = "UseShadowForIconLabelsOnDesktop" });
            VisualEffectsList.Add(new() { Name = ShowAnimationWhenMaximizingOrMinimizingString, IsVisualEnabled = false, VisualTag = "ShowAnimationWhenMaximizingOrMinimizing" });
        }

        #region 第一部分：重写父类事件

        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is AdvancedSystemOptionsPage targetPage && !Equals(advancedSystemOptionsPage, targetPage))
            {
                advancedSystemOptionsPage = targetPage;
            }

            if (RuntimeHelper.IsElevated)
            {
                string controlPanelIconPath = string.Format("::{0}", controlPanelPath);
                string homeIconPath = string.Format("::{0}", homePath);
                string libraryIconPath = string.Format("::{0}", libraryPath);
                string linuxIconPath = string.Format("::{0}", linuxPath);
                string networkIconPath = string.Format("::{0}", networkPath);
                string photoGalleryIconPath = string.Format("::{0}", photoGalleryPath);
                string recycleBinIconPath = string.Format("::{0}", recycleBinPath);
                string thisPCIconPath = string.Format("::{0}", thisPCPath);
                string userFolderIconPath = string.Format("::{0}", userFolderPath);

                // 图标在注册表中存储的键
                string networkIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", networkPath);
                string recycleBinIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", recycleBinPath);
                string thisPCIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", thisPCPath);
                string userFolderIconRegistryKeyPath = string.Format(@"Software\Microsoft\Windows\CurrentVersion\Explorer\CLSID\{0}\DefaultIcon", userFolderPath);

                // 图标在注册表中存储的值
                string networkIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, networkIconRegistryKeyPath, string.Empty);
                string recycleBinEmptyIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, recycleBinIconRegistryKeyPath, "empty");
                string recycleBinFullIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, recycleBinIconRegistryKeyPath, "full");
                string thisPCIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, thisPCIconRegistryKeyPath, string.Empty);
                string userFolderIconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, userFolderIconRegistryKeyPath, string.Empty);

                // 图标显示名称
                string controlPanelDisplayName = await GetShellIconDisplayNameAsync(controlPanelIconPath);
                string homeDisplayName = await GetShellIconDisplayNameAsync(homeIconPath);
                string libraryDisplayName = await GetShellIconDisplayNameAsync(libraryIconPath);
                string linuxDisplayName = await GetShellIconDisplayNameAsync(linuxIconPath);
                string networkDisplayName = await GetShellIconDisplayNameAsync(networkIconPath);
                string photoGalleryDisplayName = await GetShellIconDisplayNameAsync(photoGalleryIconPath);
                string recycleBinDisplayName = await GetShellIconDisplayNameAsync(recycleBinIconPath);
                string thisPCDisplayName = await GetShellIconDisplayNameAsync(thisPCIconPath);
                string userFolderDisplayName = await GetShellIconDisplayNameAsync(userFolderIconPath);

                // 图标的位置和索引
                (string networkIconLocationPath, int networkIconIndex) = await GetShellIconLocationAsync(networkIconRegistryValuePath);
                (string recycleBinFullIconLocationPath, int recycleBinFullIconIndex) = await GetShellIconLocationAsync(recycleBinFullIconRegistryValuePath);
                (string recycleBinEmptyIconLocationPath, int recycleBinEmptyIconIndex) = await GetShellIconLocationAsync(recycleBinEmptyIconRegistryValuePath);
                (string thisPCIconLocationPath, int thisPCIconIndex) = await GetShellIconLocationAsync(thisPCIconRegistryValuePath);
                (string userFolderIconLocationPath, int userFolderIconIndex) = await GetShellIconLocationAsync(userFolderIconRegistryValuePath);

                MemoryStream networkIconMemoryStream = await GetShellIconAsync(networkIconLocationPath, networkIconIndex);
                MemoryStream recycleBinFullMemoryStream = await GetShellIconAsync(recycleBinFullIconLocationPath, recycleBinFullIconIndex);
                MemoryStream recycleBinEmptyMemoryStream = await GetShellIconAsync(recycleBinEmptyIconLocationPath, recycleBinEmptyIconIndex);
                MemoryStream thisPCIconMemoryStream = await GetShellIconAsync(thisPCIconLocationPath, thisPCIconIndex);
                MemoryStream userFolderIconMemoryStream = await GetShellIconAsync(userFolderIconLocationPath, userFolderIconIndex);

                DesktopIconSettingsCollection.Clear();
                if (thisPCIconMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(thisPCIconMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new()
                        {
                            IconTag = "ThisPC",
                            IconRegistryKeyPath = thisPCIconRegistryKeyPath,
                            IconLocationPath = thisPCIconLocationPath,
                            IconIndex = thisPCIconIndex,
                            DisplayName = thisPCDisplayName,
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 1, e);
                    }
                }
                if (userFolderIconMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(userFolderIconMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new()
                        {
                            IconTag = "UserFolder",
                            IconRegistryKeyPath = userFolderIconRegistryKeyPath,
                            IconLocationPath = userFolderIconLocationPath,
                            IconIndex = userFolderIconIndex,
                            DisplayName = userFolderDisplayName,
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 2, e);
                    }
                }
                if (networkIconMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(networkIconMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new()
                        {
                            IconTag = "Network",
                            IconRegistryKeyPath = networkIconRegistryKeyPath,
                            IconLocationPath = networkIconLocationPath,
                            IconIndex = networkIconIndex,
                            DisplayName = networkDisplayName,
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 3, e);
                    }
                }
                if (recycleBinFullMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(recycleBinFullMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new()
                        {
                            IconTag = "RecycleBinFull",
                            IconRegistryKeyPath = recycleBinIconRegistryKeyPath,
                            IconLocationPath = recycleBinFullIconLocationPath,
                            IconIndex = recycleBinFullIconIndex,
                            DisplayName = string.Format("{0}{1}", recycleBinDisplayName, FullString),
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 5, e);
                    }
                }
                if (recycleBinEmptyMemoryStream is not null)
                {
                    try
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(recycleBinEmptyMemoryStream.AsRandomAccessStream());
                        DesktopIconSettingsCollection.Add(new()
                        {
                            IconTag = "RecycleBinEmpty",
                            IconRegistryKeyPath = recycleBinIconRegistryKeyPath,
                            IconLocationPath = recycleBinEmptyIconLocationPath,
                            IconIndex = recycleBinEmptyIconIndex,
                            DisplayName = string.Format("{0}{1}", recycleBinDisplayName, EmptyString),
                            IconImage = bitmapImage
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnNavigatedTo), 5, e);
                    }
                }

                bool controlPanelDesktopIconVisible = GetDesktopIconVisibility(controlPanelPath, "ControlPanel");
                bool libraryDesktopIconVisible = GetDesktopIconVisibility(libraryPath, "Library");
                bool networkDesktopIconVisible = GetDesktopIconVisibility(networkPath, "Network");
                bool recycleBinDesktopIconVisible = GetDesktopIconVisibility(recycleBinPath, "RecycleBin");
                bool thisPCDesktopIconVisible = GetDesktopIconVisibility(thisPCPath, "ThisPC");
                bool userFolderDesktopIconVisible = GetDesktopIconVisibility(userFolderPath, "UserFolder");

                DesktopIconDisplayCollection.Clear();
                DesktopIconDisplayCollection.Add(new()
                {
                    DisplayName = thisPCDisplayName,
                    IconTag = "ThisPC",
                    IsIconVisible = thisPCDesktopIconVisible
                });
                DesktopIconDisplayCollection.Add(new()
                {
                    DisplayName = recycleBinDisplayName,
                    IconTag = "RecycleBin",
                    IsIconVisible = recycleBinDesktopIconVisible
                });
                DesktopIconDisplayCollection.Add(new()
                {
                    DisplayName = UserFolderString,
                    IconTag = "UserFolder",
                    IsIconVisible = userFolderDesktopIconVisible
                });
                DesktopIconDisplayCollection.Add(new()
                {
                    DisplayName = controlPanelDisplayName,
                    IconTag = "ControlPanel",
                    IsIconVisible = controlPanelDesktopIconVisible
                });
                DesktopIconDisplayCollection.Add(new()
                {
                    DisplayName = networkDisplayName,
                    IconTag = "Network",
                    IsIconVisible = networkDesktopIconVisible
                });
                DesktopIconDisplayCollection.Add(new()
                {
                    DisplayName = libraryDisplayName,
                    IconTag = "Library",
                    IsIconVisible = libraryDesktopIconVisible
                });

                IsShortcutWithoutShortcutTextEnabled = await Task.Run(() =>
                {
                    byte[] linkValue = RegistryHelper.ReadRegistryKey<byte[]>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer", "link");
                    if (linkValue is null)
                    {
                        return false;
                    }
                    else
                    {
                        if (linkValue.Length < 4)
                        {
                            return false;
                        }
                        else
                        {
                            return linkValue[0] is 0 && linkValue[1] is 0 && linkValue[2] is 0 && linkValue[3] is 0;
                        }
                    }
                });

                if (RuntimeHelper.IsWindows11)
                {
                    bool isClassicRightClickMenuExisted = await Task.Run(() =>
                    {
                        return RegistryHelper.IsRegistryKeyExisted(Registry.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
                    });
                    RightClickMenuStyle = isClassicRightClickMenuExisted ? RightClickMenuStyleList.Find(item => Equals(item.SelectedValue, "Windows10ClassicMenu")) : RightClickMenuStyleList.Find(item => Equals(item.SelectedValue, "Windows11ModernMenu"));
                    bool isClassicFileExplorerExisted = await Task.Run(() =>
                    {
                        string itemsViewAdapter = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}", string.Empty);
                        string fileExplorerDllPath1 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}\InProcServer32", string.Empty);
                        string apartment1 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}\InProcServer32", "ThreadingModel");
                        bool flag1 = string.Equals(itemsViewAdapter, "CLSID_ItemsViewAdapter") && string.Equals(fileExplorerDllPath1, @"C:\Windows\System32\Windows.UI.FileExplorer.dll_") && string.Equals(apartment1, "Apartment");
                        string fileExplorerXamlIslandViewAdapter = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}", string.Empty);
                        string fileExplorerDllPath2 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}\InProcServer32", string.Empty);
                        string apartment2 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}\InProcServer32", "ThreadingModel");
                        bool flag2 = string.Equals(fileExplorerXamlIslandViewAdapter, "File Explorer Xaml Island View Adapter") && string.Equals(fileExplorerDllPath2, @"C:\Windows\System32\Windows.UI.FileExplorer.dll_") && string.Equals(apartment2, "Apartment");
                        byte[] tBar7Layout = RegistryHelper.ReadRegistryKey<byte[]>(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Toolbar\ShellBrowser", "ITBar7Layout");
                        return (tBar7Layout?.SequenceEqual(layout) ?? false) && flag1 && flag2;
                    });
                    FileExplorerStyle = isClassicFileExplorerExisted ? FileExplorerStyleList.Find(item => Equals(item.SelectedValue, "Windows10ClassicFileExplorer")) : FileExplorerStyleList.Find(item => Equals(item.SelectedValue, "Windows11ModernFileExplorer"));
                }

                bool homeNavigationPaneIconVisible = GetNavigationPaneIconVisibility(homePath, "Home");
                bool libraryNavigationPaneIconVisible = GetNavigationPaneIconVisibility(libraryPath, "Library");
                bool linuxNavigationPaneIconVisible = GetNavigationPaneIconVisibility(linuxPath, "Linux");
                bool photoGalleryNavigationPaneIconVisible = GetNavigationPaneIconVisibility(photoGalleryPath, "PhotoGallery");
                bool recycleBinNavigationPaneIconVisible = GetNavigationPaneIconVisibility(recycleBinPath, "RecycleBin");
                bool removableDeviceNavigationPaneIconVisible = GetNavigationPaneIconVisibility(null, "RemovableDevice");

                NavigationPaneIconDisplayCollection.Clear();
                NavigationPaneIconDisplayCollection.Add(new()
                {
                    DisplayName = homeDisplayName,
                    IconTag = "Home",
                    IsIconVisible = homeNavigationPaneIconVisible
                });
                NavigationPaneIconDisplayCollection.Add(new()
                {
                    DisplayName = photoGalleryDisplayName,
                    IconTag = "PhotoGallery",
                    IsIconVisible = photoGalleryNavigationPaneIconVisible
                });
                NavigationPaneIconDisplayCollection.Add(new()
                {
                    DisplayName = recycleBinDisplayName,
                    IconTag = "RecycleBin",
                    IsIconVisible = recycleBinNavigationPaneIconVisible
                });
                NavigationPaneIconDisplayCollection.Add(new()
                {
                    DisplayName = linuxDisplayName,
                    IconTag = "Linux",
                    IsIconVisible = linuxNavigationPaneIconVisible
                });
                NavigationPaneIconDisplayCollection.Add(new()
                {
                    DisplayName = libraryDisplayName,
                    IconTag = "Library",
                    IsIconVisible = libraryNavigationPaneIconVisible
                });
                NavigationPaneIconDisplayCollection.Add(new()
                {
                    DisplayName = RemovableDeviceString,
                    IconTag = "RemovableDevice",
                    IsIconVisible = removableDeviceNavigationPaneIconVisible
                });

                IsSyncProviderNotificationsEnabled = await Task.Run(() =>
                {
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSyncProviderNotifications");
                });

                FileExplorerHomePosition = await Task.Run(() =>
                {
                    ComboBoxItemModel fileExplorerTo = FileExplorerHomePositionList[1];
                    int launchTo = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo");
                    switch (launchTo)
                    {
                        case 1:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[0];
                                break;
                            }
                        case 2:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[1];
                                break;
                            }
                        case 3:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[2];
                                break;
                            }
                        default:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[1];
                                break;
                            }
                    }
                    return fileExplorerTo;
                });

                VisualEffects visualEffects = await Task.Run(() =>
                {
                    VisualEffects visualEffects = new()
                    {
                        VisualEffectsPlan = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting"),
                        SaveTaskbarThumbnailPreview = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "AlwaysHibernateThumbnails"),
                        AnimationControlsAndElementsInsideWindow = GetSystemParametersInfoBoolValue(SPI.SPI_GETCLIENTAREAANIMATION),
                        FadeinAndOutOrSlideMenuToView = GetSystemParametersInfoBoolValue(SPI.SPI_GETMENUANIMATION),
                        SlideToOpenCombobox = GetSystemParametersInfoBoolValue(SPI.SPI_GETCOMBOBOXANIMATION),
                        SmoothScrollListbox = GetSystemParametersInfoBoolValue(SPI.SPI_GETLISTBOXSMOOTHSCROLLING),
                        SmoothScreenFontEdges = GetSystemParametersInfoBoolValue(SPI.SPI_GETFONTSMOOTHING),
                        EnablePeek = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "EnableAeroPeek"),
                        TaskbarAnimations = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAnimations"),
                        ShowWindowContentsWhileDragging = GetSystemParametersInfoBoolValue(SPI.SPI_GETDRAGFULLWINDOWS),
                        ShowThumbnail = !RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "IconsOnly"),
                        ShowSemitransparentSelectedRectangle = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewAlphaSelect"),
                        ShowShadowUnderWindow = GetSystemParametersInfoBoolValue(SPI.SPI_GETDROPSHADOW),
                        FadeoutMenuAfterClicking = GetSystemParametersInfoBoolValue(SPI.SPI_GETSELECTIONFADE),
                        FadeinFadeoutOrSlideToolTipInView = GetSystemParametersInfoBoolValue(SPI.SPI_GETTOOLTIPANIMATION),
                        ShowShadowUnderMousePointer = GetSystemParametersInfoBoolValue(SPI.SPI_GETCURSORSHADOW),
                        UseShadowForIconLabelsOnDesktop = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewShadow"),
                        ShowAnimationWhenMaximizingOrMinimizing = GetSystemParametersInfoAnimationInfoValue(SPI.SPI_GETANIMATION)
                    };
                    return visualEffects;
                });

                VisualEffectsPlan = visualEffects.VisualEffectsPlan >= 0 && visualEffects.VisualEffectsPlan <= 3 ? VisualEffectsPlanList[visualEffects.VisualEffectsPlan] : null;
                if (VisualEffectsPlan is not null)
                {
                    VisualEffectsList[0].IsVisualEnabled = visualEffects.SaveTaskbarThumbnailPreview;
                    VisualEffectsList[1].IsVisualEnabled = visualEffects.AnimationControlsAndElementsInsideWindow;
                    VisualEffectsList[2].IsVisualEnabled = visualEffects.FadeinAndOutOrSlideMenuToView;
                    VisualEffectsList[3].IsVisualEnabled = visualEffects.SlideToOpenCombobox;
                    VisualEffectsList[4].IsVisualEnabled = visualEffects.SmoothScrollListbox;
                    VisualEffectsList[5].IsVisualEnabled = visualEffects.SmoothScreenFontEdges;
                    VisualEffectsList[6].IsVisualEnabled = visualEffects.EnablePeek;
                    VisualEffectsList[7].IsVisualEnabled = visualEffects.TaskbarAnimations;
                    VisualEffectsList[8].IsVisualEnabled = visualEffects.ShowWindowContentsWhileDragging;
                    VisualEffectsList[9].IsVisualEnabled = visualEffects.ShowThumbnail;
                    VisualEffectsList[10].IsVisualEnabled = visualEffects.ShowSemitransparentSelectedRectangle;
                    VisualEffectsList[11].IsVisualEnabled = visualEffects.ShowShadowUnderWindow;
                    VisualEffectsList[12].IsVisualEnabled = visualEffects.FadeoutMenuAfterClicking;
                    VisualEffectsList[13].IsVisualEnabled = visualEffects.FadeinFadeoutOrSlideToolTipInView;
                    VisualEffectsList[14].IsVisualEnabled = visualEffects.ShowShadowUnderMousePointer;
                    VisualEffectsList[15].IsVisualEnabled = visualEffects.UseShadowForIconLabelsOnDesktop;
                    VisualEffectsList[16].IsVisualEnabled = visualEffects.ShowAnimationWhenMaximizingOrMinimizing;
                }
                else
                {
                    foreach (VisualEffectsModel visualEffectsItem in VisualEffectsList)
                    {
                        visualEffectsItem.IsVisualEnabled = false;
                    }
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 修改桌面图标显示状态
        /// </summary>
        private async void OnDesktopIconDisplayExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DesktopIconDisplayModel desktopIconDisplay)
            {
                desktopIconDisplay.IsIconVisible = !desktopIconDisplay.IsIconVisible;
                desktopIconDisplay.IsIconVisible = await Task.Run(() =>
                {
                    bool isIconVisible = false;
                    switch (desktopIconDisplay.IconTag)
                    {
                        case "ControlPanel":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", controlPanelPath, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(controlPanelPath, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "Library":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", libraryPath, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(libraryPath, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "Network":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", networkPath, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(networkPath, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "RecycleBin":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", recycleBinPath, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(recycleBinPath, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "ThisPC":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", thisPCPath, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(thisPCPath, desktopIconDisplay.IconTag);
                                break;
                            }
                        case "UserFolder":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", userFolderPath, desktopIconDisplay.IsIconVisible ? 0 : 1);
                                isIconVisible = GetDesktopIconVisibility(userFolderPath, desktopIconDisplay.IconTag);
                                break;
                            }
                    }
                    Shell32Library.SHChangeNotify(SHCNE.SHCNE_ASSOCCHANGED, SHCNF.SHCNF_IDLIST | SHCNF.SHCNF_FLUSH, 0, 0);
                    return isIconVisible;
                });
            }
        }

        /// <summary>
        /// 修改导航窗格图标显示状态
        /// </summary>
        private async void OnNavigationPaneIconExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is NavigationPaneIconDisplayModel navigationPaneIconDisplay)
            {
                navigationPaneIconDisplay.IsIconVisible = !navigationPaneIconDisplay.IsIconVisible;
                navigationPaneIconDisplay.IsIconVisible = await Task.Run(() =>
                {
                    bool isIconVisible = false;
                    switch (navigationPaneIconDisplay.IconTag)
                    {
                        case "Home":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", homePath), "System.IsPinnedToNameSpaceTree", navigationPaneIconDisplay.IsIconVisible);
                                isIconVisible = GetNavigationPaneIconVisibility(homePath, navigationPaneIconDisplay.IconTag);
                                break;
                            }
                        case "Library":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.ClassesRoot, string.Format(@"CLSID\{0}", libraryPath), "System.IsPinnedToNameSpaceTree", navigationPaneIconDisplay.IsIconVisible);
                                isIconVisible = GetNavigationPaneIconVisibility(libraryPath, navigationPaneIconDisplay.IconTag);
                                break;
                            }
                        case "Linux":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", linuxPath), string.Empty, "Linux");
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", linuxPath), "System.IsPinnedToNameSpaceTree", navigationPaneIconDisplay.IsIconVisible);
                                isIconVisible = GetNavigationPaneIconVisibility(linuxPath, navigationPaneIconDisplay.IconTag);
                                break;
                            }
                        case "PhotoGallery":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", photoGalleryPath), "System.IsPinnedToNameSpaceTree", navigationPaneIconDisplay.IsIconVisible);
                                isIconVisible = GetNavigationPaneIconVisibility(photoGalleryPath, navigationPaneIconDisplay.IconTag);
                                break;
                            }
                        case "RecycleBin":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", recycleBinPath), "System.IsPinnedToNameSpaceTree", navigationPaneIconDisplay.IsIconVisible);
                                isIconVisible = GetNavigationPaneIconVisibility(recycleBinPath, navigationPaneIconDisplay.IconTag);
                                break;
                            }
                        case "RemovableDevice":
                            {
                                if (navigationPaneIconDisplay.IsIconVisible)
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace\DelegateFolders\{F5FB2C77-0E2F-4A16-A381-3E560C68BC83}", string.Empty, "Removable Drives");
                                }
                                else
                                {
                                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace\DelegateFolders\{F5FB2C77-0E2F-4A16-A381-3E560C68BC83}");
                                }
                                isIconVisible = GetNavigationPaneIconVisibility(null, "RemovableDevice");
                                break;
                            }
                    }
                    return isIconVisible;
                });
                if (advancedSystemOptionsPage is not null)
                {
                    advancedSystemOptionsPage.IsAdvancedSettingsInfoWarning = true;
                    advancedSystemOptionsPage.IsRestartExplorerVisible = true;
                }
            }
        }

        /// <summary>
        /// 修改视觉效果选项启用状态
        /// </summary>
        private void OnVisualEffectsExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is VisualEffectsModel visualEffects)
            {
                visualEffects.IsVisualEnabled = !visualEffects.IsVisualEnabled;
                VisualEffectsPlan = VisualEffectsPlanList[3];
            }
        }

        #endregion 第二部分：ExecuteCommand 命令调用时挂载的事件

        #region 第三部分：高级系统选项——个性化页面——挂载的事件

        /// <summary>
        /// 重建图标缓存
        /// </summary>
        private async void OnRebuildIconCacheClicked(object sender, RoutedEventArgs args)
        {
            if (!IsRebuildingIconCache)
            {
                IsRebuildingIconCache = true;
                await Task.Run(async () =>
                {
                    try
                    {
                        Process taskKillProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = "/IM explorer.exe /F",
                            Verb = "open",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        });
                        taskKillProcess.WaitForExit();
                        taskKillProcess.Dispose();
                        while (Process.GetProcessesByName("explorer").FirstOrDefault() is not null)
                        {
                            await Task.Delay(1000);
                        }

                        string iconCacheDbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IconCache.db");
                        if (File.Exists(iconCacheDbFile))
                        {
                            File.Delete(iconCacheDbFile);
                        }
                        string explorerFolder = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\Explorer"));
                        foreach (FileInfo fileInfo in from file in new DirectoryInfo(explorerFolder).EnumerateFiles() where file.Name.Contains("iconcache") || file.Name.Contains("thumbcache") select file)
                        {
                            fileInfo.Delete();
                        }
                    }
                    catch (Win32Exception e)
                    {
                        LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnRebuildIconCacheClicked), 1, e);
                    }
                    finally
                    {
                        try
                        {
                            Process explorerProcess = Process.Start(new ProcessStartInfo
                            {
                                FileName = "explorer.exe",
                                Verb = "open",
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                            });
                            explorerProcess.Dispose();
                        }
                        catch (Win32Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnRebuildIconCacheClicked), 2, e);
                        }
                    }
                });
                IsRebuildingIconCache = false;
            }
        }

        /// <summary>
        /// 修改桌面图标
        /// </summary>
        private async void OnChangeDesktopIconClicked(object sender, RoutedEventArgs args)
        {
            if (DesktopIconSettingsGridView.SelectedItem is DesktopIconSettingsModel desktopIconSettings)
            {
                StringBuilder desktopIconStringBuilder = new(desktopIconSettings.IconLocationPath, 260);
                int desktopIconIndex = desktopIconSettings.IconIndex;
                string iconRegistryValuePath = string.Empty;
                if (Shell32Library.PickIconDlg((nint)MainWindow.Current.AppWindow.Id.Value, desktopIconStringBuilder, desktopIconStringBuilder.Capacity, ref desktopIconIndex))
                {
                    await Task.Run(() =>
                    {
                        switch (desktopIconSettings.IconTag)
                        {
                            case "ThisPC":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                    break;
                                }
                            case "UserFolder":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                    break;
                                }
                            case "Network":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                    break;
                                }
                            case "RecycleBinFull":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full", string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full");
                                    break;
                                }
                            case "RecycleBinEmpty":
                                {
                                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty", string.Format("{0},{1}", desktopIconStringBuilder.ToString(), desktopIconIndex));
                                    iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty");
                                    break;
                                }
                        }
                        Shell32Library.SHChangeNotify(SHCNE.SHCNE_ASSOCCHANGED, SHCNF.SHCNF_IDLIST | SHCNF.SHCNF_FLUSH, 0, 0);
                    });

                    if (!string.IsNullOrEmpty(iconRegistryValuePath))
                    {
                        (string iconLocationPath, int iconIndex) = await GetShellIconLocationAsync(iconRegistryValuePath);
                        MemoryStream iconMemoryStream = await GetShellIconAsync(iconLocationPath, iconIndex);
                        if (iconMemoryStream is not null)
                        {
                            try
                            {
                                BitmapImage bitmapImage = new();
                                bitmapImage.SetSource(iconMemoryStream.AsRandomAccessStream());
                                desktopIconSettings.IconImage = bitmapImage;
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnChangeDesktopIconClicked), 1, e);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 还原默认图标
        /// </summary>
        private async void OnRestoreDefualtIconClicked(object sender, RoutedEventArgs args)
        {
            if (DesktopIconSettingsGridView.SelectedItem is DesktopIconSettingsModel desktopIconSettings)
            {
                string iconRegistryValuePath = string.Empty;
                await Task.Run(() =>
                {
                    switch (desktopIconSettings.IconTag)
                    {
                        case "ThisPC":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, @"%SystemRoot%\System32\imageres.dll,-109");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                break;
                            }
                        case "UserFolder":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, @"%SystemRoot%\System32\imageres.dll,-123");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                break;
                            }
                        case "Network":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty, @"%SystemRoot%\System32\imageres.dll,-25");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, string.Empty);
                                break;
                            }
                        case "RecycleBinFull":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full", @"%SystemRoot%\System32\imageres.dll,-54");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "full");
                                break;
                            }
                        case "RecycleBinEmpty":
                            {
                                RegistryHelper.SaveRegistryKey(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty", @"%SystemRoot%\System32\imageres.dll,-55");
                                iconRegistryValuePath = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, desktopIconSettings.IconRegistryKeyPath, "empty");
                                break;
                            }
                    }
                    Shell32Library.SHChangeNotify(SHCNE.SHCNE_ASSOCCHANGED, SHCNF.SHCNF_IDLIST | SHCNF.SHCNF_FLUSH, 0, 0);
                });

                if (!string.IsNullOrEmpty(iconRegistryValuePath))
                {
                    (string iconLocationPath, int iconIndex) = await GetShellIconLocationAsync(iconRegistryValuePath);
                    MemoryStream iconMemoryStream = await GetShellIconAsync(iconLocationPath, iconIndex);
                    if (iconMemoryStream is not null)
                    {
                        try
                        {
                            BitmapImage bitmapImage = new();
                            bitmapImage.SetSource(iconMemoryStream.AsRandomAccessStream());
                            desktopIconSettings.IconImage = bitmapImage;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(OnRestoreDefualtIconClicked), 1, e);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 图标选中项发生更改时触发的事件
        /// </summary>
        private void OnIconSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is GridView gridView && !Equals(SelectedIconItem, gridView.SelectedItem))
            {
                SelectedIconItem = gridView.SelectedItem is DesktopIconSettingsModel desktopIconSettingsItem ? desktopIconSettingsItem : null;
            }
            IsIconSelected = SelectedIconItem is not null;
        }

        /// <summary>
        /// 显示桌面快捷键头
        /// </summary>
        private async void OnShowDesktopShortcutArrowClicked(object sender, RoutedEventArgs args)
        {
            await Task.Run(async () =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    RegistryHelper.RemoveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons", "29");
                }
            });
            if (advancedSystemOptionsPage is not null)
            {
                advancedSystemOptionsPage.IsAdvancedSettingsInfoWarning = true;
                advancedSystemOptionsPage.IsRestartExplorerVisible = true;
            }
        }

        /// <summary>
        /// 隐藏桌面快捷键头
        /// </summary>
        private async void OnHideDesktopShortcutArrowClicked(object sender, RoutedEventArgs args)
        {
            await Task.Run(async () =>
            {
                if (RuntimeHelper.IsElevated)
                {
                    RegistryHelper.SaveRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons", "29", @"%SystemRoot%\System32\imageres.dll,197");
                }
            });
            if (advancedSystemOptionsPage is not null)
            {
                advancedSystemOptionsPage.IsAdvancedSettingsInfoWarning = true;
                advancedSystemOptionsPage.IsRestartExplorerVisible = true;
            }
        }

        /// <summary>
        /// 右键菜单样式选中项发生变化时触发的事件
        /// </summary>
        private async void OnRightClickMenuStyleSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(RightClickMenuStyle, comboBox.SelectedItem))
            {
                RightClickMenuStyle = comboBox.SelectedItem is ComboBoxItemModel rightClickMenuStyle ? rightClickMenuStyle : null;

                if (RightClickMenuStyle is not null)
                {
                    await Task.Run(() =>
                    {
                        if (Equals(RightClickMenuStyle, RightClickMenuStyleList[0]))
                        {
                            RegistryHelper.DeleteRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", true);
                        }
                        else if (Equals(RightClickMenuStyle, RightClickMenuStyleList[1]))
                        {
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", null, string.Empty);
                        }
                    });
                }

                bool isClassicRightClickMenuExisted = await Task.Run(() =>
                {
                    return RegistryHelper.IsRegistryKeyExisted(Registry.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
                });
                RightClickMenuStyle = isClassicRightClickMenuExisted ? RightClickMenuStyleList.Find(item => Equals(item.SelectedValue, "Windows10ClassicMenu")) : RightClickMenuStyleList.Find(item => Equals(item.SelectedValue, "Windows11ModernMenu"));
                if (advancedSystemOptionsPage is not null)
                {
                    advancedSystemOptionsPage.IsAdvancedSettingsInfoWarning = true;
                    advancedSystemOptionsPage.IsRestartExplorerVisible = true;
                }
            }
        }

        /// <summary>
        /// 资源管理器样式选中项发生变化时触发的事件
        /// </summary>
        private async void OnFileExplorerStyleSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(FileExplorerStyle, comboBox.SelectedItem))
            {
                FileExplorerStyle = comboBox.SelectedItem is ComboBoxItemModel fileExplorerStyle ? fileExplorerStyle : null;

                if (FileExplorerStyle is not null)
                {
                    await Task.Run(() =>
                    {
                        if (Equals(FileExplorerStyle, FileExplorerStyleList[0]))
                        {
                            RegistryHelper.DeleteRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}", true);
                            RegistryHelper.DeleteRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}", true);
                            RegistryHelper.RemoveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Toolbar\ShellBrowser", "ITBar7Layout");
                        }
                        else if (Equals(FileExplorerStyle, FileExplorerStyleList[1]))
                        {
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}", string.Empty, "CLSID_ItemsViewAdapter");
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}\InProcServer32", string.Empty, @"C:\Windows\System32\Windows.UI.FileExplorer.dll_");
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}\InProcServer32", "ThreadingModel", "Apartment");
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}", string.Empty, "File Explorer Xaml Island View Adapter");
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}\InProcServer32", string.Empty, @"C:\Windows\System32\Windows.UI.FileExplorer.dll_");
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}\InProcServer32", "ThreadingModel", "Apartment");
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Toolbar\ShellBrowser", "ITBar7Layout", layout);
                        }
                    });
                }

                bool isClassicFileExplorerExisted = await Task.Run(() =>
                {
                    string itemsViewAdapter = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}", string.Empty);
                    string fileExplorerDllPath1 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}\InProcServer32", string.Empty);
                    string apartment1 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{2aa9162e-c906-4dd9-ad0b-3d24a8eef5a0}\InProcServer32", "ThreadingModel");
                    bool flag1 = string.Equals(itemsViewAdapter, "CLSID_ItemsViewAdapter") && string.Equals(fileExplorerDllPath1, @"C:\Windows\System32\Windows.UI.FileExplorer.dll_") && string.Equals(apartment1, "Apartment");
                    string fileExplorerXamlIslandViewAdapter = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}", string.Empty);
                    string fileExplorerDllPath2 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}\InProcServer32", string.Empty);
                    string apartment2 = RegistryHelper.ReadRegistryKey<string>(Registry.CurrentUser, @"Software\Classes\CLSID\{6480100b-5a83-4d1e-9f69-8ae5a88e9a33}\InProcServer32", "ThreadingModel");
                    bool flag2 = string.Equals(fileExplorerXamlIslandViewAdapter, "File Explorer Xaml Island View Adapter") && string.Equals(fileExplorerDllPath2, @"C:\Windows\System32\Windows.UI.FileExplorer.dll_") && string.Equals(apartment2, "Apartment");
                    byte[] tBar7Layout = RegistryHelper.ReadRegistryKey<byte[]>(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Toolbar\ShellBrowser", "ITBar7Layout");
                    return (tBar7Layout?.SequenceEqual(layout) ?? false) && flag1 && flag2;
                });
                FileExplorerStyle = isClassicFileExplorerExisted ? FileExplorerStyleList.Find(item => Equals(item.SelectedValue, "Windows10ClassicFileExplorer")) : FileExplorerStyleList.Find(item => Equals(item.SelectedValue, "Windows11ModernFileExplorer"));
                if (advancedSystemOptionsPage is not null)
                {
                    advancedSystemOptionsPage.IsAdvancedSettingsInfoWarning = true;
                    advancedSystemOptionsPage.IsRestartExplorerVisible = true;
                }
            }
        }

        /// <summary>
        /// 显示 / 关闭供应商同步通知
        /// </summary>
        private async void OnSyncProviderNotificationsToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsSyncProviderNotificationsEnabled = toggleSwitch.IsOn;
                IsSyncProviderNotificationsEnabled = await Task.Run(() =>
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSyncProviderNotifications", Convert.ToInt32(IsSyncProviderNotificationsEnabled));
                    }
                    return RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSyncProviderNotifications");
                });
            }
        }

        /// <summary>
        /// 资源管理器首页位置发生变化时触发的事件
        /// </summary>
        private async void OnFileExplorerHomePositionSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(FileExplorerHomePosition, comboBox.SelectedItem))
            {
                FileExplorerHomePosition = comboBox.SelectedItem is ComboBoxItemModel fileExplorerHomePosition ? fileExplorerHomePosition : null;

                if (FileExplorerHomePosition is not null)
                {
                    await Task.Run(() =>
                    {
                        if (Equals(FileExplorerHomePosition, FileExplorerHomePositionList[0]))
                        {
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", 1);
                        }
                        else if (Equals(FileExplorerHomePosition, FileExplorerHomePositionList[1]))
                        {
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", 2);
                        }
                        else if (Equals(FileExplorerHomePosition, FileExplorerHomePositionList[2]))
                        {
                            RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", 3);
                        }
                    });
                }

                FileExplorerHomePosition = await Task.Run(() =>
                {
                    ComboBoxItemModel fileExplorerTo = FileExplorerHomePositionList[1];
                    int launchTo = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo");
                    switch (launchTo)
                    {
                        case 1:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[0];
                                break;
                            }
                        case 2:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[1];
                                break;
                            }
                        case 3:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[2];
                                break;
                            }
                        default:
                            {
                                fileExplorerTo = FileExplorerHomePositionList[1];
                                break;
                            }
                    }
                    return fileExplorerTo;
                });
            }
        }

        /// <summary>
        /// 创建快捷方式不显示快捷方式文字
        /// </summary>
        private async void OnCreateShortcutWithoutShortcutTextToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(IsShortcutWithoutShortcutTextEnabled, toggleSwitch.IsOn))
            {
                IsShortcutWithoutShortcutTextEnabled = toggleSwitch.IsOn;
                IsShortcutWithoutShortcutTextEnabled = await Task.Run(() =>
                {
                    if (IsShortcutWithoutShortcutTextEnabled)
                    {
                        RegistryHelper.SaveRegistryKey<byte[]>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer", "link", [0x00, 0x00, 0x00, 0x00]);
                    }
                    else
                    {
                        RegistryHelper.SaveRegistryKey<byte[]>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer", "link", [0x1E, 0x00, 0x00, 0x00]);
                    }

                    byte[] linkValue = RegistryHelper.ReadRegistryKey<byte[]>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer", "link");
                    if (linkValue is null)
                    {
                        return false;
                    }
                    else
                    {
                        if (linkValue.Length < 4)
                        {
                            return false;
                        }
                        else
                        {
                            return linkValue[0] is 0 && linkValue[1] is 0 && linkValue[2] is 0 && linkValue[3] is 0;
                        }
                    }
                });
                if (advancedSystemOptionsPage is not null)
                {
                    advancedSystemOptionsPage.IsAdvancedSettingsInfoWarning = true;
                    advancedSystemOptionsPage.IsRestartExplorerVisible = true;
                }
            }
        }

        /// <summary>
        /// 保存视觉效果设置
        /// </summary>
        private async void OnSaveVisualEffectsClicked(object sender, RoutedEventArgs args)
        {
            if (!IsUpdatingVisualEffects)
            {
                IsUpdatingVisualEffects = true;
                VisualEffects visualEffects = await Task.Run(() =>
                {
                    int visualEffectsPlanIndex = VisualEffectsPlanList.IndexOf(VisualEffectsPlan);
                    if (visualEffectsPlanIndex >= 0 && visualEffectsPlanIndex <= 3)
                    {
                        RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", visualEffectsPlanIndex);
                    }
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "AlwaysHibernateThumbnails", Convert.ToInt32(VisualEffectsList[0].IsVisualEnabled));
                    User32Library.SystemParametersInfo(SPI.SPI_SETCLIENTAREAANIMATION, 0, Convert.ToInt32(VisualEffectsList[1].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    User32Library.SystemParametersInfo(SPI.SPI_SETMENUANIMATION, 0, Convert.ToInt32(VisualEffectsList[2].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    User32Library.SystemParametersInfo(SPI.SPI_SETCOMBOBOXANIMATION, 0, Convert.ToInt32(VisualEffectsList[3].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    User32Library.SystemParametersInfo(SPI.SPI_SETLISTBOXSMOOTHSCROLLING, 0, Convert.ToInt32(VisualEffectsList[4].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    User32Library.SystemParametersInfo(SPI.SPI_SETFONTSMOOTHING, Convert.ToUInt32(VisualEffectsList[5].IsVisualEnabled), 0, SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "EnableAeroPeek", Convert.ToInt32(VisualEffectsList[6].IsVisualEnabled));
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAnimations", Convert.ToInt32(VisualEffectsList[7].IsVisualEnabled));
                    User32Library.SystemParametersInfo(SPI.SPI_SETDRAGFULLWINDOWS, Convert.ToUInt32(VisualEffectsList[8].IsVisualEnabled), 0, SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "IconsOnly", VisualEffectsList[9].IsVisualEnabled ? 0 : 1);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewAlphaSelect", Convert.ToInt32(VisualEffectsList[10].IsVisualEnabled));
                    User32Library.SystemParametersInfo(SPI.SPI_SETDROPSHADOW, 0, Convert.ToInt32(VisualEffectsList[11].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    User32Library.SystemParametersInfo(SPI.SPI_SETSELECTIONFADE, 0, Convert.ToInt32(VisualEffectsList[12].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    User32Library.SystemParametersInfo(SPI.SPI_SETTOOLTIPANIMATION, 0, Convert.ToInt32(VisualEffectsList[13].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    User32Library.SystemParametersInfo(SPI.SPI_SETCURSORSHADOW, 0, Convert.ToInt32(VisualEffectsList[14].IsVisualEnabled), SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
                    RegistryHelper.SaveRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewShadow", Convert.ToInt32(VisualEffectsList[15].IsVisualEnabled));
                    SetSystemParametersInfoAnimationInfoValue(SPI.SPI_SETANIMATION, new()
                    {
                        cbSize = (uint)Marshal.SizeOf<ANIMATIONINFO>(),
                        iMinAnimate = Convert.ToInt32(VisualEffectsList[16].IsVisualEnabled)
                    });
                    Shell32Library.SHChangeNotify(SHCNE.SHCNE_ASSOCCHANGED, SHCNF.SHCNF_IDLIST | SHCNF.SHCNF_FLUSH, 0, 0);

                    VisualEffects visualEffects = new()
                    {
                        VisualEffectsPlan = RegistryHelper.ReadRegistryKey<int>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting"),
                        SaveTaskbarThumbnailPreview = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "AlwaysHibernateThumbnails"),
                        AnimationControlsAndElementsInsideWindow = GetSystemParametersInfoBoolValue(SPI.SPI_GETCLIENTAREAANIMATION),
                        FadeinAndOutOrSlideMenuToView = GetSystemParametersInfoBoolValue(SPI.SPI_GETMENUANIMATION),
                        SlideToOpenCombobox = GetSystemParametersInfoBoolValue(SPI.SPI_GETCOMBOBOXANIMATION),
                        SmoothScrollListbox = GetSystemParametersInfoBoolValue(SPI.SPI_GETLISTBOXSMOOTHSCROLLING),
                        SmoothScreenFontEdges = GetSystemParametersInfoBoolValue(SPI.SPI_GETFONTSMOOTHING),
                        EnablePeek = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "EnableAeroPeek"),
                        TaskbarAnimations = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAnimations"),
                        ShowWindowContentsWhileDragging = GetSystemParametersInfoBoolValue(SPI.SPI_GETDRAGFULLWINDOWS),
                        ShowThumbnail = !RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "IconsOnly"),
                        ShowSemitransparentSelectedRectangle = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewAlphaSelect"),
                        ShowShadowUnderWindow = GetSystemParametersInfoBoolValue(SPI.SPI_GETDROPSHADOW),
                        FadeoutMenuAfterClicking = GetSystemParametersInfoBoolValue(SPI.SPI_GETSELECTIONFADE),
                        FadeinFadeoutOrSlideToolTipInView = GetSystemParametersInfoBoolValue(SPI.SPI_GETTOOLTIPANIMATION),
                        ShowShadowUnderMousePointer = GetSystemParametersInfoBoolValue(SPI.SPI_GETCURSORSHADOW),
                        UseShadowForIconLabelsOnDesktop = RegistryHelper.ReadRegistryKey<bool>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewShadow"),
                        ShowAnimationWhenMaximizingOrMinimizing = GetSystemParametersInfoAnimationInfoValue(SPI.SPI_GETANIMATION)
                    };
                    return visualEffects;
                });
                VisualEffectsPlan = visualEffects.VisualEffectsPlan >= 0 && visualEffects.VisualEffectsPlan <= 3 ? VisualEffectsPlanList[visualEffects.VisualEffectsPlan] : null;

                if (VisualEffectsPlan is not null)
                {
                    VisualEffectsList[0].IsVisualEnabled = visualEffects.SaveTaskbarThumbnailPreview;
                    VisualEffectsList[1].IsVisualEnabled = visualEffects.AnimationControlsAndElementsInsideWindow;
                    VisualEffectsList[2].IsVisualEnabled = visualEffects.FadeinAndOutOrSlideMenuToView;
                    VisualEffectsList[3].IsVisualEnabled = visualEffects.SlideToOpenCombobox;
                    VisualEffectsList[4].IsVisualEnabled = visualEffects.SmoothScrollListbox;
                    VisualEffectsList[5].IsVisualEnabled = visualEffects.SmoothScreenFontEdges;
                    VisualEffectsList[6].IsVisualEnabled = visualEffects.EnablePeek;
                    VisualEffectsList[7].IsVisualEnabled = visualEffects.TaskbarAnimations;
                    VisualEffectsList[8].IsVisualEnabled = visualEffects.ShowWindowContentsWhileDragging;
                    VisualEffectsList[9].IsVisualEnabled = visualEffects.ShowThumbnail;
                    VisualEffectsList[10].IsVisualEnabled = visualEffects.ShowSemitransparentSelectedRectangle;
                    VisualEffectsList[11].IsVisualEnabled = visualEffects.ShowShadowUnderWindow;
                    VisualEffectsList[12].IsVisualEnabled = visualEffects.FadeoutMenuAfterClicking;
                    VisualEffectsList[13].IsVisualEnabled = visualEffects.FadeinFadeoutOrSlideToolTipInView;
                    VisualEffectsList[14].IsVisualEnabled = visualEffects.ShowShadowUnderMousePointer;
                    VisualEffectsList[15].IsVisualEnabled = visualEffects.UseShadowForIconLabelsOnDesktop;
                    VisualEffectsList[16].IsVisualEnabled = visualEffects.ShowAnimationWhenMaximizingOrMinimizing;
                }
                else
                {
                    foreach (VisualEffectsModel visualEffectsItem in VisualEffectsList)
                    {
                        visualEffectsItem.IsVisualEnabled = false;
                    }
                }
            }
            IsUpdatingVisualEffects = false;
        }

        /// <summary>
        /// 视觉效果方案选中项发生变化时触发的事件
        /// </summary>
        private void OnVisualEffectsPlanSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(VisualEffectsPlan, comboBox.SelectedItem))
            {
                VisualEffectsPlan = comboBox.SelectedItem is ComboBoxItemModel visualEffectsPlan ? visualEffectsPlan : null;

                if (VisualEffectsPlan is not null && VisualEffectsPlan.SelectedValue is string tag)
                {
                    switch (tag)
                    {
                        case "WindowsChooseBestSettings":
                            {
                                foreach (VisualEffectsModel visualEffectsItem in VisualEffectsList)
                                {
                                    visualEffectsItem.IsVisualEnabled = visualEffectsItem.VisualTag is not "SaveTaskbarThumbnailPreview" && visualEffectsItem.VisualTag is not "ShowShadowUnderMousePointer";
                                }
                                break;
                            }
                        case "BestAppearance":
                            {
                                foreach (VisualEffectsModel visualEffectsItem in VisualEffectsList)
                                {
                                    visualEffectsItem.IsVisualEnabled = true;
                                }
                                break;
                            }
                        case "BestPerformance":
                            {
                                foreach (VisualEffectsModel visualEffectsItem in VisualEffectsList)
                                {
                                    visualEffectsItem.IsVisualEnabled = false;
                                }
                                break;
                            }
                    }
                }
            }
        }

        #endregion 第三部分：高级系统选项——个性化页面——挂载的事件

        /// <summary>
        /// 获取图标存储位置和索引
        /// </summary>
        private async Task<(string, int)> GetShellIconLocationAsync(string iconStoragePath)
        {
            string iconPath = string.Empty;
            int iconIndex = 0;
            await Task.Run(() =>
            {
                try
                {
                    StringBuilder stringBuilder = new(iconStoragePath);
                    iconIndex = ShlwapiLibrary.PathParseIconLocation(stringBuilder);
                    iconPath = stringBuilder.ToString();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetShellIconLocationAsync), 1, e);
                }
            });
            return ValueTuple.Create(iconPath, iconIndex);
        }

        /// <summary>
        /// 获取图标对应的图标资源
        /// </summary>
        private async Task<MemoryStream> GetShellIconAsync(string iconStoragePath, int iconIndex)
        {
            return await Task.Run(() =>
            {
                MemoryStream memoryStream = null;

                try
                {
                    nint[] phicon = new nint[1];
                    int[] piconid = new int[1];
                    User32Library.PrivateExtractIcons(iconStoragePath, iconIndex, 48, 48, phicon, piconid, 1, 0);
                    Icon icon = Icon.FromHandle(phicon[0]);
                    memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetShellIconAsync), 1, e);
                }

                return memoryStream;
            });
        }

        /// <summary>
        /// 获取图标对应的显示名称
        /// </summary>
        private async Task<string> GetShellIconDisplayNameAsync(string iconPathName)
        {
            string displayName = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    if (Shell32Library.SHCreateItemFromParsingName(iconPathName, null, typeof(IShellItem).GUID, out IShellItem shellItem) is 0)
                    {
                        shellItem.GetDisplayName(SIGDN.SIGDN_NORMALDISPLAY, out displayName);
                        Marshal.ReleaseComObject(shellItem);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetShellIconDisplayNameAsync), 1, e);
                }
            });

            return displayName;
        }

        /// <summary>
        /// 获取桌面图标显示状态
        /// </summary>
        private bool GetDesktopIconVisibility(string iconPathName, string iconTag)
        {
            int? iconValue = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", iconPathName);
            bool visible = false;
            switch (iconTag)
            {
                case "ControlPanel":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
                case "Library":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
                case "Network":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
                case "RecycleBin":
                    {
                        visible = !iconValue.HasValue || iconValue.Value is 0;
                        break;
                    }
                case "ThisPC":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
                case "UserFolder":
                    {
                        visible = iconValue.HasValue && iconValue.Value is 0;
                        break;
                    }
            }
            return visible;
        }

        /// <summary>
        /// 获取导航窗格图标显示状态
        /// </summary>
        private bool GetNavigationPaneIconVisibility(string iconPathName, string iconTag)
        {
            bool visible = false;
            switch (iconTag)
            {
                case "Home":
                    {
                        int? iconValue = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", iconPathName), "System.IsPinnedToNameSpaceTree");
                        visible = !iconValue.HasValue || iconValue.Value is not 0;
                        break;
                    }
                case "Library":
                    {
                        int? iconValue = RegistryHelper.ReadRegistryKey<int?>(Registry.ClassesRoot, string.Format(@"CLSID\{0}", iconPathName), "System.IsPinnedToNameSpaceTree");
                        visible = !iconValue.HasValue || iconValue.Value is not 0;
                        break;
                    }
                case "Linux":
                    {
                        int? iconValue = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", iconPathName), "System.IsPinnedToNameSpaceTree");
                        visible = !iconValue.HasValue || iconValue.Value is not 0;
                        break;
                    }
                case "PhotoGallery":
                    {
                        int? iconValue = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", iconPathName), "System.IsPinnedToNameSpaceTree");
                        visible = !iconValue.HasValue || iconValue.Value is not 0;
                        break;
                    }
                case "RecycleBin":
                    {
                        int? iconValue = RegistryHelper.ReadRegistryKey<int?>(Registry.CurrentUser, string.Format(@"Software\Classes\CLSID\{0}", iconPathName), "System.IsPinnedToNameSpaceTree");
                        visible = !iconValue.HasValue || iconValue.Value is not 0;
                        break;
                    }
                case "RemovableDevice":
                    {
                        visible = RegistryHelper.IsRegistryKeyExisted(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace\DelegateFolders\{F5FB2C77-0E2F-4A16-A381-3E560C68BC83}");
                        break;
                    }
            }
            return visible;
        }

        /// <summary>
        /// 获取 SystemParametersInfo 存储的 BOOL 结构体值
        /// </summary>
        private bool GetSystemParametersInfoBoolValue(SPI spi)
        {
            nint pValue = Marshal.AllocHGlobal(sizeof(int));

            try
            {
                Marshal.WriteInt32(pValue, 0);
                return User32Library.SystemParametersInfo(spi, 0, pValue, SPIF.None) && Convert.ToBoolean(Marshal.ReadInt32(pValue));
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetSystemParametersInfoBoolValue), 1, e);
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(pValue);
            }
        }

        /// <summary>
        /// 获取 SystemParametersInfo 存储的 AnimationInfo 结构体值
        /// </summary>
        private bool GetSystemParametersInfoAnimationInfoValue(SPI spi)
        {
            ANIMATIONINFO animationInfo = new()
            {
                cbSize = (uint)Marshal.SizeOf<ANIMATIONINFO>()
            };
            nint pAI = Marshal.AllocHGlobal(Marshal.SizeOf<ANIMATIONINFO>());
            try
            {
                Marshal.StructureToPtr(animationInfo, pAI, false);
                nint ptr = pAI;
                if (User32Library.SystemParametersInfo(spi, animationInfo.cbSize, ptr, 0))
                {
                    animationInfo = Marshal.PtrToStructure<ANIMATIONINFO>(pAI);
                }

                return Convert.ToBoolean(animationInfo.iMinAnimate);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(GetSystemParametersInfoAnimationInfoValue), 1, e);
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(pAI);
            }
        }

        /// <summary>
        /// 设置 SystemParametersInfo 存储的 AnimationInfo 结构体值
        /// </summary>
        private void SetSystemParametersInfoAnimationInfoValue(SPI spi, ANIMATIONINFO animationInfo)
        {
            nint pAI = Marshal.AllocHGlobal(Marshal.SizeOf<ANIMATIONINFO>());
            try
            {
                Marshal.StructureToPtr(animationInfo, pAI, false);
                User32Library.SystemParametersInfo(spi, animationInfo.cbSize, pAI, SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(AdvancedSystemOptionsPersonalizationPage), nameof(SetSystemParametersInfoAnimationInfoValue), 1, e);
            }
            finally
            {
                Marshal.FreeHGlobal(pAI);
            }
        }
    }
}
