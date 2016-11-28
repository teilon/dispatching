using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public static class dbMethods
    {
        static string targettable = "dbo.tbTrip";

        public static void saveExcavatorLoading(string imei, DateTime datetime)
        {
            string table = targettable;
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                long check = 20;

                check = con.ExecInsert(table, new
                {
                    transporterDeviceID = imei,
                    eventTypeID = 54,
                    volume_m3 = 130 / 3,
                    weight_kg = 130,
                    startTime = datetime
                });
            }
        }
        public static void saveEndExcavatorLoading(string imei, DateTime startTime, DateTime datetime)
        {
            string table = targettable;
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                con.ExecUpdate(table, new
                {
                    transporterDeviceID = imei,
                    startTime = startTime,
                    endTime = datetime
                });

            }
        }
        public static void saveLoading(string imei, DateTime datetime)
        {
            string table = targettable;
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                long check = 20;               

                check = con.ExecInsert(table, new
                {
                    transporterDeviceID = imei,
                    eventTypeID = 52,
                    volume_m3 = 130 / 3,
                    weight_kg = 130,
                    startTime = datetime
                });
                
                SaveToTXT(string.Format("result: {0}\n", check));
            }
        }
        public static void saveEndLoading(string imei, DateTime startTime, DateTime datetime)
        {
            string table = targettable;
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                con.ExecUpdate(table, new
                {
                    transporterDeviceID = imei,
                    startTime = startTime,
                    endTime = datetime
                });

            }
        }
        public static void saveUnloading(string imei, DateTime datetime)
        {
            string table = targettable;
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                long check = 20;

                check = con.ExecInsert(table, new
                {
                    transporterDeviceID = imei,
                    eventTypeID = 53,
                    volume_m3 = 130 / 3,
                    weight_kg = 130,
                    startTime = datetime
                });

                SaveToTXT(string.Format("result: {0}\n", check));
            }
        }
        public static void saveEndUnloading(string imei, DateTime startTime, DateTime datetime)
        {
            string table = targettable;
            using (IDbConnection con = new SqlConnectionFactory().Create())
            {
                con.ExecUpdate(table, new
                {
                    transporterDeviceID = imei,
                    startTime = startTime,
                    endTime = datetime
                });

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
    }
}
