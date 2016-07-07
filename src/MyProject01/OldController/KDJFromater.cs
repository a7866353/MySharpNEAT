using Encog.ML.Data.Basic;
using MyProject01.Util;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    class KDJInputSection
    {
        private DataSection[] _secArr;

        public DataSection Rate
        {
            get { return _secArr[0]; }
            set { _secArr[0] = value; }
        }
        public DataSection K
        {
            get { return _secArr[1]; }
            set { _secArr[1] = value;}
        }
        public DataSection D
        {
            get { return _secArr[2]; }
            set { _secArr[2] = value; }
        }
        public DataSection J
        {
            get { return _secArr[3]; }
            set { _secArr[3] = value; }
        }
        public KDJInputSection(double[] arr, int blockLength)
        {
            _secArr = new DataSection[4];
            Rate = new DataSection(arr, 0 * blockLength, blockLength);
            K = new DataSection(arr, 1 * blockLength, blockLength);
            D = new DataSection(arr, 2 * blockLength, blockLength);
            J = new DataSection(arr, 3 * blockLength, blockLength);
        }
    }
    class KDJOutputSection
    {
        public KDJOutputSection(double[] arr, int blockLength)
        {
            _secArr = new DataSection[5];
            Rate = new DataSection(arr, 0 * blockLength, blockLength);
            K = new DataSection(arr, 1 * blockLength, blockLength);
            D = new DataSection(arr, 2 * blockLength, blockLength);
            J = new DataSection(arr, 3 * blockLength, blockLength);
            Cross3Line = new DataSection(arr, 4 * blockLength, blockLength);
        }

        private DataSection[] _secArr;
        public DataSection Rate
        {
            get { return _secArr[0]; }
            set { _secArr[0] = value; }
        }
        public DataSection K
        {
            get { return _secArr[1]; }
            set { _secArr[1] = value ; }
        }
        public DataSection D
        {
            get { return _secArr[2]; }
            set { _secArr[2] = value; }
        }
        public DataSection J
        {
            get { return _secArr[3]; }
            set { _secArr[3] = value; }
        }
        public DataSection Cross3Line
        {
            get { return _secArr[4]; }
            set { _secArr[4] = value; }
        }

    }

    [Serializable]
    class KDJFormater : IInputDataFormater
    {
        private int _blockLength;
        public Normalizer _rateNorm;
        public double[] _buffer;
        public KDJFormater(int blockLength)
        {
            _blockLength = blockLength;
            /* rate close price
             * k value
             * d value 
             * j value 
             * k d j Cross Check
             */
            _buffer = new double[blockLength * 5];
        }
        public int InputDataLength
        {
            get { return _blockLength; }
        }

        public int ResultDataLength
        {
            get { return _buffer.Length; }
        }
        public void Normilize(BasicDataBlock dataBlock, double middleValue, double limit)
        {
            NormalizeAnalyzer norm = new NormalizeAnalyzer();
            double[] buffer = new double[dataBlock.BlockLength];
            DataSection[] secArr = CreateInputSection(buffer);
            DataSection rateSec = secArr[0];

            dataBlock.Reset();
            dataBlock.Copy(buffer);
            norm.SetTarget(middleValue, limit / 2, limit);
            norm.Init(rateSec[0]);

            while (true)
            {
                if (dataBlock.Next() == false)
                    break;
                dataBlock.Copy(buffer);
                norm.Set(rateSec);
            }

            _rateNorm = norm.Normalizer;
        }
        public BasicMLData Convert(double[] rateDataArray)
        {
            MutiplayArraryReader reader = new MutiplayArraryReader(rateDataArray, _blockLength);

            KDJInputSection inputSec = new KDJInputSection(rateDataArray, _blockLength);
            KDJOutputSection outputSec = new KDJOutputSection(_buffer, _blockLength);

            // Array.Copy(rateDataArray, _buffer, rateDataArray.Length);
            inputSec.Rate.CopyTo(outputSec.Rate);
            inputSec.K.CopyTo(outputSec.K);
            inputSec.D.CopyTo(outputSec.D);
            inputSec.J.CopyTo(outputSec.J);

            _rateNorm.Convert(outputSec.Rate);
            
            for(int i=0;i<_blockLength;i++)
            {
                outputSec.Cross3Line[i] = Math.Abs(inputSec.K[i] - inputSec.D[i])
                    + Math.Abs(inputSec.K[i] - inputSec.J[i])
                    + Math.Abs(inputSec.D[i] - inputSec.J[i]);
            }

            return new BasicMLData(_buffer, false);
        }

        public IInputDataFormater Clone()
        {
            KDJFormater formater = new KDJFormater(_blockLength);
            formater._rateNorm = _rateNorm;
            return formater;
        }

        public string GetDecs()
        {
            return "KDJ";
        }

        private enum InputSectionName
        {
            Rate = 0,
            K,
            D,
            J,
        };
        private DataSection[] CreateInputSection(double[] arr)
        {
            DataSection[] secArr = new DataSection[4];
            secArr[0] = new DataSection(arr, 0 * _blockLength, _blockLength);
            secArr[1] = new DataSection(arr, 1 * _blockLength, _blockLength);
            secArr[2] = new DataSection(arr, 2 * _blockLength, _blockLength);
            secArr[3] = new DataSection(arr, 3 * _blockLength, _blockLength);
            
            return secArr;
        }
        private enum OutputSectionName
        {
            Rate = 0,
            K,
            D,
            J,
            Cross3Line,
        };
        private DataSection[] CreateOutputSection(double[] arr)
        {
            DataSection[] secArr = new DataSection[5];
            secArr[0] = new DataSection(arr, 0 * _blockLength, _blockLength);
            secArr[1] = new DataSection(arr, 1 * _blockLength, _blockLength);
            secArr[2] = new DataSection(arr, 2 * _blockLength, _blockLength);
            secArr[3] = new DataSection(arr, 3 * _blockLength, _blockLength);
            secArr[4] = new DataSection(arr, 4 * _blockLength, _blockLength);

            return secArr;
        }
    }
    [Serializable]
    class KDJOnlyFormater : IInputDataFormater
    {
        private int _blockLength;
        public double[] _buffer;
        public KDJOnlyFormater(int blockLength)
        {
            _blockLength = blockLength;
            /* k value
             * d value 
             * j value 
             * k d j Cross Check
             */
            _buffer = new double[blockLength * 4];
        }
        public int InputDataLength
        {
            get { return _buffer.Length; }
        }

        public int ResultDataLength
        {
            get { return _buffer.Length; }
        }

        public BasicMLData Convert(double[] rateDataArray)
        {
            MutiplayArraryReader reader = new MutiplayArraryReader(rateDataArray, _blockLength);

            Array.Copy(rateDataArray, _blockLength, _buffer, 0, rateDataArray.Length - _blockLength);

            for (int i = 0; i < _blockLength; i++)
            {
                int index = _blockLength * 3 + i;
                _buffer[index] = Math.Abs(reader.Get(1, i) - reader.Get(2, i))
                    + Math.Abs(reader.Get(1, i) - reader.Get(3, i))
                    + Math.Abs(reader.Get(2, i) - reader.Get(3, i));
            }

            return new BasicMLData(_buffer, false);
        }

        public IInputDataFormater Clone()
        {
            KDJOnlyFormater formater = new KDJOnlyFormater(_blockLength);
            return formater;
        }

        public string GetDecs()
        {
            return "KDJOnly";
        }
    }

    class MutiplayArraryReader
    {
        private double[] _buffer;
        private int _dataWidth;
        private int _dataChannel;
        public MutiplayArraryReader(double[] buffer, int dataWidth)
        {
            this._buffer = buffer;
            this._dataWidth = dataWidth;
            _dataChannel = _buffer.Length / _dataWidth;
        }
        public double Get(int channel, int index)
        {
            if (channel > _dataChannel || index > _dataWidth)
                throw (new Exception("Error!"));
            return _buffer[index + channel * _dataWidth];
        }
    }

    class NormalizeSection
    {
        private double[] _data;
        private int _startIndex;
        private int _length;
        private Normalizer _norm;
        public NormalizeSection(double[] data, int startIndex, int length, Normalizer norm)
        {
            _data = data;
            _startIndex = startIndex;
            _length = length;
            _norm = norm;
        }

        public void Convert(double[] dst, int startIndex)
        {
            for(int i=0;i<_length;i++)
            {
                dst[startIndex + i] = _norm.Convert(_data[_startIndex + i]);
            }
        }
    }

}
