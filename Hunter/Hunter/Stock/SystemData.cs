using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Xml;

namespace Hunter
{
    class SystemData
    {
        public static int DailyUpdateTime = 7;
        //public static int LastUpdateTime;
        public static long LUT_stocklist_csv;

        public static void Init()
        {
            ReadData();
        }

        public static void SaveData()
        {
            WriteData();
        }

        public static void ReadData()
        {
            string relativePath = "/Data/SystemData.ini";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("Read Data " + path);

            if (!File.Exists(path))
            {
                Utility.Assert("File Not Exist! " + path);
                return;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string line;
            string key;
            string value;
            List<string> ls_key = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split('=');
                if (sParam.Length < 2)
                {
                    Utility.Assert("Read Error! " + path);
                    return;
                }

                key = sParam[0];
                value = sParam[1];

                if (ls_key.Contains(key))
                {
                    Utility.Log("Read Error! key = " + key);
                    continue;
                }

                ls_key.Add(key);

                switch (key)
                {
                    case "LUT_stocklist_csv":
                        LUT_stocklist_csv = Utility.ToInt64(value);
                        break;
                    default:
                        Utility.Assert("Error key = " + key);
                        break;
                }
            }

            sr.Close();
            fs.Close();
        }

        public static void WriteData()
        {
            string relativePath = "/Data/SystemData.ini";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("Write Data " + path);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("LUT_stocklist_csv" + "=" + LUT_stocklist_csv);

            sw.Close();
            fs.Close();
        }
    }
}
