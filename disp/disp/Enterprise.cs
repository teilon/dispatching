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

        public string AddMessage(string imei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {                                                
            DumpMessage msg = new DumpMessage()
            {
                Imei = imei,
                Location = new Location()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = 0
                },
                Datetime = datetime,//DateTime.Now,
                State = "",
                SpeedKPH = speedKPH
            };

            TransferCoordinate _tc = new TransferCoordinate(msg.Location.Latitude, msg.Location.Longitude);
            msg.Location.Latitude = _tc.X;
            msg.Location.Longitude = _tc.Y;

            string _imei = new Regex(@".{4}$").Match(msg.Imei).Value;
            //string imei = msg.Imei;                                                
            if (Dumps.IsExist(_imei))//todo: PARK CHANGE
            {
                if (Dumps[_imei].Tod == TypeOfDump.Excavator)
                    if (_imei == "3915" || _imei == "2009" || _imei == "3877" || _imei == "3107" || _imei == "6330" || _imei == "1648" || _imei == "7085")
                        ExcavatorPlaces.AddExcavatorPlace(_imei, new Point(msg.Location.Latitude, msg.Location.Longitude));
                return Dumps[_imei].AddMessage(msg);
            }

            return "NH";
        }

        #region appendix
        public string AddMessage(string imei, double latitude, double longitude, DateTime datetime)
        {                                                
            DumpMessage msg = new DumpMessage()
            {
                Imei = imei,
                Location = new Location()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = 0
                },
                Datetime = datetime,//DateTime.Now,
                State = "",
                SpeedKPH = 0
            };

            TransferCoordinate _tc = new TransferCoordinate(msg.Location.Latitude, msg.Location.Longitude);
            msg.Location.Latitude = _tc.X;
            msg.Location.Longitude = _tc.Y;

            string _imei = new Regex(@".{4}$").Match(msg.Imei).Value;
            //string imei = msg.Imei;                                                
            if (Dumps.IsExist(_imei))//todo: PARK CHANGE
            {
                if (Dumps[_imei].Tod == TypeOfDump.Excavator)
                    if(_imei == "3915" || _imei == "2009" || _imei == "3877" || _imei == "3107" || _imei == "6330")
                        ExcavatorPlaces.AddExcavatorPlace(_imei, new Point(msg.Location.Latitude, msg.Location.Longitude));
                return Dumps[_imei].AddMessage(msg);
            }
                
            return "NH";
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

        #endregion

        public void SaveMessage(string imei, string state, DateTime datetime)
        {
            //todo: save to db
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
        public int SpeedKPH;        
    }   
    public class Location
    {
        public double Longitude;
        public double Latitude;
        public double Altitude;
    }
}
