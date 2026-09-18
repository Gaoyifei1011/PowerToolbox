using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage.Streams;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 自定义扩展菜单编辑页面
    /// </summary>
    internal sealed partial class ShellMenuEditPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

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

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private string _menuTitleText;

        private string MenuTitleText
        {
            get { return _menuTitleText; }

            set
            {
                if (!Equals(_menuTitleText, value))
                {
                    _menuTitleText = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuTitleText)));
                }
            }
        }

        private bool _useIcon;

        private bool UseIcon
        {
            get { return _useIcon; }

            set
            {
                if (!Equals(_useIcon, value))
                {
                    _useIcon = value;
                    PropertyChanged?.Invoke(this, new(nameof(UseIcon)));
                }
            }
        }

        private bool _useProgramIcon;

        private bool UseProgramIcon
        {
            get { return _useProgramIcon; }

            set
            {
                if (!Equals(_useProgramIcon, value))
                {
                    _useProgramIcon = value;
                    PropertyChanged?.Invoke(this, new(nameof(UseProgramIcon)));
                }
            }
        }

        private bool _useThemeIcon;

        private bool UseThemeIcon
        {
            get { return _useThemeIcon; }

            set
            {
                if (!Equals(_useThemeIcon, value))
                {
                    _useThemeIcon = value;
                    PropertyChanged?.Invoke(this, new(nameof(UseThemeIcon)));
                }
            }
        }

        private ImageSource _defaultIconImage;

        private ImageSource DefaultIconImage
        {
            get { return _defaultIconImage; }

            set
            {
                if (!Equals(_defaultIconImage, value))
                {
                    _defaultIconImage = value;
                    PropertyChanged?.Invoke(this, new(nameof(DefaultIconImage)));
                }
            }
        }

        private string _defaultIconPath;

        private string DefaultIconPath
        {
            get { return _defaultIconPath; }

            set
            {
                if (!string.Equals(_defaultIconPath, value))
                {
                    _defaultIconPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(DefaultIconPath)));
                }
            }
        }

        private ImageSource _lightThemeIconImage;

        private ImageSource LightThemeIconImage
        {
            get { return _lightThemeIconImage; }

            set
            {
                if (!Equals(_lightThemeIconImage, value))
                {
                    _lightThemeIconImage = value;
                    PropertyChanged?.Invoke(this, new(nameof(LightThemeIconImage)));
                }
            }
        }

        private string _lightThemeIconPath;

        private string LightThemeIconPath
        {
            get { return _lightThemeIconPath; }

            set
            {
                if (!string.Equals(_lightThemeIconPath, value))
                {
                    _lightThemeIconPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(LightThemeIconPath)));
                }
            }
        }

        private ImageSource _darkThemeIconImage;

        private ImageSource DarkThemeIconImage
        {
            get { return _darkThemeIconImage; }

            set
            {
                if (!Equals(_darkThemeIconImage, value))
                {
                    _darkThemeIconImage = value;
                    PropertyChanged?.Invoke(this, new(nameof(DarkThemeIconImage)));
                }
            }
        }

        private string _darkThemeIconPath;

        private string DarkThemeIconPath
        {
            get { return _darkThemeIconPath; }

            set
            {
                if (!string.Equals(_darkThemeIconPath, value))
                {
                    _darkThemeIconPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(DarkThemeIconPath)));
                }
            }
        }

        private string _menuProgramPathText;

        private string MenuProgramPathText
        {
            get { return _menuProgramPathText; }

            set
            {
                if (!string.Equals(_menuProgramPathText, value))
                {
                    _menuProgramPathText = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuProgramPathText)));
                }
            }
        }

        private string _menuParameterText;

        private string MenuParameterText
        {
            get { return _menuParameterText; }

            set
            {
                if (!string.Equals(_menuParameterText, value))
                {
                    _menuParameterText = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuParameterText)));
                }
            }
        }

        private bool _isAlwaysRunAsAdministrator;

        private bool IsAlwaysRunAsAdministrator
        {
            get { return _isAlwaysRunAsAdministrator; }

            set
            {
                if (!Equals(_isAlwaysRunAsAdministrator, value))
                {
                    _isAlwaysRunAsAdministrator = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsAlwaysRunAsAdministrator)));
                }
            }
        }

        private bool _folderBackgroundMatch;

        private bool FolderBackgroundMatch
        {
            get { return _folderBackgroundMatch; }

            set
            {
                if (!Equals(_folderBackgroundMatch, value))
                {
                    _folderBackgroundMatch = value;
                    PropertyChanged?.Invoke(this, new(nameof(FolderBackgroundMatch)));
                }
            }
        }

        private bool _folderDesktopMatch;

        private bool FolderDesktopMatch
        {
            get { return _folderDesktopMatch; }

            set
            {
                if (!Equals(_folderDesktopMatch, value))
                {
                    _folderDesktopMatch = value;
                    PropertyChanged?.Invoke(this, new(nameof(FolderDesktopMatch)));
                }
            }
        }

        private bool _folderDirectoryMatch;

        private bool FolderDirectoryMatch
        {
            get { return _folderDirectoryMatch; }

            set
            {
                if (!Equals(_folderDirectoryMatch, value))
                {
                    _folderDirectoryMatch = value;
                    PropertyChanged?.Invoke(this, new(nameof(FolderDirectoryMatch)));
                }
            }
        }

        private bool _folderDriveMatch;

        private bool FolderDriveMatch
        {
            get { return _folderDriveMatch; }

            set
            {
                if (!Equals(_folderDriveMatch, value))
                {
                    _folderDriveMatch = value;
                    PropertyChanged?.Invoke(this, new(nameof(FolderDriveMatch)));
                }
            }
        }

        private ComboBoxItemModel _selectedFileMatchRule;

        private ComboBoxItemModel SelectedFileMatchRule
        {
            get { return _selectedFileMatchRule; }

            set
            {
                if (!Equals(_selectedFileMatchRule, value))
                {
                    _selectedFileMatchRule = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedFileMatchRule)));
                }
            }
        }

        private bool _needInputMatchFormat;

        private bool NeedInputMatchFormat
        {
            get { return _needInputMatchFormat; }

            set
            {
                if (!Equals(_needInputMatchFormat, value))
                {
                    _needInputMatchFormat = value;
                    PropertyChanged?.Invoke(this, new(nameof(NeedInputMatchFormat)));
                }
            }
        }

        private string _menuFileMatchFormatPHText;

        private string MenuFileMatchFormatPHText
        {
            get { return _menuFileMatchFormatPHText; }

            set
            {
                if (!string.Equals(_menuFileMatchFormatPHText, value))
                {
                    _menuFileMatchFormatPHText = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuFileMatchFormatPHText)));
                }
            }
        }

        private string _menuFileMatchFormatText;

        private string MenuFileMatchFormatText
        {
            get { return _menuFileMatchFormatText; }

            set
            {
                if (!string.Equals(_menuFileMatchFormatText, value))
                {
                    _menuFileMatchFormatText = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuFileMatchFormatText)));
                }
            }
        }

        private List<ComboBoxItemModel> FileMatchRuleList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal ShellMenuEditPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            UpdateParameter(args);
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 保存更改
        /// </summary>
        private async void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            if (await SaveCheckAsync())
            {
                SaveMenuInformation();

                // 复制选中的图标文件到指定目录
                MoveFileToSpecificFolder(selectedDefaultIconPath, DefaultIconPath);
                MoveFileToSpecificFolder(selectedLightThemeIconPath, LightThemeIconPath);
                MoveFileToSpecificFolder(selectedDarkThemeIconPath, DarkThemeIconPath);

                // 更新操作时间
                ShellMenuService.UpdateLastUpdateTime();
                lastUpdateTime = ShellMenuService.GetLastUpdateTime();

                if (MainWindow.Current.GetFrameContent() is ShellMenuPage shellMenuPage)
                {
                    shellMenuPage.NavigateTo(shellMenuPage.PageList[0], null, false);
                }
            }
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
            if (sender is ToggleSwitch toggleSwitch && !Equals(UseIcon, toggleSwitch.IsOn))
            {
                UseIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 使用应用程序图标修改时触发的事件
        /// </summary>
        private void OnUseProgramIconToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(UseProgramIcon, toggleSwitch.IsOn))
            {
                UseProgramIcon = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 启用主题图标按钮修改时触发的事件
        /// </summary>
        private void OnUseThemeIconToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(UseThemeIcon, toggleSwitch.IsOn))
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
                selectedDefaultIconPath = openFileDialog.FileName;
                DefaultIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, Convert.ToString(editMenuGuid), "DefaultIcon.ico");
                DefaultIconImage = GetIconImage(openFileDialog.FileName);
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
                selectedLightThemeIconPath = openFileDialog.FileName;
                LightThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, Convert.ToString(editMenuGuid), "LightThemeIcon.ico");
                LightThemeIconImage = GetIconImage(openFileDialog.FileName);
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
                selectedDarkThemeIconPath = openFileDialog.FileName;
                DarkThemeIconPath = Path.Combine(ShellMenuService.ShellMenuConfigDirectory.FullName, Convert.ToString(editMenuGuid), "DarkThemeIcon.ico");
                DarkThemeIconImage = GetIconImage(openFileDialog.FileName);
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
        private void OnFileMatchRuleSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is Microsoft.UI.Xaml.Controls.ComboBox comboBox && !Equals(SelectedFileMatchRule, comboBox.SelectedItem))
            {
                SelectedFileMatchRule = comboBox.SelectedItem is ComboBoxItemModel fileMatchRule ? fileMatchRule : null;
                MenuFileMatchFormatText = string.Empty;

                if (SelectedFileMatchRule is not null)
                {
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
        }

        /// <summary>
        /// 菜单文件匹配格式内容发生更改时的事件
        /// </summary>
        private void OnMenuFileMatchFormatTextChanged(object sender, TextChangedEventArgs args)
        {
            MenuFileMatchFormatText = (sender as Microsoft.UI.Xaml.Controls.TextBox).Text;
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            FileMatchRuleList.Add(new() { SelectedValue = "None", DisplayMember = NoneString });
            FileMatchRuleList.Add(new() { SelectedValue = "Name", DisplayMember = NameString });
            FileMatchRuleList.Add(new() { SelectedValue = "NameRegex", DisplayMember = NameRegexString });
            FileMatchRuleList.Add(new() { SelectedValue = "Extension", DisplayMember = ExtensionString });
            FileMatchRuleList.Add(new() { SelectedValue = "All", DisplayMember = AllString });
            SelectedFileMatchRule = FileMatchRuleList[4];
        }

        /// <summary>
        /// 更新参数
        /// </summary>
        private void UpdateParameter(NavigationEventArgs args)
        {
            if (args is null)
            {
                return;
            }

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
                        if (Equals(shellMenuItem.MenuFileMatchRule, FileMatchRuleList[index].SelectedValue))
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
                    DarkThemeIconImage = darkThemeIconImage;

                    if (File.Exists(DefaultIconPath))
                    {
                        DefaultIconImage = GetIconImage(DefaultIconPath);
                    }

                    if (File.Exists(LightThemeIconPath))
                    {
                        LightThemeIconImage = GetIconImage(LightThemeIconPath);
                        UpdateIcon(LightThemeIconPath, LightThemeIconImage);
                    }

                    if (File.Exists(DarkThemeIconPath))
                    {
                        DarkThemeIconImage = GetIconImage(DarkThemeIconPath);
                    }
                }
            }
        }

        /// <summary>
        /// 更新图标
        /// </summary>
        private void UpdateIcon(string iconPath, ImageSource imageSource)
        {
            if (string.IsNullOrEmpty(iconPath) || imageSource is null)
            {
                return;
            }

            try
            {
                Icon defaultIcon = Icon.ExtractAssociatedIcon(iconPath);
                MemoryStream memoryStream = new();
                defaultIcon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmapImage = new();
                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                imageSource = bitmapImage;
                memoryStream.Dispose();
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(UpdateIcon), 1, e);
            }
        }

        /// <summary>
        /// 保存检查
        /// </summary>
        private async Task<bool> SaveCheckAsync()
        {
            // 菜单数据已发生更改，通知用户手动刷新
            if (lastUpdateTime < ShellMenuService.GetLastUpdateTime())
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShellMenuNeedToRefreshData));
                return false;
            }

            // 有部分内容是必填项，没填的内容进行提示
            if (string.IsNullOrEmpty(MenuTitleText))
            {
                MenuTitleTextBox.Focus(FocusState.Programmatic);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuTitleEmpty));
                return false;
            }

            if (UseIcon && !UseProgramIcon)
            {
                if (UseThemeIcon)
                {
                    if (string.IsNullOrEmpty(LightThemeIconPath))
                    {
                        MenuLigthThemeIconBrowserButton.Focus(FocusState.Programmatic);
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuLightThemeIconPathEmpty));
                        return false;
                    }

                    if (string.IsNullOrEmpty(DarkThemeIconPath))
                    {
                        MenuDarkThemeIconBrowserButton.Focus(FocusState.Programmatic);
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuDarkThemeIconPathEmpty));
                        return false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DefaultIconPath))
                    {
                        MenuDefaultIconBrowserButton.Focus(FocusState.Programmatic);
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuDefaultIconPathEmpty));
                        return false;
                    }
                }
            }

            if (string.IsNullOrEmpty(MenuProgramPathText))
            {
                MenuProgramBrowserButton.Focus(FocusState.Programmatic);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuProgramPathEmpty));
                return false;
            }

            if ((Equals(SelectedFileMatchRule, FileMatchRuleList[1]) || Equals(SelectedFileMatchRule, FileMatchRuleList[2]) || Equals(SelectedFileMatchRule, FileMatchRuleList[3])) && string.IsNullOrEmpty(MenuFileMatchFormatText))
            {
                MenuFileMatchFormatTextBox.Focus(FocusState.Programmatic);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.MenuMatchRuleEmpty));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 保存指定菜单项信息
        /// </summary>
        private void SaveMenuInformation()
        {
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
                MenuFileMatchRule = Convert.ToString(SelectedFileMatchRule.SelectedValue),
                MenuFileMatchFormatText = MenuFileMatchFormatText
            };

            ShellMenuService.SaveShellMenuItem(editMenuKey, shellMenuItem);
        }

        /// <summary>
        /// 复制选中的图标文件到指定目录
        /// </summary>
        private void MoveFileToSpecificFolder(string iconPath, string defaultIconPath)
        {
            if (File.Exists(iconPath))
            {
                try
                {
                    string defaultIconDirectoryPath = Path.GetDirectoryName(defaultIconPath);
                    if (!Directory.Exists(defaultIconDirectoryPath))
                    {
                        Directory.CreateDirectory(defaultIconDirectoryPath);
                    }

                    File.Copy(iconPath, defaultIconPath, true);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(MoveFileToSpecificFolder), 1, e);
                }
            }
        }

        /// <summary>
        /// 更新浅色主题图标
        /// </summary>
        private BitmapImage GetIconImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            try
            {
                if (File.Exists(fileName))
                {
                    Icon icon = Icon.ExtractAssociatedIcon(fileName);
                    MemoryStream memoryStream = new();
                    icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(emptyStream);
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                    memoryStream.Dispose();
                    return bitmapImage;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(ShellMenuEditPage), nameof(GetIconImage), 1, e);
                return null;
            }
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
