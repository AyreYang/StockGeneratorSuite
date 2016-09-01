using System;
using System.Collections.Generic;
using System.Text;
using DataBase.postgresql;
using Log.common.enums;
using Log.textfile;
using SGNativeEntities.Database;
using StockGeneratorTradeAgent.Core.Configuration;
using StockGeneratorTradeAgent.Core.Utilities;
using Task;
using Task.common.enums;

namespace StockGeneratorTradeAgent.Core.Tasks
{
    public abstract class Task:TaskCore
    {
        public string id { get; private set; }
        protected Logger logger { get; set; }
        protected List<string> tables = null;

        protected DatabaseAccessor accessor = null;
        protected Action<string, RESULT> FProgress { get; private set; }

        protected Task(string id, Action<string, RESULT> progress)
        {
            this.id = id.Trim().ToUpper();
            logger = new Logger(this.id, string.Empty);
            tables = new List<string>();
            FProgress = progress;
            var settings = Config.Instance.INFO.DBSetting;
            accessor = new DatabaseAccessor(settings.Host, settings.DBName, settings.User, settings.Password);
        }

        protected override RESULT Initial(StringBuilder messager)
        {
            logger.Write(TYPE.INFO, "initializing is ok.");
            return RESULT.OK;
        }

        protected override void Stop()
        {
        }

        protected override void Completed(RESULT status, string message)
        {
            if ((int)status < (int)RESULT.NONE) this.logger.Write(TYPE.ERROR, message);
        }

        protected override void Stopped(RESULT status, string message)
        {
            if (accessor != null) accessor.Dispose();

            if (FProgress != null) FProgress(this.id, status);

            this.logger.Write(TYPE.INFO, string.Format(Messager.MSG_TASK_STOP, this.id));
            if (status != RESULT.OK)
            {
                this.logger.Write(TYPE.INFO, string.Format("(RESULT={0}):{1}", status.ToString(), message));
            }
        }
        protected override void Started(RESULT status, string message)
        {
            if (FProgress != null) FProgress(this.id, status);

            this.logger.Write(TYPE.INFO, string.Format(Messager.MSG_TASK_START, this.id));
        }

        protected bool CheckTable<T>() where T : GENTableEntity, new()
        {
            var entity = GENTableEntity.Create<T>();
            if (tables.Contains(entity.TableName)) return true;

            var check = false;
            if (check = accessor.TableExists(entity.TableName))
            {
                tables.Add(entity.TableName);
            }
            return check;
        }
    }
}
