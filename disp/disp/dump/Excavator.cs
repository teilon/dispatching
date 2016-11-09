using stateMachine;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class Excavator : Dump
    {
        public Func<GeoCoordinate, bool> SearchTruck;

        public bool IsLoadingPoint;
        public Excavator(string imei)
            : base(imei)
        {
            _tod = TypeOfDump.Excavator;                     
            _state = new DumpStatus(stateMachine.Dump.Excavator);
        }
        public Excavator(string imei, bool isloadingpoint) : this(imei)
        {
            IsLoadingPoint = isloadingpoint;
        }

        
        protected override TypeOfZone EventHandler(DumpMessage msg)
        {
            SetLocation(msg.Location);
            if (SearchTruck(Location)) 
                return TypeOfZone.OnTruckZone;
            return TypeOfZone.None;
        }
        /*
        bool _firstEvent = false;
        DateTime _starttime = default(DateTime);
        protected override TypeOfZone EventHandler(DumpMessage msg)
        {    
                                                                                                         
            SetLocation(msg.Location);
            bool checkSpeed = msg.Location.Speed == 0;

            if (SearchTruck(Location))
            {
                _state.ToLoading();
                if (!_firstEvent && _state.Current == "LL")
                {
                    _starttime = msg.Datetime;
                    dbMethods.saveExcavatorLoading(this.Imei, _starttime);
                    _firstEvent = true;  
                }
            }
            else if (!checkSpeed)
            {
                _state.OnRoad();
                if (_firstEvent)
                {
                    dbMethods.saveEndExcavatorLoading(this.Imei, _starttime, msg.Datetime);      
                    _firstEvent = false;
                }
            }
            else
            {
                _state.Stop();
                if (_firstEvent)
                {
                    dbMethods.saveEndExcavatorLoading(this.Imei, _starttime, msg.Datetime);      
                    _firstEvent = false;
                }
            }

            msg.State = _state.Current;
            
            return msg.State;
        }   
        */
    }
}
