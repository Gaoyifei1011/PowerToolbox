using Microsoft.UI.Xaml.Media;
using PowerToolbox.Extensions.Collections;
using PowerToolbox.Extensions.DataType.Enums;
using System;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 扩展菜单项数据模型
    /// </summary>
    public sealed class ShellMenuItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 菜单标题
        /// </summary>
        private string _menuTitleText;

        public string MenuTitleText
        {
            get { return _menuTitleText; }

            set
            {
                if (!string.Equals(_menuTitleText, value))
                {
                    _menuTitleText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuTitleText)));
                }
            }
        }

        /// <summary>
        /// 使用图标
        /// </summary>
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

        /// <summary>
        /// 使用应用程序图标
        /// </summary>
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

        /// <summary>
        /// 使用主题图标
        /// </summary>
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

        /// <summary>
        /// 菜单项图标
        /// </summary>
        private ImageSource _menuIcon;

        public ImageSource MenuIcon
        {
            get { return _menuIcon; }

            set
            {
                if (!Equals(_menuIcon, value))
                {
                    _menuIcon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuIcon)));
                }
            }
        }

        /// <summary>
        /// 是否选中当前菜单项
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 菜单项索引
        /// </summary>
        private int _menuIndex;

        public int MenuIndex
        {
            get { return _menuIndex; }

            set
            {
                if (!Equals(_menuIndex, value))
                {
                    _menuIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuIndex)));
                }
            }
        }

        /// <summary>
        /// 菜单键值
        /// </summary>
        public string MenuKey { get; set; }

        /// <summary>
        /// 菜单项 GUID 值
        /// </summary>
        public Guid MenuGuid { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }

        /// <summary>
        /// 默认的菜单项图标
        /// </summary>
        public string DefaultIconPath { get; set; }

        /// <summary>
        /// 浅色主题下的菜单项图标
        /// </summary>
        public string LightThemeIconPath { get; set; }

        /// <summary>
        /// 深色主题下的菜单项图标
        /// </summary>
        public string DarkThemeIconPath { get; set; }

        /// <summary>
        /// 菜单程序路径
        /// </summary>
        public string MenuProgramPathText { get; set; }

        /// <summary>
        /// 菜单参数
        /// </summary>
        public string MenuParameter { get; set; }

        /// <summary>
        /// 是否总是需要提权运行
        /// </summary>
        public bool IsAlwaysRunAsAdministrator { get; set; }

        /// <summary>
        /// 是否启用文件夹背景菜单项
        /// </summary>
        public bool FolderBackground { get; set; }

        /// <summary>
        /// 是否启用文件夹桌面菜单项
        /// </summary>
        public bool FolderDesktop { get; set; }

        /// <summary>
        /// 是否启用文件夹目录菜单项
        /// </summary>
        public bool FolderDirectory { get; set; }

        /// <summary>
        /// 是否启用文件夹驱动器菜单项
        /// </summary>
        public bool FolderDrive { get; set; }

        /// <summary>
        /// 菜单项文件匹配规则
        /// </summary>
        public string MenuFileMatchRule { get; set; }

        /// <summary>
        /// 菜单项文件匹配格式
        /// </summary>
        public string MenuFileMatchFormatText { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public WinRTObservableCollection<ShellMenuItemModel> SubMenuItemCollection { get; set; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
