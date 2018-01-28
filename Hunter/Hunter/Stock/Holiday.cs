using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class HolidayInfo
    {
        public int date;
        public string dayOfWeek;
        public int isWorkDay;
        //public string dateString;
        public HolidayInfo()
        {
            date = 0;
            dayOfWeek = "";
            isWorkDay = 0;
        }
    }

    class Holiday
    {
        public static bool bInit = false;
        public static int StartYear = 2000;

        public static Dictionary<int, HolidayInfo> HolidayDic = new Dictionary<int, HolidayInfo>();
        public static Dictionary<int, int> DateDic = new Dictionary<int, int>();

        public static bool IsWorkDay(int iDate)
        {
            if (!bInit)
            {
                Init();
            }
            if (DateDic.ContainsKey(iDate))
            {
                return DateDic[iDate] == 1;
            }
            Utility.Log("Error Date = " + iDate);
            return false;
        }

        public static void Init()
        {
            bInit = true;
            ReadHolidayFile();
        }

        public static void ReadHolidayFile()
        {
            Utility.Log("ReadHolidayFile Start...");
            
            DateTime now = DateTime.Now;
            //Console.WriteLine("now.Year = " + now.Year);
            //Console.WriteLine("now.Month = " + now.Month);
            //Console.WriteLine("now.Day = " + now.Day);
            for (int year = 2000; year <= now.Year; year++)
            {
                ReadYearHolidayFile(year);
            }

            //Utility.Log("ReadHolidayFile Finish...");
        }
        public static void ReadYearHolidayFile(int year)
        {
            string relativePath = "/Content/Holiday/Holiday_" + year.ToString() + ".csv";
            string path = Environment.CurrentDirectory + relativePath;
            //Utility.Log("ReadContent " + path);

            if (!File.Exists(path))
            {
                Utility.Assert("File Not Exist! " + path);
                return;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string line = sr.ReadLine();
            int iDate = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length < 3)
                {
                    Utility.Assert("Read Error! " + path);
                    return;
                }

                iDate = Utility.ToInt32(sParam[0]);
                if (iDate == 0) continue;
                if (DateDic.ContainsKey(iDate))
                {
                    Utility.Assert("Repeat id = " + iDate);
                    return;
                }

                DateDic.Add(iDate, Utility.ToInt32(sParam[2]));
                /*
                HolidayInfo holidayInfo = new HolidayInfo();
                holidayInfo.date = iDate;
                holidayInfo.dayOfWeek = sParam[1];
                holidayInfo.isWorkDay = Utility.ToInt32(sParam[2]);

                HolidayDic.Add(iDate, holidayInfo);
                */
            }

            sr.Close();
            fs.Close();
        }


        public static void CreateHolidayFile()
        {
            DateTime now = DateTime.Now;
            
            Console.WriteLine("now.Year = " + now.Year);
            Console.WriteLine("now.Month = " + now.Month);
            Console.WriteLine("now.Day = " + now.Day);
            for (int year = 2000; year <= now.Year; year++)
            {
                CreateYearHolidayFile(year);
            }
        }

        public static void CreateYearHolidayFile(int year)
        {
            string relativePath = "/Content/Holiday_Default/Holiday_" + year.ToString() +".csv";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("WeiteContent " + path);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("DateInt,WeekDay,WorkDay");

            DateTime dt = new DateTime(year, 1, 1);
            int date = 0;
            int isWorkDay = 0;
            while (dt.Year == year)
            {
                date = dt.Year * 10000 + dt.Month * 100 + dt.Day;
                if (dt.DayOfWeek.ToString() == "Saturday"
                    || dt.DayOfWeek.ToString() == "Sunday")
                {
                    isWorkDay = 0;
                }
                else
                {
                    isWorkDay = 1;
                }
                sw.WriteLine(date.ToString()
                    + "," + dt.DayOfWeek.ToString()
                    + "," + isWorkDay.ToString()
                    );
                dt = dt.AddDays(1);
            }

            sw.Close();
            fs.Close();
        }
        public static void TestCountHoliday()
        {
            Holiday.CreateHolidayFile();
        }
    }
}
