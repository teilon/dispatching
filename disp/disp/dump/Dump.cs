using System;
using System.Collections.Generic;
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
        protected GeoLocation _location;
        protected TypeOfDump _tod;
        protected string _parkNumber;

        protected ActionLine ActionLine;        
        #endregion

        public string Imei { get { return _imei; } }
        public GeoLocation Location { get { return _location; } }
        public TypeOfDump Tod { get { return _tod; } }
        public string ParkNumber { get { return _parkNumber; } }
        
        protected Dump(string imei)
        {
            _imei = imei;

            _location = new GeoLocation();
            ActionLine = new ActionLine();
        }        

        public void AddParkNumber(string parknumber)
        {
            _parkNumber = parknumber;
        }

        public string AddMessage(DumpMessage msg)
        {              
            return EventHandler(msg);
        }

        public override string ToString()
        {
            return ActionLine.ToString();
        }

        protected abstract string EventHandler(DumpMessage msg);
        //protected abstract void Save(DumpMessage msg);
    }
}
