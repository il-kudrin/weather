using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Data
{
    public class WeaterDataItemStorer : Storer
    {
        

        public static void UpdateRecords(List<WeatherDataItem> items)
        {
            using (var conn = GetConnection())
            {
                var storer = new MySqlWeaterDataItemStorer(conn);
                storer.UpdateRecords(items);
            }
        }

        public static string[] GetCitiesForDate(DateTime date)
        {
            using (var conn = GetConnection())
            {
                var storer = new MySqlWeaterDataItemStorer(conn);
                return storer.GetCitiesForDate(date);
            }
        }

        public static WeatherDataItem GetWeather(string city, DateTime date)
        {
            using (var conn = GetConnection())
            {
                var storer = new MySqlWeaterDataItemStorer(conn);
                return storer.GetWeather(city, date);
            }
        }
    }
}
