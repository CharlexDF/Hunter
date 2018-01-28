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
using System.Reflection;

namespace Hunter
{
    public class Content_Config
    {

    }

    public class Content_Object
    {
        //public static string RelativePath = "/Content/stock_list.csv";
        public static int ReadContent(string relativePath, Delegate_Int_Function_ArrayString readProc)
        {
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("ReadContent " + path);

            if (!File.Exists(path))
            {
                Utility.Log("File Not Exist! " + path);
                Debug.Assert(false);
                return EST.EST_ERROR;
            }

            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string line = sr.ReadLine();
            //string[] arrTitle = line.Split(',');
            //Array.IndexOf(arrTitle, "");
            while ((line = sr.ReadLine()) != null)
            {
                string[] arrStr = line.Split(',');
                if (readProc(arrStr) != EST.EST_OK)
                {
                    Utility.Log("Read Error! " + path);
                    Debug.Assert(false);
                    return EST.EST_ERROR;
                }
            }

            sr.Close();
            fs.Close();

            return EST.EST_OK;
        }
    }

    public class Content_StockList : Content_Object
    {
        public static string RelativePath = "/Content/stock_list.csv";
        public static string Title = "ID,Name,Region";
        public static int PorpCount = DDL.GetPropertyCount(typeof(Content_StockList));
        public static Dictionary<int, Content_StockList> Dic = new Dictionary<int, Content_StockList>();

        int _id;
        public int id { get { return _id; } set { _id = value; } }
        string _name;
        public string name { get { return _name; } set { _name = value; } }
        string _region;
        public string region { get { return _region; } set { _region = value; } }
        public Content_StockList()
        {
            id = 0;
            name = "";
            region = "";
        }
        public static void Init()
        {
            RelativePath = "/Content/stock_list.csv";
            Title = "ID,Name,Region";
        }
        public static int Read(string[] arrStr)
        {
            if (arrStr.Length < PorpCount)
            {
                return EST.EST_ERROR;
            }
            Debug.Assert(arrStr.Length >= PorpCount);
            //int id = 0;
            Content_StockList content = new Content_StockList();
            content.id = Utility.ToInt32(arrStr[0]);
            if (content.id == 0) return EST.EST_OK;
            if (Dic.ContainsKey(content.id))
            {
                Utility.Log("Repeat id = " + content.id);
                Debug.Assert(false);
                return EST.EST_ERROR;
            }

            content.name = arrStr[1];
            content.region = arrStr[2];

            
            Dic.Add(content.id, content);
            return EST.EST_OK;
        }
    }

    public class DDL
    {
        public static Dictionary<string, object> DataDic = new Dictionary<string, object>();

        //public Delegate_Void_Function taskProc;
        //public static List<Content_StockList> StockList;

        public static void Init()
        {
            ReadData();
        }

        public static void ReadData()
        {
            Utility.Log("Read Content Data Start.");

            ReadContent("/Content/stock_list.csv", Content_StockList.Read);

            Utility.Log("Read Content Data End.");
        }

        public static void WriteData()
        {
            Utility.Log("Read Content Data Start.");

            //ReadContent<Content_StockList>("/Content/stock_list.csv", Content_StockList.Read);

            Utility.Log("Read Content Data End.");
        }

        public static PropertyInfo[] GetPropertyInfoArray(Type type)
        {
            PropertyInfo[] props = null;
            try
            {
                //Type type = typeof(T);
                object obj = Activator.CreateInstance(type);
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            catch //(Exception ex)
            { }
            return props;
        }

        public static int GetPropertyCount(Type type)
        {
            PropertyInfo[] props = null;
            try
            {
                //Type type = typeof(T);
                object obj = Activator.CreateInstance(type);
                props = obj.GetType().GetProperties();
                props = type.GetProperties();//BindingFlags.Public | BindingFlags.Instance
            }
            catch// (Exception ex)
            { }
            return props.Length;
        }

        public static bool ReadContent(string relativePath, Delegate_Int_Function_ArrayString readProc)
        {
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("ReadContent " + path);
            
            if (!File.Exists(path))
            {
                Utility.Log("File Not Exist! " + path);
                Debug.Assert(false);
                return false;
            }

            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            
            string line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                string[] arrStr = line.Split(',');
                if (readProc(arrStr) != EST.EST_OK)
                {
                    Utility.Log("Read Error! " + path);
                    Debug.Assert(false);
                    return false;
                }
            }

            sr.Close();
            fs.Close();

            return true;
        }

        //public static int WriteStockData<T>(string relativePath, Delegate_Int_Function_ArrayString<T> writeProc)
        //{
        //    //string path = System.Environment.CurrentDirectory + relativePath;
        //    //if (File.Exists(path))
        //    //{
        //    //    File.Delete(path);
        //    //}

        //    //FileStream fs = new FileStream(path, FileMode.Create);
        //    //StreamWriter sw = new StreamWriter(fs, Encoding.Default);

        //    //sw.WriteLine("Date,Open,High,Low,Close,Volume,AdjClose");

        //    //foreach (KeyValuePair<int, StockData> it in sdata)
        //    //{
        //    //    sw.WriteLine(it.Value.date + "," + it.Value.open + "," + it.Value.high + "," + it.Value.low
        //    //        + "," + it.Value.close + "," + it.Value.volume + "," + it.Value.adj_close);//.ToString("0.00")
        //    //}

        //    //sw.Close();
        //    //fs.Close();

        //    return EST.EST_OK;
        //}

        //public static bool WriteContent<T>(string relativePath, Delegate_Int_Function_ArrayString<T> readProc)
        //{
        //    string path = Environment.CurrentDirectory + relativePath;

        //    Utility.Log("ReadContent " + path);

        //    if (!File.Exists(path))
        //    {
        //        Utility.Log("File Not Exist! " + path);
        //        return false;
        //    }

        //    FileStream fs = new FileStream(path, FileMode.Open);
        //    StreamReader sr = new StreamReader(fs, Encoding.Default);

        //    Dictionary<int, T> ContentDic = new Dictionary<int, T>();

        //    string line = sr.ReadLine();
        //    int id = 0;
        //    while ((line = sr.ReadLine()) != null)
        //    {
        //        string[] arrStr = line.Split(',');
        //        if (arrStr.Length < DDL.GetPropertyCount(typeof(T)))
        //        {
        //            Console.WriteLine("Error data id = " + id);
        //            continue;
        //        }

        //        id = Utility.ToInt32(arrStr[0]);
        //        if (id == 0)
        //        {
        //            continue;
        //        }

        //        if (ContentDic.ContainsKey(id))
        //        {
        //            Utility.Log("Read Error! " + path);
        //            Utility.Log("Repeat id = " + id);
        //            Debug.Assert(!ContentDic.ContainsKey(id));
        //            return false;
        //        }

        //        T content;
        //        readProc(arrStr, out content);

        //        ContentDic.Add(id, content);
        //    }

        //    DataDic.Add(typeof(T).GetType().FullName, ContentDic);

        //    sr.Close();
        //    fs.Close();

        //    return true;
        //}
    }
}
