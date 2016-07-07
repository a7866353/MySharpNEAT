using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.EA.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.NEAT;
using Encog.Neural.Networks.Training;
using MyProject01.DAO;
using MyProject01.TestCases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    public class NEATController
    {
        private NEATPopulation _population;
        public NEATNetwork BestNetwork;

        private ControllerDAO _dao;
        public NEATPopulation GetPopulation()
        {
           
            if (_population == null)
            {
                _population = new NEATPopulation(InputVectorLength, OutputVectorLength, PopulationNumeber);
                _population.Reset();

                 // _population.InitialConnectionDensity = 1.0; // not required, but speeds processing.
            }
            return _population;

        }

        public string Name
        {
            get { return _dao.Name; }
        }
        public int InputVectorLength
        {
            set { _dao.InputVectorLength = value; }
            get { return _dao.InputVectorLength; }
        }
        public int OutputVectorLength
        {
            set { _dao.OutputVectorLength = value; }
            get { return _dao.OutputVectorLength; }
        }
        public int PopulationNumeber
        {
            set { _dao.PopulationNumeber = value; }
            get { return _dao.PopulationNumeber; }
        }
        public double DataOffset
        {
            set { _dao.DataOffset = value; }
            get { return _dao.DataOffset; }
        }
        public double DataScale
        {
            set { _dao.DataScale = value; }
            get { return _dao.DataScale; }
        }
        public static NEATController Open(string name, bool isNew = false, bool needPopulation = true)
        {
            ControllerDAO dao = ControllerDAO.GetDAO(name, isNew);
            NEATController controller;

            if (dao.BestNetwork == null)
            {
                controller = new NEATController(dao);
                controller.InputVectorLength = controller.OutputVectorLength = controller.PopulationNumeber = -1;

            }
            else
            {
                controller = new NEATController(dao);
                if (needPopulation == true)
                    controller._population = dao.GetPopulation();
                controller.BestNetwork = dao.GetBestNetwork();

            }

            return controller;
        }

        private NEATController(ControllerDAO dao)
        {
            this._dao = dao;
        }
        public double[] Compute(double[] input)
        {
            if (BestNetwork == null)
                return null;
            IMLData output = BestNetwork.Compute(new BasicMLData(input, false));
            double[] outputArr = new double[output.Count];
            for(int i=0; i<outputArr.Length; i++)
                outputArr[i] = output[i];
            return outputArr;
        }
        public void Save()
        {
            _dao.SetBestNetwork(BestNetwork);
            _dao.Save();
            _dao.UpdatePopulation(_population);
        }

    }
}
