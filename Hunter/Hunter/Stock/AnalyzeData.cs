using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class AnalyzeData
    {
        //RiskIndicator
        public int count_days;
        public double max_drawdown;
        public double max_climb;
        public double total_returns;

        public int count;
        public double average_days;
        public double average_returns;
        public double annual_returns;

        //public List<double> returns_list;
        //public double volatility;
        public AnalyzeData()
        {
            count_days = 0;
            max_drawdown = 100;
            max_climb = 100;
            total_returns = 0;

            count = 0;
            average_days = 0;
            average_returns = 0;
            annual_returns = 0;
            //returns_list = new List<double>();
            //volatility = 0;
        }
        public AnalyzeData(AnalyzeData other)
        {
            count_days = other.count_days;
            max_drawdown = other.max_drawdown;
            max_climb = other.max_climb;
            total_returns = other.total_returns;

            count = other.count;
            average_days = other.average_days;
            average_returns = other.average_returns;
            annual_returns = other.annual_returns;
            //returns_list = new List<double>();
            //volatility = 0;
        }

        public int Calc(Stock stock)
        {
            /*
            if (stock.position.shares == 0)
            {
                return EST.EST_SKIP;
            }

            count_days++;
            int index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            List<int> ls_key = new List<int>(stock.sdata.Keys);
            int last_date = ls_key[index - 1];

            if (last_date == stock.position.buy_date)
            {
                max_climb = 100;
                max_drawdown = 100;
                stock.position.calcSellTotalMoney(stock.sdata[last_date].close);
                total_returns = 100;
                return EST.EST_OK;
            }
            
            stock.position.calcSellTotalMoney(stock.sdata[last_date].high);
            stock.position.calcReturns();
            if (stock.position.returns > max_climb)
            {
                max_climb = stock.position.returns;
            }

            stock.position.calcSellTotalMoney(stock.sdata[last_date].low);
            stock.position.calcReturns();
            if (stock.position.returns < max_drawdown)
            {
                max_drawdown = stock.position.returns;
            }

            stock.position.calcSellTotalMoney(stock.sdata[last_date].close);
            total_returns = stock.position.calcReturns();
            */
            return EST.EST_OK;
        }

        public int Sell(Stock stock)
        {
            /*
            Debug.Assert(stock.position.sell_date != 0);
            Debug.Assert(stock.position.sell_price != 0);

            stock.position.calcSellTotalMoney(stock.position.sell_price);
            total_returns = stock.position.returns;
            average_returns = stock.position.returns;
            if (stock.position.returns > max_climb)
            {
                max_climb = stock.position.returns;
            }
            if (stock.position.returns < max_drawdown)
            {
                max_drawdown = stock.position.returns;
            }

            if (count_days != 0)
            {
                annual_returns = Utility.Round2((total_returns - 100) / count_days * 250 + 100);
            }
            else
            {
                max_drawdown = 0;
                max_climb = 0;
                annual_returns = 0;
            }
            */
            return EST.EST_OK;
        }

        public static int SumData(ref AnalyzeData sumData, ref List<AnalyzeData> analyzData_List)
        {
            sumData.count = analyzData_List.Count;
            for (int i = 0; i < analyzData_List.Count; i++)
            {
                Debug.Assert(analyzData_List[i].count_days != 0);
                sumData.count_days += analyzData_List[i].count_days;
                if (sumData.max_drawdown > analyzData_List[i].max_drawdown)
                {
                    sumData.max_drawdown = analyzData_List[i].max_drawdown;
                }
                if (sumData.max_climb < analyzData_List[i].max_climb)
                {
                    sumData.max_climb = analyzData_List[i].max_climb;
                }
                sumData.total_returns += analyzData_List[i].total_returns - 100;
            }

            if (sumData.count == 0)
            {
                sumData.max_drawdown = 0;
                sumData.max_climb = 0;
                sumData.total_returns = 0;
            }
            else
            {
                sumData.count_days = sumData.count_days / sumData.count;
                sumData.total_returns = Utility.Round2(sumData.total_returns);
                sumData.average_returns = Utility.Round2(sumData.total_returns / sumData.count + 100);
                //sumData.annual_returns = Utility.Round2(sumData.total_returns / sumData.count_days * 250 + 100);
            }

            //sumData.total_returns += 100;

            return EST.EST_OK;
        }

        //public double GetVolatility()
        //{
        //    double sum = 0;
        //    double average = 0;

        //    for (int i = 0; i < returns_list.Count; i++)
        //    {
        //        sum += returns_list[i];
        //    }
        //    average = sum / returns_list.Count;
        //
        //    sum = 0;
        //    for (int i = 0; i < returns_list.Count; i++)
        //    {
        //        sum += Math.Pow(returns_list[i] - average, 2);
        //    }
        //    volatility = Math.Sqrt(sum / returns_list.Count);

        //    return volatility;
        //}
    }

}
