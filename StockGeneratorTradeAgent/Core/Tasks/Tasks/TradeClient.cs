using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task.common.enums;

namespace StockGeneratorTradeAgent.Core.Tasks.Tasks
{
    public class TradeClient:Task
    {
        public static string ID = "Trade-Client";

        public TradeClient(Action<string, RESULT> process) : base(TradeClient.ID, process) { }

        protected override RESULT Initial(StringBuilder messager)
        {
            return base.Initial(messager);
        }

        protected override RESULT Process(StringBuilder messager)
        {
            //this.logger.Write(Log.common.enums.TYPE.INFO, "running.");
            return RESULT.OK;
        }
    }
}
