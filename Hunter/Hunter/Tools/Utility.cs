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

    


    
}
