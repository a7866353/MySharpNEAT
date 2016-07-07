using Encog.Neural.Networks.Training;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{

    class DataUpdateJob : ICheckJob
    {
        private ReduceLossScore _score;
        private int _stepLength = 2;
        private BasicDataBlock _trainDataBlock;
        private int _trainLength = 12*12;
        private int _trainCountMax = 50;

        private int _startIndex;
        private int _trainCount;

        public ICalculateScore Score
        {
            get { return _score; }
        }

        public DataUpdateJob(BasicDataBlock trainDataBlock, ReduceLossScore score)
        {
            _startIndex = 0;
            _trainCount = 0;
            _trainDataBlock = trainDataBlock;
            this._score = score;
            Next();
        }

        public bool Do(Jobs.TrainerContex context)
        {
            _trainCount++;
            if( _trainCount > _trainCountMax)
            {
                _trainCount = 0;
                Next();
                return true;
            }
            return false;
        }

        private void Next()
        {
            _score.dataBlock = _trainDataBlock.GetNewBlock(_startIndex, _trainLength);
            _startIndex += _stepLength;
            if ((_startIndex + _trainLength) >= _trainDataBlock.BlockCount)
                _startIndex = 0;
        }
    }
}
