using MyProject01.Test;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyProject01.Util.DataObject;
using MyProject01.Controller;
using MyProject01.ExchangeRateTrade;
using MyProject01.DataSources;

namespace MyProject01.Agent
{
    class DataNormallizer
    {
        private double _targDataMax;
        private double _targDataMin;
        private double _dataMaxValue;
        private double _dataMinValue;
        private double[] _dataArr;

        private double _dataScale;
        private double _dataOffset;

        public void DataValueAdjust(double[] dataArr, double max, double min)
        {
            _dataArr = dataArr;
            _targDataMax = max;
            _targDataMin = min;

            _dataMaxValue = _dataMinValue = _dataArr[0];
            foreach (double data in _dataArr)
            {
                if (data > _dataMaxValue)
                    _dataMaxValue = data;
                else if (data < _dataMinValue)
                    _dataMinValue = data;
            }

            _dataScale = (_targDataMax - _targDataMin) / (_dataMaxValue - _dataMinValue);
            _dataOffset = (_dataMaxValue - _dataMinValue) * _targDataMax / (_targDataMax - _targDataMin) - _dataMaxValue;

            Normallize(_dataOffset, _dataScale);
        }
        public void Normallize(double offset, double scale)
        {
            _dataOffset = offset;
            _dataScale = scale;
            for (int i = 0; i < _dataArr.Length;i++ )
            {
                _dataArr[i] = DataConv(_dataArr[i], _dataOffset, _dataScale);
            }
        }
        private double DataConv(double value, double offset, double scale)
        {
            return (value + offset) * scale;
        }
    }
    class RateMarketAgentData
    {
        public double[] RateDataArray;
        public double Reward;

        public double InitMoney;
        public double CurrentMoney;
        public double TotalBenifit;
        public double TotalBenifitRate;

        public OrderLog LastOrderLog;


    }

    interface IRateMarketUser
    {
        MarketActions Determine(RateMarketAgentData inputData);
        double TotalErrorRate{ set; get; }

        void EpsodeEnd();
    }

    class Order
    {
#if false
        public double BuyOffset = 0.0;
        public double SellOffset = 0.0;
#else
        public double BuyOffset = 0.02;
        public double SellOffset = 0.01;

#endif
        private double _money;
        private OrderType _type;
        private MarketActions _lastAction;
        private double _startRate;
        private double _startRatePre;
        private double _currentRate;

        private int _dealCount;

        public Order(double initMoney)
        {
            _money = initMoney;
            _type = OrderType.Nothing;
            _lastAction = MarketActions.Init;
            _startRate = 0;
            _dealCount = 0;
        }
        public int DealCount
        {
            get { return _dealCount; }
        }
        public MarketActions LastAction
        {
            get { return _lastAction; }
        }
        public double GetCurrentMoney(double currentRate)
        {
            _currentRate = currentRate;
            return _money + Benifit();
            // return _money;
        }
        public double GetCurrentBenifit(double currentRate)
        {
            _currentRate = currentRate;
            return Benifit();
        }
        public OrderLog StartOrder(OrderType type, double currentRate)
        {
            OrderLog log = null;
            _currentRate = currentRate;
            _lastAction = MarketActions.Nothing;
            if (_type == type)
                return log;
            else if (_type != type && (_type != OrderType.Nothing))
                log = CloseOrder(currentRate);
            _dealCount++;
            _type = type;
            _startRatePre = _startRate;
            if (_type == OrderType.BuyOrder)
            {
                _startRate = Ask();
                _lastAction = MarketActions.Buy;
            }
            else
            {
                _startRate = Bid();
                _lastAction = MarketActions.Sell;
            }

            return log;
        }
        public OrderLog CloseOrder(double currentRate)
        {
            _currentRate = currentRate;

            OrderLog log = new OrderLog()
            {
                BenifitMoney = Benifit(),
                BenifitRate = Benifit() / _money,
                StartPrice = _startRate,
                ClosePrice = _currentRate,
            };

            _money += Benifit();
            _type = OrderType.Nothing;
            _lastAction = MarketActions.Close;

            return log;
        }

        private double Benifit()
        {
            if (_type == OrderType.Nothing)
                return 0;
            else if (_type == OrderType.BuyOrder)
                return (Bid() / _startRate - 1) * _money;
            else // for sell order
                return (_startRate / Ask() - 1) * _money;
        }
        
        private double Bid()
        {
            return _currentRate - SellOffset;
        }
        private double Ask()
        {
            return _currentRate + BuyOffset;
        }
        public enum OrderType
        {
            Nothing,
            BuyOrder,
            SellOrder,
        }

    }

    class OrderLog
    {
        public double StartPrice;
        public double ClosePrice;
        public double BenifitMoney;
        public double BenifitRate;
    }

    class RateMarketAgent
    {
        public int index = 0;
        public double InitMoney = 10000;

        private BasicDataBlock _dataBlock;
        private Order _order;
        private long _step;
        private RateMarketAgentData _stateData = new RateMarketAgentData();

        public long Step
        {
            get
            {
                return _step;
            }
        }

        public bool IsEnd { private set; get; }

        public double CurrentRateValue { get { return _dataBlock.GetRate(index); } }
        public double CurrentValue
        {
            get
            {
                return _order.GetCurrentMoney(_currentRate);
            }
        }
        public MarketActions LastAction
        {
            get
            {
                return _order.LastAction;
            }
        }
        public int DealCount
        {
            get { return _order.DealCount; }
        }
        public RateMarketAgent(BasicDataBlock dataBlock)
        {
            _stateData = new RateMarketAgentData();
            _dataBlock = dataBlock.Clone() ;
            _order = new Order(InitMoney);
            _stateData.RateDataArray = new double[_dataBlock.BlockLength];

            Reset();
        }

        public RateMarketAgentData Reset()
        {
            _step = 1;
            _order = new Order(InitMoney);
            index = 0;
            IsEnd = false;

            GetArrayValue(_stateData.RateDataArray, index);
            _stateData.Reward = 0;
            _stateData.InitMoney = InitMoney;
            _stateData.CurrentMoney = InitMoney;
            _stateData.TotalBenifit = 0;
            return _stateData;
        }

        public bool Next()
        {
            if (IsEnd == true)
                return false;
            if((index+1) >=  _dataBlock.BlockCount)
            {
                IsEnd = true;
                return false;
            }

            index++;
            _step++;
            return true;
        }
        public RateMarketAgentData TakeAction(MarketActions action)
        {
            OrderLog log = null;
            switch (action)
            {
                case MarketActions.Buy:
                    log = _order.StartOrder(Order.OrderType.BuyOrder, _currentRate);
                    break;
                case MarketActions.Sell:
                    log = _order.StartOrder(Order.OrderType.SellOrder, _currentRate);
                    break;
                case MarketActions.Close:
                    log = _order.CloseOrder(_currentRate);
                    break;
                case MarketActions.Nothing:
                    break;
                default:
                    break;
            }
            _stateData.CurrentMoney = CurrentValue;
            _stateData.TotalBenifit = CurrentValue - InitMoney;
            _stateData.TotalBenifitRate = (CurrentValue - InitMoney) / InitMoney;
            _stateData.LastOrderLog = log;
            _stateData.Reward = (CurrentValue - InitMoney) / InitMoney;
            GetArrayValue(_stateData.RateDataArray, index);
            return _stateData;
        }

        private double _currentRate
        {
            get { return _dataBlock.GetRate(index); }
        }
        
        private void GetArrayValue(double[] buffer, int offset)
        {
            _dataBlock.Copy(buffer, offset);
            return;
        }
    }

    class LearnRateMarketAgent
    {
        public double InitMoney = 10000;

        private Order _order;
        private long _step;
        private RateMarketAgentData _stateData;
        private TradeAnalzeLog _tradeLog;
        private IController _ctrl;
        private int _endPosition;
        public long CurrentIndex
        {
            get
            {
                return _ctrl.CurrentPosition;
            }
        }

        public bool IsEnd { private set; get; }

        public double CurrentRateValue
        {
            get { return _ctrl.CurrentRateSet.Close; }
        }
        public double CurrentValue
        {
            get
            {
                return _order.GetCurrentMoney(CurrentRateValue);
            }
        }
        public MarketActions LastAction
        {
            get
            {
                return _order.LastAction;
            }
        }
        public int DealCount
        {
            get { return _order.DealCount; }
        }
        public TradeAnalzeLog TradeLog
        {
            get { return _tradeLog; }
        }
        public bool SetRange(int startPos, int endPos)
        {
            int sta = Math.Max(startPos, _ctrl.SkipCount);
            int end = Math.Min(endPos+1, _ctrl.TotalLength);
            if (end <= sta)
                return false;

            _ctrl.CurrentPosition = sta;
            _endPosition = end;
            return true;
        }
        public LearnRateMarketAgent(IController ctrl)
        {
            _ctrl = ctrl;
            _stateData = new RateMarketAgentData();
            _order = new Order(InitMoney);
            _tradeLog = new TradeAnalzeLog();
            _endPosition = _ctrl.TotalLength;

            Reset();
        }

        public bool Next()
        {
            if (IsEnd == true)
                return false;
            if ((_ctrl.CurrentPosition + 1) >= _endPosition) 
            {
                IsEnd = true;
                return false;
            }

            _ctrl.CurrentPosition++;
            _step++;
            return true;
        }
        private RateMarketAgentData TakeAction(MarketActions action)
        {
            OrderLog log = null;
            switch (action)
            {
                case MarketActions.Buy:
                    log = _order.StartOrder(Order.OrderType.BuyOrder, CurrentRateValue);
                    break;
                case MarketActions.Sell:
                    log = _order.StartOrder(Order.OrderType.SellOrder, CurrentRateValue);
                    break;
                case MarketActions.Close:
                    log = _order.CloseOrder(CurrentRateValue);
                    break;
                case MarketActions.Nothing:
                    break;
                default:
                    break;
            }
            _stateData.CurrentMoney = CurrentValue;
            _stateData.TotalBenifit = CurrentValue - InitMoney;
            _stateData.TotalBenifitRate = (CurrentValue - InitMoney) / InitMoney;
            _stateData.LastOrderLog = log;
            _stateData.Reward = (CurrentValue - InitMoney) / InitMoney;
            return _stateData;
        }
        public RateMarketAgentData Reset()
        {
            _step = 1;
            _order = new Order(InitMoney);
            IsEnd = false;

            // GetArrayValue(_stateData.RateDataArray, index);
            _stateData.Reward = 0;
            _stateData.InitMoney = InitMoney;
            _stateData.CurrentMoney = InitMoney;
            _stateData.TotalBenifit = 0;

            _ctrl.CurrentPosition = _ctrl.SkipCount;

            return _stateData;
        }
        public void DoAction()
        {
            if (IsEnd == true)
                return;

            if (CurrentRateValue > 0)
            {
                // Get Action Value
                MarketActions _currentAction = _ctrl.GetAction();
                TakeAction(_currentAction);
                _tradeLog.SetStateData(_stateData);
            }
        }
    }

}
