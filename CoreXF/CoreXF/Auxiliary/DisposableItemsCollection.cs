
using System;
using System.Collections;
using System.Collections.Generic;


namespace CoreXF
{
    public class DisposableItemsCollection : IEnumerable<IDisposable>, IDisposable
    {
        public List<IDisposable> Items;

        bool _disposed;
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i]?.Dispose();
            }
            Items.Clear();
            Items = null;
        }

        public IEnumerator<IDisposable> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        public static DisposableItemsCollection operator +(DisposableItemsCollection x, IDisposable y)
        {
            if (x.Items == null)
            {
                x.Items = new List<IDisposable>();
            }
            x.Items.Add(y);
            return x;
        }
    }
}
