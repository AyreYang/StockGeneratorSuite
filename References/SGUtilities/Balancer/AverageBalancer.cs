using System;
using System.Collections.Generic;
using SGUtilities.Averager;
using SGUtilities.Cache;

namespace SGUtilities.Balancer
{
    public class AverageBalancer
    {
        private enum WORK_TIME
        {
            NONE = 0,
            MORNING = 1,
            AFTERNOON = 2
        }

        //private AverageValue Averager { get; set; }
        //public decimal Average { get { return Averager.Average; } }
        //public List<LinearList<KeyValuePair<DateTime, decimal>>> Lines { get; private set; }
        private WORK_TIME mwt_time = WORK_TIME.NONE;
        private DateTime MDT_0000
        {
            get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0); }
        }
        private DateTime MDT_1200
        {
            get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 12, 0, 0); }
        }
        private DateTime MDT_2359
        {
            get { return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59); }
        }

        private KeyValuePair<DateTime, decimal>? Pair { get; set; }

        //private SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>> cache1 = null;
        ////private SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>> cache2 = null;

        private Func<KeyValuePair<DateTime, decimal>, decimal> fValue = (pair) => { return pair.Value; };
        private Func<KeyValuePair<DateTime, decimal>, KeyValuePair<DateTime, decimal>, LinearList<KeyValuePair<DateTime, decimal>>, bool> fSwitch1 = (v1, v2, segment) =>
            {
                switch (segment.Orietion)
                {
                    case 0:
                        return false;
                    case 1:
                        if (v2.Value >= v1.Value) return false;
                        break;
                    case -1:
                        if (v2.Value <= v1.Value) return false;
                        break;
                }
                return true;
            };
        private Func<LinearList<KeyValuePair<DateTime, decimal>>, LinearList<KeyValuePair<DateTime, decimal>>, bool> fSwitch2 = (values, segment) =>
            {
                if (segment.Orietion == 0 || segment.Orietion == values.Orietion) return false;
                return values.Count >= 10;
            };

        public AverageBalancer()
        {
            //this.Averager = AverageValue.Create(count, 2);
            //this.Lines = new List<LinearList<KeyValuePair<DateTime, decimal>>>();
            Reset();

            //cache1 = new SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>>(
            //                    new LinearList<KeyValuePair<DateTime, decimal>>[] { new LinearList<KeyValuePair<DateTime, decimal>>(fValue), new LinearList<KeyValuePair<DateTime, decimal>>(fValue) },
            //                    fSwitch1, null);
            ////cache2 = new SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>>(
            ////                    new LinearList<KeyValuePair<DateTime, decimal>>[] { new LinearList<KeyValuePair<DateTime, decimal>>(fValue), new LinearList<KeyValuePair<DateTime, decimal>>(fValue) },
            ////                    null, fSwitch2);
        }

        public void Reset()
        {
            Pair = null;
            mwt_time = WORK_TIME.NONE;
        }

        public List<KeyValuePair<DateTime, decimal>> Add(KeyValuePair<DateTime, decimal> pair)
        {

            WORK_TIME lwt_time = WORK_TIME.NONE;
            List<KeyValuePair<DateTime, decimal>> lst_pairs = new List<KeyValuePair<DateTime, decimal>>();
            if ((pair.Key >= MDT_0000 && pair.Key <= MDT_1200)) lwt_time = WORK_TIME.MORNING;
            if ((pair.Key > MDT_1200 && pair.Key <= MDT_2359)) lwt_time = WORK_TIME.AFTERNOON;


            if (lwt_time != WORK_TIME.NONE)
            {
                if (Pair == null || Pair.Value.Key.Date != pair.Key.Date)
                {
                    lst_pairs.Add(pair);
                }
                else
                {
                    var seconds = (mwt_time != lwt_time) ? 1 : (pair.Key - Pair.Value.Key).Seconds;
                    var time = Pair.Value.Key;
                    while (seconds > 0)
                    {
                        if (seconds == 1)
                        {
                            lst_pairs.Add(pair);
                        }
                        else
                        {
                            time = time.AddSeconds(1);
                            var value = Pair.Value.Value;
                            lst_pairs.Add(new KeyValuePair<DateTime, decimal>(time, value));
                        }
                        seconds--;
                    }
                }


            }
            Pair = lwt_time == WORK_TIME.NONE ? null : (KeyValuePair<DateTime, decimal>?)pair;
            mwt_time = lwt_time;

            //LinearList<KeyValuePair<DateTime, decimal>> line = null;
            //List<LinearList<KeyValuePair<DateTime, decimal>>> lines = new List<LinearList<KeyValuePair<DateTime, decimal>>>();
            //lst_pairs.ForEach(p =>
            //{
            //    Averager.Add(p.Value);
            //    if (cache1.Add(new KeyValuePair<DateTime, decimal>(p.Key, Averager.Average), out line))
            //    {
            //        Lines.Add(line);
            //        lines.Add(line);
            //    }
            //});
            return lst_pairs;
        }
    }
}
