using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Test
{
    class RateDataCreator
    {
        private const string _dataFile = "data.csv";
        private DataLoader _loader;

        private const int inputCount = 14;
        private const int outputCount = 1;
        private const int testSetCount = 20;
        private const int dataInv = 1;

        public RateDataCreator()
        {
            // TODO
            // _loader = new FenghuangDataLoader(_dataFile);
        }
        public TestData GetTestData()
        {

            // check data is enough
            int inputCount = RateDataCreator.inputCount;
            int outputCount = RateDataCreator.outputCount;
            int dataInv = RateDataCreator.dataInv;
            int testSetCount = RateDataCreator.testSetCount;
            int setCount = (_loader.Count - inputCount - outputCount + dataInv) / dataInv;
            int trainSetCount = setCount - testSetCount;

            if (trainSetCount < 0)
                return null;

            MyTestDataList trainList = new MyTestDataList();
            MyTestDataList testList = new MyTestDataList();

            double[][] inputs = new double[trainSetCount][];
            double[][] outputs = new double[trainSetCount][];
            double[][] testInputs = new double[testSetCount][];
            double[][] testOutputs = new double[testSetCount][];
            int index = 0;
            MyTestData dataObj;
            // Create train data sets.
            for (int i = 0; i < trainSetCount; i++)
            {
                dataObj = new MyTestData();
                dataObj.Input = _loader.GetArr(index, inputCount);
                dataObj.Ideal = _loader.GetArr(index + inputCount, outputCount);
                index += dataInv;
                trainList.Add(dataObj);
            }

            // Create test data sets.
            for (int i = 0; i < testSetCount; i++)
            {
                dataObj = new MyTestData();
                dataObj.Input = _loader.GetArr(index, inputCount);
                dataObj.Ideal = _loader.GetArr(index + inputCount, outputCount);
                index += dataInv;
                testList.Add(dataObj);
            }

            TestData data = new TestData(trainList, testList);
            return data;
        }
        public RateSet GetData(int index)
        {
            return _loader[index].Clone();
        }
        public double[] GetData(int startIndex, int length)
        {
            return _loader.GetArr(startIndex, length);
        }


    }
}
