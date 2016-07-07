using Encog.Neural.NEAT;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Factorys.PopulationFactorys
{
    abstract class BasicPopulationFactory : IDescriptionProvider
    {
        public int PopulationNumber = 100;

        public NEATPopulation Get(int inputVectoryNumber, int outputVectoryNumber)
        {
            return Create(inputVectoryNumber, outputVectoryNumber);
        }

        abstract protected NEATPopulation Create(int inputVectoryNumber, int outputVectoryNumber);


        public string Description
        {
            get { return "Pop" + PopulationNumber; }
        }
    }
}
