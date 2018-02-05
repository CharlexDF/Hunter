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
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace Hunter
{
    /* DateTime */
    public partial class Utility
    {
        public static int GetYear(int date)
        {
            Debug.Assert(date > 19700101 && date < 20990101);
            return date / 10000;
        }
        public static int GetMonth(int date)
        {
            int month = date / 100 % 100;
            Debug.Assert(month > 0 && month < 13);
            return date / 100 % 100;
        }
        public static int GetDay(int date)
        {
            int day = date % 100;
            Debug.Assert(day > 0 && day < 32);
            return date % 100;
        }
        public static int GetHour(int time)
        {
            int hour = time / 10000;
            if (hour < 0 || hour > 24)
            {
                Utility.Log("hour = " + hour);
                Debug.Assert(hour >= 0 && hour <= 24);
            }
            return hour;
        }
        public static int GetMinute(int time)
        {
            int minute = (time / 100) % 100;
            Debug.Assert(minute >= 0 && minute <= 60);
            return (time / 100) % 100;
        }
        public static int GetSecond(int time)
        {
            int second = time % 100;
            Debug.Assert(second >= 0 && second <= 60);
            return second;
        }

        /***************************************************************/

        public static void Test()
        {
            /*
            string str_num = "-0.7abc";
            //string fix_str = Regex.Replace(str_num, @"[^\d]*", "");
            string fix_str = Utility.fixNumString(str_num);
            Console.WriteLine(fix_str);
            //double num = 0;
            */
            
            DateTime now = DateTime.Now;
            DateTime a = new DateTime(DateTime.Now.Ticks);

            Console.WriteLine(a.ToString());//2017/5/10 19:02:59
            Console.WriteLine(a.ToString("yyyyMMdd"));//20170510
            Console.WriteLine(a.ToString("yyyy-MM-dd HH:mm:ss"));//2017/5/10 19:02:59
            Console.WriteLine(a.ToString("HHmmss"));//190259
            Console.WriteLine();


            Console.WriteLine(a.ToShortDateString());//2017/5/10
            Console.WriteLine(a.ToShortTimeString());//19:02
            Console.WriteLine();

            Console.WriteLine(a.ToLongDateString());//2017年5月10日
            Console.WriteLine(a.ToLongTimeString());//19:04:59
            Console.WriteLine();

            Console.WriteLine(a.ToFileTime());//131388878998131900
            Console.WriteLine(a.ToOADate());//42865.7951367245
            Console.WriteLine();

            Console.WriteLine(a.Ticks);//636300399536052667
            Console.WriteLine();

        }
    }

    /* keep */
    public partial class Utility
    {
        public static DateTime String2DateTime(string strDate, string format = "YYYY/MM/DD HH:MM:SS")
        {
            DateTime dt = new DateTime();
            if (format == "YYYY-MM-DD")
            {
            }
            else if (format == "YYYY/MM/DD HH:MM:SS")
            {
                string[] sParam = strDate.Split(' ');
                if (sParam.Length != 2)
                {
                    Utility.Assert("sParam.Length != 2");
                }

                string[] date = sParam[0].Split('/');
                if (date.Length != 3)
                {
                    Utility.Assert("date.Length != 3");
                }

                string[] time = sParam[1].Split(':');
                if (time.Length != 3)
                {
                    Utility.Assert("time.Length != 3");
                }

                dt = new DateTime(ToInt32(date[0]), ToInt32(date[1]), ToInt32(date[2]),
                    ToInt32(time[0]), ToInt32(time[1]), ToInt32(time[2]));
            }
            else
            {
            }

            return dt;
        }

        public static int DateTimeToInt(DateTime dt)
        {
            dt.ToBinary();

            return 0;
        }
        public static int IntToDateTime(DateTime dt)
        {
            return 0;
        }

        public static int ToDate(string date)
        {
            string[] sParam = date.Split('-');
            if (sParam.Length != 3)
            {
                return 0;
            }
            return Utility.ToInt32(sParam[0]) * 10000 + Utility.ToInt32(sParam[1]) * 100 + Utility.ToInt32(sParam[2]);
        }
        public static int ToDate(DateTime dt)
        {
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }

        public static int ToTime(string strTime, string format = "hh:mm:ss")
        {
            int time = 0;
            if (format == "hh:mm:ss")
            {
                string[] sParam = strTime.Split(':');
                if (sParam.Length != 3)
                {
                    Utility.Assert("sParam.Length != 3");
                }
                time = ToInt32(sParam[0]) * 10000 + ToInt32(sParam[1]) * 100 + ToInt32(sParam[2]);
            }
            return time;
        }

        public static string Date2String(int date, string format = "")
        {
            string strDate = "";
            if (format == "YYYY-MM-DD")
            {
                strDate = Utility.GetYear(date).ToString()
                    + "-" + Utility.GetMonth(date).ToString().PadLeft(2, '0')
                    + "-" + Utility.GetDay(date).ToString().PadLeft(2, '0');
            }
            else if (format == "YYYY-MM-DD HH-MM-SS")
            {
                strDate = Utility.GetYear(date).ToString()
                    + "-" + Utility.GetMonth(date).ToString().PadLeft(2, '0')
                    + "-" + Utility.GetDay(date).ToString().PadLeft(2, '0');
            }
            else
            {
                strDate = Utility.GetYear(date).ToString()
                    + Utility.GetMonth(date).ToString().PadLeft(2, '0')
                    + Utility.GetDay(date).ToString().PadLeft(2, '0');
            }
            return strDate;
        }


        public static string GetClock()
        {
            DateTime now = DateTime.Now;
            return now.Hour.ToString().PadLeft(2, '0')
                + "-" + now.Minute.ToString().PadLeft(2, '0')
                + "-" + now.Second.ToString().PadLeft(2, '0');
        }

        public static string GetWeekDay(int date)
        {
            DateTime dt = new DateTime(date / 10000, date / 100 % 100, date % 100);

            return dt.DayOfWeek.ToString();
        }

        public static int GetWeekIndex(int date)
        {
            DateTime dt = new DateTime(date / 10000, date / 100 % 100, date % 100);
            GregorianCalendar calendar = new GregorianCalendar();
            int weekOfYear = calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }

        public static int GetWeekStartDate(int date)
        {
            int StartDate = 0;
            DateTime dt = new DateTime(date / 10000, date / 100 % 100, date % 100);
            string dayString = dt.DayOfWeek.ToString();
            Console.WriteLine(dayString);
            switch (dayString)
            {
                case "Monday":
                    StartDate = Utility.AddDays(date, 0);
                    break;
                case "Tuesday":
                    StartDate = Utility.AddDays(date, -1);
                    break;
                case "Wednesday":
                    StartDate = Utility.AddDays(date, -2);
                    break;
                case "Thursday":
                    StartDate = Utility.AddDays(date, -3);
                    break;
                case "Friday":
                    StartDate = Utility.AddDays(date, -4);
                    break;
                case "Saturday":
                    StartDate = Utility.AddDays(date, -5);
                    break;
                case "Sunday":
                    StartDate = Utility.AddDays(date, -6);
                    break;
                default:
                    Assert("GetWeekStartDate!");
                    break;
            }
            return StartDate;
        }
        public static bool IsInSameWeek(int date1, int date2)
        {
            return GetWeekStartDate(date1) == GetWeekStartDate(date1);
        }

        public static int GetMonthDay(int date)
        {
            return date % 10000;
        }
        public static int GetYearMonth(int date)
        {
            return date / 100;
        }
        public static int AddYear(int date, int num)
        {
            DateTime dt = new DateTime(date / 10000, date / 100 % 100, date % 100);
            dt = dt.AddYears(num);
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }
        public static int AddDays(int date, int nday)
        {
            DateTime dt = new DateTime(date / 10000, date / 100 % 100, date % 100);
            dt = dt.AddDays(nday);
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }
        public static int AddYearMonth(int yearMonth, int num)
        {
            DateTime dt = new DateTime(yearMonth / 100, yearMonth % 100, 1);
            dt = dt.AddMonths(num);
            return dt.Year * 100 + dt.Month;
        }

        public static int GetNowDate()
        {
            DateTime date = DateTime.Now;
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }

        public static int GetYesterDay()
        {
            return AddDays(GetNowDate(), -1);
        }

        public static int GetLastWorkDate()
        {
            int nowDate = GetNowDate();
            int lastWorkDate = AddDays(nowDate, -1);
            while (!Holiday.IsWorkDay(lastWorkDate))
            {
                lastWorkDate = AddDays(lastWorkDate, -1);
            }
            return lastWorkDate;
        }

        public static int NextWorkDate(int iDate)
        {
            iDate = AddDays(iDate, 1);
            while (!Holiday.IsWorkDay(iDate))
            {
                iDate = AddDays(iDate, 1);
            }
            return iDate;
        }

    }
}
