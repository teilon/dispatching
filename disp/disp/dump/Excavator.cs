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
        public Excavator(int id)
            : base(id)
        {
            _tod = TypeOfDump.Excavator;                     
            _state = new DumpStatus(stateMachine.Dump.Excavator);
        }
        public Excavator(int id, bool isloadingpoint) 
            : this(id)
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
    }
}
