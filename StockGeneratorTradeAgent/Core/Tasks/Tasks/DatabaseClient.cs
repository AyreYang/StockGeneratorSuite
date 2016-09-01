using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task.common.enums;
using SGNativeEntities.General;
using DataBase.postgresql;
using StockGeneratorTradeAgent.Core.Configuration;
using StockGeneratorTradeAgent.Core.Utilities;

namespace StockGeneratorTradeAgent.Core.Tasks.Tasks
{
    public class DatabaseClient:Task
    {
        public static string ID = "Database-Client";

        private DatabaseAccessor accessor = null;

        public DatabaseClient(Action<string, RESULT> process) : base(DatabaseClient.ID, process) { }

        public override RESULT Process(StringBuilder messager)
        {
            //ProcessCommand(this.GetData<SGCommand>(SocketClient.ID));

            //var list = this.GetData<List<StockInfoEntity>>(new List<string>() { SocketClient.ID, TradeClient.ID });
            //if()
            //this.logger.Write(Log.common.enums.TYPE.INFO, "running.");
            return RESULT.OK;
        }
    }
}
