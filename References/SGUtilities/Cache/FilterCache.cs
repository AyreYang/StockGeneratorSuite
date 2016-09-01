using System.Collections.Generic;
using System.Linq;
using System;

namespace SGUtilities.Cache
{
    public class FilterCache<T> : IDisposable
    {
        private const int DEFAULT_CAPACITY = 100;
        private int Index { get; set; }
        private int Capacity { get; set; }
        private List<T> Cache { get; set; }

        public int Count
        {
            get
            {
                return Cache.Count;
            }
        }

        public FilterCache(int capacity)
        {
            Index = 0;
            Cache = new List<T>();
            Capacity = (capacity > 0) ? capacity : DEFAULT_CAPACITY;
        }

        public void Clear()
        {
            Cache.Clear();
            Index = 0;
        }

        public bool Add(T value)
        {
            var existed = false;
            if (value == null)
            {
                existed = Cache.Any(data => data == null);
            }
            else
            {
                existed = Cache.Any(data => data.Equals(value));
            }

            if (!existed)
            {
                if (Cache.Count < Capacity)
                {
                    Cache.Add(value);
                }
                else
                {
                    Cache[Index++] = value;
                    Index = Index % Cache.Count;
                }
            }

            return !existed;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
