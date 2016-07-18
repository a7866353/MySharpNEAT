using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller.Jobs
{
    class TrainDataChangeJob : ICheckJob
    {
        private AgentFactory _agentFac;
        private int _startPosition;
        private int _stepOffset;
        private int _maxPos;
        private int _trainCount = 2;

        private int _currentStartPos;

        public TrainDataChangeJob(AgentFactory agentFac, int startPos, int len, int stepOffset,  int trainCount)
        {
            _agentFac = agentFac;
            _startPosition = startPos;
            _stepOffset = stepOffset;
            _trainCount = trainCount;

            _currentStartPos = _startPosition;
            _maxPos = _startPosition + len;
        }
        public bool Do(TrainerContex context)
        {
            if (context.Epoch % _trainCount != 0)
                return true;

            if (_currentStartPos >= _maxPos)
            {
                _currentStartPos = _startPosition;
            }
            else
            {
                _currentStartPos = Math.Min(_currentStartPos + _stepOffset, _maxPos);
            }

            _agentFac.StartPosition = _currentStartPos;

            return true;
        }
    }
}
