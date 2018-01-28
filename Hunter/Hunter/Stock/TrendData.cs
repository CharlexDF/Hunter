using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class ELevelType
    {
        public const int EDefault = -1;
        public const int ETick = 0;
        public const int E1m = 1;
        public const int E5m = 5;
        public const int E15m = 15;
        public const int E30m = 30;
        public const int E60m = 60;
        public const int EDay = 60 * 4;
        public const int EWeek = 60 * 4 * 5;
        public const int EMonth = -2;
        public const int EYear = -3;
    }
    [Serializable]
    public class TrendData
    {
        public int type;//controller
        public int eLevel;

        public Stock stock;

        public TickData mTickData;
        public PartData mPartData;
        public StrategyData mStrategyData;
        public List<PartData> lsPartData;
        public List<StrategyData> lsStrategyData;

        public DateTime dt;

        public TrendData(Stock _stock, int _eLevel)
        {
            stock = _stock;
            eLevel = _eLevel;
            mStrategyData = new StrategyData();
            lsPartData = new List<PartData>();
            lsStrategyData = new List<StrategyData>();
        }
        public bool AddTrendData(TrendData trendData)
        {
            return true;
        }
        public bool AddTickData(TickData tickData)
        {
            if (tickData.dt.Hour == 9 && tickData.dt.Minute == 25)
            {
                return true;
            }
            if (lsPartData == null)
            {
                lsPartData = new List<PartData>();
            }

            bool bNew = false;
            if (lsPartData.Count == 0)
            {
                bNew = true;
            }
            else
            {
                if (eLevel == ELevelType.EMonth)
                {
                    if (lsPartData[lsPartData.Count - 1].dt.Year < tickData.dt.Year)
                    {
                        bNew = true;
                    }
                    else if (lsPartData[lsPartData.Count - 1].dt.Month < tickData.dt.Month)
                    {
                        bNew = true;
                    }
                }
                else
                {
                    if (lsPartData[lsPartData.Count - 1].dt.AddMinutes(eLevel) < tickData.dt)
                    {
                        bNew = true;
                    }
                }
            }

            if (bNew)
            {
                PartData partData = new PartData(eLevel, tickData.dt);
                partData.AddData(tickData);
                lsPartData.Add(partData);
                if (lsPartData.Count > 205)
                {
                    lsPartData.RemoveAt(0);
                }
                mPartData = lsPartData[lsPartData.Count - 1];

                StrategyData newStrategyData = new StrategyData();
                lsStrategyData.Add(mStrategyData);
                if (lsStrategyData.Count > 5)
                {
                    lsStrategyData.RemoveAt(0);
                }
                mStrategyData = new StrategyData();
            }
            else
            {
                lsPartData[lsPartData.Count - 1].AddData(tickData);
            }

            return true;
        }
        public bool AddDayData(DayData _data)
        {
            /*
            if (iDataCount == CountNum)
            {
                return false;
            }
            if (CountNum == 1)
            {
                data = _data;
                iDataCount = 1;
            }
            else
            {
                lsTPoint.Add(_data);
                iDataCount = lsTPoint.Count;
                if (lsTPoint.Count == 1)
                {
                    data = Utility.Clone(lsTPoint[0]);
                }
                else
                {
                    if (data.close != lsTPoint[0].close)
                    {
                        data = Utility.Clone(lsTPoint[0]);
                        foreach (var it in lsTPoint)
                        {
                            if (data.high < it.high)
                            {
                                data.high = it.high;
                            }
                            if (data.low > it.low)
                            {
                                data.low = it.low;
                            }
                        }
                    }
                    else
                    {
                        if (data.high < lsTPoint[iDataCount].high)
                        {
                            data.high = lsTPoint[iDataCount].high;
                        }
                        if (data.low > lsTPoint[iDataCount].low)
                        {
                            data.low = lsTPoint[iDataCount].low;
                        }
                    }
                    data.close = lsTPoint[iDataCount].close;
                    data.volume += lsTPoint[iDataCount].volume;
                }
            }
            */
            return true;
        }

        public int PreProcessData(TickData tickData)
        {
            mTickData = tickData;
            return EST.EST_OK;
        }
        public int ProcessData(TickData tickData)
        {
            AddTickData(tickData);
            mStrategyData.ProcessData(this);
            return EST.EST_OK;
        }

        public bool CalcTrend()
        {
            //mStrategyData = new StrategyData();
            return true;
        }
        public PartData getPartData()
        {
            return lsPartData[lsPartData.Count - 1];
        }
        public StrategyData getStrategyData()
        {
            return lsStrategyData[lsStrategyData.Count - 1];
        }

        public double calcBuyPrice(TickData tickData)
        {
            if (lsStrategyData.Count < 3) return 0;
            StrategyData strategyData = lsStrategyData[lsStrategyData.Count - 2];
            if (strategyData.ma30.ma == 0) return 0;

            if (lsPartData[lsPartData.Count - 2].close >= strategyData.ma30.ma) return 0;
            if (tickData.price <= strategyData.ma30.ma) return 0;

            double move_buy_price = tickData.price;
            return Utility.Round2(move_buy_price);
        }
        public double calcSellPrice(TickData tickData)
        {
            //if (eOperate != EOperateState.Sell) return 0;

            if (stock.position.shares == 0) { return 0; }

            if (lsStrategyData.Count < 3) return 0;
            StrategyData strategyData = lsStrategyData[lsStrategyData.Count - 2];
            if (strategyData.ma30.ma == 0) return 0;

            if (tickData.price >= strategyData.ma30.ma) return 0;

            //double move_sell_price = trend.lsPartData[trend.lsPartData.Count - 1].close;
            double move_sell_price = tickData.price;
            return Utility.Round2(move_sell_price);
        }

        public void ExActivity(ActivityData activity)
        {
            foreach (var it in lsPartData)
            {
                it.ExActivity(activity);
            }
        }
    }
}
