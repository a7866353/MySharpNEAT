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
using System.Windows.Shapes;

namespace MyProject01.Util.View
{

    class RateSetUtility
    {
        public double maxValue;
        public double minValue;
        public double[] rateSetArr;

        public List<int> markList;

        public double posX = 0;
        public double posY = 0;
        public double scaleX = 1.0;
        public double scaleY = 1.0;
        public RateSetUtility(RateSet[] rateSetArr)
        {
            double[] dataArr = new double[rateSetArr.Length];
            for (int i = 0; i < dataArr.Length; i++)
            {
                dataArr[i] = rateSetArr[i].Close;
            }
            setData(dataArr);
            markList = new List<int>();
            
        }

        public RateSetUtility(double[] rateSetArr)
        {
            setData(rateSetArr);
            markList = new List<int>();
        }
        public void AddMark(int markIndex)
        {
            markList.Add(markIndex);
        }

        private void setData(double[] rateSetArr)
        {
            this.rateSetArr = rateSetArr;
            maxValue = minValue = rateSetArr[0];
            for (int i = 0; i < rateSetArr.Length; i++)
            {
                // maxValue
                if (rateSetArr[i] > maxValue)
                    maxValue = rateSetArr[i];

                if (rateSetArr[i] < minValue)
                    minValue = rateSetArr[i];
            }
        }

        public Geometry GetLine()
        {
            Point[] testPoints = new Point[rateSetArr.Length];
            for (int i = 0; i < testPoints.Length; i++)
            {
                testPoints[i] = new Point(posX + i * scaleX, posY + rateSetArr[i] * scaleY);
            }

            Geometry geo = PathTest.DrawLineByPointArr(testPoints);
            return geo;
        }
        public GeometryGroup GetMark()
        {
            double radiusX = 5;
            double radiusY = 20;
            GeometryGroup markGeoGroup = new GeometryGroup();
            foreach (int index in markList)
            {
                Point p = new Point(posX + index * scaleX, posY + rateSetArr[index] * scaleY);
                Geometry geo = new EllipseGeometry(p, radiusX, radiusY);
                markGeoGroup.Children.Add(geo);
            }
            return markGeoGroup;
        }
    }


    /// <summary>
    /// GraphViewer.xaml 的交互逻辑
    /// </summary>
    public partial class GraphViewer : Window
    {
        private Canvas target;
        private TransformGroup transGroup;
        private ScaleTransform scale;
        private TranslateTransform translate;

        private List<GraphLine> _graphLineList;
        //-------------
        private double mouseWheelStep = 0.1;
        private Point mouseLastPoint;

        //------------------------------
        private static GraphViewer _Instance = null;

        //---------------------
        private bool isMouseSet;
        private Point sourceMousePoint;
        private Point sourcePoint;
        private Point preScalePoint;

        public static GraphViewer Instance
        {
            get
            {
                return _Instance;
            }
        }

        //-------------------
        private double transformPosX = 0;
        private double transformPosY = 0;
        private double transformScaleX = 1;
        private double transformScaleY = 1;
        private double transformPosStep = 10;
        private double transformScaleStep = 1.2;

        //-------------------
        private double targetHeight = 1000;
        private Brush[] _lineColorArray = new Brush[]
            {
                Brushes.Black,
                Brushes.Blue,
                Brushes.Red,
                // Brushes.Yellow,
                Brushes.Aqua,
                Brushes.Coral,
                Brushes.Peru,
            };
        private int _lineColorIndex = 0;

        //-------------------------
        private List<RateSetUtility> RateSetDataList;


        //-------------------------
        private delegate void func();

        public GraphViewer()
        {
            InitializeComponent();

            this.MouseWheel += new MouseWheelEventHandler(MainWindow_MouseWheel);
            this.MouseMove += GraphViewer_MouseMove;
            this.KeyDown += GraphViewer_KeyDown;
            this.Loaded += MainWindow_Loaded;
            GraphViewer._Instance = this;

            RateSetDataList = new List<RateSetUtility>();
            _graphLineList = new List<GraphLine>();

        }

        void GraphViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                translate.X = 0;
                translate.Y = target.ActualHeight;

                transformScaleY /= target.Children[0].DesiredSize.Height / target.ActualHeight;
                transformScaleX /= target.Children[0].DesiredSize.Width / target.ActualWidth;

                UpdateLine();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Init Panel
            target = new Canvas();
            transGroup = new TransformGroup();
            // target.ClipToBounds = true;
            scale = new ScaleTransform(1, -1);
            transGroup.Children.Add(scale);
            translate = new TranslateTransform(0, 0);
            transGroup.Children.Add(translate);
            target.RenderTransform = transGroup;
            this.Content = target;
            /*
                        target.MaxWidth = 5000;
                        target.MaxHeight = 5000;
              */
            // Create a path to draw a geometry with.

            // DataLoader dataLoader = new DataLoader();
            // AddRateSet(dataLoader.ToArray());

        }


        private void printTrans()
        {
            System.Console.WriteLine("Trans:" + scale.CenterX.ToString() + "\t"
                + scale.CenterY.ToString() + "\t"
                + scale.ScaleX.ToString() + "\t"
                + scale.ScaleY.ToString() + "\t"
                + Keyboard.GetKeyStates(Key.LeftShift));
        }
        void GraphViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                isMouseSet = false;
                return;
            }
            if (isMouseSet == false)
            {
                isMouseSet = true;
                sourceMousePoint = e.GetPosition(this);
                sourcePoint.X = translate.X;
                sourcePoint.Y = translate.Y;
                return;
            }

            Point center = e.GetPosition(this);

            translate.X = sourcePoint.X + (center.X - sourceMousePoint.X);
            translate.Y = sourcePoint.Y + (center.Y - sourceMousePoint.Y);
            System.Console.WriteLine("Y:" + translate.Y.ToString());

        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            bool isImage = false;
            if (target == null)
                return;

            if (isImage == true)
            {
                Point moucePoint = Mouse.GetPosition(target);
                System.Console.WriteLine("Mouse: " + moucePoint.ToString());
                if (mouseLastPoint == null)
                    mouseLastPoint = moucePoint;
                if (Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down)
                {
                    if (e.Delta > 0)
                    {
                        scale.ScaleX += mouseWheelStep;
                    }
                    else if (e.Delta < 0)
                    {
                        scale.ScaleX -= mouseWheelStep;
                    }
                }
                else if (Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down)
                {
                    if (e.Delta > 0)
                    {
                        scale.ScaleY += mouseWheelStep;
                    }
                    else if (e.Delta < 0)
                    {
                        scale.ScaleY -= mouseWheelStep;
                    }

                }
                else
                {
                    if (e.Delta > 0)
                    {
                        mouseLastPoint = moucePoint;
                        scale.ScaleX += mouseWheelStep;
                        scale.ScaleY += mouseWheelStep;
                    }
                    else if (e.Delta < 0)
                    {
                        scale.ScaleX -= mouseWheelStep;
                        scale.ScaleY -= mouseWheelStep;
                    }
                }
            }
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (e.Delta > 0)
                    {
                        transformScaleX *= transformScaleStep;
                    }
                    else if (e.Delta < 0)
                    {
                        transformScaleX /= transformScaleStep;
                    }
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (e.Delta > 0)
                    {
                        transformScaleY *= transformScaleStep;
                    }
                    else if (e.Delta < 0)
                    {
                        transformScaleY /= transformScaleStep;
                    }
                }
                else
                {
                    if (e.Delta > 0)
                    {
                        transformScaleX *= transformScaleStep;
                        transformScaleY *= transformScaleStep;
                    }
                    else if (e.Delta < 0)
                    {
                        transformScaleX /= transformScaleStep;
                        transformScaleY /= transformScaleStep;
                    }
                }
                UpdateLine();
            }
            // scale.CenterX = mouseLastPoint.X;
            // scale.CenterY = mouseLastPoint.Y;
            printTrans();
            

            /*
            int radio = 500;
            double imageScaleRadio = target.Height / target.ActualHeight;
            Point center;
            center = e.GetPosition(target);
            double delta = (double)e.Delta / radio;
            double x = scale.ScaleX + delta;
            double y = scale.ScaleY + delta;

            scale.CenterX = center.X;
            scale.CenterY = center.Y;


            if ((target.ActualWidth * x < 100) ||
                (target.ActualWidth * y < 100) ||
                (x > 100) ||
                (y > 100))
                return;

            scale.ScaleX = x;
            scale.ScaleY = y;
            */

        }

        private void UpdateLine()
        {
            foreach (GraphLine line in _graphLineList)
            {
                line.ScaleX = transformScaleX;
                line.ScaleY = transformScaleY;
                line.Update();
            }
        }

        private Brush GetNextColor()
        {
            Brush color = _lineColorArray[_lineColorIndex];
            _lineColorIndex++;
            if (_lineColorIndex >= _lineColorArray.Length)
                _lineColorIndex = 0;
            return color;
        }

        #region Pulic_Function
        public GraphLine AddRateSet(RateSet[] rateSetArr)
        {
            double[] dataArray = new double[rateSetArr.Length];
            for (int i = 0; i < dataArray.Length; i++)
                dataArray[i] = rateSetArr[i].Close;
            return AddLineData(dataArray);


            /*
            RateSetUtility rateSetObj = new RateSetUtility(rateSetArr);
            RateSetDataList.Add(rateSetObj);
            this.Dispatcher.BeginInvoke(new func(delegate
            {
                transformPosX = 0;
                transformPosY = 0;
                transformScaleY = 400 / rateSetObj.maxValue;
                transformScaleX = 0.5;

                rateSetObj.posX = transformPosX;
                rateSetObj.posY = transformPosY;
                rateSetObj.scaleX = transformScaleX;
                rateSetObj.scaleY = transformScaleY;
                geometrys.Children.Add(rateSetObj.GetLine());
            }));
            // Return Index
            return RateSetDataList.Count - 1;
            */
        }
        public GraphLine AddLineData(double[] rateSetArr)
        {
            /*
            RateSetUtility rateSetObj = new RateSetUtility(rateSetArr);
            RateSetDataList.Add(rateSetObj);
            this.Dispatcher.BeginInvoke(new func(delegate
            {
                transformPosX = 0;
                transformPosY = 0;
                transformScaleY = 400 / rateSetObj.maxValue;
                transformScaleX = 0.5;

                rateSetObj.posX = transformPosX;
                rateSetObj.posY = transformPosY;
                rateSetObj.scaleX = transformScaleX;
                rateSetObj.scaleY = transformScaleY;
                geometrys.Children.Add(rateSetObj.GetLine());
            }));
             // Return Index
            return RateSetDataList.Count - 1;
           */

            // 输入数据规范化
            DataNormallizer norm = new DataNormallizer();
            norm.Set(rateSetArr, 0, rateSetArr.Length);
            norm.DataValueAdjust(0, targetHeight);

            // 创建Line
            GraphLine line = new GraphLine(target, rateSetArr, GetNextColor(), 1);
            _graphLineList.Add(line);
            this.Dispatcher.Invoke(new func(delegate
            {
                line.ScaleX = transformScaleX;
                line.ScaleY = transformScaleY;
                line.Update();
            }));
            return line;
        }

        public void PositionReset()
        {

        }

        /*
        public void AddMark(int index, DealPointInfomation[] markInfoArr)
        {
            int[] markArr = new int[markInfoArr.Length];
            for (int i = 0; i < markInfoArr.Length; i++)
            {
                markArr[i] = markInfoArr[i].Index;
            }
            AddMark(index, markArr);
        }


        public void AddMark(int index, int[] markArr)
        {
            if (RateSetDataList.Count < index + 1)
                return;
            RateSetUtility rateSetObj = RateSetDataList[index];

            foreach (int mark in markArr)
            {
                rateSetObj.markList.Add(mark);
            }
            this.Dispatcher.BeginInvoke(new func(delegate
            {
                geometrys.Children.Add(rateSetObj.GetMark());
            }));
        }
        */

        #endregion
    }
}
