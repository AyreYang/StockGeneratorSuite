using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGUtilities.Lines
{
    /*
    public class WeeklyLine : TimeLine
    {
        public WeeklyLine(int percision = 2) : base(percision) { }
        public WeeklyLine(DateTime time, int percision = 2) : base(time, percision) { }

        protected override void ResetTime(DateTime time)
        {
            var temp = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
            int week = (int)temp.DayOfWeek;
            if(week == 0) week = 7;

            this.From = temp.AddDays((week - 1) * -1);
            this.To = this.From.AddDays(7);
        }
    }*/

    public class WeeklyLine<T> : TimeLine<T> where T : new()
    {
        public WeeklyLine(int percision = 2) : base(percision) { }
        public WeeklyLine(DateTime time, int percision = 2) : base(percision, time, null, null, null) { }
        public WeeklyLine(Func<T, DateTime> gettime, Func<T, decimal> getValue, Action<TimeLine<T>> evnt, int percision = 2) : base(percision, null, gettime, getValue, evnt) { }

        protected override void ResetTime(DateTime time)
        {
            var temp = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
            int week = (int)temp.DayOfWeek;
            if (week == 0) week = 7;

            this.From = temp.AddDays((week - 1) * -1);
            this.To = this.From.AddDays(7);
        }
    }
}
