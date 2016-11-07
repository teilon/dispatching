using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace disp
{

    public class Point
    {
        public double X;
        public double Y;
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public class Line
    {
        public string Name;
        public string Type;
        List<Point> _points;
        public List<Point> Points { get { return _points; } }
        public Line()
        {
            _points = new List<Point>();
        }
    }
}
