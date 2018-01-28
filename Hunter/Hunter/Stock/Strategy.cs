using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class Strategy
    {
        //strategy
        public const int EMacd = 1;
        public const int EGrid = 2;
        public int mEStrategy;
        public Grid grid;
        public MaBreak maBreak;

        public Strategy()
        {
            mEStrategy = EGrid;
            grid = new Grid();
            maBreak = new MaBreak();
        }

        public Deal CalcDeal(Stock stock, TickData tickData)
        {
            Deal deal = new Deal(stock, Deal.ENone);

            if (mEStrategy == EMacd)
            {
                maBreak.CalcDeal(stock, tickData, deal);
            }
            else if (mEStrategy == EGrid)
            {
                grid.CalcDeal(stock, tickData, deal);
            }

            if (deal.eType == Deal.EBuy)
            {
                if (deal.buy_price <= 0 || deal.buy_shares <= 0)
                {
                    Utility.Assert("Wrong Deal! deal.buy_price <= 0 || deal.buy_shares <= 0");
                }
            }
            else if (deal.eType == Deal.ESell)
            {
                if (deal.sell_price <= 0 || deal.sell_shares <= 0)
                {
                    Utility.Assert("Wrong Deal! deal.sell_price <= 0 || deal.sell_shares <= 0");
                }
                if (deal.sell_shares > stock.position.shares_available)
                {
                    Utility.Assert("Wrong Deal! deal.sell_shares > stock.position.shares_available");
                }
            }
            return deal;
        }

        public void ProcessDay()
        {
            if (mEStrategy == EMacd)
            {
            }
            else if (mEStrategy == EGrid)
            {
                grid.ProcessDay();
            }
        }

        public void ProcessDeal(Stock stock, Deal deal)
        {
            if (mEStrategy == EMacd)
            {
                //if (trend.lsPartData.Count < 10) return 0;
                //if (tickData.price <= ma30.ma) return 0;
            }
            else if (mEStrategy == EGrid)
            {
                grid.ProcessDeal(stock, deal);
            }
        }
    }

    //break strategy
    public class MaBreak
    {
        public const int StartShares = 1000;
        public void CalcDeal(Stock stock, TickData tickData, Deal deal)
        {
            TrendData trend = stock.trend_30m;
            StrategyData stData = trend.mStrategyData;

            if (stock.position.shares == 0)
            {
                //buy
                if (stData.ma30.ma == 0) return;
                if (tickData.dt.Hour < 13)
                {
                    return;
                }
                if (trend.mPartData.close < stData.ma30.ma
                    && tickData.price >= stData.ma30.ma)
                {
                    deal.buy_shares = StartShares;
                    deal.buy_price = tickData.price;
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
            else
            {
                if (stock.position.shares_available != stock.position.shares) return;

                //sell
                if (tickData.price < stData.ma30.ma)
                {
                    deal.sell_shares = stock.position.shares;
                    deal.sell_price = tickData.price;
                    deal.eType = Deal.ESell;
                }
            }
        }
    }

    //reversed strategy
    public class Grid
    {
        public const double EarnRate = 0.01;
        public const double PriceRate = 0.05;
        public const double SellPriceRate = 1 + PriceRate;
        public const double BuyPriceRate = 1 - PriceRate;
        public const int SharesRate = 2;
        public const int TopTimes = 5;
        public const int StartShares = 200;
        public const int StartMoney = 5000;
        public int times;
        public double start_price;
        public double sellPrice1;
        public double highPrice;
        public double openPrice;
        public Position lastPosition;
        public Position cur_Position;

        public Grid()
        {
            times = 0;
            start_price = 0;
            lastPosition = new Position();
            cur_Position = new Position();
        }

        public void ExActivity(ActivityData activity)
        {
            double cur_bonus = 0;
            if (times == 0) return;
            if (activity.bonus > 0)
            {
                sellPrice1 -= cur_bonus;
            }
            if (activity.multiply_shares > 0)
            {
                sellPrice1 = Utility.Round2(sellPrice1 / activity.multiply_shares);
            }

            lastPosition.ExActivity(activity);
            cur_Position.ExActivity(activity);
        }

        public void CalcDeal(Stock stock, TickData tickData, Deal deal)
        {
            CalcDeal_MACD(stock, tickData, deal);
        }
        
        public void CalcDeal_Multi(Stock stock, TickData tickData, Deal deal)
        {
            TrendData trend = stock.trend_30m;
            StrategyData strategyData = trend.mStrategyData;

            if (times == 0)
            {
                if (strategyData.ma10.ma == 0) return;
                if (trend.mPartData.close < strategyData.ma10.ma
                    && tickData.price > strategyData.ma10.ma)
                {
                    deal.buy_shares = StartShares;
                    deal.buy_price = tickData.price;
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
            else if (times > 0)
            {
                if (tickData.price > stock.position.getCostPrice() * SellPriceRate)
                {
                    if (stock.position.shares_available != stock.position.shares)
                    {
                        return;
                    }
                    deal.sell_shares = stock.position.shares;
                    deal.sell_price = tickData.price;
                    //if (deal.sell_shares > stock.position.shares_available) return;
                    deal.eType = Deal.ESell;
                }
                else if (tickData.price < stock.position.getCostPrice() * BuyPriceRate)
                {
                    //EarnRate = 1.01;
                    //PriceRate = 0.02;
                    //SellPriceRate = 1 + PriceRate;
                    //bPrice2 = tickData.price;
                    //SellPrice = bPrice2 * SellPriceRate;

                    //CostMoney * EarnRate = SellMoney;
                    //(bPrice1*bShares1 + bPrice2*bShares2)*EarnRate = SellPrice*TotalShares;
                    //Mid = SellPrice / EarnRate;
                    //bPrice1*bShares1 + bPrice2*bShares2 = Mid*bShares1 + Mid*bShares2;
                    //bShares2*(bPrice2 - Mid) = bShares1*(Mid - bPrice1);
                    //bShares2 = bShares1*(Mid - bPrice1) / (bPrice2 - Mid);
                    
                    if (times == TopTimes) { return; }
                    deal.buy_price = tickData.price;
                    double bPrice1 = stock.position.getCostPrice();
                    double bPrice2 = deal.buy_price;
                    double sellPrice = bPrice2 * SellPriceRate;
                    double mid = sellPrice / (1 + EarnRate);
                    double denominator = mid - stock.position.getCostPrice();
                    double numerator = deal.buy_price - mid;
                    double x = denominator / numerator * stock.position.shares;
                    x = Math.Ceiling(x / 100) * 100;
                    deal.buy_shares = Convert.ToInt32(x);
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
        }

        public void CalcDeal_Add(Stock stock, TickData tickData, Deal deal)
        {
            TrendData trend = stock.trend_30m;
            StrategyData strategyData = trend.mStrategyData;

            if (times == 0)
            {
                if (strategyData.ma10.ma == 0) return;
                if (trend.mPartData.close > strategyData.ma10.ma
                    && tickData.price < strategyData.ma10.ma)
                {
                    deal.buy_shares = StartShares;
                    deal.buy_price = tickData.price;
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
            else if (times > 0)
            {
                if (tickData.price > stock.position.getCostPrice() * SellPriceRate)
                {
                    if (stock.position.shares_available != stock.position.shares)
                    {
                        return;
                    }
                    deal.sell_shares = stock.position.shares;
                    deal.sell_price = tickData.price;
                    //if (deal.sell_shares > stock.position.shares_available) return;
                    deal.eType = Deal.ESell;
                }
                else if (tickData.price < stock.position.getCostPrice() * BuyPriceRate)
                {
                    //EarnMoney = sMoney1 - bMoney1 = sMoney2 - bMoney1 - bMoney2;
                    //sMoney1 = sMoney2 - bMoney2;
                    //sPrice1*bShares1 = sPrice2*(bShares1+bShares2) - bPrice2*bShares2;
                    //sPrice1 = bPrice1*SellPriceRate;
                    //bPrice2 = bPrice1*BuyPriceRate;
                    //(sPrice2-bPrice2)*bShares2 = (sPrice1-sPrice2)*bShares1
                    //bShares2 = bShares1 * (sPrice1-sPrice2) / (sPrice2-bPrice2)
                    
                    //if (times == 10) { return; }
                    deal.buy_price = tickData.price;
                    double bPrice1 = stock.position.getCostPrice();
                    double bPrice2 = deal.buy_price;
                    double sPrice1 = sellPrice1;
                    double sPrice2 = bPrice2 * SellPriceRate;
                    double bShares1 = stock.position.shares;

                    double denominator = sPrice1 - sPrice2;
                    double numerator = sPrice2 - bPrice2;
                    double x = denominator / numerator * bShares1;
                    x = Math.Ceiling(x / 100) * 100;
                    deal.buy_shares = Convert.ToInt32(x);
                    if (deal.buy_shares == 0) return;
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
        }

        public void CalcDeal_MACD(Stock stock, TickData tickData, Deal deal)
        {
            TrendData trend = stock.trend_day;
            StrategyData strategyData = trend.mStrategyData;

            if (times == 0)
            {
                if (strategyData.ma10.ma == 0) return;
                if (trend.mPartData.close < strategyData.ma10.ma
                    && tickData.price > strategyData.ma10.ma)
                {
                    deal.buy_price = tickData.price;
                    deal.buy_shares = deal.getBuyShares(StartMoney);
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
            else if (times > 0)
            {
                double sPrice = stock.position.getCostPrice() * SellPriceRate;
                double bPrice = stock.position.getCostPrice() * BuyPriceRate;
                if (tickData.price > stock.position.getCostPrice() * SellPriceRate)
                {
                    if (stock.position.shares_available != stock.position.shares)
                    {
                        return;
                    }
                    if (tickData.price > strategyData.ma10.ma)
                    {
                        return;
                    }
                    deal.sell_shares = stock.position.shares;
                    deal.sell_price = tickData.price;
                    //if (deal.sell_shares > stock.position.shares_available) return;
                    deal.eType = Deal.ESell;
                }
                else if (tickData.price < stock.position.getCostPrice() * BuyPriceRate)
                {
                    //if (times == 10) { return; }
                    deal.buy_price = tickData.price;
                    deal.buy_shares = deal.getBuyShares(StartMoney);
                    if (deal.buy_shares == 0) return;
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
        }

        public double CountSellPrice_HF()
        {
            double diff = highPrice - openPrice;
            if (diff < 0.03)
            {
                return 0;
            }

            return 0;
        }
        public void CalcDeal_HighFrequency(Stock stock, TickData tickData, Deal deal)
        {
            TrendData trend = stock.trend_day;
            StrategyData strategyData = trend.mStrategyData;

            //sell
            if (stock.position.shares_available > 0)
            {
                if (openPrice == 0)
                {
                    openPrice = tickData.price;
                }
                if (tickData.price > highPrice)
                {
                    highPrice = tickData.price;
                }

                double costPrice = stock.position.getCostPrice();
                double diff = highPrice - costPrice;
                if (diff >= 0.03)
                {
                    double sPrice = openPrice + Math.Floor(diff / 1.6 * 100) / 100;
                    if (tickData.price < sPrice)
                    {
                        deal.sell_shares = stock.position.shares_available;
                        deal.sell_price = tickData.price;
                        deal.eType = Deal.ESell;
                    }
                }

                //final wall
                /*
                DateTime endDt = new DateTime(tickData.dt.Year, tickData.dt.Month, tickData.dt.Day, 14, 50, 0);
                if (tickData.dt.Ticks > endDt.Ticks)
                {
                    deal.sell_shares = stock.position.shares_available;
                    deal.sell_price = tickData.price;
                    deal.eType = Deal.ESell;
                }
                */
            }

            //buy
            if (stock.position.shares == 0)
            {
                if (tickData.price <= strategyData.ma5.low)
                {
                    deal.buy_price = tickData.price;
                    deal.buy_shares = deal.getBuyShares(StartMoney*10);
                    if (deal.buy_shares == 0) return;
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
            
            
        }

        public void ProcessDay()
        {
            highPrice = 0;
            openPrice = 0;
            cur_Position.Clear();
        }

        public void ProcessDeal(Stock stock, Deal deal)
        {
            if (deal.eType == Deal.EBuy)
            {
                times++;
                sellPrice1 = deal.buy_price * SellPriceRate;
                //start_price = deal.buy_price;
                cur_Position.AddDeal(deal);
            }
            else if (deal.eType == Deal.ESell)
            {
                times = 0;
                sellPrice1 = 0;
                //start_price = 0;
            }
        }

    }

    public class Robbery
    {
        public const int StartShares = 1000;
        public void CalcDeal(Stock stock, TickData tickData, Deal deal)
        {
            TrendData trend = stock.trend_30m;
            StrategyData stData = trend.mStrategyData;

            if (stock.position.shares == 0)
            {
                //buy
                if (stData.ma30.ma == 0) return;
                if (trend.mPartData.close < stData.ma30.ma
                    && tickData.price >= stData.ma30.ma)
                {
                    deal.buy_shares = StartShares;
                    deal.buy_price = tickData.price;
                    if (!deal.canAfford()) return;
                    deal.eType = Deal.EBuy;
                }
            }
            else
            {
                if (stock.position.shares_available != stock.position.shares) return;

                //sell
                if (tickData.price < stData.ma30.ma)
                {
                    deal.sell_shares = stock.position.shares;
                    deal.sell_price = tickData.price;
                    deal.eType = Deal.ESell;
                }
            }
        }
    }
}
