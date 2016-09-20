using System;
using System.IO;
using System.Text;
using DataBase.postgresql;
using SGCollectionAgent.Configuration;
using SGCollectionAgent.Tasks;
using SGNativeEntities.Enums;
using SGUtilities.Cache;
using SGUtilities.Shapes;
using DataBase.common.objects;
using SGNativeEntities.Database;

namespace SGCollectionAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().FullName);
            if (!Prepare())
            {
                Console.WriteLine("\nPress Enter key to stop the server...");
                Console.ReadLine();
            }
            do
            {
                Console.Write("请输入参数类型(0:EXIT, 1:START, 2:STOP)");
                var info = Console.ReadKey();
                switch (info.KeyChar)
                {
                    case '0':
                        TaskManager.Instance.Stop();
                        return;
                    case '1':
                        TaskManager.Instance.Start();
                        break;
                    case '2':
                        TaskManager.Instance.Stop();
                        break;
                    default:
                        Console.WriteLine("bad input.");
                        break;
                }
            } while (true);
        }

        private static bool Prepare()
        {
            string CONFIG_JSON_FILE_NAME = @"config\configuration.json";
            Config.Instance.Load(new FileInfo(Path.Combine(Config.RootDir, CONFIG_JSON_FILE_NAME)));
            if (!Config.Instance.IsReady)
            {
                Console.WriteLine("Config file loading ng.");
                return false;
            }

            //Create Database Accessor
            var settings = Config.Instance.INFO.DBSetting;
            var accessor = new DatabaseAccessor(settings.Host, settings.DBName, settings.User, settings.Password);

            //Create Tables
            string message = string.Empty;
            if (!CreateTables(accessor, out message)) Console.WriteLine(message);

            TaskManager.Instance.Initial();
            if (!TaskManager.Instance.IsReady)
            {
                Console.WriteLine("Tasks creating ng.");
                return false;
            }
            return true;
        }

        private static bool CreateTables(DatabaseAccessor accessor, out string message)
        {
            var lsb_messager = new StringBuilder();
            int li_errcnt = 0;

            // Create Table:STK_FAVORITE_M
            try
            {
                if (!accessor.TableExists(TABLES.STK_FAVORITE_M.ToString()))
                {
                    var script = Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey(TABLES.STK_FAVORITE_M.ToString()) ? Config.Instance.INFO.ScriptSetting.Scripts[TABLES.STK_FAVORITE_M.ToString()] : null;
                    accessor.ExecuteSQLCommand(accessor.CreateCommand(script, null));
                }
            }
            catch (Exception err)
            {
                li_errcnt++;
                lsb_messager.AppendLine(string.Format("{0}.Table({1}) creating failed.", li_errcnt, TABLES.STK_FAVORITE_M.ToString()));
                lsb_messager.AppendLine(err.ToString());
            }

            // Create Table:IDX_GENERAL_M
            try
            {
                if (!accessor.TableExists(TABLES.IDX_GENERAL_M.ToString()))
                {
                    var script = Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey(TABLES.IDX_GENERAL_M.ToString()) ? Config.Instance.INFO.ScriptSetting.Scripts[TABLES.IDX_GENERAL_M.ToString()] : null;
                    accessor.ExecuteSQLCommand(accessor.CreateCommand(script, null));
                }
            }
            catch (Exception err)
            {
                li_errcnt++;
                lsb_messager.AppendLine(string.Format("{0}.Table({1}) creating failed.", li_errcnt, TABLES.IDX_GENERAL_M.ToString()));
                lsb_messager.AppendLine(err.ToString());
            }

            // Create Table:STK_GENERAL_M
            try
            {
                if (!accessor.TableExists(TABLES.STK_GENERAL_M.ToString()))
                {
                    var script = Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey(TABLES.STK_GENERAL_M.ToString()) ? Config.Instance.INFO.ScriptSetting.Scripts[TABLES.STK_GENERAL_M.ToString()] : null;
                    accessor.ExecuteSQLCommand(accessor.CreateCommand(script, null));
                }
            }
            catch (Exception err)
            {
                li_errcnt++;
                lsb_messager.AppendLine(string.Format("{0}.Table({1}) creating failed.", li_errcnt, TABLES.STK_GENERAL_M.ToString()));
                lsb_messager.AppendLine(err.ToString());
            }

            // Create Table:STK_DAILY_TD
            try
            {
                if (!accessor.TableExists(TABLES.STK_DAILY_TD.ToString()))
                {
                    var script = Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey(TABLES.STK_DAILY_TD.ToString()) ? Config.Instance.INFO.ScriptSetting.Scripts[TABLES.STK_DAILY_TD.ToString()] : null;
                    accessor.ExecuteSQLCommand(accessor.CreateCommand(script, null));
                }
            }
            catch (Exception err)
            {
                li_errcnt++;
                lsb_messager.AppendLine(string.Format("{0}.Table({1}) creating failed.", li_errcnt, TABLES.STK_DAILY_TD.ToString()));
                lsb_messager.AppendLine(err.ToString());
            }

            // Create Table:STK_MACD_TD
            try
            {
                if (!accessor.TableExists(TABLES.STK_MACD_TD.ToString()))
                {
                    var script = Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey(TABLES.STK_MACD_TD.ToString()) ? Config.Instance.INFO.ScriptSetting.Scripts[TABLES.STK_MACD_TD.ToString()] : null;
                    accessor.ExecuteSQLCommand(accessor.CreateCommand(script, null));
                }
            }
            catch (Exception err)
            {
                li_errcnt++;
                lsb_messager.AppendLine(string.Format("{0}.Table({1}) creating failed.", li_errcnt, TABLES.STK_MACD_TD.ToString()));
                lsb_messager.AppendLine(err.ToString());
            }

            // Create Table:STK_RSI_TD
            try
            {
                if (!accessor.TableExists(TABLES.STK_RSI_TD.ToString()))
                {
                    var script = Config.Instance.INFO.ScriptSetting.Scripts.ContainsKey(TABLES.STK_RSI_TD.ToString()) ? Config.Instance.INFO.ScriptSetting.Scripts[TABLES.STK_RSI_TD.ToString()] : null;
                    accessor.ExecuteSQLCommand(accessor.CreateCommand(script, null));
                }
            }
            catch (Exception err)
            {
                li_errcnt++;
                lsb_messager.AppendLine(string.Format("{0}.Table({1}) creating failed.", li_errcnt, TABLES.STK_RSI_TD.ToString()));
                lsb_messager.AppendLine(err.ToString());
            }

            message = lsb_messager.ToString();
            return li_errcnt <= 0;
        }

    }
}
