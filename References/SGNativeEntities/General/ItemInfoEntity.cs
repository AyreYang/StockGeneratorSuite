using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGNativeEntities.Enums;

namespace SGNativeEntities.General
{
    public class ItemInfoEntity
    {
        public string Code { get; private set; }
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
        public decimal Amount { get; set; }
        public decimal Money { get; set; }
        public CODETYPE CodeType { get; set; }
        public TRADE Type { get; set; }

        public ItemInfoEntity() : this(string.Empty) { }
        public ItemInfoEntity(string code)
        {
            Code = code;

            Time = DateTime.MinValue;
            Value = 0m;
            Amount = 0m;
            Money = 0m;
            Type = TRADE.none;
        }

        public override bool Equals(object obj)
        {
            var entity = obj as ItemInfoEntity;
            if (entity == null) return false;
            return Code.Equals(entity.Code) &&
                Time.Equals(entity.Time) &&
                Value.Equals(entity.Value) &&
                Amount.Equals(entity.Amount) &&
                Money.Equals(entity.Money) &&
                CodeType.Equals(entity.CodeType) &&
                Type.Equals(entity.Type);
        }
    }
}
