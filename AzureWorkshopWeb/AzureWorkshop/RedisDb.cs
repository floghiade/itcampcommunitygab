﻿using ITCamp.Gab.Core;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWorkshop
{
    // TODO: don't forget the fix for Redis
    public class RedisDb
    {
        const string RedisCacheConnectionString =
           @"__REDIS_CONNECTION_STRING,password=__REDIS_PW__,ssl=True,abortConnect=False";
        const string RedisCacheDBName = "Weather";

        public IEnumerable<WeatherAlert> GetFromCache(string countryCode)
        {
            IDatabase cacheDb = GetCacheDb();
            //get a maximum of 100
            //TODO: switch to putting in the max value for list?
            var  results = cacheDb.ListRange(countryCode, 0, 100);

            foreach (var alert in results)
            {
                 yield return JsonConvert.DeserializeObject<WeatherAlert>(alert);
            }
        }

        private IDatabase GetCacheDb()
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisCacheConnectionString);
            IDatabase cacheDb = connection.GetDatabase();
            return cacheDb;
        }
    }
}
