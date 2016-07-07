using MyProject01.TrainerFactorys;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject01.Controller;
using MyProject01.Util;

namespace MyProject01.TestCases.RateMarketTestCases
{
    abstract class BasicRateMarketTestCase : BasicTestCase, IDescriptionProvider
    {
        protected Trainer _train;
        abstract protected void Init();
        protected DescriptionProviderList _descList;

        public BasicRateMarketTestCase()
        {
            _descList = new DescriptionProviderList();
        }


        public override void RunTest()
        {
            Init();
            _train.RunTestCase();
        }
        public override string TestDescription
        {
            get
            {
                return Description;
            }
        }

        public string Description
        {
            get 
            {
                return _descList.Description;
            }
        }
    }
}
