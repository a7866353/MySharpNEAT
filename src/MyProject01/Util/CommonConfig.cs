using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util
{
    class DataLoaderParam
    {
        public string TickerName;
        public DataTimeType TimeFrame;
        public DateTime StartDate;
        public DateTime EndDate;
        public int PreCount = 50000;
        public bool NeedTimeFrameConver = true;
        public bool IsDefault = false;

        public BasicTestDataLoader GetLoader()
        {
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader(
                    TickerName, 
                    TimeFrame, 
                    StartDate, 
                    EndDate, 
                    PreCount) 
                    { NeedTimeFrameConver = NeedTimeFrameConver };
            return loader;
        }
        public override string ToString()
        {
            return TickerName + "_" + TimeFrame.ToString() + "_" +
                "Len=" + (int)Math.Round(((EndDate - StartDate).TotalDays / 30)) + "M_" +
                StartDate.ToShortDateString() + "-" + EndDate.ToShortDateString() + "_" +
                "Cov=" + NeedTimeFrameConver;
        }
    }
    class DataLoaderParamList
    {
        static int _preCount = 50000;
        static public DataLoaderParam[] GetParams()
        {
            List<DataLoaderParam> paramList = new List<DataLoaderParam>();
            DateTime startDate;
            DateTime endDate;


            // Current
            //------------------------------

            // M1
            endDate = DateTime.Now;
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            // M30
            endDate = DateTime.Now;
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            endDate = DateTime.Now;
            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            // Current not cov
            //------------------------------
            endDate = DateTime.Now;

            // M1
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            // M5
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_5",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });
            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_5",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            // M30
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_30",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_30",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            // Recent
            //---------------------------
            endDate = new DateTime(2016, 3, 13);

            // M5
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddYears(-2);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddYears(-10);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            // M30
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddYears(-2);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddYears(-10);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            // D1
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddYears(-2);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            startDate = endDate.AddYears(-10);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = true,
                IsDefault = false
            });

            // Recent not cov
            //---------------------------
            endDate = new DateTime(2016, 3, 13);

            // M5
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_5",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_5",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddYears(-2);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_5",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddYears(-10);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_5",
                TimeFrame = DataTimeType.M5,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            // M30
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_30",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_30",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = true
            });

            startDate = endDate.AddYears(-2);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_30",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddYears(-10);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_30",
                TimeFrame = DataTimeType.M30,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            // D1
            startDate = endDate.AddMonths(-1);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1440",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddMonths(-3);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1440",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddYears(-2);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1440",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            startDate = endDate.AddYears(-10);
            paramList.Add(new DataLoaderParam()
            {
                TickerName = "USDJPY_1440",
                TimeFrame = DataTimeType.D1,
                StartDate = startDate,
                EndDate = endDate,
                PreCount = _preCount,
                NeedTimeFrameConver = false,
                IsDefault = false
            });

            return paramList.ToArray();
        }
    }

    class ServerIPParam
    {
        public string IP;
        public bool IsDefault = false;
    }
    class ServerIPParamList
    {
        static public ServerIPParam[] IPs
        {
            get
            {
                return new ServerIPParam[]
                {
                    new ServerIPParam(){ IP = "127.0.0.1", IsDefault = false},
                    new ServerIPParam(){ IP = "192.168.1.11", IsDefault = false},
                    new ServerIPParam(){ IP = "192.168.1.15", IsDefault = true},

                };
            }
        }
    }

    class CommonConfig
    {
        // public static string ServerIP = "127.0.0.1";
        public static string ServerIP = "192.168.1.15";
        public static int PopulationSize = 2048;
        public static DataLoaderParam LoaderParam = null;
        public static double BuyOffset = 0.01;
        public static double SellOffset = 0.01;
        public static int TrainingDataBlockLength = 32;
        public static int TrainingTryCount = 5;
    }




}
