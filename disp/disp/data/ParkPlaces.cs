using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class ParkPlaces
    {
        Dictionary<string, Point> _places;

        List<Line> _parkings;
        public List<Line> Parkings { get { return _parkings; } }
        public ParkPlaces()
        {
            string fileName = @"C:/mok/Parking.json";
            if (!File.Exists(fileName))
                return;
            _places = new Dictionary<string, Point>();
            List<ParkPlace> list = JSONPark.OpenJson(fileName);
            _parkings = new List<Line>();
            foreach (ParkPlace tm in list)
            {
                Line line = new Line();
                foreach (Point p in tm.Points)
                {
                    //GEOCoordinate coord = new GEOCoordinate(p.Y, p.X);
                    //coord.TransferToUTM();
                    line.Points.Add(new Point(p.X, p.Y));
                }
                _parkings.Add(line);
            }
        }
    }

    public static class JSONPark
    {
        static public List<ParkPlace> OpenJson(string fileName)
        {
            List<ParkPlace> items = null;
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                //Console.WriteLine(json);
                items = JsonConvert.DeserializeObject<List<ParkPlace>>(json);    //The type initializer for 'disp.Program' threw an exception
            }
            return items;
        }
    }
    public class ParkPlace
    {
        public string Imei;
        public string Type;
        public List<Point> Points;
    }
}
