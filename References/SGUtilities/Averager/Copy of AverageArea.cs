using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGUtilities.Lines;

namespace SGUtilities.Averager
{
    public enum AreaType
    {
        None,
        Resistance,
        Sustentation
    }

    public class AverageArea : AverageValue
    {
        private const decimal AreaHight = 0.01m;
        private const decimal AreaHight1 = 0.618m;
        private const decimal AreaHight2 = 0.382m;

        public decimal UpperBorder
        {
            get
            {
                var value = decimal.Zero;
                var area = Math.Round(Value * AreaHight, 2);
                switch (Type)
                {
                    case AreaType.None:
                        value = Math.Round(Value + (area * AreaHight1), 2);
                        break;
                    case AreaType.Resistance:
                        value = Math.Round(Value + (area * AreaHight2), 2);
                        break;
                    case AreaType.Sustentation:
                        value = Math.Round(Value + (area * AreaHight1), 2);
                        break;
                }
                return value;
            }
        }
        public decimal LowerBorder
        {
            get
            {
                var value = decimal.Zero;
                var area = Math.Round(Value * AreaHight, 2);
                switch (Type)
                {
                    case AreaType.None:
                        value = Math.Round(Value - (area * AreaHight1), 2);
                        break;
                    case AreaType.Resistance:
                        value = Math.Round(Value - (area * AreaHight1), 2);
                        break;
                    case AreaType.Sustentation:
                        value = Math.Round(Value - (area * AreaHight2), 2);
                        break;
                }
                return value;
            }
        }
        public decimal Value
        {
            get
            {
                return this.Average;
            }
        }
        public AreaType Type { get; private set; }

        public AverageArea(int count, int precision):base(count, precision)
        {
            Type = AreaType.None;
        }

        public static bool Is2AreasOverlapped(AverageArea area1, AverageArea area2)
        {
            if (area1 == null || area2 == null) return false;
            if (area1.Value == area2.Value) return true;

            AverageArea upper = null;
            AverageArea lower = null;

            if (area1.Value > area2.Value)
            {
                upper = area1;
                lower = area2;
            }
            else
            {
                upper = area2;
                lower = area1;
            }

            var overlap = lower.UpperBorder - upper.LowerBorder;
            if (overlap <= decimal.Zero) return false;

            var max = Math.Max(upper.UpperBorder, lower.UpperBorder);
            var min = Math.Min(upper.LowerBorder, lower.LowerBorder);
            var cap = max - min;

            if (cap == decimal.Zero) return false;

            var proportion = Math.Round((overlap / cap), 3);
            return proportion >= 0.618m;
        }

        public static List<KeyValuePair<LINE, AverageArea>> Merge(List<KeyValuePair<LINE, AverageArea>> areas)
        {
            var list = new List<KeyValuePair<LINE, AverageArea>>();
            if (areas != null && areas.Count > 0)
            {
                if (areas.Count == 1)
                {
                    list.Add(areas[0]);
                }
                else
                {
                    KeyValuePair<LINE, AverageArea>? area1 = null;
                    KeyValuePair<LINE, AverageArea>? area2 = null;
                    do
                    {
                        if (area1 == null)
                        {
                            area1 = areas[0];
                            areas.RemoveAt(0);
                        }
                        if (area2 == null)
                        {
                            area2 = areas[0];
                            areas.RemoveAt(0);
                        }

                        if (Is2AreasOverlapped(area1.Value.Value, area2.Value.Value))
                        {
                            area1 = (area1.Value.Key > area2.Value.Key) ? area1 : area2;
                        }
                        else
                        {
                            list.Add(area1.Value);
                            area1 = area2;
                        }
                        area2 = null;

                    } while (areas.Count > 0);

                    if (area1.HasValue) list.Add(area1.Value);
                }
            }
            return list;
        }
        public static bool IsValueBetween(decimal value, AverageArea area1, AverageArea area2)
        {
            if (area1 == null || area2 == null) return false;

            decimal max = 0m;
            decimal min = 0m;
            if (Is2AreasOverlapped(area1, area2))
            {
                max = Math.Max(area1.UpperBorder, area2.UpperBorder);
                min = Math.Min(area1.LowerBorder, area2.LowerBorder);
            }
            else
            {
                AverageArea upper = null;
                AverageArea lower = null;

                if (area1.Value > area2.Value)
                {
                    upper = area1;
                    lower = area2;
                }
                else
                {
                    upper = area2;
                    lower = area1;
                }

                max = upper.UpperBorder;
                min = lower.LowerBorder;
            }

            return (value >= min) && (value <= max);
        }
    }
}
