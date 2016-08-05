using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyProject01.Util.View
{
    public class GraphLine
    {        
        private Panel _partenPanel;
        private Brush _color;
        private int _thickness;
        private double _width = 10;
        private double _height = 10;

        static private Brush[] _lineColorArray = new Brush[]
            {
                Brushes.Black,
                Brushes.Red,
                Brushes.Yellow,
                Brushes.Blue,
                Brushes.Aqua,
                Brushes.Coral,
                Brushes.Peru,
            };
        static private int _lineColorIndex = 0;

        private Path _referedShape;
        private List<GraphMark> _graphMarkList;
        private double[] _dataArray;

        public double ScaleX = 100;
        public double ScaleY = -100;

        public GraphLine(Panel parentPanel, double[] dataArray, Brush color, int thickness = 1)
        {
            this._partenPanel = parentPanel;
            this._dataArray = dataArray;
            this._color = color;
            this._thickness = thickness;

            /*
            this._color = _lineColorArray[_lineColorIndex];
            _lineColorIndex++;
            if (_lineColorIndex >= _lineColorArray.Length)
                _lineColorIndex = 0;
            */
            _graphMarkList = new List<GraphMark>();

        }

        public void AddMark(int index, Brush color, int thickness = 1)
        {
            if (index >= _dataArray.Length)
                index = _dataArray.Length - 1;
            GraphMark mark = new GraphMark(_partenPanel, new Point(index, _dataArray[index]), color, thickness);
            mark.ScaleX = ScaleX;
            mark.ScaleY = ScaleY;
            _graphMarkList.Add(mark);
        }

        public void Update()
        {
            bool isNew = false;
            if (_referedShape == null)
            {
                _referedShape = new Path();
                isNew = true;
            }
            _referedShape.StrokeThickness = _thickness;
            _referedShape.Stroke = this._color;
            _referedShape.Data = GetLine();
            if(isNew == true)
                _partenPanel.Children.Add(_referedShape);

            // Update Mark
            foreach(GraphMark mark in _graphMarkList)
            {
                mark.ScaleX = ScaleX;
                mark.ScaleY = ScaleY;
                mark.Update();
            }
            
        }

        public void Remvoe()
        {
            if (_referedShape != null)
            {
                _partenPanel.Children.Remove(_referedShape);
                _referedShape = null;

                foreach (GraphMark mark in _graphMarkList)
                {
                    mark.Remvoe();
                    _graphMarkList.Remove(mark);
                }

            }

        }

        private Geometry GetLine()
        {
            Point[] testPoints = new Point[_dataArray.Length];
            for (int i = 0; i < testPoints.Length; i++)
            {
                testPoints[i] = new Point(i * ScaleX, _dataArray[i] * ScaleY);
            }

            Geometry geo = PathTest.DrawLineByPointArr(testPoints);
            return geo;
        }
    }
}
