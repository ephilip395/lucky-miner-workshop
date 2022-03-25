using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Lucky.Collection
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        public int capacity = 9999;

        public AsyncObservableCollection()
        {
        }

        public AsyncObservableCollection(int capacity)
        {
            this.capacity = capacity;
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }


        public void PressIfFulled(T item)
        {
            ExecuteOnSyncContext(() =>
            {
                if (base.Count == capacity){
                    base.RemoveAt(0);
                }
                base.Add(item);
            });
        }


        private void ExecuteOnSyncContext(Action action)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                action();
            }
            else
            {
                _synchronizationContext.Send(_ => action(), null);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            ExecuteOnSyncContext(() => base.InsertItem(index, item));
        }

        protected override void RemoveItem(int index)
        {
            ExecuteOnSyncContext(() => base.RemoveItem(index));
        }

        protected override void SetItem(int index, T item)
        {
            ExecuteOnSyncContext(() => base.SetItem(index, item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            ExecuteOnSyncContext(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void ClearItems()
        {
            ExecuteOnSyncContext(() => base.ClearItems());
        }
    }
}
