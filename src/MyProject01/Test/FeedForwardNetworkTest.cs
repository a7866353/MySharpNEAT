using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Pattern;
using Encog.Util.Simple;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MyProject01.Test
{
    class FeedForwardNetworkTest : NetworkTest
    {
        private string networkFileName = "network.bin";

        public override void Run()
        {
            string testName = "FeedFowardNetwork";
            int num = 1;
            NetworkTestParameter[] parmArr = new NetworkTestParameter[]
            {
                new NetworkTestParameter(testName+num++.ToString("D2"),0.03,  0.01,   500),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.03,  0.1,    500),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.03,  1,      500),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.03,  10,     500),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.03,  100,    500),
                // new NetworkParm(testName+num++.ToString("D2"),0.03,  1000,   500),
                // new NetworkParm("Test04",0.03,  10000,  100),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.01,  0.01,   500),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.01,  0.1,    500),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.01,  1,      500),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.01,  10,     500),
                // new NetworkParm(testName+num++.ToString("D2"),0.01,  100,    500),
                // new NetworkParm(testName+num++.ToString("D2"),0.01,  1000,   500),
                // new NetworkParm("Test08",0.01,  10000,  100),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.005, 0.01,   1000),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.005, 0.1,    1000),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.005, 1,      1000),
                new NetworkTestParameter(testName+num++.ToString("D2"),0.005, 10,     1000),
                 new NetworkTestParameter(testName+num++.ToString("D2"),0.0001, 100,     1000),
               // new NetworkParm(testName+num++.ToString("D2"),0.005, 100,    500),
                // new NetworkParm(testName+num++.ToString("D2"),0.005, 1000,   500),
                // new NetworkParm("Test12",0.005, 10000,  100),
            };
            foreach (NetworkTestParameter parm in parmArr)
            {
                Execute(parm);
            }
        }
       
        /// <summary>
        /// CreateNetwork
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public override BasicNetwork CreateNetworkWithTraining(TestData data, NetworkTestParameter parm)
        {
            LogPrintf("[NetworkCreate]"+parm.ToString());
            // create a neural network, without using a factory
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, data.InputSize));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, (int)(data.InputSize * parm.hidenLayerRaio)));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, data.OutputSize));
            network.Structure.FinalizeStructure();
            network.Reset();

            // create training data
            IMLDataSet trainingSet = new BasicMLDataSet(data.TrainInputs, data.TrainIdeaOutputs);

            // train the neural network
            var train = new ResilientPropagation(network, trainingSet);
            train.ThreadCount = 8;

            int epoch = 1;
            SlidingFilter filter = new SlidingFilter(5);
            do
            {
                train.Iteration();
                // if(epoch%1 == 1 )
                LogPrintf(@"Epoch #" + epoch + @" Error:" + train.Error);
                // Save network each train.
                SaveNetworkToFile(network,parm.name);
                epoch++;
                if (epoch > parm.ErrorChangeRetryCount)
                    break;
                if(filter.Add(train.Error) < parm.ErrorChangeLimit)
                    break;
            } while (true);
            train.FinishTraining();
            SaveNetworkToFile(network, parm.name);

            return network;
        }
        // For SimpleFeedForward
        public BasicNetwork CreateNetwork02(TestData data, NetworkTestParameter parm)
        {
            LogPrintf("[NetworkCreate]" + parm.ToString());
            ResultPrintf("[NetworkCreate]" + parm.ToString());
            // create a network
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                data.InputSize,
                (int)(data.InputSize * parm.hidenLayerRaio),
                (int)(data.OutputSize * 10),
                data.OutputSize,
                true);
            // create training data
            IMLDataSet trainingSet = new BasicMLDataSet(data.TrainInputs, data.TrainIdeaOutputs);

            // train the neural network
            // var train = new Backpropagation(network, trainingSet);
            var train = new ResilientPropagation(network, trainingSet);
            train.ThreadCount = 8;

            int epoch = 1;
            SlidingFilter filter = new SlidingFilter(5);
            do
            {
                train.Iteration();
                // if(epoch%1 == 1 )
                LogPrintf(@"Epoch #" + epoch + @" Error:" + train.Error);
                // Save network each train.
                SaveNetworkToFile(network, parm.name);
                epoch++;
                if (epoch > parm.ErrorChangeRetryCount)
                    break;
                if (filter.Add(train.Error) < parm.ErrorChangeLimit)
                    break;
            } while (true);
            train.FinishTraining();
            SaveNetworkToFile(network, parm.name);

            return network;
        }

        /// <summary>
        /// Use pattern to create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IMLMethod CreateFeedforwardNetwork(TestData data, NetworkTestParameter parm)
        {
            // construct a feedforward type network
            var pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = data.InputSize;
            pattern.OutputNeurons = data.OutputSize;
            pattern.AddHiddenLayer(parm.HidenLayerNum);
            return pattern.Generate();
        }


    }
}
