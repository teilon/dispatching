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

        public Truck(string imei)
            :base(imei)
        {
            _tod = TypeOfDump.Dumptruck;
            _state = new DumpStatus(); 
        }


        bool _firstEvent = false;
        bool _event = false; //false-load true-unload
        DateTime _starttime = default(DateTime);
        protected override string EventHandler(DumpMessage msg)
        {
            Location.SetLocation(msg.Location.Latitude, msg.Location.Longitude, msg.Location.Altitude);
            bool checkSpeed = msg.SpeedKPH == 0;         
            if (FindNearExcavator(Location) && checkSpeed)
            {                   
                _state.OnExcavator();
                if (!_firstEvent && _state.Current == "LL")
                {
                    _starttime = msg.Datetime;
                    dbMethods.saveLoading(this.Imei, _starttime);
                    _firstEvent = true;
                    _event = false;
                }
                    
            }
            else if (FindNearParking(Location) && checkSpeed)
            {                                   
                _state.OnParking();        
            }                                      
            else if (FindNearDepot(Location) && checkSpeed)
            {
                _state.OnDepot();
                if (!_firstEvent && _state.Current == "UU")
                {
                    _starttime = msg.Datetime;
                    dbMethods.saveUnloading(this.Imei, _starttime);
                    _firstEvent = true;
                    _event = true;
                }                       
            }
            else
            {                   
                if (!checkSpeed)
                {
                    _state.OnRoad();
                    if (_firstEvent)
                    {
                        if (!_event)
                        {
                            dbMethods.saveEndLoading(this.Imei, _starttime, msg.Datetime);
                        }
                        else
                        {
                            dbMethods.saveEndUnloading(this.Imei, _starttime, msg.Datetime);
                        }
                        _firstEvent = false;
                    }
                }                       
            }
            ActionLine.Add(new DumpAction(msg.Datetime, Location, _state.Current));
            msg.State = _state.Current;
            return msg.State.ToString();
        }         
    }
}
