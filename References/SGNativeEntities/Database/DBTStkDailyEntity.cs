using System;
using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;
using System.Collections.Generic;

namespace SGNativeEntities.Database
{
    public class DBTStkDailyEntity : SEQTableEntity
    {
        [DBField("ID", typeof(long), KeyType.Normal, false)]
        public long ID
        {
            get { return this.GetValue<long>("ID"); }
            private set { this.SetValue("ID", value); }
        }

        // 日期
        [DBField("Date", typeof(DateTime), KeyType.Primary, false)]
        public DateTime Date
        {
            get { return this.GetValue<DateTime>("Date"); }
            set { this.SetValue("Date", value); }
        }

        // 代码
        [DBField("Code", typeof(string), KeyType.Primary, false)]
        public string Code
        {
            get { return this.GetValue<string>("Code"); }
            set { if (IsValidCode(value, CODETYPE.stock)) this.SetValue("Code", value); }
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

        public DBTStkDailyEntity() : base(TABLES.STK_DAILY_TD.ToString(), null) { }
        public DBTStkDailyEntity(DatabaseAccessor accessor) : base(TABLES.STK_DAILY_TD.ToString(), accessor) { }
        public DBTStkDailyEntity(long id, DatabaseAccessor accessor) : base(TABLES.STK_DAILY_TD.ToString(), accessor) {
            this.ID = id;
        }
        public DBTStkDailyEntity(string code, DateTime date, DatabaseAccessor accessor)
            : base(TABLES.STK_DAILY_TD.ToString(), accessor)
        {
            this.Code = code;
            this.Date = date;
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
                return Date >= start;
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
            return Code.Equals(entity.Code) && Date.Equals(entity.Date) && Open.Equals(entity.Open) && Close.Equals(entity.Close);
        }

        public static System.Data.Common.DbCommand LastDailyEntityCommand(string code, DatabaseAccessor accessor)
        {
            if (!IsValidCode(code, CODETYPE.stock) || accessor == null) return null;

            var entity = new DBTStkDailyEntity();
            var parameters = new List<System.Data.Common.DbParameter>();
            var where = new System.Text.StringBuilder();
            where.AppendLine("WHERE");

            // param:code
            var parm_code = accessor.CreateParameter("Code", code);
            parameters.Add(parm_code);
            where.AppendLine(string.Format("Code = {0}", parm_code.ParameterName));

            where.AppendLine("AND");
            where.AppendLine(string.Format("Date in (SELECT MAX(Date) from {0})", entity.TableName));

            var sql = new System.Text.StringBuilder();
            sql.AppendLine(entity.SQLTableSelect);
            sql.AppendLine(where.ToString());

            return accessor.CreateCommand(sql.ToString(), parameters);
        }
        
    }

    public class DBTStkDailyEntitySort : IComparer<DBTStkDailyEntity>
    {
        public int Compare(DBTStkDailyEntity x, DBTStkDailyEntity y)
        {
            return x.Date.Date == y.Date.Date ? 0 : x.Date.Date > y.Date.Date ? 1 : -1;
        }
    }
}
