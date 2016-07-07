using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    class SectionChangeTrainer : Trainer
    {
        // public long IterationCount = 10;
#if false
        private LogFormater _log = new LogFormater();
        private BasicDataBlock _testDataBlock;
        private BasicDataBlock _trainDataBlock;
        private int _trainDataLength;

        private RateMarketTestDAO _testCaseDAO;

        protected override void PrepareRunnTestCase()
        {
            TrainingData trainData;
            // Check param
            if (Controller == null)
            {
                throw (new Exception("Parm wrong!"));
            }


            // Set test data
            trainData = DataList.GetNext();
            SetDataLength(trainData.DataBlock, trainData.TestLength);

            //Start
            _log = new LogFormater();
            _testCaseDAO = RateMarketTestDAO.GetDAO<RateMarketTestDAO>(TestName, true);
            _testCaseDAO.TestDataStartIndex = _trainDataBlock.Length;
            _testCaseDAO.TotalDataCount = _testDataBlock.Length;
            // _testCaseDAO.TestData = _testDataArray;


            byte[] LastNetData = null;

            RateMarketScore score = new RateMarketScore();
            score.TradeDecisionCtrl = DecisionCtrl;
            score.SetData(_trainDataBlock);

            // train the neural network
            train = NEATUtil.ConstructNEATTrainer(Controller.GetPopulation(), score);

            LogFile.WriteLine(_log.GetTitle());
        }

        protected override void PostItration()
        {
            if ((IterationCount > 0) && (_epoch % IterationCount == 0))
            {
                // set next test data
                trainData = DataList.GetNext();
                SetDataLength(trainData.DataBlock, trainData.TestLength);
                score.SetData(_trainDataBlock);
                train.FinishTraining();
                train = NEATUtil.ConstructNEATTrainer(Controller.GetPopulation(), score);
                LogFile.WriteLine("Change data!");
            }

            if (Epoch % 100 == 1)
            {
                score.Length = 10;
                score.StartIndex++;
                if (score.StartIndex > _trainDataBlock.Length - 10)
                    score.StartIndex = 0;
            }

            CheckReslut();
        }

        private void CheckReslut()
        {
            NEATNetwork episodeNet = (NEATNetwork)train.CODEC.Decode(train.BestGenome);
            Controller.BestNetwork = episodeNet;
            byte[] netData = NetworkToByte(episodeNet);
            if (ByteArrayCompare(netData, LastNetData) == false)
            {
                LastNetData = netData;

            }

            _log.Set(LogFormater.ValueName.Step, _epoch);

            LogFile.WriteLine(_log.GetLog());

        }



        private byte[] NetworkToByte(NEATNetwork network)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, network);

            byte[] res = stream.ToArray();
            stream.Close();
            return res;
        }
        private bool ByteArrayCompare(byte[] arr1, byte[] arr2)
        {
            if (arr1 == null || arr2 == null)
                return false;

            if (arr1.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }
            return true;
        }

        private void SetDataLength(BasicDataBlock dataBlock, int trainLength)
        {
            _testDataBlock = dataBlock;
            _trainDataLength = trainLength;

            // Update test data
            _trainDataBlock = _testDataBlock.GetNewBlock(0, _trainDataLength);

        }
#endif
      
        protected override void PostItration()
        {
            throw new NotImplementedException();
        }

        protected override Encog.ML.EA.Train.TrainEA CreateTrainEA()
        {
            throw new NotImplementedException();
        }
    }
}
