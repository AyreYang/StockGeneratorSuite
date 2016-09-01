using System;
using System.Collections.Generic;
using System.Linq;

namespace SGUtilities.Cache
{
    public class LinearList<T> : List<T>
    {
        private Func<T, decimal> FValue { get; set; }

        public int Orietion
        {
            get
            {
                var slope = Slope;
                return (slope == decimal.Zero) ? 0 : (slope > decimal.Zero) ? 1 : -1;
            }
        }
        public decimal Slope
        {
            get
            {
                if (this.Count <= 0) return decimal.Zero;

                var max = this.Max(t=>FValue(t));
                var min = this.Min(t => FValue(t));
                var imax1 = this.IndexOf(t=> max.Equals(FValue(t)));
                var imax2 = this.LastIndexOf(t => max.Equals(FValue(t)));
                var imin1 = this.IndexOf(t => min.Equals(FValue(t)));
                var imin2 = this.LastIndexOf(t => min.Equals(FValue(t)));

                var imin = imin1 + imin2;
                var imax = imax1 + imax2;
                var orietion = (imin == imax) ? 0 : ((imax - imin) / Math.Abs(imax - imin));
                switch (orietion)
                {
                    case 0:
                        return decimal.Zero;
                    case 1:
                        imin = imin1;
                        imax = imax2;
                        break;
                    case -1:
                        imin = imin2;
                        imax = imax1;
                        break;
                }

                decimal dmax = Convert.ToDecimal(max);
                decimal dmin = Convert.ToDecimal(min);
                //var tan = Math.Round(Math.Abs(dmax - dmin) / Math.Abs(imax - imin), 6) * orietion;
                var tan = this.Count == 0 ? decimal.Zero : Math.Round(Math.Abs(dmax - dmin) / this.Count, 6) * orietion;
                return tan;
            }
        }

        public LinearList() : this(null) { }
        public LinearList(Func<T, decimal> func)
        {
            FValue = (func != null) ? func : (t) =>
            {
                var value = decimal.Zero;
                try
                {
                    value = Convert.ToDecimal(t);
                }
                catch { }
                return value;
            };
        }

        public int IndexOf(Func<T, bool> func)
        {
            if (func == null) return -1;
            var index = -1;
            for (int i = 0; i < this.Count; i++)
            {
                if (func(this[i]))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        public int LastIndexOf(Func<T, bool> func)
        {
            if (func == null) return -1;
            var index = -1;
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (func(this[i]))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
