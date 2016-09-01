using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;

namespace SGNativeEntities.Database
{
    public class DBStkRSIEntity : GENTableEntity
    {
        [DBField("ID", typeof(long), KeyType.Primary, false)]
        public long ID
        {
            get { return this.GetValue<long>("ID"); }
            private set { this.SetValue("ID", value); }
        }
        [DBField("RSI1", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI1
        {
            get { return this.GetValue<decimal>("RSI1"); }
            set { this.SetValue("RSI1", value); }
        }
        [DBField("RSI2", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI2
        {
            get { return this.GetValue<decimal>("RSI2"); }
            set { this.SetValue("RSI2", value); }
        }
        [DBField("RSI3", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI3
        {
            get { return this.GetValue<decimal>("RSI3"); }
            set { this.SetValue("RSI3", value); }
        }
        [DBField("RSI1MaxEma", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI1MaxEma
        {
            get { return this.GetValue<decimal>("RSI1MaxEma"); }
            set { this.SetValue("RSI1MaxEma", value); }
        }
        [DBField("RSI1ABSEma", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI1ABSEma
        {
            get { return this.GetValue<decimal>("RSI1ABSEma"); }
            set { this.SetValue("RSI1ABSEma", value); }
        }

        [DBField("RSI2MaxEma", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI2MaxEma
        {
            get { return this.GetValue<decimal>("RSI2MaxEma"); }
            set { this.SetValue("RSI2MaxEma", value); }
        }
        [DBField("RSI2ABSEma", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI2ABSEma
        {
            get { return this.GetValue<decimal>("RSI2ABSEma"); }
            set { this.SetValue("RSI2ABSEma", value); }
        }

        [DBField("RSI3MaxEma", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI3MaxEma
        {
            get { return this.GetValue<decimal>("RSI3MaxEma"); }
            set { this.SetValue("RSI3MaxEma", value); }
        }
        [DBField("RSI3ABSEma", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI3ABSEma
        {
            get { return this.GetValue<decimal>("RSI3ABSEma"); }
            set { this.SetValue("RSI3ABSEma", value); }
        }

        public DBStkRSIEntity() : this(null) { }
        public DBStkRSIEntity(long id) : this(id, null) { }
        public DBStkRSIEntity(DatabaseAccessor accessor) : this(0, accessor) { }
        public DBStkRSIEntity(long id, DatabaseAccessor accessor)
            : base(TABLES.STK_RSI_TD.ToString(), accessor)
        {
            this.ID = id;
        }
    }
}
