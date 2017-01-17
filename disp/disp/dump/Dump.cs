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
        protected int _id;
        protected string _parknumber;
        protected GeoCoordinate _location;
        protected TypeOfDump _tod;           
        protected DumpStatus _state;
        #endregion

        public int Id { get { return _id; } }
        public string ParkNumber { get { return _parknumber; } }
        public GeoCoordinate Location { get { return _location; } }
        public TypeOfDump Tod { get { return _tod; } }          
        public DumpStatus State { get { return _state; } }
        public string CurrentState { get { return _state.Current; } }

        protected Dump(int id)
        {
            _id = id;     
            _location = new GeoCoordinate();
            SetLocation(0, 0);
        }

        public void SetLocation(GeoCoordinate coordinate)
        {
            _location = coordinate;
        }
                
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

        public bool IsDumptruck()
        {
            return _tod == TypeOfDump.Dumptruck;
        }

        public bool IsExcavator()
        {
            return _tod == TypeOfDump.Excavator;
        }
    }
}
