﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    class dbMethods
    {
        void saveLoading(string imei, DateTime datetime)
        {
            string table = "dbo.Trip";
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
            }
        }
        void saveEndLoading(string imei, DateTime startTime, DateTime datetime)
        {
            string table = "dbo.Trip";
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
        void saveUnloading(string imei, DateTime datetime)
        {
            string table = "dbo.Trip";
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
            }
        }
        void saveEndUnloading(string imei, DateTime startTime, DateTime datetime)
        {
            string table = "dbo.Trip";
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
    }
}