using System;
using System.Collections.Generic;
using System.Linq;
using SGUtilities.Averager;
using SGUtilities.Shapes;
using System.Text;

namespace SGUtilities.Lines
{
    public class LinesController : IDisposable
    {
        private List<Line> LinesCache { get; set; }
        private List<AverageLine> Lines { get; set; }

        private AverageValue Averager { get; set; }
        private Dictionary<LineType, TimeLine<KeyValuePair<DateTime, decimal>>> TimeLines { get; set; }
        
        private LineMaker LineMaker { get; set; }
        private bool Initialized { get; set; }

        public LinesController(int count, int percision = 2)
        {
            Averager = new AverageValue(count, percision);
            Lines = new List<AverageLine>();
            TimeLines = new Dictionary<LineType, TimeLine<KeyValuePair<DateTime, decimal>>>();
            LineMaker = new LineMaker(1000);
        }

        public void AddTimeLine(LineType type, TimeLine<KeyValuePair<DateTime, decimal>> line)
        {
            if (TimeLines.ContainsKey(type))
            {
                TimeLines[type] = line;
            }
            else
            {
                TimeLines.Add(type, line);
            }
        }

        public void Add(KeyValuePair<DateTime, decimal> pair)
        {
            Averager.Add(pair.Value);
            TimeLines.Values.ToList().ForEach(line => line.Add(pair));
            LineMaker.Add(new KeyValuePair<DateTime, decimal>(pair.Key, Averager.Average));
        }

        public void Reset()
        {
            Initialized = false;
            Lines.Clear();
        }

        private void MatchLines(decimal value)
        {
            Lines.Clear();
            var lines = new List<AverageLine>();
            TimeLines.Values.ToList().ForEach(timeline =>timeline.Lines.ForEach(line =>
            {
                line.Switch(LINETYPE.None);
                lines.Add(line);
            }));
            lines.Sort((x, y) =>
            {
                var xv = x.Value;
                var yv = y.Value;
                return xv == yv ? 0 : xv > yv ? -1 : 1;
            });
            lines = AverageLine.Merge(lines);
            if (lines.Any(line => line.Value == value))
            {
                Lines.Add(lines.Find(line => line.Value == value));
            }
            else
            {
                var reslines = GetResistanceLines(value, lines);
                var suslines = GetSustentationLines(value, lines);

                if (reslines != null && reslines.Count > 0) Lines.Add(reslines[0]);
                if (suslines != null && suslines.Count > 0) Lines.Add(suslines[0]);
            }

            Initialized = true;
        }

        private List<AverageLine> GetResistanceLines(decimal value, List<AverageLine> lines)
        {
            var reslines = lines.FindAll(l => l.Value > value);
            reslines.Sort((x, y) =>
            {
                var xv = x.Value;
                var yv = y.Value;
                return xv == yv ? 0 : xv > yv ? 1 : -1;
            });
            reslines.ForEach(l => l.Switch(LINETYPE.Resistance));
            return reslines;
        }
        private List<AverageLine> GetSustentationLines(decimal value, List<AverageLine> lines)
        {
            var suslines = lines.FindAll(l => l.Value < value);
            suslines.Sort((x, y) =>
            {
                var xv = x.Value;
                var yv = y.Value;
                return xv == yv ? 0 : xv > yv ? -1 : 1;
            });
            suslines.ForEach(l => l.Switch(LINETYPE.Sustentation));
            return suslines;
        }

        
        public void Process()
        {
            if (!Averager.IsValid) return;
            LinesCache = LineMaker.ExportLast(3);
            if (LinesCache == null || LinesCache.Count <= 0) return;

            if (!Initialized) MatchLines(Averager.Average);
            Lines.ForEach(line => line.ReadLines(LinesCache));
        }

        public void ClearLines()
        {
            TimeLines.Values.ToList().ForEach(line => line.Clear());
            TimeLines.Clear();
            Averager.Clear();
            LineMaker.Clear();
            Lines.Clear();
        }

        public void Dispose()
        {
            Lines.Clear();
            TimeLines.Values.ToList().ForEach(line => line.Dispose());
            TimeLines.Clear();
            Averager.Dispose();
            LineMaker.Dispose();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            // Lines
            var lines = string.Empty;
            if (LinesCache == null || LinesCache.Count <= 0)
            {
                lines = "[]";
            }
            else
            {
                var list = new List<string>();
                LinesCache.ForEach(l => list.Add(l.ToString()));
                lines = string.Format("[{0}]", string.Join(",", list));
            }
            sb.Append(string.Format("Ls:{0}", lines));

            sb.Append("\t");

            // AverageLines
            lines = string.Empty;
            if (Lines == null || Lines.Count <= 0)
            {
                lines = "[]";
            }
            else
            {
                var list = new List<string>();
                Lines.ForEach(l => list.Add(l.ToString()));
                lines = string.Format("[{0}]", string.Join(",", list));
            }
            sb.Append(string.Format("ALs:{0}", lines));

            return sb.ToString();
        }

        ~LinesController()
        {
            Dispose();
        }
    }
}
