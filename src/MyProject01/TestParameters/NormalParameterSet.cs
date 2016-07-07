using MyProject01.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.TestParameters
{
    class NormalParameterSet : BasicTestParameterSet
    {
        public NormalParameterSet(String testCaseName)
        {
            this._parmArr = new NetworkTestParameter[]
            {
                new NetworkTestParameter("",0.03,  0.01,   500),
                new NetworkTestParameter("",0.03,  0.1,    500),
                new NetworkTestParameter("",0.03,  1,      500),
                new NetworkTestParameter("",0.03,  10,     500),
                new NetworkTestParameter("",0.03,  100,    500),
                // new NetworkParm("",0.03,  1000,   500),
                // new NetworkParm("Test04",0.03,  10000,  100),
                new NetworkTestParameter("",0.01,  0.01,   500),
                new NetworkTestParameter("",0.01,  0.1,    500),
                new NetworkTestParameter("",0.01,  1,      500),
                new NetworkTestParameter("",0.01,  10,     500),
                // new NetworkParm("",0.01,  100,    500),
                // new NetworkParm("",0.01,  1000,   500),
                // new NetworkParm("Test08",0.01,  10000,  100),
                new NetworkTestParameter("",0.005, 0.01,   1000),
                new NetworkTestParameter("",0.005, 0.1,    1000),
                new NetworkTestParameter("",0.005, 1,      1000),
                new NetworkTestParameter("",0.005, 10,     1000),
                new NetworkTestParameter("",0.0001, 100,     1000),
               // new NetworkParm("",0.005, 100,    500),
                // new NetworkParm("",0.005, 1000,   500),
                // new NetworkParm("Test12",0.005, 10000,  100),
            };
        }
    }
}
