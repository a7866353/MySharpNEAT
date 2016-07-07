using MyProject01.Controller;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Factorys.ControllerFactorys
{
    abstract class BasicControllerFactory : IControllerFactory, IDescriptionProvider
    {
        protected string _name;
        protected int _inputLength;
        protected DescriptionProviderList _descList;

        public BasicControllerFactory()
        {
            _descList = new DescriptionProviderList();
        }
        public int InputLength
        {
            get
            {
                return _inputLength;
            }
            set
            {
                _inputLength = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public NetworkController Get()
        {
            NetworkController controller = NetworkController.Open(Name);
            if( controller == null)
            {
                controller = Create();
            }
            return controller;
        }
        abstract protected NetworkController Create();
        abstract public string Description { get; }
    }
}
