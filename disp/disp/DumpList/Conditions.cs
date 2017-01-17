using System;
using System.Collections.Generic;
using System.Linq;
using System.Device.Location;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public partial class DumpList
    {
        const double LOADINGRADIUS = 20;
        const double LOADINGZONERADIUS = 50;


        bool FindNearLoadingZone(GeoCoordinate coordinate)
        {
            foreach (PointParam pointparam in _loadingpoints.LoadingPoints.Values)
            {
                if (pointparam.Coordinate == null)
                    return false;
                double _radius = coordinate.GetDistanceTo(pointparam.Coordinate);
                if (_radius < pointparam.LoadingZoneRadius)
                    return true;
            }
            return false;
        }
        bool FindNearExcavator(GeoCoordinate coordinate)
        {
            foreach (PointParam pointparam in _loadingpoints.LoadingPoints.Values)
            {                   
                if (pointparam.Coordinate == null)
                    return false;
                double _radius = coordinate.GetDistanceTo(pointparam.Coordinate);
                if (_radius < pointparam.LoadingRadius)
                    return true;
            }
            return false;
        }

        bool FindNearParking(GeoCoordinate point)
        {
            bool result = false;
            foreach (Zone park in _places.Parks)
            {
                result = IsInside(new Point(park.Points[0].X, park.Points[0].Y),
                                  new Point(park.Points[1].X, park.Points[1].Y),
                                  new Point(park.Points[2].X, park.Points[2].Y),
                                  new Point(park.Points[3].X, park.Points[3].Y),
                                  new Point(point.Longitude, point.Latitude));
                if (result)
                    return result;
            }
            return result;
        }

        bool FindNearDepot(GeoCoordinate point)
        {
            bool result = false;
            foreach (Zone dept in _places.Depots)
            {
                result = IsInside(new Point(dept.Points[0].X, dept.Points[0].Y),
                                  new Point(dept.Points[1].X, dept.Points[1].Y),
                                  new Point(dept.Points[2].X, dept.Points[2].Y),
                                  new Point(dept.Points[3].X, dept.Points[3].Y),
                                  new Point(point.Longitude, point.Latitude));
                if (result)
                    return result;
            }
            return result;
        }

        public Boolean IsInside(Point v1, Point v2, Point v3, Point v4, Point test)
        {
            bool a, b, c, d;

            a = Area(test, v1, v2) < 0.0 ? true : false;
            b = Area(test, v2, v3) < 0.0 ? true : false;
            c = Area(test, v3, v4) < 0.0 ? true : false;
            d = Area(test, v4, v1) < 0.0 ? true : false;
            return ((a == b) && (a == c) && (a == d));
        }

        public static double Area(Point a, Point b, Point c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y));
        }

        bool SearchTruck(GeoCoordinate coordinate)
        {
            foreach (Dump truck in Dumps.Where(x => x.Tod == TypeOfDump.Dumptruck))
            {
                GeoCoordinate truckcoordinate = truck.Location;
                if (coordinate.GetDistanceTo(truckcoordinate) < LOADINGRADIUS && truck.CurrentState == "LL")
                    return true;
            }
            return false;
        }

        public int SearchTruck(double latitude, double longitude)
        {
            GeoCoordinate coordinate = new GeoCoordinate(latitude, longitude, 0);
            foreach (Dump truck in Dumps.Where(x => x.Tod == TypeOfDump.Dumptruck))
            {
                GeoCoordinate truckcoordinate = truck.Location;
                if (coordinate.GetDistanceTo(truckcoordinate) < LOADINGRADIUS)
                    return truck.Id;
            }
            return -1;
        }

        public int SearchExcavator(double latitude, double longitude)
        {
            GeoCoordinate coordinate = new GeoCoordinate(latitude, longitude, 0);
            foreach (Dump excav in Dumps.Where(x => x.Tod == TypeOfDump.Excavator))
            {
                GeoCoordinate excavcoordinate = excav.Location;
                if (coordinate.GetDistanceTo(excavcoordinate) < LOADINGZONERADIUS)
                    return excav.Id;
            }
            return -1;
        }

        public double SearchNearlyExcavator(int id)
        {
            double mindistance = double.MaxValue;
            GeoCoordinate coordinate = Dumps.Where(x => x.Id == id).Single().Location;
            foreach (Dump excav in Dumps.Where(x => x.Tod == TypeOfDump.Excavator))
            {
                GeoCoordinate excavcoordinate = excav.Location;
                double dist = coordinate.GetDistanceTo(excavcoordinate);
                if (dist < mindistance)
                    mindistance = dist;
            }
            return mindistance;
        }        
    }
}
