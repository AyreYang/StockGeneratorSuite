using DataBase.common;
using DataBase.postgresql;

namespace SGNativeEntities.Database
{
    public abstract class SEQTableEntity : GENTableEntity
    {
        public abstract void GenerateID();
        private string SequenceName
        {
            get
            {
                return string.Format("{0}_SEQ", this.TableName.ToUpper());
            }
        }

        public long GenerateSequence()
        {
            return (accessor != null) ? accessor.GenerateSequence(this.SequenceName) : 0;
        }

        public SEQTableEntity(string table, DatabaseAccessor accessor) : base(table, accessor) { }
    }
}
