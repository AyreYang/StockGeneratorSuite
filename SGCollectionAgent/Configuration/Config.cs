using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SGNativeEntities.Enums;
using System.Management;

namespace SGCollectionAgent.Configuration
{
    public class Config
    {
        private static readonly object m_lock = new object();
        private static volatile Config m_inst = null;

        public ConfigInfo INFO { get; private set; }
        
        public bool IsReady { get; private set; }
        public static string RootDir
        {
            get
            {
                var lfi_module = new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                return lfi_module.Directory.FullName;
            }
        }
        public static bool IsFullPath(string path){
            return Regex.IsMatch(path, @"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$");
        }


        public static Config Instance
        {
            get
            {
                lock (m_lock)
                {
                    if (m_inst == null) m_inst = new Config();
                }
                return m_inst;
            }
        }

        private Config()
        {
            IsReady = false;
            INFO = null;
        }

        public void Load(FileInfo json)
        {
            IsReady = false;
            INFO = null;
            if (json == null || !File.Exists(json.FullName)) return;

            var script = string.Empty;
            using (StreamReader sr_reader = new StreamReader(new FileStream(json.FullName, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8))
            {
                script = sr_reader.ReadToEnd();
            }
            if (string.IsNullOrWhiteSpace(script)) return;

            INFO = JsonConvert.DeserializeObject<ConfigInfo>(script);

            if (INFO.ScriptSetting != null) INFO.ScriptSetting.Load();

            IsReady = INFO != null;
        }

        public void Export(FileInfo file)
        {
            if (INFO == null) INFO = new ConfigInfo();
            var json = JsonConvert.SerializeObject(INFO);
            using (StreamWriter sw_writer = new StreamWriter(new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write), Encoding.UTF8))
            {
                sw_writer.Write(json);
            }
        }
    }

    public class ConfigInfo
    {
        public DBSetting DBSetting { get; set; }
        public ScriptSetting ScriptSetting { get; set; }
        public ImportSetting ImportSetting { get; set; }
        public TimeSetting TimeSetting { get; set; }
        public MACDSetting MACDSetting { get; set; }
        public RSISetting RSISetting { get; set; }
        //public MemoryManager MemoryManager { get; set; }
        public ConfigInfo()
        {
            DBSetting = new DBSetting();
            ScriptSetting = new ScriptSetting();
            ImportSetting = new ImportSetting();
            //TimeSetting = new TimeSetting();
            RSISetting = new RSISetting();
            //MemoryManager = new MemoryManager();
        }
    }

    public class DBSetting
    {
        public string Host { get; set; }
        public string DBName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
    public class ScriptSetting
    {
        public string Path { get; set; }
        public FileInfo FileInfo
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(Path) && File.Exists(Path.Trim())) ?
                    (Config.IsFullPath(Path)) ? new FileInfo(Path.Trim()) : new FileInfo(System.IO.Path.Combine(Config.RootDir, Path.Trim())) : null;
            }
        }

        public Dictionary<string, string> Scripts { get; private set; }

        public ScriptSetting()
        {
            Scripts = new Dictionary<string, string>();
        }

        public void Load()
        {
            var msg = string.Empty;
            var configuration = new XMLConfiguration.Configuration();
            if (configuration.Load(FileInfo, ref msg) > 0)
            {
                var list = configuration.Config.GetValueList();
                Scripts.Clear();
                if (list != null && list.Count > 0) list.ForEach(itm =>
                {
                    var key = itm.ID.Trim().ToUpper();
                    if (Scripts.ContainsKey(key))
                    {
                        Scripts[key] = itm.Value.ToString();
                    }
                    else
                    {
                        Scripts.Add(key, itm.Value.ToString());
                    }
                });
            }
        }
    }
    public class ImportSetting
    {
        public string Path { get; set; }
        public DirectoryInfo DirInfo
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(Path) && Directory.Exists(Path.Trim())) ? 
                    (Config.IsFullPath(Path)) ? new DirectoryInfo(Path.Trim()) : new DirectoryInfo(System.IO.Path.Combine(Config.RootDir, Path.Trim())) : null;
            }
        }

        public Dictionary<string, string> RawTemplates { get; set; }
        public TemplatesContianer<string, Template<TemplateUnit>> Templates
        {
            get
            {
                var templates = new TemplatesContianer<string, Template<TemplateUnit>>();
                if (RawTemplates != null && RawTemplates.Count > 0)
                {
                    foreach (string key in RawTemplates.Keys)
                    {
                        if(string.IsNullOrWhiteSpace(RawTemplates[key]))continue;
                        var units = new Template<TemplateUnit>(key.Trim().ToUpper());
                        var columns = RawTemplates[key].Trim().Split(",".ToCharArray());
                        new List<string>(columns).ForEach(col =>
                        {
                            if (string.IsNullOrWhiteSpace(col)) return;
                            var temp = col.Trim().Split(":".ToCharArray());
                            var unit = (temp.Length >= 2) ? TemplateUnit.Create(temp[0], temp[1]) : null;
                            if (unit != null)
                            {
                                units.Add(unit);
                            }
                        });

                        if (units.Count > 0) templates.Add(key, units);
                    }
                }

                return templates;
            }
        }
    }

    public class TimeSetting
    {
        private static readonly object m_lock = new object();

        private List<DateTime> RestDateList { get; set; }
        public int IsWorkingTime
        {
            get
            {
                // 0:not, 1:working, 2:real
                var now = System.DateTime.Now;
                var week = (int)now.DayOfWeek;
                if (week == 0) week = 7;

                if (!RestDateList.Contains(now.Date) && (week < 6) && IsTimeValid && (now > WTFrom.Value && now < WTTo.Value))
                {
                    return ((now > AMFrom.Value && now < AMTo.Value) ||
                            (now > PMFrom.Value && now < PMTo.Value)) ?
                            2 : 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string WorkTime { get; set; }
        private DateTime? WTFrom
        {
            get
            {
                string[] temp = null;
                int hour = 0;
                int minute = 0;
                var today = DateTime.Today;
                return (!string.IsNullOrWhiteSpace(WorkTime) &&
                    (temp = WorkTime.Trim().Split("-".ToCharArray())).Length == 2 &&
                    TryParse(temp[0], out hour, out minute)) ?
                    (DateTime?)new DateTime(today.Year, today.Month, today.Day, hour, minute, 0) : null;
            }
        }
        private DateTime? WTTo
        {
            get
            {
                string[] temp = null;
                int hour = 0;
                int minute = 0;
                var today = DateTime.Today;
                return (!string.IsNullOrWhiteSpace(WorkTime) &&
                    (temp = WorkTime.Trim().Split("-".ToCharArray())).Length == 2 &&
                    TryParse(temp[1], out hour, out minute)) ?
                    (DateTime?)new DateTime(today.Year, today.Month, today.Day, hour, minute, 0) : null;
            }
        }

        public string AM { get; set; }
        private DateTime? AMFrom
        {
            get
            {
                string[] temp = null;
                int hour = 0;
                int minute = 0;
                var today = DateTime.Today;
                return (!string.IsNullOrWhiteSpace(AM) &&
                    (temp = AM.Trim().Split("-".ToCharArray())).Length == 2 &&
                    TryParse(temp[0], out hour, out minute)) ?
                    (DateTime?)new DateTime(today.Year, today.Month, today.Day, hour, minute, 0) : null;
            }
        }
        private DateTime? AMTo
        {
            get
            {
                string[] temp = null;
                int hour = 0;
                int minute = 0;
                var today = DateTime.Today;
                return (!string.IsNullOrWhiteSpace(AM) &&
                    (temp = AM.Trim().Split("-".ToCharArray())).Length == 2 &&
                    TryParse(temp[1], out hour, out minute)) ?
                    (DateTime?)new DateTime(today.Year, today.Month, today.Day, hour, minute, 0) : null;
            }
        }

        public string PM { get; set; }
        private DateTime? PMFrom
        {
            get
            {
                string[] temp = null;
                int hour = 0;
                int minute = 0;
                var today = DateTime.Today;
                return (!string.IsNullOrWhiteSpace(PM) &&
                    (temp = PM.Trim().Split("-".ToCharArray())).Length == 2 &&
                    TryParse(temp[0], out hour, out minute)) ?
                    (DateTime?)new DateTime(today.Year, today.Month, today.Day, hour, minute, 0) : null;
            }
        }
        private DateTime? PMTo
        {
            get
            {
                string[] temp = null;
                int hour = 0;
                int minute = 0;
                var today = DateTime.Today;
                return (!string.IsNullOrWhiteSpace(PM) &&
                    (temp = PM.Trim().Split("-".ToCharArray())).Length == 2 &&
                    TryParse(temp[1], out hour, out minute)) ?
                    (DateTime?)new DateTime(today.Year, today.Month, today.Day, hour, minute, 0) : null;
            }
        }

        public bool IsTimeValid
        {
            get
            {
                return WTFrom.HasValue && WTTo.HasValue && AMFrom.HasValue && AMTo.HasValue && PMFrom.HasValue && PMTo.HasValue;
            }
        }


        public TimeSetting()
        {
            RestDateList = new List<DateTime>();
        }

        public void AddDate(DateTime date)
        {
            lock (m_lock)
            {
                RestDateList.Add(date.Date);
            }
        }
        public void Clear()
        {
            lock (m_lock)
            {
                if (RestDateList.Count > 0) RestDateList.Clear();
            }
        }

        private bool TryParse(string val, out int hour, out int minute)
        {
            hour = 0; minute = 0;
            if (string.IsNullOrWhiteSpace(val)) return false;
            var match = Regex.Match(val, @"^(?<hour>0\d{1}|1\d{1}|2[0-3]):(?<minute>[0-5]\d{1})$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (match.Success)
            {
                hour = int.Parse(match.Groups["hour"].Value);
                minute = int.Parse(match.Groups["minute"].Value);
            }
            return match.Success;
        }
    }

    public class MACDSetting
    {
        public int N1 { get; set; }
        public int N2 { get; set; }
        public int N3 { get; set; }
    }
    public class RSISetting
    {
        public int N1 { get; set; }
        public int N2 { get; set; }
        public int N3 { get; set; }
    }
    /*
    public class MemoryManager
    {
        public long TotalSize { get; private set; }
        public long AvailableSize 
        {
            get
            {
                long size = 0;
                using (ManagementClass memory = new ManagementClass("Win32_PerfFormattedData_PerfOS_Memory"))
                {

                    using (ManagementObjectCollection collection = memory.GetInstances())
                    {
                        foreach (ManagementObject obj in collection)
                        {
                            size += Int64.Parse(obj.Properties["AvailableMBytes"].Value.ToString());

                        }
                    }
                }
                return size;
            }
        }
        public decimal AvailableRatio
        {
            get
            {
                return (TotalSize == 0 || AvailableSize == 0) ? 0 : Math.Round(Convert.ToDecimal(AvailableSize) / Convert.ToDecimal(TotalSize), 2);
            }
        }

        public MemoryManager()
        {
            TotalSize = 0;
            //获取总物理内存大小
            using (ManagementClass memory = new ManagementClass("Win32_PhysicalMemory"))
            {
                using (ManagementObjectCollection collection = memory.GetInstances())
                {
                    foreach (ManagementObject obj in collection)
                    {
                        TotalSize += Int64.Parse(obj.Properties["Capacity"].Value.ToString());
                    }
                }
            }
        }
    }*/


    public class TemplatesContianer<K, V> : Dictionary<K, V>, IDisposable where V : Template<TemplateUnit>
    {
        public void Match(string script)
        {
            if (this.Count <= 0) return;
            this.Values.ToList().ForEach(template =>
            {
                template.MatchByScrtip(script);
            });
        }

        public Template<TemplateUnit> MatchedTemplate
        {
            get
            {
                var max = this.Values.ToList().Max(template => template.MatchRatio);
                if (max < 80m) return null;
                var templates = this.Values.ToList().FindAll(template => template.MatchRatio >= max);
                var count = templates.Max(template => template.Count);
                return templates.First(template => template.Count == count);
            }
        }

        public void Dispose()
        {
            foreach (K key in this.Keys) this[key].Dispose();
            this.Clear();
        }

        ~TemplatesContianer()
        {
            Dispose();
        }
    }
    public class Template<T> : List<T>, IDisposable where T : TemplateUnit
    {
        public string Name { get; private set; }
        public TABLES Table { get; private set; }

        public static char separator = Convert.ToChar(9);

        public Template(string name):base(){
            var table = TABLES.NONE;
            Enum.TryParse(name, out table);
            Table = table;
            Name = name;
        }

        public decimal MatchRatio
        {
            get
            {
                var count = this.Count(itm => itm.ColumnIndex >= 0);
                var ratio = (this.Count <= 0) ? 0 : Math.Round(((decimal)count / (decimal)this.Count), 2) * 100;
                return ratio;
            }
        }

        public void MatchByScrtip(string script)
        {
            if (string.IsNullOrWhiteSpace(script)) return;
            var titles = new List<string>();
            new List<string>(script.Trim().Split(new char[] { separator })).ForEach(itm=>titles.Add(itm.Trim()));
            this.ForEach(unit => unit.ColumnIndex = titles.IndexOf(unit.Column));
        }

        public void Dispose()
        {
            this.Clear();
        }

        ~Template()
        {
            Dispose();
        }
    }
    public class TemplateUnit
    {
        public string Column { get; private set; }
        public int ColumnIndex { get; set; }

        public string DBColumn { get; private set; }

        private TemplateUnit(string column, string dbcolumn)
        {

            Column = column.Trim();
            DBColumn = dbcolumn.Trim();

            ColumnIndex = -1;
        }

        public static TemplateUnit Create(string column, string dbcolumn)
        {
            if (string.IsNullOrWhiteSpace(column) || string.IsNullOrWhiteSpace(dbcolumn)) return null;
            return new TemplateUnit(column.Trim(), dbcolumn.Trim());
        }

        public static object ConvertVal(Type type, string val)
        {
            object value = null;
            switch (type.ToString())
            {
                case "System.DateTime":
                    value = Convert2Dt(val);
                    break;
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    value = Convert2Int(val);
                    break;
                case "System.Decimal":
                    value = Convert2Dec(val);
                    break;
                default:
                    value = val;
                    break;
            }

            return value;
        }

        private static decimal? Convert2Dec(string val)
        {
            decimal? ret = null;
            try
            {
                if (string.IsNullOrWhiteSpace(val)) return null;

                var match = Regex.Match(val, @"(?<val>(-?\d+)(\.\d+)?)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (!match.Success) return null;
                ret = Convert.ToDecimal(match.Groups["val"].Value);
            }
            catch (Exception err)
            {
                throw err;
            }
            return ret;
        }
        private static int? Convert2Int(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return null;

            var match = Regex.Match(val, @"(?<val>(-?\d+))", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success) return null;
            return int.Parse(match.Groups["val"].Value);
        }
        private static DateTime? Convert2Dt(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return null;
            var value = val.Replace("-", string.Empty).Replace("/", string.Empty).Replace(":", string.Empty);
            var match = Regex.Match(value, @"(?<val>(([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[1][0-9]|2[0-8]))))|((([0-9]{2})(0[48]|[2468][048]|[13579][26])|((0[48]|[2468][048]|[3579][26])00))0229))", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success) return null;

            var sdate = match.Groups["val"].Value;
            var left = value.Replace(sdate, string.Empty).Trim();
            DateTime? Date = null;
            if (string.IsNullOrWhiteSpace(left))
            {
                Date = DateTime.ParseExact(match.Groups["val"].Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            }else{
                if (left.Length < 6) left = (left + "000000").Substring(0, 6);
                match = Regex.Match(left, @"(?<hour>0\d{1}|1\d{1}|2[0-3])(?<minute>[0-5]\d{1})(?<second>[0-5]\d{1})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (match.Success)
                {
                    Date = DateTime.ParseExact(string.Format("{0} {1}:{2}:{3}", sdate, match.Groups["hour"].Value, match.Groups["minute"].Value, match.Groups["second"].Value), "yyyyMMdd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    Date = DateTime.ParseExact(match.Groups["val"].Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return Date;
        }
    }

}
