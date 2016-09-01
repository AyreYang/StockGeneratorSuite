using System;

namespace SGUtilities.RSI
{
    public class RSI
    {
        public int N1 { get; private set; }
        public int N2 { get; private set; }
        public int N3 { get; private set; }

        public decimal RSI1 { get; private set; }
        public decimal RSI2 { get; private set; }
        public decimal RSI3 { get; private set; }

        public decimal RSI1MaxEma { get; private set; }
        public decimal RSI1ABSEma { get; private set; }
        public decimal RSI2MaxEma { get; private set; }
        public decimal RSI2ABSEma { get; private set; }
        public decimal RSI3MaxEma { get; private set; }
        public decimal RSI3ABSEma { get; private set; }


        public decimal LRSI1MaxEma { get; private set; }
        public decimal LRSI1ABSEma { get; private set; }
        public decimal LRSI2MaxEma { get; private set; }
        public decimal LRSI2ABSEma { get; private set; }
        public decimal LRSI3MaxEma { get; private set; }
        public decimal LRSI3ABSEma { get; private set; }

        public decimal VALUE { get; private set; }

        public RSI(decimal value) : this(6, 12, 24, value, 0m, 0m, 0m, 0m, 0m, 0m) { }
        public RSI(int n1, int n2, int n3, decimal value) : this(n1, n2, n3, value, 0m, 0m, 0m, 0m, 0m, 0m) { }
        public RSI(decimal value, decimal max1, decimal abs1, decimal max2, decimal abs2, decimal max3, decimal abs3) : this(6, 12, 24, value, max1, abs1, max2, abs2, max3, abs3) { }
        public RSI(int n1, int n2, int n3, decimal value, decimal max1, decimal abs1, decimal max2, decimal abs2, decimal max3, decimal abs3)
        {
            if (n1 <= 0) throw new Exception("Initializing RSI(n1) parameter is invalid.");
            if (n2 <= 0) throw new Exception("Initializing RSI(n2) parameter is invalid.");
            if (n3 <= 0) throw new Exception("Initializing RSI(n3) parameter is invalid.");

            if (max1 < 0m) throw new Exception("Initializing RSI(max1) parameter is invalid.");
            if (abs1 < 0m) throw new Exception("Initializing RSI(abs1) parameter is invalid.");
            if (max2 < 0m) throw new Exception("Initializing RSI(max2) parameter is invalid.");
            if (abs2 < 0m) throw new Exception("Initializing RSI(abs2) parameter is invalid.");
            if (max3 < 0m) throw new Exception("Initializing RSI(max3) parameter is invalid.");
            if (abs3 < 0m) throw new Exception("Initializing RSI(abs3) parameter is invalid.");

            if (value <= 0m) throw new Exception("Initializing RSI(value) parameter is invalid.");

            this.N1 = n1;
            this.N2 = n2;
            this.N3 = n3;
            this.VALUE = value;
            this.RSI1 = decimal.Zero;
            this.RSI2 = decimal.Zero;
            this.RSI3 = decimal.Zero;

            this.RSI1MaxEma = max1;
            this.RSI1ABSEma = abs1;
            this.RSI2MaxEma = max2;
            this.RSI2ABSEma = abs2;
            this.RSI3MaxEma = max3;
            this.RSI3ABSEma = abs3;

            this.LRSI1MaxEma = decimal.Zero;
            this.LRSI1ABSEma = decimal.Zero;
            this.LRSI2MaxEma = decimal.Zero;
            this.LRSI2ABSEma = decimal.Zero;
            this.LRSI3MaxEma = decimal.Zero;
            this.LRSI3ABSEma = decimal.Zero;
        }

        public bool Add(decimal value)
        {
            if (value <= 0m) return false;

            Calculate(value);
            return true;
        }

        private void Calculate(decimal value)
        {
            var RMax = Math.Max(0m, value - VALUE);
            var RAbs = Math.Abs(value - VALUE);

            LRSI1MaxEma = RSI1MaxEma;
            LRSI1ABSEma = RSI1ABSEma;
            LRSI2MaxEma = RSI2MaxEma;
            LRSI2ABSEma = RSI2ABSEma;
            LRSI3MaxEma = RSI3MaxEma;
            LRSI3ABSEma = RSI3ABSEma;

            RSI1MaxEma = Math.Round((RMax + (N1 - 1) * LRSI1MaxEma) / N1, 4);
            RSI1ABSEma = Math.Round((RAbs + (N1 - 1) * LRSI1ABSEma) / N1, 4);
            RSI2MaxEma = Math.Round((RMax + (N2 - 1) * LRSI2MaxEma) / N2, 4);
            RSI2ABSEma = Math.Round((RAbs + (N2 - 1) * LRSI2ABSEma) / N2, 4);
            RSI3MaxEma = Math.Round((RMax + (N3 - 1) * LRSI3MaxEma) / N3, 4);
            RSI3ABSEma = Math.Round((RAbs + (N3 - 1) * LRSI3ABSEma) / N3, 4);


            if (RSI1ABSEma != decimal.Zero) RSI1 = Math.Round((RSI1MaxEma / RSI1ABSEma) * 100m, 4);
            if (RSI2ABSEma != decimal.Zero) RSI2 = Math.Round((RSI2MaxEma / RSI2ABSEma) * 100m, 4);
            if (RSI3ABSEma != decimal.Zero) RSI3 = Math.Round((RSI3MaxEma / RSI3ABSEma) * 100m, 4);

            VALUE = value;
        }
    }
}
