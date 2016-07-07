using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Test
{
    public class TestData
    {
        public MyTestDataList TrainList;
        public MyTestDataList TestList;
        public TestData(MyTestDataList trainList, MyTestDataList testList)
        {
            TrainList = trainList;
            TestList = testList;
        }

        public int InputSize
        {
            get
            {
                return TrainList.InputSize;
            }
        }
        public int OutputSize
        {
            get
            {
                return TrainList.OutputSize;
            }
        }

        public double[][] TrainInputs
        {
            get
            {
                return TrainList.Inputs;
            }
        }
        public double[][] TrainIdeaOutputs
        {
            get
            {
                return TrainList.Ideals;
            }
        }
        public double[][] TrainRealOutputs
        {
            get
            {
                return TrainList.Reals;
            }
        }

        public double[][] TestInputs
        {
            get
            {
                return TestList.Inputs;
            }
        }
        public double[][] TestIdeaOutputs
        {
            get
            {
                return TestList.Ideals;
            }
        }
        public double[][] TestRealOutputs
        {
            get
            {
                return TestList.Reals;
            }
        }

        public string ToStringResults()
        {
            string str = "";
            str += "Test data:\r\n";
            str += TestList.ResultToString();
            return str;
        }
    }
}
