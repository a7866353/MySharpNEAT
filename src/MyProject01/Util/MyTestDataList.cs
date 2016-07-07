using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyProject01.Util
{
    public class MyTestDataList : List<MyTestData>
    {
        public int InputSize
        {
            get
            {
                if (0 == this.Count)
                    return 0;
                else
                    return this[0].InputSize;
            }
        }
        public int OutputSize
        {
            get
            {
                if (0 == this.Count)
                    return 0;
                else
                    return this[0].OutputSize;
            }
        }

        public string ResultToString()
        {
            if (this.Count == 0)
                return null;
            if (this[0].Ideal == null)
                return null;
            if (this[0].Real == null)
                return null;

            string str = "";
            MemoryStream ms = new MemoryStream(10 * 1024 * 1024);
            StreamWriter sw = new StreamWriter(ms);
            for (int i = 0; i < Count; i++)
            {
                sw.Write((i+1).ToString("d4"));
                sw.Write(": ");
                sw.Write(this[i].GetError().ToString("000.0000"));
                sw.Write("\r\n");
              
            }
            sw.Flush();
            StreamReader sr = new StreamReader(ms);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            str = sr.ReadToEnd();
            sw.Close();
            sr.Close();
            ms.Close();
            return str;

        }
        public double ResultError
        {
            get
            {
                double err = 0;
                for (int i = 0; i < Count; i++)
                {
                    err += Math.Abs(this[i].GetError());
                }
                err /= Count;

                return err;
            }
        }

        public double[][] Inputs
        {
            get
            {
                double[][] dataArr = new double[Count][];
                for (int i = 0; i < Count; i++)
                {
                    dataArr[i] = this[i].Input;
                }
                return dataArr;
            }
        }
        public double[][] Ideals
        {
            get
            {
                double[][] dataArr = new double[Count][];
                for (int i = 0; i < Count; i++)
                {
                    dataArr[i] = this[i].Ideal;
                }
                return dataArr;
            }
        }
        public double[][] Reals
        {
            get
            {
                double[][] dataArr = new double[Count][];
                for (int i = 0; i < Count; i++)
                {
                    dataArr[i] = this[i].Real;
                }
                return dataArr;
            }
        }

    }
}
