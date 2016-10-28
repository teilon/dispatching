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

        public Dictionary<string, Point> Excavators { get { return _places; } }

        public ExcavatorPlaces()
        {             
            _places = new Dictionary<string, Point>();                 
        }

        public void AddExcavatorPlace(string imei, Point point)
        {
            if (_places.ContainsKey(imei))
                _places[imei] = point;
            else
                _places.Add(imei, point);     
        }
    }
    
    public class ExcavatorPlace
    {
        public string Imei;
        public List<Point> Points;
    }

}
