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
using SharpNeat.Genomes.RbfNeat;
using SharpNeat.Network;
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

    public class NEATNetwork : INeuroNetwork
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



        public DataBlock Compute(DataBlock[] data)
        {
            ISignalArray outputArr = _blockBox.OutputSignalArray;
            _blockBox.ResetState();

            for (int i = 0; i < data.Length;i++ )
            {
                ISignalArray inputArr = _blockBox.InputSignalArray;
                inputArr.CopyFrom(data[i].Data, 0);
                _blockBox.Activate();
                if (_blockBox.IsStateValid == false)
                    return null;

            }
            outputArr.CopyTo(_result.Data, 0);
            return _result;
        }


        public BasicControllerPacker GetPacker()
        {
            throw new NotImplementedException();
        }
    }

    public class NewNormalScore : IPhenomeEvaluator<IBlackBox>
    {
        public AgentFactory _agentFactory;


        private ulong _evalCount;

        public NewNormalScore(AgentFactory agentFactory)
        {
            _agentFactory = agentFactory;
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
            fitness = _agentFactory.Run(new NEATNetwork(phenome));
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

      
        private long _epoch;


        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        string _name;
        int _specieCount;

        AgentFactory _agentFactory;
        int _populationSize;

        private NeatEvolutionAlgorithm<NeatGenome> _ea;
        private IGenomeDecoder<NeatGenome, IBlackBox> _genomeDecoder;

        protected long Epoch
        {
            get { return _epoch; }
        }

        public int PopulationSize
        {
            set { _populationSize = value; }
            get { return _populationSize; }
        }
        public int SpecieCount
        {
            set { _specieCount = value; }
            get { return _specieCount; }
        }
        public int ComplexityThreshold
        {
            set { _complexityThreshold = value; }
            get { return (int)_complexityThreshold; }
        }
        public Trainer(AgentFactory agentFactory)
        {
            _agentFactory = agentFactory;

            _name = "SharpNEAT";
            _populationSize = 512;
            _specieCount = 100;
            _activationScheme = NetworkActivationScheme.CreateAcyclicScheme();
            _complexityRegulationStr = "Absolute";
            _complexityThreshold = 50;
            _description = "SharpNEAT Test";
            _parallelOptions = new ParallelOptions();

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
            _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;
            _neatGenomeParams.InitialInterconnectionsProportion = 0.3;
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
                _agentFactory.BaseController.InputVectorLength, 
                _agentFactory.BaseController.OutputVectorLength, 
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
            NewNormalScore evaluator = new NewNormalScore(_agentFactory);

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
    class RbfTrainer
    {
        private int _inVecLen;
        private int _outVecLen;
        private TrainerContex _context;


        public string TestName = "DefaultTest000";
        public ICheckJob CheckCtrl;


        private long _epoch;


        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        string _name;
        int _specieCount;
        double _rbfMutationSigmaCenter;
        double _rbfMutationSigmaRadius;


        AgentFactory _agentFactory;
        int _populationSize;

        private NeatEvolutionAlgorithm<NeatGenome> _ea;
        private IGenomeDecoder<NeatGenome, IBlackBox> _genomeDecoder;

        protected long Epoch
        {
            get { return _epoch; }
        }

        public RbfTrainer(AgentFactory agentFactory)
        {
            _agentFactory = agentFactory;

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
            _rbfMutationSigmaCenter = 0.1;
            _rbfMutationSigmaRadius = 0.1;

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
            _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;
            _neatGenomeParams.InitialInterconnectionsProportion = 0.1;
            _neatGenomeParams.ConnectionWeightMutationProbability = 0.788;
            _neatGenomeParams.AddConnectionMutationProbability = 0.001;
            _neatGenomeParams.AddConnectionMutationProbability = 0.01;
            _neatGenomeParams.NodeAuxStateMutationProbability = 0.2;

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
            IActivationFunctionLibrary activationFnLibrary = DefaultActivationFunctionLibrary.CreateLibraryRbf(_neatGenomeParams.ActivationFn, _rbfMutationSigmaCenter, _rbfMutationSigmaRadius);
            return new RbfGenomeFactory(
                _agentFactory.BaseController.InputVectorLength,
                _agentFactory.BaseController.OutputVectorLength, 
                activationFnLibrary, _neatGenomeParams
                );
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(_activationScheme);
        }

        protected NeatEvolutionAlgorithm<NeatGenome> CreateTrainEA()
        {
            _context = new TrainerContex();
            // _context.Trainer = this;

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
            NewNormalScore evaluator = new NewNormalScore(_agentFactory);

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
