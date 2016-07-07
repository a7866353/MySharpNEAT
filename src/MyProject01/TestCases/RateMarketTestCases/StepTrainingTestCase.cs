using MyProject01.Factorys.ControllerFactorys;
using MyProject01.Factorys.PopulationFactorys;
using MyProject01.Factorys.TrainerFactorys;
using MyProject01.Factorys.TrainingDataFactorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.TestCases.RateMarketTestCases
{
    class FwtReduceLossRecentM5StepTestCase : BasicRateMarketTestCase
    {
        public int DataBlockLength
        {
            set { controllerFactory.InputLength = value; }
        }
        public int PopulationNumber
        {
            set { popFactory.PopulationNumber = value; }
        }
        private NEATFWTNromStateKeepControllerFactory controllerFactory;
        private BasicTrainingDataFactory dataFactory;
        private ReduceLossStepTrainerFactory trainerFactory;
        private NormalPopulationFactory popFactory;

        public FwtReduceLossRecentM5StepTestCase()
        {
            controllerFactory = new NEATFWTNromStateKeepControllerFactory();
            dataFactory = new RecentUSDJPYM5DataFactory();
            trainerFactory = new ReduceLossStepTrainerFactory();
            popFactory = new NormalPopulationFactory();


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

            controllerFactory.NormalyzeData = trainerFactory.TrainingData.TrainDataBlock;
            controllerFactory.Name = TestName;

            trainerFactory.Controller = controllerFactory.Get();

            _train = trainerFactory.Get();
        }
    }

}
