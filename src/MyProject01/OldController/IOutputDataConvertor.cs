using MyProject01.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    interface IOutputDataConvertor
    {
        MarketActions Convert(DataBlock outData);
        int NetworkOutputLength { get; }
        IOutputDataConvertor Clone();
        string GetDesc();
    }
}
