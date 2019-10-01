using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Weather.Data;

namespace WcfWeatherService
{
   
    public class Service : IService
    {
        public TomorrowCitiesRespinse GetCitiesForTomowow()
        {
            try
            {
                var cities = WeaterDataItemStorer.GetCitiesForDate(DateTime.UtcNow.Date.AddDays(1));
                return new TomorrowCitiesRespinse()
                {
                    Cities = cities
                };
            }
            catch (Exception ex)
            {
                return new TomorrowCitiesRespinse()
                {
                    ErrorDescription = $"Server error. Message:{ex.Message} Trace: {ex.StackTrace}"
                };
            }
             
        }

        public GetWeaterResponse GetWeather(GetWeaterRequest request)
        {
            var response = new GetWeaterResponse();
            if (string.IsNullOrEmpty(request.CityName))
            {
                response.ErrorDescription = $"Bad request: No CityName in request.";
                return response;
            }
            try
            {
                var date = DateTime.UtcNow.Date.AddDays(1);
                var weather = WeaterDataItemStorer.GetWeather(request.CityName, date);
                if (weather == null)
                {
                    response.ErrorDescription = $"No weather for {request.CityName} on selected date {date.ToString("dd-MM-yyyy")}";
                    return response;
                }
                response.CityName = weather.CityName;
                response.Date = date;
                response.Description = weather.Description;
                response.MinT = weather.MinT;
                response.MaxT = weather.MaxT;
                return response;
            }
            catch(Exception ex)
            {
                response.ErrorDescription = $"Server error. Message:{ex.Message} Trace: {ex.StackTrace}";
                return response;
            }
        }
    }
}
