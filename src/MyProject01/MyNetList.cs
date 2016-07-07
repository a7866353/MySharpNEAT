using MyProject01.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01
{
    class MyNetList : List<MyNet>
    {

        public void AddNet(BasicNet net, BasicTrainingMethod method, BasicTestParameterSet parmSet)
        {
            MyNet tempNet;
            NetworkTestParameter[] parmArr = parmSet.GetParameterSet(net.GetType().ToString() + "With" + method.GetType().ToString());
            foreach (NetworkTestParameter parm in parmArr)
            {
                tempNet = new MyNet(net, method, parm);
                Add(tempNet);
            }
        }
    }
}
