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
    [Serializable]
    public class TickData
    {
        public const int IOCount = 7;
        public int id;
        public DateTime dt;
        public double price;
        public double price_change;
        public double volume;
        public double turnover;
        public string type;
    }
    [Serializable]
    public class DayData
    {
        public int date;
        public double open;
        public double high;
        public double low;
        public double close;
        public double volume;
        public double adj_close;

        public DayData()
        {
            date = 0;
            open = 0;
            high = 0;
            low = 0;
            close = 0;
            volume = 0;
            adj_close = 0;
        }

        public DayData(DayData other)
        {
            date = other.date;
            open = other.open;
            high = other.high;
            low = other.low;
            close = other.close;
            volume = other.volume;
            adj_close = other.adj_close;
        }
    };
    [Serializable]
    public class PartData
    {
        public int eLevel;
        public DateTime dt;//start date time

        public double open;
        public double high;
        public double low;
        public double close;
        public double volume;
        public PartData(int _eLevel, DateTime _dt)
        {
            eLevel = _eLevel;
            dt = _dt;
            open = 0;
            high = 0;
            low = 0;
            close = 0;
            volume = 0;
        }
        public void AddData(TickData tickData)
        {
            if (open == 0) { open = tickData.price; }

            if (high == 0) { high = tickData.price; }
            if (low == 0) { low = tickData.price; }
            if (close == 0) { close = tickData.price; }

            if (tickData.price > high) { high = tickData.price; }
            if (tickData.price < low) { low = tickData.price; }
            if (tickData.price != close) { close = tickData.price; }

            volume += tickData.volume;
        }
        public void AddData(PartData partData)
        {

        }
        public void ExActivity(ActivityData activity)
        {
            if (activity.bonus > 0)
            {
                high -= activity.bonus;
                low -= activity.bonus;
                open -= activity.bonus;
                close -= activity.bonus;
            }
            if (activity.multiply_shares > 0)
            {
                high = Math.Round(high / activity.multiply_shares, 2);
                low = Math.Round(low / activity.multiply_shares, 2);
                open = Math.Round(open / activity.multiply_shares, 2);
                close = Math.Round(close / activity.multiply_shares, 2);
            }
        }
    }

    public class ActivityData
    {
        public int date;
        public double total_shares;//unit hand (1 hand == 100 shares)
        public double issue_shares;
        public double issue_price;
        public double multiply_shares;
        public double bonus;
        public static int ReadParamsCount = 6;

        public ActivityData()
        {
            date = 0;
            total_shares = 0;
            issue_shares = 0;
            issue_price = 0;
            multiply_shares = 0;
            bonus = 0;
        }
    };
    
    public class Stock
    {
        //csv stock info
        public int sid;//代码
        public string sname;//名称
        public string industry;//细分行业
        public string area;//地区
        public double pe;//市盈率
        public double outstanding;//流通股本
        public double totals;//总股本(万)
        public double totalAssets;//总资产(万)
        public double liquidAssets;//流动资产
        public double fixedAssets;//固定资产
        public double reserved;//公积金
        public double reservedPerShare;//每股公积金
        public double eps;//每股收益
        public double bvps;//每股净资
        public double pb;//市净率
        public double timeToMarket;//上市日期
        public double undp;
        public double perundp;
        public double rev;
        public double profit;
        public double gpr;
        public double npr;
        public double holders;//持股人数

        public string eps_year;

        //extra info
        public string region;//所在交易所 sh sz 

        //stock data
        public int download_start_date;
        public int download_end_date;
        public int data_start_date;
        public int data_end_date;
        public SortedList<int, DayData> sdata;

        public int download_activity_date;
        public SortedList<int, ActivityData> activity;

        //tick data
        public int download_tick_date;
        public SortedList<int, SortedList<int, TickData>> slTickData;

        public int EState;

        public Strategy mStrategy;
        public Position position;
        public TradingRunData runData;
        public StrategyData dmData;
        public StrategyData buy_dmData;

        public TrendData trend_30m;
        public TrendData trend_day;

        public List<TradingData> TDList;

        public List<Deal> dealList;

        public Stock()
        {
            sid = 0;
            sname = "";
            region = "";

            download_start_date = 0;
            download_end_date = 0;
            data_start_date = 0;
            data_end_date = 0;
            sdata = new SortedList<int, DayData>();

            download_activity_date = 0;
            activity = new SortedList<int, ActivityData>();

            download_tick_date = 0;
            slTickData = new SortedList<int, SortedList<int, TickData>>();

            EState = EST.EST_OK;

            mStrategy = new Strategy();
            position = new Position();
            runData = new TradingRunData();
            dmData = new StrategyData();
            buy_dmData = new StrategyData();

            trend_30m = new TrendData(this, ELevelType.E30m);
            trend_day = new TrendData(this, ELevelType.EDay);

            TDList = new List<TradingData>();
        }

        public int LoadStockData()
        {
            //temp
            /*
            ReadStockData();

            List<int> ls_key = new List<int>(sdata.Keys);
            if (sdata.Count == 0)
            {
                EState = EST.EST_ERROR;
                return EState;
            }
            data_start_date = ls_key[0];
            data_end_date = ls_key[ls_key.Count - 1];
            */

            ReadActivityData();

            return EST.EST_OK;
        }

        //read data
        public int ReadStockData()
        {
            string relativePath = "/Data/StockData/" + sid.ToString().PadLeft(6, '0') + ".csv";
            string path = Environment.CurrentDirectory + relativePath;
            //Utility.Log("ReadContent " + path);

            if (!File.Exists(path))
            {
                //Utility.Log("File Not Exist! " + path);
                return EST.EST_ERROR;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            sdata.Clear();

            int iDate = 0;
            string line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length < 7)
                {
                    //Utility.Log("Read Error! sid = " + sid);
                    return EST.EST_ERROR;
                }

                iDate = Convert.ToInt32(sParam[0]);
                //if (!Holiday.IsWorkDay(iDate)) continue;
                if (sdata.ContainsKey(iDate))
                {
                    //Utility.Log("Repeat iDate = " + iDate);
                    return EST.EST_ERROR;
                }

                DayData stockData = new DayData();
                stockData.date = iDate;
                stockData.open = Utility.ToDouble(sParam[1]);
                stockData.high = Utility.ToDouble(sParam[2]);
                stockData.low = Utility.ToDouble(sParam[3]);
                stockData.close = Utility.ToDouble(sParam[4]);
                stockData.volume = Utility.ToDouble(sParam[5]);
                stockData.adj_close = Utility.ToDouble(sParam[6]);

                sdata.Add(iDate, stockData);
                
            }

            sr.Close();
            fs.Close();
            
            return EST.EST_OK;
        }
        public int WriteStockData()
        {
            string relativePath = "/Data/StockData/" + sid.ToString().PadLeft(6, '0') + ".csv";
            string path = System.Environment.CurrentDirectory + relativePath;
            //Utility.Log("WriteContent " + path);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("Date,Open,High,Low,Close,Volume,AdjClose");

            foreach (KeyValuePair<int, DayData> it in sdata)
            {
                if (!Holiday.IsWorkDay(it.Value.date)) continue;
                sw.WriteLine(
                    it.Value.date 
                    + "," + it.Value.open 
                    + "," + it.Value.high 
                    + "," + it.Value.low
                    + "," + it.Value.close 
                    + "," + it.Value.volume 
                    + "," + it.Value.adj_close
                    );//.ToString("0.00")
            }

            sw.Close();
            fs.Close();

            return EST.EST_OK;
        }
        
        public int ReadActivityData()
        {
            string path = System.Environment.CurrentDirectory + "/Data/Activity/" + sid.ToString().PadLeft(6, '0') + "_Activity.csv";
            if (!File.Exists(path))
            {
                return EST.EST_ERROR;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            activity.Clear();

            int id = 0;
            string line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length != ActivityData.ReadParamsCount)
                {
                    return EST.EST_ERROR;
                }

                id = Utility.ToInt32(sParam[0]);
                if (id == 0)
                {
                    continue;
                }
                if (activity.ContainsKey(id))
                {
                    Debug.Assert(!activity.ContainsKey(id));
                    return EST.EST_ERROR;
                }

                ActivityData activityData = new ActivityData();
                activityData.date = id;
                activityData.total_shares = Utility.ToDouble(sParam[1]);
                activityData.issue_shares = Utility.ToDouble(sParam[2]);
                activityData.issue_price = Utility.ToDouble(sParam[3]);
                activityData.multiply_shares = Utility.ToDouble(sParam[4]);
                activityData.bonus = Utility.ToDouble(sParam[5]);
                
                activity.Add(id, activityData);
            }

            sr.Close();
            fs.Close();

            return (int)EST.EST_OK;
        }
        public void WriteActivityData()
        {
            string path = System.Environment.CurrentDirectory + "/Data/Activity/" + sid.ToString().PadLeft(6, '0') + "_Activity.csv";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("Date,TotalShares,Shares,Price,Present,Bonus");

            foreach (KeyValuePair<int, ActivityData> it in activity)
            {
                sw.WriteLine(it.Value.date + "," + it.Value.total_shares + "," + it.Value.issue_shares
                    + "," + it.Value.issue_price + "," + it.Value.multiply_shares + "," + it.Value.bonus);
            }

            sw.Close();
            fs.Close();
        }
        
        public int ReadStockTickData(int iDate)
        {
            string path = System.Environment.CurrentDirectory + "/Data/TickData/"
                + sid.ToString().PadLeft(6, '0') + "/" + iDate + ".csv";

            if (!File.Exists(path))
            {
                return EST.EST_ERROR;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            SortedList<int, TickData> dayTickData = new SortedList<int, TickData>();

            int index = 0;
            int time = 0;
            string line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length != TickData.IOCount)
                {
                    Utility.Assert("sParam.Length != TickData.IOCount");
                    return EST.EST_ERROR;
                }

                index = Utility.ToInt32(sParam[0]);
                if (index == 0) { continue; }

                if (dayTickData.ContainsKey(index))
                {
                    Debug.Assert(!dayTickData.ContainsKey(index));
                    return EST.EST_ERROR;
                }

                TickData tickData = new TickData();
                tickData.id = index;
                time = Utility.ToInt32(sParam[1]);
                tickData.dt = new DateTime(Utility.GetYear(iDate),
                    Utility.GetMonth(iDate),
                    Utility.GetDay(iDate),
                    Utility.GetHour(time),
                    Utility.GetMinute(time),
                    Utility.GetSecond(time));
                tickData.price = Utility.ToDouble(sParam[2]);
                tickData.price_change = Utility.ToDouble(sParam[3]);
                tickData.volume = Utility.ToDouble(sParam[4]);
                tickData.turnover = Utility.ToDouble(sParam[5]);
                tickData.type = sParam[6];

                dayTickData.Add(index, tickData);
            }

            slTickData.Add(iDate, dayTickData);

            sr.Close();
            fs.Close();

            return EST.EST_OK;
        }
        public void WriteStockTickData(SortedList<int, TickData> dayTickData)
        {
            if (dayTickData == null || dayTickData.Count == 0)
            {
                return;
            }
            
            string folderName = "/Data/TickData/" + sid.ToString().PadLeft(6, '0');
            string fileName = dayTickData.ElementAt(0).Value.dt.ToString("yyyyMMdd") + ".csv";
            string path = Utility.CheckWriteFilePath(folderName, fileName);

            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("ID,Time,Price,PriceChange,Volume,Turnover,Type");

            foreach (var it in dayTickData)
            {
                sw.WriteLine(it.Value.id
                    + "," + it.Value.dt.ToString("HHmmss")
                    + "," + it.Value.price
                    + "," + it.Value.price_change
                    + "," + it.Value.volume
                    + "," + it.Value.turnover
                    + "," + it.Value.type);
            }

            sw.Close();
            fs.Close();
        }

        public int GetTickData(int iDate)
        {
            int ret = ReadStockTickData(iDate);
            if (ret == EST.EST_OK)
            {
                return EST.EST_OK;
            }
            else
            {
                ret = download_tick_data(iDate);
            }
            if (ret != EST.EST_OK)
            {
                Utility.Log("Failed GetTickData sid = " + sidToString() + " iDate = " + iDate);
                return EST.EST_ERROR;
            }
            return EST.EST_OK;
        }

        //helper
        public string sidToString()
        {
            return sid.ToString().PadLeft(6, '0');
        }
        public static string SidToString(int _sid)
        {
            return _sid.ToString().PadLeft(6, '0');
        }

        public int getNextDate()
        {
            List<int> ls_key = new List<int>(sdata.Keys);
            int index = sdata.IndexOfKey(runData.cur_date) + 1;
            if (index > sdata.Count - 1)
            {
                index = sdata.Count - 1;
            }
            return ls_key[index];
        }
        
        public string code_to_region()
        {
            int i = sid / 100000;
            if (i == 5 || i == 6 || i == 9)
            {
                region = "sh";
            }
            else
            {
                region = "sz";
            }
            return region;
        }
        public static string Code_To_Region(int _sid)
        {
            int i = _sid / 100000;
            string region;
            if (i == 5 || i == 6 || i == 9)
            {
                region = "sh";
            }
            else
            {
                region = "sz";
            }
            return region;
        }

        //download
        private int downloadActivityData_DZH()
        {
            //http://cj.gw.com.cn/news/stock/sh600000/gbfh.shtml
            string url = "http://cj.gw.com.cn/news/"
                + "stock/" + region + sid.ToString().PadLeft(6, '0') + "/gbfh.shtml";

            string strMsg = "";
            XmlDocument doc = new XmlDocument();
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

                strMsg = sr.ReadToEnd();

                sr.Close();
                sr.Dispose();
                response.Close();

                string table_start = "<div id=\"concept\" class=\"p10\">";
                string table_end = "</div>";
                string end_str = "</span> 历史分配纪录</h3>";

                int iTableStart = strMsg.IndexOf(table_start);
                int iStrEnd = strMsg.IndexOf(end_str);
                string strIssue = strMsg.Substring(iTableStart, iStrEnd - iTableStart);

                int iTableEnd = strMsg.IndexOf(table_end);
                strIssue = strIssue.Substring(0, iTableEnd + table_end.Length);

                strIssue = strIssue.Replace("&nbsp;", " ");
                strIssue = strIssue.Replace("&", "&amp;");

                //Utility.Log("strMsg = " + strMsg);
                doc.LoadXml(strIssue);
                if (doc == null)
                {
                    return (int)EST.EST_CONNECT_FAIL;
                }

                ActivityData aData = new ActivityData();
                XmlNode div = doc.ChildNodes[0];
                XmlNode dl = div.ChildNodes[0];
                XmlNode dd = dl.ChildNodes[1];
                if (dd == null)
                {
                    return (int)EST.EST_RECIVE_FAIL;
                }
                aData.date = Utility.ToDate(Utility.MatchString(dd.InnerText, "招股日期为", "，"));
                if (aData.date == 0)
                {
                    aData.date = Utility.ToDate(Utility.MatchString(dd.InnerText, "该公司于", "成功上市"));
                }
                aData.issue_shares = Utility.ToDouble(Utility.MatchString(dd.InnerText, "发行量为", "，")) / 100;
                aData.issue_price = Utility.ToDouble(Utility.MatchString(dd.InnerText, "上市发行价为", "。"));
                if (aData.issue_price == 0)
                {
                    aData.issue_price = Utility.ToDouble(Utility.MatchString(dd.InnerText, "上市首日开盘价为", "，"));
                }

                if (aData.date == 0 || aData.issue_shares == 0)
                {
                    return (int)EST.EST_ERROR;
                }

                activity.Add(aData.date, aData);
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return (int)EST.EST_NOT_FOUND;
                }
                return (int)EST.EST_CONNECT_FAIL;
            }
            
            return downloadActivityData_ShareBonus_DZH();
        }
        private int downloadActivityData_ShareBonus_DZH()
        {
            //http://cj.gw.com.cn/news/stock/sh600000/gbfh.shtml
            string url = "http://cj.gw.com.cn/news/"
                + "stock/" + region + sid.ToString().PadLeft(6, '0') + "/gbfh.shtml";

            string strMsg = "";
            XmlDocument doc = new XmlDocument();
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));

                strMsg = sr.ReadToEnd();

                sr.Close();
                sr.Dispose();
                response.Close();

                string table_start = "<table class=\" table_style_e\" width=\"100%\">";
                string table_end = "</table>";
                string end_str = "<!--股本分红结束-->";

                int iTableStart = strMsg.IndexOf(table_start);
                int iStrEnd = strMsg.IndexOf(end_str);
                strMsg = strMsg.Substring(iTableStart, iStrEnd - iTableStart);

                int iTableEnd = strMsg.IndexOf(table_end);
                strMsg = strMsg.Substring(0, iTableEnd + table_end.Length);

                strMsg = strMsg.Replace("<th ", "<td ");
                strMsg = strMsg.Replace("&nbsp;", " ");
                strMsg = strMsg.Replace("&", "&amp;");

                //Utility.Log("strMsg = " + strMsg);
                doc.LoadXml(strMsg);
                if (doc == null)
                {
                    return EST.EST_ERROR;
                }

                XmlNode table = doc.ChildNodes[0];
                for (int i = 0; i < table.ChildNodes.Count; i++)
                {
                    XmlNode node = table.ChildNodes[i];
                    if (node.Name != "tbody")
                    {
                        continue;
                    }
                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {
                        XmlNode tr = node.ChildNodes[j];
                        if (tr.ChildNodes.Count < 5)
                        {
                            return EST.EST_ERROR;
                        }
                        
                        if (tr.ChildNodes[3].InnerText.Contains("--"))
                        {
                            continue;
                        }

                        ActivityData aData = new ActivityData();
                        aData.date = Utility.ToDate(tr.ChildNodes[3].InnerText);

                        if (!tr.ChildNodes[0].InnerText.Contains("--"))
                        {
                            aData.multiply_shares += Utility.ToDouble(tr.ChildNodes[0].InnerText) / 10;
                        }
                        if (!tr.ChildNodes[1].InnerText.Contains("--"))
                        {
                            aData.multiply_shares += Utility.ToDouble(tr.ChildNodes[1].InnerText) / 10;
                        }
                        if (aData.multiply_shares != 0)
                        {
                            aData.multiply_shares += 1;
                        }

                        if (!tr.ChildNodes[2].InnerText.Contains("--"))
                        {
                            aData.bonus = Utility.ToDouble(tr.ChildNodes[2].InnerText) / 10;
                        }
                        
                        if (aData.date == 0 || (aData.multiply_shares == 0 && aData.bonus == 0))
                        {
                            return (int)EST.EST_ERROR;
                        }

                        if (activity.ContainsKey(aData.date))
                        {
                            if (activity[aData.date].multiply_shares != 0 && aData.multiply_shares != 0)
                            {
                                return (int)EST.EST_ERROR;
                            }
                            if (activity[aData.date].bonus != 0 && aData.bonus != 0)
                            {
                                return (int)EST.EST_ERROR;
                            }
                            if (activity[aData.date].multiply_shares == 0 && aData.multiply_shares != 0)
                            {
                                activity[aData.date].multiply_shares = aData.multiply_shares;
                            }
                            if (activity[aData.date].bonus == 0 && aData.bonus != 0)
                            {
                                activity[aData.date].bonus = aData.bonus;
                            }
                            //Debug.Assert(false);
                        }
                        else
                        {
                            activity.Add(aData.date, aData);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return (int)EST.EST_NOT_FOUND;
                }
                return (int)EST.EST_CONNECT_FAIL;
            }

            return EST.EST_OK;
        }
        //sina
        private int downloadActivityData_IssueStock_Sina()
        {
            //http://money.finance.sina.com.cn/corp/go.php/vISSUE_NewStock/stockid/600000.phtml
            string url = "http://money.finance.sina.com.cn/corp/go.php/vISSUE_NewStock/"
                + "stockid/" + sid.ToString().PadLeft(6, '0') + ".phtml";
            
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));

                string strMsg = sr.ReadToEnd();
                
                sr.Close();
                sr.Dispose();
                response.Close();

                string table_start = "<table id=\"comInfo1\">";
                string table_end = "</table>";
                string end_str = "romanceTables([\"comInfo1\"]);";

                int iTableStart = strMsg.IndexOf(table_start);
                int iStrEnd = strMsg.IndexOf(end_str);
                strMsg = strMsg.Substring(iTableStart, iStrEnd - iTableStart);

                int iTableEnd = strMsg.IndexOf(table_end);
                strMsg = strMsg.Substring(0, iTableEnd + table_end.Length);

                strMsg = strMsg.Replace("&nbsp;", " ");
                strMsg = strMsg.Replace("&", "&amp;");

                //Utility.Log("strMsg = " + strMsg);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strMsg);
                if (doc == null)
                {
                    return (int)EST.EST_CONNECT_FAIL;
                }

                int date1 = 0;
                int date2 = 0;
                int date3 = 0;
                int date4 = 0;
                int date5 = 0;
                ActivityData aData = new ActivityData();
                XmlNode table = doc.ChildNodes[0];
                for (int i = 0; i < table.ChildNodes.Count; i++)
                {
                    XmlNode node = table.ChildNodes[i];
                    if (node.Name == "tr")
                    {
                        XmlNode td1 = node.ChildNodes[0];
                        XmlNode strong = td1.ChildNodes[0];
                        if (strong.InnerText == "二级市场配售日期" && aData.date == 0)
                        {
                            XmlNode td2 = node.ChildNodes[1];
                            XmlNode a = td2.ChildNodes[0];
                            date1 = Utility.ToDate(a.InnerText);
                        }
                        else if (strong.InnerText == "上网发行日期" && aData.date == 0)
                        {
                            XmlNode td2 = node.ChildNodes[1];
                            XmlNode a = td2.ChildNodes[0];
                            date2 = Utility.ToDate(a.InnerText);
                        }
                        else if (strong.InnerText == "招股公告日" && aData.date == 0)
                        {
                            XmlNode td2 = node.ChildNodes[1];
                            XmlNode a = td2.ChildNodes[0];
                            date3 = Utility.ToDate(a.InnerText);
                        }
                        else if (strong.InnerText == "发行公告日" && aData.date == 0)
                        {
                            XmlNode td2 = node.ChildNodes[1];
                            XmlNode a = td2.ChildNodes[0];
                            date4 = Utility.ToDate(a.InnerText);
                        }
                        else if (strong.InnerText == "上市公告日" && aData.date == 0)
                        {
                            XmlNode td2 = node.ChildNodes[1];
                            XmlNode a = td2.ChildNodes[0];
                            date5 = Utility.ToDate(a.InnerText);
                        }
                        else if (strong.InnerText == "发行数量")//发行数量//上网发行数量
                        {
                            XmlNode td2 = node.ChildNodes[1];
                            string str = Utility.MatchString(td2.InnerText, "", "万股");
                            str = str.Replace(",", "");
                            aData.issue_shares = Convert.ToDouble(str) * 100;//wan to hand
                        }
                        else if (strong.InnerText == "发行价格")
                        {
                            XmlNode td2 = node.ChildNodes[1];
                            XmlNode a = td2.ChildNodes[0];
                            aData.issue_price = Math.Round(Convert.ToDouble(a.InnerText), 2);
                        }
                    }
                }

                if (date1 >= 0)
                {
                    aData.date = date1;
                }
                else if (date2 >= 0)
                {
                    aData.date = date2;
                }
                else if (date3 >= 0)
                {
                    aData.date = date3;
                }
                else if (date4 >= 0)
                {
                    aData.date = date4;
                }
                else if (date5 >= 0)
                {
                    aData.date = date5;
                }
                if (aData.date == 0 || aData.issue_shares == 0)
                {
                    return (int)EST.EST_ERROR;
                }

                activity.Add(aData.date, aData);
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return (int)EST.EST_NOT_FOUND;
                }
                return (int)EST.EST_CONNECT_FAIL;
            }
            
            return (int)EST.EST_OK;
        }
        private int downloadActivityData_AddStock_Sina()
        {
            //http://money.finance.sina.com.cn/corp/go.php/vISSUE_AddStock/stockid/600000.phtml
            string url = "http://money.finance.sina.com.cn/corp/go.php/vISSUE_AddStock/"
                + "stockid/" + sid.ToString().PadLeft(6, '0') + ".phtml";
            
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));

                string strMsg = sr.ReadToEnd();
                
                sr.Close();
                sr.Dispose();
                response.Close();

                string noData = "对不起，暂时没有相关增发记录";
                string table_start = "<table id=\"addStock0\">";
                string table_end = "</table>";
                string end_str = "romanceTables(['addStock0'";

                if (strMsg.IndexOf(noData) > 0)
                {
                    return (int)EST.EST_OK;
                }
                int iTableStart = strMsg.IndexOf(table_start);
                int iStrEnd = strMsg.IndexOf(end_str);
                strMsg = strMsg.Substring(iTableStart, iStrEnd - iTableStart);

                int iTableEnd = strMsg.LastIndexOf(table_end);
                strMsg = strMsg.Substring(0, iTableEnd);

                iTableEnd = strMsg.LastIndexOf(table_end);
                strMsg = strMsg.Substring(0, iTableEnd + table_end.Length);

                strMsg = "<div> " + strMsg + "</div>";

                strMsg = strMsg.Replace("&nbsp;", " ");
                strMsg = strMsg.Replace("&", "&amp;");

                //Utility.Log("strMsg = " + strMsg);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strMsg);

                if (doc == null)
                {
                    return (int)EST.EST_CONNECT_FAIL;
                }

                XmlNode div = doc.ChildNodes[0];
                for (int i = 0; i < div.ChildNodes.Count; i++)
                {
                    ActivityData aData = new ActivityData();
                    XmlNode table = div.ChildNodes[i];
                    for (int j = 0; j < table.ChildNodes.Count; j++)
                    {
                        XmlNode node = table.ChildNodes[j];
                        if (node.Name == "thead")
                        {
                            XmlNode tr = node.ChildNodes[0];
                            XmlNode th = tr.ChildNodes[0];
                            string str = Utility.MatchString(th.InnerText, "增发  公告日期：", "");
                            aData.date = Utility.ToDate(str);
                        }
                        else if (node.Name == "tr")
                        {
                            XmlNode td1 = node.ChildNodes[0];
                            XmlNode strong = td1.ChildNodes[0];
                            if (strong.InnerText == "发行数量：")
                            {
                                XmlNode td2 = node.ChildNodes[1];
                                string str = Utility.MatchString(td2.InnerText, "", "万股");
                                aData.issue_shares = Convert.ToDouble(str) * 100;//wan to hand
                            }
                            else if (strong.InnerText == "发行价格：")
                            {
                                XmlNode td2 = node.ChildNodes[1];
                                string str = Utility.MatchString(td2.InnerText, "", "元");
                                aData.issue_price = Math.Round(Convert.ToDouble(str), 2);
                            }
                        }
                    }

                    if (aData.date == 0 || aData.issue_shares == 0 || aData.issue_price == 0)
                    {
                        return EST.EST_ERROR;
                    }

                    if (activity.ContainsKey(aData.date))
                    {
                        if (activity[aData.date].issue_price < aData.issue_price)
                        {
                            activity[aData.date].issue_price = aData.issue_price;
                        }
                        activity[aData.date].issue_shares += aData.issue_shares;
                    }
                    else
                    {
                        activity.Add(aData.date, aData);
                    }
                }

            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return EST.EST_NOT_FOUND;
                }
                return EST.EST_CONNECT_FAIL;
            }
            
            return EST.EST_OK;
        }
        private int downloadActivityData_ShareBonus_Sina()
        {
            //http://money.finance.sina.com.cn/corp/go.php/vISSUE_ShareBonus/stockid/600000.phtml
            string url = "http://money.finance.sina.com.cn/corp/go.php/vISSUE_ShareBonus/"
                + "stockid/" + sid.ToString().PadLeft(6, '0') + ".phtml";
            
            XmlDocument doc_bonus = new XmlDocument();
            XmlDocument doc_shares = new XmlDocument();
            string strMsg = "";
            try
            {
                //bonus
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));

                strMsg = sr.ReadToEnd();
                
                sr.Close();
                sr.Dispose();
                response.Close();
                
                string table_start = "<!--分红 begin-->";
                string table_end = "<!--分红 end-->";
                int iTableStart = strMsg.IndexOf(table_start) + table_start.Length;
                int iTableEnd = strMsg.IndexOf(table_end);
                string str_bonus = strMsg.Substring(iTableStart, iTableEnd - iTableStart);
                str_bonus = str_bonus.Replace("<th ", "<td ");
                str_bonus = str_bonus.Replace("&nbsp;", " ");
                str_bonus = str_bonus.Replace("&", "&amp;");

                doc_bonus.LoadXml(str_bonus);
                if (doc_bonus == null)
                {
                    return EST.EST_ERROR;
                }

                XmlNode table = doc_bonus.ChildNodes[0];
                for (int i = 0; i < table.ChildNodes.Count; i++)
                {
                    XmlNode node = table.ChildNodes[i];
                    if (node.Name != "tbody")
                    {
                        continue;
                    }
                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {
                        XmlNode tr = node.ChildNodes[j];
                        if (tr.ChildNodes.Count < 4)
                        {
                            return (int)EST.EST_ERROR;
                        }

                        ActivityData aData = new ActivityData();
                        if (tr.ChildNodes[5].InnerText == "--")
                        {
                            continue;
                        }
                        aData.date = Utility.ToDate(tr.ChildNodes[5].InnerText);
                        aData.multiply_shares += Utility.ToDouble(tr.ChildNodes[1].InnerText) / 10;
                        aData.multiply_shares += Utility.ToDouble(tr.ChildNodes[2].InnerText) / 10;
                        if (aData.multiply_shares != 0)
                        {
                            aData.multiply_shares += 1;
                        }
                        aData.bonus = Utility.ToDouble(tr.ChildNodes[3].InnerText) / 10;

                        if (aData.date == 0 || (aData.multiply_shares == 0 && aData.bonus == 0))
                        {
                            return (int)EST.EST_ERROR;
                        }
                        if (activity.ContainsKey(aData.date))
                        {
                            if (activity[aData.date].multiply_shares != 0 && aData.multiply_shares != 0)
                            {
                                return (int)EST.EST_ERROR;
                            }
                            if (activity[aData.date].bonus != 0 && aData.bonus != 0)
                            {
                                return (int)EST.EST_ERROR;
                            }
                            if (activity[aData.date].multiply_shares == 0 && aData.multiply_shares != 0)
                            {
                                activity[aData.date].multiply_shares = aData.multiply_shares;
                            }
                            if (activity[aData.date].bonus == 0 && aData.bonus != 0)
                            {
                                activity[aData.date].bonus = aData.bonus;
                            }
                        }
                        else
                        {
                            activity.Add(aData.date, aData);
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return (int)EST.EST_NOT_FOUND;
                }
                return (int)EST.EST_CONNECT_FAIL;
            }
            
            try
            {
                //shares
                string table_start = "<!--配股 begin-->";
                string table_end = "<!--配股 end-->";
                int iTableStart = strMsg.IndexOf(table_start) + table_start.Length;
                int iTableEnd = strMsg.IndexOf(table_end);
                if (iTableStart == -1)
                {
                    return EST.EST_OK;
                }
                string str_shares = strMsg.Substring(iTableStart, iTableEnd - iTableStart);
                str_shares = str_shares.Replace("<th ", "<td ");
                str_shares = str_shares.Replace("&nbsp;", " ");
                str_shares = str_shares.Replace("&", "&amp;");

                doc_shares.LoadXml(str_shares);
                if (doc_shares == null)
                {
                    return EST.EST_OK;
                }

                XmlNode table = doc_shares.ChildNodes[0];
                for (int i = 0; i < table.ChildNodes.Count; i++)
                {
                    XmlNode node = table.ChildNodes[i];
                    if (node.Name != "tbody")
                    {
                        continue;
                    }
                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {
                        XmlNode tr = node.ChildNodes[j];
                        if (tr.ChildNodes.Count < 4)
                        {
                            return (int)EST.EST_ERROR;
                        }

                        ActivityData aData = new ActivityData();
                        if (tr.ChildNodes[5].InnerText == "--")
                        {
                            continue;
                        }
                        aData.date = Utility.ToDate(tr.ChildNodes[4].InnerText);
                        aData.multiply_shares += Utility.ToDouble(tr.ChildNodes[1].InnerText) / 10;
                        if (aData.multiply_shares != 0)
                        {
                            aData.multiply_shares += 1;
                        }
                        aData.issue_price = Utility.ToDouble(tr.ChildNodes[2].InnerText);
                        aData.bonus = 0;

                        if (aData.date == 0 || (aData.multiply_shares == 0 && aData.bonus == 0))
                        {
                            return (int)EST.EST_ERROR;
                        }
                        if (activity.ContainsKey(aData.date))
                        {
                            if (activity[aData.date].multiply_shares != 0 && aData.multiply_shares != 0)
                            {
                                return (int)EST.EST_ERROR;
                            }
                            if (activity[aData.date].bonus != 0 && aData.bonus != 0)
                            {
                                return (int)EST.EST_ERROR;
                            }
                            if (activity[aData.date].multiply_shares == 0 && aData.multiply_shares != 0)
                            {
                                activity[aData.date].multiply_shares = aData.multiply_shares;
                            }
                            if (activity[aData.date].bonus == 0 && aData.bonus != 0)
                            {
                                activity[aData.date].bonus = aData.bonus;
                            }
                        }
                        else
                        {
                            activity.Add(aData.date, aData);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return (int)EST.EST_NOT_FOUND;
                }
                return EST.EST_CONNECT_FAIL;
            }
            
            return EST.EST_OK;
        }
        private int initDownloadActivityData()
        {
            List<int> ls_key = new List<int>(activity.Keys);
            int id = sid;
            if (activity.Count == 0)
            {
                return (int)EST.EST_ERROR;
            }

            if (activity[ls_key[0]].issue_shares == 0)
            {
                return (int)EST.EST_ERROR;
            }
            activity[ls_key[0]].total_shares = activity[ls_key[0]].issue_shares;

            for (int i = 1; i < activity.Count; i++)
            {
                if (activity[ls_key[i]].issue_shares > 0)
                {
                    activity[ls_key[i]].total_shares = activity[ls_key[i - 1]].total_shares + activity[ls_key[i]].issue_shares;
                }
                else if (activity[ls_key[i]].multiply_shares > 0)
                {
                    activity[ls_key[i]].total_shares = activity[ls_key[i - 1]].total_shares * activity[ls_key[i]].multiply_shares;
                }
                else
                {
                    activity[ls_key[i]].total_shares = activity[ls_key[i - 1]].total_shares;
                }
            }
            return (int)EST.EST_OK;
        }
        public int DownloadActivityData()
        {
            //html format update at 20160614
            activity.Clear();

            /*
            int ret = 0;
            ret = downloadActivityData_DZH();
            if (ret != EST.EST_OK)
            {
                ret = downloadActivityData_IssueStock();
                if (ret == EST.EST_OK)
                {
                    ret = downloadActivityData_AddStock();
                }
                if (ret == EST.EST_OK)
                {
                    ret = downloadActivityData_ShareBonus();
                }
            }
            if (ret == EST.EST_OK)
            {
                ret = initDownloadActivityData();
            }
            if (ret == EST.EST_OK)
            {
                WriteActivityData();
            }
            else
            {
                ReadActivityData();
            }
            */

            int ret = 0;
            ret = downloadActivityData_ShareBonus_DZH();
            if (ret != EST.EST_OK)
            {
                ret = downloadActivityData_ShareBonus_Sina();
            }
            if (ret == EST.EST_OK)
            {
                WriteActivityData();
            }
            else
            {
                ReadActivityData();
            }

            return ret;
        }


        private int downloadStockDataByDate_Yahoo(int _startDate, int _endDate)
        {
            //"http://ichart.yahoo.com/table.csv?s=600000.ss&a=00&b=01&c=2010&d=04&e=01&f=2016&g=d"//20100101 20160501
            string url = "http://ichart.yahoo.com/table.csv?"
                + "s=" + sid.ToString().PadLeft(6, '0') + "." + region
                + "&a=" + (Utility.GetMonth(_startDate) - 1).ToString().PadLeft(2, '0')
                + "&b=" + Utility.GetDay(_startDate).ToString().PadLeft(2, '0')
                + "&c=" + Utility.GetYear(_startDate).ToString().PadLeft(4, '0')
                + "&d=" + (Utility.GetMonth(_endDate) - 1).ToString().PadLeft(2, '0')
                + "&e=" + Utility.GetDay(_endDate).ToString().PadLeft(2, '0')
                + "&f=" + Utility.GetYear(_endDate).ToString().PadLeft(4, '0')
                + "&g=d";

            WebClient client = new WebClient();
            Stream sm = null;
            StreamReader sr = null;
            try
            {
                sm = client.OpenRead(url);
                sr = new StreamReader(sm);
            }
            catch (System.Exception ex)
            {
                //not log in thread
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return (int)EST.EST_NOT_FOUND;
                }
                //Utility.Log("downloadStockData open url failed! stock id = " + sid);
                //Utility.Log(" Exception = " + ex.Message);
                return (int)EST.EST_CONNECT_FAIL;
            }
            if (sm == null || sr == null)
            {
                //Utility.Log("downloadStockData Stream is null! stock id = " + sid);
                return EST.EST_CONNECT_FAIL;
            }

            int id = 0;
            string line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length != 7)
                {
                    //Utility.Log("downloadStockData error! data id = " + id);
                    return (int)EST.EST_RECIVE_FAIL;
                }

                id = Utility.ToDate(sParam[0]);
                if (sdata.ContainsKey(id))
                {
                    //Utility.Log("downloadStockData stock date repeat! date = " + id);
                    return (int)EST.EST_RECIVE_FAIL;
                }

                DayData stockData = new DayData();
                stockData.date = id;
                stockData.open = Convert.ToSingle(sParam[1]);
                stockData.high = Convert.ToSingle(sParam[2]);
                stockData.low = Convert.ToSingle(sParam[3]);
                stockData.close = Convert.ToSingle(sParam[4]);
                stockData.volume = Convert.ToSingle(sParam[5]);
                stockData.adj_close = Convert.ToSingle(sParam[6]);

                sdata.Add(id, stockData);
            }

            sm.Close();
            return (int)EST.EST_OK;
        }

        private int downloadStockDataByDate_Sina_Old(int _startDate, int _endDate)
        {
            //http://biz.finance.sina.com.cn/stock/flash_hq/kline_data.php?symbol=sh600000&end_date=20160611&begin_date=20160601
            string url = "http://biz.finance.sina.com.cn/stock/flash_hq/kline_data.php"
                + "?symbol=" + region + sid.ToString().PadLeft(6, '0')
                + "&end_date=" + _endDate
                + "&begin_date=" + _startDate;

            WebClient client = new WebClient();
            Stream sm = null;
            XmlReader xmlr = null;
            try
            {
                sm = client.OpenRead(url);
                xmlr = XmlReader.Create(sm);
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return (int)EST.EST_NOT_FOUND;
                }
                return (int)EST.EST_CONNECT_FAIL;
            }
            if (sm == null || xmlr == null || xmlr.AttributeCount == 0)
            {
                return (int)EST.EST_CONNECT_FAIL;
            }

            int id = 0;
            while (xmlr.Read())
            {
                if (xmlr.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                if (xmlr.Name == "control")
                {
                    continue;
                }
                if (xmlr.AttributeCount != 7)
                {
                    return (int)EST.EST_RECIVE_FAIL;
                }

                id = Utility.ToDate(xmlr.GetAttribute(0));
                if (sdata.ContainsKey(id))
                {
                    continue;
                }

                DayData stockData = new DayData();
                stockData.date = id;
                stockData.open = Utility.ToDouble(xmlr.GetAttribute(1));
                stockData.high = Utility.ToDouble(xmlr.GetAttribute(2));
                stockData.close = Utility.ToDouble(xmlr.GetAttribute(3));
                stockData.low = Utility.ToDouble(xmlr.GetAttribute(4));
                stockData.volume = Utility.ToDouble(xmlr.GetAttribute(5));
                stockData.adj_close = 0;

                sdata.Add(id, stockData);
            }

            return EST.EST_OK;
        }
        private int downloadStockDataByDate_Sina(int _startDate, int _endDate)
        {
            //http://biz.finance.sina.com.cn/stock/flash_hq/kline_data.php?symbol=sh600000&end_date=20160611&begin_date=20160601
            string url = "http://biz.finance.sina.com.cn/stock/flash_hq/kline_data.php"
                + "?symbol=" + region + sid.ToString().PadLeft(6, '0')
                + "&end_date=" + _endDate
                + "&begin_date=" + _startDate;
            
            XmlDocument doc = new XmlDocument();
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));

                string strMsg = sr.ReadToEnd();

                doc.LoadXml(strMsg);
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "远程服务器返回错误: (404) 未找到。")
                {
                    return EST.EST_NOT_FOUND;
                }
                return EST.EST_CONNECT_FAIL;
            }
            if (doc == null)
            {
                return EST.EST_CONNECT_FAIL;
            }

            int id = 0;
            XmlNode control = doc.ChildNodes[1];
            for (int i = 0; i < control.ChildNodes.Count; i++)
            {
                XmlNode content = control.ChildNodes[i];
                if (content.Attributes.Count != 7)
                {
                    return EST.EST_RECIVE_FAIL;
                }

                id = Utility.ToDate(content.Attributes[0].Value);
                if (sdata.ContainsKey(id))
                {
                    continue;
                }

                DayData stockData = new DayData();
                stockData.date = id;
                stockData.open = Utility.ToDouble(content.Attributes[1].Value);
                stockData.high = Utility.ToDouble(content.Attributes[2].Value);
                stockData.close = Utility.ToDouble(content.Attributes[3].Value);
                stockData.low = Utility.ToDouble(content.Attributes[4].Value);
                stockData.volume = Utility.ToDouble(content.Attributes[5].Value);
                stockData.adj_close = 0;

                sdata.Add(id, stockData);
            }
            return EST.EST_OK;
        }
        
        public int download_tick_data(int iDate)
        {
            //http://market.finance.sina.com.cn/downxls.php?date=2005-01-04&symbol=sh600000
            string url = "http://market.finance.sina.com.cn/downxls.php?"
                + "date=" + Utility.Date2String(iDate, "YYYY-MM-DD")
                + "&symbol=" + region + sidToString();

            if (!Config.bFalse)
            {
                return EST.EST_SKIP;
            }
            Thread.Sleep(1000);

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
            int index = 5000;//Decrease from 5000
            int time = 0;
            int iCount = 0;
            SortedList<int, TickData> dayTickData = new SortedList<int, TickData>();
            while ((line = sr.ReadLine()) != null)
            {
                if (line.IndexOf("当天没有数据") > 0)
                {
                    Utility.Log("There is no data today! " + iDate);
                    return EST.EST_ERROR;
                }
                string[] sParam = line.Split('\t');
                if (sParam.Length < 6)
                {
                    Utility.Assert("download_tick_data Error! 0 " + iDate);
                    return EST.EST_ERROR;
                }

                time = Utility.ToTime(sParam[0]);
                if (time == 0) continue;

                if (dayTickData.ContainsKey(index))
                {
                    Utility.Assert("Repeat index = " + index + " time = " + time);
                    return EST.EST_ERROR;
                }

                iCount = 0;
                TickData tickData = new TickData();
                tickData.id = index;
                tickData.dt = new DateTime(Utility.GetYear(iDate),
                    Utility.GetMonth(iDate),
                    Utility.GetDay(iDate),
                    Utility.GetHour(time),
                    Utility.GetMinute(time),
                    Utility.GetSecond(time));
                tickData.price = Utility.ToDouble(sParam[++iCount]);//1
                if (sParam[++iCount] == "--")
                {
                    tickData.price_change = 0;
                }
                else
                {
                    tickData.price_change = Utility.ToDouble(sParam[iCount]);
                }
                tickData.volume = Utility.ToDouble(sParam[++iCount]);
                tickData.turnover = Utility.ToDouble(sParam[++iCount]);
                tickData.type = sParam[++iCount];

                dayTickData.Add(index, tickData);
                index--;
            }

            WriteStockTickData(dayTickData);

            slTickData.Add(iDate, dayTickData);
            
            return EST.EST_OK;
        }
        public int Download_Tick_Data()
        {
            int ret = 0;
            int NowDate = Utility.GetNowDate();
            int iDate = Math.Max(Hunter.StartDate, download_tick_date);

            for (; iDate < NowDate; iDate = Utility.AddDays(iDate, 1))
            {
                if (!Holiday.IsWorkDay(iDate))
                {
                    download_tick_date = iDate;
                    continue;
                }
                ret = download_tick_data(iDate);
                if (ret == EST.EST_OK)
                {
                    download_tick_date = iDate;
                    continue;
                }
                else if (ret == EST.EST_SKIP)
                {

                }
                else
                {
                    break;
                }
            }
            return EST.EST_OK;
        }

        public delegate int DeleDownloadProc(int _startDate, int _endDate);
        public int DownloadStockData()
        {
            int ret = 0, tmpr = 0;
            DeleDownloadProc downloadProc;
            downloadProc = downloadStockDataByDate_Sina;
            
            if (download_start_date == 0 || Config.bFroceDownload_StockData == true)
            {
                ret = downloadProc(Hunter.StartDate, Hunter.NowDate);
                if (ret == EST.EST_OK)
                {
                    download_start_date = Hunter.StartDate;
                    download_end_date = Hunter.NowDate;
                }
            }
            else if (download_start_date > Hunter.StartDate || download_end_date < Hunter.NowDate)
            {
                if (download_start_date > Hunter.StartDate)
                {
                    tmpr = downloadProc(Hunter.StartDate, Utility.AddDays(download_start_date, -1));
                    if (tmpr == EST.EST_OK)
                    {
                        download_start_date = Hunter.StartDate;
                    }
                    else
                    {
                        ret = tmpr;
                    }
                }
                if (download_end_date < Hunter.NowDate)
                {
                    tmpr = downloadProc(Utility.AddDays(download_end_date, 1), Hunter.NowDate);
                    if (tmpr == EST.EST_OK)
                    {
                        download_end_date = Hunter.NowDate;
                    }
                    else
                    {
                        ret = tmpr;
                    }
                }
            }
            else
            {
                ret = EST.EST_OK;//EST_SKIP
            }

            if (ret == EST.EST_OK)
            {
                WriteStockData();
            }

            if (download_activity_date == 0
                || download_activity_date < Hunter.NowDate
                || Config.bFroceDownload_ActivityData == true)
            {
                tmpr = DownloadActivityData();
                if (tmpr == EST.EST_OK)
                {
                    download_activity_date = Hunter.NowDate;
                }
                else
                {
                    ret = tmpr;
                }
            }

            return ret;
        }
        


    };
}
