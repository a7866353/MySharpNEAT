using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Util
{
    public class MyTestData
    {
        public int InputSize
        {
            get
            {
                if( null == Input )
                {
                    return 0;
                }
                else
                {
                    return Input.Length;
                }
            }
        }
        public int OutputSize
        {
            get
            {
                if (null != Real)
                {
                    return Real.Length;
                }
                else if (null != Ideal)
                {
                    return Ideal.Length;
                }
                else
                {
                    return 0;
                }
            }
        }
        public double[] Input;
        public double[] Real;
        public double[] Ideal;

        public double GetError()
        {
            double res = 0;
            for (int i = 0; i < OutputSize; i++)
            {
                res += (Ideal[i] - Real[i]) / Ideal[i] * 100;
            }
            res /= OutputSize;

            return res;
        }
        public string InputToString()
        {
            return "";
        }


    }
}
