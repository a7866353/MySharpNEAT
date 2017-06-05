using MyProject01.Controller;
using MyProject01.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.NeuroNetwork
{
    public interface INeuroNetwork
    {
        DataBlock Compute(DataBlock data);

        DataBlock Compute(DataBlock[] data);

        BasicControllerPacker GetPacker();
    }
}
