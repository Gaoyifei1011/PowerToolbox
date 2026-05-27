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
    public sealed class NavigationViewItemModel
    {
        /// <summary>
        /// 导航控件项具体类型
        /// </summary>
        public NavigationViewItemKind NavigationViewItemKind { get; set; }

        /// <summary>
        /// 导航图标
        /// </summary>
        public IconElement NavigationIcon { get; set; }

        /// <summary>
        /// 导航标题
        /// </summary>
        public string NavigationTitle { get; set; }

        /// <summary>
        /// 导航标签
        /// </summary>
        public string NavigationTag { get; set; }

        /// <summary>
        /// 导航子标签中对应的父标签
        /// </summary>
        public string ParentTag { get; set; }

        /// <summary>
        /// 导航类型
        /// </summary>
        public Type NavigationPage { get; set; }

        /// <summary>
        /// 可视状态
        /// </summary>
        public Visibility VisibleState { get; set; }

        /// <summary>
        /// 子菜单项
        /// </summary>
        public WinRTObservableCollection<NavigationViewItemModel> NavigationViewItemMenuItemsCollection { get; } = [];
    }
}
