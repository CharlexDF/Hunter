using System;

namespace Hunter
{
    class Program
    {
        public static void Test()
        {
            DateTime dt = DateTime.Now;
            DateTime clo = new DateTime(dt.Year, dt.Month, dt.Day, 9, 30, 15);
            Console.WriteLine("clo = " + clo.ToString());
            /*
            List<Name> lsName = new List<Name>();
            Name name;
            for (int i = 0; i < 10; i++)
            {
                Name a = new Name();
                a.id = i;
                lsName.Add(a);
                name = lsName[lsName.Count - 1];
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("i = " + i + " id = " + lsName[i].id);
            }
            */
        }

        static void StockTest(string[] args)
        {
            if (Config.bTrue)
            {
                Hunter.Start();
            }
            else
            {
                Test();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Program Start Run...");

            TexasHoldem.Test();

            Console.WriteLine("Program Finish Run...");
            Console.ReadKey();
        }
    }
}
