using DataBase.common;
using DataBase.postgresql;
using SGNativeEntities.Enums;
using SGNativeEntities.General;

namespace SGNativeEntities.Database
{
    public abstract class GENTableEntity : TableEntity
    {
        public static bool IsValidCode(string code, CODETYPE type)
        {
            var valid = false;
            switch (type)
            {
                case CODETYPE.index:
                    valid = IndexInfoEntity.TellMarket(code) != MARKET.none;
                    break;
                case CODETYPE.stock:
                    valid = StockInfoEntity.TellMarket(code) != MARKET.none;
                    break;
            }
            return valid;
        }
        public static string FetchCode(string src)
        {
            if (string.IsNullOrWhiteSpace(src)) return string.Empty;
            var match = System.Text.RegularExpressions.Regex.Match(src, @"(?<code>\d{6})", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            return (match.Success) ? match.Groups["code"].Value : string.Empty; 
        }

        public GENTableEntity(string table, DatabaseAccessor accessor) : base(table, accessor) { }

        public override bool CreateTable(string script)
        {
            if(string.IsNullOrWhiteSpace(script))return false;
            return base.CreateTable(script.Replace("{table-name}", this.TableName));
        }
    }
}
