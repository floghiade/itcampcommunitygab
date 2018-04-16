using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using StackExchange.Redis;
using Newtonsoft.Json;
using ITCamp.Gab.Core;

namespace itcampgab
{
    public static class CleanWeatherAlertList
    {
        const string RedisCacheConnectionString =
           @"itecampweatheralertcache.redis.cache.windows.net:6380,password=SOoBczU3jvACinqKk3/Hi8grBdC3fdHDeUffr6FPKD0=,ssl=True,abortConnect=False";
        const string RedisCacheDBName = "Weather";

        [FunctionName("CleanWeatherAlertList")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            CleanList(log, "RO");
            CleanList(log, "BG");
        }

        private static void CleanList(TraceWriter log, string countryCode)
        {
            IDatabase cacheDb = GetCacheDb();
            for (int i = 0; i <= cacheDb.ListLength(countryCode); i++)
            {
                RedisValue cacheItem = cacheDb.ListGetByIndex(countryCode, i);
                if (cacheItem.IsNullOrEmpty)
                {
                    log.Info($"CleanWeatherAlertList Item is NULL");
                    continue;
                }
                WeatherAlert item = JsonConvert.DeserializeObject<WeatherAlert>(cacheItem);
                if (DateTime.UtcNow > item.ValidFrom.Add(item.Period))
                {
                    cacheDb.ListRemove(countryCode, cacheItem);
                    log.Info($"CleanWeatherAlertList Remove: {item}");
                }
            }
        }

        private static IDatabase GetCacheDb()
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisCacheConnectionString);
            IDatabase cacheDb = connection.GetDatabase();
            return cacheDb;
        }
    }
}
