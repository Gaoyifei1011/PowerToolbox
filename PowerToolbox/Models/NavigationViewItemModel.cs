using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using System;

namespace PowerToolbox.Models
{
    /// <summary>
    /// 导航控件项数据模型
    /// </summary>
    internal sealed class NavigationViewItemModel
    {
        /// <summary>
        /// 导航控件项具体类型
        /// </summary>
        internal NavigationViewItemKind NavigationViewItemKind { get; set; }

        /// <summary>
        /// 导航图标
        /// </summary>
        internal IconElement NavigationIcon { get; set; }

        /// <summary>
        /// 导航标题
        /// </summary>
        internal string NavigationTitle { get; set; }

        /// <summary>
        /// 导航标签
        /// </summary>
        internal string NavigationTag { get; set; }

        /// <summary>
        /// 导航子标签中对应的父标签
        /// </summary>
        internal string ParentTag { get; set; }

        /// <summary>
        /// 导航类型
        /// </summary>
        internal Type NavigationPage { get; set; }

        /// <summary>
        /// 可视状态
        /// </summary>
        internal Visibility VisibleState { get; set; }

        /// <summary>
        /// 子菜单项
        /// </summary>
        internal WinRTObservableCollection<NavigationViewItemModel> NavigationViewItemMenuItemsCollection { get; } = [];
    }
}
