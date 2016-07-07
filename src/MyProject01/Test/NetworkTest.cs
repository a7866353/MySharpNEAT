using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MyProject01.Test
{
    abstract class NetworkTest
    {
        protected LogWriter logger;
        protected LogWriter resultLog;

        public NetworkTest()
        {
            // init log
            logger = new LogWriter("log.txt");
            resultLog = new LogWriter("Results.txt");
        }
        // Save network class to file.
        protected void SaveNetworkToFile(BasicNetwork network, string filename)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename + ".net", FileMode.Create);
            formatter.Serialize(stream, network);
            stream.Flush();
            stream.Close();
        }
        protected BasicNetwork LoadNetwrokFromFile(string filename)
        {
            string filePath = filename + ".net";
            if (File.Exists(filePath) == false)
                return null;

            BasicNetwork network;
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filePath, FileMode.Open);
            network = (BasicNetwork)formatter.Deserialize(stream);
            stream.Close();

            return network;
        }
        protected void LogPrintf(string str)
        {
            logger.WriteLine(str);
            LogFile.WriteLine(str);
        }
        protected void ResultPrintf(string str)
        {
            resultLog.WriteLine(str);
        }

        #region virtual functions

        public abstract BasicNetwork CreateNetworkWithTraining(TestData data, NetworkTestParameter parm);

        public abstract void Run();

        #endregion


        #region protected functions

        protected void Execute(NetworkTestParameter parm)
        {
            logger.SetFileName(parm.name + "_log.txt");
            logger.Reset();
            resultLog.SetFileName(parm.name + "_result.txt");
            resultLog.Reset();

            ResultPrintf(@"------------------------");
            LogPrintf("Test Start!" + parm.name);

            RateDataCreator dataCreator = new RateDataCreator();
            TestData data = dataCreator.GetTestData();

            if (false)
            {
                LogPrintf("[TestData]");
                string dataStr = "";
                for (int i = 0; i < data.TrainInputs.Length; i++)
                {
                    dataStr = i.ToString("D4") + ":";
                    for (int j = 0; j < data.InputSize; j++)
                    {
                        dataStr += data.TrainInputs[i][j].ToString("0.000");
                        dataStr += ", ";
                    }
                    dataStr += "| ";

                    for (int j = 0; j < data.OutputSize; j++)
                    {
                        dataStr += data.TrainIdeaOutputs[i][j].ToString("0.000");
                        dataStr += ", ";
                    }
                    LogPrintf(dataStr);
                }
            }

            BasicNetwork network = null;
            if (true)
            {
                // Create a new trained netwrok
                network = CreateNetworkWithTraining(data, parm);
                // network = CreateNetwork02(data, parm);
            }
            else
            {
                // Create network from file.
                network = LoadNetwrokFromFile(parm.name);
            }

            if (null == network)
                throw (new Exception("Empty Network."));

            // test the neural network
            //   test train data
            foreach (MyTestData dataObj in data.TrainList)
            {
                IMLData res = network.Compute(new BasicMLData(dataObj.Input));
                double[] realArr = new double[data.OutputSize];
                res.CopyTo(realArr, 0, data.OutputSize);
                dataObj.Real = realArr;
            }

            //   test test data
            foreach (MyTestData dataObj in data.TestList)
            {
                IMLData res = network.Compute(new BasicMLData(dataObj.Input));
                double[] realArr = new double[data.OutputSize];
                res.CopyTo(realArr, 0, data.OutputSize);
                dataObj.Real = realArr;
            }


            // Output results.
            int loopCnt = 1;
            ResultPrintf("------------------------");
            ResultPrintf("Neural Network Results:");
            ResultPrintf("ErrorLimit:\t" + parm.ErrorChangeLimit.ToString() + "\tNerul:\t" + parm.hidenLayerRaio.ToString());
            ResultPrintf("TotalError:\t" + data.TestList.ResultError.ToString("000.0000"));
            ResultPrintf(data.ToStringResults());
            LogPrintf("Test end!");
            LogPrintf("");
        }

        #endregion

    }
}
