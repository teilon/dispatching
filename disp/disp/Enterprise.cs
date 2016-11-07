using Newtonsoft.Json;
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
    public class Enterprise
    {
        public DumpList Dumps;
        public Places Places;
        public LoadingPoint LoadingPoints;                           

        public Enterprise()
        {                                 
            Dumps = new DumpList("park.json", null, null);
        }
        public Enterprise(string fileName)
        {
            Places = new Places();
            LoadingPoints = new LoadingPoint();                      
            Dumps = new DumpList(fileName, Places, LoadingPoints);            
        }

        public string AddMessage(string imei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {   
            DumpMessage msg = new DumpMessage(imei, latitude, longitude, datetime, speedKPH);                                           
            if (Dumps.IsExist(msg.Imei))                 
                return Dumps[msg.Imei].AddMessage(msg);
            return "NH";
        }
    }            

    public class DumpMessage
    {
        public string Imei;
        public GeoCoordinate Location;
        public DateTime Datetime;
        public string State; 
        public DumpMessage(string imei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {
            Imei = imei;
            Location = new GeoCoordinate(latitude, longitude, 0, 0, 0, (double)speedKPH, 0);
            Datetime = datetime;
            State = "";
        }       
    }                       
}
