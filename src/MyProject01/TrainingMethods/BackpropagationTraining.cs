using Encog.ML.Data;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Lma;
using Encog.Neural.Networks.Training.Propagation.Back;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.TrainingMethods
{
    class BackpropagationTraining : BasicTrainingMethod
    {
        public override double TrainNetwork(BasicNetwork network, IMLDataSet trainingSet)
        {
            double targetError = 0;
            for (int i = 0; i < trainingSet[0].Ideal.Count; i++)
                targetError += Math.Abs( trainingSet[0].Ideal[i]);
            targetError /= trainingSet[0].Ideal.Count;
            targetError *= this.ErrorChangeLimit; 
            // train the neural network
            ICalculateScore score = new TrainingSetScore(trainingSet);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
            
            var trainMain = new Backpropagation(network, trainingSet);
            trainMain.ThreadCount = 8;
            var stop = new StopTrainingStrategy(targetError, ErrorChangeTryMaxCount);
            // trainMain.AddStrategy(new Greedy());
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);

            int epoch = 0;
            while (!stop.ShouldStop())
            {
                trainMain.Iteration();
                // LogFile.WriteLine("Epoch #" + epoch.ToString("D4") + " Error:" + trainMain.Error.ToString("G6") + "\tTarget: " + targetError.ToString("G6"));
                //SaveNetworkToFile(network, testName);
                epoch++;
                if (epoch > MaxTryCount)
                    break;
            }
            return trainMain.Error;
        }
    }
}
