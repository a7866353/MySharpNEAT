using MyProject01.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.RProjects
{
    class RTrainer : BasicTrainer
    {
        public RTrainer(AgentFactory agentFactory, ICheckJob checkCtrl)
            : base(agentFactory, checkCtrl)
        {

        }

        protected override void StartTestCase()
        {
            throw new NotImplementedException();
        }
    }
}
