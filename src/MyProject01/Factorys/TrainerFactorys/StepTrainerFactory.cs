using MyProject01.Controller;
using MyProject01.Controller.Jobs;
using MyProject01.Factorys.PopulationFactorys;
using MyProject01.TrainerFactorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Factorys.TrainerFactorys
{
    class ReduceLossStepTrainerFactory : BasicTrainerFactory
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
            DataUpdateJob dataUpdateJob = new DataUpdateJob(
                TrainingData.TrainDataBlock, 
                new ReduceLossScore            
                {
                    TradeDecisionCtrl = Controller.GetDecisionController(),
                });
            mainCheckCtrl.Add(dataUpdateJob);
            mainCheckCtrl.Add(new CheckNetworkChangeJobV2());
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
            trainer.ScoreCtrl = dataUpdateJob.Score;

            return trainer;
        }

        public override string Description
        {
            get { return "ReduceLossStepTrainer"; }
        }
    }
}
