using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Data;

namespace Weather.Collector.Contracts
{
    internal interface IWeatherParser
    {
        Task<List<WeatherDataItem>> GetWeatherDataItemsAsync();
    }
}
