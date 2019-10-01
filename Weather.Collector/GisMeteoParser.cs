using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Collector.Contracts;
using Weather.Data;
using AngleSharp;

namespace Weather.Collector
{
    internal class GisMeteoParser : IWeatherParser
    {
        public GisMeteoParser(IHtmlCollector collector, ILogger logger)
        {
            _collector = collector;
            _logger = logger;

        }

        private readonly IHtmlCollector _collector;
        private readonly ILogger _logger;

        private const string _gismetioMainUrl = "https://www.gismeteo.ru";
        private const string _tenDaysUrl = "10-days/";

        private async Task<List<City>> GetCitiesAsync()
        {
            var res = new List<City>();
           
            var document = await GetDocumentByUrlAsync(_gismetioMainUrl);
            if (document == null)
            {
                _logger.Log("Will try again later");
                return res;
            }
            var cityItems = document.All.FirstOrDefault(x => x.Id == "noscript")?.Children;
            if(cityItems == null)
            {
                _logger.Log("Fail to find any city to collect, maybe gismetio website has changed");
                return res;
            }
            foreach(var item in cityItems)
            {
                if (item is AngleSharp.Html.Dom.IHtmlAnchorElement element)
                {
                    var name = element.Dataset["name"];
                    var url = element.Dataset["url"];
                    if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                    {
                        res.Add(
                            new City {
                                Name = name,
                                Url = _gismetioMainUrl + url + _tenDaysUrl
                            }
                        );
                    }
                }
                
            }
            return res;
        }

        public async Task<List<WeatherDataItem>> GetWeatherDataItemsAsync()
        {
            var res = new List<WeatherDataItem>();
            var cities = await GetCitiesAsync();
            foreach (var city in cities)
            {
                var records = await GetWeatherForCity(city);
                res.AddRange(records);
            }
            
            return res;

        }

        private async Task<List<WeatherDataItem>> GetWeatherForCity(City city)
        {
            var res = new List<WeatherDataItem>();
            var document = await GetDocumentByUrlAsync(city.Url);
            var descriptionSection = document.All.FirstOrDefault(x => x.ClassList.Contains("widget__row") && x.ClassList.Contains("widget__row_table") && x.ClassList.Contains("widget__row_icon"))?.Children;
            if (descriptionSection?.Count() != 10)
            {
                _logger.Log($"Fail to collect data for {city.Name}. description section count missmatch.");
                return res;
            }
            var descriptions = new List<string>();
            foreach (var item in descriptionSection)
            {
                var description = item.Children.FirstOrDefault()?.Children?.FirstOrDefault()as AngleSharp.Html.Dom.IHtmlSpanElement;
                if (description == null)
                {
                    _logger.Log($"Fail to collect data for {city.Name}. description section is not a Span.");
                    return res;
                }
                try
                {
                    descriptions.Add(description.Dataset["text"]);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Fail to collect data for {city.Name}. description section has no data. Exception: {ex.ToString()}");
                    return res;
                }
            }
            var tempSection = document.All
                .FirstOrDefault(x => x.ClassList.Contains("widget__row")
                    && x.ClassList.Contains("widget__row_table")
                    && x.ClassList.Contains("widget__row_temperature"))?
                 .Children?.FirstOrDefault()?
                 .Children?.FirstOrDefault()?
                 .Children.FirstOrDefault(x=>x.ClassName == "values")?
                 .Children;
            if (tempSection?.Count() != 10)
            {
                _logger.Log($"Fail to collect data for {city.Name}. maxTemp section count missmatch.");
                return res;
            }
            var maxTemps = new List<int>();
            var minTemps = new List<int>();
            foreach (var item in tempSection)
            {
                var maxTemp = item.Children?.FirstOrDefault(x=> x.ClassName == "maxt")?
                    .Children?.FirstOrDefault(x => x.ClassName == "unit unit_temperature_c") as AngleSharp.Html.Dom.IHtmlSpanElement;
                var minTemp = item.Children?.FirstOrDefault(x => x.ClassName == "mint")?
                    .Children?.FirstOrDefault(x => x.ClassName == "unit unit_temperature_c") as AngleSharp.Html.Dom.IHtmlSpanElement;
                if (maxTemp == null || minTemp == null)
                {
                    _logger.Log($"Fail to collect data for {city.Name}. description section is not a Span.");
                    return res;
                }
                if(Int32.TryParse(maxTemp.InnerHtml.Replace('−', '-'), out var maxTempInt) && Int32.TryParse(minTemp.InnerHtml.Replace('−','-'), out var minTempInt))
                { 
                    maxTemps.Add(maxTempInt);
                    minTemps.Add(minTempInt);
                }
                else
                {
                    _logger.Log($"Fail to collect data for {city.Name}. temp section has bad data.");
                    return res;
                }
            }
            var date = DateTime.UtcNow.Date;
            for (int i = 0; i < 10; i++)
            {
                res.Add(
                    new WeatherDataItem()
                    {
                        CityName = city.Name,
                        Description = descriptions[i],
                        MaxT = maxTemps[i],
                        MinT = minTemps[i],
                        Date = date
                    }
                );
                date = date.AddDays(1);
            }
            return res;
        }

        private async Task<AngleSharp.Dom.IDocument> GetDocumentByUrlAsync(string url)
        {
            var html = await _collector.GetHtmlAsync(url);
            if (html == null)
            {
                _logger.Log($"Fail get html fron url: {url}");
                return null;
            }
            var angleSharpConfig = Configuration.Default;
            var angleSharpContext = BrowsingContext.New(angleSharpConfig);
            try
            {
                var document = await angleSharpContext.OpenAsync(req => req.Content(html));
                if (document == null)
                {
                    _logger.Log($"Fail to parse html, maybe nonhtml content. Url: {url}");
                    return null;
                }
                return document;
            }
            catch
            {
                _logger.Log($"Fail to parse html, maybe nonhtml content. Url: {url}");
                return null;
            }
        }

    }
}
