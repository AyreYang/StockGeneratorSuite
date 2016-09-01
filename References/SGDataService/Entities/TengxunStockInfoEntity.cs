using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGNativeEntities.General;
using SGNativeEntities.Enums;

namespace SGDataService.Entities
{
    public class TengxunStockInfoEntity : StockInfoEntity
    {
        public List<ItemInfoEntity> TradeList { get; private set; }

        public TengxunStockInfoEntity() : this(string.Empty) { }
        public TengxunStockInfoEntity(string code)
            : base(code)
        {
            TradeList = new List<ItemInfoEntity>();
        }
    }

    public class TradeSort : IComparer<ItemInfoEntity>
    {
        public int Compare(ItemInfoEntity x, ItemInfoEntity y)
        {
            return x.Time == y.Time ? 0 : x.Time > y.Time ? 1 : -1;
        }
    }
}
