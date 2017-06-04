using MyProject01.Controller;
using MyProject01.DAO;
using MyProject01.DataSources;
using MyProject01.Util;
using MyProject01.Util.DataObject;
using SocketTestClient.RequestObject;
using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.ConnectionContoller
{
    abstract class BasicTradeOrder
    {
        protected RateDataControlDAO _dataController;
        protected DateTime _lastTradeTime;
        protected int _magicNumber;

        private ISender _sender;

        public BasicTradeOrder(string rateDataControllerName, string networkControllerName, int magicNumber, ISender sender)
        {
            _dataController = RateDataControlDAO.GetByName(rateDataControllerName);
            // For Debug at first run
            _lastTradeTime = DateTime.Now.AddDays(-10);
            _magicNumber = magicNumber;
            _sender = sender;
        }
        public string SymbolName
        {
            get { return _dataController.SymbolName; }
        }
        public string Name
        {
            get { return _dataController.Name; }
        }
        public int MagicNumber
        {
            get { return _magicNumber; }
        }
        public void DoTrade()
        {
            _dataController.Update();
            if (_dataController.LastItemTime > _lastTradeTime)
            {
                _lastTradeTime = _dataController.LastItemTime;
                MarketActions cmd = GetNextCommand(_dataController, _lastTradeTime);
                if (cmd == MarketActions.Nothing)
                    return;

                SendOrderRequest req = new SendOrderRequest();
                req.SymbolName = SymbolName+"pro";
                req.MagicNumber = MagicNumber;
                if (cmd == MarketActions.Buy)
                {
                    req.OrderCmd = SendOrderRequest.Cmd.Buy;
                    Printf(MagicNumber + ": Buy");
                }
                else if (cmd == MarketActions.Sell)
                {
                    req.OrderCmd = SendOrderRequest.Cmd.Sell;
                    Printf(MagicNumber + ": Sell");
                }
                else if (cmd == MarketActions.Close)
                {
                    req.OrderCmd = SendOrderRequest.Cmd.Close;
                    Printf(MagicNumber + ": Close");
                }
                else
                    throw (new Exception("Parm error!"));

                // ===============
                // Send
                _sender.Send(req);
                if( req.Result != 0)
                    Printf(MagicNumber + "Order Error!");
            }
            return;
        }

        abstract protected MarketActions GetNextCommand(RateDataControlDAO dataController, DateTime itemTime);
         private void Printf(string str)
        {
            System.Console.WriteLine("OrderSendController: " + str);
        }
    
   }

    class RateTradeOrder : BasicTradeOrder
    {
        protected ITradeDesisoin _decisionCtrl;
        public RateTradeOrder(string rateDataControllerName, string networkControllerName, int magicNumber, ISender sender)
            : base(rateDataControllerName, networkControllerName, magicNumber, sender)
        {
            NetworkController _networkController = NetworkController.Open(networkControllerName);
            _decisionCtrl = _networkController.GetDecisionController();

        }
        protected override MarketActions GetNextCommand(RateDataControlDAO dataController, DateTime itemTime)
        {
            DateTime lastTradeTime = dataController.LastItemTime;
            RateData[] rateDataArr = dataController.GetByEndTime(lastTradeTime, _decisionCtrl.InputDataLength);
            return Calculte(rateDataArr);
        }
        private MarketActions Calculte(RateData[] rateDataArr)
        {
            double[] dataArr = new double[rateDataArr.Length];
            for (int i = 0; i < dataArr.Length; i++)
                dataArr[i] = (rateDataArr[i].high + rateDataArr[i].low) / 2;
            MarketActions res = _decisionCtrl.GetAction(dataArr);

            return res;
        }

    }

    // TODO
    class KDJTradeOrder : BasicTradeOrder
    {
        protected ITradeDesisoin _decisionCtrl;
        public KDJTradeOrder(string rateDataControllerName, string networkControllerName, int magicNumber, ISender sender)
            : base(rateDataControllerName, networkControllerName, magicNumber, sender)
        {
            NetworkController _networkController = NetworkController.Open(networkControllerName);
            _decisionCtrl = _networkController.GetDecisionController();
             
        }
        protected override MarketActions GetNextCommand(RateDataControlDAO dataController, DateTime itemTime)
        {
            DateTime lastTradeTime = dataController.LastItemTime;
            TestDataDateRangeLoader loader = new TestDataDateRangeLoader(dataController.CollectiongName, DataTimeType.M5, lastTradeTime.AddDays(-10), lastTradeTime, 1024);
            loader.Load();
            KDJDataBlock dataBlock = new KDJDataBlock(loader, 0, loader.Count, _decisionCtrl.InputDataLength);

            double[] buffer = new double[_decisionCtrl.InputDataLength * 4];
            dataBlock.Copy(buffer, dataBlock.BlockCount - 1);

            MarketActions res = _decisionCtrl.GetAction(buffer);
            return res;
        }
    }
    class NewTradeOrder : BasicTradeOrder
    {
        protected ITradeDesisoin _decisionCtrl;
        private IController _ctrl;
        private DataTimeType _timeFrame;
        private DateTime _lastItemTime;
        private bool _isTimeFrameConvert;
        public NewTradeOrder(string rateDataControllerName, DataTimeType timeFrame, string networkControllerName, int magicNumber, ISender sender, bool IsCov = true)
            : base(rateDataControllerName, networkControllerName, magicNumber, sender)
        {
            ControllerDAOV2 dao = ControllerDAOV2.GetDAOByName(networkControllerName);
            _ctrl = dao.GetController();
            _timeFrame = timeFrame;
            _lastItemTime = DateTime.Now.AddMinutes(-2 * (int)_timeFrame);
            _isTimeFrameConvert = IsCov;
        }
        protected override MarketActions GetNextCommand(RateDataControlDAO dataController, DateTime itemTime)
        {
            DateTime lastTradeTime = dataController.LastItemTime;
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader(dataController.CollectiongName, _timeFrame, lastTradeTime.AddMinutes(-1 * dataController.TimeFrame * 1000), lastTradeTime, 2000)
                {
                    NeedTimeFrameConver = _isTimeFrameConvert,
                };
            loader.Load();

            if (_lastItemTime.AddMinutes((int)_timeFrame) > loader[loader.Count - 1].Time)
                return MarketActions.Nothing;
            _lastItemTime = loader[loader.Count - 1].Time;

            _ctrl.DataSourceCtrl = new MyProject01.DataSources.LoaderSourceCtrl(loader);
            _ctrl.CurrentPosition = _ctrl.TotalLength-1;

            return _ctrl.GetAction();
        }

    }
}
