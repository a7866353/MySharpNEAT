using MyProject01.Agent;
using MyProject01.Controller;
using MyProject01.DAO;
using MyProject01.Util;
using MyProject01.Util.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyProject01.Win
{
    /// <summary>
    /// ControllerCheckWin.xaml 的交互逻辑
    /// </summary>
    public partial class ControllerCheckWin : Window
    {
        private IController _ctrl;
        private int _preCount = 2000;
        public ControllerCheckWin()
        {
            InitializeComponent();

            this.Loaded += ControllerCheckWin_Loaded;
        }

        void ControllerCheckWin_Loaded(object sender, RoutedEventArgs e)
        {
            Debug();
        }

        private void Debug()
        {
            ControllerNameTextBox.Text = "Controller2016/5/7 13:19:59";
            StartDateTextBox.Text = "2016/1/1";
            EndDateTextBox.Text = "2016/4/1";
            TimeFrameTextBox.Text = "30";
        }

        private void SubmmitButton_Click(object sender, RoutedEventArgs e)
        {
            string ctrlName;
            DateTime staDate;
            DateTime endDate;
            DataTimeType timeFrame;
            try
            {
                ctrlName = ControllerNameTextBox.Text;
                staDate = DateTime.Parse(StartDateTextBox.Text);
                endDate = DateTime.Parse(EndDateTextBox.Text);
                timeFrame = (DataTimeType)int.Parse(TimeFrameTextBox.Text);
            }
            catch(Exception excp)
            {
                return;
            }

            LoadController(ctrlName);
            // GetDealLog("USDJPY_30", timeFrame, staDate, endDate, false);
            GetDealLog("USDJPY_1", timeFrame, staDate, endDate, true);
            GetDealLog("USDJPY_30", timeFrame, staDate, endDate, false);
        }

        private void LoadController(string name)
        {
            ControllerDAOV2 dao = ControllerDAOV2.GetDAOByName(name);
            _ctrl = dao.GetController();
        }
        private void GetDealLog(string collectionName, DataTimeType timeFrame, DateTime startDate, DateTime endDate, bool isCov)
        {
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader(collectionName, timeFrame, startDate, endDate, _preCount)
                {
                    NeedTimeFrameConver = isCov,
                };
            loader.Load();

            _ctrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(loader);
            LearnRateMarketAgent agent = new LearnRateMarketAgent(_ctrl);
            agent.Reset();
            agent.SetRange(_preCount, _ctrl.TotalLength);

            DealLogList logList = new DealLogList();
            int trainDealCount = 0;
            double startMoney = agent.InitMoney;
            double trainedMoney = 0;
            double endMoney = 0;
            while (true)
            {
                if (agent.IsEnd == true)
                    break;

                // Get Action Value
                agent.DoAction();

                // Add log
                logList.Add(agent.LastAction, agent.CurrentValue, agent.CurrentRateValue);

                // To large for test
                // epsodeLog.DealLogs.Add(dealLog);
                agent.Next();

            } // end while

            string resultStr = "";
            resultStr += "LogCount: " + logList.Count + "\r\n";
            resultStr += "MoneyResult: " + logList[logList.Count - 1].CurrentMoney + "\r\n";
            ResultTextBlock.Text = resultStr;

#if false
            endMoney = agent.CurrentValue;

            epsodeLog.TrainedDataEarnRate = (trainedMoney / startMoney) * 100;
            epsodeLog.UnTrainedDataEarnRate = (endMoney / trainedMoney) * 100;
            epsodeLog.TrainedDealCount = trainDealCount;
            epsodeLog.UntrainedDealCount = agent.DealCount - trainDealCount;
            epsodeLog.HidenNodeCount = network.Links.Length;
            epsodeLog.ResultMoney = endMoney;
            epsodeLog.Step = _epoch;
            epsodeLog.ControllerName = _context.ControllerName;
            epsodeLog.Time = _context.CurrentDate;
            epsodeLog.Save();
            epsodeLog.SaveDealLogs(logList);

            // update dao
            dao.LastTestDataEarnRate = epsodeLog.UnTrainedDataEarnRate;
            dao.LastTrainedDataEarnRate = epsodeLog.TrainedDataEarnRate;

            // update log
            _log.Set(LogFormater.ValueName.TrainScore, epsodeLog.TrainedDataEarnRate);
            _log.Set(LogFormater.ValueName.UnTrainScore, epsodeLog.UnTrainedDataEarnRate);
            _log.Set(LogFormater.ValueName.Step, _epoch);
            LogFile.WriteLine(_log.GetLog());
#endif
            // Display Graphic
            DisplayLog(logList);

        }
        private void DisplayLog(DealLogList logList)
        {
            double[] rateArr = new double[logList.Count];
            double[] currentMoneyArr = new double[logList.Count];

            for (int i = 0; i < logList.Count; i++)
            {
                DealLog log = logList[i];
                rateArr[i] = log.Rate;
                currentMoneyArr[i] = log.CurrentMoney;
            }

            // 创建显示窗口
            GraphViewer logView = new GraphViewer();
            logView.Show();
            // 增加汇率曲线
            GraphLine rateLine = logView.AddLineData(rateArr);
            // 增加交易标记
            for (int i = 0; i < logList.Count; i++)
            {
                DealLog log = logList[i];
                if (log.Action == MarketActions.Buy)
                    rateLine.AddMark(i, Brushes.Red);
                else if (log.Action == MarketActions.Sell)
                    rateLine.AddMark(i, Brushes.Green);
                else if (log.Action == MarketActions.Close)
                    rateLine.AddMark(i, Brushes.Orange);
            }
            rateLine.Update();
            // 增加当前收益曲线
            GraphLine moneyLine = logView.AddLineData(currentMoneyArr);

        }
    }
}
