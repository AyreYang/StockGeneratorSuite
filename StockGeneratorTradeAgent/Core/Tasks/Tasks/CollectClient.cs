using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task.common.enums;
using SGDataService.Interfaces;
using SGUtilities.Balancer;
using SGDataService.DataServices.WebRequest;
using StockGeneratorTradeAgent.Core.Configuration;
using DataBase.postgresql;
using Log.common.enums;
using SGDataService.Entities;
using StockGeneratorTradeAgent.Core.Utilities;
using SGNativeEntities.Database;
using SGNativeEntities.General;

namespace StockGeneratorTradeAgent.Core.Tasks.Tasks
{
    public class CollectClient : Task
    {
        public static string ID = "Collect-Client";

        private IDataService service = null;

        public CollectClient(Action<string, RESULT> process) : base(CollectClient.ID, process) { }

        protected override RESULT Initial(StringBuilder messager)
        {
            service = new TengxunDataService();

            return base.Initial(messager);
        }

        protected override RESULT Process(StringBuilder messager)
        {
            var working = Config.Instance.INFO.BizTime.IsWorkingTime;
            //working = 0;
            if (working > 0)
            {
                // Retrieve Code Data From DB
                var stocks = new List<string>();
                if (CheckTable<DBTStkFavoriteEntity>()) accessor.Retrieve<DBTStkFavoriteEntity>().ForEach(data => stocks.Add(StockInfoEntity.FormatedCode(data.Code)));
                if (stocks.Count > 0)
                {
                    service.FetchData<TengxunStockInfoEntity>(stocks).ForEach(info =>
                    {
                        if(info.IsTodayData && info.IsValid) this.PutData<TengxunStockInfoEntity>(info.Code, info);
                    });
                }
            }
            else
            {
                Config.Instance.INFO.BizTime.Clear();
                tables.Clear();
            }

            return RESULT.OK;
        }
    }

}
