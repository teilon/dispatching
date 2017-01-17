using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace disp
{
    public partial class Enterprise
    {
        public DumpList Dumps;
        public Places Places;
        public LoadingPoint LoadingPoints;         

        public Enterprise()
        {
            Places = new Places();
            LoadingPoints = new LoadingPoint();
            Dumps = new DumpList(Places, LoadingPoints);
        }         
        
        public string AddMessage(int id, double latitude, double longitude, DateTime datetime, int speedKPH)
        {
            DumpMessage msg = new DumpMessage(id, latitude, longitude, datetime, speedKPH);
            TypeOfZone toz;
            TypeOfDump tod;
            bool spd = speedKPH > SPEEDSTOPLIMIT;

            if (Dumps.IsExist(msg.Id))
            {
                Dump dump = Dumps[msg.Id];
                tod = dump.Tod;
                if (tod == TypeOfDump.Excavator)
                    Dumps.AddLoadingPoint(msg.Id.ToString(), msg.Location);
                toz = dump.AddMessage(msg);
                return DetermineState(dump, tod, toz, spd);
            }
            return "NH";
        }

        public usedExcavator ToucheExcavator(string truckimei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {
            int excav = -1;
            excav = Dumps.SearchExcavator(latitude, longitude);
            if (excav == -1)
                return null;

            TypeOfZone toz = TypeOfZone.OnTruckZone;
            TypeOfDump tod = TypeOfDump.Excavator;
            Dump dump = Dumps[excav];
            bool spd = false;                         
            string newState = DetermineState(dump, tod, toz, spd);

            return new usedExcavator() { id = excav, newstate = newState };
        }   
    }            

    public class DumpMessage
    {
        public string Imei;
        public int Id;
        public GeoCoordinate Location;
        public DateTime Datetime;
        public string State; 
        public DumpMessage(int id, double latitude, double longitude, DateTime datetime, int speedKPH)
        {
            Id = id;
            Location = new GeoCoordinate(latitude, longitude, 0, 0, 0, (double)speedKPH, 0);
            Datetime = datetime;
            State = "";
        }
        public DumpMessage(string imei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {
            Imei = imei;
            Location = new GeoCoordinate(latitude, longitude, 0, 0, 0, (double)speedKPH, 0);
            Datetime = datetime;
            State = "";
        }
    }   
    public class usedExcavator
    {
        public int id;    
        public string newstate;
    }                    
}
