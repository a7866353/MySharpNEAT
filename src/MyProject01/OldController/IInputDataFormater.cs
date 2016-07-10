using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    interface IInputDataFormater
    {
        int InputDataLength { get; }
        int ResultDataLength { get; }
        DataBlock Convert(double[] rateDataArray);
        IInputDataFormater Clone();
        string GetDecs();
    }
}
