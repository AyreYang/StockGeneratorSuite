using System;

namespace SGUtilities.Lines
{
    /*
    public class DailyLine:TimeLine
    {
        public DailyLine(int percision = 2) : base(percision) { }
        public DailyLine(DateTime time, int percision = 2) : base(time, percision) { }

        protected override void ResetTime(DateTime time)
        {
            this.From = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
            this.To = this.From.AddDays(1);
        }
    }*/

    public class DailyLine<T> : TimeLine<T> where T : new()
    {
        public DailyLine(int percision = 2) : base(percision) { }
        public DailyLine(DateTime time, int percision = 2) : base(percision, time, null, null, null) { }
        public DailyLine(Func<T, DateTime> gettime, Func<T, decimal> getValue, Action<TimeLine<T>> evnt, int percision = 2) : base(percision, null, gettime, getValue, evnt) { }

        protected override void ResetTime(DateTime time)
        {
            this.From = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
            this.To = this.From.AddDays(1);
        }
    }
}
