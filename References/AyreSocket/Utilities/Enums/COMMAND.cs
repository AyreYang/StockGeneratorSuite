using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AyreSocket.Utilities.Enums
{
    public enum COMMAND
    {
        NONE = 0,
        LOGIN = 1,
        LOGOUT = 2,
        SHOW_USERS = 3,
        MSG = 4,
        KEY = 5,

        SG = 99,
        UNKNOWN = 100,
    }
}
