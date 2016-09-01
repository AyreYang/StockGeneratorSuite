using System;
using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;

namespace SGNativeEntities.Database
{
    public class DBTStkGeneralEntity : GENTableEntity
    {
        // 代码
        [DBField("Code", typeof(string), KeyType.Primary, false)]
        public string Code {
            get { return this.GetValue<string>("Code"); }
            set { if (IsValidCode(value, CODETYPE.stock)) this.SetValue("Code", value); }
        }
        // 名称
        [DBField("Name", typeof(string), KeyType.Normal, false)]
        public string Name {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }

        // 细分行业
        [DBField("Area", typeof(string), KeyType.Normal, true)]
        public string Area
        {
            get { return this.GetValue<string>("Area"); }
            set { this.SetValue("Area", value); }
        }

        // 流通股(亿)
        [DBField("CapitalStockInCirculation", typeof(decimal), KeyType.Normal, true)]
        public decimal CapitalStockInCirculation
        {
            get { return this.GetValue<decimal>("CapitalStockInCirculation"); }
            set { this.SetValue("CapitalStockInCirculation", value); }
        }

        // 总股本(亿)
        [DBField("GeneralCapital", typeof(decimal), KeyType.Normal, true)]
        public decimal GeneralCapital
        {
            get { return this.GetValue<decimal>("GeneralCapital"); }
            set { this.SetValue("GeneralCapital", value); }
        }

        // 上市日期
        [DBField("ListingDate", typeof(DateTime), KeyType.Normal, true)]
        public DateTime ListingDate
        {
            get { return this.GetValue<DateTime>("ListingDate"); }
            set { this.SetValue("ListingDate", value); }
        }

        // 营业收入
        [DBField("PrimaryIncome", typeof(decimal), KeyType.Normal, true)]
        public decimal PrimaryIncome
        {
            get { return this.GetValue<decimal>("PrimaryIncome"); }
            set { this.SetValue("PrimaryIncome", value); }
        }

        // 每股收益
        [DBField("EarningsPerShare", typeof(decimal), KeyType.Normal, true)]
        public decimal EarningsPerShare
        {
            get { return this.GetValue<decimal>("EarningsPerShare"); }
            set { this.SetValue("EarningsPerShare", value); }
        }

        // 每股净资
        [DBField("NetAssetPerShare", typeof(decimal), KeyType.Normal, true)]
        public decimal NetAssetPerShare
        {
            get { return this.GetValue<decimal>("NetAssetPerShare"); }
            set { this.SetValue("NetAssetPerShare", value); }
        }

        // 每股公积
        [DBField("AccumulationFundPerShare", typeof(decimal), KeyType.Normal, true)]
        public decimal AccumulationFundPerShare
        {
            get { return this.GetValue<decimal>("AccumulationFundPerShare"); }
            set { this.SetValue("AccumulationFundPerShare", value); }
        }

        // 每股未分配
        [DBField("UndistributedProfitPerShare", typeof(decimal), KeyType.Normal, true)]
        public decimal UndistributedProfitPerShare
        {
            get { return this.GetValue<decimal>("UndistributedProfitPerShare"); }
            set { this.SetValue("UndistributedProfitPerShare", value); }
        }

        // 净利润
        [DBField("NetProfit", typeof(decimal), KeyType.Normal, true)]
        public decimal NetProfit
        {
            get { return this.GetValue<decimal>("NetProfit"); }
            set { this.SetValue("NetProfit", value); }
        }

        // 财务更新
        [DBField("UpdateTime", typeof(DateTime), KeyType.Normal, true)]
        public DateTime UpdateTime
        {
            get { return this.GetValue<DateTime>("UpdateTime"); }
            set { this.SetValue("UpdateTime", value); }
        }

        public bool IsDataValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name) &&
                    !DateTime.MinValue.Equals(UpdateTime) && !DateTime.MaxValue.Equals(UpdateTime) &&
                    CapitalStockInCirculation > decimal.Zero && GeneralCapital > decimal.Zero;
            }
        }

        public DBTStkGeneralEntity() : base(TABLES.STK_GENERAL_M.ToString(), null) { }
        public DBTStkGeneralEntity(DatabaseAccessor accessor) : base(TABLES.STK_GENERAL_M.ToString(), accessor) { }
        public DBTStkGeneralEntity(string code, DatabaseAccessor accessor) : base(TABLES.STK_GENERAL_M.ToString(), accessor) {
            this.Code = code;
        }

        public override bool Equals(object obj)
        {
            var entity = obj as DBTStkGeneralEntity;
            if (entity == null) return false;
            return Code.Equals(entity.Code) && UpdateTime.Equals(entity.UpdateTime);
        }
        //public bool Exists()
        //{
        //    return this.ExistsBy("Code");
        //}
    }
}
