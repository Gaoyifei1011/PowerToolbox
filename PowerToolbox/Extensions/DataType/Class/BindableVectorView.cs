using Microsoft.UI.Xaml.Interop;
using System.Collections;

namespace PowerToolbox.Extensions.DataType.Class
{
    public class BindableVectorView(IList list) : BindableVector(list), IBindableVectorView
    {
        public override bool IsReadOnly
        {
            get { return true; }
        }
    }
}
