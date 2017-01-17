using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public partial class Enterprise
    {
        const int SPEEDSTOPLIMIT = 2;

        delegate bool Condition(TypeOfDump tod, TypeOfZone toz, bool spd);
        Dictionary<Condition, Func<Dump, string>> Conditions;

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
}
