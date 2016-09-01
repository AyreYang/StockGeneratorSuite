using System;
using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;
using System.Collections.Generic;

namespace SGNativeEntities.Database
{
    public class DBTStkMinuteEntity : SEQTableEntity
    {
        [DBField("ID", typeof(long), KeyType.Normal, false)]
        public long ID
        {
            get { return this.GetValue<long>("ID"); }
            private set { this.SetValue("ID", value); }
        }

        // 代码
        [DBField("Code", typeof(string), KeyType.Primary, false)]
        public string Code
        {
            get { return this.GetValue<string>("Code"); }
            set { if (IsValidCode(value, CODETYPE.stock)) this.SetValue("Code", value); }
        }

        // 时间
        [DBField("Time", typeof(DateTime), KeyType.Primary, false)]
        public DateTime Time
        {
            get { return this.GetValue<DateTime>("Time"); }
            set { this.SetValue("Time", value); }
        }

        

        // 开盘
        [DBField("Open", typeof(decimal), KeyType.Normal, false)]
        public decimal Open
        {
            get { return this.GetValue<decimal>("Open"); }
            set { this.SetValue("Open", value); }
        }

        // 收盘
        [DBField("Close", typeof(decimal), KeyType.Normal, false)]
        public decimal Close
        {
            get { return this.GetValue<decimal>("Close"); }
            set { this.SetValue("Close", value); }
        }

        // 最高
        [DBField("High", typeof(decimal), KeyType.Normal, false)]
        public decimal High
        {
            get { return this.GetValue<decimal>("High"); }
            set { this.SetValue("High", value); }
        }

        // 最低
        [DBField("Low", typeof(decimal), KeyType.Normal, false)]
        public decimal Low
        {
            get { return this.GetValue<decimal>("Low"); }
            set { this.SetValue("Low", value); }
        }

        // 平均值
        [DBField("Average", typeof(decimal), KeyType.Normal, false)]
        public decimal Average
        {
            get { return this.GetValue<decimal>("Average"); }
            set { this.SetValue("Average", value); }
        }

        // 成交量
        [DBField("VolAmount", typeof(decimal), KeyType.Normal, false)]
        public decimal VolAmount
        {
            get { return this.GetValue<decimal>("VolAmount"); }
            set { this.SetValue("VolAmount", value); }
        }

        // 成交额
        [DBField("VolMoney", typeof(decimal), KeyType.Normal, false)]
        public decimal VolMoney
        {
            get { return this.GetValue<decimal>("VolMoney"); }
            set { this.SetValue("VolMoney", value); }
        }

        public void CalculateAverage()
        {
            Average = VolAmount != decimal.Zero ? Math.Round(VolMoney / VolAmount, 2) :
                Math.Round((High + Low) / 2, 2);
        }

        public DBTStkMinuteEntity() : base(TABLES.STK_MINUTE_TD.ToString(), null) { }
        public DBTStkMinuteEntity(DatabaseAccessor accessor) : base(TABLES.STK_MINUTE_TD.ToString(), accessor) { }
        public DBTStkMinuteEntity(long id, DatabaseAccessor accessor)
            : base(TABLES.STK_MINUTE_TD.ToString(), accessor)
        {
            this.ID = id;
        }
        public DBTStkMinuteEntity(string code, DateTime time, DatabaseAccessor accessor)
            : base(TABLES.STK_MINUTE_TD.ToString(), accessor)
        {
            this.Code = code;
            this.Time = time;
        }

        public override void GenerateID()
        {
            if (ID <= 0) ID = this.GenerateSequence();
        }

        private bool IsDateValid
        {
            get
            {
                var start = new DateTime(DateTime.Today.Year - 3, 1, 1);
                return Time >= start;
            }
        }

        public bool IsDataValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Code) &&
                    IsDateValid &&
                    Open > decimal.Zero && Close > decimal.Zero &&
                    High > decimal.Zero && Low > decimal.Zero &&
                    VolAmount > decimal.Zero && VolMoney > decimal.Zero;
            }
        }

        public override bool Equals(object obj)
        {
            var entity = obj as DBTStkDailyEntity;
            if (entity == null) return false;
            return Code.Equals(entity.Code) && Time.Equals(entity.Date) && Open.Equals(entity.Open) && Close.Equals(entity.Close);
        }
    }

    public class DBTStkMinuteEntitySort : IComparer<DBTStkMinuteEntity>
    {
        public int Compare(DBTStkMinuteEntity x, DBTStkMinuteEntity y)
        {
            return x.Time == y.Time ? 0 : x.Time > y.Time ? 1 : -1;
        }
    }
}
