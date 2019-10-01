using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfWeatherService
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        TomorrowCitiesRespinse GetCitiesForTomowow();

        [OperationContract]
        GetWeaterResponse GetWeather(GetWeaterRequest request);
    }
    [DataContract]
    public class GetWeaterRequest
    {
        [DataMember]
        public string CityName { get; set; }
    }

    [DataContract]
    public class GetWeaterResponse : BaseResponse
    {
        [DataMember]
        public string CityName { get; set; }
        [DataMember]
        public int MinT { get; set; }
        [DataMember]
        public int MaxT { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
    }

    public class TomorrowCitiesRespinse : BaseResponse
    {
        [DataMember]
        public string[] Cities { get; set; }
    }

    [DataContract]
    public abstract class BaseResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public string ErrorDescription { get; set; }
    }
   
}
