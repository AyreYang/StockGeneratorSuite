using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGUtilities.Lines
{
    /*
    public class HourLine : TimeLine
    {
        public HourLine(int percision = 2) : base(percision) { }
        public HourLine(DateTime time, int percision = 2) : base(time, percision) { }
        protected override void ResetTime(DateTime time)
        {
            DateTime[] Time0900_1030 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 9, 0, 0), new DateTime(time.Year, time.Month, time.Day, 10, 30, 0) };
            DateTime[] Time1030_1230 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 10, 30, 0), new DateTime(time.Year, time.Month, time.Day, 12, 30, 0) };
            DateTime[] Time1230_1400 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 12, 30, 0), new DateTime(time.Year, time.Month, time.Day, 14, 0, 0) };
            DateTime[] Time1400_1530 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 14, 0, 0), new DateTime(time.Year, time.Month, time.Day, 15, 30, 0) };

            DateTime[] temp = null;
            if (time < Time0900_1030[1]) temp = Time0900_1030;
            if (time >= Time1400_1530[0]) temp = Time1400_1530;
            if (time >= Time1030_1230[0] && time < Time1030_1230[1]) temp = Time1030_1230;
            if (time >= Time1230_1400[0] && time < Time1230_1400[1]) temp = Time1230_1400;

            if (temp != null)
            {
                this.From = temp[0];
                this.To = temp[1];
            }
        }
    }*/

    public class HourLine<T> : TimeLine<T> where T : new()
    {
        public HourLine(int percision = 2) : base(percision) { }
        public HourLine(DateTime time, int percision = 2) : base(percision, time, null, null, null) { }
        public HourLine(Func<T, DateTime> gettime, Func<T, decimal> getValue, Action<TimeLine<T>> evnt, int percision = 2) : base(percision, null, gettime, getValue, evnt) { }
        protected override void ResetTime(DateTime time)
        {
            DateTime[] Time0900_1030 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 9, 0, 0), new DateTime(time.Year, time.Month, time.Day, 10, 30, 0) };
            DateTime[] Time1030_1230 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 10, 30, 0), new DateTime(time.Year, time.Month, time.Day, 12, 30, 0) };
            DateTime[] Time1230_1400 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 12, 30, 0), new DateTime(time.Year, time.Month, time.Day, 14, 0, 0) };
            DateTime[] Time1400_1530 = new DateTime[] { new DateTime(time.Year, time.Month, time.Day, 14, 0, 0), new DateTime(time.Year, time.Month, time.Day, 15, 30, 0) };

            DateTime[] temp = null;
            if (time < Time0900_1030[1]) temp = Time0900_1030;
            if (time >= Time1400_1530[0]) temp = Time1400_1530;
            if (time >= Time1030_1230[0] && time < Time1030_1230[1]) temp = Time1030_1230;
            if (time >= Time1230_1400[0] && time < Time1230_1400[1]) temp = Time1230_1400;

            if (temp != null)
            {
                this.From = temp[0];
                this.To = temp[1];
            }
        }
    }
}
