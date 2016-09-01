using System;
using System.Collections.Generic;
using SGUtilities.Cache;

namespace SGUtilities.Shapes
{
    public class LineMaker : CircularCache<Line>
    {
        public Line Line { get; private set; }
        private Line Line0 { get; set; }
        public LineMaker(int capacity)
            : base(capacity)
        {
            Line = null;
            Line0 = null;
        }
        public void Add(KeyValuePair<DateTime, decimal> pair)
        {
            Line = new Line(Line0 != null ? Line0.To : pair, pair, 1);

            if (Line0 == null)
            {
                Line0 = Line;
            }
            else
            {
                if (Line0.Direction != Line.DIRECTION.NONE && Line0.Direction != Line.Direction)
                {
                    base.Add(Line0);
                    Line0 = Line;
                }
                else
                {
                    Line0.Fresh(pair);
                }
            }
        }

        public override List<Line> Export()
        {
            var lines = base.Export(0, 0);
            lines.Sort((x, y) =>
            {
                var xv = x.From.Key;
                var yv = y.From.Key;
                return xv == yv ? 0 : xv > yv ? -1 : 1;
            });
            return lines;
        }
        public override List<Line> Export(int count)
        {
            var lines = base.Export(count);
            lines.Sort((x, y) =>
            {
                var xv = x.From.Key;
                var yv = y.From.Key;
                return xv == yv ? 0 : xv > yv ? -1 : 1;
            });
            return lines;
        }
        public override List<Line> Export(int start, int count)
        {
            var lines = base.Export(start, count);
            lines.Sort((x, y) =>
            {
                var xv = x.From.Key;
                var yv = y.From.Key;
                return xv == yv ? 0 : xv > yv ? -1 : 1;
            });
            return lines;
        }

        public List<Line> ExportLast(int count)
        {
            var lines = base.Export(Math.Abs(count) * -1);
            if (!Line.Equals(Line0))
            {
                lines.Add(new Line(Line0.From, Line.From, Line0.Length - Line.Length));
            }
            lines.Add(this.Line);
            
            lines.Sort((x, y) =>
            {
                var xv = x.From.Key;
                var yv = y.From.Key;
                return xv == yv ? 0 : xv > yv ? -1 : 1;
            });
            return lines;
        }
    }
}
