using MyProject01.Controller;
using MyProject01.NeuroNetwork;
using MyProject01.Util;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.RProjects
{
    class RUtility
    {
        const string _basePath = @"D:\workplace\FANN\Workplace\RTest\rproject\";
#if true
        static string _evnPath = _basePath + @"Compiler\R-3.3.3\bin\x64";
#else
        string _evnPath = _basePath + @"Compiler\R-3.3.3\bin\xi386";
#endif
        static string _homePath = _basePath + @"Compiler\R-3.3.3";
        static string _workPath = _basePath + @"project\Test01\";

        static private REngine _engine;
        static RUtility()
        {
            _evnPath = System.IO.Path.GetFullPath(_evnPath);
            _homePath = System.IO.Path.GetFullPath(_homePath);
            _workPath = System.IO.Path.GetFullPath(_workPath);

            Console.WriteLine("==== R Start! ====");
            Console.WriteLine("EnvPath: " + _evnPath);
            Console.WriteLine("HomePath: " + _homePath);
            Console.WriteLine("WorkplacePath: " + _workPath);
            Console.WriteLine("===========");

            REngine.SetEnvironmentVariables(_evnPath, _homePath);
            // There are several options to initialize the engine, but by default the following suffice:
            _engine = REngine.GetInstance();
            SetWorkDir(_workPath);

            Source("MainFunctions.R");

        }

        public void Test()
        {
            CharacterVector res = Execute("getwd()").AsCharacter();
            res = Execute("getwd()").AsCharacter();



        }

        public double[] TrainAndTest(RateSet[] trainData, RateSet[] testData)
        {
            double[] dArr;

            //=====================================
            // Create Train Data
            dArr = new double[trainData.Length];

            for(int i=0;i<dArr.Length;i++)
            {
                dArr[i] = trainData[i].Open;
            }
            NumericVector trainOpen = _engine.CreateNumericVector(dArr);

            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = trainData[i].High;
            }
            NumericVector trainHigh = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = trainData[i].Low;
            }
            NumericVector trainLow = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = trainData[i].Close;
            }
            NumericVector trainClose = _engine.CreateNumericVector(dArr);

            //=====================================
            // Create Test Data
            dArr = new double[testData.Length];

            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].Open;
            }
            NumericVector testOpen = _engine.CreateNumericVector(dArr);

            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].High;
            }
            NumericVector testHigh = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].Low;
            }
            NumericVector testLow = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].Close;
            }
            NumericVector testClose = _engine.CreateNumericVector(dArr);

            var trainStart = Execute("trainStart").AsFunction();
            var testStart = Execute("testStart").AsFunction();

            var netObject = trainStart.Invoke(new SymbolicExpression[] { trainOpen, trainHigh, trainLow, trainClose });

            var testResult = testStart.Invoke(new SymbolicExpression[] { netObject, testOpen, testHigh, testLow, testClose });

            double[] singleArr = testResult.AsList()["sig"].AsNumeric().ToArray();

            return singleArr;


        }
        static public INeuroNetwork Train(RateSet[] trainData, int startPosition, int length)
        {
            double[] dArr;

            // In R, it is start from 1
            startPosition += 1;

            //=====================================
            // Create Train Data
            dArr = new double[trainData.Length];

            for(int i=0;i<dArr.Length;i++)
            {
                dArr[i] = trainData[i].Open;
            }
            NumericVector trainOpen = _engine.CreateNumericVector(dArr);

            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = trainData[i].High;
            }
            NumericVector trainHigh = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = trainData[i].Low;
            }
            NumericVector trainLow = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = trainData[i].Close;
            }
            NumericVector trainClose = _engine.CreateNumericVector(dArr);


            var trainStart = Execute("trainStart").AsFunction();
            var netObject = trainStart.Invoke(new SymbolicExpression[] 
            { 
                trainOpen, trainHigh, trainLow, trainClose,
                _engine.CreateNumeric(startPosition), 
                _engine.CreateNumeric(length)

            });

            RNetwork net = new RNetwork(
                netObject,
                (int)(netObject.AsList()["netInputNum"].AsNumeric().ToArray()[0]),
                (int)(netObject.AsList()["netOutputNum"].AsNumeric().ToArray()[0])
                );

            return net;


        
        }
        static public double[] Compute(SymbolicExpression net, RateSet[] testData, int startPosition, int length)
        {
            // In R, it is start from 1
            startPosition += 1;

            //=====================================
            // Create Test Data
            double[] dArr = new double[testData.Length];

            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].Open;
            }
            NumericVector testOpen = _engine.CreateNumericVector(dArr);

            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].High;
            }
            NumericVector testHigh = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].Low;
            }
            NumericVector testLow = _engine.CreateNumericVector(dArr);


            for (int i = 0; i < dArr.Length; i++)
            {
                dArr[i] = testData[i].Close;
            }
            NumericVector testClose = _engine.CreateNumericVector(dArr);

            var testStart = Execute("testStart").AsFunction();
            var testResult = testStart.Invoke(new SymbolicExpression[] 
            { 
                net, testOpen, testHigh, testLow, testClose,
                _engine.CreateNumeric(startPosition), 
                _engine.CreateNumeric(length)
            });

            double[] singleArr = testResult.AsList()["sig"].AsNumeric().ToArray();

            return singleArr;
        }

        static private void Test01()
        {

            var loadTestData = Execute("loadTestData").AsFunction();
            var trainStart = Execute("trainStart").AsFunction();
            var testStart = Execute("testStart").AsFunction();

            loadTestData.Invoke();


        }


        static private void SetWorkDir(string path)
        {
            string cmd = "setwd('" + path + "')";
            cmd = cmd.Replace('\\', '/');
            Execute(cmd);

        }
        static private void Source(string fileName)
        {
            string cmd = "source('" + fileName + "')";
            cmd = cmd.Replace('\\', '/');
            Execute(cmd);
        }

        static public SymbolicExpression Execute(string cmd)
        {
            SymbolicExpression res = null;
            try
            {
                res = _engine.Evaluate(cmd);
            }
            catch (Exception e)
            {
                _engine.Evaluate("traceback()");
            }

            return res;
        }
    }

     class RNetwork : INeuroNetwork
    {
        public bool IsInited;
        public int InputNum { get { return _inputNum; } }
        public int OutputNum { get { return _outputNum; } }

        private double[] _result;
        private int _inputNum;
        private int _outputNum;
        private SymbolicExpression _net;
        private RateSet[] _data;

        public RNetwork(SymbolicExpression net, int inputNum, int outputNum)
        {
            _inputNum = inputNum;
            _outputNum = outputNum;
            _net = net;
        }
        public DataBlock Compute(DataBlock data)
        {
            throw new NotImplementedException();
        }

        public DataBlock Compute(DataBlock[] data)
        {
            throw new NotImplementedException();
        }

        public void SetData(RateSet[] data, int startPos, int length)
        {
            IsInited = true;
            _data = data;
            _result = RUtility.Compute(_net, data, startPos, length);
            return;
        }

        public MarketActions GetAction(int position)
        {
            if (_result[position] > 0)
                return MarketActions.Buy;
            else
                return MarketActions.Sell;
        }
    }
}
