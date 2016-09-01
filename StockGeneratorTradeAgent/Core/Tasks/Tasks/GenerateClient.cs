using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task.common.enums;
using SGNativeEntities.Database;
using Task;
using Task.common.utilities;
using StockGeneratorTradeAgent.Core.Configuration;
using Log.common.enums;
using System.Threading;

namespace StockGeneratorTradeAgent.Core.Tasks.Tasks
{
    public class GenerateClient : Task
    {
        public static string ID = "Generate-Client";

        public GenerateClient(Action<string, RESULT> process) : base(GenerateClient.ID, process) {
            workers = new List<Worker>();
            AddPuts = new Dictionary<string, Action<string, DataPipe>>();
            AddGets = new Dictionary<string, Action<string, DataPipe>>();
        }
        private List<Worker> workers { get; set; }

        private Dictionary<string, Action<string, DataPipe>> AddPuts { get; set; }
        private Dictionary<string, Action<string, DataPipe>> AddGets { get; set; }

        public void AddPutPlugAction(string id, Action<string, DataPipe> action)
        {
            if (string.IsNullOrWhiteSpace(id) || action == null) return;
            if (AddPuts.ContainsKey(id)) return;
            AddPuts.Add(id, action);
        }
        public void AddGetPlugAction(string id, Action<string, DataPipe> action)
        {
            if (string.IsNullOrWhiteSpace(id) || action == null) return;
            if (AddGets.ContainsKey(id)) return;
            AddGets.Add(id, action);
        }

        protected override RESULT Initial(StringBuilder messager)
        {
            workers.ForEach(worker => worker.Start());
            return base.Initial(messager);
        }

        protected override RESULT Process(StringBuilder messager)
        {
            var working = Config.Instance.INFO.BizTime.IsWorkingTime;

            if (working > 0)
            {
                if (CheckTable<DBTStkFavoriteEntity>())
                {
                    accessor.Retrieve<DBTStkFavoriteEntity>().FindAll(data => !workers.Any(worker => worker.id.Equals(data.Code))).ForEach(entity =>
                    {
                        var StockTask = new StockClient(entity.Code, null);

                        var pipe1 = new DataPipe();
                        var pipe2 = new DataPipe();
                        AddPuts[SocketClient.ID](StockTask.id, pipe1);
                        StockTask.AddGetPlug(SocketClient.ID, pipe1);
                        AddGets[SocketClient.ID](StockTask.id, pipe2);
                        StockTask.AddPutPlug(SocketClient.ID, pipe2);

                        var pipe3 = new DataPipe();
                        var pipe4 = new DataPipe();
                        StockTask.AddPutPlug(TradeClient.ID, pipe3);
                        AddGets[TradeClient.ID](StockTask.id, pipe3);
                        StockTask.AddGetPlug(TradeClient.ID, pipe4);
                        AddPuts[TradeClient.ID](StockTask.id, pipe4);

                        var pipe5 = new DataPipe();
                        var pipe6 = new DataPipe();
                        AddPuts[CollectClient.ID](StockTask.id, pipe5);
                        StockTask.AddGetPlug(CollectClient.ID, pipe5);
                        AddGets[CollectClient.ID](StockTask.id, pipe6);
                        StockTask.AddPutPlug(CollectClient.ID, pipe6);

                        var worker = new Worker(StockTask.id, StockTask);
                        workers.Add(worker);
                        worker.Start();
                    });
                }
            }
            else
            {
                tables.Clear();
            }

            return RESULT.OK;
        }

        protected override void Stopped(RESULT status, string message)
        {
            workers.ForEach(worker => worker.Stop());
            logger.Write(TYPE.INFO, string.Format("Stopping({0})...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            while (workers.Any(worker => worker.status == WorkerStatus.running)) Thread.Sleep(1);
            logger.Write(TYPE.INFO, string.Format("Stopped({0})...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            base.Stopped(status, message);
        }

    }
}
