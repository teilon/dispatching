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
        public Func<GeoCoordinate, bool> FindNearLoadingZone;
        public Func<GeoCoordinate, bool> FindNearParking;
        public Func<GeoCoordinate, bool> FindNearDepot;       

        public Truck(int id)
            :base(id)
        {
            _tod = TypeOfDump.Dumptruck;
            _state = new DumpStatus(stateMachine.Dump.Truck);
        }

        protected override TypeOfZone EventHandler(DumpMessage msg)
        {
            SetLocation(msg.Location);
            TypeOfZone result = TypeOfZone.None;

            if (FindNearLoadingZone(Location))
            {
                if (result == TypeOfZone.OnStoragePoint)
                    result = TypeOfZone.OnLoadingOrStorageZone;
                else
                    result = TypeOfZone.OnLoadingZone;
            }
            if (FindNearExcavator(Location))
            {
                if (result == TypeOfZone.OnLoadingZone)
                    result = TypeOfZone.OnLoadingPoint;                
            }
            if (FindNearDepot(Location))
            {
                if (result == TypeOfZone.OnLoadingPoint)
                    result = TypeOfZone.OnLoadingOrStorageZone;
                else
                    result = TypeOfZone.OnStoragePoint;
            }
            if (FindNearParking(Location))
            {
                result = TypeOfZone.OnShiftChangePoint;
            }
            return result;   
        }    
    }
}
