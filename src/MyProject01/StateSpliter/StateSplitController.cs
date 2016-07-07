using MyProject01.Controller;
using MyProject01.Util;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyProject01.StateSpliter
{
    class StateSplitController
    {
        public int DataBlockLength;
        public string RateDataControllerName;
        public int SplitNumber;
        public ValueSpliterFactory SpliterFactory;
        public double[][] ResultArr;

        private DataLoader _loader;
        private ValueSpliter[] _spliterArr;
        public void Init()
        {
            // init test data
            // _loader = new MTData2Loader(RateDataControllerName);
            _loader = new MTDataLoader("USDJPY", DataTimeType.M5);
            ResultArr = null;
        }

        public void Run()
        {
            BasicDataBlock testBlock = new RateDataBlock(_loader, 0, _loader.Count, DataBlockLength);
            FWTFormater formater = new FWTFormater(DataBlockLength);
            double[] inputData = new double[DataBlockLength];

            _spliterArr = SpliterFactory.GetSpliter(formater.ResultDataLength);
            ResultArr = new double[formater.ResultDataLength][];
            while(true)
            {
                testBlock.Copy(inputData);
                formater.Convert(inputData);

                for (int i = 0; i < _spliterArr.Length;i++ )
                {
                    _spliterArr[i].Add(formater.Buffer[i]);
                }

                if (testBlock.Next() == false)
                    break;
            }

            for(int i=0;i<_spliterArr.Length;i++)
            {
                ValueSpliter spliter = _spliterArr[i];

                spliter.Sort();
                ResultArr[i] = spliter.GetSplitValue(SplitNumber);
            }
        }
        public string GetString()
        {
            string res = "";

            res += GetResultString2();
            // res += GetOutputInfomationString();

            return res;
        }

        private string GetResultString2()
        {
            MyTextWriter tw = new MyTextWriter();
            tw.WriteLine("Result:");
            tw.WriteLine("{");
            tw.TabUp();
            foreach (double[] arr in ResultArr)
            {
                tw.WriteLine("{");
                tw.TabUp();
                foreach (double val in arr)
                {
                    tw.WriteLine(val + ",");
                }
                tw.TabDown();
                tw.WriteLine("},");
            }
            tw.TabDown();
            tw.WriteLine("}");

            string res = tw.GetString();
            tw.Close();
            return res;
        }

        private string GetOutputInfomationString()
        {
            MyTextWriter tw = new MyTextWriter();
            tw.WriteLine("_spliterArr[" + _spliterArr.Length + "] = ");
            tw.WriteLine("{");
            tw.TabUp();

            for (int i = 0; i < _spliterArr.Length; i++)
            {
                tw.WriteLine("[" + i + "] = ");
                tw.WriteLine("{");
                tw.TabUp();

                ValueSpliter spliter = _spliterArr[i];
                tw.Write(spliter);

                tw.TabDown();
                tw.WriteLine("}");
            }

            tw.TabDown();
            tw.WriteLine("}");

            string res = tw.GetString();
            tw.Close();
            return res;
        }
    }
}

class MyTextWriter
{
    private StringBuilder _sb;
    private StringWriter _sw;
    private string _tabStr;
    private int _tabCount;
    public MyTextWriter()
    {
        _sb = new StringBuilder();
        _sw = new StringWriter(_sb);
        _tabCount = 0;
        UpdateTab();
    }

    private void UpdateTab()
    {
        _tabStr = "";
        for(int i=0;i<_tabCount;i++)
        {
            _tabStr += "\t";
        }
    }
    public void TabUp()
    {
        _tabCount++;
        UpdateTab();
    }
    public void TabDown()
    {
        _tabCount--;
        if (_tabCount < 0)
            _tabCount = 0;
        UpdateTab();
    }
    public void Write(string str)
    {
        _sw.Write(_tabStr + str);
    }
    public void WriteLine(string str)
    {
        _sw.WriteLine(_tabStr + str);
    }
    public void Write(Array arr)
    {
        for(int i=0;i<arr.Length;i++)
        {
            WriteLine("[" + i + "] = " + arr.GetValue(i));
        }
    }
    public void Write(List<double> arr)
    {
        for (int i = 0; i < arr.Count; i++)
        {
            WriteLine("[" + i + "] = " + arr[i]);
        }
    }
    public string GetString()
    {
        _sw.Flush();
        return _sw.ToString();
    }

    public void Close()
    {
        _sw.Close();
    }
}