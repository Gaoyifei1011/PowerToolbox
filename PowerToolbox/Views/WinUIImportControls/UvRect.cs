using System;
using Windows.Foundation;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    internal struct UvRect
    {
        internal UvMeasure Position;
        internal UvMeasure Size;

        internal readonly Rect ToRect(Orientation orientation)
        {
            if (orientation is Orientation.Vertical)
            {
                return new(Position.V, Position.U, Size.V, Size.U);
            }

            if (orientation is Orientation.Horizontal)
            {
                return new(Position.U, Position.V, Size.U, Size.V);
            }

            throw new ArgumentOutOfRangeException(nameof(orientation));
        }
    }
}