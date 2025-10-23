using Microsoft.UI.Xaml.Media;
using PowerToolbox.Extensions.DataType.Class;
using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 右键菜单项数据模型
    /// </summary>
    public class ContextMenuModel
    {
        /// <summary>
        /// 菜单图标
        /// </summary>
        public ImageSource PackageIcon { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public Uri PackageIconUri { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string PackageDisplayName { get; set; }

        /// <summary>
        /// 应用包全部名称
        /// </summary>
        public string PackageFullName { get; set; }

        /// <summary>
        /// 应用包路径
        /// </summary>
        public string PackagePath { get; set; }

        /// <summary>
        /// 子菜单项
        /// </summary>
        public WinRTObservableCollection<ContextMenuItemModel> ContextMenuItemCollection { get; set; }
    }
}
