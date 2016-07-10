using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01
{
    public class DataBlock
    {
        static public void Copy(DataBlock src, int srcOffset, DataBlock dst, int dstOffset, int length)
        {
            Array.Copy(src.Data, srcOffset, dst.Data, dstOffset, length);
        }

        public double[] Data;
        public DataBlock(int length)
        {
            Data = new double[length];
        }
        public DataBlock(double[] buffer, bool isCopy)
        {
            if( isCopy == false)
            {
                Data = buffer;
            }
            else
            {
                Data = new double[buffer.Length];
                buffer.CopyTo(Data, 0);
            }
        }
        public int Length
        {
            get { return Data.Length; }
        }
        public double this[int index]
        {
            set { Data[index] = value; }
            get { return Data[index]; }
        }
        public int CopyToFromEndIndex(int srcOffest, DataBlock dst, int dstOffset, int length)
        {
            Copy(this, srcOffest - length + 1, dst, dstOffset - length + 1, length);
            return length;
        }
        public int CopyTo(int srcOffest, DataBlock dst, int dstOffset, int length)
        {
            Copy(this, srcOffest, dst, dstOffset, length);
            return length;
        }
    }
}
