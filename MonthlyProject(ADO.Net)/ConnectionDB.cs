using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MonthlyProject_ADO.Net_
{
    class ConnectionDB
    {
        public ConnectionStringSettings student;//app.config
        public SqlConnection connection1;//database connection
        public SqlCommand cmd1;//command--select,insert............
        public void conn1(string a)
        {
            student = ConfigurationManager.ConnectionStrings["exam"];//app.config
            connection1 = new SqlConnection();//sql database connection
            connection1.ConnectionString = student.ConnectionString;
            cmd1 = connection1.CreateCommand();
            cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = a;
            connection1.Open();
        }
    }

}
