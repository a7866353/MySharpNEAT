using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util
{
    class ArrayDataNormalizeAnalyzer
    {
        private BasicNormalizeAnalyzer _sampleAnalyzer;
        private BasicNormalizeAnalyzer[] _normAnlzArr;
        private int _length;

        public ArrayDataNormalizeAnalyzer(BasicNormalizeAnalyzer anlz)
        {
            _sampleAnalyzer = anlz;
        }

        public void Init(double[] firstDataArray)
        {
            _length = firstDataArray.Length;
            _normAnlzArr = new BasicNormalizeAnalyzer[_length];
            for (int i = 0; i < _length; i++)
            {
                BasicNormalizeAnalyzer na = _sampleAnalyzer.Clone();
                na.Init(firstDataArray[i]);
                _normAnlzArr[i] = na;
            }
        }

        public void Set(double[] dataArr)
        {
            for (int i = 0; i < _length; i++)
            {
                _normAnlzArr[i].Set(dataArr[i]);
            }
        }

        public Normalizer[] NromalizerArray
        {
            get
            {
                Normalizer[] normArr = new Normalizer[_length];
                for (int i = 0; i < _length; i++)
                {
                    normArr[i] = _normAnlzArr[i].Normalizer;
                }
                return normArr;
            }
        }
    }

    class BlockDataNormalizer
    {
        private double _scale = 1;
        private double _offset = 0;

        private const double _targetDataLimit = 0.5;
        private const double _targetDataMiddleValue = 0.5;

        public double Scacle
        {
            get { return _scale; }
        }
        public double Offset
        {
            get { return _offset; }
        }

        public void Normalize(BasicDataBlock dataBlock)
        {
            double[] buffer = new double[dataBlock.BlockLength];

            dataBlock.Copy(buffer);
            NormalizeAnalyzer norm = new NormalizeAnalyzer(_targetDataMiddleValue, _targetDataLimit / 2, _targetDataLimit);
            norm.Init(buffer[0]);

            while(true)
            {
               foreach (double data in buffer)
                {
                    norm.Set(data);
                }

               if (dataBlock.Next() == false)
                   break;
            }
            _scale = norm.Scale;
            _offset = norm.Offset;
        }
    

    }

    abstract class BasicNormalizeAnalyzer
    {
        abstract public double Scale { get; }
        abstract public double Offset { get; }
        abstract public Normalizer Normalizer { get; }
        abstract public void Set(double data);
        abstract public void Init(double initdata);
        abstract public BasicNormalizeAnalyzer Clone();

        public void Set(DataSection secData)
        {
            for (int i = 0; i < secData.Length; i++)
            {
                Set(secData[i]);
            }
        }
        public void Set(double[] dataArr)
        {
            for (int i = 0; i < dataArr.Length; i++)
            {
                Set(dataArr[i]);
            }
        }


    }

    class NormalizeAnalyzer : BasicNormalizeAnalyzer
    {
        private double _limit = 0.5;
        private double _targetDataMargin = 0.25;
        private double _targetDataMid = 0;

        private double _targDataMax;
        private double _targDataMin;

        private double _dataMaxValue;
        private double _dataMinValue;

        public NormalizeAnalyzer(double middleValue, double margin, double limit)
        {
            _targetDataMid = middleValue;
            _targetDataMargin = margin;
            _limit = limit;

            _targDataMax = _targetDataMid + _targetDataMargin;
            _targDataMin = _targetDataMid - _targetDataMargin;

        }
        override public void Set(double data)
        {
            if (data > _dataMaxValue)
                _dataMaxValue = data;
            else if (data < _dataMinValue)
                _dataMinValue = data;
        }

        override public void Init(double initdata)
        {
            _dataMaxValue = _dataMinValue = initdata;
        }

        // out = (in + Offset) * Scale;
        override public double Scale
        {
            get 
            {
                return (_targDataMax - _targDataMin) / (_dataMaxValue - _dataMinValue); 
            }
        }
        override public double Offset
        {
            get { return (_dataMaxValue - _dataMinValue) * _targDataMax / (_targDataMax - _targDataMin) - _dataMaxValue; }
        }

        override public Normalizer Normalizer
        {
            get { return new NormalizerWithTrim(Offset, Scale, _targetDataMid + _limit, _targetDataMid - _limit) { SourceMaxValue = _dataMaxValue, SourceMinValue = _dataMinValue }; }
        }

        public override BasicNormalizeAnalyzer Clone()
        {
            return (NormalizeAnalyzer)MemberwiseClone();
        }
    }

    class ZeroScoreNormalizeAnalyzer : BasicNormalizeAnalyzer
    {
        private double _scale = 1.0;
        private List<double> _dataList;

        public ZeroScoreNormalizeAnalyzer(double scale)
        {
            _scale = scale;
        }

        override public void Set(double data)
        {
            _dataList.Add(data);
        }

        override public void Init(double initdata)
        {
            _dataList = new List<double>();
            _dataList.Add(initdata);
        }

        // out = (in + Offset) * Scale;
        override public double Scale
        {
            get
            {
                double ave = Offset;
                ave *=  ave;
                double sum = 0;
                foreach (double d in _dataList)
                    sum += d * d + ave;
                sum /= _dataList.Count;

                return _scale / Math.Sqrt(sum);
            }
        }
        override public double Offset
        {
            get 
            {
                double sum = 0;
                foreach (double d in _dataList)
                    sum += d;
                sum /= _dataList.Count;

                return sum;
            }
        }

        override public Normalizer Normalizer
        {
            get { return new Normalizer(Offset, Scale); }
        }


        public override BasicNormalizeAnalyzer Clone()
        {
            return new ZeroScoreNormalizeAnalyzer(_scale);
        }
    }

    [Serializable]
    class Normalizer
    {
        public double SourceMaxValue;
        public double SourceMinValue;
        public double Offset = 0;
        public double Scale = 1;


        public Normalizer(double offset, double scale)
        {
            this.Offset = offset;
            this.Scale = scale;
        }

        virtual public double Convert(double value)
        {
            double res = (value + Offset) * Scale;
            if (double.IsNaN(res) == true)
                return 0;
            else
                return res;
        }

        public void Convert(DataSection dataSec)
        {
            for (int i = 0; i < dataSec.Length; i++)
            {
                dataSec[i] = Convert(dataSec[i]);
            }
        }

        public override string ToString()
        {
            return "O"+Offset.ToString("G") + "|S" + Scale.ToString("G");
        }
    }
    [Serializable]
    class NormalizerWithTrim : Normalizer
    {
        public double MaxValue;
        public double MinValue;

        public NormalizerWithTrim(double offset, double scale, double maxValue, double minValue)
            :base(offset, scale)
        {
            MaxValue = maxValue;
            MinValue = minValue;
        }
        public override double Convert(double value)
        {
            double v = base.Convert(value);
            if (v > MaxValue)
                v = MaxValue;
            else if (v < MinValue)
                v = MinValue;
            return v;
        }
        public override string ToString()
        {
            return "O" + Offset.ToString("G") + "|S" + Scale.ToString("G")
                + "|H" + MaxValue.ToString("G") + "|L" + MinValue.ToString("G");

        }
    }

}
