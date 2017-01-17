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
        List<Zone> _list = new List<Zone>();
        public List<Zone> Depots { get { return _list.Where(x => x.Type == TypeOfZone.OnStoragePoint).ToList(); } }
        public List<Zone> Parks { get { return _list.Where(x => x.Type == TypeOfZone.OnShiftChangePoint).ToList(); } }
        public List<Zone> Deposits { get { return _list.Where(x => x.Type == TypeOfZone.OnLoadingPoint).ToList(); } }

        public Places()
        { 
            
        }
        public void Add(string id, TypeOfZone type, List<Point> points)
        {
            bool havenot = true;
            foreach(Zone z in _list)
                if(z.Id == id)
                    havenot = false;
            if(havenot)
                _list.Add(new Zone(id, type, points));
        }
        public void AddPoint(string id, Point point)
        {
            bool have = false;
            foreach (Zone z in _list)
                if (z.Id == id)
                    have = true;
            if (have)
                _list.Where(x => x.Id == id).Single().Points.Add(point);
        }
    }
    
    public class Zone
    {
        public string Id;
        public TypeOfZone Type;
        public List<Point> Points;
        public int Index;
        static int Count = 0;
        public Zone(string id, TypeOfZone toz, List<Point> list)
        {
            Index = Count;
            Id = id;
            Type = toz;
            Points = list;
            Count++;
        }
    }       
}
