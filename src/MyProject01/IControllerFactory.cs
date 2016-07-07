using MyProject01.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01
{
    interface IControllerFactory
    {
        int InputLength { get; set; }
        string Name { get; set; }
        NetworkController Get();
    }
}
