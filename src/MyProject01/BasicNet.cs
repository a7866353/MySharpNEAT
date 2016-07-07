using Encog.Neural.Networks;
using MyProject01.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01
{
    abstract class BasicNet
    {
        public abstract BasicNetwork GetNet(NetworkTestParameter parm); 
    }
}
