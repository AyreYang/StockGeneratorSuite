using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGUtilities.Lines
{
    public class MinuteLine<T> : TimeLine<T> where T : new()
    {
        public MinuteLine(int percision = 2) : base(percision) { }
        public MinuteLine(DateTime time, int percision = 2) : base(percision, time, null, null, null) { }
        public MinuteLine(Func<T, DateTime> gettime, Func<T, decimal> getValue, Action<TimeLine<T>> evnt, int percision = 2) : base(percision, null, gettime, getValue, evnt) { }
        protected override void ResetTime(DateTime time)
        {
            this.From = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);
            this.To = this.From.AddMinutes(1);
        }
    }
}
