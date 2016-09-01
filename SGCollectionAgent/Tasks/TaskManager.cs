using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Task;
using Task.common.enums;
using Task.common.utilities;
using System;

namespace SGCollectionAgent.Tasks
{
    class TaskManager
    {
        private static volatile TaskManager m_Instance = null;
        private static readonly object m_synObject = new object();
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

        public bool IsReady { get; private set; }
        private List<Worker> AllWorkers
        {
            get
            {
                var list = new List<Worker>();
                list.AddRange(managers);
                return list;
            }
        }
        public int Count
        {
            get
            {
                return managers.Count;
            }
        }

        private List<Worker> managers { get; set; }

        private TaskManager()
        {
            managers = new List<Worker>();
            IsReady = false;
        }

        public void Initial()
        {
            IsReady = false;
            //DataPipe pipe = null;
            managers.Clear();

            //var CollectTask = new CollectTask();
            //managers.Add(new Worker(CollectTask.ID, CollectTask, 1000));
            //var DatabaseTask = new DatabaseTask();
            //managers.Add(new Worker(DatabaseTask.ID, DatabaseTask));
            //pipe = new DataPipe(1000);
            //CollectTask.AddPutPlug(DatabaseTask.ID, pipe);
            //DatabaseTask.AddGetPlug(CollectTask.ID, pipe);


            var RealtimeTask = new RealtimeTask();
            managers.Add(new Worker(RealtimeTask.ID, RealtimeTask, 1000));
            var ImportTask = new ImportTask();
            managers.Add(new Worker(ImportTask.ID, ImportTask));

            IsReady = true;
        }

        public void Start()
        {
            if (!IsReady) return;
            foreach (Worker manager in managers) manager.Start();
        }

        public void Stop()
        {
            if (!IsReady) return;
            if (!managers.Any(m => m.status == WorkerStatus.running)) return;

            Console.WriteLine(string.Format("Stopping({0})...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            foreach (Worker manager in managers) manager.Stop();
            while (managers.Any(m => m.status == WorkerStatus.running)) Thread.Sleep(1);
            Console.WriteLine(string.Format("Stopped({0})...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }

    }
}
