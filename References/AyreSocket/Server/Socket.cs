using System.Collections.Generic;
using System.Linq;
using AyreSocket.Utilities;
using AyreSocket.Utilities.Enums;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace AyreSocket.Server
{
    public class Socket : WebSocketBehavior
    {
        private const string SG = "StockGenerator";
        private CommandManager CommandManager = new CommandManager();

        private Command MakeCommand(COMMAND command, Dictionary<string, object> parameters, string message, params string[] to)
        {
            return new Command(command.ToString(), message, to, parameters, new User(this.ID, this.Nickname));
        }

        protected override void OnOpen()
        {
            this.CommandManager.Initial(this.ID);
            this.SendCommand(MakeCommand(COMMAND.KEY, null, null, null));
        }

        protected override void OnClose(CloseEventArgs e)
        {
            //Sessions.Broadcast(String.Format("{0} got logged off...", _name));
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var command = this.CommandManager.Deserialize(e.Data);
            ProcessCommand(command);
        }

        private int Login(string nickname, string password, out string message)
        {
            this._login = false;
            message = null;
            if (string.IsNullOrWhiteSpace(nickname))
            {
                message = string.Format(Messager.MSG_LGIN_NM_INVALID, nickname ?? "NULL");
                return 0;
            }
            var nick = nickname.Trim();
            if (Sessions.Sessions.Any(s => s.Nickname.Equals(nick)))
            {
                message = string.Format(Messager.MSG_LGIN_EXISTED, nick);
                return 1;
            }
            this._nick = nick;
            this._login = true;
            message = string.Format(Messager.MSG_LGIN_OK, nick);
            return 2;
        }

        private void Logout(out string message)
        {
            message = string.Format(Messager.MSG_LGOUT_OK, Nickname);
            this._login = false;
            this._nick = string.Empty;
        }

        private void ProcessCommand(Command command)
        {
            if (command == null) return;

            var msg = string.Empty;
            switch (command.COMMAND)
            {
                case COMMAND.LOGIN:
                    var name = (command.Parameters.Any(p => p.Key.Equals("name"))) ? command.Parameters.First(p => p.Key.Equals("name")).Value.ToString() : string.Empty;
                    var pass = (command.Parameters.Any(p => p.Key.Equals("password"))) ? command.Parameters.First(p => p.Key.Equals("password")).Value.ToString() : string.Empty;
                    var status = Login(name, pass, out msg);
                    this.SendCommand(MakeCommand(COMMAND.LOGIN, new Dictionary<string, object> { { "status", status } }, msg, null));
                    break;
                case COMMAND.LOGOUT:
                    Logout(out msg);
                    this.SendCommand(MakeCommand(COMMAND.LOGOUT, null, msg, null));
                    break;
                case COMMAND.MSG:
                    if (command.To == null || command.To.Count <= 0)
                    {
                        this.BroadcastCommand(MakeCommand(COMMAND.MSG, command.Parameters, command.Message, null));
                    }
                    else
                    {
                        var id = string.Empty;
                        foreach (string to in command.To)
                        {
                            try
                            {
                                if ((id = FindSessionID(to)) != null)
                                    this.SendCommand(MakeCommand(COMMAND.MSG, command.Parameters, command.Message, null), id);
                            }
                            catch { }
                        }
                    }
                    break;
                case COMMAND.SHOW_USERS:
                    var users = new List<string>();
                    foreach (IWebSocketSession session in Sessions.Sessions) if(session.Login)users.Add(session.Nickname);
                    msg = string.Join(",", users.ToArray());
                    this.SendCommand(MakeCommand(COMMAND.SHOW_USERS, null, msg, null));
                    break;
                case COMMAND.SG:
                    var sg = FindSGSessionID();
                    if (!string.IsNullOrWhiteSpace(sg))
                    {
                        this.SendCommand(command, sg);
                    }
                    break;
            }
        }

        private string FindSessionID(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) return null;

            var session = Sessions.Sessions.FirstOrDefault(s => s.Login && s.Nickname.Equals(nick.Trim()));
            if (session == null) session = Sessions.Sessions.FirstOrDefault(s => s.Login && s.ID.Equals(nick.Trim()));
            return session != null ? session.ID : null;
        }

        private string FindSGSessionID()
        {
            var session = Sessions.Sessions.FirstOrDefault(s => s.Login && s.Nickname.Equals(SG));
            return session != null ? session.ID : null;
        }

        private void SendCommand(Command command, string id = null)
        {
            if (command == null) return;

            if(id == null){
                this.Send(this.CommandManager.Serialize(command));
            }else{
                Sessions.SendTo(this.CommandManager.Serialize(command, new AyreEnc(id)), id);
            }
        }
        private void BroadcastCommand(Command command)
        {
            if (command == null) return;
            foreach (IWebSocketSession session in Sessions.Sessions)
            {
                if (!session.Login || session.ID == this.ID) continue;
                if (SG.Equals(session.Nickname)) continue;
                SendCommand(command, session.ID);
            }

            //var data = command.COMMAND == COMMAND.KEY ? command.ToString() : this.CommandManager.Serialize(command);
            //Sessions.Broadcast(data);
        }
    }
}
