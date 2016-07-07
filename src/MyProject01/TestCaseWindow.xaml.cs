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
using MyProject01.TestCases;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NEAT;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Train;
using Encog.Util.Simple;
using Encog.Util.Banchmark;
using Encog.Util;
using MyProject01.TrainingMethods;
using MyProject01.Networks;
using MyProject01.Test;
using Encog.Neural.Networks;
using MyProject01.DAO;
using MyProject01.Win;
using MyProject01.Util.DllTools;
using MyProject01.Controller;
using MyProject01.TestCases.RateMarketTestCases;
using System.Threading;

namespace MyProject01
{
    public class TestCaseObject
    {
        public delegate void TestFucntion();
        public string Name;
        public String Description;
        public TestFucntion TestFunction;

        public TestCaseObject(string name, string description, TestFucntion function)
        {
            this.Name = name;
            this.Description = description;
            this.TestFunction = function;
        }
    }

    class TestCaseGroup : List<TestCaseObject>
    {
        public void Add(TestCaseGroup group)
        {
            foreach (TestCaseObject obj in group)
                Add(obj);
        }

        public void Add(BasicTestCase testCase)
        {
            TestCaseObject obj = new TestCaseObject(testCase.TestName, testCase.TestDescription, new TestCaseObject.TestFucntion(testCase.RunTest));
            Add(obj);
        }
        public void Add(BasicNewTestCase testCase)
        {
            TestCaseObject obj = new TestCaseObject(testCase.TestCaseName, "", new TestCaseObject.TestFucntion(testCase.Run));
            Add(obj);
        }

    }
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class TestCaseWindow : Window
    {
        private TestCaseGroup TestCaseList;
        private delegate void Func();

        public TestCaseWindow()
        {
            InitializeComponent();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
            this.Closing += TestCaseWindow_Closing;
            TestCaseList = new TestCaseGroup();
            // StartAllButton.Click += StartAllButton_Click;
            AddTestCase();
            int i = 0;

            foreach( TestCaseObject obj in TestCaseList)
            {
                string displayName = "[" + i.ToString("D2") + "]" + obj.Name + ": " + obj.Description;
                i++;
                Border border = new Border();
                border.BorderThickness = new Thickness(4, 1, 4, 1);
                border.BorderBrush = Brushes.Black;

                Button testButton = new Button();
                testButton.Height = 20;
                testButton.Content = displayName;
                testButton.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                testButton.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e)
                    {
                        MainWindow mainWin = new MainWindow(obj);
                        mainWin.Title = DateTime.Now.ToString() + ": " + displayName;
                        mainWin.Closed += new EventHandler(
                                delegate(object sender2, EventArgs args)
                                {
                                    Application.Current.Shutdown();
                                }
                            );
                        mainWin.Show();
                    });
                border.Child = testButton;
                MainStackPanel.Children.Add(border);
            }

            InitParamConfig();

        }
        private void InitParamConfig()
        {
            //-------------------------------
            AddParamConfigUI("ParameterConfigure:", null);


            // ServerIP
            //------------------------
#if false
            TextBox ipTb = new TextBox();
            ipTb.Text = CommonConfig.ServerIP;
            ipTb.TextChanged += new TextChangedEventHandler(
                    delegate(object sender, TextChangedEventArgs args)
                    {
                        CommonConfig.ServerIP = ipTb.Text;
                    }
                );
            AddParamConfigUI("Server IP Address", ipTb);
#endif
            StackPanel ipPanel = new StackPanel();
            foreach(ServerIPParam param in ServerIPParamList.IPs)
            {
                RadioButton rb = new RadioButton();
                rb.GroupName = "IPParam";
                rb.Content = param.IP;
                rb.Checked += new RoutedEventHandler(
                        delegate(object sender, RoutedEventArgs args)
                        {
                            CommonConfig.ServerIP = param.IP;
                            DataBaseAddress.SetIP(CommonConfig.ServerIP);
                        }
                    );
                ipPanel.Children.Add(rb);
                
                if (param.IsDefault == true)
                    rb.IsChecked = true;
            }
            AddParamConfigUI("Server IP Address", ipPanel);


            // PopulationSize
            //---------------------------
            TextBox popSizeTb = new TextBox();
            popSizeTb.Text = CommonConfig.PopulationSize.ToString();
            popSizeTb.TextChanged += new TextChangedEventHandler(
                    delegate(object sender, TextChangedEventArgs args)
                    {
                        int result;
                        if (int.TryParse(popSizeTb.Text, out result) == false)
                            return;
                        CommonConfig.PopulationSize = result;
                    }
                );
            AddParamConfigUI("Populaton Size", popSizeTb);

            // LoaderParam
            //------------------------
            StackPanel loaderPanel = new StackPanel();
            foreach(DataLoaderParam parm in DataLoaderParamList.GetParams())
            {
                RadioButton loaderParamRb = new RadioButton();
                loaderParamRb.GroupName = "LoaderParam";
                loaderParamRb.Content = parm.ToString();
                loaderParamRb.Checked += new RoutedEventHandler(
                        delegate(object sender, RoutedEventArgs args)
                        {
                            CommonConfig.LoaderParam = parm;
                        }
                    );
                loaderPanel.Children.Add(loaderParamRb);
                if (parm.IsDefault == true)
                    loaderParamRb.IsChecked = true;
            }
            AddParamConfigUI("Rate Data Loader", loaderPanel);

        }
        private void AddParamConfigUI(string name, UIElement ui)
        {
            StackPanel panel = this.ParamConfigStackPanel;
            panel.Children.Add(new Label() { Content = name });
            if (ui != null)
                panel.Children.Add(ui);
            panel.Children.Add(new Rectangle() { Height = 2, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, Fill = Brushes.Black });
        }

        void TestCaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown(-1);
        }

        private string GetTestName()
        {
            string name = null;
            this.Dispatcher.Invoke(new Func(delegate()
            {
                name = TestNameTextBox.Text;
            }));
            
            if (string.IsNullOrWhiteSpace(name) == true)
                return "DefaultTest000";
            else
                return name;
        }

        private void TestCase01()
        {
            MessageBox.Show("01");
        }

        private void TestMarketAnalyz()
        {
            // TODO
            /*
            DataLoader loader = new FenghuangDataLoader();
            MarketRateAnalyzer analyzer = new MarketRateAnalyzer(loader.ToArray());
            DealPointInfomation[] info = analyzer.GetDealInfo();
             */
        }

        private void RateAnalyzeTest()
        {
            GraphViewer win;
            this.Dispatcher.BeginInvoke(new Func(delegate()
            {
                win = new GraphViewer();
                win.Show();
                // TODO
                /*
                DataLoader dataLoader = new FenghuangDataLoader();
                MarketRateAnalyzer test = new MarketRateAnalyzer(dataLoader.ToArray());
                test.GetDealInfo();
                 */ 
            }));



        }

        private void TestANN()
        {
            // NetworkTest test;
            //  test = new FeedForwardNetworkTest();
            //  test = new ElmanNetworkTest();
            // RateAnalyzeTest();
            // test.Run();

            TestCaseFactory testCaseFactory = new TestCaseFactory();
            BasicTestCase[] testCaseArr = testCaseFactory.GetTestCases();
            Parallel.ForEach(testCaseArr, (testCase, loopState) =>
            {
                try
                {
                    testCase.RunTest();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            });
        }

        private void TestRateMarketAgent()
        {
            RateMarketQLearnTest test = new RateMarketQLearnTest();
            test.RunTest();
        }

        private void TestNEATNet()
        {
            double errorLimit = 0.01;

            IMLDataSet trainingSet = RandomTrainingFactory.Generate(1000, 1500,
                                         30, 3, -1, 1);

            double targetErrorLimit = 0;
            for (int i = 0; i < trainingSet[0].Ideal.Count; i++)
                targetErrorLimit += Math.Abs(trainingSet[0].Ideal[i]);
            targetErrorLimit /= trainingSet[0].Ideal.Count;
            targetErrorLimit *= errorLimit;

            NEATPopulation pop = new NEATPopulation(30, 3, 100);
            pop.Reset();
            pop.InitialConnectionDensity = 1.0; // not required, but speeds processing.
            ICalculateScore score = new TrainingSetScore(trainingSet);
            // train the neural network
            TrainEA train = NEATUtil.ConstructNEATTrainer(pop,score);
           
            int epoch = 1;
            LogFile.WriteLine(@"Beginning training...");
            do
            {
                train.Iteration();

                LogFile.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                         + @" Error:" + Format.FormatPercent(train.Error)
                         + @" Target Error: " + Format.FormatPercent(targetErrorLimit));
                epoch++;
            } while ((train.Error > targetErrorLimit) && !train.TrainingDone);
            train.FinishTraining();


            NEATNetwork network = (NEATNetwork)train.CODEC.Decode(train.BestGenome);

            // test the neural network
            LogFile.WriteLine(@"Neural Network Results:");
            EncogUtility.Evaluate(network, trainingSet);

        }

        private void TestBPTrain()
        {
            NetworkTestParameter parm = new NetworkTestParameter("QLearn", 0.5, 10, 10);
            parm.InputSize = 30;
            parm.OutputSize = 3;
            FeedForwardNet net = new FeedForwardNet();
            BasicNetwork network = net.GetNet(parm);
            BackpropagationTraining method = new BackpropagationTraining();
            method.ErrorChangeLimit = 0.0000001;
            method.ErrorChangeTryMaxCount = 10;
            IMLDataSet trainingSet = RandomTrainingFactory.Generate(1000, 1500,
                                         parm.InputSize, parm.OutputSize, -1, 1);

            LogFile.WriteLine(@"Beginning training...");
            method.TrainNetwork(network, trainingSet);
            LogFile.WriteLine(@"Neural Network Results:");

        }
        private void TestRateMarketNEAT()
        {
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = GetTestName();
            test.TestDataRate = 0.8;
            test.PopulationNum = 1000;
            test.DataBlockLength = 2880;
            test.RunTest();
        }

        private void TestRateMarketNEAT_Short()
        {
            string prefix = "Short_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.5;
            test.PopulationNum = 500;
            test.DataBlockLength = 12*4;
            test.DataSoreceType = 1;
            test.RateDataControllerName = "test01";
            test.RunTest();
        }
        private void TestRateMarketNEAT_Long()
        {
            string prefix = "Long_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.9;
            test.PopulationNum = 500;
            test.DataBlockLength = 300;
            test.DataSoreceType = 1;
            test.RunTest();
        }
        private void Test_1Min_Long()
        {
            string prefix = "1Min_Long_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.5;
            test.PopulationNum = 1000;
            test.DataBlockLength = 60 * 24;
            test.RateDataControllerName = "test02";
            test.DataSoreceType = 1;
            test.RunTest();
        }
        private void Test_1Min_Short()
        {
            string prefix = "1Min_Short_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.5;
            test.PopulationNum = 1000;
            test.DataBlockLength = 60 * 3;
            test.RateDataControllerName = "test02";
            test.DataSoreceType = 1;
            test.RunTest();
        }
        private void Test_5Min_Short()
        {
            string prefix = "5Min_Short_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.5;
            test.PopulationNum = 1000;
            test.DataBlockLength = 12 * 3;
            test.RateDataControllerName = "test01";
            test.DataSoreceType = 1;
            test.RunTest();
        }
        private void Long5Min_Simple()
        {
            string prefix = "Long5Min_Simple_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.7;
            test.PopulationNum = 500;
            test.DataBlockLength = 12 * 2;
            test.RateDataControllerName = "test01";
            test.DataSoreceType = 0;
            test.RunTest();
        }
        private void TestRateMarketNEATBatch()
        {
            RateMarketNEATBatchTest test = new RateMarketNEATBatchTest();
            test.TestName = GetTestName();
            test.RunTest();
        }

        private void TestDAO()
        {
            string testName = "QLearn01";
            if (RateMarketTestDAO.IsExist(testName) == true)
                RateMarketTestDAO.Remove(testName);
            RateMarketTestDAO dao = RateMarketTestDAO.GetDAO<RateMarketTestDAO>(testName);


            NetworkTestParameter parm = new NetworkTestParameter("QLearn", 0.5, 2, 10);
            // network = new MyNet(new FeedForwardNet(), new ResilientPropagationTraining(), parm);
            parm.MaxTryCount = 1000;
            MyNet network = new MyNet(new FeedForwardNet(), new BackpropagationTraining(), parm);
            network.Init(30, 3);

            dao.NetworkData = network.NetworkToByte();
            // TODO
            // dao.NetworkParamter = network.parm;

            dao.Save();

            RateMarketTestEpisodeDAO episodeDAO;
            Random rand = new Random();
            double money;
            double rate;
            for (int i = 0; i < 100; i++)
            {
                episodeDAO = (RateMarketTestEpisodeDAO)dao.CreateEpisode();
                episodeDAO.Step = i;
                episodeDAO.Save();

                DealLogList logList = new DealLogList();
                money = 10000;
                rate = 100;
                for(int j=0;j<5000;j++)
                {
                    logList.Add((MarketActions)rand.Next(3), money, rate);
                    money += rand.Next(-1000, 1000);
                    rate += (rand.NextDouble() - 0.5) * 2 * 0.001;
                }

                episodeDAO.SaveDealLogs(logList);

            }
        }

        private void TestFWT()
        {
            int len = 512;
            double[] input = new double[len];
            double[] output = new double[len];
            double[] temp = new double[len];

            // Generate test data
            int f1 = 5;
            int f2 = 10;
            int f0 = 320;
            for(int i=0;i<input.Length;i++)
            {
                if(i<input.Length/2)
                 {
                    input[i] = Math.Sin(i * 2 * Math.PI * f1 / f0);
                }
                else
                {
                    input[i] = Math.Sin(i * 2 * Math.PI * f2 / f0);
                }
            }
            DllTools.FTW_2(input, output, temp);

            double[] output2 = new double[input.Length*2];
            DllTools.FTW_5(input, output2);


        }
        private void FWT_5min_Simple()
        {
            string prefix = "FWT_5min_Simple_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.4;
            test.PopulationNum = 100;
            test.DataBlockLength = 32;
            test.RateDataControllerName = "test03";
            test.DataSoreceType = 0;
            test.IsFWT = true;
            test.RunTest();
        }
        private void FWT_5min_Long()
        {
            string prefix = "FWT_5min_Long_";
            RateMarketNEATTest test = new RateMarketNEATTest();
            test.TestName = prefix + GetTestName();
            test.TestDataRate = 0.4;
            test.PopulationNum = 100;
            test.DataBlockLength = 2048;
            test.RateDataControllerName = "test01";
            test.DataSoreceType = 0;
            test.IsFWT = true;
            test.RunTest();
        }
        private void FWT_Cuda_Test()
        {
            // double[] input = new double[] {9, 7, 3, 5};
            double[] input = new double[] { 1, 2, 3, 4 };
            double[] result = new double[] {6, 2, 1, -1};
            double[] output = new double[input.Length];
            double[] temp = new double[input.Length];
       
            DllTools.FTW_2(input, output, temp);
        }

        

        private void TestDataBaseViewer()
        {
            this.Dispatcher.BeginInvoke(new Func(delegate()
            {
                DataBaseViewer win = new DataBaseViewer();
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                win.Show();
            }));
        }


        private void AddCrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[]{16, 32, 64, 1024, 2048, 4096};
            DataTimeType[] timeFrameArr = new DataTimeType[] { DataTimeType.M1, DataTimeType.M5, DataTimeType.M10, DataTimeType.M30, DataTimeType.H1, DataTimeType.H2,  DataTimeType.D1 };
            foreach (int blockLength in dataBlockLength)
            {
                foreach(DataTimeType timeFrame in timeFrameArr)
                {
                    RawRateTestCase rawCase = new RawRateTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength, TimeFrame = timeFrame,};
                    FwtNormTestCase fwtCase = new FwtNormTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength, TimeFrame = timeFrame, };
                    group.Add(rawCase);
                    group.Add(fwtCase);
                }
            }
        }
        private void AddRecentCrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[]{16, 32, 64, 1024, 2048, 4096};
            foreach (int blockLength in dataBlockLength)
            {
                FwtReduceLossRecentM5TestCase fwtCase = new FwtReduceLossRecentM5TestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            } 
            
            foreach (int blockLength in dataBlockLength)
            {
                FwtNormRecentM30TestCase fwtCase = new FwtNormRecentM30TestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }


        }

        private void AddRecentM5CrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[] { 16, 32, 64, 1024, 2048, 4096 };
            foreach (int blockLength in dataBlockLength)
            {
                BasicRateMarketTestCase fwtCase = new FwtNormRecentM5TestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }
        }
        private void AddRecentM5StepCrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[] { 16, 32, 64, 1024, 2048, 4096 };
            foreach (int blockLength in dataBlockLength)
            {
                FwtReduceLossRecentM5StepTestCase fwtCase = new FwtReduceLossRecentM5StepTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }
        }
        private void AddKdjM5CrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[] { 16, 32, 64, 1024, 2048, 4096 };
            foreach (int blockLength in dataBlockLength)
            {
                KDJTestCase fwtCase = new KDJTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }
        }
        private void AddKdjNormM5CrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[] { 16, 32, 64, 128, 1024, 2048, 4096 };
            foreach (int blockLength in dataBlockLength)
            {
                KDJNormTestCase fwtCase = new KDJNormTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }
        }
        private void AddRateNormM5CrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[] { 16, 32, 64, 128, 1024, 2048, 4096 };
            foreach (int blockLength in dataBlockLength)
            {
                RawRateM5ShortTestCase fwtCase = new RawRateM5ShortTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }
        }
        private void AddFwtNormM5CrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[] { 16, 32, 64, 128, 1024, 2048, 4096 };
            foreach (int blockLength in dataBlockLength)
            {
                FwtNormM5ShortTestCase fwtCase = new FwtNormM5ShortTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }
        }   
        private void AddFwtNormM5RecentCrossTestCase(TestCaseGroup group)
        {
            int populatim = 512;
            int[] dataBlockLength = new int[] { 16, 32, 64, 128, 1024, 2048, 4096 };
            foreach (int blockLength in dataBlockLength)
            {
                KDJNormRecentTestCase fwtCase = new KDJNormRecentTestCase() { PopulationNumber = populatim, DataBlockLength = blockLength };
                group.Add(fwtCase);
            }
        }   
        private void AddNewTestCase(TestCaseGroup group)
        {
            BasicNewTestCase[] testCaseArr = NewTestCollecor.GetTest();
            foreach( BasicNewTestCase ca in testCaseArr)
                group.Add(ca);

            TestCaseGroup g = new TestCaseGroup
            {
                //------------------------------------
                new NewTestCase2Short(),
                // new NewTestCase(),
                new NewTestCase2(),
                new NewTestCase_FWT(),
                new NewTestCase_All(),
                new NewTestCase_All_5Min_Short(),
                new NewTestCase_All_Switch_5Min_Short(),
                new NewTestCase_All_Switch_1Day_Short(),
                new NewTestCase_All_SwitchClose_5Min_Short(),
                new NewTestCase_All_1Day_Short(),
                new NewTestCase_All_1Day_Long(),

            };

            group.Add(g);
        }
        private void AddTestCase()
        {
            TestCaseGroup oldTestList = new TestCaseGroup()
            {
                new TestCaseObject("FWT_Cuda_Test", "", new TestCaseObject.TestFucntion(FWT_Cuda_Test)),
                new TestCaseObject("FWT_5min_Long", "", new TestCaseObject.TestFucntion(FWT_5min_Long)),
                new TestCaseObject("FWT_5min_Simple", "", new TestCaseObject.TestFucntion(FWT_5min_Simple)),
                new TestCaseObject("TestFWT", "", new TestCaseObject.TestFucntion(TestFWT)),
                new TestCaseObject("Long5Min_Simple", "", new TestCaseObject.TestFucntion(Long5Min_Simple)),
                new TestCaseObject("Test_5Min_Short", "", new TestCaseObject.TestFucntion(Test_5Min_Short)),
                new TestCaseObject("Test_1Min_Short", "", new TestCaseObject.TestFucntion(Test_1Min_Short)),
                new TestCaseObject("Test_1Min_Long", "", new TestCaseObject.TestFucntion(Test_1Min_Long)),
                new TestCaseObject("TestRateMarketNEAT_Long", "", new TestCaseObject.TestFucntion(TestRateMarketNEAT_Long)),
                new TestCaseObject("TestRateMarketNEAT_Short", "", new TestCaseObject.TestFucntion(TestRateMarketNEAT_Short)),
                new TestCaseObject("TestRateMarketNEAT", "", new TestCaseObject.TestFucntion(TestRateMarketNEAT)),
                // new TestCaseObject("TestDAO", "", new TestCaseObject.TestFucntion(TestDAO)),
                // new TestCaseObject("TestRateMarketNEATBatch", "", new TestCaseObject.TestFucntion(TestRateMarketNEATBatch)),
                // new TestCaseObject("TestRateMarketAgent", "", new TestCaseObject.TestFucntion(TestRateMarketAgent)),
                // new TestCaseObject("TestAnn", "", new TestCaseObject.TestFucntion(TestANN)),
                // new TestCaseObject("TestMarketAnalyz", "", new TestCaseObject.TestFucntion(TestMarketAnalyz)),
                // new TestCaseObject("RateAnalyzeTest", "", new TestCaseObject.TestFucntion(RateAnalyzeTest)),
                // new TestCaseObject("TestNEATNet", "", new TestCaseObject.TestFucntion(TestNEATNet)),
                // new TestCaseObject("TestBPTrain", "", new TestCaseObject.TestFucntion(TestBPTrain)),
            };

            TestCaseGroup newTestList = new TestCaseGroup();
            newTestList.Add(new TestCaseObject("TestDataBaseViewer", "", new TestCaseObject.TestFucntion(TestDataBaseViewer)));

            newTestList.Add(new TestCaseObject("TestFWT", "", new TestCaseObject.TestFucntion(TestFWT)));
            // New test case
            AddNewTestCase(newTestList);

            AddFwtNormM5RecentCrossTestCase(newTestList);
            AddKdjNormM5CrossTestCase(newTestList);
            AddKdjM5CrossTestCase(newTestList);
            AddRateNormM5CrossTestCase(newTestList);
            AddFwtNormM5CrossTestCase(newTestList);

            AddRecentM5StepCrossTestCase(newTestList);
            AddRecentM5CrossTestCase(newTestList);
            AddRecentCrossTestCase(newTestList);
            // AddCrossTestCase(newTestList);
            /*
            newTestList.Add(new FwtNorm5MinTestCase() { PopulationNumber = 100, DataBlockLength = 32 });
            newTestList.Add(new FwtNorm5MinTestCase() { PopulationNumber = 100, DataBlockLength = 1024 });
            newTestList.Add(new FwtNorm1DayTestCase() { PopulationNumber = 100, DataBlockLength = 32 });
            newTestList.Add(new FwtNorm1Day3StateTestCase() { PopulationNumber = 100, DataBlockLength = 32 });
            newTestList.Add(new FwtNorm1Day3StateTestCase() { PopulationNumber = 100, DataBlockLength = 64 });
            newTestList.Add(new FwtNorm1Day3StateTestCase() { PopulationNumber = 100, DataBlockLength = 128 });
            newTestList.Add(new RawRate1DayTestCase() { PopulationNumber = 100, DataBlockLength = 32 });
            newTestList.Add(new RawRate5MinTestCase() { PopulationNumber = 100, DataBlockLength = 32 });
            newTestList.Add(new RawRate5MinTestCase() { PopulationNumber = 100, DataBlockLength = 1024 });
            newTestList.Add(new NormalRateMarketTestCase() { PopulationNumber = 100 });
            newTestList.Add(new NormalRateMarketTestCase() { PopulationNumber = 1000 });
            newTestList.Add(new Normal5MinRateMarketTestCase() { PopulationNumber = 100 });
            newTestList.Add(new Normal5MinRateMarketTestCase() { PopulationNumber = 1000 });
            */
            TestCaseList.Add(newTestList);

            // TestCaseList.Add(oldTestList);
             
        }
    }
}
