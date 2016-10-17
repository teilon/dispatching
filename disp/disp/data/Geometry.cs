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
    static class DBConst
    {
        public static string ConnectionString = @"Data Source=10.8.0.4;Initial Catalog=docflow3;Persist Security Info=True;User ID=sa;Password=123123qwE";
        //public static string ConnectionString = @"Data Source=192.168.0.101;Initial Catalog=docflow3;Persist Security Info=True;User ID=sa;Password=@qwerty123";
    }
    public static class Geometry
    {
        public static bool Intersection(Line line, Point point, double radius = 0)
        {
            return SQLIntersection(line, point, radius);
        }
        public static bool Intersection(Point pointA, Point pointB, double radius = 0)
        {
            return SQLIntersection(pointA, pointB, radius);
        }

        static bool SQLIntersection(Line line, Point point, double radius)
        {
            Regex regex = new Regex(",");
            int result;
            using (SqlConnection conn = new SqlConnection(DBConst.ConnectionString))
            {
                DbCommand command = new SqlCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "grp_in_area";

                string _point = string.Format("POINT({0} {1})", regex.Replace(point.X.ToString(), "."), regex.Replace(point.Y.ToString(), "."));
                string _line = "LINESTRING(";
                foreach (Point p in line.Points)
                {
                    _line += string.Format("{0} {1},", regex.Replace(p.X.ToString(), "."), regex.Replace(p.Y.ToString(), "."));
                }
                _line = _line.Substring(0, _line.Length - 1);
                _line += ")";

                SqlParameter param1 = new SqlParameter("@point", _point);
                command.Parameters.Add(param1);
                SqlParameter param2 = new SqlParameter("@line", _line);
                command.Parameters.Add(param2);
                SqlParameter param3 = new SqlParameter("@radius", radius);
                command.Parameters.Add(param3);

                SqlParameter returnValue = new SqlParameter("@result", 0);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValue);

                command.Connection = conn;
                command.Connection.Open();
                command.ExecuteNonQuery();
                result = Int32.Parse(command.Parameters["@result"].Value.ToString());
                command.Connection.Close();
            }
            //Console.WriteLine(" >> {0}", result);
            return result == 1;
        }
        static bool SQLIntersection(Point pointA, Point pointB, double radius)
        {
            Regex regex = new Regex(",");
            int result;
            using (SqlConnection conn = new SqlConnection(DBConst.ConnectionString))
            {
                DbCommand command = new SqlCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "grp_in_area_points";

                string _pointA = string.Format("POINT({0} {1})", regex.Replace(pointA.X.ToString(), "."), regex.Replace(pointA.Y.ToString(), "."));
                string _pointB = string.Format("POINT({0} {1})", regex.Replace(pointB.X.ToString(), "."), regex.Replace(pointB.Y.ToString(), "."));

                SqlParameter param1 = new SqlParameter("@pointA", _pointA);
                command.Parameters.Add(param1);
                SqlParameter param2 = new SqlParameter("@pointB", _pointB);
                command.Parameters.Add(param2);
                SqlParameter param3 = new SqlParameter("@radius", regex.Replace(radius.ToString(), "."));
                command.Parameters.Add(param3);

                SqlParameter returnValue = new SqlParameter("@result", 0);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValue);

                command.Connection = conn;
                command.Connection.Open();
                command.ExecuteNonQuery();
                result = Int32.Parse(command.Parameters["@result"].Value.ToString());
                command.Connection.Close();
            }
            //Console.WriteLine(" >> {0}", result);
            return result == 1;
        }
    }
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
        List<Point> _points;
        public List<Point> Points { get { return _points; } }
        public Line()
        {
            _points = new List<Point>();
        }
    }
}
