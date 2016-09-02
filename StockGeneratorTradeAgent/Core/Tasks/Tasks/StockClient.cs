using System;
using System.Collections.Generic;
using System.Text;
using DataBase.common.enums;
using DataBase.postgresql;
using Log.common.enums;
using SGDataService.Entities;
using SGNativeEntities.Database;
using SGUtilities.Averager;
using SGUtilities.Balancer;
using SGUtilities.Lines;
using StockGeneratorTradeAgent.Core.Utilities;
using Task.common.enums;
using DataBase.common.objects;

namespace StockGeneratorTradeAgent.Core.Tasks.Tasks
{
    public class StockClient:Task
    {
        private LinesController LinesController { get; set; }

        private AverageBalancer Balancer { get; set; }

        public StockClient(string id, Action<string, RESULT> process) : base(id.Trim().ToUpper(), process) {
        }

        protected override RESULT Initial(StringBuilder messager)
        {
            Balancer = new AverageBalancer();
            LinesController = new LinesController(60);
            InitialTimeLine();
            return base.Initial(messager);
        }

        protected override RESULT Process(StringBuilder messager)
        {
            var data = this.GetData<TengxunStockInfoEntity>(CollectClient.ID);

            if (data != null)
            {
                Balancer.Add(new KeyValuePair<DateTime, decimal>(data.Time, data.Current)).ForEach(pair =>LinesController.Add(pair));
                LinesController.Process();
            }

            ProcessCommand(this.GetData<SGCommand>(SocketClient.ID));

            //if (data != null) this.logger.Write(TYPE.INFO, data.ToString());
            return RESULT.OK;
        }

        protected override void Stopped(RESULT status, string message)
        {
            LinesController.ClearLines();
            Balancer.Reset();
            base.Stopped(status, message);
        }

        private void ProcessCommand(SGCommand command)
        {
            if (command == null) return;

            switch (command.Command)
            {
                case SGCMD.RTM:
                    //var data = balancer.StockInfo;
                    //if (data != null)
                    //{
                    //    command.Message = data.ToString();
                    //    this.PutData<SGCommand>(SocketClient.ID, command);
                    //}
                    break;
                default:
                    break;
            }
        }
        
        private void InitialTimeLine()
        {
            var HourLine = new HourLine<KeyValuePair<DateTime, decimal>>((pair) => { return pair.Key; }, (pair) => { return pair.Value; }, null);
            HourLine.AddLine(new AverageLine(LINE.HOUR5, (int)AverageType.AT5, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            HourLine.AddLine(new AverageLine(LINE.HOUR10, (int)AverageType.AT10, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            HourLine.AddLine(new AverageLine(LINE.HOUR30, (int)AverageType.AT30, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            var DailyLine = new DailyLine<KeyValuePair<DateTime, decimal>>((pair) => { return pair.Key; }, (pair) => { return pair.Value; }, null);
            DailyLine.AddLine(new AverageLine(LINE.DAILY5, (int)AverageType.AT5, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            DailyLine.AddLine(new AverageLine(LINE.DAILY10, (int)AverageType.AT10, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            DailyLine.AddLine(new AverageLine(LINE.DAILY30, (int)AverageType.AT30, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            var WeeklyLine = new WeeklyLine<KeyValuePair<DateTime, decimal>>((pair) => { return pair.Key; }, (pair) => { return pair.Value; }, null);
            WeeklyLine.AddLine(new AverageLine(LINE.WEEKLY5, (int)AverageType.AT5, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            WeeklyLine.AddLine(new AverageLine(LINE.WEEKLY10, (int)AverageType.AT10, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));
            WeeklyLine.AddLine(new AverageLine(LINE.WEEKLY30, (int)AverageType.AT30, 2).BindEvent("BreakIn", BreakIn).BindEvent("BreakBack", BreakBack).BindEvent("BreakOut", BreakOut).BindEvent("Touched", Touched));

            var date = (GENTableEntity.Count<DBTStkDailyEntity>(accessor) > 0) ? GENTableEntity.MaxValue<DBTStkDailyEntity, DateTime?>(accessor, "Date") : null;
            var time = (GENTableEntity.Count<DBTStkMinuteEntity>(accessor) > 0) ? GENTableEntity.MaxValue<DBTStkMinuteEntity, DateTime?>(accessor, "Time") : null;

            // Initialize DailyLine and WeeklyLine
            if (date.HasValue)
            {
                using (var page = new EntityPage<DBTStkDailyEntity>(
                    new Clause("Code = {Code} AND Date >= {Date}").AddParam("Code", this.id).AddParam("Date", date.Value.Date.AddMonths(-8)),
                    new Sort().Add("Date", Sort.Orientation.asc), 100,
                    accessor
                    ))
                {
                    int pageno = 0;
                    List<DBTStkDailyEntity> lst_data = null;
                    while ((lst_data = page.Retrieve(++pageno)).Count > 0)
                    {
                        lst_data.ForEach(data =>
                        {
                            if (!DailyLine.IsReady) DailyLine.Initialize(data.Date);
                            if (!WeeklyLine.IsReady) WeeklyLine.Initialize(data.Date);

                            DailyLine.Add(new KeyValuePair<DateTime, decimal>(data.Date, data.Close));
                            WeeklyLine.Add(new KeyValuePair<DateTime, decimal>(data.Date, data.Close));
                        });
                    }
                }
            }
            if (!DailyLine.IsReady) DailyLine.Initialize(DateTime.Today);
            if (!WeeklyLine.IsReady) WeeklyLine.Initialize(DateTime.Today);

            // Initialize HourLine
            if (time.HasValue)
            {
                using (var page = new EntityPage<DBTStkMinuteEntity>(
                    new Clause("Code = {Code} AND Time >= {Time}").AddParam("Code", this.id).AddParam("Time", time.Value.Date.AddDays(-20)),
                    new Sort().Add("Time", Sort.Orientation.asc), 1000,
                    accessor
                    ))
                {
                    int pageno = 0;
                    List<DBTStkMinuteEntity> lst_data = null;
                    while ((lst_data = page.Retrieve(++pageno)).Count > 0)
                    {
                        lst_data.ForEach(data =>
                        {
                            if (!HourLine.IsReady) HourLine.Initialize(data.Time);

                            HourLine.Add(new KeyValuePair<DateTime, decimal>(data.Time, data.Close));
                        });
                    }
                }
            }
            if (!HourLine.IsReady) HourLine.Initialize(DateTime.Now);

            LinesController.AddTimeLine(LineType.HOUR, HourLine);
            LinesController.AddTimeLine(LineType.DAILY, DailyLine);
            LinesController.AddTimeLine(LineType.WEEKLY, WeeklyLine);

            logger.Write(TYPE.INFO, string.Format("HL5:{0}, HL10:{1}, HL30:{2}", HourLine.Lines.Find(l => l.ID == LINE.HOUR5).Value, HourLine.Lines.Find(l => l.ID == LINE.HOUR10).Value, HourLine.Lines.Find(l => l.ID == LINE.HOUR30).Value));
            logger.Write(TYPE.INFO, string.Format("DL5:{0}, DL10:{1}, DL30:{2}", DailyLine.Lines.Find(l => l.ID == LINE.DAILY5).Value, DailyLine.Lines.Find(l => l.ID == LINE.DAILY10).Value, DailyLine.Lines.Find(l => l.ID == LINE.DAILY30).Value));
            logger.Write(TYPE.INFO, string.Format("WL5:{0}, WL10:{1}, WL30:{2}", WeeklyLine.Lines.Find(l => l.ID == LINE.WEEKLY5).Value, WeeklyLine.Lines.Find(l => l.ID == LINE.WEEKLY10).Value, WeeklyLine.Lines.Find(l => l.ID == LINE.WEEKLY30).Value));

            return;
        }

        private void BreakIn(EventArg evnt)
        {
            this.logger.Write(TYPE.INFO, string.Format("BreakIn:({0})", evnt.ToString()));
        }
        private void BreakBack(EventArg evnt)
        {
            this.logger.Write(TYPE.INFO, string.Format("BreakBack:({0})", evnt.ToString()));
        }
        private void BreakOut(EventArg evnt)
        {
            this.logger.Write(TYPE.INFO, string.Format("BreakOut:({0})", evnt.ToString()));
            LinesController.Reset();
        }
        private void Touched(EventArg evnt)
        {
            this.logger.Write(TYPE.INFO, string.Format("Touched:({0})", evnt.ToString()));
        }
        
    }
}
