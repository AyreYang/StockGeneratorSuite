using System;
using System.Collections.Generic;
using System.Linq;
using SGUtilities.Averager;

namespace SGUtilities.Lines
{
    public enum LineType
    {
        MINUTE,
        HOUR,
        DAILY,
        WEEKLY
    }
    public enum LINE
    {
        HOUR5 = 5,
        HOUR10 = 10,
        HOUR30 = 30,

        DAILY5 = 20,
        DAILY10 = 40,
        DAILY30 = 120,

        WEEKLY5 = 100,
        WEEKLY10 = 200,
        WEEKLY30 = 600
    }
    /*
    public abstract class TimeLine : IDisposable
    {
        protected Dictionary<AverageType, AverageValue> Lines { get; set; }
        protected List<KeyValuePair<DateTime, decimal>> Paires { get; set; }

        protected DateTime From { get; set; }
        protected DateTime To { get; set; }
        public bool IsReady { get; private set; }

        protected abstract void ResetTime(DateTime time);

        public TimeLine(int percision = 2)
        {
            IsReady = false;

            Lines = new Dictionary<AverageType, AverageValue>();
            Paires = new List<KeyValuePair<DateTime, decimal>>();

            From = DateTime.MinValue;
            To = DateTime.MinValue;
        }
        public TimeLine(DateTime time, int percision = 2)
            : this(percision)
        {
            Initialize(time);
        }

        public bool Initialize(DateTime time)
        {
            IsReady = false;
            From = DateTime.MinValue;
            To = DateTime.MinValue;

            if (DateTime.MinValue.Equals(time) || DateTime.MaxValue.Equals(time)) return false;
            ResetTime(time);
            IsReady = true;

            return IsReady;
        }
        
        public bool Add(KeyValuePair<DateTime, decimal> paire)
        {
            if (!IsReady) return false;
            if (paire.Key >= From && paire.Key < To)
            {
                if (Paires.Any(ent => ent.Key.Equals(paire.Key) && ent.Value.Equals(paire.Value))) return false;
                //Counter.Add(paire.Value);
                Paires.Add(paire);
                return true;
            }
            else
            {
                if (paire.Key < From) return false;

                if (Paires.Count > 0) Lines.Values.ToList().ForEach(line => line.Add(Paires[Paires.Count - 1].Value));
                Paires.Clear();
                
                ResetTime(paire.Key);

                return Add(paire);
            }
        }
        public void AddLine(AverageType id, AverageValue liner)
        {
            if (id == AverageType.AVERAGE_NONE || liner == null) return;
            if (Lines.ContainsKey(id)) return;
            Lines.Add(id, liner);
        }
        public decimal? AverageOf(AverageType id)
        {
            if (!IsReady) return null;
            if (!Lines.ContainsKey(id)) return null;
            return (Paires.Count <= 0) ? Lines[id].Average : Lines[id].TryAdd(Paires[Paires.Count - 1].Value);
        }

        public void Dispose()
        {
            foreach (AverageType key in Lines.Keys) Lines[key].Dispose();
            Paires.Clear();
        }

        ~TimeLine()
        {
            Dispose();
        }
    }
    */
    public abstract class TimeLine<T> : IDisposable where T : new()
    {
        public List<AverageLine> Lines { get; private set; }
        //protected List<T> Values { get; set; }
        public T Value { get; protected set; }
        //public bool HasContent
        //{
        //    get { return Values.Count > 0; }
        //}

        protected DateTime From { get; set; }
        protected DateTime To { get; set; }
        public bool IsReady { get; private set; }

        private Func<T, DateTime> _time = null;
        private Func<T, DateTime> FuncTime
        {
            get { return _time; }
            set
            {
                if (value == null)
                {
                    _time = (o) =>
                    {
                        var time = DateTime.Now;
                        try
                        {
                            time = Convert.ToDateTime(o);
                        }
                        catch { }
                        return time;
                    };
                }
                else
                {
                    _time = value;
                }
            }
        }
        private Func<T, decimal> _value = null;
        private Func<T, decimal> FuncValue
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null)
                {
                    _value = (o) =>
                    {
                        var val = decimal.Zero;
                        try
                        {
                            val = Convert.ToDecimal(o);
                        }
                        catch { }
                        return val;
                    };
                }
                else
                {
                    _value = value;
                }
            }
        }
        protected Action<TimeLine<T>> EvntSwitch { get; set; }

        private int Percision { get; set; }

        protected abstract void ResetTime(DateTime time);

        //public decimal? Open
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return null;
        //        return FuncValue(Values[0]);
        //    }
        //}
        //public decimal? Close
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return null;
        //        return FuncValue(Values[Values.Count - 1]);
        //    }
        //}
        //public decimal? High
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return null;
        //        return Values.Max(v => FuncValue(v));
        //    }
        //}
        //public decimal? Low
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return null;
        //        return Values.Min(v => FuncValue(v));
        //    }
        //}
        //public DateTime? OpenTime
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return null;
        //        return Values.Min(v => FuncTime(v));
        //    }
        //}
        //public DateTime? CloseTime
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return null;
        //        return Values.Max(v => FuncTime(v));
        //    }
        //}

        public TimeLine(int percision, DateTime? time, Func<T, DateTime> gettime, Func<T, decimal> getValue, Action<TimeLine<T>> evnt)
        {
            IsReady = false;

            Lines = new List<AverageLine>();
            //Values = new List<T>();
            Value = default(T);

            From = DateTime.MinValue;
            To = DateTime.MinValue;
            FuncTime = gettime;
            FuncValue = getValue;
            EvntSwitch = evnt;

            Percision = (percision <= 0) ? 0 : percision;
            if (time.HasValue) Initialize(time.Value);

        }
        public TimeLine() : this(0, null, null, null, null) { }
        public TimeLine(int percision) : this(percision, null, null, null, null) { }
        public TimeLine(DateTime time) : this(0, time, null, null, null) { }
        public TimeLine(int percision, DateTime time) : this(percision, time, null, null, null) { }
        public TimeLine(Func<T, DateTime> gettime, Func<T, decimal> getValue, Action<TimeLine<T>> evnt) : this(0, null, gettime, getValue, evnt) { }
        public TimeLine(int percision, Func<T, DateTime> gettime, Func<T, decimal> getValue, Action<TimeLine<T>> evnt) : this(percision, null, gettime, getValue, evnt) { }

        public bool Initialize(DateTime time)
        {
            IsReady = false;
            From = DateTime.MinValue;
            To = DateTime.MinValue;

            if (DateTime.MinValue.Equals(time) || DateTime.MaxValue.Equals(time)) return false;
            ResetTime(time);
            IsReady = true;

            return IsReady;
        }
        //public void ToComplete()
        //{
        //    if (HasContent)
        //    {
        //        if (Lines.Count > 0) Lines.Values.ToList().ForEach(line => line.Add(this.Close.Value));
        //        if (EvntSwitch != null) EvntSwitch(this);
        //        Values.Clear();
        //    }
        //}

        public virtual void Add(T val)
        {
            if (!IsReady) return;
            var time = FuncTime(val);
            var value = FuncValue(val);
            if (time >= From && time < To)
            {
                //if (Values.Any(ent => ent.Equals(val))) return false;
                ////Counter.Add(paire.Value);
                //Values.Add(val);
                Value = val;
                if (Lines.Count > 0) Lines.ForEach(line => line.TryAdd(value));
            }
            else if(time >= To)
            {
                //ToComplete();
                if (EvntSwitch != null) EvntSwitch(this);
                if (Lines.Count > 0) Lines.ForEach(line => line.Add(value));
                ResetTime(time);

                //return Add(val);
            }
        }
        public void AddLine(AverageLine line)
        {
            if (line != null && !Lines.Any(l => l.ID.Equals(line.ID))) Lines.Add(line);
        }
        //public decimal? AverageOf(AverageType id)
        //{
        //    if (!IsReady) return null;
        //    if (!Lines.ContainsKey(id)) return null;
        //    return (Values.Count <= 0) ? Lines[id].Average : Lines[id].TryAdd(this.Close.Value);
        //}
        //public List<KeyValuePair<AverageType, decimal>> AveragesOf()
        //{
        //    var list = new List<KeyValuePair<AverageType, decimal>>();
        //    Lines.Keys.ToList().ForEach(k =>
        //    {
        //        var value = AverageOf(k);
        //        if (value.HasValue) list.Add(new KeyValuePair<AverageType, decimal>(k, value.Value));
        //    });
        //    return list;
        //}

        public void Clear()
        {
            Lines.ForEach(l => l.Clear());
        }

        public void Dispose()
        {
            Lines.ForEach(l => l.Dispose());
        }

        ~TimeLine()
        {
            Dispose();
        }
    }

}
