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
        string _evnPath = _basePath + @"Compiler\R-3.3.3\bin\x64";
#else
        string _evnPath = _basePath + @"Compiler\R-3.3.3\bin\xi386";
#endif
        string _homePath = _basePath + @"Compiler\R-3.3.3";
        string _workPath = _basePath + @"project\Test01\";

        private REngine _engine;
        public RUtility()
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

        private void Test01()
        {

            var loadTestData = Execute("loadTestData").AsFunction();
            var trainStart = Execute("trainStart").AsFunction();
            var testStart = Execute("testStart").AsFunction();

            loadTestData.Invoke();


        }


        private void SetWorkDir(string path)
        {
            string cmd = "setwd('" + path + "')";
            cmd = cmd.Replace('\\', '/');
            Execute(cmd);

        }
        private void Source(string fileName)
        {
            string cmd = "source('" + fileName + "')";
            cmd = cmd.Replace('\\', '/');
            Execute(cmd);
        }

        private SymbolicExpression Execute(string cmd)
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
}
