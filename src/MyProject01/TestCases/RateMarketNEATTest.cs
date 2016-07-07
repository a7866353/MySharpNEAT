using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.EA.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.NEAT;
using Encog.Neural.Networks.Training;
using Encog.Util;
using Encog.Util.Banchmark;
using Encog.Util.Simple;
using MyProject01.Agent;
using MyProject01.DAO;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MyProject01.Controller;
using MyProject01.Util.DataObject;
using MyProject01.Factorys.TrainerFactorys;

namespace MyProject01.TestCases
{

    class RateMarketNEATTest : BasicTestCase
    {
        public double TestDataRate = 0.75;
        public int DataBlockLength = 300;
        public  int PopulationNum = 50;
        public int DataSoreceType = 0;
        public string RateDataControllerName = "test01";
        public bool IsFWT = false;

        Trainer _train;
        public override void RunTest()
        {
            // init controller
            string controllerName = TestName;
            NetworkController controller = NetworkController.Open(controllerName);
            if (controller == null)
            {
                TradeDecisionController decisionCtrl = new TradeDecisionController();
                if (IsFWT == true)
                    decisionCtrl._inputFormater = new FWTFormater(DataBlockLength);
                else
                    decisionCtrl._inputFormater = new RateDataFormater(DataBlockLength);
                // decisionCtrl._outputConvertor = new TradeStateSwitchConvertor();
                decisionCtrl._outputConvertor = new TradeStateKeepConvertor();
                decisionCtrl.BestNetwork = null;

                controller = NetworkController.Create(controllerName, decisionCtrl);
            }
            

            // init test data
            DataLoader loader;
            if (DataSoreceType == 0)
            {
                loader = new MTDataLoader("USDJPY", DataTimeType.M5);
                // loader.Fillter(new DateTime(2013, 1, 1), DateTime.Now);
            }
            else
            {
                loader = new MTData2Loader(RateDataControllerName);
            }
            /* Loader中的无用处理，改到用InputConver中实现
            if(controller.DataScale == 0)
            {
                controller.DataScale = loader.Scale;
                controller.DataOffset = loader.Offset;
            }
            else
            {
                loader.Normallize(controller.DataOffset, controller.DataScale);
            }
            */
            BasicDataBlock testBlock;

            // testBlock = loader.CreateDataBlock(0, loader.Count, DataBlockLength);
            testBlock = new RateDataBlock(loader, 0, loader.Count, DataBlockLength);



            //
            FirstTrainerFactory factory = new FirstTrainerFactory();
            factory.TestCaseName = TestName;
            // factory.TradeDesionin = controller.GetDecisionController();
            _train = factory.Get();
            // _train.DataList.Add(new TrainingData(testBlock, (int)(testBlock.Length * TestDataRate) ));

            // start trainning
            _train.TestName = TestName;
            // _train.Controller = controller;
            _train.RunTestCase();
        }

    }
}
