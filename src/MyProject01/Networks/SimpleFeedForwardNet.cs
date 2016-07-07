using Encog.Neural.Networks;
using Encog.Util.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Networks
{
    class SimpleFeedForwardNet : BasicNet
    {
        public override BasicNetwork GetNet(Test.NetworkTestParameter parm)
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(
              parm.InputSize,
              (int)(parm.InputSize * parm.hidenLayerRaio),
              (int)(parm.OutputSize * parm.hidenLayerRaio),
              parm.OutputSize,
              true);

            return network;
        }
    }
}
