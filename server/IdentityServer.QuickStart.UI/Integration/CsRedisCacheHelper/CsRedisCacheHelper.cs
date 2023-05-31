// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using CSRedis;
using Laiye.SaasMp.WebApi.Integration;
using System.Security.Cryptography;
using System.Text.Json;

namespace IdentityServer.QuickStart.UI.Integration.CsRedisCacheHelper
{

    public static class CacheHelperConstants
    {
        public static JsonSerializerOptions DefaultJsonSerializerSettings = new JsonSerializerOptions();
    }

    public class CsRedisCacheHelper : ICacheHelper
    {
        private CSRedisClient RedisClient => _csRedisClientManager.RedisClient;

        private CSRedisClientManager _csRedisClientManager;

        private ILogger<CsRedisCacheHelper> Logger { get; }

        public CsRedisCacheHelper(CSRedisClientManager csRedisClientManager, ILogger<CsRedisCacheHelper> logger)
        {
            _csRedisClientManager = csRedisClientManager;
            Logger = logger;
        }

        private string GetCacheKey(string category, string key)
        {
            return $"{CacheKeys.RedisKeyPrefix}:{category}:{key}";
        }

        public async Task RemoveCacheAsync(string category, string key)
        {
            var cacheKey = GetCacheKey(category, key);
            await RedisClient.DelAsync(cacheKey);
        }

        public async Task SetCacheValueAsync(string category, string key, string value, TimeSpan expireSpan)
        {
            var cacheKey = GetCacheKey(category, key);
            await RedisClient.SetAsync(cacheKey, value, expireSpan);
        }

        public async Task<T> GetFromCacheAsync<T>(string category,
           string key,
           TimeSpan expireSpan,
           Func<Task<T>> dataFunc,
           bool randomExpire = false) where T : class
        {
            return await GetFromCacheAsync(category, key, expireSpan, dataFunc, CacheHelperConstants.DefaultJsonSerializerSettings, randomExpire);
        }


        public async Task<T> GetFromCacheAsync<T>(string category,
            string key,
            TimeSpan expireSpan,
            Func<Task<T>> dataFunc,
            JsonSerializerOptions jsonSerializerSettings,
            bool randomExpire = false) where T : class
        {

            using var generator = RandomNumberGenerator.Create();
            var bytes = new byte[1];
            generator.GetNonZeroBytes(bytes);
            var nonceSeconds = bytes[0] % 30;

            if (randomExpire)
            {
                expireSpan = expireSpan.Add(TimeSpan.FromSeconds(nonceSeconds));
            }

            var cacheKey = GetCacheKey(category, key);
            var cached = await RedisClient.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                try
                {
                    var obj = JsonSerializer.Deserialize<T>(cached, jsonSerializerSettings);
                    return obj;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, $"Cached value {cacheKey} is corrupted");
                    Logger.LogInformation($"* type is: {typeof(T)}");
                    Logger.LogInformation($"* value is: {cached}");
                    await RedisClient.DelAsync(cacheKey);
                }
            }

            var data = await dataFunc();
            // 数据没取对，就不要放进去了
            if (data != null)
            {
                await RedisClient.SetAsync(cacheKey, JsonSerializer.Serialize(data, jsonSerializerSettings), expireSpan);
            }

            return data;
        }

        public async Task RSetAddAsync(string category, string key, params string[] values)
        {
            await RedisClient.SAddAsync(GetCacheKey(category, key), values);
        }

        public async Task<string[]> RSetScanAsync(string category, string key, long cursor, string pattern, int count)
        {
            var result = await RedisClient.SScanAsync(GetCacheKey(category, key), cursor, $"{CacheKeys.RedisKeyPrefix}:{category}:{pattern}", count);

            return result.Items;
        }

        public async Task RSetRemAsync(string category, string key, params string[] values)
        {
            await RedisClient.SRemAsync(GetCacheKey(category, key), values);
        }

        public async Task<long> IncrByAsync(string category, string key, long step = 1)
        {
            return await RedisClient.IncrByAsync(GetCacheKey(category, key), step);
        }

        public async Task ExpireAtAsync(string category, string key, DateTime expireTime)
        {
            await RedisClient.ExpireAtAsync(GetCacheKey(category, key), expireTime);
        }

        public async Task<bool> ExistsAsync(string category, string key)
        {
            return await RedisClient.ExistsAsync(GetCacheKey(category, key));
        }

        public async Task<long> TtlAsync(string category, string key)
        {
            return await RedisClient.TtlAsync(GetCacheKey(category, key));
        }

        public async Task<T> GetCacheValueAsync<T>(string category, string key)
        {
            return await RedisClient.GetAsync<T>(GetCacheKey(category, key));
        }

        public async Task ExpireCacheAsync(string category, string key, TimeSpan timeSpan)
        {
            await RedisClient.ExpireAsync(GetCacheKey(category, key), timeSpan);
        }
    }

}
