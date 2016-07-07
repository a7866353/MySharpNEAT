using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util.DataObject
{
    public interface IDataBlock
    {
        // Block的个数
        int BlockCount { get; }
        int BlockLength { get; }
        double GetRate(int i);
        int Copy(double[] array, int index);
        BasicDataBlock GetNewBlock(int startIndex, int length);
        BasicDataBlock Clone();
    }
}
