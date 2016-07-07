using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using MyProject01.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Networks
{
    class ElmanNet : BasicNet
    {
        public override BasicNetwork GetNet(NetworkTestParameter parm)
        {
            var pattern = new ElmanPattern
            {
                ActivationFunction = new ActivationSigmoid(),
                InputNeurons = parm.InputSize,
                OutputNeurons = parm.OutputSize
            };
            pattern.AddHiddenLayer(parm.HidenLayerNum);
            return (BasicNetwork)pattern.Generate();
        }
    }
}
