using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Data
{
    public class WeatherDataItem
    {
        public string CityName { get; set; }
        public int MinT { get; set; }
        public int MaxT { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
