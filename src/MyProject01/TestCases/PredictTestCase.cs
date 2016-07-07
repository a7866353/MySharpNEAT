using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using MyProject01.Test;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.TestCases
{
    class PredictTestCase : BasicTestCase
    {
        public PredictTestCase()
        {

        }

        public override void RunTest()
        {
            /*
            if (isParameterSet == false)
                return;

            NetworkTestParameter parm = _network.parm;
            _logger.SetFileName(parm.name + "_log.txt");
            _logger.Reset();
            // resultLog.SetFileName(parm.name + "_result.txt");
            // resultLog.Reset();

            // ResultPrintf(@"------------------------");
            LogPrintf("Test Start!" + parm.name);


            if (false)
            {
                LogPrintf("[TestData]");
                string dataStr = "";
                for (int i = 0; i < _testData.TrainInputs.Length; i++)
                {
                    dataStr = i.ToString("D4") + ":";
                    for (int j = 0; j < _testData.InputSize; j++)
                    {
                        dataStr += _testData.TrainInputs[i][j].ToString("0.000");
                        dataStr += ", ";
                    }
                    dataStr += "| ";

                    for (int j = 0; j < _testData.OutputSize; j++)
                    {
                        dataStr += _testData.TrainIdeaOutputs[i][j].ToString("0.000");
                        dataStr += ", ";
                    }
                    LogPrintf(dataStr);
                }
            }

            // Create a new trained netwrok
            // TODO
            // network = _network.(_testData, parm);
            // network = CreateNetwork02(_testData, parm);
            _network.Init(_testData.InputSize, _testData.OutputSize);
            _network.Training(_testData);
            // test the neural network
            //   test train data
            _network.Compute(_testData.TrainList);

            //   test test data
            _network.Compute(_testData.TestList);

            // Output results.
            // TODO: 
            // ResultPrintf("------------------------");
            // ResultPrintf("Neural Network Results:");
            // ResultPrintf("ErrorLimit:\t" + parm.errorlimit.ToString() + "\tNerul:\t" + parm.hidenLayerRaio.ToString());
            // ResultPrintf("TotalError:\t" + _testData.TestList.ResultError.ToString("000.0000"));
            // ResultPrintf(_testData.ToStringResults());
            // LogPrintf("Test end!");
            LogPrintf("");
            */
        }

    }
}
