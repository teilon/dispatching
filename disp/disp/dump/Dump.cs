using stateMachine;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace disp
{
    public abstract class Dump
    {
        #region fields
        protected string _imei;
        protected string _parknumber;
        protected GeoCoordinate _location;
        protected TypeOfDump _tod;           
        protected DumpStatus _state;
        #endregion

        public string Imei { get { return _imei; } }
        public string ParkNumber { get { return _parknumber; } }
        public GeoCoordinate Location { get { return _location; } }
        public TypeOfDump Tod { get { return _tod; } }          
        public DumpStatus State { get { return _state; } }

        protected Dump(string imei)
        {
            _imei = imei;           
            _location = new GeoCoordinate();
            SetLocation(0, 0);
        }

        public void SetLocation(GeoCoordinate coordinate)
        {
            _location = coordinate;
        }

        /// <summary>
        /// Set current location
        /// </summary>
        /// <param name="latitude">Gets or sets the latitude of the GeoCoordinate.</param>
        /// <param name="longitude">Gets or sets the longitude of the GeoCoordinate.</param>
        /// <param name="altitude">Gets the altitude of the GeoCoordinate, in meters.</param>
        /// <param name="speed">Gets or sets the speed in meters per second.</param>
        /// <param name="course">Gets or sets the heading in degrees, relative to true north.</param>
        public void SetLocation(double latitude, double longitude, double altitude = 0, double speed = 0, double course = 0)
        {
            _location.Latitude = latitude;
            _location.Longitude = longitude;
            _location.Altitude = altitude;
            _location.Speed = speed;
            _location.Course = course;
        }
        public void AddParkNumber(string parknumber)
        {
            _parknumber = parknumber;
        }

        public TypeOfZone AddMessage(DumpMessage msg)
        {              
            return EventHandler(msg);
        }    

        protected abstract TypeOfZone EventHandler(DumpMessage msg);
    }
}
