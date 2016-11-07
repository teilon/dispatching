using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class LoadingPoint
    {
        Dictionary<string, GeoCoordinate> _loadingpoints;

        public Dictionary<string, GeoCoordinate> Excavators { get { return _loadingpoints; } }

        public LoadingPoint()
        {             
            _loadingpoints = new Dictionary<string, GeoCoordinate>();                 
        }                   

        public void AddLoadingPoint(string imei, GeoCoordinate point)
        {
            if (_loadingpoints.ContainsKey(imei))
                _loadingpoints[imei] = point;
            else
                _loadingpoints.Add(imei, point);
        }
    }     

}
