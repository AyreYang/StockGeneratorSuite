using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SGDataService.Interfaces;
using SGNativeEntities.Enums;
using SGNativeEntities.General;
using SGDataService.Entities;

namespace SGDataService.DataServices.WebRequest
{
    public class SinaDataService : IDataService
    {
        private const string url = "http://hq.sinajs.cn/";

        private bool TryParse(string script, out StockInfoEntity entity)
        {
            entity = null;
            if (string.IsNullOrWhiteSpace(script)) return false;
            var match = Regex.Match(script, "^var [a-zA-Z0-9/_]+[=]{1}\"(?<data>.*)\";$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var result = match.Success;
            if (result)
            {
                var data = match.Groups["data"].Value;
                if (string.IsNullOrWhiteSpace(data)) return false;

                string[] temp = data.Split(',');
                if (temp.Length < 32) return false;

                int index = 0;
                entity = new StockInfoEntity();
                entity.Name = temp[index];
                entity.TodayOpen = decimal.Parse(temp[++index]);
                entity.YesterdayClose = decimal.Parse(temp[++index]);
                entity.Current = decimal.Parse(temp[++index]);
                entity.High = decimal.Parse(temp[++index]);
                entity.Low = decimal.Parse(temp[++index]);
                ++index; ++index;
                //entity.Buy = decimal.Parse(temp[++index]);
                //entity.Sell = decimal.Parse(temp[++index]);
                entity.GVolAmount = int.Parse(temp[++index]);
                entity.GVolMoney = decimal.Parse(temp[++index]);
                
                for (int i = 0; i < 5; i++) entity.BuyList.Add(new OrderItemEntity()
                {
                    Amount = Convert.ToDecimal(temp[++index]),
                    Price = Convert.ToDecimal(temp[++index]),
                    Flag = TRADE.buy
                });

                for (int i = 0; i < 5; i++) entity.SellList.Add(new OrderItemEntity()
                {
                    Amount = Convert.ToDecimal(temp[++index]),
                    Price = Convert.ToDecimal(temp[++index]),
                    Flag = TRADE.sale
                });
                //entity.Time = Convert.ToDateTime(string.Format("{0} {1}", temp[30].Replace('/', '-'), temp[31]));
                entity.Time = Convert.ToDateTime(string.Format("{0} {1}", temp[++index].Replace('/', '-'), temp[++index]));
            }
            return result;
        }

        public T FetchData<T>(string code) where T : BasicInfoEntity
        {
            var market = StockInfoEntity.TellMarket(code);
            if (market == MARKET.none) return null;
            var ncode = market.ToString() + code;

            var parameters = new Dictionary<string, string>();
            parameters.Add("list", ncode);
            var script = WebRequestor.Send(url, parameters, WebRequestor.RequestMethod.get, Encoding.GetEncoding("GB2312"));
            StockInfoEntity info = null;
            TryParse(script, out info);

            object result = info;
            return (T)result;
        }


        public List<T> FetchData<T>(List<string> codes) where T : BasicInfoEntity
        {
            throw new NotImplementedException();
        }
    }
}
