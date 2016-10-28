using stateMachine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class Truck : Dump
    {                                                    
        public Func<GeoLocation, bool> FindNearExcavator;
        public Func<GeoLocation, bool> FindNearParking;
        public Func<GeoLocation, bool> FindNearDepot;
        //protected DumpStatus _state;

        bool _startEvent = false;
        DateTime startTime = default(DateTime);
        TimeSpan ts_load = new TimeSpan(0, 3, 0);
        TimeSpan ts_unload = new TimeSpan(0, 1, 30);

        bool _start;
        int _count;

        bool _firstLoad = true;

        public int Count { get { return _count; } }

        public Truck(string imei)
            :base(imei)
        {
            _tod = TypeOfDump.Dumptruck;
            _state = new DumpStatus();
            //
            _start = false;   
        }

        protected override string EventHandler(DumpMessage msg)
        {
            Location.SetLocation(msg.Location.Latitude, msg.Location.Longitude, msg.Location.Altitude);
            bool checkSpeed = msg.SpeedKPH == 0;
            if (FindNearExcavator(Location) && checkSpeed)
            {                   
                _state.OnExcavator();
                                   
            }
            else if (FindNearParking(Location) && checkSpeed)
            {                                   
                _state.OnParking();        
            }                                      
            else if (FindNearDepot(Location) && checkSpeed)
            {
                _state.OnDepot();                       
            }
            else
            {    
                if (!checkSpeed)    
                    _state.OnRoad();  
            }
            ActionLine.Add(new DumpAction(msg.Datetime, Location, _state.Current));
            msg.State = _state.Current;
            return msg.State.ToString();
        }         
    }
}
