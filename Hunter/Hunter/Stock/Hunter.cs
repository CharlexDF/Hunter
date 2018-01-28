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
    partial class Hunter
    {
        public static Dictionary<int, Stock> StockDic;
        public static List<Deal> DealList;

        public static int StartDate = 20050104;
        public static int EndDate = 20170427;
        public static List<int> BugList;
        public static int NowDate;
        public static int YesDate;
        
        public static void Init()
        {
            SystemData.Init();

            NowDate = Utility.GetNowDate();
            YesDate = Utility.GetLastWorkDate();
            BugList = new List<int>();

            StockDic = new Dictionary<int, Stock>();
            DealList = new List<Deal>();
            
            DateTime dtLastUpdate = new DateTime(SystemData.LUT_stocklist_csv);
            if (Utility.bNeedUpdate(dtLastUpdate))
            {
                int ret = DownloadUpdate();
                if (ret == EST.EST_OK)
                {
                    SystemData.LUT_stocklist_csv = DateTime.Now.Ticks;
                    SystemData.SaveData();
                }
                else
                {
                    Utility.Assert("DownloadUpdate LUT_stocklist_csv Fail");
                }
            }
            else
            {
                ReadStockList();
            }
            ReadStockInfo();

            ReadAllStockData();

            //ReadBugList();

            if (Config.bDownloadUpdate)
            {
                DownloadAllStockData();
            }
        }

        //load stock list from manual configuration
        private static int DownloadUpdate()
        {
            //ALL_STOCK_BASICS_FILE = P_TYPE['http'] + DOMAINS['oss'] + '/tsdata/%sall%s.csv'
            string url = "http://file.tushare.org/tsdata/all.csv";
            
            StreamReader sr = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));
            }
            catch (Exception ex)
            {
                Utility.Log("Error!" + ex.Message);
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return EST.EST_NOT_FOUND;
                }
                return EST.EST_CONNECT_FAIL;
            }
            if (sr == null)
            {
                return EST.EST_CONNECT_FAIL;
            }

            string line = sr.ReadLine();
            int id = 0;
            int iCount = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length < 23)
                {
                    Utility.Assert("DownloadUpdate Error! 0");
                    return EST.EST_ERROR;
                }

                id = Utility.ToInt32(sParam[0]);
                if (id == 0) continue;
                if (StockDic.ContainsKey(id))
                {
                    Utility.Assert("Repeat id = " + id);
                    return EST.EST_ERROR;
                }

                iCount = 0;
                Stock stock = new Stock();
                stock.sid = id;
                stock.sname = sParam[++iCount];//1
                stock.industry = sParam[++iCount];
                stock.area = sParam[++iCount];
                stock.pe = Utility.ToDouble(sParam[++iCount]);
                stock.outstanding = Utility.ToDouble(sParam[++iCount]);
                stock.totals = Utility.ToDouble(sParam[++iCount]);
                stock.totalAssets = Utility.ToDouble(sParam[++iCount]);
                stock.liquidAssets = Utility.ToDouble(sParam[++iCount]);
                stock.fixedAssets = Utility.ToDouble(sParam[++iCount]);
                stock.reserved = Utility.ToDouble(sParam[++iCount]);
                stock.reservedPerShare = Utility.ToDouble(sParam[++iCount]);
                stock.eps = Utility.ToDouble(sParam[++iCount]);
                stock.bvps = Utility.ToDouble(sParam[++iCount]);
                stock.pb = Utility.ToDouble(sParam[++iCount]);
                stock.timeToMarket = Utility.ToDouble(sParam[++iCount]);
                stock.undp = Utility.ToDouble(sParam[++iCount]);
                stock.perundp = Utility.ToDouble(sParam[++iCount]);
                stock.rev = Utility.ToDouble(sParam[++iCount]);
                stock.profit = Utility.ToDouble(sParam[++iCount]);
                stock.gpr = Utility.ToDouble(sParam[++iCount]);
                stock.npr = Utility.ToDouble(sParam[++iCount]);
                stock.holders = Utility.ToDouble(sParam[++iCount]);

                stock.code_to_region();

                StockDic.Add(id, stock);
            }

            WriteStockList();

            return EST.EST_OK;
        }
        private static void ReadStockList()
        {
            string relativePath = "/Data/stock_list.csv";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("ReadContent " + path);

            if (!File.Exists(path))
            {
                Utility.Assert("File Not Exist! " + path);
                return;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string line = sr.ReadLine();
            int id = 0;
            int iCount = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length < 23)
                {
                    Utility.Assert("Read Error! " + path);
                    return;
                }

                id = Utility.ToInt32(sParam[0]);
                if (id == 0) continue;
                if (StockDic.ContainsKey(id))
                {
                    Utility.Assert("Repeat id = " + id);
                    return;
                }

                iCount = 0;
                Stock stock = new Stock();
                stock.sid = id;
                stock.sname = sParam[++iCount];//1
                stock.industry = sParam[++iCount];
                stock.area = sParam[++iCount];
                stock.pe = Utility.ToDouble(sParam[++iCount]);
                stock.outstanding = Utility.ToDouble(sParam[++iCount]);
                stock.totals = Utility.ToDouble(sParam[++iCount]);
                stock.totalAssets = Utility.ToDouble(sParam[++iCount]);
                stock.liquidAssets = Utility.ToDouble(sParam[++iCount]);
                stock.fixedAssets = Utility.ToDouble(sParam[++iCount]);
                stock.reserved = Utility.ToDouble(sParam[++iCount]);
                stock.reservedPerShare = Utility.ToDouble(sParam[++iCount]);
                stock.eps = Utility.ToDouble(sParam[++iCount]);
                stock.bvps = Utility.ToDouble(sParam[++iCount]);
                stock.pb = Utility.ToDouble(sParam[++iCount]);
                stock.timeToMarket = Utility.ToDouble(sParam[++iCount]);
                stock.undp = Utility.ToDouble(sParam[++iCount]);
                stock.perundp = Utility.ToDouble(sParam[++iCount]);
                stock.rev = Utility.ToDouble(sParam[++iCount]);
                stock.profit = Utility.ToDouble(sParam[++iCount]);
                stock.gpr = Utility.ToDouble(sParam[++iCount]);
                stock.npr = Utility.ToDouble(sParam[++iCount]);
                stock.holders = Utility.ToDouble(sParam[++iCount]);

                stock.code_to_region();

                StockDic.Add(id, stock);
            }

            sr.Close();
            fs.Close();
        }
        private static void WriteStockList()
        {
            string folderName = "/Data";
            string fileName = "stock_list.csv";
            string path = Utility.CheckWriteFilePath(folderName, fileName);

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("ID,Name,Industry,Area"
                + ",pe,outstanding,totals,totalAssets"
                + ",liquidAssets,fixedAssets"
                + ",reserved,reservedPerShare"
                + ",eps,bvps"
                + ",pb,timeToMarket"
                + ",undp,perundp,rev,profit,gpr,npr"
                + ",holders");

            foreach (KeyValuePair<int, Stock> it in StockDic)
            {
                sw.WriteLine(
                    it.Value.sid.ToString().PadLeft(6, '0')
                    + "," + it.Value.sname
                    + "," + it.Value.industry
                    + "," + it.Value.area
                    + "," + it.Value.pe
                    + "," + it.Value.outstanding
                    + "," + it.Value.totals
                    + "," + it.Value.totalAssets
                    + "," + it.Value.liquidAssets
                    + "," + it.Value.fixedAssets
                    + "," + it.Value.reserved
                    + "," + it.Value.reservedPerShare
                    + "," + it.Value.eps
                    + "," + it.Value.bvps
                    + "," + it.Value.pb
                    + "," + it.Value.timeToMarket
                    + "," + it.Value.undp
                    + "," + it.Value.perundp
                    + "," + it.Value.rev
                    + "," + it.Value.profit
                    + "," + it.Value.gpr
                    + "," + it.Value.npr
                    + "," + it.Value.holders
                    );
            }

            sw.Close();
            fs.Close();
        }

        //read stock info from runtime record data
        private static void ReadStockInfo()
        {
            string relativePath = "/Data/stock_info.csv";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("ReadContent " + path);

            if (!File.Exists(path))
            {
                Utility.Assert("File Not Exist! " + path);
                return;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string line = sr.ReadLine();
            int id = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length < 6)
                {
                    Utility.Assert("Read Error! " + path);
                    return;
                }
                
                id = Utility.ToInt32(sParam[0]);
                if (!StockDic.ContainsKey(id))
                {
                    //Utility.Log("Read Error! id = " + id);
                    continue;
                }

                Stock stock = null;
                StockDic.TryGetValue(id, out stock);
                stock.download_start_date = Utility.ToInt32(sParam[3]);
                stock.download_end_date = Utility.ToInt32(sParam[4]);
                stock.download_activity_date = Utility.ToInt32(sParam[5]);
            }

            sr.Close();
            fs.Close();
        }
        private static void WriteStockInfo()
        {
            string relativePath = "/Data/stock_info.csv";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("WriteContent " + path);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("ID,Name,Region" 
                + ",Download_StartDate,Download_EndDate" 
                + ",Download_ActivityDate");
            
            foreach (KeyValuePair<int, Stock> it in StockDic)
            {
                sw.WriteLine(
                    it.Value.sid.ToString().PadLeft(6, '0') 
                    + "," + it.Value.sname 
                    + "," + it.Value.region 
                    + "," + it.Value.download_start_date 
                    + "," + it.Value.download_end_date 
                    + "," + it.Value.download_activity_date
                    );
            }

            sw.Close();
            fs.Close();
        }

        private static void ReadAllStockData()
        {
            Utility.Log("Start load all stock data...");

            if (Config.bFalse)
            {
                ThreadController.InitParams(StockDic.Count, false);
                foreach (var it in StockDic)
                {
                    ThreadController.StartProc(new ThreadData_Stock(it.Value, it.Value.LoadStockData));
                }
                ThreadController.WaitQueueFinished();
            }
            else
            {
                foreach (var it in StockDic)
                {
                    it.Value.LoadStockData();
                }
            }
            
            Utility.Log("Finish load all stock data...");
        }
        private static void WriteAllStockData()
        {
            Utility.Log("Start write all stock data...");

            ThreadController.InitParams(StockDic.Count, false);
            foreach (var it in StockDic)
            {
                //it.Value.WriteStockData();
                ThreadController.StartProc(new ThreadData_Stock(it.Value, it.Value.WriteStockData));
            }
            ThreadController.WaitQueueFinished();

            Utility.Log("Finish write all stock data...");
        }
        
        
        //download
        public static int DownloadAllStockData()
        {
            Utility.Log("Start download stock data...");

            NowDate = Utility.GetNowDate();
            
            if (Config.bDownloadUpdate)
            {
                ThreadController.InitParams(StockDic.Count, true);
                foreach (KeyValuePair<int, Stock> it in StockDic)
                {
                    //it.Value.DownloadStockData();
                    ThreadController.StartProc(new ThreadData_Stock(it.Value, it.Value.DownloadStockData));
                }
                ThreadController.WaitQueueFinished();
                WriteStockInfo();
            }
            else
            {
                foreach (KeyValuePair<int, Stock> it in StockDic)
                {
                    if (Config.bDebug)
                    {
                        if (it.Value.sid != Config.DebugSid)
                        {
                            continue;
                        }
                        //if (!BugList.Contains(it.Value.sid.ToString().PadLeft(6, '0')))
                        //{
                        //    continue;
                        //}
                    }
                    it.Value.DownloadStockData();
                }
            }

            Utility.Log("Finish download stock data...");

            return 0;
        }
        
        //helper
        public static void GetStockInfo(int sid, out Stock sinfo)
        {
            if (StockDic.Count == 0)
            {
                Utility.Log("findStockInfoBySID failed! m_StockInfo.Count = 0");
                Debug.Assert(false);
            }
            if (!StockDic.TryGetValue(sid, out sinfo))
            {
                Utility.Log("findStockInfoBySID failed! sid = " + sid);
                Debug.Assert(false);
            }
        }
        
        //Debug
        private static void ReadBugList()
        {
            string relativePath = "/Data/buglist.csv";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("ReadContent " + path);

            if (!File.Exists(path))
            {
                Utility.Assert("File Not Exist! " + path);
                return;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string sid = "";
            int id = 0;
            while ((sid = sr.ReadLine()) != null)
            {
                if (sid == "")
                {
                    continue;
                }
                id = Utility.ToInt32(sid);
                if (BugList.Contains(id))
                {
                    Utility.Log("stock id repeat! id = " + id);
                    Debug.Assert(!BugList.Contains(id));
                    return;
                }

                BugList.Add(id);
            }

            sr.Close();
            fs.Close();
        }
        public static void WriteBugList()
        {
            string relativePath = "/Data/buglist.csv";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("WriteContent " + path);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            for (int i = 0; i < BugList.Count; i++)
            {
                sw.WriteLine(BugList[i]);
            }

            sw.Close();
            fs.Close();
        }


        public static void Start()
        {
            Init();

            TradingStrategy TDStrategy = new TradingStrategy();
            if (Config.bTrue)
            {
                TDStrategy.run_back_test();
            }
            else
            {
                TDStrategy.pick_stock();
            }
        }
        
    }
}
