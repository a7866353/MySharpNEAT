using MyProject01.Controller;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.DataSources
{
    public interface IDataSource
    {
        void Copy(int index, DataBlock buffer, int offset, int length);
        int TotalLength { get; }
        int SkipCount { get; }
        RateSet this[int index] { get; }

    }
}
