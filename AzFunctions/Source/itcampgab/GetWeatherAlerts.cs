using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using StackExchange.Redis;
using Newtonsoft.Json;
using ITCamp.Gab.Core;
using Microsoft.ServiceBus.Messaging;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace itcampgab
{
    public static class GetWeatherAlerts
    {
        const string RedisCacheConnectionString = 
            @"__REDIS_CONNECTION_STRING,password=__REDIS_PW__,ssl=True,abortConnect=False";
        const string RedisCacheDBName = "Weather";

        const string SeviceBusConnectionString =
            @"Endpoint=sb://__SB_ENDPOINT__;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=__SB_SAS_KEY__";
        const string TopicName = "livealert";

        [FunctionName("GetWeatherAlerts")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            WeatherAlert weatherAlert = GetWeatherAlert();

            AddToCache(weatherAlert);
            SendMessageToServiceBus(weatherAlert);
            //LogItemList(log, weatherAlert.CountryCode);

            log.Info($"GetWeatherAlerts END: {weatherAlert.ToString()}");
        }

        private static WeatherAlert GetWeatherAlert()
        {
            using (var client = new HttpClient(new HttpClientHandler()))
            {
                client.BaseAddress = new Uri(@"__FUNCTION_URL__");
                HttpResponseMessage response = client.GetAsync(@"api/GenerateWeatherAlert").Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                WeatherAlert alert = JsonConvert.DeserializeObject<WeatherAlert>(result);
                return alert;
            }
        }

        private static void SendMessageToServiceBus(WeatherAlert weatherAlert)
        {
            TopicClient topicClient = TopicClient.CreateFromConnectionString(SeviceBusConnectionString, TopicName);
            string jsonWeatherAlert = JsonConvert.SerializeObject(weatherAlert);
            BrokeredMessage message = new BrokeredMessage(jsonWeatherAlert);
            message.Properties.Add("countrycode", weatherAlert.CountryCode);
            topicClient.Send(message);

        }

        private static void AddToCache(WeatherAlert weatherAlert)
        {
            string jsonWeatherAlert = JsonConvert.SerializeObject(weatherAlert);
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisCacheConnectionString);
            IDatabase cacheDb = connection.GetDatabase();

            cacheDb.ListRightPush(weatherAlert.CountryCode, jsonWeatherAlert);

            connection.Close();
        }

        private static void LogItemList(TraceWriter log, string countryCode)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisCacheConnectionString);
            IDatabase cacheDb = connection.GetDatabase();
            for (int i = 0; i <= cacheDb.ListLength(countryCode); i++)
            {
                RedisValue cacheItem = cacheDb.ListGetByIndex(countryCode, i);
                if (cacheItem.IsNullOrEmpty)
                {
                    log.Info($"GetWeatherAlerts Item is NULL");
                    continue;
                }
                WeatherAlert item = JsonConvert.DeserializeObject<WeatherAlert>(cacheItem);
                log.Info($"GetWeatherAlerts Item from list: {item}");
            }

            connection.Close();
        }
    }
}
