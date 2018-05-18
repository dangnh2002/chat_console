using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_chat_console
{
    class Session
    {
        public static string channel_name { get; set; }
        public static string pass { get; set; }
        public static string username { get; set; }
        public static bool firstload = true;
        public static string user { get; set; }
        public static int id_mess { get; set; }
        public static int id_mess_last { get; set; }
    }
}
