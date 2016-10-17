using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class DepotPlaces
    {
        List<Line> _depots;
        public List<Line> Depots { get { return _depots; } }
                
        public DepotPlaces()
        { 
            string fileName = @"C:\mok\depots.json";
            if (!File.Exists(fileName))
                return;                   
            List<DepotPlace> list = JSONDepot.OpenJson(fileName);
                  
            _depots = new List<Line>();
            foreach (DepotPlace tm in list)
            {
                Line line = new Line();
                foreach (Point p in tm.Points)
                {
                    GEOCoordinate coord = new GEOCoordinate(p.Y, p.X);
                    coord.TransferToUTM();
                    line.Points.Add(new Point(coord.X, coord.Y));
                }
                _depots.Add(line);
            }
        }
    }

    public static class JSONDepot
    {
        public static List<DepotPlace> OpenJsonString(string input)
        {                     
            return JsonConvert.DeserializeObject<List<DepotPlace>>(input);            
        }
        public static List<DepotPlace> OpenJson(string fileName)
        {
            List<DepotPlace> items = null;
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                //Console.WriteLine(json);
                items = JsonConvert.DeserializeObject<List<DepotPlace>>(json);
            }
            return items;
        }
    }
    public class DepotPlace
    {
        public string Imei;
        public string Type;
        public List<Point> Points;
    }
}
