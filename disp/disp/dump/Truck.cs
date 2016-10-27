using stateMachine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class Truck : Dump
    {                                                    
        public Func<GeoLocation, bool> FindNearExcavator;
        public Func<GeoLocation, bool> FindNearParking;
        public Func<GeoLocation, bool> FindNearDepot;
        protected DumpStatus _state;

        bool _startEvent = false;
        DateTime startTime = default(DateTime);
        TimeSpan ts_load = new TimeSpan(0, 3, 0);
        TimeSpan ts_unload = new TimeSpan(0, 1, 30);

        bool _start;
        int _count;

        bool _firstLoad = true;

        public int Count { get { return _count; } }

        public Truck(string imei)
            :base(imei)
        {
            _tod = TypeOfDump.Dumptruck;
            _state = new DumpStatus();
            //
            _start = false;   
        }

        protected override string EventHandler(DumpMessage msg)
        {
            Location.SetLocation(msg.Location.Latitude, msg.Location.Longitude, msg.Location.Altitude);
            bool checkSpeed = msg.SpeedKPH == 0;
            if (FindNearExcavator(Location) && checkSpeed)
            {                   
                _state.OnExcavator();
                /*
                if (_startEvent)
                    if(msg.Datetime - startTime > ts_load)   //todo: < or >
                    {
                        _state.OnExcavator();
                        _start = true;
                        if (_firstLoad)
                        {
                            //saveLoading(msg.Datetime - ts);
                            _firstLoad = false;                            
                        }
                        //ActionLine.Add(new DumpAction(msg.Datetime, Location, "LL"));
                    }else
                    {
                        _state.OnRoad();//todo: event is sub-load
                    }
                if (!_startEvent)
                {
                    startTime = msg.Datetime;
                    _startEvent = true;
                    _state.OnRoad();//todo: event is sub-load
                }
                */                     
            }                                        
            else if (FindNearParking(Location) && checkSpeed)
            {
                //_state.OnRoad();
                _state.OnParking();
                //ActionLine.Add(new DumpAction(msg.Datetime, Location, "PP"));
            }                                      
            else if (FindNearDepot(Location) && checkSpeed)
            {
                _state.OnDepot();
                /*
                if (_startEvent)
                    if (msg.Datetime - startTime > ts_unload)
                    {
                        _state.OnDepot();
                        //ActionLine.Add(new DumpAction(msg.Datetime, Location, "UU"));
                        if (_start)
                        {
                            _count++;
                            _start = false;
                            //saveUnloading(msg.Datetime);
                        }
                    }else
                    {                                              
                        _state.OnRoad();//todo: event is sub-unload
                    }
                if (!_startEvent)
                {
                    startTime = msg.Datetime;
                    _startEvent = true;
                    _state.OnRoad();//todo: event is sub-unload
                }
                */                     
            }
            else
            {
                //if (_state.Current == "LL")
                //saveEndLoading(startTime, msg.Datetime);
                //if (_state.Current == "UU")
                //saveEndUnloading(startTime, msg.Datetime);
                if (!checkSpeed)    
                    _state.OnRoad();
                //ActionLine.Add(new DumpAction(msg.Datetime, Location, "MM"));
                //_startEvent = false;
                //_firstLoad = true;
            }
            msg.State = _state.Current;
            return msg.State.ToString();
        }


        /*   
               transporterDeviceID = 
               loaderDeviceID          NVARCHAR(32)   NULL,
               depotID                 NVARCHAR(32)   NULL,
               eventTypeID             INT             NULL,
               volume_m3               FLOAT(53)    NULL,
               weight_kg               FLOAT(53)    NULL,
               startTime               DateTime        NOT NULL,
               endTime                 DateTime        NULL
               */
        void saveLoading(DateTime datetime)
        {
            string table = "dbo.Trip";
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                long check = 20;

                check = con.ExecInsert(table, new
                {
                    transporterDeviceID = this.Imei,
                    eventTypeID = 52,
                    volume_m3 = 130 / 3,
                    weight_kg = 130,
                    startTime = datetime
                });
            }
        }
        void saveEndLoading(DateTime startTime, DateTime datetime)
        {
            string table = "dbo.Trip";
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {                   
                con.ExecUpdate(table, new
                {
                    transporterDeviceID = this.Imei,
                    startTime = startTime,
                    endTime = datetime
                });                

            }
        }
        void saveUnloading(DateTime datetime)
        {
            string table = "dbo.Trip"; 
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                long check = 20;

                check = con.ExecInsert(table, new
                {
                    transporterDeviceID = this.Imei,
                    eventTypeID = 53,
                    volume_m3 = 130 / 3,
                    weight_kg = 130,
                    startTime = datetime
                });
            }
        }
        void saveEndUnloading(DateTime startTime, DateTime datetime)
        {
            string table = "dbo.Trip";
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                con.ExecUpdate(table, new
                {
                    transporterDeviceID = this.Imei,
                    startTime = startTime,
                    endTime = datetime
                });

            }
        }
    }
}
