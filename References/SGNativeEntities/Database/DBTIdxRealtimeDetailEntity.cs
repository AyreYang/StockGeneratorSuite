using System;
using System.Text.RegularExpressions;
using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;
using SGNativeEntities.General;

namespace SGNativeEntities.Database
{
    public class DBTIdxRealtimeDetailEntity : SEQTableEntity
    {
        private const long ticks = 10000000;

        [DBField("ID", typeof(long), KeyType.Primary, false)]
        public long ID {
            get { return this.GetValue<long>("ID"); }
            set { this.SetValue("ID", value); }
        }
        [DBField("Index", typeof(decimal), KeyType.Normal, false)]
        public decimal Index
        {
            get { return this.GetValue<decimal>("Index"); }
            set { this.SetValue("Index", value); }
        }
        [DBField("VolAmount", typeof(decimal), KeyType.Normal, false)]
        public decimal VolAmount {
            get { return this.GetValue<decimal>("VolAmount"); }
            set { this.SetValue("VolAmount", value); }
        }
        [DBField("VolMoney", typeof(decimal), KeyType.Normal, false)]
        public decimal VolMoney {
            get { return this.GetValue<decimal>("VolMoney"); }
            set { this.SetValue("VolMoney", value); }
        }

        [DBField("Time", typeof(DateTime), KeyType.Normal, false)]
        public DateTime Time {
            get { return this.GetValue<DateTime>("Time"); }
            set { this.SetValue("Time", value); }
        }

        public DBTIdxRealtimeDetailEntity(string code, DatabaseAccessor accessor = null) : base(TableName(code), accessor) { }
        public static DBTIdxRealtimeDetailEntity From(ItemInfoEntity info)
        {
            if (info == null || string.IsNullOrWhiteSpace(info.Code)) return null;
            var entity = new DBTIdxRealtimeDetailEntity(info.Code);
            entity.Index = info.Value;
            entity.VolAmount = info.Amount;
            entity.VolMoney = info.Money;
            entity.Time = info.Time;
            return entity;
        }
        public static string TableName(string code)
        {
            if (!IsValidCode(code, CODETYPE.index)) throw new Exception(string.Format("Code({0}) is invalid.", code??string.Empty));
            return string.Format("{0}_{1}", TABLES.IDX_REALTIME_TD, code.Trim().ToUpper());
        }
        public string Code
        {
            get
            {
                var match = Regex.Match(base.TableName, @"(?<code>\d{6})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                return (match.Success) ? match.Groups["code"].Value : string.Empty;
            }
        }

        public override void GenerateID()
        {
            if (ID <= 0) ID = this.GenerateSequence();
        }
    }
}
