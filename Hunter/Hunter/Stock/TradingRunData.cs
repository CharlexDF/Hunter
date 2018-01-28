using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class TradingRunData
    {
        public int start_date;
        public int end_date;
        public int cur_date;

        //assets
        public double cash;
        public double shares_value;

        public double principal_start;
        public double principal_cur;
        public double total_capital;

        public TradingRunData()
        {
            //back test
            start_date = 0;
            end_date = 0;
            cur_date = 0;

            //deal
            principal_start = 0;
            principal_cur = 0;
            total_capital = 0;
        }

        public TradingRunData(TradingRunData other)
        {
            //back test
            start_date = other.start_date;
            end_date = other.end_date;
            cur_date = other.cur_date;

            //deal
            principal_start = other.principal_start;
            principal_cur = other.principal_cur;
            total_capital = other.total_capital;
        }

        public void setPrincipal(double _principal)
        {
            principal_start = _principal;
            principal_cur = principal_start;
            total_capital = principal_start;
        }

        //public double getTotalCapital()
        //{
        //    double TotalStockAssets = 0;
        //    int iDate = 0;

        //    foreach (KeyValuePair<int, Stock> it in CurDealList)
        //    {
        //        iDate = cur_date;
        //        while (!it.Value.sdata.ContainsKey(iDate))
        //        {
        //            iDate = Utility.AddDays(iDate, -1);
        //        }
        //        TotalStockAssets += it.Value.tdata.deal.GetSellTotalMoney(it.Value.sdata[iDate].close);
        //    }
        //    total_capital = principal_cur + TotalStockAssets;
        //    return total_capital;
        //}

    }
}
