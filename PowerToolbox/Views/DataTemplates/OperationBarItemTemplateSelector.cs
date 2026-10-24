using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Models;

namespace PowerToolbox.Views.DataTemplates
{
    /// <summary>
    /// 操作栏数据模版选择器
    /// </summary>
    internal sealed partial class OperationBarItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OrdinaryItemTemplate { get; set; }

        public DataTemplate CheckUpdateItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is OperationBarItemModel operationBarItem)
            {
                if (operationBarItem.OperationBarKind is OperationBarKind.Ordinary)
                {
                    return OrdinaryItemTemplate;
                }
                else if (operationBarItem.OperationBarKind is OperationBarKind.CheckUpdate)
                {
                    return CheckUpdateItemTemplate;
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
