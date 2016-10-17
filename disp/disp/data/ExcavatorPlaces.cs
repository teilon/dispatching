using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class ExcavatorPlaces
    {
        Dictionary<string, Point> _places;

        List<Line> _excavators;
        public List<Line> Excavators { get { return _excavators; } }
        public ExcavatorPlaces()
        {
            string fileName = @"C:/mok/excv.json";
            if (!File.Exists(fileName))
                return;
            _places = new Dictionary<string, Point>();
            List<ExcavatorPlace> list = JSONExcavator.OpenJson(fileName);
            _excavators = new List<Line>();
            foreach (ExcavatorPlace tm in list)
            {
                Line line = new Line();
                foreach (Point p in tm.Points)
                {
                    GEOCoordinate coord = new GEOCoordinate(ConvertCoordinate.getLatitudeInGradus(p.Y), ConvertCoordinate.getLongitudeInGradus(p.X));
                    coord.TransferToUTM();
                    line.Points.Add(new Point(coord.X, coord.Y));
                }
                _excavators.Add(line);
            }
        }
    }

    public static class JSONExcavator
    {
        static public List<ExcavatorPlace> OpenJson(string fileName)
        {
            List<ExcavatorPlace> items = null;
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                //Console.WriteLine(json);
                items = JsonConvert.DeserializeObject<List<ExcavatorPlace>>(json);    //The type initializer for 'disp.Program' threw an exception
            }
            return items;
        }
    }
    public class ExcavatorPlace
    {
        public string Imei;
        public List<Point> Points;
    }

}
