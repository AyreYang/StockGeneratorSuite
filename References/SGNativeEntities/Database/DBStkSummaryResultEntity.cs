using DataBase.common.attributes;
using DataBase.common.enums;
using System;
using SGNativeEntities.Enums;
using DataBase.postgresql;

namespace SGNativeEntities.Database
{
    public class DBStkSummaryResultEntity : SEQTableEntity
    {
        [DBField("ID", typeof(int), KeyType.Primary, false)]
        public int ID
        {
            get { return this.GetValue<int>("ID"); }
            set { this.SetValue("ID", value); }
        }
        [DBField("Code", typeof(string), KeyType.Normal, false)]
        public string Code
        {
            get { return this.GetValue<string>("Code"); }
            set { if (IsValidCode(value, CODETYPE.stock)) this.SetValue("Code", value); }
        }
        [DBField("Time", typeof(DateTime), KeyType.Normal, false)]
        public DateTime Time
        {
            get { return this.GetValue<DateTime>("Time"); }
            set { this.SetValue("Time", value); }
        }

        #region PAVG30
        [DBField("PAVG30_ORIENT", typeof(int), KeyType.Normal, false)]
        public int PAVG30_ORIENT
        {
            get { return this.GetValue<int>("PAVG30_ORIENT"); }
            set { this.SetValue("PAVG30_ORIENT", value); }
        }
        [DBField("PAVG30_DAYS", typeof(int), KeyType.Normal, false)]
        public int PAVG30_DAYS
        {
            get { return this.GetValue<int>("PAVG30_DAYS"); }
            set { this.SetValue("PAVG30_DAYS", value); }
        }
        [DBField("PAVG30_VALUE", typeof(decimal), KeyType.Normal, false)]
        public decimal PAVG30_VALUE
        {
            get { return this.GetValue<decimal>("PAVG30_VALUE"); }
            set { this.SetValue("PAVG30_VALUE", value); }
        }
        [DBField("PAVG30_Date1", typeof(DateTime), KeyType.Normal, false)]
        public DateTime PAVG30_Date1
        {
            get { return this.GetValue<DateTime>("PAVG30_Date1"); }
            set { this.SetValue("PAVG30_Date1", value); }
        }
        [DBField("PAVG30_Date2", typeof(DateTime), KeyType.Normal, false)]
        public DateTime PAVG30_Date2
        {
            get { return this.GetValue<DateTime>("PAVG30_Date2"); }
            set { this.SetValue("PAVG30_Date2", value); }
        }
        #endregion

        #region DEA
        [DBField("DEA_ORIENT", typeof(int), KeyType.Normal, false)]
        public int DEA_ORIENT
        {
            get { return this.GetValue<int>("DEA_ORIENT"); }
            set { this.SetValue("DEA_ORIENT", value); }
        }
        [DBField("DEA_DAYS", typeof(int), KeyType.Normal, false)]
        public int DEA_DAYS
        {
            get { return this.GetValue<int>("DEA_DAYS"); }
            set { this.SetValue("DEA_DAYS", value); }
        }
        [DBField("DEA_VALUE", typeof(decimal), KeyType.Normal, false)]
        public decimal DEA_VALUE
        {
            get { return this.GetValue<decimal>("DEA_VALUE"); }
            set { this.SetValue("DEA_VALUE", value); }
        }
        [DBField("DEA_Date1", typeof(DateTime), KeyType.Normal, false)]
        public DateTime DEA_Date1
        {
            get { return this.GetValue<DateTime>("DEA_Date1"); }
            set { this.SetValue("DEA_Date1", value); }
        }
        [DBField("DEA_Date2", typeof(DateTime), KeyType.Normal, false)]
        public DateTime DEA_Date2
        {
            get { return this.GetValue<DateTime>("DEA_Date2"); }
            set { this.SetValue("DEA_Date2", value); }
        }
        #endregion

        #region RSI
        [DBField("RSI_VALUE1", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI_VALUE1
        {
            get { return this.GetValue<decimal>("RSI_VALUE1"); }
            set { this.SetValue("RSI_VALUE1", value); }
        }
        [DBField("RSI_VALUE2", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI_VALUE2
        {
            get { return this.GetValue<decimal>("RSI_VALUE2"); }
            set { this.SetValue("RSI_VALUE2", value); }
        }
        [DBField("RSI_VALUE3", typeof(decimal), KeyType.Normal, false)]
        public decimal RSI_VALUE3
        {
            get { return this.GetValue<decimal>("RSI_VALUE3"); }
            set { this.SetValue("RSI_VALUE3", value); }
        }
        [DBField("RSI_Date", typeof(DateTime), KeyType.Normal, false)]
        public DateTime RSI_Date
        {
            get { return this.GetValue<DateTime>("RSI_Date"); }
            set { this.SetValue("RSI_Date", value); }
        }
        #endregion

        public DBStkSummaryResultEntity() : this(null) { }
        public DBStkSummaryResultEntity(DatabaseAccessor accessor = null) : base(TABLES.STK_SUM_RESULT_TD.ToString(), accessor) { }

        public override void GenerateID()
        {
            if (ID <= 0) ID = (int)this.GenerateSequence();
        }
    }
}
