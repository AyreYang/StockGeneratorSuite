using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGNativeEntities.Enums;
using Newtonsoft.Json;

namespace SGDataService.DataServices.WebRequest
{
    public class JuheDataService
    {
        public string URL_ALL = @"http://web.juhe.cn:8080/finance/stock/{0}all";
        private string AppKey { get; set; }     //AppKey:02274aa08815755fcdfd584de5f2cb78

        public JuheDataService(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new Exception("AppKey is null or empty.");
            AppKey = key;
        }

        public AllStockPackage FetchDataAll(MARKET market, int page)
        {
            if (market == MARKET.none) return null;
            if (page <= 0) return null;

            AllStockPackage data = null;

            var url = string.Format(URL_ALL, market.ToString());
            var parameters = new Dictionary<string, string>();
            parameters.Add("key", AppKey);
            parameters.Add("page", page.ToString());
            var script = WebRequestor.Send(url, parameters, WebRequestor.RequestMethod.get, Encoding.UTF8);
            if (!string.IsNullOrWhiteSpace(script)) data = JsonConvert.DeserializeObject<AllStockPackage>(script);

            return data;
        }
    }

    public class AllStockPackage
    {
        public int error_code { get; set; }
        public string reason { get; set; }
        public AllStockResultPackage result { get; set; }
    }

    public class AllStockResultPackage
    {
        public string totalCount { get; set; }
        public int TotalCount
        {
            get
            {
                var count = 0;
                int.TryParse(totalCount, out count);
                return count;
            }
        }
        public string page { get; set; }
        public int Page
        {
            get
            {
                var count = 0;
                int.TryParse(page, out count);
                return count;
            }
        }
        public string num { get; set; }
        public int NumberPerPage
        {
            get
            {
                var count = 0;
                int.TryParse(num, out count);
                return count;
            }
        }

        public List<StockData> data { get; set; }
    }

    public class StockData
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public string trade { get; set; }
        public string pricechange { get; set; }
        public string changepercent { get; set; }
        public string buy { get; set; }
        public string sell { get; set; }
        public string settlement { get; set; }
        public string open { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string volume { get; set; }
        public string amount { get; set; }
        public string code { get; set; }
        public string ticktime { get; set; }
    }
}
