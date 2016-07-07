using MyProject01.Test;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01
{
    abstract class BasicTestParameterSet
    {
        protected NetworkTestParameter[] _parmArr;
        protected LogWriter _logger;

        public NetworkTestParameter[] GetParameterSet(string testCaseName)
        {
            int num = 1;
            foreach (NetworkTestParameter parm in _parmArr)
            {
                parm.name = testCaseName + num++.ToString("D2");
            }

            return _parmArr;
        }
    }
}
