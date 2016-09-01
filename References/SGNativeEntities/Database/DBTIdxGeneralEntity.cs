using DataBase.common.attributes;
using DataBase.common.enums;
using DataBase.postgresql;
using SGNativeEntities.Enums;

namespace SGNativeEntities.Database
{
    public class DBTIdxGeneralEntity : GENTableEntity
    {
        [DBField("Code", typeof(string), KeyType.Primary, false)]
        public string Code {
            get { return this.GetValue<string>("Code"); }
            set { if (IsValidCode(value, CODETYPE.index)) this.SetValue("Code", value); }
        }
        [DBField("Name", typeof(string), KeyType.Normal, false)]
        public string Name {
            get { return this.GetValue<string>("Name"); }
            set { this.SetValue("Name", value); }
        }

        public bool IsDataValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
            }
        }

        public DBTIdxGeneralEntity() : base(TABLES.IDX_GENERAL_M.ToString(), null) { }
        public DBTIdxGeneralEntity(DatabaseAccessor accessor) : base(TABLES.IDX_GENERAL_M.ToString(), accessor) { }
        public DBTIdxGeneralEntity(string code, DatabaseAccessor accessor) : base(TABLES.IDX_GENERAL_M.ToString(), accessor) 
        {
            Code = code;
        }

        public override bool Equals(object obj)
        {
            var entity = obj as DBTIdxGeneralEntity;
            if (entity == null) return false;
            return Code.Equals(entity.Code) && Name.Equals(entity.Name);
        }
    }
}
