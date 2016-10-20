using disp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeLine;

namespace disp
{
    class TempExcavatorPlaces
    {
        Dictionary<string, BaseGraph.Point> _places;
        public TempExcavatorPlaces()
        {
            _places = new Dictionary<string, BaseGraph.Point>();
        }
        public void Add(string imei, BaseGraph.Point point)
        {
            if (_places.ContainsKey(imei))
                _places[imei] = point;
            else
                _places.Add(imei, point);
        }
        public void PrintToFile(string fileName)    //@"excv.txt"
        {
            Regex regex = new Regex(@",");           
            using (FileStream file = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter _writer = new StreamWriter(file, Encoding.UTF8))
            {
                _writer.Write("[");
                foreach (var item in _places)
                {
                    _writer.Write("{");
                    _writer.Write("Imei:\"{0}\",Points:[", item.Key);
                    _writer.Write("{");
                    _writer.Write("X:{0},Y:{1}", regex.Replace(item.Value.X.ToString(), "."), regex.Replace(item.Value.Y.ToString(), "."));
                    _writer.Write("},]},\n");
                }
                    
                _writer.Write("]");
            }
        }
    }

    class Program
    {
        static disp.Enterprise ent = new disp.Enterprise("park.json");
        static void Main(string[] args)
        {
            Temp();

            //delay
            Console.WriteLine("end");
            Console.ReadLine();
        }

        static void Temp()
        {
            SourceData sd = new SourceData();
            //List<Tag> list = sd.GetTags(@"G:\bt\Logs\log_2016.09.14\all.2016-09-06T10.19.25.9098904+06.00.dmp");
            //List<Board> _boards = sd.GetBoards(@"G:\bt\Logs\log_2016.09.14\all.2016-09-06T10.19.25.9098904+06.00.dmp");
            List<Pack> list = sd.GetTimeLine(@"D:\Logs\log_2016.09.21[+data]\all.2016-09-16T20.48.09.4036915+06.00.dmp",
                                            Convert.ToDateTime("10.09.2016 19:30:00"),
                                            Convert.ToDateTime("17.09.2016 19:30:00"));

            //int i = 0;
            //int n = 0;

            Regex regex = new Regex(@".{4}$");
            string _imei = "";

            
            TempExcavatorPlaces tep = new TempExcavatorPlaces();
            foreach (Pack pack in list)
            {
                //pack.Parse();
                _imei = regex.Match(pack.msg.Imei).Value;
                Dump dump = ent.Dumps[_imei];

                if(dump != null)
                    if(dump.Tod == TypeOfDump.Excavator)
                    {
                        /*
                        ent.AddMessage(JsonConvert.SerializeObject(new DumpMessage()
                        {
                            imei = _imei,
                            latitude = pack.msg.Latitude,
                            longitude = pack.msg.Longitude,
                            altitude = pack.msg.Altitude,
                            datetime = pack.msg.TimeCreated
                        }));
                        */
                        tep.Add(_imei, new BaseGraph.Point(dump.Location.Longitude, dump.Location.Latitude));
                    }                
            }
            tep.PrintToFile(@"excv0.json");
            

            foreach (Pack pack in list)
            {
                //pack.Parse();
                _imei = regex.Match(pack.msg.Imei).Value;
                Dump dump = ent.Dumps[_imei];

                if (dump != null)
                    if (dump.Tod == TypeOfDump.Dumptruck)
                    {

                        ent.AddMessage(JsonConvert.SerializeObject(new DumpMessage()
                        {
                            Imei = pack.msg.Imei,
                            Location = new Location()
                            {
                                Latitude = Utils.getLatitudeInGradus(pack.msg.Latitude),
                                Longitude = Utils.getLongitudeInGradus(pack.msg.Longitude),
                                Altitude = pack.msg.Altitude
                            },
                            State = "",
                            Datetime = pack.msg.TimeCreated
                        }));

                    }
            }
            PrintA(list);
            PrintB();
            PrintC();
        }

        static void PrintA(List<Pack> list)
        {
            string _imei = "";
            Regex regex = new Regex(@".{4}$");
            using (FileStream file = new FileStream("line.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter _writer = new StreamWriter(file, Encoding.UTF8))
            {
                foreach (Pack pack in list)
                {
                    //pack.Parse();
                    _imei = regex.Match(pack.msg.Imei).Value;
                    _writer.Write("{0}\t{1}\t{2}    {3}\n", _imei, pack.msg.TimeCreated, pack.msg.Longitude, pack.msg.Latitude);
                }

            }
        }
        static void PrintB()
        {
            string _imei = "";
            Regex regex = new Regex(@".{4}$");
            using (FileStream file = new FileStream("counter.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter _writer = new StreamWriter(file, Encoding.UTF8))
            {
                foreach (Dump dump in ent.Dumps.Dumps)
                {
                    //pack.Parse();
                    if(dump.Tod == TypeOfDump.Dumptruck)
                    {
                        Truck truck = (Truck)dump;
                        _imei = regex.Match(truck.Imei).Value;
                        _writer.Write("{0} [{1}]\t{2}\n", _imei, truck.ParkNumber, truck.Count);
                    }                    
                }

            }
        }
        static void PrintC()
        {
            string _imei = "";
            Regex regex = new Regex(@".{4}$");
            using (FileStream file = new FileStream("actionlines.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter _writer = new StreamWriter(file, Encoding.UTF8))
            {
                foreach (Dump dump in ent.Dumps.Dumps)
                {
                    _imei = regex.Match(dump.Imei).Value;
                    _writer.Write("***\t{0} [{1}]\t***\n", _imei, dump.ParkNumber);
                    //pack.Parse();
                    if (dump.Tod == TypeOfDump.Dumptruck)
                    {
                        Truck truck = (Truck)dump;                      
                        _writer.Write("{0}\n", truck.ToString());
                    }
                }

            }
        }
    }

}