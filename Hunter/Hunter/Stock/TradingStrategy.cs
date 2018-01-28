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
    public partial class TradingStrategy
    {
        public static double Slippage = 0.05;
        public static double StopPriceRate = 0.70;

        public static TradingRunData RunData = new TradingRunData();
        public static Dictionary<int, Stock> SelectedStockDic = new Dictionary<int, Stock>();

        //position
        public static double Cash;
        public static double SharesValue;
        public static double Captital;
        public static Dictionary<int, Deal> DealDic = new Dictionary<int, Deal>();
        
        public Delegate_Int_Function_Stock dele_ProcessStock;
        
        public TradingStrategy()
        {
        }
        
        public void write_selected_stock()
        {
            string folderName = "/Data/Test";
            string fileName = "selected_stocklist.csv";
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

            foreach (KeyValuePair<int, Stock> it in SelectedStockDic)
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

        public void pick_stock()
        {
            int Count_Gem = 0;//创业板
            int Count_Unlisted = 0;//未上市
            int Count_Hold = 0;//现在持股
            //int Count_Fit = 0;//合适策略
            int Count_PB = 0;
            int Count_PE = 0;

            int NowDate = Utility.GetNowDate();
            SelectedStockDic.Clear();

            foreach (var it in Hunter.StockDic)
            {
                Stock stock = it.Value;

                //  exclude stock by error
                if (stock.EState != EST.EST_OK)
                {
                    //continue;
                }
                if (stock.sid / 100000 == 3)
                {
                    Count_Gem++;
                    continue;
                }
                if (stock.timeToMarket > NowDate)
                {
                    Count_Unlisted++;
                    continue;
                }

                //  process day data
                //stock.dmData.ProcessData(stock);
                if (stock.position.shares > 0)
                {
                    Count_Hold++;
                    continue;
                }
                else if (stock.position.shares == 0)
                {
                    if (stock.pb > 5)
                    {
                        Count_PB++;
                        continue;
                    }
                    if (stock.pe > 15)
                    {
                        Count_PE++;
                        continue;
                    }
                }

                SelectedStockDic.Add(stock.sid, stock);
            }

            if (Config.bTrue)
            {
                int cut = Hunter.StockDic.Count - SelectedStockDic.Count;
                Utility.Log("select_stock Now.Count = " + SelectedStockDic.Count);
                Utility.Log("select_stock Before.Count = " + Hunter.StockDic.Count);
                Utility.Log("select_stock cut = " + cut);
                Utility.Log("Count_Gem          = " + Count_Gem);
                Utility.Log("Count_Unlisted     = " + Count_Unlisted);
                Utility.Log("Count_Hold         = " + Count_Hold);
                Utility.Log("Count_PB           = " + Count_PB);
                Utility.Log("Count_PE           = " + Count_PE);
            }
        }
        
        #region /* Data Statistics */
        #endregion

        #region /* Back Test */
        #endregion


        #region /* new function */
        public void run_data_test()
        {
            //foreach stock exclude error
            //run_stock_day()
        }

        public void run_back_test()
        {
            Utility.Log("Start TradingStrategy run_back_test...");

            Cash = 100000;
            SharesValue = 0;
            Captital = 100000;

            RunData.start_date = 20160101;//20050104 20160101 20170101
            RunData.end_date = 20170501;  //20050301 20160815 20170501

            //foreach (var it in Hunter.StockDic)
            {
                //it.Value.dealList = new List<Deal>();
            }

            //select stock list
            select_stock();

            for (RunData.cur_date = RunData.start_date;
                RunData.cur_date <= RunData.end_date;
                RunData.cur_date = Utility.NextWorkDate(RunData.cur_date))
            {
                if (!Holiday.IsWorkDay(RunData.cur_date)) { continue; }
                Console.Clear();
                Utility.Log("Run date = " + RunData.cur_date);
                //todo:process day data to four tick data
                foreach (var it in SelectedStockDic)
                {
                    ExActivity(it.Value);

                    Stock stock = it.Value;
                    int ret = stock.GetTickData(RunData.cur_date);
                    if (ret != EST.EST_OK) continue;

                    stock.position.ProcessDay();

                    run_day(stock);
                }

                foreach (var item in SelectedStockDic)
                {
                    item.Value.mStrategy.ProcessDay();
                }
                DataMining.ProcessDay();
            }

            DataMining.WriteAllDeal();
            DataMining.WriteMonthlyData();

            Utility.Log("Finish TradingStrategy run_back_test... ");
        }
        
        public void select_stock()
        {
            SelectedStockDic.Clear();

            foreach (var it in Hunter.StockDic)
            {
                Stock stock = it.Value;

                //  exclude stock by error
                if (stock.EState != EST.EST_OK)
                {
                    //continue;
                }
                if (stock.sid / 100000 == 3)
                {
                    continue;
                }
                if (stock.timeToMarket > RunData.end_date)
                {
                    continue;
                }
                if (stock.sid != Config.DebugSid)
                {
                    continue;
                }
                //  process day data
                //stock.dmData.ProcessData(stock);

                if (stock.position.shares == 0)
                {
                    //  exclude stock by day data
                    /*
                    if (stock.dmData.eOperate != EOperateState.Buy)
                    {
                        continue;
                    }
                    */

                    //  exclude stock by fundamental data
                    //to do...
                }
                
                SelectedStockDic.Add(stock.sid, stock);
            }

            if (Config.bTrue)
            {
                int cut = Hunter.StockDic.Count - SelectedStockDic.Count;
                Utility.Log("select_stock cut = " + cut);
                Utility.Log("select_stock StockDic.Count = " + SelectedStockDic.Count);
                Utility.Log("select_stock StockList.Count = " + Hunter.StockDic.Count);
            }
        }

        public int run_day(Stock stock)
        {
            SortedList<int, TickData> dayTickData;
            stock.slTickData.TryGetValue(RunData.cur_date, out dayTickData);
            if (dayTickData == null)
            {
                Utility.Assert("dayTickData == null");
                return EST.EST_ERROR;
            }

            foreach (var it in dayTickData)
            {
                run_tick(stock, it.Value);
            }

            return EST.EST_OK;
        }

        public void run_real()
        {
            Utility.Log("Start TradingStrategy run_real...");
            
            RunData.cur_date = Utility.GetNowDate();
            foreach (var it in Hunter.StockDic)
            {
                it.Value.runData = new TradingRunData();
                //it.Value.runData.start_date = Math.Max(it.Value.data_start_date, Utility.AddYear(sRunData.start_date, -1));
                //it.Value.runData.end_date = Math.Min(it.Value.data_end_date, sRunData.end_date);
                it.Value.runData.cur_date = it.Value.data_start_date;
                it.Value.dmData = new StrategyData();
            }

            select_stock();//control stock count within 50
            
            //thread sleep time between open time
            
            foreach (var it in SelectedStockDic)
            {
                Stock stock = it.Value;
                run_tick_realtime(stock);
            }

            //time gap
            Thread.Sleep(1000 * 1);

            Utility.Log("Finish TradingStrategy run_real... ");
        }
        
        public void run_tick_realtime(Stock stock)
        {
            //get realtime tick data
            int ret = stock.GetTickData(RunData.cur_date);

            TickData tickdata = new TickData();

            //run strategy
            run_tick(stock, tickdata);
        }

        public void run_tick(Stock stock, TickData tickData)
        {
            stock.trend_30m.PreProcessData(tickData);
            
            Deal deal = stock.mStrategy.CalcDeal(stock, tickData);
            
            //sell
            while (deal.eType == Deal.ESell)
            {
                if (!isTradeTime(tickData.dt)) break;
                deal.sell_dt = new DateTime(tickData.dt.Ticks);
                deal.backtest_sell(deal.sell_price, deal.sell_shares, ESR.EPR_Normal);

                stock.position.AddDeal(deal);
                stock.mStrategy.ProcessDeal(stock, deal);
                deal.copy_position = Utility.Clone(stock.position);
                stock.position.Clear();
                DataMining.DealList.Add(deal);
                TradingStrategy.AddCash(deal.sell_total_money);
                break;
            }
            FinalDateCloseTrading(stock, tickData);

            //buy
            while (deal.eType == Deal.EBuy)
            {
                if (!isTradeTime(tickData.dt)) break;
                if (isEndDate()) break;
                deal.buy_dt = new DateTime(tickData.dt.Ticks);
                if (!deal.canAfford()) return;
                deal.backtest_buy(deal.buy_price, deal.buy_shares);

                stock.position.AddDeal(deal);
                stock.mStrategy.ProcessDeal(stock, deal);
                deal.copy_position = Utility.Clone(stock.position);
                DataMining.DealList.Add(deal);
                TradingStrategy.AddCash(deal.buy_total_money * -1);
                break;
            }

            stock.trend_day.AddTickData(tickData);
            stock.trend_day.mStrategyData.ProcessData(stock.trend_day);
            stock.trend_30m.AddTickData(tickData);
            stock.trend_30m.mStrategyData.ProcessData(stock.trend_30m);
        }
        #endregion
        
        #region /* Real Time */

        public virtual void RealTimeTrading() { }

        #endregion

        

        #region /* Money Control */
        public static double GetCurCash()
        {
            return 1000000;
        }
        public static double AddCash(double addMoney)
        {
            if (addMoney > 0)
            {
                Cash += addMoney;
            }
            else
            {
                if (Cash < Math.Abs(addMoney))
                {
                    Utility.Assert("Cash < addMoney");
                    return Cash;
                }
                Cash += addMoney;
            }
            return Cash;
        }
        public static double GetSharesValue()
        {
            SharesValue = 0;
            foreach(var it in SelectedStockDic)
            {
                SharesValue += it.Value.position.getMarketValue(it.Value.trend_30m.mPartData.close);
            }
            return SharesValue;
        }
        public static double GetCurCapital()
        {
            return Cash + GetSharesValue();
        }
        public static double GetBuyVolume()
        {
            return GetCurCapital() * 0.1 * 0.1;
        }
        #endregion

        #region /* Help Tools */
        //public virtual void Buy(Stock stock)
        //{
        //    if (ERunType == ERun_BackTest_Statistics)
        //    {
        //        DealController.Buy(stock.runData, stock);
        //    }
        //    else if (ERunType == ERun_BackTest_Simulate)
        //    {
        //        DealController.Buy(sRunData, stock);
        //    }
        //}

        //public virtual void Sell(Stock stock)
        //{
        //    if (ERunType == ERun_BackTest_Statistics)
        //    {
        //        DealController.Sell(stock.runData, stock);
        //    }
        //    else if (ERunType == ERun_BackTest_Simulate)
        //    {
        //        DealController.Sell(sRunData, stock);
        //    }
        //}

        
        public bool isTradeTime(DateTime dt)
        {
            if (dt.Hour < 9 || dt.Hour > 14)
            {
                return false;
            }
            else if (dt.Hour == 9 && dt.Minute < 30)
            {
                return false;
            }
            else if (dt.Hour == 14 && dt.Minute > 56)
            {
                return false;
            }
            return true;
        }
        public bool isEndDate()
        {
            return RunData.cur_date >= RunData.end_date;
        }

        public virtual bool IsFinalDate(Stock stock)
        {
            return stock.runData.cur_date >= stock.data_end_date || stock.runData.cur_date >= RunData.end_date;
        }
        public virtual bool IsOverFinalDate(Stock stock)
        {
            return stock.runData.cur_date > stock.data_end_date || stock.runData.cur_date > RunData.end_date;
        }

        public virtual void FinalDateCloseTrading(Stock stock, TickData tickData)
        {
            if (stock.position.shares == 0) return;

            if (!isEndDate()) return;

            Deal deal = new Deal(stock, Deal.ESell);
            deal.sell_dt = new DateTime(tickData.dt.Ticks);
            deal.backtest_sell(stock.trend_30m.lsPartData[stock.trend_30m.lsPartData.Count - 1].close,
                deal.getSellShares(),
                ESR.EPR_EndDate);
        }

        public virtual void ExActivity(Stock stock)
        {
            int iCurDate = RunData.cur_date;
            if (!stock.activity.ContainsKey(iCurDate)) return;
            
            stock.position.ExActivity(stock.activity[iCurDate]);
            stock.mStrategy.grid.ExActivity(stock.activity[iCurDate]);
            stock.trend_day.ExActivity(stock.activity[iCurDate]);
            stock.trend_30m.ExActivity(stock.activity[iCurDate]);
        }
        #endregion
        
    }
}
