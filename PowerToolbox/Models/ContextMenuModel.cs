using Microsoft.UI.Xaml.Media;
using PowerToolbox.Extensions.DataType.Class;
using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 右键菜单项数据模型
    /// </summary>
    internal class ContextMenuModel
    {
        /// <summary>
        /// 菜单图标
        /// </summary>
        internal ImageSource PackageIcon { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        internal Uri PackageIconUri { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        internal string PackageDisplayName { get; set; }

        /// <summary>
        /// 应用包全部名称
        /// </summary>
        internal string PackageFullName { get; set; }

        /// <summary>
        /// 应用包路径
        /// </summary>
        internal string PackagePath { get; set; }

        /// <summary>
        /// 子菜单项
        /// </summary>
        internal WinRTObservableCollection<ContextMenuItemModel> ContextMenuItemCollection { get; set; }
    }
}
