using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Util
{
    class LogFile
    {
        public delegate void WriteLineFunction(string str);
        public static List<WriteLineFunction> FuncList;
        static LogFile()
        {
            FuncList = new List<WriteLineFunction>();
        }

        public static void WriteLine(string str)
        {
            foreach (WriteLineFunction func in FuncList)
            {
                func(str);
            }
        }
    }
}
