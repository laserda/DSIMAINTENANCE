using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DSIAppMaintenance.DataBase
{
    public class DBUtils
    {

        public string database { get; set; }



        public  SqlConnection GetDBConnection()
        {
           
            /*
            string datasource = @"DIRECT7";
            this.database = "DSI_BASE_ECOOK_rempl";
            string username = "sa";
            string password = "DIRECT@2017";
            */

            string datasource = @"DIRECT7";
            this.database = "dsi_agromexdk";
            string username = "sa";
            string password = "DIRECT@2017";
            
            //string datasource = @"DIRECT11\DIRECTSOFT";

            //string database = "agrice_web_traca";
            //string username = "sa";
            //string password = "DIRECT@2017";

            return DBSQLUtils.GetDBConnection(datasource, this.database, username, password);
        }
    }
}
