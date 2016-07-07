using Encog.ML.Data;
using Encog.Neural.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01
{
    abstract class BasicTrainingMethod
    {
        public double ErrorChangeLimit = 0.1;
        public int ErrorChangeTryMaxCount = 20;
        public double ErrorLimit = 0.1;
        public int MaxTryCount = 1000;
        public abstract double TrainNetwork( BasicNetwork network, IMLDataSet trainingSet );
    }
}
