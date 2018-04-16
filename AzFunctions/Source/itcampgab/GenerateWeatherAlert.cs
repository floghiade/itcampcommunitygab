using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using ITCamp.Gab.Core;
using Newtonsoft.Json;
using System;

namespace itcampgab
{
    public static class GenerateWeatherAlert
    {
        /// <example>
        /// https://itcampgab.azurewebsites.net/api/GenerateWeatherAlert
        /// </example>
        [FunctionName("GenerateWeatherAlert")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            WeatherAlertService alertService = new WeatherAlertService();
            WeatherAlert alert = alertService.GetWeatherAlert();
            string jsonWeatherAlert = JsonConvert.SerializeObject(alert);

            log.Info($"GenerateWeatherAlert END {alert.ToString()}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonWeatherAlert);

            return response;
        }
    }
}
