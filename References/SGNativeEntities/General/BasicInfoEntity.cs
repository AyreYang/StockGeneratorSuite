using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGNativeEntities.Enums;

namespace SGNativeEntities.General
{
    public abstract class BasicInfoEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal TodayOpen { get; set; }
        public decimal YesterdayClose { get; set; }
        public abstract MARKET Market { get; }

        public decimal Current { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }

        public decimal GVolAmount { get; set; }     // 成交量 单位:手
        public decimal GVolMoney { get; set; }      // 成交额 单位:万
        public decimal VolAmount { get; set; }
        public decimal VolMoney { get; set; }

        public DateTime Time { get; set; }

        public bool IsValid
        {
            get
            {
                return TodayOpen > decimal.Zero && GVolAmount > decimal.Zero && GVolMoney > decimal.Zero;
            }
        }
        public bool IsTodayData
        {
            get
            {
                return DateTime.Today.Date.Equals(Time.Date) && IsValid;
            }
        }

        public BasicInfoEntity()
        {
            Code = string.Empty;
            Name = string.Empty;
            TodayOpen = decimal.Zero;
            YesterdayClose = decimal.Zero;

            Current = decimal.Zero;
            High = decimal.Zero;
            Low = decimal.Zero;

            GVolAmount = decimal.Zero;
            GVolMoney = decimal.Zero;
            VolAmount = decimal.Zero;
            VolMoney = decimal.Zero;

            Time = DateTime.Now;
        }

        public abstract T Clone<T>()where T : BasicInfoEntity;

        public override string ToString()
        {
            return string.Format("{0}:{1}[H:{2}-L:{3}][A:{4}-M:{5}]", Code, Current, High, Low, VolAmount, VolMoney);
        }
    }
}
