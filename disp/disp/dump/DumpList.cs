using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace disp
{
    public class DumpList
    {
        public List<Dump> Dumps;
        DepotPlaces _depots;
        ExcavatorPlaces _excavators;
        ParkPlaces _parkings;

        public int Length { get { return Dumps.Count; } }

        public Dump this[int index]
        {
            get
            {
                return Dumps[index];
            }
        }

        public Dump this[string imei]
        {
            get
            {
                foreach(Dump d in Dumps)
                {
                    if(d.Imei == imei)
                        return d;
                }
                return null;
            }
        }   

        public DumpList(string fileName, DepotPlaces depots, ExcavatorPlaces excavators, ParkPlaces parkings)
        {               
            Dumps = new List<Dump>();
            if(File.Exists(fileName))
                FillDumpList(fileName);
            _depots = depots;
            _excavators = excavators;
            _parkings = parkings;
        }

        void FillDumpList(string fileName)
        {
            List<Item> _items = JSONData.OpenJson(fileName);
            bool IsDepot = true;
            bool IsNotDepot = false;
            //Regex regex = new Regex(@".{4}$");
            foreach(Item item in _items)
            {
                switch (item.type)
                {
                    case "t":
                        AddDumptruck(item.name).AddParkNumber(item.park);
                        break;
                    case "e":
                        AddExcavator(item.name, IsNotDepot).AddParkNumber(item.park);
                        break;
                    case "ed":
                        AddExcavator(item.name, IsDepot).AddParkNumber(item.park);
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

        Dump AddExcavator(string imei, bool isdepot)
        {
            if (!IsExist(TypeOfDump.Excavator, imei))
            {
                Dumps.Add(new Excavator(imei, isdepot));
                return Dumps.Last();
            }
            return null;                
        }   

        bool IsExist(TypeOfDump tod, string imei)
        {
            foreach (Dump d in Dumps)
                if (d.Tod == tod && d.Imei == imei)
                    return true;
            return false;
        }
        public bool IsExist(string imei)
        {
            foreach (Dump d in Dumps)
                if (d.Imei == imei)
                    return true;
            return false;
        }
        public void RemoveDump(int index)
        {
            Dumps.Remove(Dumps[index]);
        }

        //     
        
        bool FindNearExcavator(GeoLocation point)
        {
            //double radius = 0.079162;
            double radius = 20;            
            bool result = false;
            foreach (Line line in _excavators.Excavators)
            {                   
                if (Geometry.Intersection(new Point(line.Points[0].Y, line.Points[0].X), new Point(point.Longitude, point.Latitude), radius))
                {
                    result = true;
                }
            }                
            return result;
        }
        double hypo(Line line, GeoLocation point)
        {
            double a = Math.Abs(line.Points[0].X - point.Longitude);
            double b = Math.Abs(line.Points[0].Y - point.Latitude);
            return Math.Sqrt(a * a + b * b);
        }
        void ToTXT(string s)
        {
            using (FileStream file = new FileStream("02.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter _writer = new StreamWriter(file, Encoding.UTF8))
            {
                _writer.Write("{0}\n", s);
            }
        }

        bool FindNearParking(GeoLocation point)
        {            
            //double radius = 0.029162;
            double radius = 20;
            foreach (Line line in _parkings.Parkings)
            {
                if (Geometry.Intersection(new Point(line.Points[0].Y, line.Points[0].X), new Point(point.Longitude, point.Latitude), radius))
                {
                    return true;
                }
            }
            return false;
        }

        bool FindNearDepot(GeoLocation point)
        {
            //double radius = 0.079162;
            double radius = 20;
            bool result = false;
            foreach (Line line in _depots.Depots)
            {
                if (Geometry.Intersection(line, new Point(point.Latitude, point.Longitude), radius))
                {
                    result = true;
                }
            }
            return result;
        }
        class DepotRound
        {
            double Latitude;
            double Longitude;
            double Altitude;
            double Radius;
            public DepotRound(double latitude, double longitude, double altitude, double radius)
            {
                Latitude = latitude;
                Longitude = longitude;
                Altitude = altitude;
                Radius = radius;
            }
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
