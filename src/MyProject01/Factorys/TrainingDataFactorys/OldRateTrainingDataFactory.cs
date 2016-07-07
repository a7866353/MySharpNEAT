using MyProject01.Controller;
using MyProject01.Util;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Factorys.TrainingDataFactorys
{
    class OldRateDataRange
    {
        static public DateTime M5ShortStart = new DateTime(2014, 7, 1);
        static public DateTime M5ShortEnd = M5ShortStart.AddDays(20);
    }
    class OldRateTrainingDataFactory : BasicTrainingDataFactory
    {
        public double TestDataRate = 0.7;
        public DateTime StartDateTime = new DateTime(2013, 10, 31);
        public DateTime EndDateTime = new DateTime(2014, 10, 31);
        public DataTimeType TimeFrame = DataTimeType.D1;
        public int Count = 1000;
        protected override TrainingData Create()
        {
            BasicTestDataLoader loader;
            loader = new TestDataDateRangeLoader("USDJPY", TimeFrame, StartDateTime, EndDateTime, DataBlockLength - 1);
            loader.Load();
            // loader.Fillter(new DateTime(2013, 1, 1), DateTime.Now);

            RateDataBlock testBlock = new RateDataBlock(loader, 0, loader.Count, DataBlockLength);
            TrainingData td = new TrainingData(
                new RateDataBlock(loader, 0, loader.Count, DataBlockLength),
                new RateDataBlock(loader, 0, (int)(loader.Count * TestDataRate), DataBlockLength)
                );

            return td;
        }

        public override string Description
        {
            get { return "USDJPY_" + Enum.GetName(typeof(DataTimeType), TimeFrame); }
        }
    }
    class OldRate1DayTrainingDataFactory : BasicTrainingDataFactory
    {
        public double TestDataRate = 0.7;
        public DateTime StartDateTime = new DateTime(2013, 10, 31);
        public DateTime EndDateTime = new DateTime(2014, 10, 31);
        public DataTimeType TimeFrame = DataTimeType.D1;     
        public int Count = 1000;
        protected override TrainingData Create()
        {
            BasicTestDataLoader loader;
            loader = new TestDataDateRangeLoader("USDJPY", DataTimeType.D1, StartDateTime, EndDateTime, DataBlockLength-1);
            loader.Load();
            // loader.Fillter(new DateTime(2013, 1, 1), DateTime.Now);
           
            RateDataBlock testBlock = new RateDataBlock(loader, 0, loader.Count, DataBlockLength);
            TrainingData td = new TrainingData(
                new RateDataBlock(loader, 0, loader.Count, DataBlockLength),
                new RateDataBlock(loader, 0, (int)(loader.Count * TestDataRate), DataBlockLength)
                );

            return td;
        }

        public override string Description
        {
            get { return "USDJPY_1DAY"; }
        }
    }

    class OldRate5MinTrainingDataFactory : BasicTrainingDataFactory
    {
        public double TestDataRate = 0.7;
        public DateTime StartDateTime = new DateTime(2013, 10, 31);
        public DateTime EndDateTime = new DateTime(2014, 10, 31);
        public int Count = 1000;
        protected override TrainingData Create()
        {
            BasicTestDataLoader loader;
            loader = new TestDataDateRangeLoader("USDJPY", DataTimeType.M5, StartDateTime, EndDateTime, DataBlockLength - 1);
            loader.Load();
            RateDataBlock testBlock = new RateDataBlock(loader, 0, loader.Count, DataBlockLength);
            TrainingData td = new TrainingData(
                new RateDataBlock(loader, 0, loader.Count, DataBlockLength),
                new RateDataBlock(loader, 0, (int)(loader.Count * TestDataRate), DataBlockLength)
                );

            return td;
        }

        public override string Description
        {
            get { return "USDJPY_5MIN"; }
        }
    }

    class OldRate5MinKDJDataFactory : BasicTrainingDataFactory
    {
        public double TestDataRate = 0.7;
        public DateTime StartDateTime = OldRateDataRange.M5ShortStart;
        public DateTime EndDateTime = OldRateDataRange.M5ShortEnd;
        public int Count = 1000;
        protected override TrainingData Create()
        {
            BasicTestDataLoader loader;
            loader = new TestDataDateRangeLoader("USDJPY", DataTimeType.M5, StartDateTime, EndDateTime, DataBlockLength - 1);
            loader.Load();
            RateDataBlock testBlock = new RateDataBlock(loader, 0, loader.Count, DataBlockLength);
            TrainingData td = new TrainingData(
                new KDJDataBlock(loader, 0, loader.Count, DataBlockLength),
                new KDJDataBlock(loader, 0, (int)(loader.Count * TestDataRate), DataBlockLength)
                );

            return td;
        }

        public override string Description
        {
            get { return "USDJPY_5MIN_KDJ"; }
        }
    }

}
