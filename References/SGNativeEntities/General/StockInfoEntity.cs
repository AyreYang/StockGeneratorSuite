using System;
using System.Collections.Generic;
using System.Linq;
using SGNativeEntities.Enums;

namespace SGNativeEntities.General
{
    public class StockInfoEntity : BasicInfoEntity
    {
        protected int BuyIndex = 0;
        protected int SellIndex = 0;

        public override MARKET Market
        {
            get
            {
                return TellMarket(this.Code);
            }
        }

        public decimal TurnoverRatio { get; set; }  // 换手率
        public decimal PBRatio { get; set; }        // 市净率 Price-To-Book Ratio
        public decimal PERatio { get; set; }        // 市盈率 Price-To-Earning Ratio

        public List<OrderItemEntity> BuyList { get; protected set; }
        public List<OrderItemEntity> SellList { get; protected set; }
        public decimal Buy
        {
            get
            {
                decimal price = Current;
                if (BuyList != null && BuyList.Count > 0)
                {
                    price = (BuyIndex < 0) ? BuyList.First().Price : (BuyIndex < BuyList.Count) ? BuyList[BuyIndex].Price : BuyList.Last().Price;
                }
                return price;
            }
        }
        public decimal Sell {
            get
            {
                decimal price = Current;
                if (SellList != null && SellList.Count > 0)
                {
                    price = (SellIndex < 0) ? SellList.First().Price : (SellIndex < SellList.Count) ? SellList[SellIndex].Price : SellList.Last().Price;
                }
                return price;
            }
        }
        
        public TRADE TradeFalg
        {
            get
            {
                var flag = TRADE.none;
                if (Current.Equals(Buy)) flag = TRADE.sale;
                if (Current.Equals(Sell)) flag = TRADE.buy;
                if (Buy == Sell) flag = TRADE.middle;
                return flag;
            }
        }

        public bool IsValidTrade
        {
            get
            {
                return TradeFalg != TRADE.none && VolAmount > decimal.Zero && VolMoney > decimal.Zero;
            }
        }

        public StockInfoEntity() : base()
        {
            TurnoverRatio = decimal.Zero;
            PBRatio = decimal.Zero;
            PERatio = decimal.Zero;

            BuyList = new List<OrderItemEntity>();
            SellList = new List<OrderItemEntity>();
        }
        public StockInfoEntity(string code):this()
        {
            if (IsValidCode(code)) Code = code;
        }

        public static bool IsValidCode(string code)
        {
            return (!string.IsNullOrWhiteSpace(code)) && System.Text.RegularExpressions.Regex.IsMatch(code, @"^\d{6}$");
        }
        public static MARKET TellMarket(string code)
        {
            var market = MARKET.none;
            if (IsValidCode(code))
            {
                var prefix = code.Substring(0, 2);
                if (prefix == "51")
                {
                    market = MARKET.sh;
                }
                else
                {
                    if (Convert.ToInt32(prefix) >= 60)
                    {
                        market = MARKET.sh;
                    }
                    else
                    {
                        market = MARKET.sz;
                    }
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
            object entity = new StockInfoEntity()
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

                TurnoverRatio = this.TurnoverRatio,
                PBRatio = this.PBRatio,
                PERatio = this.PERatio,
                BuyList = new List<OrderItemEntity>(this.BuyList),
                SellList = new List<OrderItemEntity>(this.SellList),
                Time = this.Time
            };
            return (T)entity;
        }

    }

}
