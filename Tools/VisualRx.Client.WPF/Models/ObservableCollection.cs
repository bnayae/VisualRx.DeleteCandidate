using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VisualRx.Client.WPF
{
    public class ObservableCollection<T> : IList<T>, INotifyCollectionChanged
            where T : class
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void RaisCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged == null)
                return;
            Task.Factory.StartNew(() =>
            {
                CollectionChanged?.Invoke(this, args);
            }, CancellationToken.None, TaskCreationOptions.None, App.UITaskScheduler);
        }

        ImmutableList<T> _source = ImmutableList<T>.Empty;

        public T this[int index]
        {
            get { return _source[index]; }
            set { _source = _source.Replace(_source[index], value); }
        }

        public int Count => _source.Count;

        public bool IsReadOnly=>false;

        public void Add(T item)
        {
            _source = _source.Add(item);
            RaisCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _source = ImmutableList<T>.Empty;
        }

        public bool Contains(T item)
        {
            return _source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.AsEnumerable().GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _source.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _source = _source.Insert(index, item);
        }

        public bool Remove(T item)
        {
            _source = _source.Remove(item);
            return true;
        }

        public void RemoveAt(int index)
        {
            _source = _source.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.AsEnumerable().GetEnumerator();
        }
    }
}
