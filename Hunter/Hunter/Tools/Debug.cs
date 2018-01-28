using System;
using System.Collections.Generic;
using System.Text;

namespace Hunter.Tools
{
    public class Logger
    {
        public static void Error(string sMsg = "")
        {
            Console.WriteLine("Error: " + sMsg);
            Console.ReadKey();
        }
    }
}
