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
    public class Config
    {
        public static string VERSION = "0.1.1";//0:测试版 1:第1版 1:小版本

        public static int StartDate = 20050101;

        public static string UrlHead = "http://";
        public static string SinaDomains = "finance.sina.com.cn";
        //params
        public static double SlippagePrice = 0.01;
        public static double SlippageRate = 0.01;
        public static double BuySlippagePrice = 1 + SlippageRate;
        public static double SellSlippagePrice = 1 - SlippageRate;
        
        //
        public static double StampDutyRate = 0.001;        //印花税 卖方成交金额的0.1% 
        public static double CommissionRate = 0.0003;      //佣金 双向收取,成交金额的0.03%，最低5元
        public static double TransferFeeRate = 0.0001;     //过户费 按成交股票数量0.06%收取,双向收取,仅上海股票

        public static int DebugSid = 601668;//002496 600000 601668

        //setting
        public static bool bTrue = true;
        public static bool bFalse = false;
        public static bool bDebug = true;
        public static bool bDebugSingle = true;
        public static bool bRecordDebugList = false;
        public static bool bMultiThread = true;

        public static bool bDownloadUpdate = false;
        public static bool bFroceDownload_StockData = false;
        public static bool bFroceDownload_ActivityData = false;

        public static bool bLogBuyDeal = false;
        public static bool bLogSellDeal = false;


    }
}

