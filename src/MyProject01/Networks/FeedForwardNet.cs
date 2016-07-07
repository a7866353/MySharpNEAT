using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using MyProject01.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Networks
{
    class FeedForwardNet : BasicNet
    {

        public override BasicNetwork GetNet(NetworkTestParameter parm)
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, parm.InputSize));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, (int)(parm.InputSize * parm.hidenLayerRaio)));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, (int)(parm.InputSize * parm.hidenLayerRaio)));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, parm.OutputSize));
            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }
    }

}
