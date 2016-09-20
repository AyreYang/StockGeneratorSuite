using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGUtilities.Shapes
{
    public class Line
    {
        public enum DIRECTION
        {
            NONE = -100,
            UP = 1,
            FLAT = 0,
            DOWN = -1
        }
        public enum RELATIONSHIP
        {
            NONE = -100,

            OVERLAPPING = 0,        //重叠
            
            TOUCHINGUP = 1,         //向上触碰
            TOUCHINGDOWN = -1,      //向下触碰
            CROSSINGUP = 2,         //上穿
            CROSSINGDOWN = -2,      //下穿
            UPPER = 3,              //上方
            LOWER = -3             //下方
        }

        public KeyValuePair<DateTime, decimal> From { get; private set; }
        public KeyValuePair<DateTime, decimal> To { get; private set; }
        public decimal Length { get; private set; }
        public DIRECTION Direction
        {
            get
            {
                if (Length <= 0) return DIRECTION.NONE;
                var span = To.Value - From.Value;
                span = (span == decimal.Zero) ? decimal.Zero : span / Math.Abs(span);
                return (DIRECTION)span;
            }
        }
        public decimal Slope
        {
            get
            {
                var slope = Math.Round(Math.Abs(To.Value - From.Value) / (Length <= 0 ? 1 : Length), 6);
                return slope;
            }
        }

        public Line(KeyValuePair<DateTime, decimal> from, KeyValuePair<DateTime, decimal> to, decimal length = 0)
        {
            From = from;
            To = to;
            Length = (From.Key.Ticks == To.Key.Ticks || length <= 0) ? 0 : length;
        }

        public void Fresh(KeyValuePair<DateTime, decimal> to)
        {
            To = to;
            Length++;
        }

        public RELATIONSHIP Tell(decimal val)
        {
            var relationship = RELATIONSHIP.NONE;
            switch (Direction)
            {
                case DIRECTION.FLAT:
                    if (To.Value == val) relationship = RELATIONSHIP.OVERLAPPING;
                    if (To.Value > val) relationship = RELATIONSHIP.UPPER;
                    if (To.Value < val) relationship = RELATIONSHIP.LOWER;
                    break;
                case DIRECTION.UP:
                    if (From.Value < val && To.Value > val)
                    {
                        relationship = RELATIONSHIP.CROSSINGUP;
                    }
                    else
                    {
                        if (From.Value >= val) relationship = RELATIONSHIP.UPPER;
                        if (To.Value == val) relationship = RELATIONSHIP.TOUCHINGUP;
                        if (To.Value < val) relationship = RELATIONSHIP.LOWER;
                    }
                    break;
                case DIRECTION.DOWN:
                    if (From.Value > val && To.Value < val)
                    {
                        relationship = RELATIONSHIP.CROSSINGDOWN;
                    }
                    else
                    {
                        if (To.Value > val) relationship = RELATIONSHIP.UPPER;
                        if (To.Value == val) relationship = RELATIONSHIP.TOUCHINGDOWN;
                        if (From.Value <= val) relationship = RELATIONSHIP.LOWER;
                    }
                    break;
            }
            return relationship;
        }

        public override bool Equals(object obj)
        {
            var equal = false;
            if (obj != null && obj is Line)
            {
                var line = obj as Line;
                equal = this.From.Key.Equals(line.From.Key) && this.From.Value.Equals(line.From.Value) &&
                    this.To.Key.Equals(line.To.Key) && this.To.Value.Equals(line.To.Value) &&
                    this.Length.Equals(line.Length);
            }
            return equal;
        }

        public override string ToString()
        {
            return string.Format("(<from={0},to={1},len={2}><dir={3},slope={4}>)", From, To, Length, Direction, Slope);
        }
    }
}
