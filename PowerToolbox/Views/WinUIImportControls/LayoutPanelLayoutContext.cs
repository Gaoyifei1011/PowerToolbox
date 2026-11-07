using System.Collections.Generic;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    internal sealed class LayoutPanelLayoutContext(LayoutPanel owner) : NonVirtualizingLayoutContext
    {
        private readonly LayoutPanel _owner = owner;

        protected override IReadOnlyList<UIElement> ChildrenCore
        {
            get
            {
                List<UIElement> children = [.. _owner.Children];
                return children;
            }
        }

        protected override object LayoutStateCore
        {
            get
            {
                return _owner.LayoutState;
            }
            set
            {
                _owner.LayoutState = value;
            }
        }
    }
}
