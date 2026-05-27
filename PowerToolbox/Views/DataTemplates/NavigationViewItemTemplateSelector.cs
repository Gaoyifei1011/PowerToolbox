using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;

namespace PowerToolbox.Views.DataTemplates
{
    /// <summary>
    /// 导航项数据模板选择器
    /// </summary>
    public class NavigationViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NavigationViewItemTemplate { get; set; }

        public DataTemplate NavigationViewItemSeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is NavigationViewItemModel navigationViewItem)
            {
                if(navigationViewItem.NavigationViewItemKind is NavigationViewItemKind.Item)
                {
                    return NavigationViewItemTemplate;
                }
                else if(navigationViewItem.NavigationViewItemKind is NavigationViewItemKind.Seperator)
                {
                    return NavigationViewItemSeparatorTemplate;
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
