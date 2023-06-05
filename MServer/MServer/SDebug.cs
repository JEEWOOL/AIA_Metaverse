using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public static class SDebug
    {
        static bool isDebug { get; set; }

        static SDebug()
        {
            isDebug = true;
        }

        public static void WriteLog(string message)
        {
            if (isDebug)
            {
#if UNITY_STANDALONE_WIN
                Debug.Log(message);
#else
                Console.WriteLine(message);
#endif
            }
        }
    }
}
