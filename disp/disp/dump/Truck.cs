using stateMachine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class Truck : Dump
    {                                                    
        public Func<GeoCoordinate, bool> FindNearExcavator;
        public Func<GeoCoordinate, bool> FindNearParking;
        public Func<GeoCoordinate, bool> FindNearDepot;
        //public Func<GeoCoordinate, bool> IntersectGeozone;
        //protected DumpStatus _state;

        public Truck(string imei)
            :base(imei)
        {
            _tod = TypeOfDump.Dumptruck;
            _state = new DumpStatus(stateMachine.Dump.Truck);
        }
        
        bool _firstEvent = false;
        bool _event = false; //false-load true-unload
        DateTime _starttime = default(DateTime);
        protected override string EventHandler(DumpMessage msg)
        {
            SetLocation(msg.Location);
            bool checkSpeed = msg.Location.Speed == 0;        

            if (FindNearExcavator(Location) && (checkSpeed || _state.Current == "LL"))
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
            else if (FindNearParking(Location) && (checkSpeed || _state.Current == "PP"))
            {                              
                _state.OnParking();        
            }                                      
            else if (FindNearDepot(Location) && (checkSpeed || _state.Current == "UU"))
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
            msg.State = _state.Current;
            return msg.State;
        }         
    }
}
