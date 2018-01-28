using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class BasicData
    {
        public int CountNum;
        public BasicData(int _CountNum) { CountNum = _CountNum; }
    }

    public class Excursion
    {
        public int CountNum;

        public double max;
        public double min;
        public double eur;

        public Excursion(int _CountNum)
        {
            CountNum = _CountNum;

            max = 0;
            min = 0;
            eur = 0;
        }

        public Excursion(Excursion other)
        {
            CountNum = other.CountNum;

            max = other.max;
            min = other.min;
            eur = other.eur;
        }


        public Excursion Calc(Stock stock)
        {
            double MFE = 0;
            double MAE = 0;

            List<int> ls_key = new List<int>(stock.sdata.Keys);
            int index = 0;

            index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index + CountNum > stock.sdata.Count)
            {
                return this;
            }

            max = stock.sdata[stock.runData.cur_date].close;
            min = stock.sdata[stock.runData.cur_date].close;
            for (int i = index + CountNum - 1; i >= index; i--)
            {
                if (stock.sdata[ls_key[i]].high > max)
                {
                    max = stock.sdata[ls_key[i]].high;
                }
                if (stock.sdata[ls_key[i]].low < min)
                {
                    min = stock.sdata[ls_key[i]].low;
                }
            }

            MFE = max - stock.sdata[stock.runData.cur_date].close;
            MAE = stock.sdata[stock.runData.cur_date].close - min;

            if (MFE == 0)
            {
                eur = -Utility.Round2(MAE);
            }
            else if (MAE == 0)
            {
                eur = 9;
            }
            else
            {
                eur = Utility.Round2(MFE / MAE);
                if (eur > 10)
                {
                    eur = 10;
                }
            }

            return this;
        }
    }

    public class Turtle : BasicData
    {
        public double high;
        public double low;
        public double PDC;
        public double N;
        public double TR;
        public double PDN;
        public int Unit;

        public Turtle(int _CountNum) : base(_CountNum)
        {
            high = 0;
            low = 0;
            PDC = 0;
            N = 0;
            TR = 0;
            PDN = 0;
            Unit = 0;
        }

        public Turtle(Turtle other) : base(other.CountNum)
        {
            high = other.high;
            low = other.low;
            PDC = other.PDC;
            N = other.N;
            TR = other.TR;
            PDN = other.PDN;
            Unit = other.Unit;
        }

        public int Calc(Stock stock)
        {
            double sum = 0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);

            int index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index < CountNum)
            {
                return EST.EST_SKIP;
            }

            high = stock.sdata[ls_key[index - CountNum]].high;
            low = stock.sdata[ls_key[index - CountNum]].low;
            for (int i = index - CountNum; i < index; i++)
            {
                if (stock.sdata[ls_key[i]].high > high)
                {
                    high = stock.sdata[ls_key[i]].high;
                }
                if (stock.sdata[ls_key[i]].low < low)
                {
                    low = stock.sdata[ls_key[i]].low;
                }
                sum += stock.sdata[ls_key[i]].close;
            }

            TR = Math.Max(Math.Max(stock.sdata[ls_key[index]].high - stock.sdata[ls_key[index]].low,
                                 stock.sdata[ls_key[index]].high - stock.sdata[ls_key[index - 1]].close),
                                 stock.sdata[ls_key[index - 1]].close - stock.sdata[ls_key[index]].low);

            if (PDN == 0)
            {
                PDN = TR;
            }

            N = ((CountNum - 1) * PDN + TR) / CountNum;

            PDN = N;
            if (N == 0)
            {
                //Debug.Assert(N != 0);
                Unit = int.MaxValue;
            }
            else
            {
                Unit = Convert.ToInt32(TradingStrategy.GetCurCash() * 0.01 * 0.01 / N) * 100;
            }
            //DollarPerPoint 100*0.01 = OneHand*BasePoint

            return EST.EST_OK;
        }
    }

    public class MACD : BasicData
    {
        public double ma;
        public double low;
        public double high;

        public MACD(int _CountNum) : base(_CountNum)
        {
            ma = 0;
            low = 0;
            high = 0;
        }

        public MACD(MACD other) : base(other.CountNum)
        {
            ma = other.ma;
            low = other.low;
            high = other.high;
        }

        public double Calc(Stock stock)
        {
            int index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index < CountNum)
            {
                return 0;
            }

            double sum_close = 0;
            double sum_low = 0;
            double sum_high = 0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);
            for (int i = index - CountNum; i < index; i++)
            {
                sum_close += stock.sdata[ls_key[i]].close;
                sum_low += stock.sdata[ls_key[i]].low;
                sum_high += stock.sdata[ls_key[i]].high;
            }
            ma = Utility.Round2(sum_close / CountNum);
            low = sum_low / CountNum;
            high = sum_high / CountNum;

            return ma;
        }
        public double Calc(TrendData trend)
        {
            int index = trend.lsPartData.Count;
            if (trend.lsPartData.Count < CountNum)
            {
                return 0;
            }

            double sum_close = 0;
            double sum_low = 0;
            double sum_high = 0;

            for (int i = index - CountNum; i < index; i++)
            {
                sum_close += trend.lsPartData[i].close;
                sum_low += trend.lsPartData[i].low;
                sum_high += trend.lsPartData[i].high;
            }
            ma = Utility.Round2(sum_close / CountNum);
            low = Utility.Round2(sum_low / CountNum);
            high = Utility.Round2(sum_high / CountNum);

            return ma;
        }
    }


    public class Linear : BasicData
    {
        public double a;
        public double b;
        public MACD macd;

        public Linear(int _CountNum) : base(_CountNum)
        {
            a = 0;
            b = 0;
            macd = new MACD(CountNum);
        }

        public Linear(Linear other) : base(other.CountNum)
        {
            a = other.a;
            b = other.b;
            macd = new MACD(other.macd);
        }

        public Linear Calc(Stock stock)
        {
            int idx = 0;
            double Molecule = 0;
            double Denominator = 0;

            List<int> ls_key = new List<int>(stock.sdata.Keys);
            int index = 0;
            index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index < CountNum)
            {
                return this;
            }

            double ay = macd.Calc(stock);
            double ax = (1.0 + CountNum) / 2;

            for (int x = 1; x <= CountNum; x++)
            {
                idx = index - CountNum + x - 1;
                Molecule += x * stock.sdata[ls_key[idx]].close;
                Denominator += x * x;
            }
            Molecule -= CountNum * ax * ay;
            Denominator -= CountNum * ax * ax;

            b = Math.Round(Molecule / Denominator, 4);
            a = Utility.Round2(ay - b * ax);

            return this;
        }
    }

    public class Distribution : BasicData
    {
        public static int TypeTotal = 0;
        public static int StartCountNum = 0;

        public static double StopLossRate = 0.95;
        public static double ResistRate = 0.05;
        public static double ResistRate_Sell = 0.05;

        public static double GetPriceRange(double price)
        {
            double range = 0;
            double multi = 0.02;
            if (price < 10)
            {
                range = 0.1;
            }
            else if (price >= 10 && price < 20)
            {
                range = 10 * multi;
            }
            else if (price >= 20 && price < 40)
            {
                range = 20 * multi;
            }
            else if (price >= 40 && price < 80)
            {
                range = 40 * multi;
            }
            else if (price >= 80 && price < 160)
            {
                range = 80 * multi;
            }
            else if (price >= 160 && price < 320)
            {
                range = 160 * multi;
            }
            else if (price >= 320)
            {
                range = 320 * multi;
            }
            return range;
        }
        public static double GetPricePoint(double price)
        {
            double range = GetPriceRange(price);
            double point = Math.Round(Math.Round(price / range) * range, 2);
            if (point < 0)
            {
                point = 0;
            }
            return point;
        }
        public static double GetHighPoint(double price)
        {
            double point = GetPricePoint(price) + GetPriceRange(price);
            return Utility.Round2(point);
        }
        
        public double High;
        public double Low;
        public double Mid;
        public double TotalVolume;
        public double AverageVolume;
        public double rank;
        public SortedList<double, double> SupplyDistribution;

        public Distribution(int _CountNum) : base(_CountNum)
        {
            High = 0;
            Low = 0;
            Mid = 0;
            TotalVolume = 0;
            AverageVolume = 0;
            SupplyDistribution = new SortedList<double, double>();
        }

        public Distribution(Box other) : base(other.CountNum)
        {
            High = other.High;
            Low = other.Low;
            Mid = other.Mid;
            TotalVolume = other.TotalVolume;
            AverageVolume = other.AverageVolume;
            SupplyDistribution = other.SupplyDistribution;
        }

        public void ExActivity(Stock stock)
        {
            int iCurDate = stock.runData.cur_date;
            if (!stock.activity.ContainsKey(iCurDate))
            {
                return;
            }

            if (Low != 0)
            {
                if (stock.activity[iCurDate].bonus > 0)
                {
                    Low -= Utility.Round2(stock.activity[iCurDate].bonus);
                }
                if (stock.activity[iCurDate].multiply_shares > 0)
                {
                    Low = Utility.Round2(Low / stock.activity[iCurDate].multiply_shares);
                }
            }

            double PricePoint = 0;
            List<double> ls_sdkey = null;
            if (SupplyDistribution.Count > 0)
            {
                if (stock.activity[iCurDate].bonus > 0)//分红
                {
                    SortedList<double, double> tmpSL = new SortedList<double, double>();
                    ls_sdkey = new List<double>(SupplyDistribution.Keys);
                    for (int j = 0; j < SupplyDistribution.Count; j++)
                    {
                        PricePoint = Box.GetPricePoint(Utility.Round2(ls_sdkey[j] - stock.activity[iCurDate].bonus));
                        if (tmpSL.ContainsKey(PricePoint))
                        {
                            tmpSL[PricePoint] += SupplyDistribution[ls_sdkey[j]];
                        }
                        else
                        {
                            tmpSL.Add(PricePoint, SupplyDistribution[ls_sdkey[j]]);
                        }
                    }
                    SupplyDistribution = tmpSL;
                }

                if (stock.activity[iCurDate].multiply_shares > 0)//拆股
                {
                    SortedList<double, double> tmpSL = new SortedList<double, double>();
                    ls_sdkey = new List<double>(SupplyDistribution.Keys);
                    for (int j = 0; j < SupplyDistribution.Count; j++)
                    {
                        PricePoint = Box.GetPricePoint(ls_sdkey[j] / stock.activity[iCurDate].multiply_shares);
                        if (tmpSL.ContainsKey(PricePoint))
                        {
                            tmpSL[PricePoint] += SupplyDistribution[ls_sdkey[j]] * stock.activity[iCurDate].multiply_shares;
                        }
                        else
                        {
                            tmpSL.Add(PricePoint, SupplyDistribution[ls_sdkey[j]] * stock.activity[iCurDate].multiply_shares);
                        }
                    }
                    SupplyDistribution = tmpSL;
                }
            }
        }

        public void EstimateSupplyer(double left)
        {
            int count = 0;
            int i = 0;
            double average = 0;
            List<double> ls_sdkey = null;

            while (left > 0)
            {
                ls_sdkey = new List<double>(SupplyDistribution.Keys);
                count = SupplyDistribution.Count;
                if (count == 0) return;

                average = Convert.ToInt64(Math.Round(left / count, 0) + 1);

                for (i = count - 1; i >= 0; i--)
                {
                    if (left <= 0) break;

                    if (left > average)
                    {
                        if (SupplyDistribution[ls_sdkey[i]] > average)
                        {
                            left -= average;
                            SupplyDistribution[ls_sdkey[i]] -= average;
                        }
                        else
                        {
                            left -= SupplyDistribution[ls_sdkey[i]];
                            SupplyDistribution.Remove(ls_sdkey[i]);
                        }
                    }
                    else
                    {
                        if (SupplyDistribution[ls_sdkey[i]] >= left)
                        {
                            SupplyDistribution[ls_sdkey[i]] -= left;
                            left = 0;
                        }
                        else
                        {
                            left -= SupplyDistribution[ls_sdkey[i]];
                            SupplyDistribution[ls_sdkey[i]] = 0;
                            SupplyDistribution.Remove(ls_sdkey[i]);
                        }
                    }
                }
            }
        }

        public int Calc(Stock stock)
        {
            ExActivity(stock);

            int index = 0;
            int iPreDate = 0;
            double PricePoint = 0;
            double per = 0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);

            index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index == 0) return EST.EST_SKIP;

            iPreDate = ls_key[index - 1];
            if (index < CountNum)
            {
                TotalVolume += stock.sdata[iPreDate].volume;
                PricePoint = GetPricePoint(stock.sdata[iPreDate].close);
                if (SupplyDistribution.ContainsKey(PricePoint))
                {
                    SupplyDistribution[PricePoint] += stock.sdata[iPreDate].volume;
                }
                else
                {
                    SupplyDistribution.Add(PricePoint, stock.sdata[iPreDate].volume);
                }
                return EST.EST_SKIP;
            }
            else
            {

                EstimateSupplyer(stock.sdata[iPreDate].volume);
                PricePoint = GetPricePoint(stock.sdata[iPreDate].close);
                if (SupplyDistribution.ContainsKey(PricePoint))
                {
                    SupplyDistribution[PricePoint] += stock.sdata[iPreDate].volume;
                }
                else
                {
                    SupplyDistribution.Add(PricePoint, stock.sdata[iPreDate].volume);
                }
            }

            ////////////////////////////////////////////////////////////
            Mid = 0;
            double sumPercent = 0;
            List<double> Percent = new List<double>();
            List<double> ls_sdkey = new List<double>(SupplyDistribution.Keys);

            TotalVolume = 0;
            foreach (var it in SupplyDistribution)
            {
                TotalVolume += it.Value;
            }
            AverageVolume = Utility.Round2(TotalVolume / (index + 1));

            for (int i = 0; i < SupplyDistribution.Count; i++)
            {
                per = Math.Round(SupplyDistribution[ls_sdkey[i]] / TotalVolume, 4);
                Percent.Add(per);
                sumPercent += per;
                if (Mid == 0 && sumPercent > 0.5)
                {
                    Mid = ls_sdkey[i];
                }
            }
            if (sumPercent < 0.95)
            {
                Utility.Log("Date:" + stock.runData.cur_date + " sumPercent = " + Utility.Round2(sumPercent));
            }

            ////////////////////////////////////////////////////////////
            //PricePoint = Stock.GetPricePoint(sdata[ls_key[index - 1]].close);
            High = 0;
            double ArbitragePrice = 0;
            double UpResistPercent = 0;
            double ResistancePrice = 0;
            //double CurHigh = 0;
            for (int i = 0; i < SupplyDistribution.Count; i++)
            {
                if (stock.sdata[iPreDate].close <= ls_sdkey[i] && Percent[i] > ResistRate)//
                {
                    ResistancePrice = ls_sdkey[i];// Utility.Round2(GetHighPoint(ls_sdkey[i]) * 1.00);
                    ArbitragePrice = Utility.Round2(ResistancePrice * 1.10);
                    UpResistPercent = 0;
                    for (int j = 0; j < Percent.Count; j++)
                    {
                        if (ls_sdkey[j] >= ResistancePrice && ls_sdkey[j] <= ArbitragePrice)
                        {
                            UpResistPercent += Percent[j];
                        }
                    }
                    if (UpResistPercent < 0.10)
                    {
                        High = ResistancePrice;
                    }
                    if (High > Mid * 0.95)
                    {
                        High = 0;
                    }
                    //High = GetHighPoint(ls_sdkey[i]);
                    break;
                }
            }

            ////////////////////////////////////////////////////////////
            //Low = 0;
            double move_low = 0;
            for (int i = 0; i < SupplyDistribution.Count; i++)
            {
                if (stock.sdata[ls_key[index - 1]].high >= ls_sdkey[i] && Percent[i] > 0.05)//
                {
                    move_low = ls_sdkey[i] * StopLossRate;
                }
            }
            if (move_low > Low)
            {
                Low = move_low;
            }

            return EST.EST_OK;
        }
    }

    public class Box : BasicData
    {
        public static int TypeTotal = 0;
        public static int StartCountNum = 0;

        public static double StopLossRate = 0.95;
        public static double ResistRate = 0.05;
        public static double ResistRate_Sell = 0.05;

        public static double GetPriceRange(double price)
        {
            double range = 0;
            double multi = 0.02;
            if (price < 10)
            {
                range = 0.1;
            }
            else if (price >= 10 && price < 20)
            {
                range = 10 * multi;
            }
            else if (price >= 20 && price < 40)
            {
                range = 20 * multi;
            }
            else if (price >= 40 && price < 80)
            {
                range = 40 * multi;
            }
            else if (price >= 80 && price < 160)
            {
                range = 80 * multi;
            }
            else if (price >= 160 && price < 320)
            {
                range = 160 * multi;
            }
            else if (price >= 320)
            {
                range = 320 * multi;
            }
            return range;
        }
        public static double GetPricePoint(double price)
        {
            double range = GetPriceRange(price);
            double point = Math.Round(Math.Round(price / range) * range, 2);
            if (point < 0)
            {
                point = 0;
            }
            return point;
        }
        public static double GetHighPoint(double price)
        {
            double point = GetPricePoint(price) + GetPriceRange(price);
            return Utility.Round2(point);
        }


        public double High;
        public double Low;
        public double Mid;
        public double TotalVolume;
        public double AverageVolume;
        public SortedList<double, double> SupplyDistribution;

        public Box(int _CountNum) : base(_CountNum)
        {
            High = 0;
            Low = 0;
            Mid = 0;
            TotalVolume = 0;
            AverageVolume = 0;
            SupplyDistribution = new SortedList<double, double>();
        }

        public Box(Box other) : base(other.CountNum)
        {
            High = other.High;
            Low = other.Low;
            Mid = other.Mid;
            TotalVolume = other.TotalVolume;
            AverageVolume = other.AverageVolume;
            SupplyDistribution = other.SupplyDistribution;
        }

        public void ExActivity(Stock stock)
        {
            int iCurDate = stock.runData.cur_date;
            if (!stock.activity.ContainsKey(iCurDate))
            {
                return;
            }

            if (Low != 0)
            {
                if (stock.activity[iCurDate].bonus > 0)
                {
                    Low -= Utility.Round2(stock.activity[iCurDate].bonus);
                }
                if (stock.activity[iCurDate].multiply_shares > 0)
                {
                    Low = Utility.Round2(Low / stock.activity[iCurDate].multiply_shares);
                }
            }

            double PricePoint = 0;
            List<double> ls_sdkey = null;
            if (SupplyDistribution.Count > 0)
            {
                if (stock.activity[iCurDate].bonus > 0)//分红
                {
                    SortedList<double, double> tmpSL = new SortedList<double, double>();
                    ls_sdkey = new List<double>(SupplyDistribution.Keys);
                    for (int j = 0; j < SupplyDistribution.Count; j++)
                    {
                        PricePoint = Box.GetPricePoint(Utility.Round2(ls_sdkey[j] - stock.activity[iCurDate].bonus));
                        if (tmpSL.ContainsKey(PricePoint))
                        {
                            tmpSL[PricePoint] += SupplyDistribution[ls_sdkey[j]];
                        }
                        else
                        {
                            tmpSL.Add(PricePoint, SupplyDistribution[ls_sdkey[j]]);
                        }
                    }
                    SupplyDistribution = tmpSL;
                }

                if (stock.activity[iCurDate].multiply_shares > 0)//拆股
                {
                    SortedList<double, double> tmpSL = new SortedList<double, double>();
                    ls_sdkey = new List<double>(SupplyDistribution.Keys);
                    for (int j = 0; j < SupplyDistribution.Count; j++)
                    {
                        PricePoint = Box.GetPricePoint(ls_sdkey[j] / stock.activity[iCurDate].multiply_shares);
                        if (tmpSL.ContainsKey(PricePoint))
                        {
                            tmpSL[PricePoint] += SupplyDistribution[ls_sdkey[j]] * stock.activity[iCurDate].multiply_shares;
                        }
                        else
                        {
                            tmpSL.Add(PricePoint, SupplyDistribution[ls_sdkey[j]] * stock.activity[iCurDate].multiply_shares);
                        }
                    }
                    SupplyDistribution = tmpSL;
                }
            }
        }

        public void EstimateSupplyer(double left)
        {
            int count = 0;
            int i = 0;
            double average = 0;
            List<double> ls_sdkey = null;

            while (left > 0)
            {
                ls_sdkey = new List<double>(SupplyDistribution.Keys);
                count = SupplyDistribution.Count;
                if (count == 0) return;

                average = Convert.ToInt64(Math.Round(left / count, 0) + 1);

                for (i = count - 1; i >= 0; i--)
                {
                    if (left <= 0) break;

                    if (left > average)
                    {
                        if (SupplyDistribution[ls_sdkey[i]] > average)
                        {
                            left -= average;
                            SupplyDistribution[ls_sdkey[i]] -= average;
                        }
                        else
                        {
                            left -= SupplyDistribution[ls_sdkey[i]];
                            SupplyDistribution.Remove(ls_sdkey[i]);
                        }
                    }
                    else
                    {
                        if (SupplyDistribution[ls_sdkey[i]] >= left)
                        {
                            SupplyDistribution[ls_sdkey[i]] -= left;
                            left = 0;
                        }
                        else
                        {
                            left -= SupplyDistribution[ls_sdkey[i]];
                            SupplyDistribution[ls_sdkey[i]] = 0;
                            SupplyDistribution.Remove(ls_sdkey[i]);
                        }
                    }
                }
            }
        }

        public int Calc(Stock stock)
        {
            ExActivity(stock);

            int index = 0;
            int iPreDate = 0;
            double PricePoint = 0;
            double per = 0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);

            index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index == 0) return EST.EST_SKIP;

            iPreDate = ls_key[index - 1];
            if (index < CountNum)
            {
                TotalVolume += stock.sdata[iPreDate].volume;
                PricePoint = GetPricePoint(stock.sdata[iPreDate].close);
                if (SupplyDistribution.ContainsKey(PricePoint))
                {
                    SupplyDistribution[PricePoint] += stock.sdata[iPreDate].volume;
                }
                else
                {
                    SupplyDistribution.Add(PricePoint, stock.sdata[iPreDate].volume);
                }
                return EST.EST_SKIP;
            }
            else
            {

                EstimateSupplyer(stock.sdata[iPreDate].volume);
                PricePoint = GetPricePoint(stock.sdata[iPreDate].close);
                if (SupplyDistribution.ContainsKey(PricePoint))
                {
                    SupplyDistribution[PricePoint] += stock.sdata[iPreDate].volume;
                }
                else
                {
                    SupplyDistribution.Add(PricePoint, stock.sdata[iPreDate].volume);
                }
            }

            ////////////////////////////////////////////////////////////
            Mid = 0;
            double sumPercent = 0;
            List<double> Percent = new List<double>();
            List<double> ls_sdkey = new List<double>(SupplyDistribution.Keys);

            TotalVolume = 0;
            foreach (var it in SupplyDistribution)
            {
                TotalVolume += it.Value;
            }
            AverageVolume = Utility.Round2(TotalVolume / (index + 1));

            for (int i = 0; i < SupplyDistribution.Count; i++)
            {
                per = Math.Round(SupplyDistribution[ls_sdkey[i]] / TotalVolume, 4);
                Percent.Add(per);
                sumPercent += per;
                if (Mid == 0 && sumPercent > 0.5)
                {
                    Mid = ls_sdkey[i];
                }
            }
            if (sumPercent < 0.95)
            {
                Utility.Log("Date:" + stock.runData.cur_date + " sumPercent = " + Utility.Round2(sumPercent));
            }

            ////////////////////////////////////////////////////////////
            //PricePoint = Stock.GetPricePoint(sdata[ls_key[index - 1]].close);
            High = 0;
            double ArbitragePrice = 0;
            double UpResistPercent = 0;
            double ResistancePrice = 0;
            //double CurHigh = 0;
            for (int i = 0; i < SupplyDistribution.Count; i++)
            {
                if (stock.sdata[iPreDate].close <= ls_sdkey[i] && Percent[i] > ResistRate)//
                {
                    ResistancePrice = ls_sdkey[i];// Utility.Round2(GetHighPoint(ls_sdkey[i]) * 1.00);
                    ArbitragePrice = Utility.Round2(ResistancePrice * 1.10);
                    UpResistPercent = 0;
                    for (int j = 0; j < Percent.Count; j++)
                    {
                        if (ls_sdkey[j] >= ResistancePrice && ls_sdkey[j] <= ArbitragePrice)
                        {
                            UpResistPercent += Percent[j];
                        }
                    }
                    if (UpResistPercent < 0.10)
                    {
                        High = ResistancePrice;
                    }
                    if (High > Mid * 0.95)
                    {
                        High = 0;
                    }
                    //High = GetHighPoint(ls_sdkey[i]);
                    break;
                }
            }

            ////////////////////////////////////////////////////////////
            //Low = 0;
            double move_low = 0;
            for (int i = 0; i < SupplyDistribution.Count; i++)
            {
                if (stock.sdata[ls_key[index - 1]].high >= ls_sdkey[i] && Percent[i] > 0.05)//
                {
                    move_low = ls_sdkey[i] * StopLossRate;
                }
            }
            if (move_low > Low)
            {
                Low = move_low;
            }

            return EST.EST_OK;
        }
    }

    public class KDJ
    {
        public int CountNum;
        public double kt;
        public double dt;
        public double jt;
        double rsvt = 0;
        double high = 0;
        double low = 0;
        public double buy_break = 0;
        public double sell_break = 0;
        public bool bInactive;
        public bool bBreakthrough;
        public KDJ()
        {
            CountNum = 9;
            kt = -1;
            dt = 0;
            jt = 0;
            buy_break = 0;
            sell_break = 0;
            bInactive = false;
            bBreakthrough = false;
        }
        public KDJ(KDJ other)
        {
            kt = other.kt;
            dt = other.dt;
            jt = other.jt;
            buy_break = other.buy_break;
            sell_break = other.sell_break;
        }
        public double Calc(Stock stock)
        {
            int index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index < CountNum) return 0;
            //int last = index - 1;

            low = int.MaxValue;
            high = 0;
            List<int> ls_key = new List<int>(stock.sdata.Keys);
            for (int i = index - CountNum; i < index; i++)
            {
                if (high < stock.sdata[ls_key[i]].high)
                {
                    high = stock.sdata[ls_key[i]].high;
                }
                if (low >= stock.sdata[ls_key[i]].low)
                {
                    low = stock.sdata[ls_key[i]].low;
                }
            }

            double ct = stock.sdata[ls_key[index - 1]].close;
            rsvt = (ct - low) / (high - low) * 100;
            if (kt == -1)
            {
                kt = rsvt / 3 + 50 * 2 / 3;
                dt = kt / 3 + 50 * 2 / 3;
            }
            else
            {
                kt = rsvt / 3 + kt * 2 / 3;
                dt = kt / 3 + dt * 2 / 3;
            }
            jt = 3 * dt - 2 * kt;

            /*
            if (bInactive)
            {
                if (jt > 20)
                {
                    bBreakthrough = true;
                }
            }
            */
            double rsv = 0;
            if (jt < 20)
            {
                bInactive = true;
                rsv = 6 * dt - 20 * 3 - 2 * kt;
                buy_break = rsv * (high - low) / 100 + low;
            }
            else
            {
                buy_break = 0;
                bInactive = false;
            }

            CalcSellBreak(stock);
            /*
            if (jt > 80)
            {
                CalcSellBreak(stock);
            }
            else
            {
                sell_break = 0;
            }
            */

            return buy_break;
        }
        public double CalcBuyBreak(Stock stock)
        {
            double rsv = 6 * dt - 20 * 3 - 2 * kt;
            buy_break = rsv * (high - low) / 100 + low;
            return buy_break;
        }
        public double CalcSellBreak(Stock stock)
        {
            double rsv = 6 * dt - 80 * 3 - 2 * kt;
            sell_break = rsv * (high - low) / 100 + low;
            return sell_break;
        }
        public double CalcBreakthrough2(Stock stock)
        {
            buy_break = jt * (high - low) / 100 + low;
            return buy_break;
        }
    }

    public class PERatio
    {
        public double pe;
        public double risk;

        public PERatio()
        {
            pe = 0;
            risk = 0;
        }
        public PERatio(PERatio other)
        {
            pe = other.pe;
            risk = other.risk;
        }
        public double Calc(Stock stock)
        {
            int iDate = stock.runData.cur_date;
            int iCurYear = Utility.GetYear(iDate);
            int iLastYear = iCurYear - 1;
            double earn = 0;
            foreach (var it in stock.activity)
            {
                if (Utility.GetYear(it.Value.date) == iLastYear)
                {
                    earn += it.Value.bonus;
                }
            }
            int index = stock.sdata.IndexOfKey(stock.runData.cur_date);
            if (index < 1) return 0;

            List<int> ls_key = new List<int>(stock.sdata.Keys);
            pe = Utility.Round2(stock.sdata[ls_key[index]].open / earn);

            double ma = stock.dmData.ma90.ma;
            risk = Utility.Round2(stock.sdata[stock.runData.cur_date].open / ma) * 100;

            return pe;
        }
    }

}
