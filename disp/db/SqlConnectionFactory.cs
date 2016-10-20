using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace disp
{
    public class SqlConnectionFactory
    {
        public static string ConnectionString { get; set; }

        public IDbConnection Create()
        {
            //string conStr = @"Data Source=10.8.0.4;Initial Catalog=docflow3;Persist Security Info=True;User ID=sa;Password=123123qwE";
            string conStr = @"Data Source=192.168.0.101;Initial Catalog=docflow3;Persist Security Info=True;User ID=sa;Password=@qwerty123";


            var dbConnection = new SqlConnection(conStr);
            dbConnection.Open();
            return dbConnection;
        }
    }
}