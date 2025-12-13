using Microsoft.UI.Xaml.Interop;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerToolbox.Extensions.DataType.Class
{
    public class BindableVector : IList, IBindableVector
    {
        private readonly IList implementation;

        public uint Size
        {
            get { return (uint)implementation.Count; }
        }

        public bool IsFixedSize
        {
            get { return implementation.IsFixedSize; }
        }

        public object SyncRoot
        {
            get { return implementation.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return implementation.IsSynchronized; }
        }

        public object this[int index]
        {
            get { return implementation[index]; }

            set { implementation[index] = value; }
        }

        public int Count
        {
            get { return implementation.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return implementation.IsReadOnly; }
        }

        public BindableVector()
        {
            implementation = new List<object>();
        }

        public BindableVector(IList list)
        {
            implementation = list;
        }

        public int Add(object item)
        {
            return implementation.Add(item);
        }

        public void Clear()
        {
            implementation.Clear();
        }

        public bool Contains(object item)
        {
            return implementation.Contains(item);
        }

        public void CopyTo(Array array, int index)
        {
            implementation.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return implementation.GetEnumerator();
        }

        public int IndexOf(object item)
        {
            return implementation.IndexOf(item);
        }

        public void Insert(int index, object item)
        {
            implementation.Insert(index, item);
        }

        public void Remove(object item)
        {
            implementation.Remove(item);
        }

        public void RemoveAt(int index)
        {
            implementation.RemoveAt(index);
        }

        public object GetAt(uint index)
        {
            return implementation[(int)index];
        }

        public IBindableVectorView GetView()
        {
            return new BindableVectorView(implementation);
        }

        public bool IndexOf(object value, out uint index)
        {
            int indexOf = implementation.IndexOf(value);

            if (indexOf >= 0)
            {
                index = (uint)indexOf;
                return true;
            }
            else
            {
                index = 0;
                return false;
            }
        }

        public void SetAt(uint index, object value)
        {
            implementation[(int)index] = value;
        }

        public void InsertAt(uint index, object value)
        {
            implementation.Insert((int)index, value);
        }

        public void RemoveAt(uint index)
        {
            implementation.RemoveAt((int)index);
        }

        public void Append(object value)
        {
            implementation.Add(value);
        }

        public void RemoveAtEnd()
        {
            implementation.RemoveAt(implementation.Count - 1);
        }

        public IBindableIterator First()
        {
            return new BindableIterator(implementation);
        }
    }
}
