using MyProject01.DataSources;
using MyProject01.DataSources.DataSourceParams;
using MyProject01.Util;
using MyProject01.Util.DllTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    public interface ISensor
    {
        int SkipCount { get; }
        int TotalLength { get; }
        int DataBlockLength { get; }
        IDataSource DataSource { get; }
        DataSourceCtrl DataSourceCtrl { set; }
        int Copy(int index, DataBlock buffer, int offset);
        void Init();
        ISensor Clone();
    }
    [Serializable]
    class SensorGroup : List<ISensor>, ISensor
    {
        public int SkipCount
        {
            get
            {
                int maxValue = this[0].SkipCount;
                foreach(ISensor sen in this)
                {
                    maxValue = Math.Max(sen.SkipCount, maxValue);
                }
                return maxValue;
            }
        }

        public int TotalLength
        {
            get
            {
                int minValue = this[0].TotalLength;
                foreach (ISensor sen in this)
                {
                    minValue = Math.Min(sen.TotalLength, minValue);
                }
                return minValue;

            }
        }

        public int DataBlockLength
        {
            get
            {
                int sum = 0;
                foreach (ISensor sen in this)
                {
                    sum += sen.DataBlockLength;
                }
                return sum;
            }
        }
        public int Copy(int index, DataBlock buffer, int offset)
        {
            int idx = 0;
            foreach(ISensor sen in this)
            {
                sen.Copy(index, buffer, idx + offset);
                idx += sen.DataBlockLength;
            }
            return idx;
        }

        public void Init()
        {
            foreach (ISensor sen in this)
            {
                sen.Init();
            }
        }


        public IDataSource DataSource
        {
            get
            {
                return this[0].DataSource;
            }
        }


        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                foreach (ISensor sen in this)
                {
                    sen.DataSourceCtrl = value;
                }
            }
        }

        public ISensor Clone()
        {
            SensorGroup sensorGroup = new SensorGroup();
            foreach(ISensor sen in this)
            {
                sensorGroup.Add(sen.Clone());
            }

            return sensorGroup;
        }
    }

    [Serializable]
    class RateSensor : ISensor
    {
        private int _dataCount;
        [NonSerialized]
        private IDataSource _dataSource;
        public RateSensor(int dataCount)
        {
            _dataCount = dataCount;
        }
        public int SkipCount
        {
            get { return _dataCount-1; }
        }

        public int TotalLength
        {
            get 
            {
                if (_dataSource == null)
                    return 0;
                else
                    return _dataSource.TotalLength; 
            }
        }

        public int DataBlockLength
        {
            get { return _dataCount; }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            _dataSource.Copy(index, buffer, offset, DataBlockLength);
            return 1;
        }
        public void Init()
        {
            // Nothing
        }


        public IDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
            }
        }


        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                RateDataSourceParam param = new RateDataSourceParam(5);
                this._dataSource = value.Get(param);
            }
        }


        public ISensor Clone()
        {
            return MemberwiseClone() as ISensor;
        }
    }

    [Serializable]
    class RateFWTSensor : ISensor
    {
        private int _dataCount;
        private int _index;
        [NonSerialized]
        private IDataSource _dataSource;
        [NonSerialized]
        private DataBlock _dataBuffer;
        [NonSerialized]
        private double[] _tmpBuffer;
        [NonSerialized]
        private DataBlock _outputBuffer;
        public RateFWTSensor(int dataCount)
        {
            _dataCount = dataCount;
            _index = 0;
            Init();
        }
        public int CurrentPosition
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public int SkipCount
        {
            get { return _dataCount - 1; }
        }

        public int TotalLength
        {
            get { return _dataSource.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _dataCount; }
        }

        public int Copy(int index, DataBlock buffer, int startIndex)
        {
            _dataSource.Copy(index, _dataBuffer, 0, _dataBuffer.Length);
            DllTools.FTW_4(_dataBuffer.Data, _outputBuffer.Data, _tmpBuffer);
            DataBlock.Copy(_outputBuffer, 0, buffer, startIndex, _outputBuffer.Length);

            // buffer[startIndex] = 0;
            return 1;
        }
        public void Init()
        {
            _dataBuffer = new DataBlock(DataBlockLength);
            _tmpBuffer = new double[DataBlockLength];
            _outputBuffer = new DataBlock(DataBlockLength);
        }


        public IDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
        }


        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                RateDataSourceParam param = new RateDataSourceParam(5);
                this._dataSource = value.Get(param);
            }
        }


        public ISensor Clone()
        {
            ISensor sen = MemberwiseClone() as ISensor;
            sen.Init();
            return sen;
        }
    }
    [Serializable]
    class RateFWT2Sensor : ISensor
    {
        private int _dataCount;
        private int _index;
        private int _outputCount;
        [NonSerialized]
        private IDataSource _dataSource;
        [NonSerialized]
        private DataBlock _dataBuffer;
        [NonSerialized]
        private DataBlock _outputBuffer;

        public int DataCollectLength = 4;
        private int CopyResult(DataBlock resultBuf, DataBlock outBuf, int len)
        {
            int blockLength = _dataCount;
            int outPos = 0;
            int copyOffset = 0;
            int copyLen;
            while(true)
            {
                if( blockLength == 1 )
                    return outPos;
                if( blockLength % 2 != 0)
                {
                    throw (new Exception("Param error!"));
                }
                blockLength /= 2;
                if( blockLength < len)
                {
                    copyLen = blockLength;
                }
                else
                {
                    copyLen = len;
                }
                int staPos = blockLength - copyLen;

                if (outBuf != null)
                    resultBuf.CopyTo(copyOffset + staPos, outBuf, outPos, copyLen);
                outPos += copyLen;

                if (outBuf != null)
                    resultBuf.CopyTo(copyOffset + blockLength + staPos, outBuf, outPos, copyLen);
                outPos += copyLen;

                copyOffset += blockLength * 2;
            }
        }
        public RateFWT2Sensor(int dataCount)
        {
            _dataCount = dataCount;
            _index = 0;
            Init();
        }
        public int CurrentPosition
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public int SkipCount
        {
            get { return _dataCount - 1; }
        }

        public int TotalLength
        {
            get { return _dataSource.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _outputCount; }
        }

        public int Copy(int index, DataBlock buffer, int startIndex)
        {
            _dataSource.Copy(index, _dataBuffer, 0, _dataBuffer.Length);
            DataBlock outTmp = new DataBlock(_dataBuffer.Length * 2);
            DllTools.FTW_5(_dataBuffer.Data, outTmp.Data);

            CopyResult(outTmp, _outputBuffer, DataCollectLength);
            DataBlock.Copy(_outputBuffer, 0, buffer, startIndex, _outputBuffer.Length);

            // buffer[startIndex] = 0;
            return 1;
        }
        public void Init()
        {
            _dataBuffer = new DataBlock(_dataCount);
            _outputCount = CopyResult(_dataBuffer, null, DataCollectLength);
            _outputBuffer = new DataBlock(_outputCount);
        }


        public IDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
        }


        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                RateDataSourceParam param = new RateDataSourceParam(5);
                this._dataSource = value.Get(param);
            }
        }


        public ISensor Clone()
        {
            ISensor sen = MemberwiseClone() as ISensor;
            sen.Init();
            return sen;
        }
    }
    [Serializable]
    class RateWaveletSensor : ISensor
    {
        private int _dataCount;
        private int _index;
        private int _outputCount;
        [NonSerialized]
        private IDataSource _dataSource;
       
        private int _dataCollectLength = 4;
        private WaveletFunction _wavelet;

        private int CopyResult(DataBlock resultBuf, DataBlock outBuf, int len)
        {
            int blockLength = _dataCount;
            int outPos = 0;
            int copyOffset = 0;
            int copyLen;
            while (true)
            {
                if (blockLength == 1)
                    return outPos;
                if (blockLength % 2 != 0)
                {
                    throw (new Exception("Param error!"));
                }
                blockLength /= 2;
                if (blockLength < len)
                {
                    copyLen = blockLength;
                }
                else
                {
                    copyLen = len;
                }
                int staPos = blockLength - copyLen;

                if (outBuf != null)
                    resultBuf.CopyTo(copyOffset + staPos, outBuf, outPos, copyLen);
                outPos += copyLen;

                if (outBuf != null)
                    resultBuf.CopyTo(copyOffset + blockLength + staPos, outBuf, outPos, copyLen);
                outPos += copyLen;

                copyOffset += blockLength * 2;
            }
        }
        public RateWaveletSensor(int dataCount, BasicWavelet waveletFunc, int dataCollectLength=4)
        {

            _wavelet = new WaveletFunction(waveletFunc);
            _dataCount = dataCount;
            _dataCollectLength = dataCollectLength;
            _index = 0;
            Init();
        }
        public int CurrentPosition
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public int SkipCount
        {
            get { return _dataCount - 1; }
        }

        public int TotalLength
        {
            get { return _dataSource.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _outputCount; }
        }

        public int Copy(int index, DataBlock buffer, int startIndex)
        {
            DataBlock _dataBuffer = new DataBlock(_dataCount);
            DataBlock _outputBuffer = new DataBlock(_outputCount);

            _dataSource.Copy(index, _dataBuffer, 0, _dataBuffer.Length);
            DataBlock outTmp = new DataBlock(_dataBuffer.Length * 2);
            DllTools.CalculateWavelet(_dataBuffer.Data, outTmp.Data, _wavelet);

            CopyResult(outTmp, _outputBuffer, _dataCollectLength);
            DataBlock.Copy(_outputBuffer, 0, buffer, startIndex, _outputBuffer.Length);

            // buffer[startIndex] = 0;
            return 1;
        }
        public void Init()
        {
            DataBlock _dataBuffer = new DataBlock(_dataCount);
            _outputCount = CopyResult(_dataBuffer, null, _dataCollectLength);
        }


        public IDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
        }


        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                RateDataSourceParam param = new RateDataSourceParam(5);
                this._dataSource = value.Get(param);
            }
        }


        public ISensor Clone()
        {
            ISensor sen = MemberwiseClone() as ISensor;
            sen.Init();
            return sen;
        }
    }

    //===========================================
    [Serializable]
    abstract class BasicKDJSensor : ISensor
    {
        [NonSerialized]
        protected KDJDataSource _dataSource;
        private int _blockLength;

        public int AverageRange = 9;

        public BasicKDJSensor(int blockLength)
        {
            _blockLength = blockLength;
        }

        public int SkipCount
        {
            get { return _dataSource.SkipCount + _blockLength - 1; }
        }

        public int TotalLength
        {
            get
            {
                if (_dataSource == null)
                    return 0;
                else
                    return _dataSource.TotalLength;
            }
        }

        public int DataBlockLength
        {
            get { return _blockLength; }
        }

        public IDataSource DataSource
        {
            get { return _dataSource; }
        }

        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                KDJDataSourceParam param = new KDJDataSourceParam() { AveRange = AverageRange };
                _dataSource = value.Get(param) as KDJDataSource;
            }
        }

        abstract public int Copy(int index, DataBlock buffer, int offset);

        public void Init()
        {
            // Todo nothing.
            return;
        }


        public ISensor Clone()
        {
            return MemberwiseClone() as ISensor;
        }
    }

    [Serializable]
    class KDJ_KSensor : BasicKDJSensor
    {
        public KDJ_KSensor(int blockLength): base(blockLength)
        {

        }

        public override int Copy(int index, DataBlock buffer, int offset)
        {
            _dataSource.CopyK(index, buffer, offset, this.DataBlockLength);
            return this.DataBlockLength;
        }
    }

    [Serializable]
    class KDJ_DSensor : BasicKDJSensor
    {
        public KDJ_DSensor(int blockLength): base(blockLength)
        {

        }

        public override int Copy(int index, DataBlock buffer, int offset)
        {
            _dataSource.CopyD(index, buffer, offset, this.DataBlockLength);
            return this.DataBlockLength;
        }
    }

    [Serializable]
    class KDJ_JSensor : BasicKDJSensor
    {
        public KDJ_JSensor(int blockLength): base(blockLength)
        {

        }

        public override int Copy(int index, DataBlock buffer, int offset)
        {
            _dataSource.CopyJ(index, buffer, offset, this.DataBlockLength);
            return this.DataBlockLength;
        }
    }
    [Serializable]
    class KDJ_KDCrossSensor : BasicKDJSensor
    {
        public KDJ_KDCrossSensor(int blockLength)
            : base(blockLength)
        {

        }

        public override int Copy(int index, DataBlock buffer, int offset)
        {
            for (int i = 0; i < DataBlockLength; i++)
            {
                int idx = index + i - DataBlockLength + 1;
                buffer[offset + i] = _dataSource.KArr[idx] - _dataSource.DArr[idx];
            } 
            return this.DataBlockLength;
        }
    }
    [Serializable]
    class KDJ_DJCrossSensor : BasicKDJSensor
    {
        public KDJ_DJCrossSensor(int blockLength)
            : base(blockLength)
        {

        }

        public override int Copy(int index, DataBlock buffer, int offset)
        {
            for (int i = 0; i < DataBlockLength; i++)
            {
                int idx = index + i - DataBlockLength + 1;
                buffer[offset + i] = _dataSource.DArr[idx] - _dataSource.JArr[idx];
            }
            return this.DataBlockLength;
        }
    }
    [Serializable]
    class KDJ_KJCrossSensor : BasicKDJSensor
    {
        public KDJ_KJCrossSensor(int blockLength)
            : base(blockLength)
        {

        }

        public override int Copy(int index, DataBlock buffer, int offset)
        {
            for (int i = 0; i < DataBlockLength; i++)
            {
                int idx = index + i - DataBlockLength + 1;
                buffer[offset + i] = _dataSource.KArr[idx] - _dataSource.JArr[idx];
            }
            return this.DataBlockLength;
        }
    }

    [Serializable]
    class KDJ_CrossSensor : BasicKDJSensor
    {
        public KDJ_CrossSensor(int blockLength)
            : base(blockLength)
        {

        }

        public override int Copy(int index, DataBlock buffer, int offset)
        {
            for (int i = 0; i < DataBlockLength; i++)
            {
                int idx = index+i - DataBlockLength + 1;
                double value = 0;
                value += Math.Abs(_dataSource.KArr[idx] - _dataSource.DArr[idx]);
                value += Math.Abs(_dataSource.DArr[idx] - _dataSource.JArr[idx]);
                value += Math.Abs(_dataSource.JArr[idx] - _dataSource.KArr[idx]);
                buffer[offset + i] = value;
            }
            return this.DataBlockLength;
        }
    }

    //========================================================================================

    [Serializable]
    class FirstDerivative : ISensor
    {
        private ISensor _source;
        public FirstDerivative(ISensor source)
        {
            _source = source;
        }

        public int SkipCount
        {
            get { return _source.SkipCount + DataBlockLength - 1; }
        }

        public int TotalLength
        {
            get { return _source.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _source.DataBlockLength - 1; }
        }

        public IDataSource DataSource
        {
            get { return _source.DataSource; }
        }

        public DataSourceCtrl DataSourceCtrl
        {
            set { _source.DataSourceCtrl = value; }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            DataBlock data = new DataBlock(_source.DataBlockLength);
            _source.Copy(index, data, 0);
            for(int i=0;i<DataBlockLength;i++)
            {
                buffer[offset + i] = data[i + 1] - data[i];
            }
            return DataBlockLength;
        }


        public void Init()
        {
            // Todo nothing
        }

        public ISensor Clone()
        {
            return new FirstDerivative(_source);
        }
    }


    [Serializable]
    class SecondDerivative : ISensor
    {
        private ISensor _source;
        public SecondDerivative(ISensor source)
        {
            _source = source;
        }

        public int SkipCount
        {
            get { return _source.SkipCount + DataBlockLength - 1; }
        }

        public int TotalLength
        {
            get { return _source.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _source.DataBlockLength - 2; }
        }

        public IDataSource DataSource
        {
            get { return _source.DataSource; }
        }

        public DataSourceCtrl DataSourceCtrl
        {
            set { _source.DataSourceCtrl = value; }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            DataBlock data = new DataBlock(_source.DataBlockLength);
            _source.Copy(index, data, 0);

            for (int i = 0; i < data.Length-1; i++)
            {
                data[i] = data[i + 1] - data[i];
            }

            for (int i = 0; i < data.Length -2; i++)
            {
                buffer[offset + i] = data[i + 1] - data[i];
            }

            return data.Length - 2;
        }

        public void Init()
        {
            // Todo Nothing
        }

        public ISensor Clone()
        {
            return new SecondDerivative(_source);
        }
    }

    [Serializable]
    class SensorCross : ISensor
    {
        private ISensor _sen1;
        private ISensor _sen2;
        private int _outputLen;
        public SensorCross(ISensor sen1, ISensor sen2, int outputLen)
        {
            _sen1 = sen1;
            _sen2 = sen2;
            _outputLen = outputLen;
             // TODO
        }
        public int SkipCount
        {
            get { return _sen1.SkipCount + _outputLen - 1; }
        }

        public int TotalLength
        {
            get { return _sen1.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _outputLen; }
        }

        public IDataSource DataSource
        {
            get { return _sen1.DataSource; }
        }

        public DataSourceCtrl DataSourceCtrl
        {
            set 
            {
                _sen1.DataSourceCtrl = value;
                _sen2.DataSourceCtrl = value;
            }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            DataBlock data1 = new DataBlock(_sen1.DataBlockLength);
            DataBlock data2 = new DataBlock(_sen1.DataBlockLength);
            _sen1.Copy(index, data1, 0);
            _sen2.Copy(index, data2, 0);

            int dstOffset = _sen1.DataBlockLength - this.DataBlockLength;
            for (int i = 0; i < this.DataBlockLength; i++)
            {
                buffer[offset + i] = data1[i + dstOffset] - data2[i + dstOffset];
            }
            return DataBlockLength;
        }

        public void Init()
        {
            // Todo nothing
        }

        public ISensor Clone()
        {
            return new SensorCross(_sen1, _sen2, _outputLen);
        }
    }

    abstract class BasicCrossPartten
    {
        abstract public ISensor GetPartten(ISensor[] sensorArr);
    }
    class CrossPartten01 : BasicCrossPartten
    {
        public override ISensor GetPartten(ISensor[] sensorArr)
        {
            SensorGroup senGroup = new SensorGroup();

            // For single data line;
            foreach (ISensor sen in sensorArr)
            {
                senGroup.Add(new FirstDerivative(sen));
                senGroup.Add(new SecondDerivative(sen));
            }

            // For two data line
            for (int i = 0; i < sensorArr.Length - 1; i++)
            {
                for (int j = i + 1; j < sensorArr.Length; j++)
                {
                    ISensor sen1 = sensorArr[i];
                    ISensor sen2 = sensorArr[j];

                    senGroup.Add(new SensorCross(sen1, sen2, 1));

                    senGroup.Add(new FirstDerivative(new SensorCross(sen1, sen2, 2)));
                    senGroup.Add(new SecondDerivative(new SensorCross(sen1, sen2, 2)));

                    senGroup.Add(new SensorCross(new FirstDerivative(sen1), new FirstDerivative(sen2), 1));
                    senGroup.Add(new SensorCross(new SecondDerivative(sen1), new SecondDerivative(sen2), 1));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new FirstDerivative(sen1),
                            new FirstDerivative(sen2),
                            2)
                        ));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new SecondDerivative(sen1),
                            new SecondDerivative(sen2),
                            2)
                        ));

                    senGroup.Add(new SecondDerivative(
                            new SensorCross(
                            new FirstDerivative(sen1),
                            new FirstDerivative(sen2),
                            2)
                        ));

                    senGroup.Add(new SecondDerivative(
                            new SensorCross(
                            new SecondDerivative(sen1),
                            new SecondDerivative(sen2),
                            2)
                        ));

                }
            }
            return senGroup;
        }
    }
    class CrossPartten02 : BasicCrossPartten
    {
        public override ISensor GetPartten(ISensor[] sensorArr)
        {
            SensorGroup senGroup = new SensorGroup();

            // For single data line;
            foreach (ISensor sen in sensorArr)
            {
                senGroup.Add(new FirstDerivative(sen));
                senGroup.Add(new SecondDerivative(sen));
            }

            // For two data line
            for (int i = 0; i < sensorArr.Length - 1; i++)
            {
                for (int j = i + 1; j < sensorArr.Length; j++)
                {
                    ISensor sen1 = sensorArr[i];
                    ISensor sen2 = sensorArr[j];

                    senGroup.Add(new SensorCross(sen1, sen2, 4));

                    senGroup.Add(new FirstDerivative(new SensorCross(sen1, sen2, 4)));
                    senGroup.Add(new SecondDerivative(new SensorCross(sen1, sen2, 8)));

                    senGroup.Add(new SensorCross(new FirstDerivative(sen1), new FirstDerivative(sen2), 4));
                    senGroup.Add(new SensorCross(new SecondDerivative(sen1), new SecondDerivative(sen2), 4));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new FirstDerivative(sen1),
                            new FirstDerivative(sen2),
                            4)
                        ));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new SecondDerivative(sen1),
                            new SecondDerivative(sen2),
                            4)
                        ));

                    senGroup.Add(new SecondDerivative(
                            new SensorCross(
                            new FirstDerivative(sen1),
                            new FirstDerivative(sen2),
                            8)
                        ));

                    senGroup.Add(new SecondDerivative(
                            new SensorCross(
                            new SecondDerivative(sen1),
                            new SecondDerivative(sen2),
                            8)
                        ));

                }
            }
            return senGroup;
        }
    }
    class CrossPartten03 : BasicCrossPartten
    {
        public override ISensor GetPartten(ISensor[] sensorArr)
        {
            SensorGroup senGroup = new SensorGroup();

            // For single data line;
            foreach (ISensor sen in sensorArr)
            {
                senGroup.Add(new FirstDerivative(sen));
                senGroup.Add(new SecondDerivative(sen));
            }

            // For two data line
            for (int i = 0; i < sensorArr.Length - 1; i++)
            {
                for (int j = i + 1; j < sensorArr.Length; j++)
                {
                    ISensor sen1 = sensorArr[i];
                    ISensor sen2 = sensorArr[j];

                    senGroup.Add(new SensorCross(sen1, sen2, 2));

                    senGroup.Add(new FirstDerivative(new SensorCross(sen1, sen2, 2)));

                    senGroup.Add(new SensorCross(new FirstDerivative(sen1), new FirstDerivative(sen2), 2));
                    senGroup.Add(new SensorCross(new SecondDerivative(sen1), new SecondDerivative(sen2), 2));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new FirstDerivative(sen1),
                            new FirstDerivative(sen2),
                            2)
                        ));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new SecondDerivative(sen1),
                            new SecondDerivative(sen2),
                            2)
                        ));
                }
            }
            return senGroup;
        }
    }

    class CrossPartten04 : BasicCrossPartten
    {
        public override ISensor GetPartten(ISensor[] sensorArr)
        {
            SensorGroup senGroup = new SensorGroup();

            // For two data line
            for (int i = 0; i < sensorArr.Length - 1; i++)
            {
                for (int j = i + 1; j < sensorArr.Length; j++)
                {
                    ISensor sen1 = sensorArr[i];
                    ISensor sen2 = sensorArr[j];

                    senGroup.Add(new SensorCross(sen1, sen2, 2));

                    senGroup.Add(new FirstDerivative(new SensorCross(sen1, sen2, 2)));

                    senGroup.Add(new SensorCross(new FirstDerivative(sen1), new FirstDerivative(sen2), 2));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new FirstDerivative(sen1),
                            new FirstDerivative(sen2),
                            2)
                        ));
                }
            }
            return senGroup;
        }
    }
    class CrossPartten05 : BasicCrossPartten
    {
        public override ISensor GetPartten(ISensor[] sensorArr)
        {
            SensorGroup senGroup = new SensorGroup();

            // For two data line
            for (int i = 0; i < sensorArr.Length - 1; i++)
            {
                for (int j = i + 1; j < sensorArr.Length; j++)
                {
                    ISensor sen1 = sensorArr[i];
                    ISensor sen2 = sensorArr[j];

                    senGroup.Add(new SensorCross(sen1, sen2, 3));

                    senGroup.Add(new SensorCross(new FirstDerivative(sen1), new FirstDerivative(sen2), 1));

                    senGroup.Add(new FirstDerivative(
                            new SensorCross(
                            new FirstDerivative(sen1),
                            new FirstDerivative(sen2),
                            1)
                        ));
                }
            }
            return senGroup;
        }
    }


    //======================================
    [Serializable]
    class WaveletSensor : ISensor
    {
        private int _dataCount;
        private int _index;
        private int _outputCount;
        private ISensor _source;

        private int _dataCollectLength = 4;
        private WaveletFunction _wavelet;

        private int CopyResult(DataBlock resultBuf, DataBlock outBuf, int len)
        {
            int blockLength = _dataCount;
            int outPos = 0;
            int copyOffset = 0;
            int copyLen;
            while (true)
            {
                if (blockLength == 1)
                    return outPos;
                if (blockLength % 2 != 0)
                {
                    throw (new Exception("Param error!"));
                }
                blockLength /= 2;
                if (blockLength < len)
                {
                    copyLen = blockLength;
                }
                else
                {
                    copyLen = len;
                }
                int staPos = blockLength - copyLen;

                if (outBuf != null)
                    resultBuf.CopyTo(copyOffset + staPos, outBuf, outPos, copyLen);
                outPos += copyLen;

                if (outBuf != null)
                    resultBuf.CopyTo(copyOffset + blockLength + staPos, outBuf, outPos, copyLen);
                outPos += copyLen;

                copyOffset += blockLength * 2;
            }
        }
        public WaveletSensor(ISensor source, BasicWavelet waveletFunc, int dataCollectLength = 4)
        {
            _wavelet = new WaveletFunction(waveletFunc);
            _source = source;
            _dataCount = source.DataBlockLength;
            _dataCollectLength = dataCollectLength;
            _index = 0;
            Init();
        }
        public int CurrentPosition
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public int SkipCount
        {
            get { return _dataCount - 1; }
        }

        public int TotalLength
        {
            get { return _source.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _outputCount; }
        }

        public int Copy(int index, DataBlock buffer, int startIndex)
        {
            DataBlock _dataBuffer = new DataBlock(_dataCount);
            DataBlock _outputBuffer = new DataBlock(_outputCount);

            _source.Copy(index, _dataBuffer, 0);
            DataBlock outTmp = new DataBlock(_dataBuffer.Length * 2);
            DllTools.CalculateWavelet(_dataBuffer.Data, outTmp.Data, _wavelet);

            CopyResult(outTmp, _outputBuffer, _dataCollectLength);
            DataBlock.Copy(_outputBuffer, 0, buffer, startIndex, _outputBuffer.Length);

            // buffer[startIndex] = 0;
            return 1;
        }
        public void Init()
        {
            DataBlock _dataBuffer = new DataBlock(_dataCount);
            _outputCount = CopyResult(_dataBuffer, null, _dataCollectLength);
        }


        public IDataSource DataSource
        {
            get
            {
                return _source.DataSource;
            }
        }


        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                _source.DataSourceCtrl = value;
            }
        }


        public ISensor Clone()
        {
            ISensor sen = MemberwiseClone() as ISensor;
            sen.Init();
            return sen;
        }
    }


    [Serializable]
    class SensorYieldRate : ISensor
    {
        private ISensor _sourceSen;

        public SensorYieldRate(ISensor srcSensor)
        {
            _sourceSen = srcSensor;
        }
        public int SkipCount
        {
            get { return _sourceSen.SkipCount + 1; }
        }

        public int TotalLength
        {
            get { return _sourceSen.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _sourceSen.DataBlockLength - 1; }
        }

        public IDataSource DataSource
        {
            get { return _sourceSen.DataSource; }
        }

        public DataSourceCtrl DataSourceCtrl
        {
            set { _sourceSen.DataSourceCtrl = value; }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            DataBlock tmp = new DataBlock(_sourceSen.DataBlockLength);
            _sourceSen.Copy(index, tmp, 0);

            for(int i=0;i<tmp.Length-1;i++)
            {
                buffer[offset + i] = Math.Log(tmp[i + 1] / tmp[i]);
            }

            return tmp.Length - 1;
        }

        public void Init()
        {
            return;
        }

        public ISensor Clone()
        {
            return MemberwiseClone() as ISensor;
        }
    }


    class SensorUtility
    {

        private static ISensor[] GetKDJSensor(int dataLen, int averageRange)
        {
            ISensor[] sensorArr = new ISensor[]
            {

                new KDJ_KSensor(dataLen){ AverageRange = averageRange },
                new KDJ_JSensor(dataLen){ AverageRange = averageRange },
                new KDJ_DSensor(dataLen){ AverageRange = averageRange },
            };
            return sensorArr;
        }
        public static ISensor GetKDJCrossSensor(int dataLen, int[] averageRangeArr, BasicCrossPartten parrten)
        {
            SensorGroup senGroup = new SensorGroup();

            // Add KDJ Sensor
            List<ISensor> crossSenList = new List<ISensor>();
            foreach(int aveRng in averageRangeArr)
            {
                foreach (ISensor sen in GetKDJSensor(dataLen, aveRng))
                    crossSenList.Add(sen);
            }

            // Add Rate for crossing
            crossSenList.Add(new RateSensor(dataLen));

            senGroup.Add(parrten.GetPartten(crossSenList.ToArray()));
            senGroup.Add(new KDJ_KSensor(dataLen));
            senGroup.Add(new KDJ_DSensor(dataLen));
            senGroup.Add(new KDJ_JSensor(dataLen));

            return senGroup;
        }
    }

}
