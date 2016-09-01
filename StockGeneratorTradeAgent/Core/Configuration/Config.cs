using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace StockGeneratorTradeAgent.Core.Configuration
{
    public class Config
    {
        private static readonly object m_lock = new object();
        private static volatile Config m_inst = null;

        public ConfigInfo INFO { get; private set; }
        public bool IsReady { get; private set; }

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

        private Config() {
            IsReady = false;
            INFO = null;
        }

        public void Load(FileInfo file)
        {
            IsReady = false;
            INFO = null;
            if (file == null || !File.Exists(file.FullName)) return;

            var json = string.Empty;
            using (StreamReader sr_reader = new StreamReader(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8))
            {
                json = sr_reader.ReadToEnd();
            }
            if (string.IsNullOrWhiteSpace(json)) return;

            INFO = JsonConvert.DeserializeObject<ConfigInfo>(json);

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
        public TimeSetting BizTime { get; set; }
        public WebSocketInfo WebSocket { get; set; }
        public DBSetting DBSetting { get; set; }
        public TradeAccount TradeAccount { get; set; }
        public TradePack BTrade { get; set; }
        public TradePack STrade { get; set; }
        public JuHeData JuHeData { get; set; }
        public ConfigInfo()
        {
            BizTime = new TimeSetting();
            WebSocket = new WebSocketInfo();
            DBSetting = new DBSetting();
            TradeAccount = new TradeAccount();
            BTrade = new TradePack();
            STrade = new TradePack();
            JuHeData = new JuHeData();
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

    public class WebSocketInfo
    {
        public string URL { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
    public class DBSetting
    {
        public string Host { get; set; }
        public string DBName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
    public class TradeAccount
    {
        public string AppName { get; set; }
        public string Account { get; set; }
    }
    public class TradePack
    {
        public Rect Code { get; set; }
        public Rect Price { get; set; }
        public Rect Amount { get; set; }
        public Rect Confirm { get; set; }
        public TradePack()
        {
            Code = new Rect();
            Price = new Rect();
            Amount = new Rect();
            Confirm = new Rect();
        }
    }
    public class Rect
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int WHnd { get; set; }
    }
    public class JuHeData
    {
        public string AppKey { get; set; }
    }
}
