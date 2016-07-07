using MyProject01.Networks;
using MyProject01.TrainingMethods;
using MyProject01.TestParameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01
{
    class MyNetFactory
    {
        static BasicNet[] netArr = new BasicNet[]
        {
            new ElmanNet(),
            new FeedForwardNet(),
            new SimpleFeedForwardNet(),
        };

        static BasicTrainingMethod[] methodArr = new BasicTrainingMethod[]
        {
            new BackpropagationTraining(),
            new LevenTraining(),
        };

        static BasicTestParameterSet[] paramSetArr = new BasicTestParameterSet[]
        {
            new NormalParameterSet(""),
        };

        static public MyNetList GetNetList()
        {
            MyNetList netList = new MyNetList();
            foreach(BasicNet net in netArr)
            {
                foreach(BasicTrainingMethod method in methodArr)
                {
                    foreach(BasicTestParameterSet parmSet in paramSetArr)
                    {
                    netList.AddNet(net, method, parmSet);
                    }
                }
            }

            return netList;
        }
        
    
    }
}
