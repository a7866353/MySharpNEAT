using MyProject01.Agent;
using MyProject01.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.ExchangeRateTrade
{
      class TradeController
    {
        private RateMarketAgent _agent;
        private ITradeDesisoin _decisionCtl;
        private RateMarketAgentData stateData;
        private MarketActions _currentAction;
        private TradeAnalzeLog _tradeLog;

        public RateMarketAgent Agent
        {
            get { return _agent; }
        }
        public MarketActions LastAction
        {
            get { return _currentAction; }
        }

        public TradeAnalzeLog TradeLog
        {
            get { return _tradeLog; }
        }

        public TradeController(RateMarketAgent agent, ITradeDesisoin decision)
        {
            _agent = agent;
            _decisionCtl = decision;
            _tradeLog = new TradeAnalzeLog();
            stateData = _agent.Reset();
        }
        public void DoAction()
        {
            _currentAction = MarketActions.Init;
            if (_agent.IsEnd == true)
                return;

            if (_agent.CurrentRateValue > 0)
            {
                // Get Action Value
                _currentAction = ChoseAction(stateData.RateDataArray);
                stateData = _agent.TakeAction(_currentAction);
                _tradeLog.SetStateData(stateData);
                _agent.Next();
            }
        }

        private MarketActions ChoseAction(double[] rateDataArray)
        {
            MarketActions currentAction = _decisionCtl.GetAction(rateDataArray);
            return currentAction;
        }
    }

    class TradeAnalzeLog
    {
        public double MaxProfit;
        public double MaxLoss;
        public double BestTradeRate;
        public double WorstTradeRate;
        public double GrossProfit;
        public double GrossLoss;

        private List<OrderLog> _orderLogList;
        private bool _isInited;

        public TradeAnalzeLog()
        {
            _orderLogList = new List<OrderLog>();
            MaxProfit = 0;
            MaxLoss = 0;
            BestTradeRate = 0;
            WorstTradeRate = 0;
            GrossProfit = 0;
            GrossLoss = 0;

        }

        public void SetStateData(RateMarketAgentData data)
        {
            if (data.TotalBenifit > MaxProfit)
                MaxProfit = data.TotalBenifit;

            if (data.TotalBenifit < MaxLoss)
                MaxLoss = data.TotalBenifit;

            if( data.LastOrderLog != null)
            {
                OrderLog log = data.LastOrderLog;
                if (log.BenifitRate > BestTradeRate)
                    BestTradeRate = log.BenifitRate;

                if (log.BenifitRate < WorstTradeRate)
                    WorstTradeRate = log.BenifitRate;

                if( log.BenifitMoney > 0)
                {
                    GrossProfit += log.BenifitMoney;
                }
                else
                {
                    GrossLoss += log.BenifitMoney;
                }
            }
        }

    }
}
