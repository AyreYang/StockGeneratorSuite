using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGDataService.Interfaces;
using SGNativeEntities.General;
using SGNativeEntities.Enums;
using System.Text.RegularExpressions;
using SGDataService.Entities;

namespace SGDataService.DataServices.WebRequest
{
    public class TengxunDataService : IDataService
    {
        private const string url = "http://qt.gtimg.cn/";

        private bool TryParse(string script, out TengxunStockInfoEntity entity)
        {
            entity = null;
            if (string.IsNullOrWhiteSpace(script)) return false;
            var match = Regex.Match(script, "^[a-zA-Z0-9/_]+[=]{1}\"(?<data>.*)\";$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var result = match.Success;
            if (result)
            {
                var data = match.Groups["data"].Value;
                if (string.IsNullOrWhiteSpace(data)) return false;

                string[] temp = data.Split('~');
                if (temp.Length < 50) return false;

                int index = 0;
                entity = new TengxunStockInfoEntity();
                entity.Name = temp[++index];                                                                                            // index:1
                entity.Code = temp[++index];                                                                                            // index:2
                entity.Current = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : decimal.Parse(temp[index]);                          // index:3
                entity.YesterdayClose = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : decimal.Parse(temp[index]);                   // index:4
                entity.TodayOpen = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : decimal.Parse(temp[index]);                        // index:5
                ++index; ++index; ++index;

                for (int i = 0; i < 5; i++) entity.BuyList.Add(new OrderItemEntity()
                {
                    Price = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : Convert.ToDecimal(temp[index]),
                    Amount = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : Convert.ToDecimal(temp[index]),
                    Flag = TRADE.buy
                });

                for (int i = 0; i < 5; i++) entity.SellList.Add(new OrderItemEntity()
                {
                    Price = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : Convert.ToDecimal(temp[index]),
                    Amount = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : Convert.ToDecimal(temp[index]),
                    Flag = TRADE.sale
                });

                ++index; // the lateset 5 trades

                var stime = temp[++index];
                var year = Convert.ToInt32(stime.Substring(0, 4));
                var month = Convert.ToInt32(stime.Substring(4, 2));
                var day = Convert.ToInt32(stime.Substring(6, 2));
                var hour = Convert.ToInt32(stime.Substring(8, 2));
                var minute = Convert.ToInt32(stime.Substring(10, 2));
                var second = Convert.ToInt32(stime.Substring(12, 2));
                entity.Time = new DateTime(year, month, day, hour, minute, second);
                ++index; ++index;

                entity.High = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : decimal.Parse(temp[index]);
                entity.Low = (string.IsNullOrWhiteSpace(temp[++index])) ? 0m : decimal.Parse(temp[index]);

                entity.TurnoverRatio = (string.IsNullOrWhiteSpace(temp[38])) ? 0m : decimal.Parse(temp[38]);
                entity.PBRatio = (string.IsNullOrWhiteSpace(temp[46])) ? 0m : decimal.Parse(temp[46]);
                entity.PERatio = (string.IsNullOrWhiteSpace(temp[39])) ? 0m : decimal.Parse(temp[39]);

                entity.GVolAmount = (string.IsNullOrWhiteSpace(temp[36])) ? 0m : int.Parse(temp[36]);
                entity.GVolMoney = (string.IsNullOrWhiteSpace(temp[37])) ? 0m : decimal.Parse(temp[37]);

                var date = entity.Time.Date.ToString("yyyy-MM-dd");
                var list = entity.TradeList;
                var subscript = temp[29];
                var scripts = subscript.Split(new char[] { '|' });
                var code = entity.Code;
                if(scripts != null && scripts.Length > 0)new List<string>(scripts).ForEach(s =>
                {
                    var scrpts = s.Split(new char[] { '/' });
                    if (scrpts.Length < 6) return;
                    var ent = new ItemInfoEntity(code);
                    ent.CodeType = CODETYPE.stock;
                    ent.Time = DateTime.Parse(string.Format("{0} {1}", date, scrpts[0]));
                    ent.Value = (string.IsNullOrWhiteSpace(scrpts[1])) ? 0m : decimal.Parse(scrpts[1]);
                    ent.Amount = (string.IsNullOrWhiteSpace(scrpts[2])) ? 0m : decimal.Parse(scrpts[2]);
                    ent.Type = (string.IsNullOrWhiteSpace(scrpts[3])) ? TRADE.none : "S".Equals(scrpts[3].Trim().ToUpper()) ? TRADE.sale : "B".Equals(scrpts[3].Trim().ToUpper()) ? TRADE.buy : "M".Equals(scrpts[3].Trim().ToUpper()) ? TRADE.middle : TRADE.none;
                    ent.Money = (string.IsNullOrWhiteSpace(scrpts[4])) ? 0m : decimal.Parse(scrpts[4]);
                    list.Add(ent);
                });

                if (entity.TradeList != null && entity.TradeList.Count > 0) entity.TradeList.Sort(new TradeSort());
                
            }
            return result;
        }
        private bool TryParse(string script, out TengxunIndexInfoEntity entity)
        {
            entity = null;
            if (string.IsNullOrWhiteSpace(script)) return false;
            var match = Regex.Match(script, "^[a-zA-Z0-9/_]+[=]{1}\"(?<data>.*)\";$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var result = match.Success;
            if (result)
            {
                var data = match.Groups["data"].Value;
                if (string.IsNullOrWhiteSpace(data)) return false;

                string[] temp = data.Split('~');
                if (temp.Length < 50) return false;

                entity = new TengxunIndexInfoEntity();
                entity.Name = temp[1];                                                                     
                entity.Code = temp[2];                                                                     
                entity.Current = (string.IsNullOrWhiteSpace(temp[3])) ? 0m : decimal.Parse(temp[3]);       
                entity.YesterdayClose = (string.IsNullOrWhiteSpace(temp[4])) ? 0m : decimal.Parse(temp[4]);
                entity.TodayOpen = (string.IsNullOrWhiteSpace(temp[5])) ? 0m : decimal.Parse(temp[5]);     

                var stime = temp[30];
                var year = Convert.ToInt32(stime.Substring(0, 4));
                var month = Convert.ToInt32(stime.Substring(4, 2));
                var day = Convert.ToInt32(stime.Substring(6, 2));
                var hour = Convert.ToInt32(stime.Substring(8, 2));
                var minute = Convert.ToInt32(stime.Substring(10, 2));
                var second = Convert.ToInt32(stime.Substring(12, 2));
                entity.Time = new DateTime(year, month, day, hour, minute, second);

                entity.High = (string.IsNullOrWhiteSpace(temp[33])) ? 0m : decimal.Parse(temp[33]);
                entity.Low = (string.IsNullOrWhiteSpace(temp[34])) ? 0m : decimal.Parse(temp[34]);

                entity.GVolAmount = (string.IsNullOrWhiteSpace(temp[36])) ? 0m : int.Parse(temp[36]);
                entity.GVolMoney = (string.IsNullOrWhiteSpace(temp[37])) ? 0m : decimal.Parse(temp[37]);

                var date = entity.Time.Date.ToString("yyyy-MM-dd");
                var list = entity.IndexList;
                var subscript = temp[29];
                var scripts = subscript.Split(new char[] { '|' });
                var code = entity.Code;
                if (scripts != null && scripts.Length > 0) new List<string>(scripts).ForEach(s =>
                {
                    var scrpts = s.Split(new char[] { '/' });
                    if (scrpts.Length < 6) return;
                    var ent = new ItemInfoEntity(code);
                    ent.CodeType = CODETYPE.index;
                    ent.Time = DateTime.Parse(string.Format("{0} {1}", date, scrpts[0]));
                    ent.Value = (string.IsNullOrWhiteSpace(scrpts[1])) ? 0m : decimal.Parse(scrpts[1]);
                    ent.Amount = (string.IsNullOrWhiteSpace(scrpts[2])) ? 0m : decimal.Parse(scrpts[2]);
                    ent.Type = (string.IsNullOrWhiteSpace(scrpts[3])) ? TRADE.none : "S".Equals(scrpts[3].Trim().ToUpper()) ? TRADE.sale : "B".Equals(scrpts[3].Trim().ToUpper()) ? TRADE.buy : "M".Equals(scrpts[3].Trim().ToUpper()) ? TRADE.middle : TRADE.none;
                    ent.Money = (string.IsNullOrWhiteSpace(scrpts[4])) ? 0m : decimal.Parse(scrpts[4]);
                    list.Add(ent);
                });
                if (entity.IndexList != null && entity.IndexList.Count > 0) entity.IndexList.Sort(new IndexSort());
            }
            return result;
        }


        public T FetchData<T>(string code) where T : BasicInfoEntity
        {
            return FetchData<T>(new List<string>() { code } ).FirstOrDefault();
        }

        public List<T> FetchData<T>(List<string> codes) where T : BasicInfoEntity
        {
            var list = new List<T>();

            if (typeof(T) == typeof(TengxunIndexInfoEntity))
            {
                GetIndexData(codes).ForEach(itm =>
                {
                    object tmp = itm;
                    list.Add((T)tmp);
                });
            }

            if (typeof(T) == typeof(TengxunStockInfoEntity))
            {
                GetStockData(codes).ForEach(itm =>
                {
                    object tmp = itm;
                    list.Add((T)tmp);
                });
            }
            return list;
        }

        private List<TengxunStockInfoEntity> GetStockData(List<string> codes)
        {
            var separator = Convert.ToChar(10);
            var list = new List<TengxunStockInfoEntity>();
            var container = new List<string>();
            if (codes != null || codes.Count > 0)
            {
                codes.ForEach(cd =>
                {
                    var code = cd.Trim().ToLower();
                    if (!container.Contains(code)) container.Add(code);
                });
                var scodes = string.Join(",", container);

                //var sb = new StringBuilder();
                //container.ForEach(cd =>
                //{
                //    var market = TengxunStockInfoEntity.TellMarket(cd);
                //    if (market == MARKET.none) return;
                //    var ncode = market.ToString() + cd;
                //    if (sb.Length > 0) sb.Append(",");
                //    sb.Append(ncode);
                //});

                var parameters = new Dictionary<string, string>();
                parameters.Add("q", scodes);
                var script = WebRequestor.Send(url, parameters, WebRequestor.RequestMethod.get, Encoding.GetEncoding("GB2312"));

                var scripts = (!string.IsNullOrWhiteSpace(script)) ? new List<string>(script.Split(new char[] { separator })) : new List<string>();
                scripts.ForEach(scrpt =>
                {
                    TengxunStockInfoEntity info = null;
                    TryParse(scrpt, out info);
                    if (info != null) list.Add(info);
                });

            }
            return list;
        }
        private List<TengxunIndexInfoEntity> GetIndexData(List<string> codes)
        {
            var separator = Convert.ToChar(10);
            var list = new List<TengxunIndexInfoEntity>();
            var container = new List<string>();
            if (codes != null || codes.Count > 0)
            {
                codes.ForEach(cd =>
                {
                    var code = cd.Trim().ToLower();
                    if (!container.Contains(code)) container.Add(code);
                });
                var scodes = string.Join(",", container);

                //var sb = new StringBuilder();
                //container.ForEach(cd =>
                //{
                //    var market = TengxunIndexInfoEntity.TellMarket(cd);
                //    if (market == MARKET.none) return;
                //    var ncode = market.ToString() + cd;
                //    if (sb.Length > 0) sb.Append(",");
                //    sb.Append(ncode);
                //});

                var parameters = new Dictionary<string, string>();
                parameters.Add("q", scodes);
                var script = WebRequestor.Send(url, parameters, WebRequestor.RequestMethod.get, Encoding.GetEncoding("GB2312"));

                var scripts = (!string.IsNullOrWhiteSpace(script)) ? new List<string>(script.Split(new char[] { separator })) : new List<string>();
                scripts.ForEach(scrpt =>
                {
                    TengxunIndexInfoEntity info = null;
                    TryParse(scrpt, out info);
                    if (info != null) list.Add(info);
                });

            }
            return list;
        }
    }
}
