using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AyreSocket.Utilities
{
    internal class Messager
    {
        public const string MSG_LGIN_NM_INVALID = "Nickname({0}) is invalid.";
        public const string MSG_LGIN_EXISTED = "Nickname({0}) has been existed.";
        public const string MSG_LGIN_OK = "Login({0}) is ok.";
        public const string MSG_LGOUT_OK = "Logout({0}).";

        public const string ERR_USR_ID = "Invalid User ID.";
        public const string ERR_USR_NICK = "Invalid User NickName.";
        public const string ERR_ENC_KEY_INVALID = "EncryptionKey({0}) is invalid.";
    }
}
