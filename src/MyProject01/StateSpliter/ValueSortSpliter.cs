using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.StateSpliter
{
    abstract class ValueSpliter : List<double>
    {
        abstract public double[] GetSplitValue(int num);
    }
    abstract class ValueSpliterFactory
    {
        public ValueSpliter[] GetSpliter(int num)
        {
            ValueSpliter[] spliterArr = new ValueSpliter[num];
            for (int i = 0; i < spliterArr.Length; i++)
            {
                spliterArr[i] = GetNewValueSpliter();
            }
            return spliterArr;
        }
        abstract protected ValueSpliter GetNewValueSpliter();

    }

    class ValueSortSpliter : ValueSpliter
    {
        public override double[] GetSplitValue(int num)
        {
            int step = Count / num;
            double[] splitValue = new double[num];

            for (int i = 0; i < num - 1; i++)
            {
                splitValue[i] = this[step * i];
            }
            splitValue[num - 1] = this[this.Count - 1];

            return splitValue;
        }
    }
    class ValueSortSpliterFactory : ValueSpliterFactory
    {

        protected override ValueSpliter GetNewValueSpliter()
        {
            return new ValueSortSpliter();
        }
    }



    class MiddleValueSpliter : ValueSpliter
    {
        public double MiddleValue;
        public override double[] GetSplitValue(int num)
        {
            int step = Count / num;
            double[] splitValue = new double[num];

            for (int i = 0; i < Count - 1; i++)
            {
                splitValue[i] = this[step * i];
            }
            splitValue[num - 1] = this[this.Count - 1];

            return splitValue;
        }
    }
    class MiddleValueSpliterFactory : ValueSpliterFactory
    {
        public double MiddleValue;
        protected override ValueSpliter GetNewValueSpliter()
        {
            return new MiddleValueSpliter() { MiddleValue = MiddleValue };
        }
    }


}
