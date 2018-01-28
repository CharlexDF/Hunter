using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hunter
{
    public class DataMining
    {
        public static List<TradingData> TDList = new List<TradingData>();
        public static Dictionary<int, Dictionary<int, AnalyzeData>> Sid_Year_SumData_Dic = new Dictionary<int, Dictionary<int, AnalyzeData>>();
        public static Dictionary<int, AnalyzeData> Avg_Year_SumData_Dic = new Dictionary<int, AnalyzeData>();

        public static Mutex Mutex_Deal = new Mutex();
        public static void Statistics()
        {
            foreach (var it in TradingStrategy.SelectedStockDic)
            {
                foreach (var tradingData in it.Value.TDList)
                {
                    TDList.Add(tradingData);
                }
            }
            WriteDataMining("DM");
            WriteDataMining_StrategyStatistics();
        }

        public static void Statistics_BackTest()
        {
            foreach (var it in TradingStrategy.SelectedStockDic)
            {
                foreach (var tradingData in it.Value.TDList)
                {
                    TDList.Add(tradingData);
                }
            }
            WriteDataMining("BT");
            WriteDataMining_StrategyStatistics();
        }

        public static void Statistics_RealTime()
        {
            foreach (var it in TradingStrategy.SelectedStockDic)
            {
                foreach (var tradingData in it.Value.TDList)
                {
                    TDList.Add(tradingData);
                }
            }
            WriteDataMining("RT");
        }

        public static void ReadDataMining()
        {
            string path = System.Environment.CurrentDirectory + "/Data/DataMining.csv";
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string line = sr.ReadLine();
            //int id = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] sParam = line.Split(',');
                if (sParam.Length != 6)
                {
                    Debug.Assert(sParam.Length == 6);
                    return;
                }

                /*
                id = Utility.ToInt32(sParam[0]);

                StaData staData = null;
                StockList.TryGetValue(id, out stock);
                //stockInfo.sid = id;
                //stockInfo.sname = sParam[1];
                //stockInfo.region = sParam[2];
                stock.download_start_date = Utility.ToInt32(sParam[3]);
                stock.download_end_date = Utility.ToInt32(sParam[4]);
                stock.download_activity_date = Utility.ToInt32(sParam[5]);
                */
            }

            sr.Close();
            fs.Close();
        }
        public static void WriteDataMining(string name)
        {
            string CutTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + Utility.GetClock();
            string relativePath = "/Data/DataMining/" + name + "_TDList[" + CutTime + "].csv";
            string path = Environment.CurrentDirectory + relativePath;
            Utility.Log("WriteContent " + path);

            if (File.Exists(path) && Utility.IsFileNotOpen(path))
            {
                File.Delete(path);
            }

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine(
                "StockId,BuyDate,SellDate"
                + ",BuyPrice"
                + ",SellPrice"
                //+ ",SellMidPrice,SellLow"

                + ",ESellRet"
                + ",CountDays"


                + ",PERatio"
                + ",Risk"

                + ",MAMinClose"
                + ",MAMaxClose"
                //+ ",MA5High"
                //+ ",MA5Low"

                //+ ",LClose"
                //+ ",LOpen,LHigh,LLow,LClose"

                );

            for (int i = 0; i < TDList.Count; i++)
            {
                sw.WriteLine(
                    TDList[i].stock.sidToString()
                    + "," + TDList[i].deal.buy_date
                    + "," + TDList[i].deal.sell_date

                    + "," + TDList[i].deal.buy_price
                    + "," + TDList[i].deal.sell_price
                    //+ "," + TDList[i].deal.sell_mid_price
                    //+ "," + TDList[i].deal.sell_low

                    + "," + TDList[i].deal.ESellReason
                    + "," + TDList[i].deal.analyzData.count_days


                    + "," + TDList[i].buy_dmData.pe.pe
                    + "," + TDList[i].buy_dmData.pe.risk


                    + "," + TDList[i].buy_dmData.ma5.ma
                    + "," + TDList[i].buy_dmData.ma90.ma
                    //+ "," + TDList[i].dmData.ma5.high
                    //+ "," + TDList[i].dmData.ma5.low

                    //+ "," + TDList[i].buy_dmData.last_sdata.open
                    //+ "," + TDList[i].dmData.last_sdata.high
                    //+ "," + TDList[i].dmData.last_sdata.low
                    //+ "," + TDList[i].buy_dmData.last_sdata.close

                    );
            }

            sw.Close();
            fs.Close();
        }

        public static void WriteDataMining_StrategyStatistics()
        {
            int i, year;

            Utility.Log("==================== WriteDataMining_StrategyStatistics");
            Utility.Log("DmList.Count = " + TDList.Count);
            Utility.Log("==================== 1");

            Dictionary<int, List<AnalyzeData>> Year_DataList_Dic = new Dictionary<int, List<AnalyzeData>>();
            List<AnalyzeData> DmList_analyzData_List = new List<AnalyzeData>();
            for (i = 0; i < TDList.Count; i++)
            {
                Debug.Assert(TDList[i].deal.analyzData.count_days != 0);

                year = Utility.GetYear(TDList[i].deal.buy_date);
                if (Year_DataList_Dic.ContainsKey(year))
                {
                    List<AnalyzeData> analyzData_List;
                    Year_DataList_Dic.TryGetValue(year, out analyzData_List);
                    analyzData_List.Add(TDList[i].deal.analyzData);
                }
                else
                {
                    List<AnalyzeData> analyzData_List = new List<AnalyzeData>();
                    analyzData_List.Add(TDList[i].deal.analyzData);
                    Year_DataList_Dic.Add(year, analyzData_List);
                }

                DmList_analyzData_List.Add(TDList[i].deal.analyzData);
            }
            Year_DataList_Dic.Add(0, DmList_analyzData_List);

            Utility.Log("==================== 2");

            Dictionary<int, AnalyzeData> Year_SumData_Dic = new Dictionary<int, AnalyzeData>();
            List<int> ls_yskey = new List<int>(Year_DataList_Dic.Keys);
            for (i = 0; i < Year_DataList_Dic.Count; i++)
            {
                year = ls_yskey[i];
                List<AnalyzeData> analyzData_List;
                Year_DataList_Dic.TryGetValue(year, out analyzData_List);
                if (analyzData_List == null) { continue; }
                AnalyzeData sumData = new AnalyzeData();
                AnalyzeData.SumData(ref sumData, ref analyzData_List);
                Year_SumData_Dic.Add(year, sumData);
            }
            Utility.Log("==================== 3");

            //////////////////// write /////////////////////
            string CutTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + Utility.GetClock();
            string relativePath = "/Data/DataMining/StrategyStatistics[" + CutTime + "].csv";
            string path = System.Environment.CurrentDirectory + relativePath;
            Utility.Log("WriteContent " + path);

            if (File.Exists(path) && Utility.IsFileNotOpen(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            string line = "Year,A_Count,A_CntDays,A_TolReturns,A_AvgReturns";
            sw.WriteLine(line);
            
            List<int> year_List = new List<int>(Year_SumData_Dic.Keys);
            year_List.Sort();
            for (i = 0; i < year_List.Count; i++)
            {
                line = "";
                year = year_List[i];
                line += year;
                line += "," + Year_SumData_Dic[year].count;
                line += "," + Year_SumData_Dic[year].count_days;
                line += "," + Year_SumData_Dic[year].total_returns;
                line += "," + Year_SumData_Dic[year].average_returns;

                sw.WriteLine(line);
            }

            sw.Close();
            fs.Close();


            Utility.Log("==================== 4");
        }

        public static TrendData MonthlyData = new TrendData(null, ELevelType.EMonth);
        public static void ProcessDay()
        {
            TickData tickData = new TickData();
            tickData.dt = new DateTime(Utility.GetYear(TradingStrategy.RunData.cur_date),
                Utility.GetMonth(TradingStrategy.RunData.cur_date),
                Utility.GetDay(TradingStrategy.RunData.cur_date));
            tickData.price = TradingStrategy.GetCurCapital();
            MonthlyData.AddTickData(tickData);
        }
        public static void WriteMonthlyData()
        {
            Utility.Log("==================== WriteMonthlyData");
            Utility.Log("Month Count = " + MonthlyData.lsPartData.Count);
            Utility.Log("==================== 1");
            
            //////////////////// write /////////////////////
            string CutTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + Utility.GetClock();
            string relativePath = "/Data/DataMining/MonthlyData[" + CutTime + "].csv";
            string path = System.Environment.CurrentDirectory + relativePath;
            Utility.Log("WriteContent " + path);

            if (File.Exists(path) && Utility.IsFileNotOpen(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            string line = "Date,Open,High,Low,Close,MonthReturn,TotalReturn";
            sw.WriteLine(line);

            foreach (var it in MonthlyData.lsPartData)
            {
                sw.WriteLine(it.dt.ToString("yyyy-MM-dd")
                    + "," + it.open
                    + "," + it.high
                    + "," + it.low
                    + "," + it.close
                    + "," + Math.Round(it.close / it.open * 100, 2)
                    + "," + Math.Round(it.close / MonthlyData.lsPartData[0].open * 100, 2)
                    );
            }

            sw.Close();
            fs.Close();
            
            Utility.Log("==================== 2");
        }

        public static List<Deal> DealList = new List<Deal>();
        public static void WriteAllDeal()
        {
            Utility.Log("==================== WriteAllDeal");
            Utility.Log("DealList.Count = " + DealList.Count);
            Utility.Log("==================== 1");

            //////////////////// write /////////////////////
            string CutTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + Utility.GetClock();
            string relativePath = "/Data/DataMining/AllDeal[" + CutTime + "].csv";
            string path = System.Environment.CurrentDirectory + relativePath;
            Utility.Log("WriteContent " + path);

            if (File.Exists(path) && Utility.IsFileNotOpen(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            string line = "Date,Time,Type,Price,Shares,TotalMoney"
                +",P_CostPrice,P_CostMoney";
            sw.WriteLine(line);

            int BuyCount = 0;
            int SellCount = 0;
            foreach (var it in DealList)
            {
                if (it.eType == Deal.EBuy)
                {
                    BuyCount++;
                    sw.WriteLine(it.buy_dt.ToString("yyyy-MM-dd")
                    + "," + it.buy_dt.ToString("HH:mm:ss")
                    + "," + "Buy"
                    + "," + it.buy_price
                    + "," + it.buy_shares
                    + "," + it.buy_total_money
                    + "," + it.copy_position.getCostPrice()
                    + "," + it.copy_position.cost_money
                    );
                }
                else if (it.eType == Deal.ESell)
                {
                    SellCount++;
                    sw.WriteLine(it.sell_dt.ToString("yyyy-MM-dd")
                    + "," + it.sell_dt.ToString("HH:mm:ss")
                    + "," + "Sell"
                    + "," + it.sell_price
                    + "," + it.sell_shares
                    + "," + it.sell_total_money
                    + "," + it.copy_position.getCostPrice()
                    + "," + it.copy_position.cost_money
                    );
                }
                
            }
            sw.WriteLine("");
            sw.WriteLine("BuyCount,SellCount");
            sw.WriteLine(BuyCount + "," + SellCount);

            sw.Close();
            fs.Close();

            Utility.Log("BuyCount = " + BuyCount + ", SellCount = " + SellCount);
            Utility.Log("==================== 2");
        }


    }
}
