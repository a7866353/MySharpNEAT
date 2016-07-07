using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using MyProject01.Test;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyProject01.DAO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MyProject01
{
    class MyNet
    {
        public BasicNet net;
        public NetworkTestParameter parm;
        public BasicTrainingMethod method;

        //-----------------------------------------------
        private BasicNetwork _network;

        //--------------
        private RateMarketTestDAO myNetDao;

        public MyNet(BasicNet net, BasicTrainingMethod method, NetworkTestParameter parm)
        {
            this.net = net;
            this.method = method;
            this.parm = parm;
            method.ErrorChangeLimit = parm.ErrorChangeLimit;
            method.ErrorChangeTryMaxCount = parm.ErrorChangeRetryCount;
            myNetDao = BasicTestCaseDAO.GetDAO<RateMarketTestDAO>("QLearn01");
        }

        public void Init(int inputSize, int outputSize)
        {
            parm.InputSize = inputSize;
            parm.OutputSize = outputSize;
            _network = net.GetNet(parm);
            _network.Reset();
            
        }

        public void Training(TestData testData)
        {
            // Get Network
            if( _network == null )
            {
                parm.InputSize = testData.InputSize;
                parm.OutputSize = testData.OutputSize;
                _network = net.GetNet(parm);
            }
           

        }
        public double Training(double[][] inputData, double[][] ideaOutputData)
        {   
            // Trainning
            IMLDataSet trainingSet = new BasicMLDataSet(inputData, ideaOutputData);
            return method.TrainNetwork(_network, trainingSet);
        }

        public double Training(double[] inputData, double[] ideaOutputData)
        {
            if (_network == null)
            {
                parm.InputSize = inputData.Length;
                parm.OutputSize = ideaOutputData.Length;
                _network = net.GetNet(parm);
            }
            double[][] inputDataArray = new double[][]
             {
                 inputData,
             };
            double[][] outputDataArray = new double[][]
             {
                 ideaOutputData,
             };
            // Trainning
            IMLDataSet trainingSet = new BasicMLDataSet(inputDataArray, outputDataArray);
            return method.TrainNetwork(_network, trainingSet);
        }

        public double[] Compute(double[] inputData)
        {
            IMLData res = _network.Compute(new BasicMLData(inputData));
            double[] realArr = new double[parm.OutputSize];
            res.CopyTo(realArr, 0, parm.OutputSize);
            return realArr;
        }
        public void Compute(MyTestDataList dataList)
        {
            foreach (MyTestData data in dataList)
            {
                IMLData res = _network.Compute(new BasicMLData(data.Input));
                double[] realArr = new double[data.OutputSize];
                res.CopyTo(realArr, 0, data.OutputSize);
                data.Real = realArr;
            }
        }
        public void SaveNetwork()
        {
            // TODO
            // myNetDao.NetworkParamter = this.parm;
            MemoryStream stream = new MemoryStream();
            BinaryFormatter  formatter = new BinaryFormatter();
            formatter.Serialize(stream, this._network);

            myNetDao.NetworkData = stream.ToArray();
            stream.Close();
            myNetDao.Save();
        }
        public void LoadNetwork()
        {
            MemoryStream stream = new MemoryStream(myNetDao.NetworkData);
            BinaryFormatter formatter = new BinaryFormatter();
            this._network = (BasicNetwork)formatter.Deserialize(stream);
            stream.Close();
        }

        public byte[] NetworkToByte()
        {
            // TODO
            // myNetDao.NetworkParamter = this.parm;
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this._network);

            byte[] res = stream.ToArray();
            stream.Close();
            return res;
        }

    }
}
