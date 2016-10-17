using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public DepotPlaces DepotPlaces;
        public ExcavatorPlaces ExcavatorPlaces;
        public ParkPlaces ParkPlaces;

        public Enterprise()
        {                                 
            Dumps = new DumpList("park.json", null, null, null);
        }
        public Enterprise(string fileName)
        {
            DepotPlaces = new DepotPlaces();
            ExcavatorPlaces = new ExcavatorPlaces();
            ParkPlaces = new ParkPlaces();
            Dumps = new DumpList(fileName, DepotPlaces, ExcavatorPlaces, ParkPlaces); 
        }

        string GetImeiFromMessage(string message)
        {                                                                 
            string pattern = @"((?<=Imei:)\w{8})";              
            Regex reg = new Regex(pattern);                     
            return reg.Match(message).Value;                       
        }

        public string AddMessage(string message)        
        {
            DumpMessageTemp _msg = JsonConvert.DeserializeObject<DumpMessageTemp>(message);

            DumpMessage msg = DumpMessageFabric(_msg);


            TransferCoordinate _tc = new TransferCoordinate(msg.Location.Latitude, msg.Location.Longitude);
            msg.Location.Latitude = _tc.X;
            msg.Location.Longitude = _tc.Y;

            string imei = new Regex(@".{4}$").Match(msg.Imei).Value;     
            //string imei = msg.Imei;                                                
            if (Dumps.IsExist(imei))                                                     //todo: PARK CHANGE
                return Dumps[imei].AddMessage(msg);
            return "NH";
        }

        DumpMessage DumpMessageFabric(DumpMessageTemp tmp)
        {
            return new DumpMessage()
            {
                Imei = tmp.Imei,
                Location = tmp.Location,
                Datetime = new DateTime(tmp.Datetime),
                State = tmp.State
            };
        }
    }

    public class DumpMessageTemp
    {
        public string Imei;
        public Location Location;
        public long Datetime;
        public string State;
    }
    public class DumpMessage
    {
        public string Imei;
        public Location Location;
        public DateTime Datetime;
        public string State;        
    }   
    public class Location
    {
        public double Longitude;
        public double Latitude;
        public double Altitude;
    }
}
