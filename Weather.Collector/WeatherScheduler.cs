using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weather.Collector.Contracts;
using Weather.Data;

namespace Weather.Collector
{
    internal class WeatherScheduler
    {
        IWeatherParser _parser;
        ILogger _logger;
        public WeatherScheduler(IWeatherParser parser, ILogger logger)
        {
            _parser = parser;
            _logger = logger;
            _nextRun = DateTime.UtcNow;
        }

        private DateTime _nextRun;

        public async Task Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Invoke(DateTime.UtcNow);
                await Task.Delay(500);
            }
            return;
        }

        private async Task Invoke(DateTime time)
        {
            if (time < _nextRun)
                return;
            try
            {
                _logger.Log("Start collecting iteration");
                var wetherItems = await _parser.GetWeatherDataItemsAsync();
                WeaterDataItemStorer.UpdateRecords(wetherItems);
                _logger.Log($"wether data collected. {wetherItems.Count} records collected.");
            }
            catch (Exception ex)
            {
                _logger.Log($"Fail to collect weater. Will try again in 10 min. Exception: {ex.ToString()}");
            }
            finally
            {
                _nextRun = DateTime.UtcNow.AddMinutes(1);
            }
            

        }
        
        
    }
}
