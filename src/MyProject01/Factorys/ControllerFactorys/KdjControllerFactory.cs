using MyProject01.Controller;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Factorys.ControllerFactorys
{
    class KdjControllerFactory : BasicControllerFactory
    {
        protected override NetworkController Create()
        {
            NetworkController controller;

            TradeDecisionController decisionCtrl = new TradeDecisionController();
            decisionCtrl._inputFormater = new KDJOnlyFormater(_inputLength);
            decisionCtrl._outputConvertor = new TradeStateKeepConvertor();
            decisionCtrl.BestNetwork = null;

            controller = NetworkController.Create(Name, decisionCtrl);
            return controller;
        }

        public override string Description
        {
            get { return "KDJ_" + _inputLength.ToString(); }
        }
    }

    class KdjNormControllerFactory : BasicControllerFactory
    {
        public BasicDataBlock NormalyzeData;
        public double MiddleValue = 0.5;
        public double Margin = 0.2;
        protected override NetworkController Create()
        {
            NetworkController controller;

            TradeDecisionController decisionCtrl = new TradeDecisionController();

            KDJFormater form = new KDJFormater(_inputLength);
            form.Normilize(NormalyzeData, MiddleValue, Margin);

            decisionCtrl._inputFormater = form;
            decisionCtrl._outputConvertor = new TradeStateKeepConvertor();
            decisionCtrl.BestNetwork = null;

            controller = NetworkController.Create(Name, decisionCtrl);
            return controller;
        }

        public override string Description
        {
            get { return "KDJNorm_" + _inputLength.ToString(); }
        }
    }

}
