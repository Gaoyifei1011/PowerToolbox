using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation.Collections;

namespace PowerToolbox.Extensions.Collections
{
    /// <summary>
    /// 扩展 ObservableCollection 以支持 WinRT 的 IObservableVector 通知集合发生变化
    /// </summary>
    public class WinRTObservableCollection<T> : ObservableCollection<T>, IObservableVector<object>
    {
        public event VectorChangedEventHandler<object> VectorChanged;

        protected override void ClearItems()
        {
            base.ClearItems();

            if (VectorChanged is not null)
            {
                VectorChanged(this, new VectorChangedEventArgs
                {
                    CollectionChange = CollectionChange.Reset,
                    Index = 0
                });
            }
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            if (VectorChanged is not null)
            {
                VectorChanged(this, new VectorChangedEventArgs
                {
                    CollectionChange = CollectionChange.ItemRemoved,
                    Index = Convert.ToUInt32(index)
                });
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (VectorChanged is not null)
            {
                VectorChanged(this, new VectorChangedEventArgs
                {
                    CollectionChange = CollectionChange.ItemInserted,
                    Index = Convert.ToUInt32(index)
                });
            }
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);

            if (VectorChanged is not null)
            {
                VectorChanged(this, new VectorChangedEventArgs
                {
                    CollectionChange = CollectionChange.ItemChanged,
                    Index = Convert.ToUInt32(index)
                });
            }
        }

        int IList<object>.IndexOf(object item)
        {
            return IndexOf((T)item);
        }

        void IList<object>.Insert(int index, object item)
        {
            Insert(index, (T)item);
        }

        object IList<object>.this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                SetItem(index, (T)value);
            }
        }

        void ICollection<object>.Add(object item)
        {
            Add((T)item);
        }

        bool ICollection<object>.Contains(object item)
        {
            return Contains((T)item);
        }

        void ICollection<object>.CopyTo(object[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentException(null, nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("The remaining space of the target array is insufficient to accommodate all the elements in the collection");
            }

            for (int index = 0; index < Items.Count; index++)
            {
                array[arrayIndex + index] = Items[index];
            }
        }

        bool ICollection<object>.Remove(object item)
        {
            return Remove((T)item);
        }

        bool ICollection<object>.IsReadOnly
        {
            get
            {
                return (this as ICollection<T>).IsReadOnly;
            }
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            IList<object> items = [.. Items];
            return items.GetEnumerator();
        }
    }
}
