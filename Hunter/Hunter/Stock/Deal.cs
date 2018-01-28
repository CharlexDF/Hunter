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
    public class Deal
    {
        //type
        public const int ENone = 0;
        public const int EBuy = 1;
        public const int ESell = 2;

        //public
        public int eType;
        public Stock stock;
        public Position copy_position;
        
        //type:buy
        public DateTime buy_dt;
        public double buy_price;
        public int buy_shares;
        public double buy_money;
        public double buy_poundage;//手续费
        public double buy_total_money;

        //type:sell
        public DateTime sell_dt;
        public double sell_price;
        public int sell_shares;
        public double sell_money;
        public double sell_poundage;
        public double sell_total_money;
        public int ESellReason;

        //type:position
        /*
        public int shares;
        
        public double stop_price;
        public double returns;
        */
        public int shares_available;
        public double cur_price;
        public double cost_price;
        public double cur_money;
        public double cost_money;
        public double bonus;
        //useless variable
        public double sell_low;
        public int buy_date;
        public int sell_date;
        public AnalyzeData analyzData;

        public Deal(Stock _stock = null, int type = 0)
        {
            analyzData = new AnalyzeData();
            stock = _stock;
            eType = type;

            buy_date = 0;
            buy_price = 0;
            buy_shares = 0;
            buy_money = 0;
            buy_poundage = 0;
            buy_total_money = 0;

            sell_date = 0;
            sell_price = 0;
            sell_low = 0;
            sell_shares = 0;
            sell_money = 0;
            sell_poundage = 0;
            sell_total_money = 0;
            
            shares_available = 0;
            bonus = 0;
            ESellReason = ESR.EPR_None;
        }

        public Deal(Deal other)
        {
            stock = other.stock;
            analyzData = new AnalyzeData(other.analyzData);

            buy_date = other.buy_date;
            buy_price = other.buy_price;
            buy_shares = other.buy_shares;
            buy_money = other.buy_money;
            buy_poundage = other.buy_poundage;
            buy_total_money = other.buy_total_money;

            sell_date = other.sell_date;
            sell_price = other.sell_price;
            sell_low = other.sell_low;
            sell_money = other.sell_money;
            sell_poundage = other.sell_poundage;
            sell_total_money = other.sell_total_money;
            
            bonus = other.bonus;
            ESellReason = other.ESellReason;
        }
        
        public void Clear()
        {
            stock = null;

            buy_date = 0;
            buy_price = 0;
            buy_shares = 0;
            buy_money = 0;
            buy_poundage = 0;

            sell_date = 0;
            sell_price = 0;
            sell_low = 0;
            sell_money = 0;
            sell_poundage = 0;
            
            bonus = 0;
            ESellReason = ESR.EPR_None;
        }
        
        public double getBuyPoundage()
        {
            double transfer_fee = 0;
            double commission = 0;

            if (buy_shares == 0 || buy_money == 0)
            {
                Utility.Log("!!!!!!!!!");
            }
            Debug.Assert(stock != null);
            Debug.Assert(buy_shares != 0);
            Debug.Assert(buy_money != 0);

            if (stock.region == "sh")
            {
                transfer_fee = buy_shares * Config.TransferFeeRate;
                //transfer_fee = transfer_fee < 1 ? 1 : transfer_fee;
            }
            commission = buy_money * Config.CommissionRate;
            commission = commission < 5 ? 5 : commission;

            buy_poundage = Math.Round(transfer_fee + commission, 2);
            return buy_poundage;
        }
        public double calcBuyTotalMoney(double _buy_price)
        {
            //buy_price = _buy_price + Config.Slippage;
            buy_money = Utility.Round2(buy_price * buy_shares);
            buy_poundage = getBuyPoundage();
            buy_total_money = Utility.Round2(buy_money + buy_poundage);
            return buy_total_money;
        }

        public double getSellPoundage()
        {
            double stamp_duty = 0;
            double transfer_fee = 0;
            double commission = 0;

            Debug.Assert(stock != null);
            Debug.Assert(sell_shares != 0);
            //Debug.Assert(sold_volume != 0);
            if (sell_money == 0)
            {
                sell_poundage = 0;
                return sell_poundage;
            }
            
            stamp_duty = sell_money * Config.StampDutyRate;
            if (stock.region == "sh")
            {
                transfer_fee = sell_shares * Config.TransferFeeRate;
                //transfer_fee = transfer_fee < 1 ? 1 : transfer_fee;
            }
            commission = sell_money * Config.CommissionRate;
            commission = commission < 5 ? 5 : commission;

            sell_poundage = Math.Round(stamp_duty + transfer_fee + commission, 2);
            return sell_poundage;
        }
        public double calcSellTotalMoney(double _sell_price)
        {
            sell_price = _sell_price;
            sell_money = Utility.Round2(sell_shares * sell_price);
            sell_poundage = getSellPoundage();
            sell_total_money = Utility.Round2(sell_money - sell_poundage + bonus);
            return sell_total_money;
        }

        #region/* position control */
        public bool canAfford()
        {            
            return calcBuyTotalMoney(buy_price) <= TradingStrategy.Cash;
        }
        public int getSellShares()
        {
            sell_shares = stock.position.shares_available;
            return sell_shares;
        }
        public int getBuyShares(double money)
        {
            //money = TradingStrategy.GetCurCapital() * 0.01 * 0.01
            /*
            if (stock.position.shares > 0)
            {
                buy_shares = 0;
                return 0;
            }
            */
            buy_shares = Convert.ToInt32(Math.Floor(money * 0.01 / buy_price) * 100);
            while (calcBuyTotalMoney(buy_price) > TradingStrategy.Cash)
            {
                buy_shares -= 100;
                if (buy_shares <= 0)
                {
                    buy_shares = 0;
                    return 0;
                }
            }
            return buy_shares;
        }
        #endregion

        /* trading */
        public int backtest_buy(double price, int shares)
        {
            buy_price = price + Config.SlippagePrice*0;
            buy_shares = shares;
            calcBuyTotalMoney(buy_price);

            //TradingStrategy.AddCurCash(stock.deal.buy_total_money);

            return EST.EST_OK;
        }

        public int backtest_sell(double price, int shares, int eReason)
        {
            sell_price = price - Config.SlippagePrice;
            sell_shares = shares;
            ESellReason = eReason;
            calcSellTotalMoney(sell_price);
            
            //stock.dmData.ProcessSell(stock);
            /*
            TradingData tradingData = new TradingData();
            tradingData.SetData(sinfo);
            sinfo.TDList.Add(tradingData);
            */
            
            return EST.EST_OK;
        }

        

    }
    
}
