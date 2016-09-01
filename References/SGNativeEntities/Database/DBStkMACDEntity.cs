using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;

namespace SGNativeEntities.Database
{
    public class DBStkMACDEntity : GENTableEntity
    {
        [DBField("ID", typeof(long), KeyType.Normal, false)]
        public long ID
        {
            get { return this.GetValue<long>("ID"); }
            private set { this.SetValue("ID", value); }
        }
        [DBField("EMA12", typeof(decimal), KeyType.Normal, false)]
        public decimal EMA12
        {
            get { return this.GetValue<decimal>("EMA12"); }
            set { this.SetValue("EMA12", value); }
        }
        [DBField("EMA26", typeof(decimal), KeyType.Normal, false)]
        public decimal EMA26
        {
            get { return this.GetValue<decimal>("EMA26"); }
            set { this.SetValue("EMA26", value); }
        }
        [DBField("DIFF", typeof(decimal), KeyType.Normal, false)]
        public decimal DIFF
        {
            get { return this.GetValue<decimal>("DIFF"); }
            set { this.SetValue("DIFF", value); }
        }
        [DBField("DEA", typeof(decimal), KeyType.Normal, false)]
        public decimal DEA
        {
            get { return this.GetValue<decimal>("DEA"); }
            set { this.SetValue("DEA", value); }
        }
        [DBField("BAR", typeof(decimal), KeyType.Normal, false)]
        public decimal BAR
        {
            get { return this.GetValue<decimal>("BAR"); }
            set { this.SetValue("BAR", value); }
        }

        public DBStkMACDEntity() : this(null) { }
        public DBStkMACDEntity(long id) : this(id, null) { }
        public DBStkMACDEntity(DatabaseAccessor accessor) : this(0, accessor) { }
        public DBStkMACDEntity(long id, DatabaseAccessor accessor)
            : base(TABLES.STK_MACD_TD.ToString(), accessor)
        {
            this.ID = id;
        }
    }
}
