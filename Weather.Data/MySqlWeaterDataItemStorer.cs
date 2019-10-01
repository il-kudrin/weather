using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Weather.Data
{
    public class MySqlWeaterDataItemStorer 
    {
        MySqlConnection _connection;

        public MySqlWeaterDataItemStorer(DbConnection connection)
        {
            if (connection is MySqlConnection)
                _connection = connection as MySqlConnection;
            else
                throw new Exception("connection is not MySql");
        }


        public void UpdateRecords(List<WeatherDataItem> items)
        {
            if (items == null || items.Count == 0)
                return;
            StringBuilder queryBuilder = new StringBuilder("INSERT INTO WeatherItems (CityName, Date, Discription, MinT, MaxT) VALUES ");

            List<string> Rows = new List<string>();
            foreach(var item in items)
            {
                Rows.Add(string.Format("('{0}','{1}','{2}',{3},{4})",
                    MySqlHelper.EscapeString(item.CityName.Substring(0,Math.Min(100, item.CityName.Length))),
                    MySqlHelper.EscapeString(item.Date.ToString("yyyy-MM-dd")),
                    MySqlHelper.EscapeString(item.Description.Substring(0, Math.Min(500, item.Description.Length))),
                    item.MinT,
                    item.MaxT
                    ));
            }
            queryBuilder.Append(string.Join(",", Rows));
            queryBuilder.Append("ON DUPLICATE KEY UPDATE  Discription = VALUES(Discription), MinT = VALUES(MinT), MaxT = VALUES(MaxT)");
            using (var tran = _connection.BeginTransaction())
            {

                using (var cmd = new MySqlCommand(queryBuilder.ToString(),_connection,tran))
                {
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
            }
        }

        public string[] GetCitiesForDate(DateTime date)
        {
            var res = new List<string>();
            var query = "SELECT CityName FROM WeatherItems WHERE Date = @date";
            using (var cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@date", date.Date);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        res.Add(rd.GetString(0));
                    }
                }
            }
            return res.ToArray(); 
        }

        public WeatherDataItem GetWeather(string city, DateTime date)
        {
            var query = "SELECT  CityName, Date, Discription, MinT, MaxT FROM WeatherItems WHERE Date = @date AND CityName = @cityName";
            using (var cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@date", date.Date);
                cmd.Parameters.AddWithValue("@cityName", city);
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new WeatherDataItem()
                        {
                            CityName = rd.GetString(0),
                            Date = rd.GetDateTime(1),
                            Description = rd.GetString(2),
                            MinT = rd.GetInt32(3),
                            MaxT = rd.GetInt32(4)
                        };
                    }
                }
            }
            return null;
        }

    }
}
