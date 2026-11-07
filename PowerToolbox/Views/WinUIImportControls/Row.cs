using System;
using System.Collections.Generic;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    internal sealed class Row
    {
        internal readonly List<UvRect> ChildrenRects = [];

        internal UvMeasure Size;

        internal UvRect Rect()
        {
            UvMeasure position = new();

            if (ChildrenRects.Count > 0)
            {
                position = ChildrenRects[0].Position;
            }

            return new()
            {
                Position = position,
                Size = Size
            };
        }

        internal void Add(UvMeasure position, UvMeasure size)
        {
            UvRect rect = new()
            {
                Position = position,
                Size = size
            };

            ChildrenRects.Add(rect);
            double newU = position.U + size.U;
            double newV = Math.Max(Size.V, size.V);
            Size = new UvMeasure(newU, newV);
        }
    }
}