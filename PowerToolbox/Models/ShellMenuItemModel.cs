using Microsoft.UI.Xaml.Media;
using PowerToolbox.Extensions.DataType.Classes;
using PowerToolbox.Extensions.DataType.Enums;
using System;
using System.ComponentModel;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 扩展菜单项数据模型
    /// </summary>
    internal sealed class ShellMenuItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 菜单标题
        /// </summary>
        internal string MenuTitleText
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuTitleText)));
                }
            }
        }

        /// <summary>
        /// 使用图标
        /// </summary>
        internal bool UseIcon
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(UseIcon)));
                }
            }
        }

        /// <summary>
        /// 使用应用程序图标
        /// </summary>
        internal bool UseProgramIcon
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(UseProgramIcon)));
                }
            }
        }

        /// <summary>
        /// 使用主题图标
        /// </summary>
        internal bool UseThemeIcon
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(UseThemeIcon)));
                }
            }
        }

        /// <summary>
        /// 菜单项图标
        /// </summary>
        internal ImageSource MenuIcon
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuIcon)));
                }
            }
        }

        /// <summary>
        /// 是否选中当前菜单项
        /// </summary>
        internal bool IsSelected
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 菜单项索引
        /// </summary>
        internal int MenuIndex
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(MenuIndex)));
                }
            }
        }

        /// <summary>
        /// 菜单键值
        /// </summary>
        internal string MenuKey { get; set; }

        /// <summary>
        /// 菜单项 GUID 值
        /// </summary>
        internal Guid MenuGuid { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        internal MenuType MenuType { get; set; }

        /// <summary>
        /// 默认的菜单项图标
        /// </summary>
        internal string DefaultIconPath { get; set; }

        /// <summary>
        /// 浅色主题下的菜单项图标
        /// </summary>
        internal string LightThemeIconPath { get; set; }

        /// <summary>
        /// 深色主题下的菜单项图标
        /// </summary>
        internal string DarkThemeIconPath { get; set; }

        /// <summary>
        /// 菜单程序路径
        /// </summary>
        internal string MenuProgramPathText { get; set; }

        /// <summary>
        /// 菜单参数
        /// </summary>
        internal string MenuParameter { get; set; }

        /// <summary>
        /// 是否总是需要提权运行
        /// </summary>
        internal bool IsAlwaysRunAsAdministrator { get; set; }

        /// <summary>
        /// 是否启用文件夹背景菜单项
        /// </summary>
        internal bool FolderBackground { get; set; }

        /// <summary>
        /// 是否启用文件夹桌面菜单项
        /// </summary>
        internal bool FolderDesktop { get; set; }

        /// <summary>
        /// 是否启用文件夹目录菜单项
        /// </summary>
        internal bool FolderDirectory { get; set; }

        /// <summary>
        /// 是否启用文件夹驱动器菜单项
        /// </summary>
        internal bool FolderDrive { get; set; }

        /// <summary>
        /// 菜单项文件匹配规则
        /// </summary>
        internal string MenuFileMatchRule { get; set; }

        /// <summary>
        /// 菜单项文件匹配格式
        /// </summary>
        internal string MenuFileMatchFormatText { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        internal WinRTObservableCollection<ShellMenuItemModel> SubMenuItemCollection { get; set; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
