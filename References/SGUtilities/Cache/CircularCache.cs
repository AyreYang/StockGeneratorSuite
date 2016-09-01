using System;
using System.Collections.Generic;

namespace SGUtilities.Cache
{
    public class CircularCache<T> : IDisposable
    {
        public int Index { get; private set; }
        public int Capacity { get; private set; }
        protected List<T> Cache { get; private set; }
        public T LastValue
        {
            get
            {
                var index = (Index + 1) % Capacity;
                var value = (index >= Cache.Count) ? (Cache.Count > 0) ? Cache[0] : default(T) : Cache[index];
                return value;
            }
        }
        //private int FirstIndex
        //{
        //    get
        //    {
        //        return Cache.Count <= 0 ? -1 : (Index + 1) % Cache.Count;
        //    }
        //}
        //private int LastIndex
        //{
        //    get
        //    {
        //        return Cache.Count <= 0 ? -1 : (Cache.Count < Capacity) ? Index : (Cache.Count - 1) + FirstIndex;
        //    }
        //}

        public CircularCache(int capacity)
        {
            Capacity = (capacity <= 0) ? int.MaxValue : capacity;
            Index = -1;

            Cache = new List<T>();
        }

        public virtual void Add(T value)
        {
            T replaced = default(T);
            Add(value, out replaced);
        }
        public virtual bool Add(T value, out T replaced)
        {
            replaced = default(T);
            Index = (++Index % Capacity);
            if (Index >= Cache.Count)
            {
                Cache.Add(value);
                return false;
            }
            else
            {
                replaced = Cache[Index];
                Cache[Index] = value;
                return true;
            }
        }

        public virtual List<T> Export()
        {
            return Export(0, 0);
        }
        public virtual List<T> Export(int start, int count)
        {
            var list = new List<T>();
            if (Cache.Count > 0 && start >= 0 && start < Capacity)
            {
                var si = (Index + 1) % Cache.Count;
                var ei = (Cache.Count < Capacity) ? Index : (Cache.Count - 1) + si;
                var index = si + start;
                var flag = count == 0 ? 0 : count / Math.Abs(count);
                switch (flag)
                {
                    case 0:
                    case 1:
                        while (index <= ei)
                        {
                            var idx = index++ % Capacity;
                            if (idx >= Cache.Count) continue;
                            list.Add(Cache[idx]);
                            if (list.Count == Math.Abs(count)) break;
                        }
                        break;
                    case -1:
                        while (index >= si)
                        {
                            var idx = index-- % Capacity;
                            if (idx >= Cache.Count) continue;
                            list.Add(Cache[idx]);
                            if (list.Count == Math.Abs(count)) break;
                        }
                        break;
                }
            }
            return list;
        }
        public virtual List<T> Export(int count)
        {
            return Export((count < 0) ? Capacity - 1 : 0, count);
        }

        public virtual void Clear()
        {
            Cache.Clear();
        }

        public void Dispose()
        {
            Cache.Clear();
        }

        ~CircularCache()
        {
            Dispose();
        }
    }
}
