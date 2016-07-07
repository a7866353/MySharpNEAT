using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace MyProject01
{
    class PathTest
    {
        public void Draw()
        {

        }

        public Geometry StreamGeometryPointExample()
        {
            Point[] pointArr = new Point[1000];

            int pos = -1;
            for (int i = 0; i < pointArr.Length; i++)
            {
                pointArr[i] = new Point(i, pos + 100);
                pos++;
                if (pos > 1)
                    pos = -1;
            }

            return DrawPointByPointArr(pointArr);
        }
        public static Geometry DrawPointByPointArr(Point[] pointArr)
        {
            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            // Open a StreamGeometryContext that can be used to describe this StreamGeometry 
            // object's contents.
            using (StreamGeometryContext ctx = geometry.Open())
            {
                for (int i = 0; i < pointArr.Length; i++)
                {
                    // Begin the triangle at the point specified. Notice that the shape is set to 
                    // be closed so only two lines need to be specified below to make the triangle.
                    ctx.BeginFigure(pointArr[i], false /* is filled */, true /* is closed */);

                    // Draw a line to the next specified point.
                    ctx.LineTo(pointArr[i], true /* is stroked */, false /* is smooth join */);
                }
            }
            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();

            return geometry;
        }


        //---------------------------------------------------------
      
        public static Geometry DrawLineByPointArr(Point[] pointArr)
        {
            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            // Open a StreamGeometryContext that can be used to describe this StreamGeometry 
            // object's contents.
            using (StreamGeometryContext ctx = geometry.Open())
            {
                // Begin the triangle at the point specified. Notice that the shape is set to 
                // be closed so only two lines need to be specified below to make the triangle.
                ctx.BeginFigure(pointArr[0], false /* is filled */, false /* is closed */);
                for (int i = 1; i < pointArr.Length; i++)
                {
                    // Draw a line to the next specified point.
                    ctx.LineTo(pointArr[i], true /* is stroked */, false /* is smooth join */);
                }
            }
            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();

            return geometry;
        }

        private void scalePointArr(Point[] pointArr, double scaleX, double scaleY)
        {
            if (pointArr == null)
                return;

            for (int i = 0; i < pointArr.Length; i++)
            {
                pointArr[i].X = pointArr[i].X * scaleX;
                pointArr[i].Y = pointArr[i].Y * scaleY;
            }
        }
        private void movePointArr(Point[] pointArr, double x, double y)
        {
            if (pointArr == null)
                return;

            for (int i = 0; i < pointArr.Length; i++)
            {
                pointArr[i].X = pointArr[i].X + x;
                pointArr[i].Y = pointArr[i].Y + y;
            }
        }

/*
        //---------------------------------------------------------
        public Geometry StreamGeometryAreaExample()
        {
            Point[] areaPoints;
            Point[] testPoints = new Point[2000];
            PointsCreater creater = new PointsCreater(new Point(-1, 0), new Point(1, 1));
            creater.judgeFuncList.Add(new PointsCreater.JudgeFuc(edgeFunc1));
            areaPoints = creater.GetByDefinition(1000,1000);

            int cnt = 0;
            int pointNo,loopCnt;
            Random rand = new Random();
            while (true)
            {
                // get a point
                pointNo = rand.Next(areaPoints.Length);
                for (loopCnt = 0; loopCnt < cnt; loopCnt++)
                {
                    if (testPoints[loopCnt] == areaPoints[pointNo])
                        break;
                }
                if (loopCnt == cnt)
                {
                    testPoints[cnt] = areaPoints[pointNo];
                    cnt++;
                    if (cnt == testPoints.Length)
                        break;
                }
            }
            movePointArr(testPoints, 1, 0);
            scalePointArr(testPoints, 100, 100);
            return DrawPointByPointArr(testPoints);
        }
 */
        private bool edgeFunc1(Point point)
        {
            const double radiusLarge = 1;
            const double radiusSmall = 0.7;

            double res = point.X * point.X + point.Y * point.Y;
            if ((res <= radiusLarge * radiusLarge) && (res >= radiusSmall * radiusSmall))
            {
                return true;
            }

            return false;
        }
    }
}
