using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SocketTestClient
{
    class CSVStringPacker
    {
        private List<string> _stringList;
        public CSVStringPacker()
        {
            _stringList = new List<string>();
        }

        public void Add(string str)
        {
            _stringList.Add(str);
        }
        public void Add(DateTime time)
        {
            _stringList.Add(time.ToString());
        }
        public void Add(double d)
        {
            _stringList.Add(d.ToString("G"));
        }

        public string GetPackedString()
        {
            string str = "";
            for (int i = 0; i < _stringList.Count; i++)
            {
                str += _stringList[i];
                if (i < _stringList.Count - 1)
                    str += ",";
            }
            return str;
        }
    }
    class CSVLogFormater
    {

        static private string[] _titleList = new string[]
        {
            "LogTime",
            "Name",
            "ReqStaTime",
            "ReqEndTime",
            "ReqDuration",
            "RstCount",
            "RstStaTime",
            "RstEndTime"
        };
        static private string _logFileName = "RateDataReqLog";
        static private StreamWriter _writer;

        static CSVLogFormater()
        {
            string fileName = _logFileName+".csv";
            if( File.Exists(fileName) == false )
            {
                _writer = new StreamWriter(fileName);
                _writer.WriteLine(StrArrPack(_titleList));
                _writer.Flush();
            }
            else
            {
                _writer = new StreamWriter(fileName);
                _writer.BaseStream.Seek(0, SeekOrigin.End);
            }
            _writer.AutoFlush = true;
        }

        static public void Add(string name, DateTime reqSta, DateTime reqEnd, int rstCount, DateTime rstSta, DateTime rstEnd)
        {
            CSVStringPacker pack = new CSVStringPacker();
            pack.Add(DateTime.Now);
            pack.Add(name);
            pack.Add(reqSta);
            pack.Add(reqEnd);
            pack.Add((reqEnd - reqSta).ToString());
            pack.Add(rstCount);
            pack.Add(rstSta);
            pack.Add(rstEnd);

            _writer.WriteLine(pack.GetPackedString());
            _writer.BaseStream.Flush();
            _writer.Flush();
        }

        static private string StrArrPack(string[] strArr)
        {
            string str = "";
            for (int i = 0; i < strArr.Length; i++)
            {
                str += strArr[i];
                if (i < strArr.Length - 1)
                    str += ",";
            }
            return str;
        }

    }
}
