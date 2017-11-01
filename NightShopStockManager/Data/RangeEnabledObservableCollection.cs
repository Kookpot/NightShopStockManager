using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NightShopStockManager
{
    public class RangeEnabledObservableCollection<T> : ObservableCollection<T>
    {
        public void InsertRange(IEnumerable<T> items)
        {
            CheckReentrancy();
            foreach (var item in items)
                Items.Add(item);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void InsertRangeNoNotify(IEnumerable<T> items)
        {
            CheckReentrancy();
            foreach (var item in items)
                Items.Add(item);
        }

        public void InsertNoNotify(T item)
        {
            CheckReentrancy();
            Items.Add(item);
        }

        public void RefreshWithoutChange()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}