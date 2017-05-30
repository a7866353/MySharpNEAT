using MyProject01.Controller;
using MyProject01.NeuroNetwork;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.RProjects
{
    class RTrainerData
    {
        public RateSet[] Data;
        public int StartPosition;
        public int Length;
    }
    class RTrainer : BasicTrainer
    {
        private RTrainerData _data;
        public RTrainer(ICheckJob checkCtrl, RTrainerData data)
            : base(checkCtrl)
        {
            _data = data;
        }

        protected override void StartTestCase()
        {
            INeuroNetwork net = RUtility.Train(_data.Data, _data.StartPosition, _data.Length);
            this.UpdateTrainInfo(net, 1);
        }
    }
}
