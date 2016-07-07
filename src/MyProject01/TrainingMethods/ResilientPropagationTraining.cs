using Encog.ML.Data;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Banchmark;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.TrainingMethods
{
    class ResilientPropagationTraining : BasicTrainingMethod
    {

        public override double TrainNetwork(Encog.Neural.Networks.BasicNetwork network, Encog.ML.Data.IMLDataSet trainingSet)
        {
            double idealSum = 0;
            int torenteCount = 0;
            for (int i = 0; i < trainingSet[0].Ideal.Count; i++)
                idealSum += Math.Abs(trainingSet[0].Ideal[i]);
            idealSum /= trainingSet[0].Ideal.Count;
            idealSum *= ErrorChangeLimit;

            var trainMain = new ResilientPropagation(network, trainingSet);
            trainMain.ThreadCount = 16;

            ICalculateScore score = new TrainingSetScore(trainingSet);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);

            var stop = new StopTrainingStrategy(idealSum, ErrorChangeTryMaxCount);
            // trainMain.AddStrategy(new Greedy());
            // trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);

            int epoch = 0;
            while (true)
            {
                trainMain.Iteration();
                // LogFile.WriteLine("Training Epoch #" + epoch + " Error:" + trainMain.Error);
                // SaveNetworkToFile(network, testName);
                if (trainMain.Error < idealSum)
                {
                    torenteCount++;
                    if (torenteCount >= ErrorChangeTryMaxCount)
                        break;
                }
                else
                {
                    torenteCount = 0;
                }
                epoch++;


            }
            return trainMain.Error;

        }
    }
}
