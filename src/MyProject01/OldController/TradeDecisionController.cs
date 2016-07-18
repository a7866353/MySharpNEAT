using MyProject01.NeuroNetwork;
using MyProject01.Util;
using MyProject01.Util.DataObject;
using MyProject01.Util.DllTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    public enum MarketActions
    {
        Nothing = 0,
        Buy,
        Sell,
        Close,
        Init,
    };
    public enum InputDataFormaterType
    {
        None = 0,
        FWT = 1,
        Rate = 2,
    }
    class InputDataFormaterFactor
    {
        static public IInputDataFormater Create(InputDataFormaterType type, int inputDataLength)
        {
            IInputDataFormater result;
            switch (type)
            {
                case InputDataFormaterType.FWT:
                    result = new FWTFormater(inputDataLength);
                    break;
                case InputDataFormaterType.Rate:
                    result = new RateDataFormater(inputDataLength);
                    break;
                default:
                    throw (new Exception("Input parm error!"));
            }

            return result;
        }
    }
    class FWTFormater : IInputDataFormater
    {
        private int _inputDataLength;
        public double[] Buffer;
        private double[] _tempBuffer;

        public FWTFormater(int inputDataLength)
        {
            _inputDataLength = inputDataLength;
            Buffer = new double[_inputDataLength];
            _tempBuffer = new double[_inputDataLength];
        }
        public DataBlock Convert(double[] rateDataArray)
        {
            if (Buffer.Length != rateDataArray.Length)
            {
                throw (new Exception("Input Param Error!"));
            }

            DllTools.FTW_4(rateDataArray, Buffer, _tempBuffer);


            MyProject01.Util.DataNormallizer adj = new MyProject01.Util.DataNormallizer();
            Buffer[0] = Buffer[1];
            adj.Set(Buffer, 0, Buffer.Length);
            adj.DataValueAdjust(-0.01, 0.01);

            return new DataBlock(Buffer, false);
        }


        public int NetworkInputLength
        {
            get { return _inputDataLength; }
        }


        public IInputDataFormater Clone()
        {
            FWTFormater ret = new FWTFormater(_inputDataLength);
            return ret;
        }

        public int InputDataLength
        {
            get { return _inputDataLength; }
        }

        public int ResultDataLength
        {
            get { return _inputDataLength; }
        }


        public string GetDecs()
        {
            return "FWT";
        }
    }

    [Serializable]
    class FWTNormFormater : IInputDataFormater
    {
        private int _inputDataLength;
        public double[] Buffer;
        public Normalizer[] NormalizerArray;
        private double[] _tempBuffer;

        public FWTNormFormater(int inputDataLength)
        {
            _inputDataLength = inputDataLength;
            Buffer = new double[_inputDataLength];
            _tempBuffer = new double[_inputDataLength];


            // Init normalizers
            NormalizerArray = new Normalizer[NetworkInputLength];
            for(int i=0;i<NormalizerArray.Length;i++)
            {
                NormalizerArray[i] = new Normalizer(0, 1);
            }

        }

        public void Normilize(BasicDataBlock dataBlock, double middleValue, double margin)
        {
            ArrayDataNormalizeAnalyzer norm = new ArrayDataNormalizeAnalyzer(new NormalizeAnalyzer(0.5, 0.1 / 2, 0.1));
            double[] buffer = new double[dataBlock.BlockLength];
            dataBlock.Reset();
            dataBlock.Copy(buffer);
            Convert(buffer);
            norm.Init(this.Buffer);

            while (true)
            {
                if (dataBlock.Next() == false)
                    break;
                dataBlock.Copy(buffer);
                Convert(buffer);
                norm.Set(this.Buffer);
            }

            NormalizerArray = norm.NromalizerArray;
        }

        public DataBlock Convert(double[] rateDataArray)
        {
            if (Buffer.Length != rateDataArray.Length)
            {
                throw (new Exception("Input Param Error!"));
            }

            DllTools.FTW_4(rateDataArray, Buffer, _tempBuffer);


            for (int i = 0; i < Buffer.Length;i++ )
            {
                Buffer[i] = NormalizerArray[i].Convert(Buffer[i]);
            }

            return new DataBlock(Buffer, false);
        }


        public int NetworkInputLength
        {
            get { return _inputDataLength; }
        }


        public IInputDataFormater Clone()
        {
            FWTNormFormater ret = new FWTNormFormater(_inputDataLength);
            ret.NormalizerArray = NormalizerArray;
            return ret;
        }

        public int InputDataLength
        {
            get { return _inputDataLength; }
        }

        public int ResultDataLength
        {
            get { return _inputDataLength; }
        }


        public string GetDecs()
        {
            return "FWT_Norm";
        }
    }
    [Serializable]
    class RateDataFormater : IInputDataFormater
    {
        private int _inputDataLength;
        public Normalizer Normalizer = new Normalizer(0, 1);
        public RateDataFormater(int inputDataLength)
        {
            _inputDataLength = inputDataLength;
        }
        public DataBlock Convert(double[] rateDataArray)
        {
            for (int i = 0; i < rateDataArray.Length;i++ )
            {
                rateDataArray[i] = Normalizer.Convert(rateDataArray[i]);
            }
            return new DataBlock(rateDataArray, false);
        }
        public int NetworkInputLength
        {
            get { return _inputDataLength; }
        }


        public IInputDataFormater Clone()
        {
            return (IInputDataFormater)MemberwiseClone();
        }

        public int InputDataLength
        {
            get { return _inputDataLength; }
        }


        public int ResultDataLength
        {
            get { return _inputDataLength; }
        }


        public string GetDecs()
        {
            return "RateAdjust";
        }
    }

    public enum OutputDataConvertorType
    {
        None = 0,
        StateKeep = 1,
        Switch = 2,
    }
    class OutputDataConvertorFactory
    {
        static public IOutputDataConvertor Create(OutputDataConvertorType type)
        {
            IOutputDataConvertor result;
            switch (type)
            {
                case OutputDataConvertorType.Switch:
                    result = new TradeStateSwitchConvertor();
                    break;
                default:
                    throw (new Exception("Input parm error!"));
            }

            return result;
        }
    }

    [Serializable]
    class TradeStateSwitchConvertor : IOutputDataConvertor
    {
        public MarketActions Convert(DataBlock output)
        {
            MarketActions currentAction;
            // Choose an action
            int maxActionIndex = 0;
            for (int i = 1; i < output.Length; i++)
            {
                if (output[maxActionIndex] < output[i])
                    maxActionIndex = i;
            }

            // Do action
            switch (maxActionIndex)
            {
                case 0:
                    currentAction = MarketActions.Nothing;
                    break;
                case 1:
                    currentAction = MarketActions.Buy;
                    break;
                case 2:
                    currentAction = MarketActions.Sell;
                    break;
                default:
                    currentAction = MarketActions.Nothing;
                    break;
            }
            return currentAction;
        }


        public int NetworkOutputLength
        {
            get { return 3; }
        }


        public IOutputDataConvertor Clone()
        {
            return (IOutputDataConvertor)MemberwiseClone();
        }


        public string GetDesc()
        {
            return "StateSwitch";
        }
    }
    [Serializable]
    class TradeStateKeepConvertor : IOutputDataConvertor
    {
        private MarketActions _lastAction = MarketActions.Nothing;
        public MarketActions Convert(DataBlock output)
        {
            MarketActions currentAction;
            // Choose an action
            int maxActionIndex = 0;
            for (int i = 1; i < output.Length; i++)
            {
                if (output[maxActionIndex] < output[i])
                    maxActionIndex = i;
            }

            // Do action
            switch (maxActionIndex)
            {
                case 0:
                    if( _lastAction == MarketActions.Buy)
                        currentAction = MarketActions.Sell;
                    else if( _lastAction == MarketActions.Sell)
                        currentAction = MarketActions.Buy;
                    else
                        currentAction = MarketActions.Nothing;
                    break;
                case 1:
                    currentAction = MarketActions.Buy;
                    break;
                case 2:
                    currentAction = MarketActions.Sell;
                    break;
                default:
                    currentAction = MarketActions.Nothing;
                    break;
            }

            _lastAction = currentAction;
            return currentAction;
        }


        public int NetworkOutputLength
        {
            get { return 3; }
        }


        public IOutputDataConvertor Clone()
        {
            return (IOutputDataConvertor)MemberwiseClone();
        }


        public string GetDesc()
        {
            return "StateKeep";
        }
    }
    [Serializable]
    class TradeStateKeepWithCloseOrderConvertor : IOutputDataConvertor
    {
        private MarketActions _lastAction = MarketActions.Nothing;
        public MarketActions Convert(DataBlock output)
        {
            MarketActions currentAction;
            // Choose an action
            int maxActionIndex = 0;
            for (int i = 1; i < output.Length; i++)
            {
                if (output[maxActionIndex] < output[i])
                    maxActionIndex = i;
            }

            // Do action
            switch (maxActionIndex)
            {
                case 0:
                    currentAction = MarketActions.Close;
                    break;
                case 1:
                    currentAction = MarketActions.Buy;
                    break;
                case 2:
                    currentAction = MarketActions.Sell;
                    break;
                default:
                    currentAction = MarketActions.Close;
                    break;
            }

            _lastAction = currentAction;
            return currentAction;
        }


        public int NetworkOutputLength
        {
            get { return 3; }
        }


        public IOutputDataConvertor Clone()
        {
            return (IOutputDataConvertor)MemberwiseClone();
        }


        public string GetDesc()
        {
            return "StateKeepWithClose";
        }
    }

    public interface ITradeDesisoin
    {
        MarketActions GetAction(double[] input);
        ITradeDesisoin Clone();
        void UpdateNetwork(INeuroNetwork network);
        int InputDataLength { get; }
        int NetworkInputVectorLength { get; }
        int NetworkOutputVectorLenth { get; }
    }

    [Serializable]
    class TradeDecisionController : ITradeDesisoin
    {
        public IInputDataFormater _inputFormater;
        public IOutputDataConvertor _outputConvertor;
        public INeuroNetwork BestNetwork;

        public MarketActions GetAction(double[] input)
        {
            if (BestNetwork == null)
                return MarketActions.Nothing;
            DataBlock inData = _inputFormater.Convert(input);

            DataBlock output = BestNetwork.Compute(inData);
            MarketActions result = _outputConvertor.Convert(output);
            return result;
        }
        public ITradeDesisoin Clone()
        {
            TradeDecisionController newCtrl = new TradeDecisionController();
            newCtrl.BestNetwork = BestNetwork;
            newCtrl._inputFormater = _inputFormater.Clone();
            newCtrl._outputConvertor = _outputConvertor.Clone();

            return newCtrl;
        }


        public void UpdateNetwork(INeuroNetwork network)
        {
            BestNetwork = network;
        }


        public int InputDataLength
        {
            get { return _inputFormater.InputDataLength; }
        }


        public int NetworkInputVectorLength
        {
            get { return _inputFormater.ResultDataLength; }
        }

        public int NetworkOutputVectorLenth
        {
            get { return _outputConvertor.NetworkOutputLength; }
        }
    }
}
