using MyProject01.Controller;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.InputDataAnalyzerTool
{
    [Serializable]
    class DoubleDataList : List<double>
    {
        static public DoubleDataList Load(string path)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(path, FileMode.Open);
            }
            catch(Exception )
            {
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            return (DoubleDataList)formatter.Deserialize(stream);

        }
        public void Save(string path)
        {
            FileStream stream = File.Create(path);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Close();
        }

    }
    class DataListFileManager
    {
        public string PathName = @"D:\Temp\";
        public string FileName = "DataList";
        private string[] _fileNames;
        List<string> _vaildFilePathList;

        public void Save(DoubleDataList[] dataListArr)
        {
            for(int i=0;i<dataListArr.Length; i++)
            {
                dataListArr[i].Save(PathName + GetFileNameByIndex(i));
            }
        }

        public DoubleDataList[] GetAll()
        {
            _fileNames = Directory.GetFiles(PathName);
            int index = 0;
            _vaildFilePathList = new List<string>();

            while(true)
            {
                string vailedFileName = CheckFileName(index);
                if (vailedFileName == null)
                    break;

                _vaildFilePathList.Add(PathName + vailedFileName);
                index++;
            }

            if (_vaildFilePathList.Count == 0)
                return null;

            DoubleDataList[] dataListArr = new DoubleDataList[_vaildFilePathList.Count];
            for (int i = 0; i < _vaildFilePathList.Count; i++)
            {
                dataListArr[i] = DoubleDataList.Load(_vaildFilePathList[i]);
            }
            return dataListArr;
        }

        private string CheckFileName(int index)
        {
            for(int i=0;i<_fileNames.Length;i++)
            {
                if (_fileNames[i].CompareTo(GetFileNameByIndex(index)) == 0)
                    return _fileNames[i];
            }
            return null;
        }

        private string GetFileNameByIndex(int i)
        {
            return FileName + "_" + i.ToString();
        }
    }
    class InputDataAnalyzer
    {
        private DoubleDataList[] _dataList;
        public int DataBlockLength = 1024;

        public void Run()
        {
            DataLoader loader = new MTDataLoader("USDJPY", DataTimeType.M5);
            RateDataBlock testBlock = new RateDataBlock(loader, 0, loader.Count, DataBlockLength);
            

            Parallel.For(0, testBlock.BlockCount, index =>
                {
                    double[] rateData = new double[DataBlockLength];
                    testBlock.Copy(rateData, index);

                    FWTFormater format = new FWTFormater(DataBlockLength);
                    format.Convert(rateData);

                    lock (_dataList)
                    {
                        for (int i = 0; i < DataBlockLength; i++)
                        {
                            _dataList[i].Add(format.Buffer[i]);
                        }
                    }

                });


            DataListFileManager fileMana = new DataListFileManager();
            fileMana.Save(_dataList);
        }
    }



}
