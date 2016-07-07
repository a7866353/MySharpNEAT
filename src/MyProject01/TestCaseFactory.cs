using MyProject01.Test;
using MyProject01.TestCases;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01
{
    class TestCaseFactory
    {
        private MyNetList _netList;
        
        public BasicTestCase[] GetTestCases()
        {
            RateDataCreator dataCreator = new RateDataCreator();
            BasicTestCase testCase;
            LogWriter logger = new LogWriter("Test");
            List<BasicTestCase> testCaseList = new List<BasicTestCase>();
            _netList = MyNetFactory.GetNetList();
            foreach(MyNet net in _netList)
            {
                testCase = new PredictTestCase();

                testCase.SetParmVar(net, dataCreator.GetTestData(), logger);
                testCaseList.Add(testCase);
            }

            return testCaseList.ToArray();
        }

        public BasicTestCase GetRateMarketTest()
        {
            return new RateMarketQLearnTest();
        }
    }
}
