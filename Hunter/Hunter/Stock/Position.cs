using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    [Serializable]
    public class Position
    {
        //public Stock stock;
        public int shares;//当前持仓数
        public int shares_available;//当前可用的股票数
        //public double cur_price;//当前股价
        public double cost_price;//成本价格
        public double cost_money;//持仓成本
        //public double bonus;

        //public double totalShares() { return shares + shares_available; }//当前持仓数
        public double getMarketValue(double close)//当前市值
        {
            return Math.Round(shares * close, 2);
        }
        public double getCostPrice()
        {
            if (shares == 0)
            {
                cost_price = 0;
            }
            else
            {
                cost_price = Math.Round(cost_money / shares, 2);
            }
            return cost_price;
        }

        public Position()//Stock _stock
        {
            //stock = _stock;
        }
        
        public int AddDeal(Deal deal)
        {
            if (deal.eType == Deal.EBuy)
            {
                shares += deal.buy_shares;
                //cur_price = deal.buy_price;
                cost_money += deal.buy_total_money;
            }
            else if (deal.eType == Deal.ESell)
            {
                if (shares_available < deal.sell_shares)
                {
                    Utility.Assert("shares_available < deal.sell_shares");
                    return EST.EST_ERROR;
                }
                shares -= deal.sell_shares;
                shares_available -= deal.sell_shares;
                //cur_price = deal.sell_price;
                cost_money -= deal.sell_total_money;
            }
            cost_money = Math.Round(cost_money, 2);
            return EST.EST_OK;
        }

        public void Clear()
        {
            shares = 0;
            shares_available = 0;
            //cur_price = 0;
            cost_price = 0;
            cost_money = 0;
        }

        public void ProcessDay()
        {
            shares_available = shares;
            //getMarketValue();
            getCostPrice();
        }
        
        public void ExActivity(ActivityData activity)
        {
            double cur_bonus = 0;
            if (shares != 0)
            {
                if (activity.bonus > 0)
                {
                    cur_bonus = shares * activity.bonus;
                    TradingStrategy.Cash += cur_bonus;
                    cost_money -= cur_bonus;
                }
                if (activity.multiply_shares > 0)
                {
                    //sell_price = Utility.Round2(stop_price / activity.multiply_shares);
                    //stop_price = Utility.Round2(stop_price / activity.multiply_shares);
                    shares = Convert.ToInt32(shares * activity.multiply_shares);
                }
            }
        }
    }
}
