using Windows.Foundation;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    internal struct UvMeasure
    {
        internal double U;
        internal double V;

        internal UvMeasure(double u, double v)
        {
            U = u;
            V = v;
        }

        internal UvMeasure(Orientation orientation, Size size)
        {
            if (orientation is Orientation.Horizontal)
            {
                U = size.Width;
                V = size.Height;
            }
            else
            {
                U = size.Height;
                V = size.Width;
            }
        }

        internal UvMeasure(Orientation orientation, double width, double height)
        {
            if (orientation is Orientation.Horizontal)
            {
                U = width;
                V = height;
            }
            else
            {
                U = height;
                V = width;
            }
        }

        internal readonly UvMeasure Add(double u, double v)
        {
            return new(U + u, V + v);
        }

        internal readonly UvMeasure Add(UvMeasure measure)
        {
            return Add(measure.U, measure.V);
        }

        internal readonly Size ToSize(Orientation orientation)
        {
            if (orientation is Orientation.Horizontal)
            {
                return new(U, V);
            }

            return new(V, U);
        }
    }
}