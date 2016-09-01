using System;
using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;
using SGNativeEntities.General;

namespace SGNativeEntities.Database
{
    public class DBTStkRealtimeDetailEntity : SEQTableEntity
    {
        private const long ticks = 10000000;

        [DBField("ID", typeof(long), KeyType.Primary, false)]
        public long ID {
            get { return this.GetValue<long>("ID"); }
            set { this.SetValue("ID", value); }
        }
        [DBField("Price", typeof(decimal), KeyType.Normal, false)]
        public decimal Price
        {
            get { return this.GetValue<decimal>("Price"); }
            set { this.SetValue("Price", value); }
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

        [DBField("Flag", typeof(int), KeyType.Normal, false)]
        public int Flag
        {
            get { return this.GetValue<int>("Flag"); }
            set { this.SetValue("Flag", value); }
        }

        [DBField("Time", typeof(DateTime), KeyType.Normal, false)]
        public DateTime Time {
            get { return this.GetValue<DateTime>("Time"); }
            set { this.SetValue("Time", value); }
        }

        public DBTStkRealtimeDetailEntity(string code, DatabaseAccessor accessor = null) : base(TableName(code), accessor) { }
        public static DBTStkRealtimeDetailEntity From(ItemInfoEntity info)
        {
            if (info == null || string.IsNullOrWhiteSpace(info.Code)) return null;
            var entity = new DBTStkRealtimeDetailEntity(info.Code);
            entity.Price = info.Value;
            entity.Flag = (int)info.Type;
            entity.VolAmount = info.Amount;
            entity.VolMoney = info.Money;
            entity.Time = info.Time;
            return entity;
        }
        public static string TableName(string code)
        {
            if (!IsValidCode(code, CODETYPE.stock)) throw new Exception(string.Format("Code({0}) is invalid.", code ?? string.Empty));
            return string.Format("{0}_{1}", TABLES.STK_REALTIME_TD, code.Trim().ToUpper());
        }
        public string Code
        {
            get
            {
                var code = FetchCode(base.TableName);
                return IsValidCode(code, CODETYPE.stock) ? code : string.Empty;
            }
        }

        public override void GenerateID()
        {
            if (ID <= 0) ID = this.GenerateSequence();
        }
    }
}
