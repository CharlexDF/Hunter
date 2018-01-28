using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    //enum state type
    public class EST
    {
        public static int Count = 0;
        public static int EST_OK = Count++;
        public static int EST_ERROR = Count++;
        public static int EST_CONNECT_FAIL = Count++;
        public static int EST_NOT_FOUND = Count++;
        public static int EST_RECIVE_FAIL = Count++;
        public static int EST_WRITE_FAIL = Count++;
        public static int EST_SKIP = Count++;
    };

    //EOperateState
    public class EOperateState
    {
        public static int Count = 0;
        public static int None = Count++;       //0
        public static int Buy = Count++;        //1
        public static int Sell = Count++;       //2
        public static int Hold = Count++;       //3
    }

    
    
    //ESellReason
    public class ESR
    {
        public static int Count = 0;
        public static int EPR_None = Count++;       //0
        public static int EPR_Normal = Count++;     //1
        public static int EPR_Stop = Count++;       //2
        public static int EPR_OverDate = Count++;   //3
        public static int EPR_EndDate = Count++;    //4
    }

    class EnumDDL
    {
    }
}
