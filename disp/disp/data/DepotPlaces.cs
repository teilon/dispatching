using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class Places
    {
        List<Line> _places;
        public List<Line> Depots { get { return _places.Where(x => x.Type == "dept").ToList(); } }
        public List<Line> Parks { get { return _places.Where(x => x.Type == "park").ToList(); } }

        public Places()
        { 
            string fileName = @"C:\mok\depots.json";
            if (!File.Exists(fileName))
                return;                   
            List<DepotPlace> list = JSONDepot.OpenJson(fileName);
                  
            _places = new List<Line>();
            foreach (DepotPlace tm in list)
            {
                Line line = new Line();
                //if(tm.Type == "dept")
                {
                    foreach (Point p in tm.Points)
                    {
                        //GEOCoordinate coord = new GEOCoordinate(p.Y, p.X);
                        //coord.TransferToUTM();
                        line.Points.Add(new Point(p.X, p.Y));                        
                    }
                    line.Name = tm.Imei;
                    line.Type = tm.Type;
                    _places.Add(line);
                }                
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
