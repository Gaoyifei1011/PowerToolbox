using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Services.Root;
using PowerToolbox.Services.Shell;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Windows.Storage.Streams;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 自定义扩展菜单编辑页面
    /// </summary>
    public sealed partial class ShellMenuEditPage : Page, INotifyPropertyChanged
    {
        private readonly string AllString = ResourceService.ShellMenuEditResource.GetString("All");
        private readonly string ExtensionString = ResourceService.ShellMenuEditResource.GetString("Extension");
        private readonly string IconFilterConditionString = ResourceService.ShellMenuEditResource.GetString("IconFilterCondition");
        private readonly string MenuFileExtensionFormatString = ResourceService.ShellMenuEditResource.GetString("MenuFileExtensionFormat");
        private readonly string MenuFileNameFormatString = ResourceService.ShellMenuEditResource.GetString("MenuFileNameFormat");
        private readonly string MenuFileNameRegexFormatString = ResourceService.ShellMenuEditResource.GetString("MenuFileNameRegexFormat");
        private readonly string NameString = ResourceService.ShellMenuEditResource.GetString("Name");
        private readonly string NameRegexString = ResourceService.ShellMenuEditResource.GetString("NameRegex");
        private readonly string NoneString = ResourceService.ShellMenuEditResource.GetString("None");
        private readonly string ProgramFilterConditionString = ResourceService.ShellMenuEditResource.GetString("ProgramFilterCondition");
        private readonly string SelectIconString = ResourceService.ShellMenuEditResource.GetString("SelectIcon");
        private readonly string SelectProgramString = ResourceService.ShellMenuEditResource.GetString("SelectProgram");
        private readonly InMemoryRandomAccessStream emptyStream = new();
        private DateTimeOffset lastUpdateTime = ShellMenuService.GetLastUpdateTime();
        private string selectedDefaultIconPath = string.Empty;
        private string selectedLightThemeIconPath = string.Empty;
        private string selectedDarkThemeIconPath = string.Empty;
        private Guid editMenuGuid;
        private string editMenuKey;
        private int editMenuIndex;

        private string _menuTitleText;

        public string MenuTitleText
        {
            get { return _menuTitleText; }

            set
            {
                if (!Equals(_menuTitleText, value))
                {
                    _menuTitleText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuTitleText)));
                }
            }
        }

        private bool _useIcon;

        public bool UseIcon
        {
            get { return _useIcon; }

            set
            {
                if (!Equals(_useIcon, value))
                {
                    _useIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseIcon)));
                }
            }
        }

        private bool _useProgramIcon;

        public bool UseProgramIcon
        {
            get { return _useProgramIcon; }

            set
            {
                if (!Equals(_useProgramIcon, value))
                {
                    _useProgramIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseProgramIcon)));
                }
            }
        }

        private bool _useThemeIcon;

        public bool UseThemeIcon
        {
            get { return _useThemeIcon; }

            set
            {
                if (!Equals(_useThemeIcon, value))
                {
                    _useThemeIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseThemeIcon)));
                }
            }
        }

        private ImageSource _defaultIconImage;

        public ImageSource DefaultIconImage
        {
            get { return _defaultIconImage; }

            set
            {
                if (!Equals(_defaultIconImage, value))
                {
                    _defaultIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultIconImage)));
                }
            }
        }

        private string _defaultIconPath;

        public string DefaultIconPath
        {
            get { return _defaultIconPath; }

            set
            {
                if (!string.Equals(_defaultIconPath, value))
                {
                    _defaultIconPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultIconPath)));
                }
            }
        }

        private ImageSource _lightThemeIconImage;

        public ImageSource LightThemeIconImage
        {
            get { return _lightThemeIconImage; }

            set
            {
                if (!Equals(_lightThemeIconImage, value))
                {
                    _lightThemeIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LightThemeIconImage)));
                }
            }
        }

        private string _lightThemeIconPath;

        public string LightThemeIconPath
        {
            get { return _lightThemeIconPath; }

            set
            {
                if (!string.Equals(_lightThemeIconPath, value))
                {
                    _lightThemeIconPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LightThemeIconPath)));
                }
            }
        }

        private ImageSource _darkThemeIconImage;

        public ImageSource DarkThemeIconImage
        {
            get { return _darkThemeIconImage; }

            set
            {
                if (!Equals(_darkThemeIconImage, value))
                {
                    _darkThemeIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DarkThemeIconImage)));
                }
            }
        }

        private string _darkThemeIconPath;

        public string DarkThemeIconPath
        {
            get { return _darkThemeIconPath; }

            set
            {
                if (!string.Equals(_darkThemeIconPath, value))
                {
                    _darkThemeIconPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DarkThemeIconPath)));
                }
            }
        }

        private string _menuProgramPathText;

        public string MenuProgramPathText
        {
            get { return _menuProgramPathText; }

            set
            {
                if (!string.Equals(_menuProgramPathText, value))
                {
                    _menuProgramPathText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuProgramPathText)));
                }
            }
        }

        private string _menuParameterText;

        public string MenuParameterText
        {
            get { return _menuParameterText; }

            set
            {
                if (!string.Equals(_menuParameterText, value))
                {
                    _menuParameterText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuParameterText)));
                }
            }
        }

        private bool _isAlwaysRunAsAdministrator;

        public bool IsAlwaysRunAsAdministrator
        {
            get { return _isAlwaysRunAsAdministrator; }

            set
            {
                if (!Equals(_isAlwaysRunAsAdministrator, value))
                {
                    _isAlwaysRunAsAdministrator = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAlwaysRunAsAdministrator)));
                }
            }
        }

        private bool _folderBackgroundMatch;

        public bool FolderBackgroundMatch
        {
            get { return _folderBackgroundMatch; }

            set
            {
                if (!Equals(_folderBackgroundMatch, value))
                {
                    _folderBackgroundMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderBackgroundMatch)));
                }
            }
        }

        private bool _folderDesktopMatch;

        public bool FolderDesktopMatch
        {
            get { return _folderDesktopMatch; }

            set
            {
                if (!Equals(_folderDesktopMatch, value))
                {
                    _folderDesktopMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderDesktopMatch)));
                }
            }
        }

        private bool _folderDirectoryMatch;

        public bool FolderDirectoryMatch
        {
            get { return _folderDirectoryMatch; }

            set
            {
                if (!Equals(_folderDirectoryMatch, value))
                {
                    _folderDirectoryMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderDirectoryMatch)));
                }
            }
        }

        private bool _folderDriveMatch;

        public bool FolderDriveMatch
        {
            get { return _folderDriveMatch; }

            set
            {
                if (!Equals(_folderDriveMatch, value))
                {
                    _folderDriveMatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderDriveMatch)));
                }
            }
        }

        private KeyValuePair<string, string> _selectedFileMatchRule;

        public KeyValuePair<string, string> SelectedFileMatchRule
        {
            get { return _selectedFileMatchRule; }

            set
            {
                if (!Equals(_selectedFileMatchRule, value))
                {
                    _selectedFileMatchRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFileMatchRule)));
                }
            }
        }

        private bool _needInputMatchFormat;

        public bool NeedInputMatchFormat
        {
            get { return _needInputMatchFormat; }

            set
            {
                if (!Equals(_needInputMatchFormat, value))
                {
                    _needInputMatchFormat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NeedInputMatchFormat)));
                }
            }
        }

        private string _menuFileMatchFormatPHText;

        public string MenuFileMatchFormatPHText
        {
            get { return _menuFileMatchFormatPHText; }

            set
            {
                if (!string.Equals(_menuFileMatchFormatPHText, value))
                {
                    _menuFileMatchFormatPHText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuFileMatchFormatPHText)));
                }
            }
        }

        private string _menuFileMatchFormatText;

        public string MenuFileMatchFormatText
        {
            get { return _menuFileMatchFormatText; }

            set
            {
                if (!string.Equals(_menuFileMatchFormatText, value))
                {
                    _menuFileMatchFormatText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuFileMatchFormatText)));
                }
            }
        }

        private List<KeyValuePair<string, string>> FileMatchRuleList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellMenuEditPage()
        {
            InitializeComponent();

            FileMatchRuleList.Add(new KeyValuePair<string, string>("None", NoneString));
            FileMatchRuleList.Add(new KeyValuePair<string, string>("Name", NameString));
            FileMatchRuleList.Add(new KeyValuePair<string, string>("NameRegex", NameRegexString));
            FileMatchRuleList.Add(new KeyValuePair<string, string>("Extension", ExtensionString));
            FileMatchRuleList.Add(new KeyValuePair<string, string>("All", AllString));
            SelectedFileMatchRule = FileMatchRuleList[4];
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            lastUpdateTime = ShellMenuService.GetLastUpdateTime();

            if (args.Parameter is List<object> argsList && argsList.Count is 2 && argsList[1] is ShellMenuItem shellMenuItem)
            {
                // 添加菜单
                if (argsList[0] is ShellEditKind.AddMenu)
                {
                    selectedDefaultIconPath = string.Empty;
                    selectedLightThemeIconPath = string.Empty;
                    selectedDarkThemeIconPath = string.Empty;
                    editMenuKey = shellMenuItem.MenuKey;
                    editMenuGuid = shellMenuItem.MenuGuid;
                    editMenuIndex = shellMenuItem.MenuIndex;
                    selectedDefaultIconPath = string.Empty;
                    selectedLightThemeIconPath = string.Empty;
                    selectedDarkThemeIconPath = string.Empty;
                    MenuTitleText = string.Empty;
                    UseIcon = true;
                    UseProgramIcon = true;
                    UseThemeIcon = false;
                    DefaultIconPath = string.Empty;
                    BitmapImage defaultIconImage = new();
                    defaultIconImage.SetSource(emptyStream);
                    DefaultIconImage = defaultIconImage;
                    LightThemeIconPath = string.Empty;
                    BitmapImage lightThemeIconImage = new();
                    lightThemeIconImage.SetSource(emptyStream);
                    LightThemeIconImage = lightThemeIconImage;
                    DarkThemeIconPath = string.Empty;
                    BitmapImage darkThemeIconImage = new();
                    darkThemeIconImage.SetSource(emptyStream);
                    DarkThemeIconImage = lightThemeIconImage;
                    MenuProgramPathText = string.Empty;
                    MenuParameterText = string.Empty;
                    FolderBackgroundMatch = false;
                    IsAlwaysRunAsAdministrator = false;
                    FolderDesktopMatch = false;
                    FolderDirectoryMatch = false;
                    FolderDriveMatch = false;
                    SelectedFileMatchRule = FileMatchRuleList[4];
                    NeedInputMatchFormat = false;
                    MenuFileMatchFormatPHText = string.Empty;
                    MenuFileMatchFormatText = string.Empty;
                }
                // 编辑菜单
                else if (argsList[0] is ShellEditKind.EditMenu)
                {
                    selectedDefaultIconPath = string.Empty;
                    selectedLightThemeIconPath = string.Empty;
                    selectedDarkThemeIconPath = string.Empty;
                    editMenuKey = shellMenuItem.MenuKey;
                    editMenuIndex = shellMenuItem.MenuIndex;
                    editMenuGuid = shellMenuItem.MenuGuid;
                    MenuTitleText = shellMenuItem.MenuTitleText;
                    UseIcon = shellMenuItem.UseIcon;
                    UseProgramIcon = shellMenuItem.UseProgramIcon;
                    UseThemeIcon = shellMenuItem.UseThemeIcon;
                    DefaultIconPath = shellMenuItem.DefaultIconPath;
                    LightThemeIconPath = shellMenuItem.LightThemeIconPath;
                    DarkThemeIconPath = shellMenuItem.DarkThemeIconPath;
                    MenuProgramPathText = shellMenuItem.MenuProgramPath;
                    MenuParameterText = shellMenuItem.MenuParameter;
                    IsAlwaysRunAsAdministrator = shellMenuItem.IsAlwaysRunAsAdministrator;
                    FolderBackgroundMatch = shellMenuItem.FolderBackground;
                    FolderDesktopMatch = shellMenuItem.FolderDesktop;
                    FolderDirectoryMatch = shellMenuItem.FolderDirectory;
                    FolderDriveMatch = shellMenuItem.FolderDrive;
                    MenuFileMatchFormatText = shellMenuItem.MenuFileMatchFormatText;
                    SelectedFileMatchRule = FileMatchRuleList[4];

                    for (int index = 0; index < FileMatchRuleList.Count; index++)
                    {
                        if (Equals(shellMenuItem.MenuFileMatchRule, FileMatchRuleList[index].Key))
                        {
                            SelectedFileMatchRule = FileMatchRuleList[index];
                        }
                    }

                    NeedInputMatchFormat = !Equals(SelectedFileMatchRule, FileMatchRuleList[0]) && !Equals(SelectedFileMatchRule, FileMatchRuleList[4]);
                    BitmapImage defaultIconImage = new();
                    defaultIconImage.SetSource(emptyStream);
                    DefaultIconImage = defaultIconImage;
                    BitmapImage lightThemeIconImage = new();
                    lightThemeIconImage.SetSource(emptyStream);
                    LightThemeIconImage = lightThemeIconImage;
                    BitmapImage darkThemeIconImage = new();
                    darkThemeIconImage.SetSource(emptyStream);
                    DarkThemeIconImage = lightThemeIconImage;

                    if (File.Exists(DefaultIconPath))
                    {
                        try
                        {
                            Icon defaultIcon = Icon.ExtractAssociatedIcon(DefaultIconPath);
                            MemoryStream memoryStream = new();
                            defaultIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            defaultIconImage = new();
                            defaultIconImage.SetSource(memoryStream.AsRandomAccessStream());
                            DefaultIconImage = defaultIconImage;
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnNavigatedTo), 1, e);
                        }
                    }

                    if (File.Exists(LightThemeIconPath))
                    {
                        try
                        {
                            Icon lightThemeIcon = Icon.ExtractAssociatedIcon(LightThemeIconPath);
                            MemoryStream memoryStream = new();
                            lightThemeIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            lightThemeIconImage = new();
                            lightThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                            LightThemeIconImage = lightThemeIconImage;
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnNavigatedTo), 2, e);
                        }
                    }

                    if (File.Exists(DarkThemeIconPath))
                    {
                        try
                        {
                            Icon darkThemeIcon = Icon.ExtractAssociatedIcon(DarkThemeIconPath);
                            MemoryStream memoryStream = new();
                            darkThemeIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            darkThemeIconImage = new();
                            darkThemeIconImage.SetSource(memoryStream.AsRandomAccessStream());
                            DarkThemeIconImage = darkThemeIconImage;
                            memoryStream.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnNavigatedTo), 3, e);
                        }
                    }
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：自定义扩展菜单编辑页面——挂载的事件

        /// <summary>
        /// 保存更改
        /// </summary>
        private async void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return;
            }

            // 有部分内容是必填项，没填的内容进行提示
            if (string.IsNullOrEmpty(MenuTitleText))
            {
                MenuTitleTextBox.Focus(FocusState.Programmatic);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuTitleEmpty));
                return;
            }

            if (UseIcon && !UseProgramIcon)
            {
                if (UseThemeIcon)
                {
                    if (string.IsNullOrEmpty(LightThemeIconPath))
                    {
                        MenuLigthThemeIconBrowserButton.Focus(FocusState.Programmatic);
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuLightThemeIconPathEmpty));
                        return;
                    }

                    if (string.IsNullOrEmpty(DarkThemeIconPath))
                    {
                        MenuDarkThemeIconBrowserButton.Focus(FocusState.Programmatic);
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuDarkThemeIconPathEmpty));
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DefaultIconPath))
                    {
                        MenuDefaultIconBrowserButton.Focus(FocusState.Programmatic);
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuDefaultIconPathEmpty));
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(MenuProgramPathText))
            {
                MenuProgramBrowserButton.Focus(FocusState.Programmatic);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuProgramPathEmpty));
                return;
            }

            if ((Equals(SelectedFileMatchRule, FileMatchRuleList[1]) || Equals(SelectedFileMatchRule, FileMatchRuleList[2]) || Equals(SelectedFileMatchRule, FileMatchRuleList[3])) && string.IsNullOrEmpty(MenuFileMatchFormatText))
            {
                MenuFileMatchFormatTextBox.Focus(FocusState.Programmatic);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuMatchRuleEmpty));
                return;
            }

            // 保存指定菜单项信息
            ShellMenuItem shellMenuItem = new()
            {
                MenuGuid = editMenuGuid,
                MenuIndex = editMenuIndex,
                MenuTitleText = MenuTitleText,
                UseIcon = UseIcon,
                UseProgramIcon = UseProgramIcon,
                UseThemeIcon = UseThemeIcon,
                DefaultIconPath = DefaultIconPath,
                LightThemeIconPath = LightThemeIconPath,
                DarkThemeIconPath = DarkThemeIconPath,
                MenuProgramPath = MenuProgramPathText,
                MenuParameter = MenuParameterText,
                IsAlwaysRunAsAdministrator = IsAlwaysRunAsAdministrator,
                FolderBackground = FolderBackgroundMatch,
                FolderDesktop = FolderDesktopMatch,
                FolderDirectory = FolderDirectoryMatch,
                FolderDrive = FolderDriveMatch,
                MenuFileMatchRule = SelectedFileMatchRule.Key,
                MenuFileMatchFormatText = MenuFileMatchFormatText
            };

            ShellMenuService.SaveShellMenuItem(editMenuKey, shellMenuItem);

            // 复制选中的图标文件到指定目录
            if (File.Exists(selectedDefaultIconPath))
            {
                try
                {
                    string defaultIconDirectoryPath = Path.GetDirectoryName(DefaultIconPath);
                    if (!Directory.Exists(defaultIconDirectoryPath))
                    {
                        Directory.CreateDirectory(defaultIconDirectoryPath);
                    }

                    File.Copy(selectedDefaultIconPath, DefaultIconPath, true);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnSaveClicked), 1, e);
                }
            }

            if (File.Exists(selectedLightThemeIconPath))
            {
                try
                {
                    string lightThemeIconDirectoryPath = Path.GetDirectoryName(LightThemeIconPath);
                    if (!Directory.Exists(lightThemeIconDirectoryPath))
                    {
                        Directory.CreateDirectory(lightThemeIconDirectoryPath);
                    }

                    File.Copy(selectedLightThemeIconPath, LightThemeIconPath, true);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnSaveClicked), 2, e);
                }
            }

            if (File.Exists(selectedDarkThemeIconPath))
            {
                try
                {
                    string darkThemeIconDirectoryPath = Path.GetDirectoryName(DarkThemeIconPath);
                    if (!Directory.Exists(darkThemeIconDirectoryPath))
                    {
                        Directory.CreateDirectory(darkThemeIconDirectoryPath);
                    }

                    File.Copy(selectedDarkThemeIconPath, DarkThemeIconPath, true);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnSaveClicked), 3, e);
                }
            }

            if (MainWindow.Current.GetFrameContent() is ShellMenuPage shellMenuPage)
            {
                shellMenuPage.NavigateTo(shellMenuPage.PageList[0], null, false);
            }

            ShellMenuService.UpdateLastUpdateTime();
            lastUpdateTime = ShellMenuService.GetLastUpdateTime();
        }

        /// <summary>
        /// 菜单标题内容发生更改时的事件
        /// </summary>
        private void OnTitleTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuTitleText = (sender as Microsoft.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 使用图标修改时触发的事件
        /// </summary>
        private void OnUseIconToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                UseIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 使用应用程序图标修改时触发的事件
        /// </summary>
        private void OnUseProgramIconToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                UseProgramIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 启用主题图标按钮修改时触发的事件
        /// </summary>
        private void OnUseThemeIconToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                UseThemeIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 默认图标修改
        /// </summary>
        private void OnDefaultIconBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = IconFilterConditionString,
                Title = SelectIconString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                try
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(emptyStream);
                    selectedDefaultIconPath = openFileDialog.FileName;
                    DefaultIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, Convert.ToString(editMenuGuid), "DefaultIcon.ico");
                    Icon defaultIcon = Icon.ExtractAssociatedIcon(openFileDialog.FileName);
                    MemoryStream memoryStream = new();
                    defaultIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                    DefaultIconImage = bitmapImage;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnDefaultIconBrowserClicked), 1, e);
                }
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 浅色主题图标修改
        /// </summary>
        private void OnLightThemeIconBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = IconFilterConditionString,
                Title = SelectIconString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                try
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(emptyStream);
                    selectedLightThemeIconPath = openFileDialog.FileName;
                    LightThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, Convert.ToString(editMenuGuid), "LightThemeIcon.ico");
                    Icon lightThemeIcon = Icon.ExtractAssociatedIcon(openFileDialog.FileName);
                    MemoryStream memoryStream = new();
                    lightThemeIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                    LightThemeIconImage = bitmapImage;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnLightThemeIconBrowserClicked), 1, e);
                }
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 深色主题图标修改
        /// </summary>
        private void OnDarkThemeIconBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = IconFilterConditionString,
                Title = SelectIconString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                try
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(emptyStream);
                    selectedDarkThemeIconPath = openFileDialog.FileName;
                    DarkThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, Convert.ToString(editMenuGuid), "DarkThemeIcon.ico");
                    Icon icon = Icon.ExtractAssociatedIcon(openFileDialog.FileName);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                    DarkThemeIconImage = bitmapImage;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(OnDarkThemeIconBrowserClicked), 1, e);
                }
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 修改菜单程序文件路径
        /// </summary>
        private void OnMenuProgramPathBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = ProgramFilterConditionString,
                Title = SelectProgramString
            };
            if (openFileDialog.ShowDialog() is DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                MenuProgramPathText = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// 菜单参数内容发生更改时的事件
        /// </summary>
        private void OnMenuParameterTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuParameterText = (sender as Microsoft.UI.Xaml.Controls.TextBox).Text;
        }

        /// <summary>
        /// 总是需要提权运行修改时触发的事件
        /// </summary>
        private void OnIsAlwaysRunAsAdministratorToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                IsAlwaysRunAsAdministrator = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 修改菜单文件匹配规则
        /// </summary>
        private void OnFileMatchRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is KeyValuePair<string, string> fileMatchRule)
            {
                SelectedFileMatchRule = fileMatchRule;
                MenuFileMatchFormatText = string.Empty;

                if (Equals(SelectedFileMatchRule, FileMatchRuleList[0]) || Equals(SelectedFileMatchRule, FileMatchRuleList[4]))
                {
                    NeedInputMatchFormat = false;
                    MenuFileMatchFormatPHText = string.Empty;
                }
                else if (Equals(SelectedFileMatchRule, FileMatchRuleList[1]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = MenuFileNameFormatString;
                }
                else if (Equals(SelectedFileMatchRule, FileMatchRuleList[2]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = string.Format(MenuFileNameRegexFormatString, @"[\s\S]+.jpg | [\w\W]*.jpg");
                }
                else if (Equals(SelectedFileMatchRule, FileMatchRuleList[3]))
                {
                    NeedInputMatchFormat = true;
                    MenuFileMatchFormatPHText = MenuFileExtensionFormatString;
                }
            }
        }

        /// <summary>
        /// 菜单文件匹配格式内容发生更改时的事件
        /// </summary>
        private void OnMenuFileMatchFormatTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuFileMatchFormatText = (sender as Microsoft.UI.Xaml.Controls.TextBox).Text;
        }

        #endregion 第二部分：自定义扩展菜单编辑页面——挂载的事件
    }
}
