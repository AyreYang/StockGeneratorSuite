using DataBase.postgresql;
using Log.common.enums;
using Log.textfile;
using Task;
using Task.common.enums;
using System.Text;
using SGCollectionAgent.Configuration;

namespace SGCollectionAgent.Tasks
{
    public abstract class Task : TaskCore
    {
        public string id { get; private set; }
        protected Logger logger { get; set; }
        protected DatabaseAccessor accessor = null;

        protected Task(string id)
        {
            this.id = id.Trim().ToUpper();
            logger = new Logger(this.id, string.Empty);
        }

        protected override RESULT Initial(StringBuilder messager)
        {
            var settings = Config.Instance.INFO.DBSetting;
            accessor = new DatabaseAccessor(settings.Host, settings.DBName, settings.User, settings.Password);

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

        protected override void Started(RESULT status, string message)
        {
            if (status == RESULT.OK)
            {
                this.logger.Write(TYPE.INFO, "started.");
            }
            else
            {
                if ((int)status < (int)RESULT.NONE)
                {
                    this.logger.Write(TYPE.ERROR, message);
                }
            }
        }

        protected override void Stopped(RESULT status, string message)
        {
            if ((int)status < (int)RESULT.NONE) this.logger.Write(TYPE.ERROR, message);

            if (accessor != null) accessor.Dispose();
            this.logger.Write(TYPE.INFO, "stopped.");
        }
    }
}
