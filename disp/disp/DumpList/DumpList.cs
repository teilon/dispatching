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
    public partial class DumpList
    {
        public List<Dump> Dumps;
        Places _places;
        LoadingPoint _loadingpoints;                     

        public Dump this[int id]     { get { return Dumps.Where(x => x.Id == id).FirstOrDefault(); } }
        public DumpList(Places places, LoadingPoint loadingpoints)
        {
            Dumps = new List<Dump>();
            _places = places;
            _loadingpoints = loadingpoints;
        }
        
        public bool Contains(int key)
        {
            return Dumps.Where(x => x.Id == key).FirstOrDefault() != null;
        }
        public void AddLoadingPoint(string id, GeoCoordinate point, double loadingradius = 0)
        {
            _loadingpoints.AddLoadingPoint(id, point, loadingradius);
            List<Point> _list = new List<Point>();
            _list.Add(new Point(point.Longitude, point.Latitude));
            _places.Add(id, TypeOfZone.OnLoadingPoint, _list);
        }                                                                    

        public Dump AddDumptruck(int id)
        {
            if (!IsExist(TypeOfDump.Dumptruck, id))
            {   
                Dumps.Add(new Truck(id));
                Truck dump = (Truck)Dumps.Last();
                dump.FindNearLoadingZone = FindNearLoadingZone;
                dump.FindNearExcavator = FindNearExcavator;
                dump.FindNearParking = FindNearParking;
                dump.FindNearDepot = FindNearDepot;
                return dump;
            }
            return Dumps.Where(x => x.Id == id && x.Tod == TypeOfDump.Dumptruck).First();
        }

        public Dump AddExcavator(int id, bool isloadingpoint = false)
        {
            if (!IsExist(TypeOfDump.Excavator, id))
            {
                Dumps.Add(new Excavator(id, isloadingpoint));
                Excavator dump = (Excavator)Dumps.Last();
                dump.SearchTruck = SearchTruck;
                return Dumps.Last();
            }
            return Dumps.Where(x => x.Id == id && x.Tod == TypeOfDump.Excavator).First();
        }   
        bool IsExist(TypeOfDump tod, int id)
        {
            return (Dumps.Where(x => x.Id == id && x.Tod == tod)).FirstOrDefault() != null;
        }

        public bool IsExist(int id)
        {
            return (Dumps.Where(x => x.Id == id)).FirstOrDefault() != null;
        }

        public void RemoveDump(int index)
        {
            Dumps.Remove(Dumps[index]);
        }
    }        
}
