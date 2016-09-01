using System;
using System.Collections.Generic;

namespace SGUtilities.Cache
{
    public class SegCache<L,T> : IDisposable where L : List<T>, new()
    {
        private const int DEF_SEG = 2;

        public int Index { get; private set; }
        private L[] Cache { get; set; }
        public L Segment
        {
            get
            {
                return Cache[Index];
            }
        }
        private T Value { get; set; }

        private Func<T, T, L, bool> FuncSwitch1 { get; set; }
        private Func<L, L, bool> FuncSwitch2 { get; set; }

        public SegCache(int seg, Func<T, T, L, bool> swtch1, Func<L, L, bool> swtch2)
        {
            Cache = new L[(seg < DEF_SEG) ? DEF_SEG : seg];
            for (int i = 0; i < Cache.Length; i++) Cache[i] = new L();

            FuncSwitch1 = (swtch1 != null) ? swtch1 : null;
            FuncSwitch2 = (swtch2 != null) ? swtch2 : null;
            Value = default(T);
            Index = 0;
        }
        public SegCache(L[] caches, Func<T, T, L, bool> swtch1, Func<L, L, bool> swtch2)
        {
            if (caches == null || caches.Length < DEF_SEG) throw new Exception(string.Format("Caches is empty or the count is less than limited({0})", DEF_SEG));
            Cache = caches;

            FuncSwitch1 = (swtch1 != null) ? swtch1 : null;
            FuncSwitch2 = (swtch2 != null) ? swtch2 : null;
            Value = default(T);
            Index = 0;
        }

        public bool Add(T value, out L segment)
        {
            segment = default(L);
            var OIndex = Index;
            Index = (FuncSwitch1 != null && FuncSwitch1(this.Value, value, this.Segment)) ? (Index + 1) % Cache.Length : Index;
            var switched = (OIndex != Index);
            if (switched) Cache[Index].Clear();
            if (switched) segment = Cache[OIndex];
            Cache[Index].Add(value);
            Value = value;

            return switched;
        }

        public bool Add(L values, out L segment)
        {
            segment = default(L);
            if (values == null || values.Count <= 0) return false;
            
            var OIndex = Index;
            Index = (FuncSwitch2 != null && FuncSwitch2(values, this.Segment)) ? (Index + 1) % Cache.Length : Index;
            var switched = (OIndex != Index);
            if (switched) Cache[Index].Clear();
            if (switched) segment = Cache[OIndex];
            Cache[Index].AddRange(values);
            Value = values[values.Count - 1];

            return switched;
        }

        public void Dispose()
        {
            foreach (L cache in Cache) cache.Clear();
        }
    }
}
