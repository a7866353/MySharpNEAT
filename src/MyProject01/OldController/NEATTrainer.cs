using MyProject01.Agent;
using MyProject01.Controller.Jobs;
using MyProject01.DAO;
using MyProject01.ExchangeRateTrade;
using MyProject01.NeuroNetwork;
using MyProject01.Util;
using MyProject01.Util.DataObject;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MyProject01.Controller
{

    class NEATNetwork : INeuroNetwork
    {
        private IBlackBox _blockBox;
        private DataBlock _result;
        public NEATNetwork(IBlackBox blockBox)
        {
            _blockBox = blockBox;
            _result = new DataBlock(_blockBox.OutputCount);
        }

        public DataBlock Compute(DataBlock data)
        {
            ISignalArray inputArr = _blockBox.InputSignalArray;
            ISignalArray outputArr = _blockBox.OutputSignalArray;
            
            _blockBox.ResetState();
            inputArr.CopyFrom(data.Data, 0);
            _blockBox.Activate();
            if (_blockBox.IsStateValid == false)
                return null;
            outputArr.CopyTo(_result.Data, 0);

            return _result;
        }

    }
    public class NewNormalScore : IPhenomeEvaluator<IBlackBox>
    {
        public ControllerFactory _ctrlFactory;
        public int StartPosition = 50000;
        public int TrainDataLength
        {
            get { return _trainDataLength; }
        }

        private int _trainDataLength;
        private ulong _evalCount;

        public NewNormalScore(ControllerFactory ctrlFactory, int trainDataLength)
        {
            _ctrlFactory = ctrlFactory;
            _trainDataLength = trainDataLength;
            _evalCount = 0;
        }


        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return false; }
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            double fitness = 0;

            _evalCount++;
            IController ctrl = _ctrlFactory.Get();

            ctrl.UpdateNetwork(new NEATNetwork(phenome));
            LearnRateMarketAgent agent = new LearnRateMarketAgent(ctrl);
            agent.SetRange(StartPosition, StartPosition + _trainDataLength);

            while (true)
            {
                if (agent.IsEnd == true)
                    break;

                agent.DoAction();
                agent.Next();
                if (agent.IsEnd == true)
                    break;
            }
            //            System.Console.WriteLine("S: " + agent.CurrentValue);
            double score = agent.CurrentValue - agent.InitMoney;
            // System.Console.WriteLine("S: " + score);
            // return score;
            _ctrlFactory.Free(ctrl);


            fitness = agent.CurrentValue;
            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
            // throw new NotImplementedException();
        }
    }

    class Trainer
    {
        private int _inVecLen;
        private int _outVecLen;
        private TrainerContex _context;
  
        
        public string TestName = "DefaultTest000";
        public ICheckJob CheckCtrl;

        private double _testRate = 0.7;
        private int _startPosition = 50000;
        private int _trainBlockLength = 4096;
        
        private long _epoch;
        private int _trainDataLength;
        private int _testDataLength;


        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        string _name;
        int _specieCount;

        ControllerFactory _ctrlFactory;
        int _populationSize;

        private NeatEvolutionAlgorithm<NeatGenome> _ea;
        private IGenomeDecoder<NeatGenome, IBlackBox> _genomeDecoder;

        protected long Epoch
        {
            get { return _epoch; }
        }

        public Trainer(int inVectorLength, int outVectorLength)
        {
            _inVecLen = inVectorLength;
            _outVecLen = outVectorLength;
        }
        public Trainer(ControllerFactory ctrlFactory)
        {
            _ctrlFactory = ctrlFactory;

            _trainDataLength = _ctrlFactory.BaseController.TotalLength;

            _name = "SharpNEAT";
            _populationSize = CommonConfig.PopulationSize;
            _specieCount = 10;
            _activationScheme = NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1);
            _complexityRegulationStr = "Absolute";
            _complexityThreshold = 10;
            _description = "SharpNEAT Test";
            _parallelOptions = new ParallelOptions();

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
            // _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;
        }

        public void Initialize(string name, XmlElement xmlConfig)
        {
            _name = name;
            _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
            _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
            _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
            _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
            _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
            _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");
            _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
            _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;
            _neatGenomeParams.InitialInterconnectionsProportion = 0.1;


        }
        public void RunTestCase()
        {
            LogFile.WriteLine(@"Beginning training...");

            _ea = CreateTrainEA();
            _epoch = 1;

            _ea.UpdateEvent += train_UpdateEvent;
            _ea.StartContinue();
        }

        void train_UpdateEvent(object sender, EventArgs e)
        {
            _context.Epoch = Epoch;
            _context.CurrentDate = DateTime.Now;
            _context.BestNetwork = new NEATNetwork(_genomeDecoder.Decode(_ea.CurrentChampGenome));
            _context.Fitness = _ea.CurrentChampGenome.EvaluationInfo.Fitness;
            CheckCtrl.Do(_context);

            // double fitness = _ea.CurrentChampGenome.EvaluationInfo.Fitness;
            // LogFile.WriteLine(_epoch.ToString() + ": " + fitness.ToString());
           _epoch++;
        }
        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(
                _ctrlFactory.BaseController.InputVectorLength, 
                _ctrlFactory.BaseController.OutputVectorLength, 
                _neatGenomeParams
                );
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(_activationScheme);
        }

        protected NeatEvolutionAlgorithm<NeatGenome> CreateTrainEA()
        {
            _context = new TrainerContex();
            _context.Trainer = this;

            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(_populationSize, 0);

            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, _parallelOptions);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create the evolution algorithm.
            NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

            // Create IBlackBox evaluator.
            NewNormalScore evaluator = new NewNormalScore(_ctrlFactory, Math.Min(_trainDataLength, _trainBlockLength))
            {
                StartPosition = _startPosition
            };

            // Create genome decoder.
            _genomeDecoder = CreateGenomeDecoder();

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            IGenomeListEvaluator<NeatGenome> innerEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(_genomeDecoder, evaluator, _parallelOptions);

            // Wrap the list evaluator in a 'selective' evaulator that will only evaluate new genomes. That is, we skip re-evaluating any genomes
            // that were in the population in previous generations (elite genomes). This is determined by examining each genome's evaluation info object.
            IGenomeListEvaluator<NeatGenome> selectiveEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(
                                                                                    innerEvaluator,
                                                                                    SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());
            // Initialize the evolution algorithm.
            ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);

            return ea;
        }
    }

    class LogFormater
    {
        private double[] _valueArray;
        public enum ValueName
        {
            Step = 0,
            TrainScore,
            UnTrainScore,
        }

        public LogFormater()
        {
            _valueArray = new double[Enum.GetValues(typeof(ValueName)).Length];
        }

        public string GetTitle()
        {
            string title = "";
            string[] arr = Enum.GetNames(typeof(ValueName));
            for (int i = 0; i < arr.Length; i++)
                title += arr[i].ToString() + "\t";
            return title;
        }

        public string GetLog()
        {
            string resStr = "";
            for (int i = 0; i < _valueArray.Length; i++)
                resStr += _valueArray[i].ToString("G6") + "    \t";
            return resStr;
        }

        public void Set(ValueName name, double v)
        {
            this._valueArray[(int)name] = v;
        }

    }

    class TrainingData
    {
        private BasicDataBlock _testDataBlock;
        private BasicDataBlock _trainDataBlock;
        public BasicDataBlock TestDataBlock
        {
            get { return _testDataBlock; }
        }
        public BasicDataBlock TrainDataBlock
        {
            get { return _trainDataBlock; }
        }

        public TrainingData(BasicDataBlock testBlock, BasicDataBlock trainBlock)
        {
            _testDataBlock = testBlock;
            _trainDataBlock = trainBlock;
        }

    }
    class TrainDataList : List<TrainingData>
    {
        private Random _rand;

        public TrainDataList()
        {
            _rand = new Random();
        }
        public TrainingData GetNext()
        {
            return this[_rand.Next(Count)];
        }
    }
}
