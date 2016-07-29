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

using MyProject01.Networks;
using MyProject01.Test;

using MyProject01.DAO;
using MyProject01.Win;
using MyProject01.Util.DllTools;
using MyProject01.Controller;

using System.Threading;
using MyProject01.DataSources;

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

    interface ITestCase
    {
        string Name { get; }
        string Description { get; }
        void Run();
    }
    class TestCaseGroup : List<TestCaseObject>
    {
        public void Add(TestCaseGroup group)
        {
            foreach (TestCaseObject obj in group)
                Add(obj);
        }


        public void Add(BasicNewTestCase testCase)
        {
            TestCaseObject obj = new TestCaseObject(testCase.TestCaseName, "", new TestCaseObject.TestFucntion(testCase.Run));
            Add(obj);
        }

        public void Add(ITestCase test)
        {
            TestCaseObject obj = new TestCaseObject(test.Name, test.Description, new TestCaseObject.TestFucntion(test.Run));
            Add(obj);
        }
        public void Add(List<ITestCase> testList)
        {
            foreach (ITestCase tc in testList)
                Add(tc);
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

            // BuyOffset
            //---------------------------
            TextBox buyOffsetTb = new TextBox();
            buyOffsetTb.Text = CommonConfig.BuyOffset.ToString();
            buyOffsetTb.TextChanged += new TextChangedEventHandler(
                    delegate(object sender, TextChangedEventArgs args)
                    {
                        double result;
                        if (double.TryParse(buyOffsetTb.Text, out result) == false)
                            return;
                        CommonConfig.BuyOffset = result;
                    }
                );
            AddParamConfigUI("BuyOffset", buyOffsetTb);

            // SellOffset
            //---------------------------
            TextBox sellOffsetTb = new TextBox();
            sellOffsetTb.Text = CommonConfig.SellOffset.ToString();
            sellOffsetTb.TextChanged += new TextChangedEventHandler(
                    delegate(object sender, TextChangedEventArgs args)
                    {
                        double result;
                        if (double.TryParse(sellOffsetTb.Text, out result) == false)
                            return;
                        CommonConfig.SellOffset = result;
                    }
                );
            AddParamConfigUI("SellOffset", sellOffsetTb);

            AddTrainingDataBlockLengthParam();
            AddTrainingTryCountParam();

            // LoaderParam
            //------------------------
            StackPanel loaderPanel = new StackPanel();
            foreach (DataLoaderParam parm in DataLoaderParamList.GetParams())
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
        private void AddTrainingDataBlockLengthParam()
        {
            TextBox tb = new TextBox();
            tb.Text = CommonConfig.TrainingDataBlockLength.ToString();
            tb.TextChanged += new TextChangedEventHandler(
                    delegate(object sender, TextChangedEventArgs args)
                    {
                        int result;
                        if (int.TryParse(tb.Text, out result) == false)
                            return;
                        CommonConfig.TrainingDataBlockLength = result;
                    }
                );
            AddParamConfigUI("TrainingDataBlockLength", tb);
        }
        private void AddTrainingTryCountParam()
        {
            TextBox tb = new TextBox();
            tb.Text = CommonConfig.TrainingTryCount.ToString();
            tb.TextChanged += new TextChangedEventHandler(
                    delegate(object sender, TextChangedEventArgs args)
                    {
                        int result;
                        if (int.TryParse(tb.Text, out result) == false)
                            return;
                        CommonConfig.TrainingTryCount = result;
                    }
                );
            AddParamConfigUI("TrainingTryCount", tb);
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
        

        private void TestDataBaseViewer()
        {
            this.Dispatcher.BeginInvoke(new Func(delegate()
            {
                DataBaseViewer win = new DataBaseViewer();
                win.Closed += new EventHandler(
                    delegate(object sender, EventArgs args)
                    {
                        Application.Current.Shutdown();
                    }
                );
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                win.Show();
            }));
        }
        private void ControllerViewer()
        {
            this.Dispatcher.BeginInvoke(new Func(delegate()
            {
                ControllerCheckWin win = new ControllerCheckWin();
                win.Closed += new EventHandler(
                    delegate(object sender, EventArgs args)
                    {
                        Application.Current.Shutdown();
                    }
                );
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                win.Show();
            }));
        }

        private void TestDataAnalyzer()
        {
            this.Dispatcher.BeginInvoke(new Func(delegate()
            {
                BasicTestDataLoader _loader = NewTestDataPacket.GetRecnetM30_3Month();
                _loader.Load();
                DataSourceCtrl dsc = new DataSources.DataSourceCtrl(_loader);
                ISensor Sensor = new RateWaveletSensor(64, new Daubechies20Wavelet(), 4);
                Sensor.DataSourceCtrl = dsc;

                DataAnalyzer[] anzArr = new DataAnalyzer[Sensor.DataBlockLength];
                for (int i = 0; i < anzArr.Length;i++ )
                {
                    anzArr[i] = new DataAnalyzer();
                    anzArr[i].Init(2048);
                }
                DataBlock db = new DataBlock(Sensor.DataBlockLength);
                for(int i=50000;i<Sensor.TotalLength;i++)
                {
                    Sensor.Copy(i, db, 0);
                    for (int j = 0; j < db.Length;j++ )
                        anzArr[j].AddData(db.Data[j]);
                }

                foreach (DataAnalyzer anz in anzArr)
                {
                    DataAnalyzerDesc[] descArr = anz.GetResult();
                    LogFile.WriteLine("DataAnalyzer[]");
                    LogFile.WriteLine("========================");
                    foreach (DataAnalyzerDesc desc in descArr)
                    {
                        LogFile.WriteLine(desc.ToString());
                    }
                }

            }));
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

            TestCaseGroup newTestList = new TestCaseGroup();
            newTestList.Add(new TestCaseObject("TestDataBaseViewer", "", new TestCaseObject.TestFucntion(TestDataBaseViewer)));
            newTestList.Add(new TestCaseObject("ControllerViewer", "", new TestCaseObject.TestFucntion(ControllerViewer)));
            newTestList.Add(new TestCaseObject("TestDataAnalyzer", "", new TestCaseObject.TestFucntion(TestDataAnalyzer)));


            // Add Renko Test
            newTestList.Add(RenkoTestCaseList.GetTest());

            // New test case
            AddNewTestCase(newTestList);

            TestCaseList.Add(newTestList);

        }
    }
}
