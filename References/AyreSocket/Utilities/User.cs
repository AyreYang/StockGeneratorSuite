using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AyreSocket.Utilities
{
    public class User
    {
        public string ID { get; private set; }
        public string NickName { get; private set; }

        public User(string id, string nickname)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new Exception(Messager.ERR_USR_ID);
            if (string.IsNullOrWhiteSpace(nickname)) throw new Exception(Messager.ERR_USR_NICK);

            ID = id.Trim();
            NickName = nickname.Trim();
        }
    }
}
