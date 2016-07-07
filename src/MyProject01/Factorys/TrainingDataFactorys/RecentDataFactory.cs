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
    class RecentUSDJPYM30DataFactory : BasicTrainingDataFactory
    {
        public double TestDataRate = 0.7;
        // public DateTime StartDateTime = new DateTime(2012, 6, 1);
        public DateTime EndDateTime = DateTime.Now;
        public DateTime StartDateTime = DateTime.Now.AddMonths(-24);
        private DataTimeType TimeFrame = DataTimeType.M30;
        private string SymbolName = "USDJPYpro_30_USDJPYpro30";
        protected override TrainingData Create()
        {
            BasicTestDataLoader loader;
            loader = new TestDataDateRangeLoader(SymbolName, TimeFrame, StartDateTime, EndDateTime, DataBlockLength - 1);
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
            get { return "USDJPYpro30" + "R:" + TestDataRate.ToString("G2"); }
        }
    }

    class RecentUSDJPYM5DataFactory : BasicTrainingDataFactory
    {
        public double TestDataRate = 0.85;
        // public DateTime StartDateTime = new DateTime(2012, 6, 1);
        public DateTime EndDateTime = DateTime.Now;
        public DateTime StartDateTime = DateTime.Now.AddMonths(-3);
        private DataTimeType TimeFrame = DataTimeType.M5;
        private string SymbolName = "USDJPYpro_5_USDJPYpro5";

        protected override TrainingData Create()
        {
            BasicTestDataLoader loader;
            loader = new TestDataDateRangeLoader(SymbolName, TimeFrame, StartDateTime, EndDateTime, DataBlockLength - 1);
            loader.Load();
            // loader.Fillter(new DateTime(2013, 1, 1), DateTime.Now);

            TrainingData td = new TrainingData(
                new RateDataBlock(loader, 0, loader.Count, DataBlockLength),
                new RateDataBlock(loader, 0, (int)(loader.Count * TestDataRate), DataBlockLength)
                );

            return td;
        }

        public override string Description
        {
            get { return "USDJPYpro5" + "R:" + TestDataRate.ToString("G2"); }
        }
    }

    class RecentUSDJPYKDJM5DataFactory : BasicTrainingDataFactory
    {
        public double TestDataRate = 0.85;
        // public DateTime StartDateTime = new DateTime(2012, 6, 1);
        public DateTime EndDateTime = DateTime.Now;
        public DateTime StartDateTime = DateTime.Now.AddMonths(-3);
        private DataTimeType TimeFrame = DataTimeType.M5;
        private string SymbolName = "USDJPYpro_5_USDJPYpro5";

        protected override TrainingData Create()
        {
            BasicTestDataLoader loader;
            loader = new TestDataDateRangeLoader(SymbolName, TimeFrame, StartDateTime, EndDateTime, DataBlockLength - 1);
            loader.Load();
            // loader.Fillter(new DateTime(2013, 1, 1), DateTime.Now);

            TrainingData td = new TrainingData(
                new KDJDataBlock(loader, 0, loader.Count, DataBlockLength),
                new KDJDataBlock(loader, 0, (int)(loader.Count * TestDataRate), DataBlockLength)
                );

            return td;
        }

        public override string Description
        {
            get { return "USDJPYpro5" + "R:" + TestDataRate.ToString("G2"); }
        }
    }

}
