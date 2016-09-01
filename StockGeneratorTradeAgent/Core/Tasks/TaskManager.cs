using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task;
using Task.common.utilities;
using StockGeneratorTradeAgent.Core.Tasks.Tasks;
using Task.common.enums;

namespace StockGeneratorTradeAgent.Core.Tasks
{
    public class TaskManager
    {
        private static volatile TaskManager m_Instance = null;
        private static readonly object m_synObject = new object();
        private static readonly object m_synProgress = new object();
        public static TaskManager Instance
        {
            get
            {
                lock (m_synObject)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new TaskManager();
                    }
                }
                return m_Instance;
            }
        }

        private Action<string> FProgress;
        private Action<long> FComplete;

        public bool IsReady { get; private set; }
        private List<Worker> AllWorkers
        {
            get
            {
                var list = new List<Worker>();
                list.AddRange(managers);
                list.AddRange(workers);
                return list;
            }
        }
        public int Count
        {
            get
            {
                return managers.Count + workers.Count;
            }
        }

        private List<Worker> managers { get; set; }     // Socket, Trade, Database
        private List<Worker> workers { get; set; }

        private List<string> progress { get; set; }

        private TaskManager()
        {
            managers = new List<Worker>();
            workers = new List<Worker>();
            progress = new List<string>();
            IsReady = false;
        }

        private void Progress(string id, RESULT status)
        {
            if (FProgress == null) return;
            lock (m_synProgress)
            {
                progress.Add(id);
                FProgress(id);
                if (progress.Count >= Count)
                {
                    var count = progress.Count;
                    progress.Clear();
                    if (FComplete != null) FComplete(count);
                }
            }
        }

        public void Initial(Action<string> progress, Action<long> complete)
        {
            FProgress = progress;
            FComplete = complete;
            IsReady = false;
            DataPipe pipe1 = null;
            DataPipe pipe2 = null;
            managers.Clear();
            workers.Clear();

            // 1. Create Managers
            var SocketTask = new SocketClient(Progress);
            var CollectTask = new CollectClient(Progress);
            var TradeTask = new TradeClient(Progress);

            var GenerateTask = new GenerateClient(Progress);
            GenerateTask.AddGetPlugAction(SocketClient.ID, SocketTask.AddGetPlug);
            GenerateTask.AddPutPlugAction(SocketClient.ID, SocketTask.AddPutPlug);
            GenerateTask.AddGetPlugAction(CollectClient.ID, CollectTask.AddGetPlug);
            GenerateTask.AddPutPlugAction(CollectClient.ID, CollectTask.AddPutPlug);
            GenerateTask.AddGetPlugAction(TradeClient.ID, TradeTask.AddGetPlug);
            GenerateTask.AddPutPlugAction(TradeClient.ID, TradeTask.AddPutPlug);

            pipe1 = new DataPipe();
            pipe2 = new DataPipe();
            SocketTask.AddPutPlug(CollectClient.ID, pipe1);
            CollectTask.AddGetPlug(SocketClient.ID, pipe1);
            SocketTask.AddGetPlug(CollectClient.ID, pipe2);
            CollectTask.AddPutPlug(SocketClient.ID, pipe2);

            pipe1 = new DataPipe();
            pipe2 = new DataPipe();
            SocketTask.AddPutPlug(TradeClient.ID, pipe1);
            TradeTask.AddGetPlug(SocketClient.ID, pipe1);
            SocketTask.AddGetPlug(TradeClient.ID, pipe2);
            TradeTask.AddPutPlug(SocketClient.ID, pipe2);


            managers.Add(new Worker(GenerateClient.ID, GenerateTask));
            managers.Add(new Worker(SocketClient.ID, SocketTask));
            managers.Add(new Worker(CollectClient.ID, CollectTask));
            managers.Add(new Worker(TradeClient.ID, TradeTask));

            //// 2. Create Workers
            //if (stocks != null && stocks.Count > 0)
            //{
            //    foreach (string id in stocks)
            //    {
            //        if(string.IsNullOrWhiteSpace(id) || workers.Any(w=>w.id.Equals(id.Trim().ToUpper())))continue;
            //        var StockTask = new StockClient(id.Trim().ToUpper(), Progress);

            //        pipe1 = new DataPipe();
            //        pipe2 = new DataPipe();
            //        SocketTask.AddPutPlug(StockTask.id, pipe1);
            //        StockTask.AddGetPlug(SocketClient.ID, pipe1);
            //        SocketTask.AddGetPlug(StockTask.id, pipe2);
            //        StockTask.AddPutPlug(SocketClient.ID, pipe2);

            //        pipe1 = new DataPipe();
            //        pipe2 = new DataPipe();
            //        StockTask.AddPutPlug(TradeClient.ID, pipe1);
            //        TradeTask.AddGetPlug(StockTask.id, pipe1);
            //        StockTask.AddGetPlug(TradeClient.ID, pipe2);
            //        TradeTask.AddPutPlug(StockTask.id, pipe2);

            //        pipe1 = new DataPipe();
            //        pipe2 = new DataPipe();
            //        DatabaseTask.AddPutPlug(StockTask.id, pipe1);
            //        StockTask.AddGetPlug(DatabaseClient.ID, pipe1);
            //        DatabaseTask.AddGetPlug(StockTask.id, pipe2);
            //        StockTask.AddPutPlug(DatabaseClient.ID, pipe2);

            //        workers.Add(new Worker(StockTask.id, StockTask, 1000));
            //    }
            //}
            IsReady = true;
        }

        public void Start()
        {
            if (!IsReady) return;
            progress.Clear();
            var all = this.AllWorkers;
            foreach (Worker worker in all) worker.Start();
        }

        public void Stop()
        {
            if (!IsReady) return;
            progress.Clear();
            var all = this.AllWorkers;
            foreach (Worker worker in all) worker.Stop();
        }
    }
}
