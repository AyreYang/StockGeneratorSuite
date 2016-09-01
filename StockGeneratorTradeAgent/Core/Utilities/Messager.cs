using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockGeneratorTradeAgent.Core.Utilities
{
    internal class Messager
    {
        public static string MSG_TASK_INIT_OK = "Initializing is ok.";
        public static string MSG_TASK_START = "Task({0}) started.";
        public static string MSG_TASK_STOP = "Task({0}) stopped.";

        public static string MSG_SOCKET_CONNECTING = "Socket is connecting...";
        public static string MSG_SOCKET_CONNECTED = "Socket connected.";
        public static string MSG_SOCKET_CLOSED = "Socket closed.";
        public static string MSG_SOCKET_LOGIN_OK = "Login(nick:{0}) ok.";
        public static string MSG_SOCKET_LOGINING = "Logining...";
        public static string MSG_SOCKET_LOGIN_NG = "Login(status:{0}) ng.";
        public static string MSG_SOCKET_BAD_COMMAND = "Bad command.";


        public static string ERR_NULL_OR_EMPTY = "({0}) is null or empty.";
    }
}
