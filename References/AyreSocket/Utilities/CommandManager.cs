using System;
using AyreSocket.Utilities.Enums;
using Newtonsoft.Json;

namespace AyreSocket.Utilities
{
    public class TransPackage
    {
        public string Data { get; set; }
        public long Ticks { get; set; }

        public TransPackage()
        {
            Data = string.Empty;
            Ticks = (DateTime.Now.Ticks % 100000) + 10000;
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

    public class CommandManager
    {
        private AyreEnc _enc = null;

        public void Initial(string key)
        {
            _enc = new AyreEnc(key);
        }


        public string Serialize(Command command, AyreEnc enc = null)
        {
            var lenc = (enc != null) ? enc : (_enc != null) ? _enc : null;
            if (lenc == null) return null;
            if(command == null)return null;

            var package = new TransPackage();
            package.Data = (command.COMMAND == COMMAND.KEY) ? command.ToString() : lenc.Encrypt(command.ToString(), package.Ticks.ToString());
            return package.ToString();
        }
        public Command Deserialize(string src)
        {
            if (string.IsNullOrWhiteSpace(src)) return null;

            Command command = null;
            var package = JsonConvert.DeserializeObject<TransPackage>(src);
            if (package != null)
            {
                var data = (_enc != null) ? _enc.Decrypt(package.Data, package.Ticks.ToString()) : package.Data;
                command = Command.CreateCommand(data);
            }
            return command;
        }
    }
}
