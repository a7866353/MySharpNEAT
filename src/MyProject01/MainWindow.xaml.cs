using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using MyProject01.Util;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MyProject01.Test;
using MyProject01.Util.View;

namespace MyProject01
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void func();
        private Thread workThread;
        private TestCaseObject _testObject;
        private StreamWriter _fileLogStream;
        public MainWindow(TestCaseObject testObject)
        {
            InitializeComponent();
            this._testObject = testObject;
            
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Closing += MainWindow_Closing;


            OutputTextBox.Text = "";
            OutputTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            
            OutputTextBox.Foreground = Brushes.Green;
            OutputTextBox.FontSize = 12;
            OutputTextBox.Background = Brushes.Black;

            // Set lowest priority
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle;

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (workThread != null)
                workThread.Abort();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string logName = "Log.txt";
            bool isLogFileLocked = false;
            FileStream fs = null;
            try
            {
                fs = File.Open(logName, FileMode.Create);
            }
            catch(Exception excp)
            {
                isLogFileLocked = true;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            if (isLogFileLocked == false)
            {
                _fileLogStream = new StreamWriter(logName, false);
                _fileLogStream.AutoFlush = true;
                
            }
            LogFile.FuncList.Add(new LogFile.WriteLineFunction(WriteText));
            LogFile.FuncList.Add(new LogFile.WriteLineFunction(WriteToFile));
           workThread = new Thread(new ThreadStart(MainWorkFunction));
            workThread.Priority = ThreadPriority.BelowNormal;
            workThread.Start();

        }
        private void CloseWindows()
        {
            this.Dispatcher.BeginInvoke(new func(delegate
            {
                if (_fileLogStream != null)
                    _fileLogStream.Close();
                this.Close();

            }));
        }
        private void WriteText(string str)
        {
            this.Dispatcher.BeginInvoke (new func(delegate
            {
                if (OutputTextBox.Text.Length > 50000)
                    OutputTextBox.Clear();
                OutputTextBox.AppendText(str + "\r\n");
                OutputTextBox.ScrollToEnd();
            }));
        }
        
        private void WriteToFile(string str)
        {
            if (_fileLogStream == null)
                return;
            _fileLogStream.WriteLine(str);
        }

        private void MainWorkFunction()
        {
            LogFile.WriteLine(@"Test start...");
            if (_testObject.TestFunction != null)
                _testObject.TestFunction();
            LogFile.WriteLine(@"Test end...");
        }
    }




}
