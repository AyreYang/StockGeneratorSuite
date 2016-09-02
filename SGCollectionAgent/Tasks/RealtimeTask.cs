using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGCollectionAgent.Configuration;
using SGDataService.DataServices.WebRequest;
using SGDataService.Entities;
using SGDataService.Interfaces;
using SGNativeEntities.Database;
using SGNativeEntities.Enums;
using SGNativeEntities.General;
using SGUtilities.Cache;
using SGUtilities.Lines;
using Task.common.enums;

namespace SGCollectionAgent.Tasks
{
    public class RealtimeTask : Task
    {
        public static string ID = "Realtime-Task";

        private IDataService service = null;
        private List<string> tables = null;
        private FilterCache<ItemInfoEntity> filter4idx = null;
        private FilterCache<ItemInfoEntity> filter4stk = null;
        private Dictionary<string, TengxunMinuteLine> lines = null;
        private Dictionary<string, DBTStkDailyEntity> LastDayEntities = null;
        private Dictionary<string, TengxunStockInfoEntity> CurrentEntities = null;

        public RealtimeTask()
            : base(ID) { }

        protected override RESULT Initial(StringBuilder messager)
        {
            LastDayEntities = new Dictionary<string, DBTStkDailyEntity>();
            CurrentEntities = new Dictionary<string, TengxunStockInfoEntity>();
            service = new TengxunDataService();
            tables = new List<string>();
            filter4idx = new FilterCache<ItemInfoEntity>(1000);
            filter4stk = new FilterCache<ItemInfoEntity>(1000);

            lines = new Dictionary<string, TengxunMinuteLine>();
            return base.Initial(messager);
        }

        protected override RESULT Process(StringBuilder messager)
        {
            var working = Config.Instance.INFO.TimeSetting.IsWorkingTime;

            if (working > 0)
            {
                // Retrieve Code Data From DB
                var stocks = new List<string>();
                if (CheckTable<DBTStkFavoriteEntity>()) accessor.RetrieveEntity<DBTStkFavoriteEntity>().ForEach(data => stocks.Add(StockInfoEntity.FormatedCode(data.Code)));
                if (stocks.Count > 0)
                {
                    var list = service.FetchData<TengxunStockInfoEntity>(stocks);
                    foreach (TengxunStockInfoEntity info in list) ProcessStockInfo(info);
                }
            }
            else
            {
                RecordDailyInfo();
                LastDayEntities.Clear();
                Config.Instance.INFO.TimeSetting.Clear();
                tables.Clear();
                filter4idx.Clear();
                filter4stk.Clear();

                lines.Values.ToList().ForEach(line => line.TriggerSwitch());
                lines.Values.ToList().ForEach(line => line.Clear());
            }

            return RESULT.OK;
        }

        private void SaveEntity(TimeLine<ItemInfoEntity> line)
        {
            if (CheckTable<DBTStkMinuteEntity>())
            {
                var entity = new DBTStkMinuteEntity(accessor);
                entity.Code = line.Value.Code;
                
                if (line is TengxunMinuteLine)
                {
                    var tengxunline = line as TengxunMinuteLine;
                    entity.Time = new DateTime(tengxunline.CloseTime.Year, tengxunline.CloseTime.Month, tengxunline.CloseTime.Day, tengxunline.CloseTime.Hour, tengxunline.CloseTime.Minute, 0);
                    entity.Open = tengxunline.Open;
                    entity.Close = tengxunline.Close;
                    entity.High = tengxunline.High;
                    entity.Low = tengxunline.Low;
                    entity.Average = tengxunline.Average;
                    entity.VolAmount = tengxunline.VolAmount;
                    entity.VolMoney = tengxunline.VolMoney;
                }
            
                entity.GenerateID();
                entity.Save();
            }

        }
        private void ProcessStockInfo(TengxunStockInfoEntity info)
        {
            if (info == null || !info.IsTodayData || info.TradeList.Count <= 0) return;
            if (!LastDayEntities.ContainsKey(info.Code))
            {
                var entity = accessor.RetrieveEntity<DBTStkDailyEntity>(DBTStkDailyEntity.LastDailyEntityCommand(info.Code, accessor)).FirstOrDefault();
                if (entity != null)
                {
                    if (entity.Close != info.YesterdayClose)
                    {
                        entity.Close = info.YesterdayClose;
                        entity.Save();
                    }
                    LastDayEntities.Add(entity.Code, entity);
                }
            }
            if (!CurrentEntities.ContainsKey(info.Code))
            {
                CurrentEntities.Add(info.Code, info);
            }
            else
            {
                CurrentEntities[info.Code] = info;
            }
            if (!lines.ContainsKey(info.Code)) lines.Add(info.Code, new TengxunMinuteLine(SaveEntity));
            foreach (ItemInfoEntity item in info.TradeList)
            {
                if (!lines[info.Code].IsReady) lines[info.Code].Initialize(item.Time);
                if (filter4stk.Add(item)) lines[info.Code].Add(item);
            }
        }
        private void RecordDailyInfo()
        {
            if (CurrentEntities.Count <= 0) return;
            if (!CheckTable<DBTStkDailyEntity>()) return;

            CurrentEntities.Values.ToList().ForEach(info =>
            {
                var entity = new DBTStkDailyEntity(info.Code, info.Time.Date, accessor);
                entity.Open = info.TodayOpen;
                entity.Close = info.Current;
                entity.High = info.High;
                entity.Low = info.Low;
                entity.VolAmount = info.GVolAmount;
                entity.VolMoney = info.GVolMoney;

                entity.GenerateID();
                entity.Save();
            });
            CurrentEntities.Clear();
        }
        private bool CheckTable<T>() where T : GENTableEntity, new()
        {
            var entity = GENTableEntity.Create<T>();
            if (tables.Contains(entity.TableName)) return true;

            var script = string.Empty;
            var scripts = Config.Instance.INFO.ScriptSetting.Scripts;
            if (entity is DBTStkMinuteEntity) script = scripts.ContainsKey(TABLES.STK_MINUTE_TD.ToString()) ? scripts[TABLES.STK_MINUTE_TD.ToString()] : null;
            if (entity is DBTStkDailyEntity) script = scripts.ContainsKey(TABLES.STK_DAILY_TD.ToString()) ? scripts[TABLES.STK_DAILY_TD.ToString()] : null;
            if (entity is DBTStkFavoriteEntity) script = scripts.ContainsKey(TABLES.STK_FAVORITE_M.ToString()) ? scripts[TABLES.STK_FAVORITE_M.ToString()] : null;

            var check = false;
            accessor.SetDBAccessor2(entity);
            if (check = entity.CreateTable(script)) tables.Add(entity.TableName);
            return check;
        }
    }


    internal class TengxunMinuteLine : MinuteLine<ItemInfoEntity>
    {
        public TengxunMinuteLine(Action<TimeLine<ItemInfoEntity>> evnt)
            : base(
                (o) =>
                {
                    return o.Time;
                },
                (o) =>
                {
                    return o.Value;
                },evnt
            ) 
        {
            OpenTime = DateTime.MinValue;
            CloseTime = DateTime.MinValue;
            Open = decimal.MinValue;
            Close = decimal.MinValue;
            High = decimal.MinValue;
            Low = decimal.MinValue;

            VolAmount = decimal.MinValue;
            VolMoney = decimal.MinValue;
        }
        public decimal VolAmount { get; private set; }
        //public decimal VolAmount
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return decimal.Zero;
        //        return Values.Sum(p => p.Amount) * 100m;
        //    }
        //}
        public decimal VolMoney { get; private set; }
        //public decimal VolMoney
        //{
        //    get
        //    {
        //        if (Values.Count <= 0) return decimal.Zero;
        //        return Values.Sum(p => p.Money);
        //    }
        //}

        public DateTime OpenTime { get; private set; }
        public DateTime CloseTime { get; private set; }
        public decimal Open { get; private set; }
        public decimal Close { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }

        private bool NeedSwitch { get; set; }
        public void TriggerSwitch()
        {
            if (NeedSwitch)
            {
                if (EvntSwitch != null) EvntSwitch(this);
                NeedSwitch = false;
            }
        }

        public override void Add(ItemInfoEntity val)
        {
            if (!IsReady) return;
            var time = val.Time;
            var value = val.Value;
            if (time >= From && time < To)
            {
                if (OpenTime == DateTime.MinValue) OpenTime = time;
                if (Open == decimal.MinValue) Open = value;
                CloseTime = time;
                Close = value;
                if (High == decimal.MinValue)
                {
                    High = value;
                }
                else
                {
                    if (value > High) High = value;
                }
                if (Low == decimal.MinValue)
                {
                    Low = value;
                }
                else
                {
                    if (value < Low) Low = value;
                }
                if (VolAmount == decimal.MinValue)
                {
                    VolAmount = val.Amount * 100m;
                }
                else
                {
                    VolAmount = VolAmount + (val.Amount * 100m);
                }
                if (VolMoney == decimal.MinValue)
                {
                    VolMoney = val.Money;
                }
                else
                {
                    VolMoney = VolMoney + val.Money;
                }

                Value = val;
                if (Lines.Count > 0) Lines.ForEach(line => line.TryAdd(value));
            }
            else if (time >= To)
            {
                //ToComplete();
                TriggerSwitch();

                OpenTime = time;
                Open = value;
                CloseTime = time;
                Close = value;
                High = value;
                Low = value;
                VolAmount = val.Amount * 100m;
                VolMoney = val.Money;

                if (Lines.Count > 0) Lines.ForEach(line => line.Add(value));
                ResetTime(time);

                //return Add(val);
            }

            NeedSwitch = true;
        }
        public decimal Average
        {
            get
            {
                var amnt = VolAmount;
                var mny = VolMoney;
                return (amnt == decimal.Zero) ? decimal.Zero : Math.Round((mny / amnt), 2);
            }
        }
    }
}
