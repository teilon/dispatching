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
        const int SPEEDSTOPLIMIT = 2;

        delegate bool Condition(TypeOfDump tod, TypeOfZone toz, bool spd);
        Dictionary<Condition, Func<Dump, string>> Conditions;


        public DumpList Dumps;
        public Places Places;
        public LoadingPoint LoadingPoints;

        
        //Dictionary<string, DateTime> MSDevice;

        public Enterprise()
        {
            Places = new Places();
            LoadingPoints = new LoadingPoint();
            Dumps = new DumpList(Places, LoadingPoints);
        }

        public Enterprise(string fileName)
        {
            Places = new Places();
            LoadingPoints = new LoadingPoint();                      
            Dumps = new DumpList(fileName, Places, LoadingPoints);
            //Conditions = GetConditions();

            //temp
            //MSDevice = new Dictionary<string, DateTime>();
        }
                

        Dictionary<Condition, Func<Dump, string>> GetConditions()
        {
            Dictionary<Condition, Func<Dump, string>> _conditions = new Dictionary<Condition, Func<Dump, string>>();
            _conditions.Add(checkLoad, OnLoad);
            _conditions.Add(checkUnload, OnUnload);
            _conditions.Add(checkShiftChange, OnShiftChange);
            _conditions.Add(checkOutage, OnOutage);
            _conditions.Add(checkMove, OnMove);
            return _conditions;
        }

        /*
        public void AddOldPlacesOfExcavator(string imei, double latitude, double longitude, double altitude)
        {
            FillLoadZone(imei, new GeoCoordinate(latitude, longitude, altitude));
        }
        */

        public string AddMessage(string imei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {   
            DumpMessage msg = new DumpMessage(imei, latitude, longitude, datetime, speedKPH);
            TypeOfZone toz;
            TypeOfDump tod;
            bool spd = speedKPH > SPEEDSTOPLIMIT;

            if (Dumps.IsExist(msg.Imei))
            {         
                Dump dump = Dumps[msg.Imei];                
                tod = dump.Tod;
                if (tod == TypeOfDump.Excavator)
                    Dumps.AddLoadingPoint(msg.Imei, msg.Location);
                toz = dump.AddMessage(msg);

                string oldState = dump.State.Current;
                string newState = DetermineState(dump, tod, toz, spd);

                //if(tod == TypeOfDump.Dumptruck)
                //    SaveNewEvent(oldState, newState, imei, datetime);
                return newState;
            }                    
            return "NH";
        }

        public usedExcavator ToucheExcavator(string truckimei, double latitude, double longitude, DateTime datetime, int speedKPH)
        {
            string excav = Dumps.SearchExcavator(latitude, longitude);
            if (excav == "")
                return null;

            TypeOfZone toz = TypeOfZone.OnTruckZone;
            TypeOfDump tod = TypeOfDump.Excavator;
            Dump dump = Dumps[excav];
            bool spd = false;
            //string oldState = dump.State.Current;
            string newState = DetermineState(dump, tod, toz, spd);

            return new usedExcavator() { imei = excav, newstate = newState };
        }

        public int GetCurrentZone(string imei, double latitude, double longitude)
        {
            return Dumps.GetCurrentZone(imei, latitude, longitude);
        }

        /*
        void FillLoadZone(string imei, GeoCoordinate location)
        {
            if (
                imei == "354868053063915" ||
                imei == "354868056852009" ||
                imei == "354868054433877" ||
                imei == "354868056817085" ||
                imei == "354868053058196" ||
                imei == "354868053063956" ||
                imei == "354868052961648"
                )
            {
                Dumps.AddLoadingPoint(imei, location);
            }
        }
        */
        /*
        void AddToMSDevice(string imei, DateTime datetime)
        {
            if (!MSDevice.ContainsKey(imei))
                MSDevice.Add(imei, datetime);
            else
                MSDevice[imei] = datetime;    
        }
        */
        /*
        void SaveNewEvent(string stold, string stnew, string imei, DateTime datetime)
        {
            SaveToTXT(string.Format("result: {0}\n", "in Current"));
            if (IsFirstLoad(stold, stnew))
            {
                SaveToTXT(string.Format("result: {0}\n", "IsFirstLoad"));
                dbMethods.saveLoading(imei, datetime);
                SaveToTXT(string.Format("result: {0}\n", "IsFirstLoad+"));
                AddToMSDevice(imei, datetime);
                return;
            }
            if (IsLastLoad(stold, stnew))
            {
                SaveToTXT(string.Format("result: {0}\n", "IsLastLoad"));
                dbMethods.saveEndLoading(imei, MSDevice[imei], datetime);
                SaveToTXT(string.Format("result: {0}\n", "IsLastLoad+"));
                return;
            }
            if (IsFirstUnload(stold, stnew))
            {
                SaveToTXT(string.Format("result: {0}\n", "IsLastLoad"));
                dbMethods.saveUnloading(imei, datetime);
                SaveToTXT(string.Format("result: {0}\n", "IsLastLoad+"));
                AddToMSDevice(imei, datetime);
                return;
            }
            if (IsLastUnload(stold, stnew))
            {
                SaveToTXT(string.Format("result: {0}\n", "IsLastUnload"));
                dbMethods.saveEndLoading(imei, MSDevice[imei], datetime);
                SaveToTXT(string.Format("result: {0}\n", "IsLastUnload+"));
                return;
            }
        }        

        public static void SaveToTXT(string input)
        {
            string fileName = string.Format(@"C:\mok\log_{0}.txt", DateTime.Now.Year + DateTime.Now.DayOfYear);
            using (FileStream file = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter _writer = new StreamWriter(file, Encoding.UTF8))
            {
                _writer.Write(input);
            }
        }
        */

        //getstate
        string DetermineState(Dump dump, TypeOfDump tod, TypeOfZone toz, bool spd)
        {
            string curstate = dump.State.Current;
            string state = dump.State.Current;
            if (checkLoadZone(tod, toz, false) && curstate == state)
                state = OnLoadingZone(dump);
            if (checkLoad(tod, toz, (spd || dump.State.Current == "LL")) && curstate == state)
                state = OnLoad(dump);
            if (checkUnload(tod, toz, (spd || dump.State.Current == "UU")) && curstate == state)
                state = OnUnload(dump);
            if (checkShiftChange(tod, toz, (spd || dump.State.Current == "PP")) && curstate == state)
                state = OnShiftChange(dump);
            if (checkOutage(tod, toz, spd) && curstate == state)
                state = OnOutage(dump);
            if (checkMove(tod, toz, spd) && curstate == state)
                state = OnMove(dump);
            return state;
        }
        //set state
        string OnLoadingZone(Dump dump)
        {
            dump.State.InTheLoadingZone();
            return dump.State.Current;
        }
        string OnLoad(Dump dump)
        {
            dump.State.OnLoad();
            return dump.State.Current;
        }
        string OnUnload(Dump dump)
        {
            dump.State.OnDepot();
            return dump.State.Current;
        }
        string OnShiftChange(Dump dump)
        {
            dump.State.OnParking();
            return dump.State.Current;
        }
        string OnMove(Dump dump)
        {
            dump.State.OnRoad();
            return dump.State.Current;
        }
        string OnOutage(Dump dump)
        {
            dump.State.Stop();
            return dump.State.Current;
        }
        //check zone
        bool checkLoadZone(TypeOfDump tod, TypeOfZone toz, bool spd)
        {
            return (tod == TypeOfDump.Dumptruck && (toz == TypeOfZone.OnLoadingZone || toz == TypeOfZone.OnLoadingOrStorageZone))
                //|| (tod == TypeOfDump.Excavator && toz == TypeOfZone.OnTruckZone)
                ;
        }
        bool checkLoad(TypeOfDump tod, TypeOfZone toz, bool spd)
        {
            return (tod == TypeOfDump.Dumptruck && toz == TypeOfZone.OnLoadingPoint && !spd) ||
                (tod == TypeOfDump.Excavator && toz == TypeOfZone.OnTruckZone);
        }
        bool checkUnload(TypeOfDump tod, TypeOfZone toz, bool spd)                         
        {
            return (tod == TypeOfDump.Dumptruck && (toz == TypeOfZone.OnStoragePoint || toz == TypeOfZone.OnLoadingOrStorageZone) && !spd);
        }
        bool checkShiftChange(TypeOfDump tod, TypeOfZone toz, bool spd)
        {
            return (tod == TypeOfDump.Dumptruck && toz == TypeOfZone.OnShiftChangePoint && !spd) ||
                (tod == TypeOfDump.Dumptruck && toz == TypeOfZone.OnGarage && !spd);
        }
        bool checkOutage(TypeOfDump tod, TypeOfZone toz, bool spd)
        {
            return (tod == TypeOfDump.Excavator && toz == TypeOfZone.None && !spd)
                || (tod == TypeOfDump.Dumptruck && toz == TypeOfZone.None && !spd);
        }
        bool checkMove(TypeOfDump tod, TypeOfZone toz, bool spd)
        {
            return (tod == TypeOfDump.Excavator && toz == TypeOfZone.None && spd)
                || (tod == TypeOfDump.Dumptruck && toz == TypeOfZone.None && spd);                
        }
        // events
        bool IsFirstLoad(string stold, string stnew)
        {
            return (stold != stnew) && stnew == "LL";
        }
        bool IsLastLoad(string stold, string stnew)
        {
            return (stold != stnew) && stold == "LL";
        }
        bool IsFirstUnload(string stold, string stnew)
        {
            return (stold != stnew) && stnew == "UU";
        }
        bool IsLastUnload(string stold, string stnew)
        {
            return (stold != stnew) && stold == "UU";
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
    public class usedExcavator
    {
        public string imei;    
        public string newstate;
    }                    
}
