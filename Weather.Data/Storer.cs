using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Data
{
    public abstract class Storer
    {
        private static string _connectionString;

        static Storer()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Weather"].ConnectionString;
        }

        internal static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(_connectionString);     
            conn.Open();
            return conn;
        }
    }
}
