using MyProject01.Controller;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.TrainerFactorys
{
    abstract class BasicTrainerFactory : IDescriptionProvider
    {
        public string Name;

        public Trainer Get()
        {
            return Create();
        }

        abstract protected Trainer Create();

        abstract public string Description
        {
            get;
        }
    }
}
