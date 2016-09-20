using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using DataBase.common;
using DataBase.common.objects;
using DataBase.postgresql;
using Log.common.enums;
using SGCollectionAgent.Configuration;
using SGNativeEntities.Database;
using SGNativeEntities.Enums;
using SGNativeEntities.General;
using SGUtilities.Averager;
using SGUtilities.Cache;
using SGUtilities.MACD;
using SGUtilities.RSI;
using SGUtilities.TextFile;
using Task.common.enums;

namespace SGCollectionAgent.Tasks
{
    public class ImportTask : Task
    {
        public static string ID = "Import-Task";

        private List<string> tables = null;

        public ImportTask()
            : base(ID) { }

        protected override RESULT Initial(StringBuilder messager)
        {
            tables = new List<string>();
            return base.Initial(messager);
        }

        protected override RESULT Process(StringBuilder messager)
        {
            var dir = Config.Instance.INFO.ImportSetting.DirInfo;
            if (dir == null || !Directory.Exists(dir.FullName)) return RESULT.OK;
            var files = Directory.GetFiles(dir.FullName).Where(f => f.Trim().ToLower().EndsWith(".txt"));
            if (files == null || files.Count() <= 0) return RESULT.OK;

            //Files Loading...
            var del = true; var count = 0; var total = 0;
            var file = new FileInfo(files.First());
            logger.Write(TYPE.INFO, string.Format("1.importing file({0})...", file.Name));
            try
            {
                
                using (var templates = Config.Instance.INFO.ImportSetting.Templates)
                {
                    Template<TemplateUnit> template = null;
                    using (var reader = new StreamReader(File.OpenRead(file.FullName), EncodingType.GetType(file.FullName)))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if ((template = templates.MatchedTemplate) == null)
                            {
                                templates.Match(line);
                            }
                            else
                            {
                                if (template.Table == TABLES.NONE)
                                {
                                    logger.Write(TYPE.WARNING, string.Format("template({0}) is invalid.", template.Name));
                                    break;
                                }

                                switch (template.Table)
                                {
                                    case TABLES.STK_GENERAL_M:
                                        if (SaveEntity(ExportEntity<DBTStkGeneralEntity>(line, template))) count++;
                                        total++;
                                        break;
                                    case TABLES.STK_DAILY_TD:
                                        var code1 = DBTStkDailyEntity.FetchCode(file.Name);
                                        if (count <= 0) if (!ClearDaily(code1)) continue;
                                        if (SaveEntity(ExportEntity<DBTStkDailyEntity>(line, template), code1)) count++;
                                        total++;
                                        break;
                                    case TABLES.STK_MINUTE_TD:
                                        var code2 = DBTStkMinuteEntity.FetchCode(file.Name);
                                        if (count <= 0) if (!ClearMinute(code2)) continue;
                                        if (SaveEntity(ExportEntity<DBTStkMinuteEntity>(line, template), code2)) count++;
                                        total++;
                                        break;
                                    case TABLES.IDX_GENERAL_M:
                                        if (SaveEntity(ExportEntity<DBTIdxGeneralEntity>(line, template))) count++;
                                        total++;
                                        break;
                                    case TABLES.STK_FAVORITE_M:
                                        if (SaveEntity(ExportEntity<DBTStkFavoriteEntity>(line, template))) count++;
                                        total++;
                                        break;
                                }
                            }
                        }
                    }

                    if (template != null)
                    {
                        switch (template.Table)
                        {
                            case TABLES.STK_DAILY_TD:
                                if (count > 0) CalculateDaily(DBTStkDailyEntity.FetchCode(file.Name));
                                break;
                        }
                    }
                }
            }
            catch (IOException err)
            {
                logger.Write(TYPE.ERROR, string.Format("import file({0}) failed(IOException:{1}).", file.Name, err.Message));
                del = false;
            }
            catch (UnauthorizedAccessException err)
            {
                logger.Write(TYPE.ERROR, string.Format("import file({0}) failed(UnauthorizedAccessException:{1}).", file.Name, err.Message));
                del = false;
            }
            catch (Exception err)
            {
                logger.Write(err);
            }
            finally
            {
                logger.Write(TYPE.INFO, string.Format("2.imported data rows({0}/{1})", count, total));
                if (del) File.Delete(file.FullName);
            }

            return RESULT.OK;
        }


        private bool SaveEntity(DBTStkGeneralEntity entity)
        {
            if (entity == null) return false;
            if (entity.IsDataValid && CheckTable<DBTStkGeneralEntity>())
            {
                var stk_general = new DBTStkGeneralEntity(entity.Code, accessor);
                if (stk_general.Fresh() > 0 && entity.Equals(stk_general)) return false;
                stk_general.Copy(entity);
                if (stk_general.Save() > 0)
                {
                    //logger.Write(TYPE.INFO, string.Format("import data([CD:{0}][UPDT:{1}]) in (STK_GENERAL_TD)", stk_general.Code, stk_general.UpdateTime.ToString("yyyy-MM-dd")));
                    return true;
                }
            }
            return false;
        }
        private bool SaveEntity(DBTStkDailyEntity entity, string code)
        {
            if (entity == null) return false;

            entity.Code = code;
            if (entity.IsDataValid && CheckTable<DBTStkDailyEntity>())
            {
                var stk_daily = new DBTStkDailyEntity(entity.Code, entity.Date, accessor);
                if (stk_daily.Fresh() > 0 && entity.Equals(stk_daily)) return false;
                stk_daily.Copy(entity, "ID");
                stk_daily.GenerateID();
                if (stk_daily.Save() > 0)
                {
                    //logger.Write(TYPE.INFO, string.Format("import data([CD:{0}][TDDT:{1}]) in (STK_DAILY_TD)", stk_daily.Code, stk_daily.Date.ToString("yyyy-MM-dd")));
                    return true;
                }
            }
            return false;
        }
        private bool SaveEntity(DBTStkMinuteEntity entity, string code)
        {
            if (entity == null) return false;

            entity.Code = code;
            entity.CalculateAverage();
            if (entity.IsDataValid && CheckTable<DBTStkMinuteEntity>())
            {
                var stk_minute = new DBTStkMinuteEntity(entity.Code, entity.Time, accessor);
                if (stk_minute.Fresh() > 0 && entity.Equals(stk_minute)) return false;
                stk_minute.Copy(entity, "ID");
                stk_minute.GenerateID();
                if (stk_minute.Save() > 0)
                {
                    //logger.Write(TYPE.INFO, string.Format("import data([CD:{0}][TDDT:{1}]) in (STK_DAILY_TD)", stk_daily.Code, stk_daily.Date.ToString("yyyy-MM-dd")));
                    return true;
                }
            }
            return false;
        }

        private bool SaveEntity(DBTIdxGeneralEntity entity)
        {
            if (entity == null) return false;
            if (entity.IsDataValid && CheckTable<DBTIdxGeneralEntity>())
            {
                var idx_general = new DBTIdxGeneralEntity(entity.Code, accessor);
                if (idx_general.Fresh() > 0 && entity.Equals(idx_general)) return false;
                idx_general.Copy(entity);
                if (idx_general.Save() > 0)
                {
                    //logger.Write(TYPE.INFO, string.Format("import data([CD:{0}][NAME:{1}]) in (IDX_GENERAL_TD)", idx_general.Code, idx_general.Name));
                    return true;
                }
            }
            return false;
        }
        private bool SaveEntity(DBTStkFavoriteEntity entity)
        {
            if (entity == null) return false;
            if (entity.IsDataValid && CheckTable<DBTStkFavoriteEntity>())
            {
                var stk_favorite = new DBTStkFavoriteEntity(entity.Code, accessor);
                if (stk_favorite.Fresh() > 0 && entity.Equals(stk_favorite)) return false;
                stk_favorite.Copy(entity);
                if (stk_favorite.Save() > 0)
                {
                    //logger.Write(TYPE.INFO, string.Format("import data([CD:{0}][NAME:{1}]) in (STK_FAVORITE_TD)", stk_favorite.Code, stk_favorite.Name));
                    return true;
                }
            }
            return false;
        }

        private bool ClearDaily(string code)
        {
            if (StockInfoEntity.TellMarket(code) == MARKET.none) return false;
            if (CheckTable<DBTStkDailyEntity>() && Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey("DEL_DAILY_BY_CD"))
            {
                var scripts = Config.Instance.INFO.ScriptSetting.Scripts;
                var param = accessor.CreateParameter("CODE", code);
                var cmd = accessor.CreateCommand(scripts["DEL_DAILY_BY_CD"], new List<DbParameter>() { param });
                if (accessor.ExecuteSQLCommand(cmd) < 0)
                {
                    logger.Write(TYPE.ERROR, string.Format("delete daily data failed.({0})", code));
                    logger.Write(TYPE.ERROR, accessor.LastError);
                    return false;
                }
            }
            return true;
        }
        private bool ClearMinute(string code)
        {
            if (StockInfoEntity.TellMarket(code) == MARKET.none) return false;
            if (CheckTable<DBTStkMinuteEntity>() && Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey("DEL_MINUTE_BY_CD"))
            {
                var scripts = Config.Instance.INFO.ScriptSetting.Scripts;
                var param = accessor.CreateParameter("CODE", code);
                var cmd = accessor.CreateCommand(scripts["DEL_MINUTE_BY_CD"], new List<DbParameter>() { param });
                if (accessor.ExecuteSQLCommand(cmd) < 0)
                {
                    logger.Write(TYPE.ERROR, string.Format("delete minute data failed.({0})", code));
                    logger.Write(TYPE.ERROR, accessor.LastError);
                    return false;
                }
            }
            return true;
        }

        private void CalculateDaily(string code)
        {
            if (string.IsNullOrEmpty(code)) return;
            var scripts = Config.Instance.INFO.ScriptSetting.Scripts;
            var macdsetting = Config.Instance.INFO.MACDSetting;
            var rsisetting = Config.Instance.INFO.RSISetting;

            // Delete MACD, RSI values
            if ((CheckTable<DBStkMACDEntity>() && scripts.ContainsKey("DEL_MACD_BY_CD")) &&
                (CheckTable<DBStkRSIEntity>() && scripts.ContainsKey("DEL_RSI_BY_CD")))
            {
                var lprm_macd = accessor.CreateParameter("CODE", code);
                var lcmd_macd = accessor.CreateCommand(scripts["DEL_MACD_BY_CD"], new List<DbParameter>() { lprm_macd });
                var lprm_rsi = accessor.CreateParameter("CODE", code);
                var lcmd_rsi = accessor.CreateCommand(scripts["DEL_RSI_BY_CD"], new List<DbParameter>() { lprm_rsi });
                if (accessor.ExecuteSQLCommand(new List<DbCommand>() { lcmd_macd, lcmd_rsi }) < 0)
                {
                    logger.Write(TYPE.ERROR, string.Format("delete MACD and RSI data failed.({0})", code));
                    logger.Write(TYPE.ERROR, accessor.LastError);
                    return;
                }
            }
            else
            {
                return;
            }

            // Retrieve Stocks
            var page = new EntityPage<DBTStkDailyEntity>(
                new Clause("Code = {Code}").AddParam("Code", code),
                new Sort().Add("ID", Sort.Orientation.asc), 100, accessor);
            List<DBTStkDailyEntity> lst_daily = null;DateTime ldt_date = DateTime.Today;
            MACD macd = null; RSI rsi = null; AverageValue daily30 = null;
            #region Cache For MACD
            SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>> cache1 = null;
            SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>> cache2 = null;
            LinearList<KeyValuePair<DateTime, decimal>> list1 = null;
            LinearList<KeyValuePair<DateTime, decimal>> list2 = null;
            #endregion
            #region Cache For AVG
            SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>> cache3 = null;
            SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>> cache4 = null;
            LinearList<KeyValuePair<DateTime, decimal>> list3 = null;
            LinearList<KeyValuePair<DateTime, decimal>> list4 = null;
            #endregion
            Func<KeyValuePair<DateTime,decimal>, decimal> fValue = (pair)=>{return pair.Value;};
            Func<KeyValuePair<DateTime, decimal>, KeyValuePair<DateTime, decimal>, LinearList<KeyValuePair<DateTime,decimal>>, bool> fSwitch1 = (v1, v2, segment) => {
                switch (segment.Orietion)
                {
                    case 0:
                        return false;
                    case 1:
                        if (v2.Value >= v1.Value) return false;
                        break;
                    case -1:
                        if (v2.Value <= v1.Value) return false;
                        break;
                }
                return true;
            };
            Func<LinearList<KeyValuePair<DateTime, decimal>>, LinearList<KeyValuePair<DateTime, decimal>>, bool> fSwitch2 = (values, segment) =>
            {
                if (segment.Orietion == 0 || segment.Orietion == values.Orietion) return false;
                return values.Count >= 10;
            };
            int pageno = 0; var count = 0;
            while ((lst_daily = page.Retrieve(++pageno)).Count > 0)
            {
                for (int i = 0; i < lst_daily.Count; i++,count++)
                {
                    try
                    {
                        var daily = lst_daily[i];
                        ldt_date = daily.Date;
                        if (pageno == 1 && i == 0)
                        {
                            macd = new MACD(daily.Close);
                            rsi = new RSI(rsisetting.N1, rsisetting.N2, rsisetting.N3, daily.Close);
                            daily30 = new AverageValue(30, 4);
                            daily30.Add(daily.Close);
                            cache1 = new SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>>(
                                new LinearList<KeyValuePair<DateTime, decimal>>[] { new LinearList<KeyValuePair<DateTime, decimal>>(fValue), new LinearList<KeyValuePair<DateTime, decimal>>(fValue) },
                                fSwitch1, null);
                            cache2 = new SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>>(
                                new LinearList<KeyValuePair<DateTime, decimal>>[] { new LinearList<KeyValuePair<DateTime, decimal>>(fValue), new LinearList<KeyValuePair<DateTime, decimal>>(fValue) },
                                null, fSwitch2);

                            cache3 = new SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>>(
                                new LinearList<KeyValuePair<DateTime, decimal>>[] { new LinearList<KeyValuePair<DateTime, decimal>>(fValue), new LinearList<KeyValuePair<DateTime, decimal>>(fValue) },
                                fSwitch1, null);
                            cache4 = new SegCache<LinearList<KeyValuePair<DateTime, decimal>>, KeyValuePair<DateTime, decimal>>(
                                new LinearList<KeyValuePair<DateTime, decimal>>[] { new LinearList<KeyValuePair<DateTime, decimal>>(fValue), new LinearList<KeyValuePair<DateTime, decimal>>(fValue) },
                                null, fSwitch2);
                        }
                        else
                        {
                            macd.Add(daily.Close);
                            rsi.Add(daily.Close);
                            daily30.Add(daily.Close);
                            if (cache1.Add(new KeyValuePair<DateTime, decimal>(daily.Date, macd.DEA), out list1))
                            {
                                cache2.Add(list1, out list2);
                            }
                            if (cache3.Add(new KeyValuePair<DateTime, decimal>(daily.Date, daily30.Average), out list3))
                            {
                                cache4.Add(list3, out list4);
                            }
                        }

                        var lent_macd = new DBStkMACDEntity(daily.ID);
                        lent_macd.EMA12 = macd.EMA12;
                        lent_macd.EMA26 = macd.EMA26;
                        lent_macd.DIFF = macd.DIFF;
                        lent_macd.DEA = macd.DEA;
                        lent_macd.BAR = macd.BAR;

                        var lent_rsi = new DBStkRSIEntity(daily.ID);
                        lent_rsi.RSI1 = rsi.RSI1;
                        lent_rsi.RSI2 = rsi.RSI2;
                        lent_rsi.RSI3 = rsi.RSI3;
                        lent_rsi.RSI1MaxEma = rsi.RSI1MaxEma;
                        lent_rsi.RSI1ABSEma = rsi.RSI1ABSEma;
                        lent_rsi.RSI2MaxEma = rsi.RSI2MaxEma;
                        lent_rsi.RSI2ABSEma = rsi.RSI2ABSEma;
                        lent_rsi.RSI3MaxEma = rsi.RSI3MaxEma;
                        lent_rsi.RSI3ABSEma = rsi.RSI3ABSEma;

                        accessor.SaveEntity(lent_macd, lent_rsi);
                        accessor.Commit();
                    }
                    catch { }
                }
            }
            if (count > 120)
            {
                cache2.Add(cache1.Segment, out list2);
                cache4.Add(cache3.Segment, out list4);

                var lent_sum = new DBStkSummaryResultEntity();
                lent_sum.Code = code;
                lent_sum.Time = DateTime.Now;
                lent_sum.PAVG30_ORIENT = cache4.Segment.Orietion;
                lent_sum.PAVG30_DAYS = cache4.Segment.Count;
                lent_sum.PAVG30_VALUE = cache4.Segment.Slope;
                lent_sum.PAVG30_Date1 = cache4.Segment[0].Key;
                lent_sum.PAVG30_Date2 = cache4.Segment[cache4.Segment.Count - 1].Key;

                lent_sum.DEA_ORIENT = cache2.Segment.Orietion;
                lent_sum.DEA_DAYS = cache2.Segment.Count;
                lent_sum.DEA_VALUE = cache2.Segment.Slope;
                lent_sum.DEA_Date1 = cache2.Segment[0].Key;
                lent_sum.DEA_Date2 = cache2.Segment[cache2.Segment.Count - 1].Key;

                lent_sum.RSI_Date = ldt_date;
                lent_sum.RSI_VALUE1 = rsi.RSI1;
                lent_sum.RSI_VALUE2 = rsi.RSI2;
                lent_sum.RSI_VALUE3 = rsi.RSI3;

                if (CheckTable<DBStkSummaryResultEntity>())
                {
                    accessor.SetDBAccessor2(lent_sum);
                    lent_sum.GenerateID();
                    lent_sum.Save();
                }
            }
            if (page != null) page.Dispose();
            if (cache1 != null) cache1.Dispose();
            if (cache2 != null) cache2.Dispose();
            if (cache3 != null) cache3.Dispose();
            if (cache4 != null) cache4.Dispose();
            if (daily30 != null) daily30.Dispose();
        }

        private bool CheckTable<T>()where T : GENTableEntity, new()
        {
            var entity = GENTableEntity.Create<T>();
            if (tables.Contains(entity.TableName)) return true;

            var script = string.Empty;
            var scripts = Config.Instance.INFO.ScriptSetting.Scripts;
            if (entity is DBTStkGeneralEntity) script = scripts.ContainsKey(TABLES.STK_GENERAL_M.ToString()) ? scripts[TABLES.STK_GENERAL_M.ToString()] : null;
            if (entity is DBTStkDailyEntity) script = scripts.ContainsKey(TABLES.STK_DAILY_TD.ToString()) ? scripts[TABLES.STK_DAILY_TD.ToString()] : null;
            if (entity is DBTStkMinuteEntity) script = scripts.ContainsKey(TABLES.STK_MINUTE_TD.ToString()) ? scripts[TABLES.STK_MINUTE_TD.ToString()] : null;

            if (entity is DBTIdxGeneralEntity) script = scripts.ContainsKey(TABLES.IDX_GENERAL_M.ToString()) ? scripts[TABLES.IDX_GENERAL_M.ToString()] : null;
            if (entity is DBTStkFavoriteEntity) script = scripts.ContainsKey(TABLES.STK_FAVORITE_M.ToString()) ? scripts[TABLES.STK_FAVORITE_M.ToString()] : null;

            if (entity is DBStkMACDEntity) script = scripts.ContainsKey(TABLES.STK_MACD_TD.ToString()) ? scripts[TABLES.STK_MACD_TD.ToString()] : null;
            if (entity is DBStkRSIEntity) script = scripts.ContainsKey(TABLES.STK_RSI_TD.ToString()) ? scripts[TABLES.STK_RSI_TD.ToString()] : null;

            if (entity is DBStkSummaryResultEntity) script = scripts.ContainsKey(TABLES.STK_SUM_RESULT_TD.ToString()) ? scripts[TABLES.STK_SUM_RESULT_TD.ToString()] : null;

            var check = false;
            accessor.SetDBAccessor2(entity);
            if (check = entity.CreateTable(script)) tables.Add(entity.TableName);
            return check;
        }

        private T ExportEntity<T>(string line, Template<TemplateUnit> template) where T : TableEntity
        {
            if (string.IsNullOrWhiteSpace(line) || template == null) return default(T);
            var separator = Convert.ToChar(9);

            var values = line.Trim().Split(new char[] { separator });
            if (values == null || values.Length <= 0) return default(T);

            object data = null;
            TableEntity entity = null;
            if (typeof(T) == typeof(DBTStkGeneralEntity)) entity = new DBTStkGeneralEntity();
            if (typeof(T) == typeof(DBTStkDailyEntity)) entity = new DBTStkDailyEntity();
            if (typeof(T) == typeof(DBTStkMinuteEntity)) entity = new DBTStkMinuteEntity();
            if (typeof(T) == typeof(DBTIdxGeneralEntity)) entity = new DBTIdxGeneralEntity();
            if (typeof(T) == typeof(DBTStkFavoriteEntity)) entity = new DBTStkFavoriteEntity();


            if (entity != null)
            {
                try
                {
                    var temp = new Dictionary<string, string>();
                    template.FindAll(t => t.ColumnIndex >= 0).ForEach(itm =>
                    {
                        var value = (itm.ColumnIndex < values.Length) ? values[itm.ColumnIndex] : string.Empty;
                        if (temp.ContainsKey(itm.DBColumn))
                        {
                            temp[itm.DBColumn] = (string.IsNullOrEmpty(temp[itm.DBColumn])) ? value : temp[itm.DBColumn] + value;
                        }
                        else
                        {
                            temp.Add(itm.DBColumn, value);
                        }
                    });

                    foreach (string col in temp.Keys)
                    {
                        var column = entity.GetColumn(col);
                        if (column == null) continue;
                        column.Value = TemplateUnit.ConvertVal(column.DataType, temp[col]);
                        entity.SetColumn(column);
                    }

                }
                catch (Exception err)
                {
                    logger.Write(TYPE.ERROR, string.Format("Template:{0} Line:{1}", template.Table.ToString(), line));
                    logger.Write(err);
                }

                data = entity;
            }

            return (data == null) ? default(T) : (T)data;
        }

    }
}
