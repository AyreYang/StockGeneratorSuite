using System;

namespace SGUtilities.MACD
{
    // 12:26:9
    public class MACD
    {
        public static decimal FACTOR_EMA12_0 = Math.Round((2m / 13m), 4);
        public static decimal FACTOR_EMA26_0 = Math.Round((2m / 27m), 4);
        public static decimal FACTOR_DEA_0 = Math.Round((2m / 10m), 4);

        public static decimal FACTOR_EMA12_1 = Math.Round((11m / 13m), 4);
        public static decimal FACTOR_EMA26_1 = Math.Round((25m / 27m), 4);
        public static decimal FACTOR_DEA_1 = Math.Round((8m / 10m), 4);

        public decimal DIFF { get; private set; }
        public decimal DEA { get; private set; }
        public decimal BAR { get; private set; }
        public decimal VALUE { get; private set; }

        public bool IsInitial { get; private set; }

        public decimal LDEA { get; private set; }
        public decimal LEMA12 { get; private set; }
        public decimal LEMA26 { get; private set; }
        public decimal EMA12 { get; private set; }
        public decimal EMA26 { get; private set; }

        public MACD(decimal value)
        {
            if (value <= 0m) throw new Exception("Initializing MACD(value) parameter is invalid.");
            this.VALUE = value;

            this.DIFF = decimal.Zero;
            this.DEA = decimal.Zero;

            this.BAR = decimal.Zero;
            this.LEMA12 = decimal.Zero;
            this.LEMA26 = decimal.Zero;
            this.LDEA = decimal.Zero;
            this.EMA12 = decimal.Zero;
            this.EMA26 = decimal.Zero;

            this.IsInitial = true;
        }
        public MACD(decimal ema12, decimal ema26, decimal dea, decimal value)
        {
            if (ema12 <= 0m) throw new Exception("Initializing MACD(ema12) parameter is invalid.");
            if (ema26 <= 0m) throw new Exception("Initializing MACD(ema26) parameter is invalid.");
            if (dea <= 0m) throw new Exception("Initializing MACD(dea) parameter is invalid.");
            if (value <= 0m) throw new Exception("Initializing MACD(value) parameter is invalid.");
            this.EMA12 = ema12;
            this.EMA26 = ema26;
            this.DIFF = this.EMA12 - this.EMA26;
            this.DEA = dea;
            this.BAR = 2 * (DIFF - DEA);
            this.VALUE = value;

            this.LEMA12 = decimal.Zero;
            this.LEMA26 = decimal.Zero;
            this.LDEA = decimal.Zero;

            this.IsInitial = false;
        }

        public bool Add(decimal value)
        {
            if (value <= 0m) return false;

            if(IsInitial){
                IsInitial = false;
                Calculate0(value);
            }else{
                Calculate1(value);
            }
            return true;
        }

        // 首次计算
        private void Calculate0(decimal value)
        {
            //EMA（12）= 前日收盘价＋（今日收盘价 - 前日收盘价）×2/13
            EMA12 = Math.Round((VALUE + (value - VALUE) * FACTOR_EMA12_0), 4);
            //EMA（26）= 前日收盘价＋（今日收盘价 - 前日收盘价）×2/27
            EMA26 = Math.Round((VALUE + (value - VALUE) * FACTOR_EMA26_0), 4);

            //DIFF=今日EMA（12）- 今日EMA（26）
            DIFF = EMA12 - EMA26;
            //DEA（MACD）= 0＋今日DIFF×2/10
            DEA = Math.Round((0 + DIFF * FACTOR_DEA_0), 4);
            //BAR=2×(DIFF－DEA)
            BAR = Math.Round((2 * (DIFF - DEA)), 4);

            VALUE = value;
        }

        // 持续计算
        private void Calculate1(decimal value)
        {
            LEMA12 = EMA12;
            LEMA26 = EMA26;
            LDEA = DEA;

            //EMA（12）= 前一日EMA（12）×11/13＋今日收盘价×2/13
            EMA12 = Math.Round(((LEMA12 * FACTOR_EMA12_1) + (value * FACTOR_EMA12_0)), 4);
            //EMA（26）= 前一日EMA（26）×25/27＋今日收盘价×2/27
            EMA26 = Math.Round(((LEMA26 * FACTOR_EMA26_1) + (value * FACTOR_EMA26_0)), 4);

            //DIFF=今日EMA（12）- 今日EMA（26）
            DIFF = EMA12 - EMA26;
            //DEA（MACD）= 前一日DEA×8/10＋今日DIF×2/10
            DEA = Math.Round(((LDEA * FACTOR_DEA_1) + (DIFF * FACTOR_DEA_0)), 4);
            //BAR=2×(DIFF－DEA)
            BAR = Math.Round((2 * (DIFF - DEA)), 4);

            VALUE = value;
        }
    }
}
