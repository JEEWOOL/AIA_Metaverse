using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class DBDebug
    {
        static bool isDebug { get; set; }

        static DBDebug()
        {
            isDebug = true;
        }

        public static void WriteLog(string message)
        {
            if (isDebug)
                Console.WriteLine(message);
        }
    }
}
