using Encog.ML;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{

    public interface IControllerPacker
    {
        IController GetController();
    }

    [Serializable]
    class ControllerPacker : IControllerPacker
    {
        private ISensor _sensor;
        private IActor _actor;
        private IMLRegression _neuroNetwork;
        private Normalizer[] _norm;

        public IMLRegression NeuroNetwork
        {
            set { _neuroNetwork = value; }
        }
        static public ControllerPacker FromBinary(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            ControllerPacker obj = (ControllerPacker)formatter.Deserialize(stream);
            return obj;
        }

        public ControllerPacker(ISensor sensor, IActor actor, IMLRegression net, Normalizer[] norm)
        {
            _sensor = sensor;
            _actor = actor;
            _neuroNetwork = net;
            _norm = norm;
        }

        public byte[] GetData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            byte[] data = stream.ToArray();
            stream.Close();
            return data;
        }

        public IController GetController()
        {
            BasicController ctrl = new BasicController(_sensor, _actor);
            ctrl.UpdateNetwork(_neuroNetwork);
            ctrl.NormalizerArray = _norm;
            return ctrl;
        }


    }
}
