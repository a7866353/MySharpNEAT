using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace MyProject01.Util
{
    class LogWriter
    {
        private string _fileName;
        private const string _newline = "\r\n";

        public LogWriter(string file_name)
        {
            this._fileName = file_name;
        }
        public void SetFileName(string fileName)
        {
            this._fileName = fileName;
        }
        public void Reset()
        {
            StreamWriter sw = new StreamWriter(_fileName, false);
            sw.Close();
        }
        public void WriteLine(string str)
        {
            StreamWriter sw = new StreamWriter(_fileName,true);
            ///sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(str);
            sw.Close();
        }
        public void Write(string str)
        {
            StreamWriter sw = new StreamWriter(_fileName, true);
            sw.Write(str);
            sw.Close();
        }
    }
}
