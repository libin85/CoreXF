
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CoreXF
{
    public abstract class GenericCache<TKey, TValue> where TValue : class
    {
        protected Dictionary<TKey, TValue> Dictionary = new Dictionary<TKey, TValue>();

        public TValue GetOrCreateValue(TKey key)
        {
            TValue wref;
            if (!Dictionary.TryGetValue(key, out wref))
            {
                wref = CreateValue(key);
                Dictionary.Add(key,wref);
            };
            return wref;
        }

        public TValue GetValue(TKey key)
        {
            TValue wref;
            if (!Dictionary.TryGetValue(key, out wref))
            {
                return default(TValue);
            };
            return wref;
        }

        public void AddValue(TKey key, TValue value)
        {
            if (Dictionary.ContainsKey(key))
            {
                return;
            }

            Dictionary.Add(key, value);
        }

        public abstract TValue CreateValue(TKey key);

        public virtual void Clear()
        {
            Dictionary.Clear();
        }
    }

    public class GenericWeakCache<TKey, TValue> where TValue : class
    {
        public ConcurrentDictionary<TKey, WeakReference<TValue>> Dictionary = new ConcurrentDictionary<TKey, WeakReference<TValue>>();

        public void Add(TKey key, TValue value)
        {
            if (Dictionary.ContainsKey(key)) throw new Exception($"Key {key} already exists in the dictionary");
            Dictionary.TryAdd(key, new WeakReference<TValue>(value));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = null;
            WeakReference<TValue> wref;
            if (!Dictionary.TryGetValue(key, out wref)) return false;

            TValue vref;
            if (!wref.TryGetTarget(out vref))
            {
                Dictionary.TryRemove(key, out wref);
                return false;
            }
            value = vref;
            return true;
        }

        public TValue GetValue(TKey key)
        {
            TValue vref;
            if (!TryGetValue(key, out vref)) return default(TValue);
            return vref;
        }

        public void Clear()
        {
            Dictionary.Clear();
        }
    }
}
