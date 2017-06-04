using SocketTestClient.Sender;
using SocketTestClient.RequestObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using MyProject01.DAO;
using System.Threading;

namespace SocketTestClient.ConnectionContoller
{
    class RateDataNeed
    {
        public string Name;
        public string SymbolName;
        public int Timeframe;
    }
    class CollectArgs : EventArgs
    {
        public string CollectionTableName;
        public int UpdatedCount;
    }

    abstract class RataDataCollectTask
    {
        protected ISender _sender;
        protected double _updateInterval;
        protected TimeSpan _getDuration;

        public delegate void ChangeHandler();
        public event ChangeHandler OnChange;

        public RataDataCollectTask(ISender sender)
        {
            _sender = sender;
        }

        public virtual double UpdateInterval
        {
            set { _updateInterval = value; }
            get { return _updateInterval; }
        }
        public int Update()
        {
            if (_sender.State != DeamonState.Connected)
                return 0;

            int cnt = InternalUpdate();
            if (OnChange!=null)
                OnChange();
            return cnt;
        }
        protected abstract int InternalUpdate();
        protected void Printf(string str)
        {
            System.Console.WriteLine("[RateData]" + str);
        }
        public abstract string CollectionTableName { get; }
    }

    class RateDataRequestController : RataDataCollectTask
    {
        private RateDataControlDAO _dao;
        private double _updateInterval;
        private DateTimeFormatInfo _dtFormat;
        private RateByTimeRequest _lastRequest;

        public RateDataRequestController(RateDataControlDAO dao, double updateInterval, ISender sender)
            : base(sender)
        {
            _dao = dao;
            _updateInterval = updateInterval;

            _lastRequest = null;
            _dtFormat = new DateTimeFormatInfo();
            _dtFormat.ShortDatePattern = "yyyy.mm.dd hh:mm:ss";
        }

        private IRequest GetRequest()
        {
            // dao.Update(); // error
            TimeSpan timeDulation = new TimeSpan(0, 0, (int)(_updateInterval * _dao.TimeFrame));
            if (DateTime.Now - _dao.LastGetTime <= timeDulation)
                return null;

            TimeSpan nextDulation;
            if (_lastRequest == null)
                nextDulation = new TimeSpan((long)((long)_dao.TimeFrame * TimeSpan.TicksPerMinute * _updateInterval));
            else
                nextDulation = new TimeSpan();
            RateByTimeRequest req = new RateByTimeRequest();
            req.SymbolName = _dao.SymbolName;
            req.TimeFrame = _dao.TimeFrame;
            req.StartTime = _dao.LastItemTime;
            req.StopTime = _dao.LastGetTime + nextDulation;
            if (req.StopTime > DateTime.Now)
                req.StopTime = DateTime.Now;

            _lastRequest = req;
            return req;
        }

        public void SetResult(IRequest req)
        {
            RateDataIndicateRequest indicate = (RateDataIndicateRequest)req;
            RateInfo[] infoArr = indicate.RateInfoArray;
            List<RateData> dataList = new List<RateData>();

            if (infoArr != null)
            {
                foreach (RateInfo info in infoArr)
                {
                    DateTime time = Convert.ToDateTime(info.time, _dtFormat);
                    if (time <= _dao.LastItemTime)
                        continue;

                    RateData data = new RateData();
                    data.time = time;
                    data.high = info.high;
                    data.low = info.low;
                    data.open = info.open;
                    data.close = info.close;
                    data.real_volume = info.real_volume;
                    data.tick_volume = info.tick_volume;
                    data.spread = info.spread;

                    dataList.Add(data);
                }
                if (dataList.Count != 0)
                    _dao.Add(dataList.ToArray());
            }

            _dao.LastGetTime = _lastRequest.StopTime;
            _dao.Save();

            if (dataList.Count > 0)
            {
                Printf("Get:" + _dao.SymbolName + "_" + _dao.TimeFrame + " From" + 
                    _lastRequest.StartTime + " to " + _lastRequest.StopTime + " Count:" + dataList.Count);
            }
        }

        protected override int InternalUpdate()
        {
            throw new NotImplementedException();
        }

        public override string CollectionTableName
        {
            get { return _dao.CollectiongName; }
        }
    }


    class RateByTimeRequestController : RataDataCollectTask
    {
        private RateDataControlDAO _dao;
        private double _updateInterval;
        private DateTimeFormatInfo _dtFormat;
        private RateByTimeRequest _lastRequest;
        private TimeSpan _getDataBlockDuration;
        private TimeSpan _getDuration;

        public RateByTimeRequestController(RateDataControlDAO dao, double updateInterval, ISender sender)
            : base(sender)
        {
            _dao = dao;
            _updateInterval = updateInterval = 2;

            _lastRequest = null;
            _dtFormat = new DateTimeFormatInfo();
            _dtFormat.ShortDatePattern = "yyyy.mm.dd hh:mm:ss";

            int min = (int)(_updateInterval * _dao.TimeFrame);
            int sec = (int)(((_updateInterval * _dao.TimeFrame) - (int)(_updateInterval * _dao.TimeFrame))*60);
            _getDataBlockDuration = new TimeSpan(0, min, sec);
            _getDuration = _getDataBlockDuration;
        }

        private RateByTimeRequest GetRequest()
        {
            // dao.Update(); // error
            DateTime tartgetTime = _dao.LastGetTime + _getDuration;
            if (tartgetTime > DateTime.Now.AddMinutes(_dao.TimeFrame*-1))
            {
                // _dao.LastGetTime = DateTime.Now;
                // _dao.Save();
                return null;
            }

            RateByTimeRequest req = new RateByTimeRequest();
            req.SymbolName = _dao.SymbolName;
            req.TimeFrame = _dao.TimeFrame;
            req.StartTime = _dao.LastItemTime;
            req.StopTime = tartgetTime;

            Printf("Send:" + _dao.SymbolName + "_" + _dao.TimeFrame + " From" +
                req.StartTime + " to " + req.StopTime );


            _lastRequest = req;
            return req;
        }

        private void SetResult(RateInfo[] infoArr)
        {
            List<RateData> dataList = new List<RateData>();

            if (infoArr != null)
            {
                foreach (RateInfo info in infoArr)
                {
                    DateTime time = Convert.ToDateTime(info.time, _dtFormat);
                    if (time <= _dao.LastItemTime)
                        continue;

                    RateData data = new RateData();
                    data.time = time;
                    data.high = info.high;
                    data.low = info.low;
                    data.open = info.open;
                    data.close = info.close;
                    data.real_volume = info.real_volume;
                    data.tick_volume = info.tick_volume;
                    data.spread = info.spread;

                    dataList.Add(data);
                }
                if (dataList.Count != 0)
                    _dao.Add(dataList.ToArray());
            }

            _dao.LastGetTime = _lastRequest.StopTime;
            _dao.Save();

            if (infoArr == null || infoArr.Length == 0)
            {
                CSVLogFormater.Add(_dao.Name, _lastRequest.StartTime, _lastRequest.StopTime,
                    0,
                    _lastRequest.StartTime,
                    _lastRequest.StartTime);
            }
            else
            {
                CSVLogFormater.Add(_dao.Name, _lastRequest.StartTime, _lastRequest.StopTime,
                    infoArr.Length,
                    Convert.ToDateTime(infoArr[0].time, _dtFormat),
                    Convert.ToDateTime(infoArr[infoArr.Length - 1].time, _dtFormat));

            }
            // for debug
            if (dataList.Count > 0)
            {
                Printf("Get:" + _dao.SymbolName + "_" + _dao.TimeFrame + " From" +
                    _lastRequest.StartTime + " to " + _lastRequest.StopTime + " Count:" + dataList.Count);
            }
            if (dataList.Count == 0)
            {
                _getDuration += _getDataBlockDuration;
            }
            else
            {
                _getDuration = _getDataBlockDuration;
            }
        }

        protected override int InternalUpdate()
        {
            RateByTimeRequest req = GetRequest();
            _sender.Send(req);
            if (req.RateInfoArray != null && req.RateInfoArray.Length > 0)
            {
                SetResult(req.RateInfoArray);
                return req.RateInfoArray.Length;
            }
            else
            {
                return 0;
            }
        }

        public override string CollectionTableName
        {
            get { return _dao.CollectiongName; }
        }
    }
    class RateByTimeRequestController2 : RataDataCollectTask
    {
        private RateDataControlDAO _dao;
        private DateTimeFormatInfo _dtFormat;
        private RateByTimeRequest2 _lastRequest;
        private TimeSpan _getDataBlockDuration;

        public RateByTimeRequestController2(RateDataControlDAO dao, double updateInterval, ISender sender)
            : base(sender)
        {
            _dao = dao;
            _updateInterval = updateInterval = 2;

            _lastRequest = null;
            _dtFormat = new DateTimeFormatInfo();
            _dtFormat.ShortDatePattern = "yyyy.mm.dd hh:mm:ss";

            int min = (int)(_updateInterval * _dao.TimeFrame);
            int sec = (int)(((_updateInterval * _dao.TimeFrame) - (int)(_updateInterval * _dao.TimeFrame)) * 60);
            _getDataBlockDuration = new TimeSpan(0, min, sec);
            _getDuration = _getDataBlockDuration;
        }

        public override double UpdateInterval
        {
            set
            {
                _updateInterval = value;
                int min = (int)(_updateInterval * _dao.TimeFrame);
                int sec = (int)(((_updateInterval * _dao.TimeFrame) - (int)(_updateInterval * _dao.TimeFrame)) * 60);
                _getDataBlockDuration = new TimeSpan(0, min, sec);
                _getDuration = _getDataBlockDuration;
            }
        }
        private RateByTimeRequest2 GetRequest()
        {
            // dao.Update(); // error
            DateTime tartgetTime = _dao.LastGetTime + _getDuration;
            if (tartgetTime > DateTime.Now)
            {
                return null;
            }

            RateByTimeRequest2 req = new RateByTimeRequest2();
            req.SymbolName = _dao.SymbolName;
            req.TimeFrame = _dao.TimeFrame;
            req.StartTime = _dao.LastItemTime;

            // Printf("Send:" + _dao.SymbolName + "_" + _dao.TimeFrame + " From" +
            //     req.StartTime);


            _lastRequest = req;
            return req;
        }

        private void SetResult(RateInfo[] infoArr)
        {
            List<RateData> dataList = new List<RateData>();

            if (infoArr != null)
            {
                foreach (RateInfo info in infoArr)
                {
                    DateTime time = Convert.ToDateTime(info.time, _dtFormat);
                    if (time <= _dao.LastItemTime)
                        continue;

                    RateData data = new RateData();
                    data.time = time;
                    data.high = info.high;
                    data.low = info.low;
                    data.open = info.open;
                    data.close = info.close;
                    data.real_volume = info.real_volume;
                    data.tick_volume = info.tick_volume;
                    data.spread = info.spread;

                    dataList.Add(data);
                }
                if (dataList.Count != 0)
                    _dao.Add(dataList.ToArray());
            }

            if (dataList.Count > 0)
            {
                _dao.LastGetTime = _dao.LastItemTime;
            }
            else
            {
                _dao.LastGetTime = DateTime.Now;
            }
            _dao.Save();

            if (infoArr == null || infoArr.Length == 0)
            {
                CSVLogFormater.Add(_dao.Name, _lastRequest.StartTime, DateTime.Now,
                    0,
                    _lastRequest.StartTime,
                    _lastRequest.StartTime);
            }
            else
            {
                CSVLogFormater.Add(_dao.Name, _lastRequest.StartTime, DateTime.Now,
                    infoArr.Length,
                    Convert.ToDateTime(infoArr[0].time, _dtFormat),
                    Convert.ToDateTime(infoArr[infoArr.Length - 1].time, _dtFormat));

            }
            // for debug
            if (dataList.Count > 0)
            {
                Printf("Get:" + _dao.SymbolName + "_" + _dao.TimeFrame + " From" +
                    _lastRequest.StartTime + " To" + _dao.LastItemTime + " Count:" + dataList.Count);
            }
        }

        protected override int InternalUpdate()
        {
            int GetSum = 0;
            while (true)
            {
                RateByTimeRequest2 req = GetRequest();
                if (req == null)
                    return GetSum;


                _sender.Send(req);
                if (req.RateInfoArray != null && req.RateInfoArray.Length > 0)
                {
                    SetResult(req.RateInfoArray);
                    GetSum += req.RateInfoArray.Length;
                }
                else
                {
                    return GetSum;
                }
            }
        }

        public override string CollectionTableName
        {
            get { return _dao.CollectiongName; }
        }
    }

    class RateByCountRequestController : RataDataCollectTask
    {
        private int _getCount = 1024;
        private RateDataControlDAO _dao;
        private double _updateInterval;
        private DateTimeFormatInfo _dtFormat;
        private DateTime _lastUpdateTime;
        private RateByCountRequest _lastRequest;
        private ISender _sender;

        public RateByCountRequestController(RateDataControlDAO dao, double updateInterval, ISender sender)
            : base(sender)
        {
            _dao = dao;
            _updateInterval = updateInterval;
            
            _lastRequest = null;
            _dtFormat = new DateTimeFormatInfo();
            _dtFormat.ShortDatePattern = "yyyy.mm.dd hh:mm:ss";
        }

        public RateByCountRequest GetRequest()
        {
            // dao.Update(); // error
            TimeSpan timeDulation = new TimeSpan(0, 0, (int)(_updateInterval * _dao.TimeFrame));
            if (DateTime.Now - _dao.LastGetTime <= timeDulation)
                return null;

            _lastUpdateTime = DateTime.Now;
            TimeSpan nextDulation = new TimeSpan((long)_dao.TimeFrame * TimeSpan.TicksPerMinute * 1024);
            RateByCountRequest req = new RateByCountRequest();
            req.SymbolName = _dao.SymbolName;
            req.TimeFrame = _dao.TimeFrame;
            req.StartTime = _dao.LastItemTime;
            req.Count = _getCount;

            return req;
        }

        public void SetResult(RateInfo[] infoArr)
        {
            List<RateData> dataList = new List<RateData>();

            if (infoArr != null)
            {
                foreach (RateInfo info in infoArr)
                {
                    DateTime time = Convert.ToDateTime(info.time, _dtFormat);
                    if (time <= _dao.LastItemTime)
                        continue;

                    RateData data = new RateData();
                    data.time = time;
                    data.high = info.high;
                    data.low = info.low;
                    data.open = info.open;
                    data.close = info.close;
                    data.real_volume = info.real_volume;
                    data.tick_volume = info.tick_volume;
                    data.spread = info.spread;

                    dataList.Add(data);
                }
                if (dataList.Count != 0)
                    _dao.Add(dataList.ToArray());
            }

            _dao.LastGetTime = _lastUpdateTime;
            _dao.Save();

            if (dataList.Count > 0)
            {
                Printf("Get:" + _dao.SymbolName + "_" + _dao.TimeFrame + " From" + 
                    _lastRequest.StartTime + " Count " + _lastRequest.Count + " GetCount:" + dataList.Count);
            }
        }

        protected override int InternalUpdate()
        {
            RateByCountRequest req = GetRequest();
            _sender.Send(req);
            if (req.RateInfoArray != null && req.RateInfoArray.Length > 0)
            {
                SetResult(req.RateInfoArray);
                return req.RateInfoArray.Length;
            }
            else
            {
                return 0;
            }
        }

        public override string CollectionTableName
        {
            get { return _dao.CollectiongName; }
        }
    }

    class RateDataController
    {
        private RateDataDAOList _rateDataList;
        private int _watchListIndex;
        private TimeSpan _sendingBlockDuration = new TimeSpan(24, 0, 0);
        private bool _isSymbolListUpdated;

        private List<RataDataCollectTask> _watchList;

        private string[] _interalSymbols;
        private ISender _sender;

        private int[] _timeFrameArray = new int[]
        {
            1,      // 1 minute
#if true
            5,      // 5 minutes
            15,     // 15 minutes
            30,     // 30 minutes
            60,     // 1 hour
            240,    // 4 hours
            1440,   // 1 day
            10080,  // 1 week
            43200,  // 1 month
#endif
        };

        public RateDataController(ISender sender)
        {
            _rateDataList = new RateDataDAOList();
            _watchList = new List<RataDataCollectTask>();
            _isSymbolListUpdated = false;
            _watchListIndex = -1;
            _sender = sender;
            //-----------------
#if false
            _interalSymbols = new string[]
            {
                "AUDJPY",
                "AUDJPYpro",
                "AUDUSD",
                "AUDUSDpro",
                "CHFJPY",
                "CHFJPYpro",
                "EURCAD",
                "EURCADpro",
                "EURCHF",
                "EURCHFpro",
                "EURGBP",
                "EURGBPpro",
                "EURJPY",
                "EURJPYpro",
                "EURUSD",
                "EURUSDpro",
                "GBPCHF",
                "GBPCHFpro",
                "GBPJPY",
                "GBPJPYpro",
                "GBPUSD",
                "GBPUSDpro",
                "NZDJPY",
                "NZDJPYpro",
                "NZDUSD",
                "NZDUSDpro",
                "USDCAD",
                "USDCADpro",
                "USDJPY",
                "USDJPYpro",
                "USDCHF",
                "USDCHFpro",
                "XAGUSD",
                "XAGUSDpro",
                "XAUUSD",
                "XAUUSDpro",
            };
#else
            _interalSymbols = new string[]
            {
                "USDJPY",
            };

#endif
            AddSymbol(_interalSymbols);
        }

        public void Start()
        {
            int checkCount = 0;
            while (true)
            {
                // Check network state
                if (_sender.State != DeamonState.Connected)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                _watchListIndex++;
                if (_watchListIndex >= _watchList.Count)
                    _watchListIndex = 0;

                RataDataCollectTask rateReqCtrl = _watchList[_watchListIndex];
                int cnt = rateReqCtrl.Update();
                if (cnt > 0)
                {
                    checkCount = 0;
                }
                else
                {
                    checkCount++;
                    if (checkCount >= _watchList.Count)
                    {
                        checkCount = 0;
                        Thread.Sleep(1000 * 10);
                    }
                }
            }

        }


        public void AddSymbol(string[] symbols)
        {
            // Add all Symbol
            foreach( string symbol in symbols)
            {
                foreach( int timeFrame in _timeFrameArray)
                {
                    string name = symbol + "_" + timeFrame;
                    if (_rateDataList.Get(name) == null)
                    {
                        _rateDataList.Add(name, symbol, timeFrame, new DateTime(2000, 1, 1, 0, 0, 0));
                    }

                }
            }
            _isSymbolListUpdated = true;

            // Add All symbol to watch list.
            foreach(RateDataControlDAO dao in _rateDataList)
            {
                // _watchList.Add(new RateDataRequestController(dao, 512));
                // _watchList.Add(new RateByTimeRequestController(dao, 512, _sender));
                _watchList.Add(new RateByTimeRequestController2(dao, 512, _sender));
            }
        }

        public RataDataCollectTask FindByName(string collectionName)
        {
            RataDataCollectTask targetTask = null;
            foreach (RataDataCollectTask task in _watchList)
            {
                if (task.CollectionTableName.CompareTo(collectionName) != 0)
                    continue;

                targetTask = task;
                break;
            }
            return targetTask;
        }

    }
}
