using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Test
{
    public class NetworkTestParameter
    {
        //-----------------------------------------------
        // Common
        public string name;
        public double ErrorChangeLimit = 0.01;
        public int ErrorChangeRetryCount = 10;
        public double ErrorLimit = 0.01;
        public int MaxTryCount = 1000;

        //-----------------------------------------------
        // Network
        public int InputSize;
        public int OutputSize;
        public int HidenLayerNum;
        public double hidenLayerRaio = 10000;

        //-----------------------------------------------
        // Test

        public NetworkTestParameter(string name, double changeErrorLimit, double hidenLayerRaio, int changeErrorRetryCnt)
        {
            this.name = name;
            this.hidenLayerRaio = hidenLayerRaio;
            this.ErrorChangeLimit = changeErrorLimit;
            this.ErrorChangeRetryCount = changeErrorRetryCnt;
        }
        public NetworkTestParameter(string name, int hidenLayerNum)
        {
            this.name = name;
            this.HidenLayerNum = hidenLayerNum;
        }

        public override string ToString()
        {
            return "Err:" + ErrorChangeLimit.ToString("G2") + " NeroRaio:" + hidenLayerRaio.ToString();
        }
    }
}
