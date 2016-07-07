using MyProject01.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Factorys.ControllerFactorys
{
    class NEATRateStateKeepControllerFactory : BasicControllerFactory
    {
        public double Offset = 0;
        public double Scale = 1;
        public NEATRateStateKeepControllerFactory()
        {
            Name = Description;
        }
        protected override NetworkController Create()
        {
            NetworkController controller;

            TradeDecisionController decisionCtrl = new TradeDecisionController();
            decisionCtrl._inputFormater = new RateDataFormater(_inputLength) { Normalizer = new Util.Normalizer(Offset, Scale) };
            decisionCtrl._outputConvertor = new TradeStateKeepConvertor();
            decisionCtrl.BestNetwork = null;

            controller = NetworkController.Create(Name, decisionCtrl);
            return controller;
        }

        public override string Description
        {
            get { return "Rate_StateKeep_" + _inputLength.ToString();}
        }
    }

}
