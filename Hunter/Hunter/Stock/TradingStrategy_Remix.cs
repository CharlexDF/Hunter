using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    //TradingStrategy
    /*
     
        public void DataStatistics()
        {
            Utility.Log("TradingStrategy DataStatistics Start...");

            RunData.start_date = 20150104;//20050104
            RunData.end_date = 20161203;

            //init stockList
            foreach (var it in Hunter.StockDic)
            {
                if (it.Value.EState != EST.EST_OK)
                {
                    continue;
                }
                it.Value.runData = new TradingRunData();
                it.Value.runData.start_date = Math.Max(it.Value.data_start_date, Utility.AddYear(RunData.start_date, -1));
                it.Value.runData.end_date = Math.Min(it.Value.data_end_date, RunData.end_date);
                it.Value.runData.cur_date = it.Value.data_start_date;
                it.Value.dmData = new StrategyData();

                SelectedStockDic.Add(it.Key, it.Value);
            }

            //////////////////////////////////////////////
            dele_ProcessStock = DataStatisticsStock;
            if (Config.bFalse)
            {
                ThreadController.InitParams(SelectedStockDic.Count, false);
                foreach (var it in SelectedStockDic)
                {
                    ThreadController.StartProc(new ThreadData_TradingStrategy(it.Value, dele_ProcessStock));
                }
                ThreadController.WaitQueueFinished();
            }
            else
            {
                foreach (var it in SelectedStockDic)
                {
                    if (it.Key != Config.DebugSid)
                    {
                        continue;
                    }
                    dele_ProcessStock(it.Value);
                }
            }

            //Statistics
            DataMining.Statistics();
            
            Utility.Log("TradingStrategy DataStatistics Finish... ");
        }

        public int DataStatisticsStock(Stock stock)
        {
            double hands = 0;
            double move_buy_price = 0;
            double move_sell_price = 0;

            for (stock.runData.cur_date = stock.runData.start_date;
                !IsOverFinalDate(stock);
                stock.runData.cur_date = Utility.AddDays(stock.runData.cur_date, 1))
            {
                ExActivity(stock);
                if (!stock.sdata.ContainsKey(stock.runData.cur_date))
                {
                    continue;
                }

                //ProcessData
                stock.dmData.ProcessData(stock);

                if (stock.runData.cur_date < RunData.start_date)
                {
                    continue;
                }

                //sell
                while (stock.deal.shares > 0)
                {
                    move_sell_price = stock.dmData.CalcSellPrice(stock);
                    if (move_sell_price > 0)
                    {
                        if (move_sell_price > stock.sdata[stock.runData.cur_date].high)
                        {
                            Utility.Log("ERROR!!! Sell too high");
                            break;
                        }
                        if (move_sell_price < stock.sdata[stock.runData.cur_date].low)
                        {
                            Utility.Log("ERROR!!! Sell too low");
                            break;
                        }
                        stock.deal.sell_price = move_sell_price;
                        stock.deal.ESellReason = ESR.EPR_Normal;
                        Deal.BackTest_Sell(stock);
                        break;
                    }

                    FinalDateCloseTrading(stock);
                    break;
                }

                //buy
                move_buy_price = stock.dmData.CalcBuyPrice(stock);
                while (move_buy_price > 0)
                {
                    if (IsFinalDate(stock)) break;
                    if (stock.deal.shares > 0) break;
                    if (move_buy_price > stock.sdata[stock.runData.cur_date].high)
                    {
                        Utility.Log("ERROR!!! Buy too high");
                        break;
                    }
                    if (move_buy_price < stock.sdata[stock.runData.cur_date].low)
                    {
                        Utility.Log("ERROR!!! Buy too low");
                        break;
                    }
                    stock.deal.buy_price = move_buy_price;
                    hands = Math.Floor(GetCurCapital() * 0.01 * 0.01 / stock.deal.buy_price) * 100;
                    if (hands == 0) break;
                    stock.deal.buy_shares = Convert.ToInt32(hands);
                    stock.deal.stop_price = stock.deal.buy_price * TradingStrategy.StopPriceRate;
                    stock.deal.sell_price = move_sell_price;
                    Deal.BackTest_Buy(stock);
                    break;
                }

                

            }
            return EST.EST_OK;
        }

    */
    /*
     
        public virtual void BackTestStatistics()
        {
            Utility.Log("Start TradingStrategy BackTestStatistics...");

            RunData.start_date = 20050104;
            RunData.end_date = 20051231;

            //init stockList
            foreach (var it in Hunter.StockDic)
            {
                if (it.Value.EState != EST.EST_OK)
                {
                    continue;
                }
                it.Value.runData = new TradingRunData();
                it.Value.runData.start_date = Math.Max(it.Value.data_start_date, RunData.start_date);
                it.Value.runData.end_date = Math.Min(it.Value.data_end_date, RunData.end_date);
                it.Value.runData.cur_date = it.Value.data_start_date;

                SelectedStockDic.Add(it.Key, it.Value);
            }

            /////////////////////////////////
            dele_ProcessStock = BackTestStock;
            if (!Config.bDebug)
            {
                ThreadController.InitParams(SelectedStockDic.Count, false);
                foreach (var it in SelectedStockDic)
                {
                    ThreadController.StartProc(new ThreadData_TradingStrategy(it.Value, dele_ProcessStock));
                }
                ThreadController.WaitQueueFinished();
            }
            else
            {
                foreach (KeyValuePair<int, Stock> it in SelectedStockDic)
                {
                    //if (it.Key != Config.DebugSid)
                    //{
                    //    continue;
                    //}
                    //if (it.Key / 10 != 60000)
                    //{
                    //    continue;
                    //}
                    dele_ProcessStock(it.Value);
                }
            }

            Utility.Log("Finish TradingStrategy Backtest_Statistics... ");
        }
        
        public virtual int BackTestStock(Stock stock) { return EST.EST_OK; }
        */









    /*
    public class TradingStrategy_Remix : TradingStrategy
    {
        ////////////////////////////////////////////////////////////////////

        public void ss_DataStatistics()
        {
            Utility.Log("TradingStrategy DataStatistics Start...");

            RunData.start_date = 20150104;//20050104
            RunData.end_date = 20161203;

            //init stockList
            foreach (var it in Hunter.StockDic)
            {
                if (it.Value.EState != EST.EST_OK)
                {
                    continue;
                }
                it.Value.runData = new TradingRunData();
                it.Value.runData.start_date = Math.Max(it.Value.data_start_date, Utility.AddYear(RunData.start_date, -1));
                it.Value.runData.end_date = Math.Min(it.Value.data_end_date, RunData.end_date);
                it.Value.runData.cur_date = it.Value.data_start_date;
                it.Value.dmData = new StrategyData();

                SelectedStockDic.Add(it.Key, it.Value);
            }

            //////////////////////////////////////////////
            dele_ProcessStock = DataStatisticsStock;
            if (Config.bFalse)
            {
                ThreadController.InitParams(SelectedStockDic.Count, false);
                foreach (var it in SelectedStockDic)
                {
                    ThreadController.StartProc(new ThreadData_TradingStrategy(it.Value, dele_ProcessStock));
                }
                ThreadController.WaitQueueFinished();
            }
            else
            {
                foreach (var it in SelectedStockDic)
                {
                    if (it.Key != Config.DebugSid)
                    {
                        continue;
                    }
                    dele_ProcessStock(it.Value);
                }
            }

            //Statistics
            DataMining.Statistics();

            Utility.Log("TradingStrategy DataStatistics Finish... ");
        }

        public int ss_DataStatisticsStock(Stock stock)
        {
            double hands = 0;
            double move_buy_price = 0;
            double move_sell_price = 0;

            for (stock.runData.cur_date = stock.runData.start_date;
                !IsOverFinalDate(stock);
                stock.runData.cur_date = Utility.AddDays(stock.runData.cur_date, 1))
            {
                ExActivity(stock);
                if (!stock.sdata.ContainsKey(stock.runData.cur_date))
                {
                    continue;
                }

                //ProcessData
                stock.dmData.ProcessData(stock);

                if (stock.runData.cur_date < RunData.start_date)
                {
                    continue;
                }

                //sell
                while (stock.deal.shares > 0)
                {
                    move_sell_price = stock.dmData.CalcSellPrice(stock);
                    if (move_sell_price > 0)
                    {
                        if (move_sell_price > stock.sdata[stock.runData.cur_date].high)
                        {
                            Utility.Log("ERROR!!! Sell too high");
                            break;
                        }
                        if (move_sell_price < stock.sdata[stock.runData.cur_date].low)
                        {
                            Utility.Log("ERROR!!! Sell too low");
                            break;
                        }
                        stock.deal.sell_price = move_sell_price;
                        stock.deal.ESellReason = ESR.EPR_Normal;
                        Deal.BackTest_Sell(stock);
                        break;
                    }

                    FinalDateCloseTrading(stock);
                    break;
                }

                //buy
                move_buy_price = stock.dmData.CalcBuyPrice(stock);
                while (move_buy_price > 0)
                {
                    if (IsFinalDate(stock)) break;
                    if (stock.deal.shares > 0) break;
                    if (move_buy_price > stock.sdata[stock.runData.cur_date].high)
                    {
                        Utility.Log("ERROR!!! Buy too high");
                        break;
                    }
                    if (move_buy_price < stock.sdata[stock.runData.cur_date].low)
                    {
                        Utility.Log("ERROR!!! Buy too low");
                        break;
                    }
                    stock.deal.buy_price = move_buy_price;
                    hands = Math.Floor(GetCurCapital() * 0.01 * 0.01 / stock.deal.buy_price) * 100;
                    if (hands == 0) break;
                    stock.deal.buy_shares = Convert.ToInt32(hands);
                    stock.deal.stop_price = stock.deal.buy_price * TradingStrategy.StopPriceRate;
                    stock.deal.sell_price = move_sell_price;
                    Deal.BackTest_Buy(stock);
                    break;
                }



            }
            return EST.EST_OK;
        }


        ////////////////////////////////////////////////////////////////////
        //BackTest
        public override void BackTestStatistics()
        {
            Utility.Log("Start TradingStrategy_Remix BackTestStatistics...");

            RunData.start_date = 20120104;
            RunData.end_date = 20161203;

            //DataMining.ReadDataMining_AnnualAnalyz();

            //init stockList
            foreach (var it in Hunter.StockDic)
            {
                if (it.Value.EState != EST.EST_OK)
                {
                    continue;
                }
                it.Value.runData = new TradingRunData();
                it.Value.runData.start_date = Math.Max(it.Value.data_start_date, Utility.AddYear(RunData.start_date, -1));
                it.Value.runData.end_date = Math.Min(it.Value.data_end_date, RunData.end_date);
                it.Value.runData.cur_date = it.Value.data_start_date;

                SelectedStockDic.Add(it.Key, it.Value);
            }

            //////////////////////////////////////////////
            dele_ProcessStock = BackTestStock_HighFrequecy;
            if (Config.bDebug)
            {
                foreach (var it in SelectedStockDic)
                {
                    if (it.Key != 600010)
                    {
                        continue;
                    }
                    dele_ProcessStock(it.Value);
                }
            }
            else
            {
                ThreadController.InitParams(SelectedStockDic.Count, false);
                foreach (var it in SelectedStockDic)
                {
                    ThreadController.StartProc(new ThreadData_TradingStrategy(it.Value, dele_ProcessStock));
                }
                ThreadController.WaitQueueFinished();
            }

            //Statistics
            DataMining.Statistics();

            Utility.Log("Finish TradingStrategy_Remix Backtest_Statistics... ");
        }

        //BackTestStock
        public int BackTestStock_Box(Stock stock)//trend
        {
            double hands = 0;
            int index = 0;
            int LastDate = 0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);
            
            for (; !IsOverFinalDate(stock); stock.runData.cur_date = Utility.AddDays(stock.runData.cur_date, 1))
            {
                ExActivity(stock);
                if (!stock.sdata.ContainsKey(stock.runData.cur_date))
                {
                    continue;
                }

                //ProcessData
                stock.dmData.ProcessData(stock);

                if (stock.runData.cur_date < RunData.start_date)
                {
                    //return EST.EST_OK;
                    continue;
                }

                int CountNum = 5;
                index = stock.sdata.IndexOfKey(stock.runData.cur_date);
                if (index < CountNum)
                {
                    continue;
                }
                LastDate = ls_key[index - 1];
                
                if (!IsGoodToTrade(stock))
                {
                    continue;
                }

                if (stock.deal.shares == 0)
                {
                    if (IsFinalDate(stock))
                    {
                        continue;
                    }
                    
                    double key_price = stock.dmData.b200.High;
                    if (key_price == 0)
                    {
                        continue;
                    }
                    if (key_price > stock.dmData.ma90.ma * 2)
                    {
                        continue;
                    }
                    if (stock.sdata[stock.runData.cur_date].high > key_price)
                    {
                        stock.deal.buy_price = key_price;
                        if (stock.sdata[stock.runData.cur_date].open > key_price)
                        {
                            stock.deal.buy_price = stock.sdata[stock.runData.cur_date].open;
                        }

                        hands = Math.Floor(GetCurCapital() * 0.01 * 0.01 / stock.deal.buy_price);
                        if (hands == 0)
                        {
                            continue;
                        }
                        stock.deal.buy_date = stock.runData.cur_date;
                        stock.deal.buy_shares = Convert.ToInt32(hands * 100);
                        stock.deal.stop_price = stock.deal.buy_price * TradingStrategy.StopPriceRate;

                        Deal.BackTest_Buy(stock);
                    }
                }
                else
                {
                    int idx = stock.sdata.IndexOfKey(stock.deal.buy_date);
                    //double key_price = tur.low;
                    double move_stop_price = stock.dmData.b200.Low;
                    //double cur_stop_price = Math.Max(stock.deal.stop_price, move_stop_price);

                    if (stock.sdata[stock.runData.cur_date].low > stock.deal.buy_adj_price * 1.02)
                    {
                        if (stock.deal.buy_date != 0 && stock.sdata[stock.runData.cur_date].low < move_stop_price)//stop
                        {
                            stock.deal.sell_date = stock.runData.cur_date;
                            stock.deal.sell_price = stock.deal.stop_price;
                            stock.deal.ESellReason = ESR.EPR_Stop;
                            Deal.BackTest_Sell(stock);
                        }
                    }

                    FinalDateCloseTrading(stock);
                }

            }
            return EST.EST_OK;
        }
        
        public int BackTestStock_HighFrequecy(Stock stock)//trend
        {
            int index = 0;
            DayData lastData;
            double hands = 0;
            double move_buy_price = 0;
            double move_sell_price = 0;
            double sell_rate = 1.01;
            List<int> ls_key = new List<int>(stock.sdata.Keys);

            for (stock.runData.cur_date = stock.runData.start_date;
                !IsOverFinalDate(stock);
                stock.runData.cur_date = Utility.AddDays(stock.runData.cur_date, 1))
            {
                ExActivity(stock);
                if (!stock.sdata.ContainsKey(stock.runData.cur_date))
                {
                    continue;
                }

                //ProcessData
                stock.dmData.ProcessData(stock);

                if (stock.runData.cur_date < RunData.start_date)
                {
                    continue;
                }
                
                if (stock.deal.shares == 0)
                {
                    if (IsFinalDate(stock)) continue;
                    
                    index = stock.sdata.IndexOfKey(stock.runData.cur_date);
                    if (index < 1) continue;
                    lastData = stock.sdata[ls_key[index - 1]];

                    move_buy_price = Utility.Round2((lastData.low + stock.dmData.ma5.low) * 0.5);
                    move_sell_price = Utility.Round2(stock.dmData.ma5.high * 1.02);
                    if (stock.dmData.ma5.ma > stock.dmData.ma90.ma) continue;
                    if (stock.dmData.ma5.ma > stock.dmData.ma90.ma) continue;
                    if (lastData.close > stock.dmData.ma5.ma) continue;
                    if (stock.dmData.ma5.high < stock.dmData.ma5.low * 1.03) continue;

                    if (move_buy_price == 0) continue;

                    if (stock.sdata[stock.runData.cur_date].low < move_buy_price)
                    {
                        if (stock.sdata[stock.runData.cur_date].open < move_buy_price)
                        {
                            move_buy_price = stock.sdata[stock.runData.cur_date].open;
                        }
                        stock.deal.buy_price = move_buy_price;
                        hands = Math.Floor(GetCurCapital() * 0.01 * 0.01 / stock.deal.buy_price) * 100;
                        if (hands == 0) continue;
                        stock.deal.buy_shares = Convert.ToInt32(hands);
                        stock.deal.stop_price = stock.deal.buy_price * TradingStrategy.StopPriceRate;
                        stock.deal.sell_price = move_sell_price;

                        Deal.BackTest_Buy(stock);
                    }
                }
                else
                {
                    //if (stock.dmData.analyzData.count_days == 1)
                    //{
                    //    sell_rate = 1.02;
                    //}
                    //else
                    //{
                    //    sell_rate = 1.01;
                    //}
                    move_sell_price = stock.deal.sell_price;// Math.Max(stock.deal.stop_price, stock.deal.sell_price);
                    if (stock.sdata[stock.runData.cur_date].low < move_sell_price
                        && move_sell_price < stock.sdata[stock.runData.cur_date].high)
                    {
                        stock.deal.calcSellTotalMoney(move_sell_price);
                        
                        if (stock.deal.sell_total_money > stock.deal.buy_total_money * sell_rate)
                        {
                            if (stock.sdata[stock.runData.cur_date].open > move_sell_price)
                            {
                                move_sell_price = stock.sdata[stock.runData.cur_date].open;
                            }
                            stock.deal.sell_price = move_sell_price;
                            stock.deal.ESellReason = ESR.EPR_Normal;
                            Deal.BackTest_Sell(stock);
                        }
                    }

                    FinalDateCloseTrading(stock);
                }

            }
            return EST.EST_OK;
        }

        public int BackTestStock_Chan(Stock stock)
        {
            int index = 0;
            DayData lastData;
            double hands = 0;
            double move_buy_price = 0;
            double move_sell_price = 0;
            double sell_rate = 1.01;
            List<int> ls_key = new List<int>(stock.sdata.Keys);

            for (stock.runData.cur_date = stock.runData.start_date;
                !IsOverFinalDate(stock);
                stock.runData.cur_date = Utility.AddDays(stock.runData.cur_date, 1))
            {
                ExActivity(stock);
                if (!stock.sdata.ContainsKey(stock.runData.cur_date))
                {
                    continue;
                }

                //ProcessData
                stock.dmData.ProcessData(stock);

                if (stock.runData.cur_date < RunData.start_date)
                {
                    continue;
                }

                if (stock.deal.shares == 0)
                {
                    if (IsFinalDate(stock)) continue;

                    index = stock.sdata.IndexOfKey(stock.runData.cur_date);
                    if (index < 1) continue;
                    lastData = stock.sdata[ls_key[index - 1]];

                    move_buy_price = Utility.Round2((lastData.low + stock.dmData.ma5.low) * 0.5);
                    move_sell_price = Utility.Round2(stock.dmData.ma5.high * 1.02);
                    if (stock.dmData.ma5.ma > stock.dmData.ma90.ma) continue;
                    if (stock.dmData.ma5.ma > stock.dmData.ma90.ma) continue;
                    if (lastData.close > stock.dmData.ma5.ma) continue;
                    if (stock.dmData.ma5.high < stock.dmData.ma5.low * 1.03) continue;

                    if (move_buy_price == 0) continue;

                    if (stock.sdata[stock.runData.cur_date].low < move_buy_price)
                    {
                        if (stock.sdata[stock.runData.cur_date].open < move_buy_price)
                        {
                            move_buy_price = stock.sdata[stock.runData.cur_date].open;
                        }
                        stock.deal.buy_price = move_buy_price;
                        hands = Math.Floor(GetCurCapital() * 0.01 * 0.01 / stock.deal.buy_price) * 100;
                        if (hands == 0) continue;
                        stock.deal.buy_shares = Convert.ToInt32(hands);
                        stock.deal.stop_price = stock.deal.buy_price * TradingStrategy.StopPriceRate;
                        stock.deal.sell_price = move_sell_price;

                        Deal.BackTest_Buy(stock);
                    }
                }
                else
                {
                    //if (stock.dmData.analyzData.count_days == 1)
                    //{
                    //    sell_rate = 1.02;
                    //}
                    //else
                    //{
                    //    sell_rate = 1.01;
                    //}
                    move_sell_price = stock.deal.sell_price;// Math.Max(stock.deal.stop_price, stock.deal.sell_price);
                    if (stock.sdata[stock.runData.cur_date].low < move_sell_price
                        && move_sell_price < stock.sdata[stock.runData.cur_date].high)
                    {
                        stock.deal.calcSellTotalMoney(move_sell_price);

                        if (stock.deal.sell_total_money > stock.deal.buy_total_money * sell_rate)
                        {
                            if (stock.sdata[stock.runData.cur_date].open > move_sell_price)
                            {
                                move_sell_price = stock.sdata[stock.runData.cur_date].open;
                            }
                            stock.deal.sell_price = move_sell_price;
                            stock.deal.ESellReason = ESR.EPR_Normal;
                            Deal.BackTest_Sell(stock);
                        }
                    }

                    FinalDateCloseTrading(stock);
                }

            }
            return EST.EST_OK;
        }

        //filter
        public bool IsGoodToTrade(Stock stock)
        {
            Dictionary<int, AnalyzeData> Year_sumData_Dic;
            if (!DataMining.Sid_Year_SumData_Dic.TryGetValue(stock.sid, out Year_sumData_Dic))
            {
                return false;
            }

            int cur_year = Utility.GetYear(stock.runData.cur_date);
            int year = cur_year - 3;
            for (; year <= cur_year; year++)
            {
                AnalyzeData sumData;
                if (!Year_sumData_Dic.TryGetValue(year, out sumData))
                {
                    return false;
                }
                AnalyzeData annualData;
                if (!DataMining.Avg_Year_SumData_Dic.TryGetValue(year, out annualData))
                {
                    return false;
                }
                if (sumData.average_returns < annualData.average_returns)
                {
                    return false;
                }
            }
            
            return true;
        }




        ////////////////////////////////////////////////////////////////////
        //RealTime
        public override void RealTimeTrading()
        {
            Utility.Log("Start TradingStrategy_Remix RealTimeTrading...");

            RunData.start_date = Utility.AddYear(Utility.GetLastWorkDate(), -1);
            RunData.end_date = Utility.GetNowDate();

            //init stockList
            foreach (var it in Hunter.StockDic)
            {
                if (it.Value.EState != EST.EST_OK)
                {
                    continue;
                }
                it.Value.runData = new TradingRunData();

                it.Value.runData.start_date = Math.Max(it.Value.data_start_date, RunData.start_date);
                it.Value.runData.end_date = Math.Min(it.Value.data_end_date, RunData.end_date);
                it.Value.runData.cur_date = RunData.end_date;

                SelectedStockDic.Add(it.Key, it.Value);
            }

            //////////////////////////////////////////////
            dele_ProcessStock = RealTimeStock_Robbery;
            if (Config.bDebug)
            {
                foreach (var it in SelectedStockDic)
                {
                    if (it.Key != Config.DebugSid)
                    {
                        continue;
                    }
                    dele_ProcessStock(it.Value);
                }
            }
            else
            {
                ThreadController.InitParams(SelectedStockDic.Count, false);
                foreach (var it in SelectedStockDic)
                {
                    ThreadController.StartProc(new ThreadData_TradingStrategy(it.Value, dele_ProcessStock));
                }
                ThreadController.WaitQueueFinished();
            }

            //Statistics
            DataMining.Statistics_RealTime();

            Utility.Log("Finish TradingStrategy_Remix RealTimeTrading... ");
        }

        public int RealTimeStock_Robbery(Stock stock)
        {
            int index = 0;
            DayData lastData;
            double hands = 0;
            double move_buy_price = 0;
            double move_sell_price = 0;
            //double sell_rate = 1.0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);

            for (stock.runData.cur_date = RunData.end_date;
                !IsOverFinalDate(stock);
                stock.runData.cur_date = Utility.AddDays(stock.runData.cur_date, 1))
            {
                ExActivity(stock);
                if (!stock.sdata.ContainsKey(stock.runData.cur_date)) continue;
                if (stock.runData.cur_date != RunData.end_date) continue;

                //ProcessData
                stock.dmData.ProcessData(stock);
                
                if (stock.deal.shares == 0)
                {
                    index = stock.sdata.IndexOfKey(stock.runData.cur_date);
                    if (index < 1) continue;
                    lastData = stock.sdata[ls_key[index - 1]];

                    move_buy_price = Utility.Round2(stock.dmData.ma5.low * 0.98);
                    move_sell_price = Utility.Round2(stock.dmData.ma5.high * 1.02);
                    if (stock.dmData.ma5.ma > stock.dmData.ma90.ma) continue;
                    //if (stock.dmData.ma5.ma > stock.dmData.ma20.ma) continue;
                    if (lastData.close > stock.dmData.ma5.ma) continue;
                    if (stock.dmData.ma5.high < stock.dmData.ma5.low * 1.03) continue;

                    if (move_buy_price == 0) continue;

                    stock.deal.buy_price = move_buy_price;
                    hands = Math.Floor(GetCurCapital() * 0.01 * 0.01 / stock.deal.buy_price) * 100;
                    if (hands == 0) continue;
                    stock.deal.buy_shares = Convert.ToInt32(hands);
                    stock.deal.stop_price = stock.deal.buy_price * StopPriceRate;
                    stock.deal.sell_price = move_sell_price;

                    Deal.RealTime_Buy(stock);
                }

            }
            return EST.EST_OK;
        }

        public int RealTimeStock_Tortoise(Stock stock)//trend
        {
            double hands = 0;
            int index = 0;
            int LastDate = 0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);

            for (; !IsOverFinalDate(stock); stock.runData.cur_date = Utility.AddDays(stock.runData.cur_date, 1))
            {
                ExActivity(stock);
                if (!stock.sdata.ContainsKey(stock.runData.cur_date))
                {
                    continue;
                }

                //ProcessData
                stock.dmData.ProcessData(stock);

                if (stock.runData.cur_date < RunData.start_date)
                {
                    //return EST.EST_OK;
                    continue;
                }

                int CountNum = 5;
                index = stock.sdata.IndexOfKey(stock.runData.cur_date);
                if (index < CountNum)
                {
                    continue;
                }
                LastDate = ls_key[index - 1];

                if (!IsGoodToTrade(stock))
                {
                    continue;
                }

                if (stock.deal.shares == 0)
                {
                    if (IsFinalDate(stock))
                    {
                        continue;
                    }

                    double key_price = stock.dmData.b200.High;
                    if (key_price == 0)
                    {
                        continue;
                    }
                    if (key_price > stock.dmData.ma90.ma * 2)
                    {
                        continue;
                    }
                    if (stock.sdata[stock.runData.cur_date].high > key_price)
                    {
                        stock.deal.buy_price = key_price;
                        if (stock.sdata[stock.runData.cur_date].open > key_price)
                        {
                            stock.deal.buy_price = stock.sdata[stock.runData.cur_date].open;
                        }

                        hands = Math.Floor(GetCurCapital() * 0.01 * 0.01 / stock.deal.buy_price);
                        if (hands == 0)
                        {
                            continue;
                        }
                        stock.deal.buy_date = stock.runData.cur_date;
                        stock.deal.buy_shares = Convert.ToInt32(hands * 100);
                        stock.deal.stop_price = stock.deal.buy_price * TradingStrategy.StopPriceRate;

                        Deal.BackTest_Buy(stock);
                    }
                }
                else
                {
                    int idx = stock.sdata.IndexOfKey(stock.deal.buy_date);
                    //double key_price = tur.low;
                    double move_stop_price = stock.dmData.b200.Low;
                    double cur_stop_price = Math.Max(stock.deal.stop_price, move_stop_price);

                    if (stock.sdata[stock.runData.cur_date].low > stock.deal.buy_adj_price * 1.02)
                    {
                        if (stock.deal.buy_date != 0 && stock.sdata[stock.runData.cur_date].low < cur_stop_price)//stop
                        {
                            stock.deal.sell_date = stock.runData.cur_date;
                            stock.deal.sell_price = stock.deal.stop_price;
                            stock.deal.ESellReason = ESR.EPR_Stop;
                            Deal.BackTest_Sell(stock);
                        }
                    }

                    FinalDateCloseTrading(stock);
                }

            }
            return EST.EST_OK;
        }
        

    }
    */
}
