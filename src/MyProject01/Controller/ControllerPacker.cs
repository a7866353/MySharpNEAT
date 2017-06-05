using MyProject01.NeuroNetwork;
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

    [Serializable]
    public abstract class BasicControllerPacker
    {
        static public BasicControllerPacker FromBinary(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            BasicControllerPacker obj = (BasicControllerPacker)formatter.Deserialize(stream);
            return obj;
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

        abstract public IController GetController();
    }

   

    [Serializable]
    class ControllerPacker : BasicControllerPacker
    {
        private ISensor _sensor;
        private IActor _actor;
        private INeuroNetwork _neuroNetwork;
        private Normalizer[] _norm;

        public INeuroNetwork NeuroNetwork
        {
            set { _neuroNetwork = value; }
        }


        public ControllerPacker(ISensor sensor, IActor actor, INeuroNetwork net, Normalizer[] norm)
        {
            _sensor = sensor;
            _actor = actor;
            _neuroNetwork = net;
            _norm = norm;
        }


        public override IController GetController()
        {
            BasicController ctrl = new BasicController(_sensor, _actor);
            ctrl.UpdateNetwork(_neuroNetwork);
            ctrl.NormalizerArray = _norm;
            return ctrl;
        }


    }
}
