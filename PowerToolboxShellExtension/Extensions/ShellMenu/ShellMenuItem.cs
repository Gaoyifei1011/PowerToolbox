using System;
using System.Collections.Generic;

namespace PowerToolboxShellExtension.Extensions.ShellMenu
{
    /// <summary>
    /// 菜单项设置选项
    /// </summary>
    public sealed class ShellMenuItem
    {
        /// <summary>
        /// 菜单键值
        /// </summary>
        internal string MenuKey { get; set; }

        /// <summary>
        /// 菜单项 GUID 值
        /// </summary>
        internal Guid MenuGuid { get; set; }

        /// <summary>
        /// 菜单项标题
        /// </summary>
        internal string MenuTitleText { get; set; }

        /// <summary>
        /// 使用图标
        /// </summary>
        internal bool UseIcon { get; set; }

        /// <summary>
        /// 使用应用程序图标
        /// </summary>
        internal bool UseProgramIcon { get; set; }

        /// <summary>
        /// 使用主题图标
        /// </summary>
        internal bool UseThemeIcon { get; set; }

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
        internal string MenuProgramPath { get; set; }

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
        /// 菜单项索引
        /// </summary>
        internal int MenuIndex { get; set; }

        /// <summary>
        /// 子菜单项
        /// </summary>
        internal List<ShellMenuItem> SubShellMenuItem { get; set; } = [];
    }
}
