using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using AyreSocket.Utilities.Enums;

namespace AyreSocket.Utilities
{
    public class Command
    {
        #region Command Scripts
        public static string CMD_LOGIN = "$" + COMMAND.LOGIN.ToString() + "[name={0},password={1}]";
        public static string CMD_MSG = "$" + COMMAND.MSG.ToString() + "[{0}]:{1}";
        #endregion

        public string command { get; private set; }
        public COMMAND COMMAND
        {
            get
            {
                var cmd = COMMAND.UNKNOWN;
                if (string.IsNullOrWhiteSpace(command))
                {
                    cmd = COMMAND.NONE;
                }
                else
                {
                    if (!Enum.TryParse<COMMAND>(command.Trim().ToUpper(), out cmd))
                    {
                        cmd = COMMAND.UNKNOWN;
                    }
                }
                return cmd;
            }
        }

        public User Requestor { get; private set; }

        public Dictionary<string, object> Parameters { get; private set; }
        //public string From { get; set; }
        public List<string> To { get; private set; }
        public string Message { get; set; }

        public Command() : this(null, null, null) { }
        public Command(string command, string message) : this(command, message, null, null) { }
        public Command(string command, string message, string[] to) : this(command, message, to, null) { }
        public Command(string command, string message, string[] to, Dictionary<string, object> parameters) : this(command, message, to, parameters, null) { }
        public Command(string command, string message, string[] to, Dictionary<string, object> parameters, User requestor)
        {
            this.command = (string.IsNullOrWhiteSpace(command)) ? string.Empty : command.Trim();
            this.Message = message;
            this.Requestor = requestor;
            //From = string.Empty;
            To = new List<string>();
            if (to != null) foreach (string user in to)
                {
                    if (!string.IsNullOrWhiteSpace(user)) To.Add(user.Trim());
                }
            Parameters = parameters == null ? new Dictionary<string, object>() : parameters;
        }

        public static bool TryParse(string line, out Command command)
        {
            command = null;
            if (string.IsNullOrWhiteSpace(line)) return false;
            var data = line.Trim();
            var result = false;
            string cmd = string.Empty;
            string msg = string.Empty;
            string param = string.Empty;
            List<string> to = null;
            Dictionary<string, object> parameters = null;

            Match match = Regex.Match(line, @"^\$(?<cmd>[a-z_A-Z0-9]+)(\[(?<param>.*)?\])?(\:(?<msg>.*))?", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (match.Success)
            {
                cmd = match.Groups["cmd"].Value;
                msg = match.Groups["msg"].Value;
                param = match.Groups["param"].Value;
                AnalyzeParam(param, out to, out parameters);
            }
            result = match.Success;
            if (result) command = new Command(cmd, msg, to.ToArray(), parameters);
            return result;
        }
        public static Command CreateCommand(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            Command result = null;
            try
            {
                string temp = null;
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                var command = dict.ContainsKey("command") && dict["command"] != null ? dict["command"].ToString() : string.Empty;
                var message = dict.ContainsKey("Message") && dict["Message"] != null ? dict["Message"].ToString() : string.Empty;
                //var from = dict.ContainsKey("From") && dict["From"] != null ? dict["From"].ToString() : string.Empty;
                temp = dict.ContainsKey("Requestor") && dict["Requestor"] != null ? dict["Requestor"].ToString() : string.Empty;
                var requestor = string.IsNullOrWhiteSpace(temp) ? null : JsonConvert.DeserializeObject<User>(temp);
                temp = dict.ContainsKey("To") && dict["To"] != null ? dict["To"].ToString() : string.Empty;
                var to = string.IsNullOrWhiteSpace(temp) ? null : JsonConvert.DeserializeObject<List<string>>(temp);
                temp = dict.ContainsKey("Parameters") && dict["Parameters"] != null ? dict["Parameters"].ToString() : string.Empty;
                var parameters = string.IsNullOrWhiteSpace(temp) ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(temp);

                result = new Command(command, message, to != null ? to.ToArray() : null, parameters)
                {
                    Requestor = requestor
                };
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            return result;
        }

        private static bool AnalyzeParam(string param, out List<string> to, out Dictionary<string, object> parameters)
        {
            to = new List<string>();
            parameters = new Dictionary<string, object>();

            if (string.IsNullOrWhiteSpace(param)) return false;

            var at = '@';
            var separator = new char[] { ',' };

            var array = param.Trim().Split(separator);
            foreach (string item in array)
            {
                if (item.Contains(at))
                {
                    var ats = item.Trim().Split(new char[] { at });
                    foreach (string a in ats) if (!string.IsNullOrWhiteSpace(a)) to.Add(a.Trim());
                }
                else
                {
                    var match = Regex.Match(item, @"(?<key>[a-z_A-Z0-9]+)\=(?<value>.*)?", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (match.Success)
                    {
                        var key = match.Groups["key"].Value;
                        var value = match.Groups["value"].Value;
                        if (!string.IsNullOrWhiteSpace(key) && !parameters.ContainsKey(key)) parameters.Add(key, value);
                    }
                }
            }

            return true;
        }

        public override string ToString()
        {
            var data = string.Empty;
            try
            {
                data = JsonConvert.SerializeObject(this);
            }
            catch (Exception err)
            {
                throw err;
            }
            return data;
        }

    }
}
