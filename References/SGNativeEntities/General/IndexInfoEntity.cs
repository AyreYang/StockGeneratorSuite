using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGNativeEntities.Enums;

namespace SGNativeEntities.General
{
    public class IndexInfoEntity : BasicInfoEntity
    {
        public override MARKET Market
        {
            get
            {
                return TellMarket(this.Code);
            }
        }

        public IndexInfoEntity()
            : base() { }
        public IndexInfoEntity(string code)
            : this()
        {
            if (IsValidCode(code)) Code = code;
        }


        public static bool IsValidCode(string code)
        {
            var ValidCodes = new string[] { "399006", "000001", "399001" };
            return (!string.IsNullOrWhiteSpace(code)) && ValidCodes.Contains(code);
        }


        public static MARKET TellMarket(string code)
        {
            var market = MARKET.none;
            if (IsValidCode(code))
            {
                switch (code)
                {
                    case "399006":          // 创业板指数
                        market = MARKET.sz;
                        break;
                    case "399001":          // 深成指
                        market = MARKET.sz;
                        break;
                    case "000001":          // 上综指
                        market = MARKET.sh;
                        break;
                }
            }
            return market;
        }
        public static string FormatedCode(string code)
        {
            var market = TellMarket(code);
            return (market == MARKET.none) ? null : string.Format("{0}{1}", market.ToString().ToUpper(), code.Trim());
        }

        public override T Clone<T>()
        {
            object entity = new IndexInfoEntity()
            {
                Code = this.Code,
                Name = this.Name,
                TodayOpen = this.TodayOpen,
                YesterdayClose = this.YesterdayClose,
                Current = this.Current,
                High = this.High,
                Low = this.Low,
                GVolAmount = this.GVolAmount,
                GVolMoney = this.GVolMoney,
                VolAmount = this.VolAmount,
                VolMoney = this.VolMoney,

                Time = this.Time
            };
            return (T)entity;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}[H:{2}-L:{3}][A:{4}-M:{5}]", Code, Current, High, Low, VolAmount, VolMoney);
        }
    }
}
