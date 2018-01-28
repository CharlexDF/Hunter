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
    class ThreadData
    {
        public static int Count = 0;
        public virtual int DoProc() { return EST.EST_OK; }
    }
    
    class ThreadData_Stock : ThreadData
    {
        public Stock stock;
        public Delegate_Int_Function taskProc;

        public ThreadData_Stock(Stock _stock, Delegate_Int_Function _taskProc)
        {
            stock = _stock;
            taskProc = _taskProc;
        }

        public override int DoProc()
        {
            return taskProc();
        }
    }
    
    class ThreadData_TradingStrategy : ThreadData
    {
        public Stock stock;
        public Delegate_Int_Function_Stock taskProc;

        public ThreadData_TradingStrategy(Stock _stock, Delegate_Int_Function_Stock _taskProc)
        {
            stock = _stock;
            taskProc = _taskProc;
        }

        public override int DoProc()
        {
            return taskProc(stock);
        }
    }

    class ThreadController
    {
        private static int TotalCount;
        private static int FinishCount;
        private static int SuccessCount;
        private static int FailCount;
        public static bool bShowLog;
        private static bool bStartProc;
        private static Mutex Mutex_ThreadCount = new Mutex();
        
        public static void InitParams(int _TotalCount, bool _bShowLog)
        {
            TotalCount = _TotalCount;
            FinishCount = 0;
            SuccessCount = 0;
            FailCount = 0;
            bStartProc = false;

            bShowLog = _bShowLog;
        }

        /*
        public static void StartProc(ThreadData threadInfo)
        {
            bStartProc = true;
            //Hunter.BugList.Clear();
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoTaskProc), threadInfo);
        }
        private static void DoTaskProc(object _threadInfo)
        {
            ThreadData threadInfo = new ThreadData();// (ThreadData_Stock)_threadInfo;
            int ErrCode = threadInfo.DoProc();
            ThreadFinishCount(threadInfo.stock, ErrCode);
        }
        */

        public static void StartProc(ThreadData_Stock threadInfo)
        {
            bStartProc = true;
            //Hunter.BugList.Clear();
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoTaskProc_Stock), threadInfo);
        }
        private static void DoTaskProc_Stock(object _threadInfo)
        {
            ThreadData_Stock threadInfo = (ThreadData_Stock)_threadInfo;
            int ErrCode = threadInfo.DoProc();
            ThreadFinishCount(threadInfo.stock, ErrCode);
        }

        public static void StartProc(ThreadData_TradingStrategy threadInfo)
        {
            bStartProc = true;
            //Hunter.BugList.Clear();
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoTaskProc_TradingStrategy), threadInfo);
        }
        private static void DoTaskProc_TradingStrategy(object _threadInfo)
        {
            ThreadData_TradingStrategy threadInfo = (ThreadData_TradingStrategy)_threadInfo;
            int ErrCode = threadInfo.DoProc();
            ThreadFinishCount(threadInfo.stock, ErrCode);
        }

        private static void ThreadFinishCount(Stock stock, int err)
        {
            Mutex_ThreadCount.WaitOne();

            FinishCount++;
            if (err == EST.EST_OK)
            {
                SuccessCount++;
                if (bShowLog)
                {
                    //Utility.Log("Th " + FinishCount + "/" + TotalCount + " Thread OK! stock id =  " + stock.sid);
                }
            }
            else if (err == EST.EST_SKIP)
            {
                SuccessCount++;
                if (bShowLog)
                {
                    //Utility.Log("Th " + FinishCount + "/" + TotalCount + " Thread Skip! stock id =  " + stock.sid);
                }
            }
            else
            {
                FailCount++;
                //Hunter.BugList.Add(stock.sidToString());
                if (!bShowLog)
                {
                    Utility.Log(" Error sid =  " + stock.sidToString() + " error code = " + err);
                }
            }

            if (bShowLog)
            {
                Utility.Log("Th " + FinishCount + "/" + TotalCount);
            }

            Mutex_ThreadCount.ReleaseMutex();
        }

        public static void WaitQueueFinished()
        {
            if (bStartProc == false)
            {
                Utility.Log("WaitQueueFinished no thread running...");
                return;
            }

            while (FinishCount < TotalCount)
            {
                if (!bShowLog)
                {
                    Utility.Log("   Thread progress " + FinishCount + "/" + TotalCount);
                }

                Thread.Sleep(1000);
            }

            Utility.Log("   Thread progress " + FinishCount + "/" + TotalCount);
            Utility.Log("WaitQueueFinished progress " + SuccessCount + "/" + TotalCount);

            //Stock.WriteBugList();
        }

        
    }
}
