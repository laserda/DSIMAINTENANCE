using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DSIAppMaintenance.DataBase
{
    public class DBSQLUtils
    {
        public static SqlConnection  GetDBConnection(string datasource, string database, string username, string password)
        {
            // Connection String.
            var connString = @"Data Source=" + datasource + ";Initial Catalog="  + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }
    }
}
