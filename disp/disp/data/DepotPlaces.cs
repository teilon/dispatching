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
        //List<Line> _places;
        //public List<Line> Depots { get { return _places.Where(x => x.Type == "dept").ToList(); } }
        //public List<Line> Parks { get { return _places.Where(x => x.Type == "park").ToList(); } }

        List<Zone> _list = new List<Zone>();
        public List<Zone> Depots { get { return _list.Where(x => x.Type == TypeOfZone.OnStoragePoint).ToList(); } }
        public List<Zone> Parks { get { return _list.Where(x => x.Type == TypeOfZone.OnShiftChangePoint).ToList(); } }
        public List<Zone> Deposits { get { return _list.Where(x => x.Type == TypeOfZone.OnLoadingPoint).ToList(); } }

        public Places()
        { 
            /*
            string fileName = @"C:\mok\depots.json";
            if (!File.Exists(fileName))
                return;
            List<JSONZone> _jsonlist = JSONPlaces.OpenJson(fileName);

            foreach(JSONZone z in _jsonlist)
            {
                TypeOfZone toz = (z.Type == "dept") ? TypeOfZone.OnStoragePoint : (z.Type == "park") ? TypeOfZone.OnShiftChangePoint : TypeOfZone.None;                
                _list.Add(new Zone(z.Imei, toz, z.Points));
            }
            */
        }
        public void Add(string imei, TypeOfZone type, List<Point> points)
        {
            bool havenot = true;
            foreach(Zone z in _list)
                if(z.Imei == imei)
                    havenot = false;
            if(havenot)
                _list.Add(new Zone(imei, type, points));
        }
        public void AddPoint(string imei, Point point)
        {
            bool have = false;
            foreach (Zone z in _list)
                if (z.Imei == imei)
                    have = true;
            if (have)
                _list.Where(x => x.Imei == imei).Single().Points.Add(point);
        }
    }
    
    public class Zone
    {
        public string Imei;
        public TypeOfZone Type;
        public List<Point> Points;
        public int Index;
        static int Count = 0;
        public Zone(string imei, TypeOfZone toz, List<Point> list)
        {
            Index = Count;
            Imei = imei;
            Type = toz;
            Points = list;
            Count++;
        }
    }

    internal static class JSONPlaces
    {
        public static List<JSONZone> OpenJsonString(string input)
        {
            return JsonConvert.DeserializeObject<List<JSONZone>>(input);
        }
        public static List<JSONZone> OpenJson(string fileName)
        {
            List<JSONZone> items = null;
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                //Console.WriteLine(json);
                items = JsonConvert.DeserializeObject<List<JSONZone>>(json);
            }
            return items;
        }
    }
    internal class JSONZone
    {
        public string Imei;
        public string Type;
        public List<Point> Points;
    }
}
