using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AyreSocket.Utilities;

namespace StockGeneratorTradeAgent.Core.Utilities
{
    public enum SGCMD
    {
        NONE,
        RTM,
        CANCEL
    }
    public class SGCommand
    {
        public SGCMD Command { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }
        public string RequestorID { get; private set; }
        public string Message { get; set; }
        public bool Continuous { get; private set; }

        public SGCommand(SGCMD command, string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new Exception(string.Format(Messager.ERR_NULL_OR_EMPTY, "RequestorID"));
            Command = command;
            RequestorID = id.Trim();
            Parameters = new Dictionary<string, object>();
            SetContinuous();
        }

        private void SetContinuous()
        {
            switch (Command)
            {
                case SGCMD.RTM:
                    Continuous = true;
                    break;
                default:
                    Continuous = false;
                    break;
            }
        }

        public static SGCommand ConvertFromCommand(Command command)
        {
            if (command == null) return null;
            SGCommand sgcmd = null;
            if (!command.Parameters.ContainsKey("command")) return null;
            SGCMD cmd = SGCMD.NONE;
            if (!Enum.TryParse<SGCMD>(command.Parameters["command"].ToString().Trim().ToUpper(), out cmd)) return null;
            if (cmd == SGCMD.NONE) return null;

            sgcmd = new SGCommand(cmd, command.Requestor.ID);
            foreach (string key in command.Parameters.Keys)
            {
                if ((!string.IsNullOrWhiteSpace(key)) && (!"command".Equals(key.Trim()))) sgcmd.Parameters.Add(key.Trim(), command.Parameters[key]);
            }
            return sgcmd;
        }
    }
}
