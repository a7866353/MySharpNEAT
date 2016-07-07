using MyProject01.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject01.Util;

namespace MyProject01.Factorys.TrainingDataFactorys
{
    abstract class BasicTrainingDataFactory : IDescriptionProvider
    {
        public int DataBlockLength;
        public TrainingData Get()
        {
            return Create();
        }

        abstract protected TrainingData Create();

        abstract public string Description
        {
            get;
        }
    }
}
