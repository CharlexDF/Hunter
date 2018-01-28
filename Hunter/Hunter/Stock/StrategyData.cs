using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class StrategyData
    {
        public DateTime dt;
        public KDJ kdj;
        public PERatio pe;
        //public Turtle tr5;
        public Turtle tr20;
        public Turtle tr55;
        public MACD ma5;
        public MACD ma10;
        public MACD ma20;
        public MACD ma30;
        public MACD ma90;
        public Box b200;
        
        public StrategyData()
        {
            kdj = new KDJ();
            pe = new PERatio();
            tr20 = new Turtle(20);
            tr55 = new Turtle(55);
            ma5 = new MACD(5);
            ma10 = new MACD(10);
            ma20 = new MACD(20);
            ma30 = new MACD(30);
            ma90 = new MACD(90);
            b200 = new Box(200);
        }

        public StrategyData(StrategyData other)
        {
            kdj = new KDJ(other.kdj);
            pe = new PERatio(other.pe);
            tr20 = new Turtle(other.tr20);
            tr55 = new Turtle(other.tr55);
            ma5 = new MACD(other.ma5);
            ma10 = new MACD(other.ma10);
            ma20 = new MACD(other.ma20);
            ma30 = new MACD(other.ma30);
            ma90 = new MACD(other.ma90);
            b200 = new Box(other.b200);
        }

        public int PreProcessData(TrendData trend)
        {
            return EST.EST_OK;
        }
        public int ProcessData(TrendData trend)
        {
            ma5.Calc(trend);
            ma10.Calc(trend);
            ma20.Calc(trend);
            ma30.Calc(trend);
            ma90.Calc(trend);
            //CalcBuyInfo(trend);
            return EST.EST_OK;
        }
    }
}
