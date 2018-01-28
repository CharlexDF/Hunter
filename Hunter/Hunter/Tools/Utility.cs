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
    public delegate void Delegate_Void_Function();

    public delegate int Delegate_Int_Function_ArrayString(string[] strList);

    public delegate int Delegate_Int_Function();
    public delegate int Delegate_Int_Function_Stock(Stock stock);
    
    public delegate double Delegate_Double_Function();
    public delegate double Delegate_Double_Function_Double(double num);
    public delegate double Delegate_Double_Function_Stock(Stock stock);

    /* tools */
    public partial class Utility
    {
        /* Convert */
        public static int ToInt32(string str)
        {
            if (str == "") { return 0; }
            return Convert.ToInt32(str);
        }
        public static long ToInt64(string str)
        {
            if (str == "") { return 0; }
            return Convert.ToInt64(str);
        }
        public static float ToSingle(string str)
        {
            if (str == "") { return 0; }
            return (float)Math.Round(Convert.ToSingle(str), 2);
        }
        public static double ToDouble(string str)
        {
            if (str == "") { return 0; }
            str = fixNumString(str);
            return Math.Round(Convert.ToDouble(str), 2);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }
        
        public static bool Equal(double a, double b, double e = 0.0001)
        {
            double min = b - e;
            double max = b + e;
            if (a >= min && a <= max)
            {
                return true;
            }
            return false;
        }

        public static double Round2(double num)
        {
            return Math.Round(num, 2);
        }


        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static void CopyValue(object target, object origin)
        {
            try
            {
                Type type = origin.GetType();
                PropertyInfo[] props = type.GetProperties();
                foreach (PropertyInfo pInfo in props)
                {
                    if (pInfo.CanWrite)
                    {
                        string[] sParam = pInfo.PropertyType.FullName.Split('.');
                        Console.WriteLine("PropertyType.FullName = " + pInfo.PropertyType.FullName);
                        Console.WriteLine("PropertyType.FirstName = " + sParam[0]);
                        if (sParam[0] != "System")//failed in list
                        {
                            object sub_origin = pInfo.GetValue(origin, null);
                            Type sub_type = pInfo.GetType();
                            object sub_target = Activator.CreateInstance(pInfo.PropertyType);
                            CopyValue(sub_target, sub_origin);
                            pInfo.SetValue(target, sub_target, null);
                            continue;
                        }
                        else
                        {
                            object value = pInfo.GetValue(origin, null);
                            pInfo.SetValue(target, value, null);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private PropertyInfo[] GetPropertyInfoArray<T>(T t)
        {
            PropertyInfo[] props = null;
            try
            {
                Type type = typeof(T);
                object obj = Activator.CreateInstance(type);
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            catch// (Exception ex)
            { }
            return props;
        }

        public static string GetStackTrace()
        {
            string info = null;
            //设置为true，这样才能捕获到文件路径名和当前行数，当前行数为GetFrames代码的函数，也可以设置其他参数  
            StackTrace st = new StackTrace(true);
            //得到当前的所有堆栈  
            StackFrame[] sf = st.GetFrames();
            for (int i = 0; i < sf.Length; ++i)
            {
                info = info + "\r\n" + " FileName=" + sf[i].GetFileName() + " fullname =" + sf[i].GetMethod().DeclaringType.FullName + " function=" + sf[i].GetMethod().Name + " FileLineNumber=" + sf[i].GetFileLineNumber();
            }
            return info;
        }

        public static string fixNumString(string src)
        {
            string fixStr = src;
            if (fixStr == null || fixStr == "")
            {
                return fixStr;
            }
            for (int i = fixStr.Length - 1; i > 0; i--)
            {
                if (fixStr[i] >= '0' && fixStr[i] <= '9')
                {
                    continue;
                }
                if (fixStr[i] == '.' || fixStr[i] == '-')
                {
                    continue;
                }
                fixStr = fixStr.Remove(i, 1);
            }

            return fixStr;
        }

        public static string MatchString(string src, string strStart, string strEnd)
        {
            if (strStart != "")
            {
                int iStart = src.IndexOf(strStart);
                if (iStart == -1)
                {
                    return "";
                }
                iStart += strStart.Length;
                src = src.Substring(iStart);
            }
            if (strEnd != "")
            {
                int iEnd = src.IndexOf(strEnd);
                if (iEnd == -1)
                {
                    return "";
                }
                src = src.Substring(0, iEnd);
            }
            return src;
        }
        public static string GetValueAnd(string strStart, string strEnd, string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            string regex = @"^.*" + strStart + "(?<content>.+?)" + strEnd + ".*$";
            Regex rgClass = new Regex(regex, RegexOptions.Singleline);
            Match match = rgClass.Match(text);
            return match.Groups["content"].Value;
        }

        public static bool IsFileNotOpen(string path)
        {
            bool ret = true;
            if (File.Exists(path))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (System.Exception ex)
                {
                    Debug.Assert(ex.Message.ToString() == "");
                    ret = false;
                }
            }
            return ret;
        }

        public static int Assert(string strLog)
        {
            Log("Assert : " + strLog);
            Debug.Assert(false);
            return EST.EST_ERROR;
        }

        public static Mutex mutex_Log = new Mutex();
        public static string CurTime = "";
        public static void Log(string log)
        {
            mutex_Log.WaitOne();

            if (CurTime == "")
            {
                CurTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + GetClock();
            }
            string path = System.Environment.CurrentDirectory + "/Log/log " + CurTime + ".txt";
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            string fixlog = "[" + GetClock() + "] " + log;
            sw.WriteLine(fixlog);
            Console.WriteLine(fixlog);
            sw.Close();
            fs.Close();

            mutex_Log.ReleaseMutex();
        }
    }
    
    /* update */
    public partial class Utility
    {
        public static bool bNeedUpdate(DateTime dtLastUpdate)
        {
            if (dtLastUpdate.Ticks == 0)
            {
                return true;
            }

            if (DateTime.Now > dtLastUpdate.AddDays(1))
            {
                return true;
            }

            if (dtLastUpdate.Hour < 7)
            {
                return true;
            }

            return false;
        }
    }

    /* update */
    public partial class Utility
    {
        public static FileStream OpenFile(string relativePath)
        {
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("OpenFile " + path);
            if (!File.Exists(path))
            {
                Utility.Assert("File Not Exist! " + path);
                //return;
            }
            if (true)
            {

            }
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
            }
            catch (Exception)
            {
                Utility.Assert("Can not open file! " + path);
                throw;
            }
            return null;
        }

        public static string CheckWriteFilePath(string folderName, string fileName, bool bLog = false)
        {
            string folderPath = Environment.CurrentDirectory + folderName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string path = folderPath + "/" + fileName;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (bLog)
            {
                Utility.Log("WriteContent " + path);
            }
            return path;
        }
    }

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
