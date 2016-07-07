using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyProject01.Agent;
using MyProject01.Test;
using MyProject01.TrainingMethods;
using MyProject01.Networks;
using System.Threading.Tasks;
using MyProject01.Reinforcement;

namespace MyProject01.TestCases
{
    class RateMarketQLearnTest : BasicTestCase
    {

        public override void RunTest()
        {

            // Init network
            NetworkTestParameter parm = new NetworkTestParameter("QLearn", 0.5, 2, 10);
            // network = new MyNet(new FeedForwardNet(), new ResilientPropagationTraining(), parm);
            parm.MaxTryCount = 1000;
            MyNet network = new MyNet(new FeedForwardNet(), new BackpropagationTraining(), parm);
            network.Init(30, 3);

            List<RateMarketAgent> agentList = new List<RateMarketAgent>();
            for(int i=0;i<1;i++)
            {
                QLearn qlearn = new QLearn(network);
                // TODO
                // RateMarketAgent agent = new RateMarketAgent(qlearn);
                // agentList.Add(agent);
            }

            // Parallel.ForEach(agentList, currentAgent => currentAgent.TakeAction());
        }
    }
}
