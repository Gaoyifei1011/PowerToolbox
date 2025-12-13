using Microsoft.UI.Xaml.Interop;
using System.Collections;

namespace PowerToolbox.Extensions.DataType.Class
{
    public class BindableIterator(IEnumerable enumerable) : IBindableIterator
    {
        private readonly IEnumerator enumerator = enumerable.GetEnumerator();

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public object Current
        {
            get { return enumerator.Current; }
        }

        public bool HasCurrent
        {
            get { return enumerator.Current is not null; }
        }
    }
}
