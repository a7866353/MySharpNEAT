using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyProject01.Util
{
    class CSVItem
    {
        private CSVReader _reader;
        private string _name;
        private int _idx;

        public string Name
        {
            get { return _name; }
        }
        public int Index
        {
            get { return _idx; }
        }
        public string Value
        {
            get { return _reader.CurrentLine[_idx]; }
        }
        public CSVItem(CSVReader reader, string name, int idx)
        {
            _reader = reader;
            _name = name;
            _idx = idx;
        }
        public string GetValue()
        {
            return _reader.CurrentLine[_idx];
        }

    }
    class CSVReader
    {
        private string[] _currentLine;
        private string[] _firstLine;
        private StreamReader _sr;

        public string[] CurrentLine
        {
            get
            {
                return _currentLine;
            }
        }
        public string[] FirstLine
        {
            get
            {
                return _firstLine;
            }
        }        
        public void Open(string path)
        {
            _sr = new StreamReader(File.OpenRead(path));
            _currentLine = GetLine();
            _firstLine = _currentLine;
        }

        public bool NextLine()
        {
            string[] line = GetLine();
            if(line == null)
                return false;
            _currentLine = line;
            return true;

        }

        private string[] GetLine()
        {
            string line = _sr.ReadLine();
            if (line == null)
                return null;

            string[] res = line.Split(',');
            for (int i = 0; i < res.Length;i++ )
            {
                string str = res[i];
                if (str[0] == '\"')
                    res[i] = str.Substring(1, str.Length - 2);

            }
            return res;
        }
    }
    class CVSWriter
    {
        private StreamWriter _sw;

        public void Write(string fileName, DataLoader loader)
        {
            _sw = new StreamWriter(File.OpenWrite(fileName));

            // Write Header 
            WriteTitle();

            for(int i=0;i<loader.Count;i++)
            {
                WriteLine(loader[i]);
            }
            _sw.Close();

        }
        private void WriteLine(RateSet set)
        {
            _sw.WriteLine(
                set.Time.ToString() + ',' +
                set.Open.ToString() + ',' +
                set.High.ToString() + ',' +
                set.Low.ToString() + ',' +
                set.Close.ToString()
                );
        }
        private void WriteTitle()
        {
            _sw.WriteLine(
                "Date" + ',' +
                "OPEN" + ',' +
                "HIGH" + ',' +
                "LOW" + ',' +
                "CLOSE"
                );
        }
    }
}
