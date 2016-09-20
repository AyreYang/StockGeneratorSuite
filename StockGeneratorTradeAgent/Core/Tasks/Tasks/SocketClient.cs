using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task.common.enums;
using WebSocketSharp;
using StockGeneratorTradeAgent.Core.Configuration;
using AyreSocket.Utilities;
using AyreSocket.Utilities.Enums;
using StockGeneratorTradeAgent.Core.Utilities;
using System.Collections.Concurrent;
using SGNativeEntities.General;
using Task.common.utilities;

namespace StockGeneratorTradeAgent.Core.Tasks.Tasks
{
    internal enum LOGINSTATUS
    {
        LOGOUT,
        LOGINING,
        LOGIN
    }
    internal enum CONNECTSTATUS
    {
        DISCONNECTED,
        CONNECTING,
        CONNECTED
    }
    public class SocketClient:Task
    {
        public static string ID = "Socket-Client";
        private WebSocket Socket;
        private CommandManager Commander;
        private ConcurrentQueue<SGCommand> CommandQueue;
        private List<SGCommand> ContinuousCommands;

        private string key;
        //private string nick;
        private User user;
        private LOGINSTATUS LoginStatus;
        private CONNECTSTATUS Connected;

        public SocketClient(Action<string, RESULT> process) : base(SocketClient.ID, process) {}

        #region Task Events
        protected override RESULT Initial(StringBuilder messager)
        {
            return RESULT.NG;

            //key = null;
            //user = null;
            //Connected = CONNECTSTATUS.DISCONNECTED;
            //LoginStatus = LOGINSTATUS.LOGOUT;
            //CommandQueue = new ConcurrentQueue<SGCommand>();
            //ContinuousCommands = new List<SGCommand>();
            //Commander = new CommandManager();

            ////Socket = new WebSocket(Config.Instance.INFO.WebSocket.URL);
            //Socket = new WebSocket("ws://127.0.0.1:4649/Socket");
            //Socket.OnOpen += OnOpen;
            //Socket.OnMessage += OnMessage;
            //Socket.OnClose += OnClose;
            //Socket.OnError += OnError;

            //Connect();

            //this.logger.Write(Log.common.enums.TYPE.INFO, Messager.MSG_TASK_INIT_OK);
            //return RESULT.OK;
        }
        protected override RESULT Process(StringBuilder messager)
        {
            if (Connected != CONNECTSTATUS.CONNECTED)
            {
                Connect();
            }
            else
            {
                switch (LoginStatus)
                {
                    case LOGINSTATUS.LOGOUT:
                        Login();
                        break;
                    case LOGINSTATUS.LOGINING:
                        break;
                    case LOGINSTATUS.LOGIN:
                        //1.Add/Remove Command
                        SGCommand command = null;
                        if (CommandQueue.TryDequeue(out command) && command != null)
                        {
                            if (!command.Continuous)
                            {
                                ProcessCommand(command);
                            }
                            else
                            {
                                if (command.Continuous && !ContinuousCommands.Any(cmd => cmd.Command == command.Command && cmd.RequestorID.Equals(command.RequestorID)))
                                {
                                    ContinuousCommands.Add(command);
                                }
                            }
                            
                        }
                        //2.Process Continuous Commands
                        foreach (SGCommand cmd in ContinuousCommands) ProcessCommand(cmd);

                        //3.Send Message to Server
                        var list = this.GetData<SGCommand>();
                        if (list.Count > 0)
                        {
                            foreach (SGCommand cmd in list)
                            {
                                SendCommand(MakeMSGCommand(cmd.RequestorID, cmd.Message));
                            }
                        }
                        break;
                }
            }
            
            return RESULT.OK;
        }
        protected override void Stop()
        {
            Socket.Close();
        }
        #endregion


        #region Socket Events
        private void OnOpen(object sender, EventArgs e)
        {
            this.logger.Write(Log.common.enums.TYPE.INFO, Messager.MSG_SOCKET_CONNECTED);
        }
        private void OnMessage(object sender, MessageEventArgs e)
        {
            var command = Commander.Deserialize(e.Data);
            if (command != null)
            {
                switch (command.COMMAND)
                {
                    case COMMAND.KEY:
                        key = command.Requestor.ID;
                        Commander.Initial(key);
                        this.logger.Write(Log.common.enums.TYPE.INFO, string.Format("key:{0}", key));
                        Login();
                        Connected = CONNECTSTATUS.CONNECTED;
                        break;
                    case COMMAND.LOGIN:
                        var status = Convert.ToInt32(command.Parameters["status"]);
                        if (status == 2)
                        {
                            user = command.Requestor;
                            LoginStatus = LOGINSTATUS.LOGIN;
                            this.logger.Write(Log.common.enums.TYPE.INFO, string.Format(Messager.MSG_SOCKET_LOGIN_OK, user.NickName));
                        }
                        else
                        {
                            LoginStatus = LOGINSTATUS.LOGOUT;
                            this.logger.Write(Log.common.enums.TYPE.INFO, string.Format(Messager.MSG_SOCKET_LOGIN_NG, status));
                        }
                        break;
                    case COMMAND.SG:
                        var sgcmd = SGCommand.ConvertFromCommand(command);
                        if (sgcmd != null)
                        {
                            CommandQueue.Enqueue(sgcmd);
                        }
                        else
                        {
                            SendCommand(MakeMSGCommand(command.Requestor.ID, Messager.MSG_SOCKET_BAD_COMMAND));
                        }
                        break;
                }
            }
        }
        private void OnClose(object sender, CloseEventArgs e)
        {
            Connected = CONNECTSTATUS.DISCONNECTED;
            this.logger.Write(Log.common.enums.TYPE.INFO, Messager.MSG_SOCKET_CLOSED);
        }
        private void OnError(object sender, ErrorEventArgs e)
        {
            this.logger.Write(Log.common.enums.TYPE.ERROR, string.Format("socket-error:{0}", e.Message));
        }
        #endregion

        #region Private Common Methods
        private Command MakeMSGCommand(string id, string msg)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var script = string.Format(Command.CMD_MSG, "@" + id, string.Empty);
            Command command = null;
            if (Command.TryParse(script, out command))
            {
                command.Message = msg;
            }
            return command;
        }
        private void SendCommand(Command command)
        {
            if (command == null) return;

            var data = Commander.Serialize(command);
            Socket.Send(data);
        }
        private void Login()
        {
            var script = string.Format(Command.CMD_LOGIN, Config.Instance.INFO.WebSocket.User, Config.Instance.INFO.WebSocket.Password);
            Command command = null;
            if (Command.TryParse(script, out command))
            {
                LoginStatus = LOGINSTATUS.LOGINING;
                this.logger.Write(Log.common.enums.TYPE.INFO, Messager.MSG_SOCKET_LOGINING);
                SendCommand(command);
            }
        }
        private void Connect()
        {
            if (Connected == CONNECTSTATUS.DISCONNECTED)
            {
                this.logger.Write(Log.common.enums.TYPE.INFO, Messager.MSG_SOCKET_CONNECTING);
                Connected = CONNECTSTATUS.CONNECTING;
                Socket.Connect();
            }
        }
        #endregion

        #region Process Commands
        private void ProcessCommand(SGCommand command)
        {
            if (command == null) return;

            switch (command.Command)
            {
                case SGCMD.RTM:
                    Proc_RTM(command);
                    //if(!Proc_RTM(command)){
                    //    SendCommand(MakeMSGCommand(command.RequestorID, "bad command."));
                    //}
                    break;
                case SGCMD.CANCEL:
                    Proc_CANCEL(command);
                    break;
            }
        }
        private void Proc_RTM(SGCommand command)
        {
            var code = (command.Parameters.ContainsKey("code")) ? command.Parameters["code"].ToString() : null;
            if (StockInfoEntity.IsValidCode(code))
            {
                var plug = this.FindPlug<PlugIn>(code);
                if (plug != null && plug.DataCount <= 0) this.PutData<SGCommand>(code, command);
            }
        }
        private void Proc_CANCEL(SGCommand command)
        {
            var lx_temp = (command.Parameters.ContainsKey("cmd")) ? command.Parameters["cmd"] : null;
            var ls_cmd = (lx_temp != null) ? lx_temp.ToString().Trim().ToUpper() : null;
            if (string.IsNullOrWhiteSpace(ls_cmd)) return;
            SGCMD lsg_cmd = SGCMD.NONE;
            if (!Enum.TryParse<SGCMD>(ls_cmd, out lsg_cmd) || lsg_cmd == SGCMD.NONE) return;

            var lsg_ccmd = ContinuousCommands.FirstOrDefault(cmd => cmd.Command == lsg_cmd && cmd.RequestorID.Equals(command.RequestorID));
            if (lsg_ccmd != null) ContinuousCommands.Remove(lsg_ccmd);
        }
        #endregion
    }
}
