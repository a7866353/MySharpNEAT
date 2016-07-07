using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyProject01.Networks;
using MyProject01.TrainingMethods;
using MyProject01.TestParameters;
using MyProject01.Test;
using MyProject01.Util;
using MyProject01.Controller;
using MyProject01.Agent;


namespace MyProject01.Reinforcement
{
    class QLearn : IRateMarketUser
    {
        public double _totalError;
        public int _totalCount;
        private List<double[]> _previousStateList;
        private List<double[]> _outputDataArrayList;
        private int _previousOutputSelete;
        public MyNet network;
        private Random rand;

        private int _greedRate = 95;
        private double _discountRate = 0.80;
        private double _scaleRate = 1000;
        private double _learnRate = 0.7;
        // Paramters

        public QLearn(MyNet network)
        {
            rand = new Random();
            _previousOutputSelete = -1;

            this._previousStateList = new List<double[]>();
            this._outputDataArrayList = new List<double[]>();


            _totalError = 0;
            _totalCount = 0;

            this.network = network;
        }

        MarketActions IRateMarketUser.Determine(RateMarketAgentData state)
        {
            int actionIndex;
            int greedActionIndex;

            // Calcute Q Value
            double[] actionQValueArr = network.Compute(state.RateDataArray);

            // Find greed action
            double maxValue = actionQValueArr[0];
            greedActionIndex = 0;
            for (int i = 1; i < actionQValueArr.Length; i++)
            {
                if (actionQValueArr[i] <= maxValue)
                    continue;
                maxValue = actionQValueArr[i];
                greedActionIndex = i;
            }

            // Choose a action.
            if (rand.Next(100) < _greedRate)
            {
                // Greed action
                actionIndex = greedActionIndex;
            }
            else
            {
                // Explorer action
                actionIndex = rand.Next(actionQValueArr.Length);
            }

            // Learn
            if (_previousStateList.Count != 0)
            {
                double error;
                int index = _previousStateList.Count - 1;
                double value = _outputDataArrayList[_previousStateList.Count - 1][_previousOutputSelete];
                _outputDataArrayList[_previousStateList.Count - 1][_previousOutputSelete] = ((1 - _learnRate) * value) + _learnRate * (state.Reward + _discountRate * actionQValueArr[greedActionIndex]);
                error = network.Training(_previousStateList.ToArray(), _outputDataArrayList.ToArray());
                // error = network.Training(_previousStateList[index], _outputDataArrayList[index]);
                // LogFile.WriteLine(error.ToString("G"));
                _totalError += error;
                _totalCount++;
            }

            _previousStateList.Add(state.RateDataArray);
            _outputDataArrayList.Add(actionQValueArr);

            _previousOutputSelete = actionIndex;

            switch(actionIndex)
            {
                case 0:
                    return MarketActions.Buy;
                case 1:
                    return MarketActions.Nothing;
                case 2:
                    return MarketActions.Sell;
                default: 
                    return MarketActions.Nothing;
            }
        }

        double IRateMarketUser.TotalErrorRate
        {
            get
            {
                double errorSum = 0;
                foreach (double d in _outputDataArrayList[_outputDataArrayList.Count-1])
                    errorSum += Math.Abs(d);
                errorSum /= _outputDataArrayList[_outputDataArrayList.Count-1].Length;
                return _totalError / errorSum / _totalCount;
            }
            set
            {
                _totalError = 0; _totalCount = 0;
            }
        }

        public void EpsodeEnd()
        {
            network.SaveNetwork();
            _totalError = 0; _totalCount = 0;
        }
    }
}
