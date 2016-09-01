using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGNativeEntities.General;
using SGNativeEntities.Enums;

namespace SGDataService.Entities
{
    public class TengxunIndexInfoEntity : IndexInfoEntity
    {
        public List<ItemInfoEntity> IndexList { get; private set; }

        public TengxunIndexInfoEntity() : this(string.Empty) { }
        public TengxunIndexInfoEntity(string code)
            : base(code)
        {
            IndexList = new List<ItemInfoEntity>();
        }
    }

    public class IndexSort : IComparer<ItemInfoEntity>
    {
        public int Compare(ItemInfoEntity x, ItemInfoEntity y)
        {
            return x.Time == y.Time ? 0 : x.Time > y.Time ? 1 : -1;
        }
    }
}
