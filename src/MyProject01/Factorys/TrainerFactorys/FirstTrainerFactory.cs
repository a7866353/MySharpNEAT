using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject01.Controller.Jobs;
using MyProject01.Factorys.PopulationFactorys;
using MyProject01.Controller;
using MyProject01.TrainerFactorys;

namespace MyProject01.Factorys.TrainerFactorys
{
    class FirstTrainerFactory : BasicTrainerFactory
    {
        public string TestCaseName;
        public string TestDescription;
        public NetworkController Controller;
        public TrainingData TrainingData;
        public BasicPopulationFactory PopulationFacotry;

        protected override Trainer Create()
        {
            NormalTrainer trainer = new NormalTrainer();

            TrainResultCheckSyncController mainCheckCtrl = new TrainResultCheckSyncController();
            mainCheckCtrl.Add(new CheckNetworkChangeJob());
            mainCheckCtrl.Add(new UpdataControllerJob(Controller));

            // TrainResultCheckAsyncController subCheckCtrl = new TrainResultCheckAsyncController();
            // subCheckCtrl.Add(new UpdateTestCaseJob() 
            mainCheckCtrl.Add(new UpdateTestCaseJob()
            {
                TestName = TestCaseName,
                TestDescription = TestDescription,
                DecisionCtrl = Controller.GetDecisionController(),
                TestDataBlock = TrainingData.TestDataBlock,
            });

            // mainCheckCtrl.Add(subCheckCtrl);

            trainer.CheckCtrl = mainCheckCtrl;
            trainer.DecisionCtrl = Controller.GetDecisionController();
            trainer.TrainDataBlock = TrainingData.TrainDataBlock;
            trainer.PopulationFacotry = PopulationFacotry;
            trainer.ScoreCtrl = new NormalScore()
            {
                TradeDecisionCtrl = trainer.DecisionCtrl,
                dataBlock = trainer.TrainDataBlock,
            };

            return trainer;
        }

        public override string Description
        {
            get { return "NormalTrainer"; }
        }
    }

    class ReduceLossTrainerFactory : BasicTrainerFactory
    {
        public string TestCaseName;
        public string TestDescription;
        public NetworkController Controller;
        public TrainingData TrainingData;
        public BasicPopulationFactory PopulationFacotry;

        protected override Trainer Create()
        {
            NormalTrainer trainer = new NormalTrainer();

            TrainResultCheckSyncController mainCheckCtrl = new TrainResultCheckSyncController();
            mainCheckCtrl.Add(new CheckNetworkChangeJob());
            mainCheckCtrl.Add(new UpdataControllerJob(Controller));

            // TrainResultCheckAsyncController subCheckCtrl = new TrainResultCheckAsyncController();
            // subCheckCtrl.Add(new UpdateTestCaseJob() 
            mainCheckCtrl.Add(new UpdateTestCaseJob()
            {
                TestName = TestCaseName,
                TestDescription = TestDescription,
                DecisionCtrl = Controller.GetDecisionController(),
                TestDataBlock = TrainingData.TestDataBlock,
            });

            // mainCheckCtrl.Add(subCheckCtrl);

            trainer.CheckCtrl = mainCheckCtrl;
            trainer.DecisionCtrl = Controller.GetDecisionController();
            trainer.TrainDataBlock = TrainingData.TrainDataBlock;
            trainer.PopulationFacotry = PopulationFacotry;
            trainer.ScoreCtrl = new ReduceLossScore()
            {
                TradeDecisionCtrl = trainer.DecisionCtrl,
                dataBlock = trainer.TrainDataBlock,
            };

            return trainer;
        }

        public override string Description
        {
            get { return "ReduceLossTrainer"; }
        }
    }

    class NewTrainerFactory : BasicTrainerFactory
    {
        public string TestCaseName;
        public string TestDescription;
        public NetworkController Controller;
        public TrainingData TrainingData;
        public BasicPopulationFactory PopulationFacotry;

        protected override Trainer Create()
        {
            NormalTrainer trainer = new NormalTrainer();

            TrainResultCheckSyncController mainCheckCtrl = new TrainResultCheckSyncController();
            mainCheckCtrl.Add(new CheckNetworkChangeJob());
            mainCheckCtrl.Add(new UpdataControllerJob(Controller));

            // TrainResultCheckAsyncController subCheckCtrl = new TrainResultCheckAsyncController();
            // subCheckCtrl.Add(new UpdateTestCaseJob() 
            mainCheckCtrl.Add(new UpdateTestCaseJob()
            {
                TestName = TestCaseName,
                TestDescription = TestDescription,
                DecisionCtrl = Controller.GetDecisionController(),
                TestDataBlock = TrainingData.TestDataBlock,
            });

            // mainCheckCtrl.Add(subCheckCtrl);

            trainer.CheckCtrl = mainCheckCtrl;
            trainer.DecisionCtrl = Controller.GetDecisionController();
            trainer.TrainDataBlock = TrainingData.TrainDataBlock;
            trainer.PopulationFacotry = PopulationFacotry;
            trainer.ScoreCtrl = new ReduceLossScore()
            {
                TradeDecisionCtrl = trainer.DecisionCtrl,
                dataBlock = trainer.TrainDataBlock,
            };

            return trainer;
        }

        public override string Description
        {
            get { return "ReduceLossTrainer"; }
        }
    }

}
