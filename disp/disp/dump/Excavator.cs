using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class Excavator : Dump
    {
        public bool IsDepot;
        public Excavator(string imei)
            : base(imei)
        {
            _tod = TypeOfDump.Excavator;
        }
        public Excavator(string imei, bool isdepot)
            : this(imei)
        {
            IsDepot = isdepot;
        }
        protected override string EventHandler(DumpMessage msg)
        {
            Location.SetLocation(msg.Location.Latitude, msg.Location.Longitude, msg.Location.Altitude);
            msg.State = "NN";
            return "NN";
        }                                              
    }
}
