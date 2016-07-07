using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util
{
    interface IDescriptionProvider
    {
        string Description { get; }
    }
    class StringDescriptionProvider : IDescriptionProvider
    {
        private string _desc = "";
        public StringDescriptionProvider(string desc)
        {
            _desc = desc;
        }

        public string Description
        {
            get { return _desc; }
        }
    }
    class DescriptionProviderList : List<IDescriptionProvider>, IDescriptionProvider
    {
        public void Add(string desc)
        {
            Add(new StringDescriptionProvider(desc));
        }
        public string Description
        {
            get 
            {
                string str = "";
                foreach(IDescriptionProvider desc in this)
                {
                    str += desc.Description + "_";
                }
                return str;
            }
        }
    }
}
