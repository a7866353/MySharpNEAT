using Encog.Neural.NEAT;
using MyProject01.Agent;
using MyProject01.DAO;
using MyProject01.ExchangeRateTrade;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject01.Controller.Jobs;
using MyProject01.Util;
using MyProject01.Factorys.PopulationFactorys;
using MyProject01.Util.DllTools;
using MyProject01.NeuroNetwork;

namespace MyProject01.Controller
{
    class NormalTrainer : Trainer
    {
        private TrainerContex _context;
        private RateMarketTestDAO _testDAO;

        public ITradeDesisoin DecisionCtrl;
        public ICalculateScore ScoreCtrl;
        public BasicDataBlock TrainDataBlock;
        public BasicPopulationFactory PopulationFacotry;
        protected override void PostItration()
        {
            _context.Epoch = Epoch;
            CheckCtrl.Do(_context);

            // Update Test Data
                // Nothing
        }

        protected override TrainEA CreateTrainEA()
        {
            _context = new TrainerContex();
            _context._trainDataBlock = TrainDataBlock;

            // train the neural network
            TrainEA train = NEATUtil.ConstructNEATTrainer(
                PopulationFacotry.Get(DecisionCtrl.NetworkInputVectorLength, DecisionCtrl.NetworkOutputVectorLenth),
                ScoreCtrl);
            _context.trainEA = train;

            return train;
        }
    }

    public class NormalScore : ICalculateScore
    {
        private BasicDataBlock _dataBlock;
        public int StartIndex;
        public int Length;
        public ITradeDesisoin TradeDecisionCtrl;
        public BasicDataBlock dataBlock
        {
            get { return _dataBlock; }
            set
            {
                _dataBlock = value;
                StartIndex = 0;
                Length = _dataBlock.BlockCount;
            }
        }

        public void SetData(BasicDataBlock dataBlock)
        {
            _dataBlock = dataBlock;
            StartIndex = 0;
            Length = _dataBlock.BlockCount;
        }

        public bool ShouldMinimize
        {
            get { return false; }
        }
        public bool RequireSingleThreaded
        {
            get { return false; }
        }

        public double CalculateScore(IMLMethod network)
        {
            RateMarketAgent agent = new RateMarketAgent(_dataBlock.GetNewBlock(StartIndex, Length));
            ITradeDesisoin decisionCtrl = TradeDecisionCtrl.Clone();
#if true
            // NetworkDllTools dllNet = new NetworkDllTools((NEATNetwork)network);
            NetworkFloatDllTools dllNet = new NetworkFloatDllTools((NEATNetwork)network);
            decisionCtrl.UpdateNetwork((INeuroNetwork)dllNet);
#else
            decisionCtrl.UpdateNetwork((INeuroNetwork)network);
#endif
            TradeController tradeCtrl = new TradeController(agent, decisionCtrl);
            while (true)
            {
                if (agent.CurrentRateValue > 0)
                {
                    // Get Action Value
                    tradeCtrl.DoAction();
                }
                if (agent.IsEnd == true)
                    break;
            }
            //            System.Console.WriteLine("S: " + agent.CurrentValue);
            double score = agent.CurrentValue - agent.InitMoney;
            // System.Console.WriteLine("S: " + score);
            // return score;
            return agent.CurrentValue;
        }

    }

    public class ReduceLossScore : ICalculateScore
    {
        private BasicDataBlock _dataBlock;
        public int StartIndex;
        public int Length;
        public ITradeDesisoin TradeDecisionCtrl;
        public BasicDataBlock dataBlock
        {
            get { return _dataBlock; }
            set
            {
                _dataBlock = value;
                StartIndex = 0;
                Length = _dataBlock.BlockCount;
            }
        }

        public void SetData(BasicDataBlock dataBlock)
        {
            _dataBlock = dataBlock;
            StartIndex = 0;
            Length = _dataBlock.BlockCount;
        }

        public bool ShouldMinimize
        {
            get { return false; }
        }
        public bool RequireSingleThreaded
        {
            get { return false; }
        }

        public double CalculateScore(IMLMethod network)
        {
            RateMarketAgent agent = new RateMarketAgent(_dataBlock.GetNewBlock(StartIndex, Length));
            ITradeDesisoin decisionCtrl = TradeDecisionCtrl.Clone();
            decisionCtrl.UpdateNetwork((INeuroNetwork)network);
            TradeController tradeCtrl = new TradeController(agent, decisionCtrl);
            while (true)
            {
                if (agent.CurrentRateValue > 0)
                {
                    // Get Action Value
                    tradeCtrl.DoAction();
                }
                if (agent.IsEnd == true)
                    break;
            }
            double score =
                tradeCtrl.TradeLog.GrossLoss * 10  +
                tradeCtrl.TradeLog.MaxLoss * 10 +
                (tradeCtrl.Agent.CurrentValue - tradeCtrl.Agent.InitMoney);

            return score;
        }

    }

    class CNeatNetwork : INeuroNetwork
    {
        public CNeatNetwork(NEATNetwork network)
        {
            
        }

        public DataBlock Compute(Encog.ML.Data.DataBlock input)
        {
            throw new NotImplementedException();
        }

        public int InputCount
        {
            get { throw new NotImplementedException(); }
        }

        public int OutputCount
        {
            get { throw new NotImplementedException(); }
        }
    }
}
