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

        public string AddMessage(string imei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {
            DumpMessage msg = new DumpMessage()
            {
                Imei = imei,
                Location = new GeoCoordinate(latitude, longitude, 0, 0, 0, (double)speedKPH, 0),                
                Datetime = datetime,
                State = "",         
            };

            /*
            TransferCoordinate _tc = new TransferCoordinate(msg.Location.Latitude, msg.Location.Longitude);
            msg.Location.Latitude = _tc.X;
            msg.Location.Longitude = _tc.Y;
            */

            //string _imei = new Regex(@".{4}$").Match(msg.Imei).Value;
            string _imei = msg.Imei;                                                
            if (Dumps.IsExist(_imei))//todo: PARK CHANGE
            {
                if (Dumps[_imei].Tod == TypeOfDump.Excavator)
                    if (_imei == "354868053063915" ||         //134
                        _imei == "354868056852009" ||         //157
                        _imei == "354868054433877" ||         //125
                        _imei == "354868053058196" ||         //13
                        _imei == "354868056817085" ||         //70
                        _imei == "354868052961648" ||         //39
                        _imei == "354868053063956")           //66
                        ExcavatorPlaces.AddExcavatorPlace(_imei, msg.Location);
                string currentstate = Dumps[_imei].AddMessage(msg);
                /*
                if (Dumps[_imei].Tod == TypeOfDump.Dumptruck && currentstate == "LL")
                {                                                                  
                    foreach(Dump excv in Dumps.Dumps.Where(x => x.Tod == TypeOfDump.Excavator))
                    {
                        AddMessage(excv.Imei, excv.Location.Latitude, excv.Location.Longitude, msg.Datetime);
                    }
                }
                */
                return currentstate;
            }

            return "NH";
        }

        #region appendix
        /*
        string GetImeiFromMessage(string message)
        {
            string pattern = @"((?<=Imei:)\w{8})";
            Regex reg = new Regex(pattern);
            return reg.Match(message).Value;
        }

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
                        ExcavatorPlaces.AddExcavatorPlace(_imei, new GeoCoordinate(msg.Location.Latitude, msg.Location.Longitude));
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
        public class DumpMessageTemp
        {
            public string Imei;
            public Location Location;
            public long Datetime;
            public string State;
        }
        */
        #endregion

    }            

    public class DumpMessage
    {
        public string Imei;
        public GeoCoordinate Location;
        public DateTime Datetime;
        public string State;        
    }                       
}
