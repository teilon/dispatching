using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace disp
{
    public class DumpList
    {
        const double EXCAVATORRADIUS = 45;

        public List<Dump> Dumps;
        Places _places;
        LoadingPoint _loadingpoints;
        ParkPlaces _parkings;

        public int Length { get { return Dumps.Count; } }

        public Dump this[int index] { get { return Dumps[index]; } }

        public Dump this[string imei] { get { return Dumps.Where(x => x.Imei == imei).FirstOrDefault(); } }

        public DumpList(string fileName, Places places, LoadingPoint loadingpoints, ParkPlaces parkings)
        {
            Dumps = new List<Dump>();
            if (File.Exists(fileName))
                FillDumpList(fileName);
            _places = places;
            _loadingpoints = loadingpoints;
            _parkings = parkings;            
        }   

        void FillDumpList(string fileName)
        {
            List<Item> _items = JSONData.OpenJson(fileName);
            bool IsLoadingPoint = true;
            bool IsNotLoadingPoint = false;
            //Regex regex = new Regex(@".{4}$");
            foreach(Item item in _items)
            {
                switch (item.type)
                {
                    case "t":
                        AddDumptruck(item.name).AddParkNumber(item.park);
                        break;
                    case "e":
                        AddExcavator(item.name, IsNotLoadingPoint).AddParkNumber(item.park);
                        break;
                    case "el":
                        AddExcavator(item.name, IsLoadingPoint).AddParkNumber(item.park);
                        //todo: from db get last place
                        _loadingpoints.AddLoadingPoint(item.name, Dumps.Last().Location);
                        break;
                    default:
                        break;
                }
                
            }
        }

        Dump AddDumptruck(string imei)
        {
            if (!IsExist(TypeOfDump.Dumptruck, imei))
            {   
                Dumps.Add(new Truck(imei));
                Truck dump = (Truck)Dumps.Last();
                dump.FindNearExcavator = FindNearExcavator;
                dump.FindNearParking = FindNearParking;
                dump.FindNearDepot = FindNearDepot;
                return dump;
            }
            return null;                
        }

        Dump AddExcavator(string imei, bool isloadingpoint)
        {
            if (!IsExist(TypeOfDump.Excavator, imei))
            {
                Dumps.Add(new Excavator(imei, isloadingpoint));
                Excavator dump = (Excavator)Dumps.Last();
                dump.SearchTruck = SearchTruck;
                return Dumps.Last();
            }
            return null;                
        }   

        bool IsExist(TypeOfDump tod, string imei)
        {
            return (Dumps.Where(x => x.Imei == imei && x.Tod == tod)).FirstOrDefault() != null;
        }
        public bool IsExist(string imei)
        {   
            return (Dumps.Where(x => x.Imei == imei)).FirstOrDefault() != null;
        }
        public void RemoveDump(int index)
        {
            Dumps.Remove(Dumps[index]);
        }        

        #region appendix hypo    
        double hypo(GeoLocation pointA, GeoLocation pointB)
        {
            double a = Math.Abs(pointA.Longitude - pointB.Longitude);
            double b = Math.Abs(pointA.Latitude - pointB.Latitude);
            return Math.Sqrt(a * a + b * b);
        }
        double hypo(Line line, GeoLocation point)
        {
            double a = Math.Abs(line.Points[0].X - point.Longitude);
            double b = Math.Abs(line.Points[0].Y - point.Latitude);
            return Math.Sqrt(a * a + b * b);
        }
        #endregion

        #region appendix toTXT
        void ToTXT(string s)
        {
            using (FileStream file = new FileStream("02.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter _writer = new StreamWriter(file, Encoding.UTF8))
            {
                _writer.Write("{0}\n", s);
            }
        }
        #endregion

        bool FindNearExcavator(GeoCoordinate coordinate)
        {
            foreach (GeoCoordinate excavatorcoordinate in _loadingpoints.Excavators.Values)
            {
                if (coordinate.GetDistanceTo(excavatorcoordinate) < EXCAVATORRADIUS)
                    return true;
            }
            return false;
        }

        bool FindNearParking(GeoCoordinate point)
        {       
            bool result = false;
            foreach (Line line in _places.Parks)
            {   
                result = IsInside(new Point(line.Points[0].X, line.Points[0].Y), 
                                  new Point(line.Points[1].X, line.Points[1].Y), 
                                  new Point(line.Points[2].X, line.Points[2].Y), 
                                  new Point(line.Points[3].X, line.Points[3].Y), 
                                  new Point(point.Longitude, point.Latitude));
                if (result)
                    return result;
            }
            return result;
        }

        bool FindNearDepot(GeoCoordinate point)
        {                                       
            bool result = false;
            foreach (Line line in _places.Depots)
            {   
                result = IsInside(new Point(line.Points[0].X, line.Points[0].Y), 
                                  new Point(line.Points[1].X, line.Points[1].Y), 
                                  new Point(line.Points[2].X, line.Points[2].Y), 
                                  new Point(line.Points[3].X, line.Points[3].Y), 
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
            foreach(Dump truck in Dumps.Where(x => x.Tod == TypeOfDump.Dumptruck))
            {
                GeoCoordinate truckcoordinate = truck.Location;
                if (coordinate.GetDistanceTo(truckcoordinate) < EXCAVATORRADIUS)
                    return true;    
            }   
            return false;
        }   
    }
    static class JSONData
    {                                                         
        static public List<Item> OpenJson(string fileName)
        {
            List<Item> items = null;
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
            return items;
        }

    }
    public class Item
    {
        public string park;
        public string name;
        public string ipaddres;
        public string type = "";
    }
}
