using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace PowerToolbox.Views.CustomControls
{
    /// <summary>
    /// 带扩展器的表示列表项的视觉元素
    /// </summary>
    public partial class ExpanderListViewItemPresenter : ListViewItemPresenter
    {
        public ExpanderListViewItemPresenter()
        {
            Loaded += (sender, args) =>
            {
                if (VisualTreeHelper.GetChildrenCount(this) is 3)
                {
                    if (VisualTreeHelper.GetChild(this, 0) is Border listViewItemPresenterBorder)
                    {
                        listViewItemPresenterBorder.BorderBrush = BorderBrush;
                        listViewItemPresenterBorder.BorderThickness = BorderThickness;
                        listViewItemPresenterBorder.Margin = new Thickness();
                    }

                    if (VisualTreeHelper.GetChild(this, 2) is Border checkBoxListViewItemPresenterBorder)
                    {
                        checkBoxListViewItemPresenterBorder.VerticalAlignment = VerticalAlignment.Top;
                        checkBoxListViewItemPresenterBorder.Margin = new() { Left = 14, Right = 0, Top = 20, Bottom = 0 };
                    }
                }
            };
        }
    }
}
