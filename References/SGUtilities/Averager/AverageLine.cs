using System;
using System.Collections.Generic;
using SGUtilities.Shapes;
using SGUtilities.Lines;

namespace SGUtilities.Averager
{
    public enum LINETYPE
    {
        None,
        Resistance,
        Sustentation
    }

    public class EventArg
    {
        public Line Line { get; private set; }
        public LINE ID { get; private set; }
        public int Score { get; private set; }

        public LINETYPE Type { get; private set; }

        public decimal Value { get; private set; }
        public decimal UpperBorder { get; private set; }
        public decimal LowerBorder { get; private set; }

        public EventArg(AverageLine timeline, Line line)
        {
            if (timeline != null)
            {
                ID = timeline.ID;
                Score = timeline.Score;
                Type = timeline.Type;
                Value = timeline.Value;
                UpperBorder = timeline.UpperBorder;
                LowerBorder = timeline.LowerBorder;
            }
            Line = line;
        }

        public override string ToString()
        {
            return string.Format("AverageLine:[id={0},type={1},score={2}][value={3},upper={4},lower={5}] {6}", ID, Type, Score, Value, UpperBorder, LowerBorder, Line != null ? Line.ToString() : string.Empty);
        }
    }

    public class AverageLine : AverageValue
    {
        private const decimal TST_VALUE = 10m;
        private const decimal TST_HIGHT = 0.01m;
        private const decimal TST_HIGHT1 = 0.618m;
        private const decimal TST_HIGHT2 = 0.382m;

        private const decimal AreaHight = 0.01m;
        private const decimal AreaHight1 = 0.618m;
        private const decimal AreaHight2 = 0.382m;

        public int Score { get; private set; }

        public LINE ID { get; private set; }

        private decimal TempValue { get; set; }

        public decimal UpperBorder
        {
            get
            {
                var value = decimal.Zero;
                var hight = IsTest ? TST_HIGHT : AreaHight;
                var hight1 = IsTest ? TST_HIGHT1 : AreaHight1;
                var hight2 = IsTest ? TST_HIGHT2 : AreaHight2;

                var area = Math.Round(Value * hight, 2);
                switch (Type)
                {
                    case LINETYPE.None:
                        value = Math.Round(Value + (area * Math.Max(hight1, hight2)), 2);
                        break;
                    case LINETYPE.Resistance:
                        value = Math.Round(Value + (area * hight2), 2);
                        break;
                    case LINETYPE.Sustentation:
                        value = Math.Round(Value + (area * hight1), 2);
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
                var hight = IsTest ? TST_HIGHT : AreaHight;
                var hight1 = IsTest ? TST_HIGHT1 : AreaHight1;
                var hight2 = IsTest ? TST_HIGHT2 : AreaHight2;

                var area = Math.Round(Value * hight, 2);
                switch (Type)
                {
                    case LINETYPE.None:
                        value = Math.Round(Value - (area * Math.Max(hight1, hight2)), 2);
                        break;
                    case LINETYPE.Resistance:
                        value = Math.Round(Value - (area * hight1), 2);
                        break;
                    case LINETYPE.Sustentation:
                        value = Math.Round(Value - (area * hight2), 2);
                        break;
                }
                return value;
            }
        }
        public decimal Value
        {
            get
            {
                return IsTest ? TST_VALUE : base.TryAdd(TempValue);
            }
        }
        public LINETYPE Type { get; private set; }

        private Action<EventArg> BreakIn { get; set; }
        private Action<EventArg> BreakBack { get; set; }
        private Action<EventArg> BreakOut { get; set; }
        private Action<EventArg> Touched { get; set; }

        private bool IsTest { get; set; }

        public static bool Overlapped(AverageLine line1, AverageLine line2)
        {
            if (line1 == null || line2 == null) return false;
            if (line1.Value == line2.Value) return true;

            AverageLine upper = null;
            AverageLine lower = null;

            if (line1.Value > line2.Value)
            {
                upper = line1;
                lower = line2;
            }
            else
            {
                upper = line2;
                lower = line1;
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
        public static List<AverageLine> Merge(List<AverageLine> lines)
        {
            var list = new List<AverageLine>();
            if (lines != null && lines.Count > 0)
            {
                if (lines.Count == 1)
                {
                    list.Add(lines[0]);
                }
                else
                {
                    AverageLine line1 = null;
                    AverageLine line2 = null;
                    do
                    {
                        if (line1 == null)
                        {
                            line1 = lines[0];
                            lines.RemoveAt(0);
                        }
                        if (line2 == null)
                        {
                            line2 = lines[0];
                            lines.RemoveAt(0);
                        }

                        if (Overlapped(line1, line2))
                        {
                            line1 = (line1.ID > line2.ID) ? line1 : line2;
                        }
                        else
                        {
                            list.Add(line1);
                            line1 = line2;
                        }
                        line2 = null;

                    } while (lines.Count > 0);

                    if (line1 != null) list.Add(line1);
                }
            }
            return list;
        }

        public AverageLine(LINE id, int count, int precision, bool test = false)
            : base(count, precision)
        {
            ID = id;

            Type = LINETYPE.None;
            Score = 0;
            IsTest = test;
        }

        public AverageLine BindEvent(string id, Action<EventArg> func)
        {
            if (!string.IsNullOrWhiteSpace(id) && func != null)
            {
                switch (id.Trim().ToUpper())
                {
                    case "BREAKIN":
                        BreakIn = func;
                        break;
                    case "BREAKBACK":
                        BreakBack = func;
                        break;
                    case "BREAKOUT":
                        BreakOut = func;
                        break;
                    case "TOUCHED":
                        Touched = func;
                        break;
                }
            }
            return this;
        }

        public override void Add(decimal value)
        {
            base.Add(TempValue);
            this.TryAdd(value);
        }
        public override decimal TryAdd(decimal value)
        {
            TempValue = value;
            return base.TryAdd(TempValue);
        }
        public void Switch(LINETYPE tp)
        {
            if (Type != tp) Type = tp;
        }

        public Line.RELATIONSHIP ReadLines(List<Line> lines)
        {
            var relation = Line.RELATIONSHIP.NONE;
            switch (Type)
            {
                case LINETYPE.None:
                    switch (relation = Analyze(this.Value, lines))
                    {
                        case Line.RELATIONSHIP.UPPER:
                        case Line.RELATIONSHIP.CROSSINGUP:
                            if ((relation = Analyze(this.UpperBorder, lines)) == Line.RELATIONSHIP.CROSSINGUP)
                            {
                                Score = 0;
                                if (BreakOut != null) BreakOut(new EventArg(this, lines[0]));
                            }
                            break;

                        case Line.RELATIONSHIP.LOWER:
                        case Line.RELATIONSHIP.CROSSINGDOWN:
                            if ((relation = Analyze(this.LowerBorder, lines)) == Line.RELATIONSHIP.CROSSINGDOWN)
                            {
                                Score = 0;
                                if (BreakOut != null) BreakOut(new EventArg(this, lines[0]));
                            }
                            break;
                    }
                    break;

                case LINETYPE.Resistance:

                    switch (relation = Analyze(this.Value, lines))
                    {
                        case Line.RELATIONSHIP.UPPER:
                            if ((relation = Analyze(this.UpperBorder, lines)) == Line.RELATIONSHIP.CROSSINGUP)
                            {
                                Score = 0;
                                if (BreakOut != null) BreakOut(new EventArg(this, lines[0]));
                            }
                            break;

                        case Line.RELATIONSHIP.TOUCHINGUP:
                        case Line.RELATIONSHIP.CROSSINGUP:
                            if ((Score % 10) == 1)
                            {
                                Score += 1;
                                if (Touched != null) Touched(new EventArg(this, lines[0]));
                            }
                            break;

                        case Line.RELATIONSHIP.LOWER:
                        case Line.RELATIONSHIP.CROSSINGDOWN:
                            switch (relation = Analyze(this.LowerBorder, lines))
                            {
                                case Line.RELATIONSHIP.CROSSINGUP:
                                    if ((Score % 10) == 0)
                                    {
                                        Score += 1;
                                        if (BreakIn != null) BreakIn(new EventArg(this, lines[0]));
                                    }
                                    break;
                                case Line.RELATIONSHIP.CROSSINGDOWN:
                                    Score -= 1;
                                    if ((Score % 10) > 0)
                                    {
                                        Score = (Score - (Score % 10)) + 10;
                                        if (BreakBack != null) BreakBack(new EventArg(this, lines[0]));
                                    }
                                    break;
                            }
                            break;
                    }
                    break;

                case LINETYPE.Sustentation:

                    switch (relation = Analyze(this.Value, lines))
                    {
                        case Line.RELATIONSHIP.UPPER:
                        case Line.RELATIONSHIP.CROSSINGUP:
                            switch (relation = Analyze(this.UpperBorder, lines))
                            {
                                case Line.RELATIONSHIP.CROSSINGDOWN:
                                    if ((Score % 10) == 0)
                                    {
                                        Score += 1;
                                        if (BreakIn != null) BreakIn(new EventArg(this, lines[0]));
                                    }
                                    break;
                                case Line.RELATIONSHIP.CROSSINGUP:
                                    Score -= 1;
                                    if ((Score % 10) > 0)
                                    {
                                        Score = (Score - (Score % 10)) + 10;
                                        if (BreakBack != null) BreakBack(new EventArg(this, lines[0]));
                                    }
                                    break;
                            }
                            break;


                        case Line.RELATIONSHIP.TOUCHINGDOWN:
                        case Line.RELATIONSHIP.CROSSINGDOWN:
                            if ((Score % 10) == 1)
                            {
                                Score += 1;
                                if (Touched != null) Touched(new EventArg(this, lines[0]));
                            }
                            break;

                        case Line.RELATIONSHIP.LOWER:
                            if ((relation = Analyze(this.LowerBorder, lines)) == Line.RELATIONSHIP.CROSSINGDOWN)
                            {
                                Score = 0;
                                if (BreakOut != null) BreakOut(new EventArg(this, lines[0]));
                            }
                            break;
                    }
                    break;
            }
            return relation;
        }
        private Line.RELATIONSHIP Analyze(decimal value, List<Line> lines)
        {
            var relation = Line.RELATIONSHIP.NONE;
            var relation0 = Line.RELATIONSHIP.NONE;
            var relation1 = Line.RELATIONSHIP.NONE;
            if (lines.Count > 0)
            {
                var index = 0;
                do
                {
                    
                    var line = lines[index];
                    var last = index == lines.Count - 1;
                    if (index == 0)
                    {
                        relation0 = line.Tell(value);
                    }
                    else
                    {
                        relation1 = line.Tell(value);
                    }
                    switch (relation0)
                    {
                        case Line.RELATIONSHIP.OVERLAPPING:        //重叠
                            switch (relation1)
                            {
                                //case Line.RELATIONSHIP.NONE:
                                //    if (lines.Count <= 1) relation = relation0;
                                //    break;

                                case Line.RELATIONSHIP.TOUCHINGUP:
                                    relation = Line.RELATIONSHIP.TOUCHINGUP;
                                    break;
                                case Line.RELATIONSHIP.TOUCHINGDOWN:
                                    relation = Line.RELATIONSHIP.TOUCHINGDOWN;
                                    break;
                                case Line.RELATIONSHIP.OVERLAPPING:
                                    break;
                                //default:
                                //    throw new Exception();
                            }
                            break;
                        case Line.RELATIONSHIP.TOUCHINGUP:        //向上触碰
                            relation = Line.RELATIONSHIP.TOUCHINGUP;
                            break;
                        case Line.RELATIONSHIP.TOUCHINGDOWN:      //向下触碰
                            relation = Line.RELATIONSHIP.TOUCHINGDOWN;
                            break;
                        case Line.RELATIONSHIP.CROSSINGUP:        //上穿
                            relation = Line.RELATIONSHIP.CROSSINGUP;
                            break;
                        case Line.RELATIONSHIP.CROSSINGDOWN:      //下穿
                            relation = Line.RELATIONSHIP.CROSSINGDOWN;
                            break;
                        case Line.RELATIONSHIP.UPPER:             //上方
                            switch (relation1)
                            {
                                //case Line.RELATIONSHIP.NONE:
                                //    if (lines.Count <= 1) relation = relation0;
                                //    break;

                                case Line.RELATIONSHIP.CROSSINGUP:
                                case Line.RELATIONSHIP.TOUCHINGDOWN:
                                case Line.RELATIONSHIP.UPPER:
                                    relation = Line.RELATIONSHIP.UPPER;
                                    break;

                                case Line.RELATIONSHIP.TOUCHINGUP:
                                    relation = Line.RELATIONSHIP.CROSSINGUP;
                                    break;

                                //case Line.RELATIONSHIP.LOWER:
                                //case Line.RELATIONSHIP.CROSSINGDOWN:
                                //    throw new Exception();
                            }
                            break;
                        case Line.RELATIONSHIP.LOWER:             //下方
                            switch (relation1)
                            {
                                //case Line.RELATIONSHIP.NONE:
                                //    if (lines.Count <= 1) relation = relation0;
                                //    break;

                                case Line.RELATIONSHIP.CROSSINGDOWN:
                                case Line.RELATIONSHIP.TOUCHINGUP:
                                case Line.RELATIONSHIP.LOWER:
                                    relation = Line.RELATIONSHIP.LOWER;
                                    break;

                                case Line.RELATIONSHIP.TOUCHINGDOWN:
                                    relation = Line.RELATIONSHIP.CROSSINGDOWN;
                                    break;

                                //case Line.RELATIONSHIP.UPPER:
                                //case Line.RELATIONSHIP.CROSSINGUP:
                                //    throw new Exception();
                            }
                            break;

                    }

                    if (last && relation == Line.RELATIONSHIP.NONE)
                    {
                        if (lines.Count == 1)
                        {
                            relation = relation0;
                        }
                        else
                        {
                            if (relation1 == Line.RELATIONSHIP.OVERLAPPING)
                            {
                                switch (relation0)
                                {
                                    case Line.RELATIONSHIP.UPPER:
                                        relation = Line.RELATIONSHIP.CROSSINGUP;
                                        break;
                                    case Line.RELATIONSHIP.LOWER:
                                        relation = Line.RELATIONSHIP.CROSSINGDOWN;
                                        break;
                                    default:
                                        relation = Line.RELATIONSHIP.OVERLAPPING;
                                        break;
                                }
                            }
                            else
                            {
                                relation = relation1;
                            }

                        }
                    }
                } while (++index < lines.Count && relation == Line.RELATIONSHIP.NONE);
            }

            return relation;
        }

        public override string ToString()
        {
            return string.Format("(<id={0},sc={1}><type={2},uv={3},val={4},lv={5}>)", ID.ToString(), Score,
                Type.ToString(),
                UpperBorder, Value, LowerBorder
                );
        }
    }
}
