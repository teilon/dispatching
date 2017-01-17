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
        Dictionary<int, GeoCoordinate> _excavators;
        Dictionary<int, PointParam> _loadingpoints;

        public Dictionary<int, GeoCoordinate> Excavators { get { return _excavators; } }
        public Dictionary<int, PointParam> LoadingPoints { get { return _loadingpoints; } }


        public LoadingPoint()
        {             
            _excavators = new Dictionary<int, GeoCoordinate>();
            _loadingpoints = new Dictionary<int, PointParam>();
        }                   

        public void AddLoadingPoint(string id, GeoCoordinate point, double loadRadius)
        {
            int _id = int.Parse(id);
            if (_excavators.ContainsKey(_id)) _excavators[_id] = point;
            else _excavators.Add(_id, point);
            if (_loadingpoints.ContainsKey(_id))
            {
                _loadingpoints[_id].Coordinate = point;
                if(loadRadius != 0)
                    _loadingpoints[_id].LoadingRadius = loadRadius;
            }
            else
            {
                _loadingpoints.Add(_id, new PointParam() { Coordinate = point, LoadingRadius = loadRadius });
            }                   
        }
        public void UpdateLoadingPoint(int id, GeoCoordinate point)
        {
            if (_excavators.ContainsKey(id))
                _excavators[id] = point;
        }
    }     
    public class PointParam
    {
        public GeoCoordinate Coordinate;
        public double LoadingRadius;
        public double LoadingZoneRadius = 50;
    }

}
