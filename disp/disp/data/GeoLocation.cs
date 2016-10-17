using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    //todo: or Point3
    public class GeoLocation
    {
        private double _latitude = 0;
        private double _longitude = 0;
        private double _altitude = 0;

        public double Latitude { get { return _latitude; } }
        public double Longitude { get { return _longitude; } }
        public double Altitude { get { return _altitude; } }

        public void SetLocation(double lat, double lon, double alt)
        {
            _latitude = lat;
            _longitude = lon;
            _altitude = alt;
        }                  
    }
}
