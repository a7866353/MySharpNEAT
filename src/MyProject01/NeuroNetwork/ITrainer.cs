using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.NeuroNetwork
{
    interface  ITrainer
    {
        void Initial();
        void RunTestCase();
        long Epoch { get; }
        string Name { get; }

    }
}
