using MyProject01.Factorys.ControllerFactorys;
using MyProject01.Factorys.PopulationFactorys;
using MyProject01.Factorys.TrainerFactorys;
using MyProject01.Factorys.TrainingDataFactorys;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.TestCases.RateMarketTestCases
{
    class KDJTestCase : BasicRateMarketTestCase
    {
        public int DataBlockLength
        {
            set { controllerFactory.InputLength = value; }
        }
        public int PopulationNumber
        {
            set { popFactory.PopulationNumber = value; }
        }

        private BasicControllerFactory controllerFactory;
        private OldRate5MinKDJDataFactory dataFactory;
        private FirstTrainerFactory trainerFactory;
        private NormalPopulationFactory popFactory;

        public KDJTestCase()
        {
            controllerFactory = new KdjControllerFactory();
            dataFactory = new OldRate5MinKDJDataFactory();
            trainerFactory = new FirstTrainerFactory();
            popFactory = new NormalPopulationFactory();

            controllerFactory.InputLength = 32;

            _descList.Add(controllerFactory);
            _descList.Add(dataFactory);
            _descList.Add(trainerFactory);
            _descList.Add(popFactory);
        }

        protected override void Init()
        {
            dataFactory.DataBlockLength = controllerFactory.InputLength;

            trainerFactory.PopulationFacotry = popFactory;
            trainerFactory.TestDescription = TestDescription;

            trainerFactory.TrainingData = dataFactory.Get();
            trainerFactory.TestCaseName = TestName;

            controllerFactory.Name = TestName;
            trainerFactory.Controller = controllerFactory.Get();

            _train = trainerFactory.Get();
        }
    }
    class KDJNormTestCase : BasicRateMarketTestCase
    {
        public int DataBlockLength
        {
            set { controllerFactory.InputLength = value; }
        }
        public int PopulationNumber
        {
            set { popFactory.PopulationNumber = value; }
        }

        private KdjNormControllerFactory controllerFactory;
        private OldRate5MinKDJDataFactory dataFactory;
        private FirstTrainerFactory trainerFactory;
        private NormalPopulationFactory popFactory;

        public KDJNormTestCase()
        {
            controllerFactory = new KdjNormControllerFactory();
            dataFactory = new OldRate5MinKDJDataFactory();
            trainerFactory = new FirstTrainerFactory();
            popFactory = new NormalPopulationFactory();

            controllerFactory.InputLength = 32;

            _descList.Add(controllerFactory);
            _descList.Add(dataFactory);
            _descList.Add(trainerFactory);
            _descList.Add(popFactory);
        }

        protected override void Init()
        {
            dataFactory.DataBlockLength = controllerFactory.InputLength;

            trainerFactory.PopulationFacotry = popFactory;
            trainerFactory.TestDescription = TestDescription;

            trainerFactory.TrainingData = dataFactory.Get();
            trainerFactory.TestCaseName = TestName;

            controllerFactory.Name = TestName;
            controllerFactory.NormalyzeData = dataFactory.Get().TestDataBlock;
            trainerFactory.Controller = controllerFactory.Get();

            _train = trainerFactory.Get();
        }
    }
    class KDJNormRecentTestCase : BasicRateMarketTestCase
    {
        public int DataBlockLength
        {
            set { controllerFactory.InputLength = value; }
        }
        public int PopulationNumber
        {
            set { popFactory.PopulationNumber = value; }
        }

        private KdjNormControllerFactory controllerFactory;
        private RecentUSDJPYKDJM5DataFactory dataFactory;
        private FirstTrainerFactory trainerFactory;
        private NormalPopulationFactory popFactory;

        public KDJNormRecentTestCase()
        {
            controllerFactory = new KdjNormControllerFactory();
            dataFactory = new RecentUSDJPYKDJM5DataFactory();
            trainerFactory = new FirstTrainerFactory();
            popFactory = new NormalPopulationFactory();

            controllerFactory.InputLength = 32;

            _descList.Add(controllerFactory);
            _descList.Add(dataFactory);
            _descList.Add(trainerFactory);
            _descList.Add(popFactory);
        }

        protected override void Init()
        {
            dataFactory.DataBlockLength = controllerFactory.InputLength;

            trainerFactory.PopulationFacotry = popFactory;
            trainerFactory.TestDescription = TestDescription;

            trainerFactory.TrainingData = dataFactory.Get();
            trainerFactory.TestCaseName = TestName;

            controllerFactory.Name = TestName;
            controllerFactory.NormalyzeData = dataFactory.Get().TestDataBlock;
            trainerFactory.Controller = controllerFactory.Get();

            _train = trainerFactory.Get();
        }
    }

}
