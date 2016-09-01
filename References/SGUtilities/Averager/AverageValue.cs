using System;
using SGUtilities.Cache;

namespace SGUtilities.Averager
{
    public enum AverageType
    {
        AT5 = 5,
        AT10 = 10,
        AT30 = 30
    }

    public class AverageValue : CircularCache<decimal>
    {
        public decimal TotalValue { get; private set; }
        public int Decimal { get; private set; }

        public MidpointRounding MidPointFlag { get; private set; }
        public int Count
        {
            get
            {
                return Cache.Count;
            }
        }
        public decimal Average
        {
            get
            {
                return (Count > 0) ? Math.Round(TotalValue / Count, Decimal, MidPointFlag) : 0m;
            }
        }
        public bool IsValid
        {
            get
            {
                return Count >= Capacity;
            }
        }

        public AverageValue() : this(0, 2) { }
        public AverageValue(int count) : this(count, 2) { }
        public AverageValue(int count, int precision, MidpointRounding flag = MidpointRounding.AwayFromZero)
            : base(count)
        {
            TotalValue = decimal.Zero;
            Decimal = precision;
            MidPointFlag = flag;
        }
        /*
        public static AverageValue Create(int ai_cnt)
        {
            return Create(ai_cnt, 0);
        }
        public static AverageValue Create(int ai_cnt, int ai_decimal, MidpointRounding amr_flag = MidpointRounding.AwayFromZero)
        {
            int li_cnt, li_decimal;
            li_cnt = (ai_cnt < 0) ? 0 : ai_cnt;
            li_decimal = (ai_decimal < 0) ? 0 : ai_decimal;
            li_decimal = (ai_decimal > 10) ? 10 : ai_decimal;
            return new AverageValue(li_cnt, li_decimal, amr_flag);
        }
        */
        public override void Add(decimal value)
        {
            var replaced = decimal.Zero;
            if(base.Add(value, out replaced))
            {
                TotalValue = TotalValue + value - replaced;
            }
            else
            {
                TotalValue = TotalValue + value;
            }
        }
        public virtual decimal TryAdd(decimal value)
        {
            var total = TotalValue + value - LastValue;
            var count = Count + 1;
            if(count > Capacity) count = Capacity;
            return Math.Round(total / count, Decimal, MidPointFlag);
        }

        public override void Clear()
        {
            base.Clear();
            TotalValue = decimal.Zero;
        }
    }
}
