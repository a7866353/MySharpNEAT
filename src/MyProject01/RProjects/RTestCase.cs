using MyProject01.Controller;
using MyProject01.Controller.Jobs;
using MyProject01.DAO;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.RProjects
{
    class RTestCase
    {
        private ControllerFactory _ctrlFac;
        private RController _testCtrl;
        private BasicTestDataLoader _loader;
        private AgentFactory _agentFac;

        private double _testRate = 0.7;
        private int _startPosition = 50000;
        private int _trainBlockLength = 32;
        private int _trainTryCount = 2;

        private int _trainDataLength;
        private int _testDataLength;

        public DateTime CreateDatetime = DateTime.Now;
        public String TestCaseName = "RTest";
        public void Run()
        {
            // Config Server IP
            DataBaseAddress.SetIP(CommonConfig.ServerIP);

            _trainBlockLength = CommonConfig.TrainingDataBlockLength;
            _trainTryCount = CommonConfig.TrainingTryCount;

            _loader = CommonConfig.LoaderParam.GetLoader();
            _loader.Load();

            _testCtrl = new RController();
            _testCtrl.StartPosition = _startPosition;
            _testCtrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(_loader);

            int totalDataLength = _testCtrl.TotalLength - _startPosition;
            _trainDataLength = (int)(totalDataLength * _testRate);
            _testDataLength = totalDataLength - _trainDataLength;

            if (_trainBlockLength == 0)
                _trainBlockLength = _trainDataLength;

            _ctrlFac = new ControllerFactory(_testCtrl);

            _agentFac = new AgentFactory(_ctrlFac);
            _agentFac.StartPosition = _startPosition;
            _agentFac.TrainDataLength = _trainBlockLength;

            RTrainerData data = new RTrainerData()
            {
                  Data = _loader.ToArray(),
                  StartPosition = _startPosition,
                  Length = _trainDataLength
            };
            RTrainer trainer = new RTrainer(CreateCheckCtrl(), data);

            trainer.TestName = "";

            trainer.RunTestCase();
        }

        private ICheckJob CreateCheckCtrl()
        {
            TrainResultCheckSyncController mainCheckCtrl = new TrainResultCheckSyncController();

            IController testCtrl = _ctrlFac.Get();
            testCtrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(_loader);
            mainCheckCtrl.Add(new NewUpdateControllerJob(TestCaseName));
            mainCheckCtrl.Add(new NewUpdateTestCaseJob()
            {
                TestName = CreateDatetime.ToString() + ": " + TestCaseName,
                TestDescription = TestCaseName + "|" +
                    CommonConfig.LoaderParam.ToString() + "|" +
                    "Offset=" + CommonConfig.BuyOffset + "," + CommonConfig.SellOffset + "|" +
                    "TrnBlk=" + _trainBlockLength + "," + "TrnCnt=" + _trainTryCount
                    ,

                Controller = testCtrl,
                TrainDataLength = _trainDataLength,
                TestDataLength = _testDataLength,
                StartPosition = _startPosition,
            });

            mainCheckCtrl.Add(new TrainDataChangeJob(_agentFac, _startPosition, _trainDataLength, _trainBlockLength, _trainTryCount));
            return mainCheckCtrl;

        }

    }
}
