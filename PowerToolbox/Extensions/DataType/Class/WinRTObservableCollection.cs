using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PowerToolbox.Extensions.DataType.Class
{
    /// <summary>
    /// 扩展 ObservableCollection，以通知 WinUI 3 集合已经发生变化
    /// </summary>
    public class WinRTObservableCollection<T> : ObservableCollection<T>, Microsoft.UI.Xaml.Interop.INotifyCollectionChanged
    {
        public new event Microsoft.UI.Xaml.Interop.NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnCollectionChanged(args);

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        CollectionChanged?.Invoke(this, new Microsoft.UI.Xaml.Interop.NotifyCollectionChangedEventArgs(Microsoft.UI.Xaml.Interop.NotifyCollectionChangedAction.Add, new BindableVector(args.NewItems), new BindableVector(args.OldItems), args.NewStartingIndex, args.OldStartingIndex));
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        CollectionChanged?.Invoke(this, new Microsoft.UI.Xaml.Interop.NotifyCollectionChangedEventArgs(Microsoft.UI.Xaml.Interop.NotifyCollectionChangedAction.Remove, new BindableVector(args.NewItems), new BindableVector(args.OldItems), args.NewStartingIndex, args.OldStartingIndex));
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        CollectionChanged?.Invoke(this, new Microsoft.UI.Xaml.Interop.NotifyCollectionChangedEventArgs(Microsoft.UI.Xaml.Interop.NotifyCollectionChangedAction.Replace, new BindableVector(args.NewItems), new BindableVector(args.OldItems), args.NewStartingIndex, args.OldStartingIndex));
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        CollectionChanged?.Invoke(this, new Microsoft.UI.Xaml.Interop.NotifyCollectionChangedEventArgs(Microsoft.UI.Xaml.Interop.NotifyCollectionChangedAction.Move, new BindableVector(args.NewItems), new BindableVector(args.OldItems), args.NewStartingIndex, args.OldStartingIndex));
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        CollectionChanged?.Invoke(this, new Microsoft.UI.Xaml.Interop.NotifyCollectionChangedEventArgs(Microsoft.UI.Xaml.Interop.NotifyCollectionChangedAction.Reset, new BindableVector(args.NewItems), new BindableVector(args.OldItems), args.NewStartingIndex, args.OldStartingIndex));
                        break;
                    }
            }
        }
    }
}
