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
    public class GraphMark
    {
        private Panel _partenPanel;
        private Brush _color;
        private int _thickness;
        private Point _point;
        private double _width = 4;
        private double _height = 8;

        private Ellipse _referedShape;

        public double ScaleX = 1.0;
        public double ScaleY = 1.0;

        public GraphMark( Panel parentPanel, Point point, Brush color, int thickness = 1 )
        {
            // point.X -= 0.5;
            // point.X -= _width * 0.5;
            // point.Y -= _height * 0.5;
            this._partenPanel = parentPanel;
            this._point = point;
            this._color = color;
            this._thickness = thickness; 

        }

        public void Update()
        {
            bool isNew = false;
            if (_referedShape == null)
            {
                _referedShape = new Ellipse();
                _referedShape.Fill = this._color;
                isNew = true;
            }
            _referedShape.StrokeThickness = _thickness;
            _referedShape.Stroke = this._color;
            _referedShape.Width = (int)(_width);
            _referedShape.Height = (int)(_height);
            _referedShape.VerticalAlignment = VerticalAlignment.Top;
            _referedShape.Margin = new Thickness(_point.X * ScaleX -_width*0.5, _point.Y * ScaleY -_height * 0.5, 0, 0);
            // _referedShape.Margin = new Thickness(_point.X * ScaleX, _point.Y * ScaleY, 0, 0);

            if( isNew == true)
                _partenPanel.Children.Add(_referedShape);
            
        }

        public void Remvoe()
        {
            if (_referedShape != null)
            {
                _partenPanel.Children.Remove(_referedShape);
                _partenPanel = null;
            }

        }
    }
}
