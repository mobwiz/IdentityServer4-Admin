// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Laiye.SaasMp.WebApi.Integration;
using System.Security.Cryptography;
using System.Text.Json;

namespace Mobwiz.Common.Tests.FakeObjects
{
    internal interface ICacheHelperTestable
    {
        // 可用于检查 key
        bool CacheExisted(string category, string key);

        bool RSetContains(string category, string key, string value);

        int RSetSize(string category, string key);

        string GetStringValue(string category, string key);
    }

    internal class MockCacheHelper : ICacheHelper, ICacheHelperTestable
    {
        class CacheObject<T>
        {
            public T Value { get; set; }
            public TimeSpan? ExpireTime { get; set; }
        }

        private Dictionary<string, CacheObject<string>> stringCaches = new Dictionary<string, CacheObject<string>>();
        private Dictionary<string, CacheObject<HashSet<string>>> hsetCaches = new Dictionary<string, CacheObject<HashSet<string>>>();

        public async Task<T> GetFromCacheAsync<T>(string category, string key, TimeSpan expireSpan, Func<Task<T>> dataFunc, bool randomExpire = false) where T : class
        {
            return await GetFromCacheAsync(category, key, expireSpan, dataFunc, CacheHelperConstants.DefaultJsonSerializerSettings, randomExpire);
        }

        public async Task<T> GetFromCacheAsync<T>(string category, string key, TimeSpan expireSpan, Func<Task<T>> dataFunc, JsonSerializerOptions jsonSerializerSettings, bool randomExpire = false) where T : class
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
            if (stringCaches.TryGetValue(cacheKey, out var cachedObject))
            {
                if (!string.IsNullOrEmpty(cachedObject.Value))
                {
                    try
                    {
                        var obj = JsonSerializer.Deserialize<T>(cachedObject.Value, jsonSerializerSettings);
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        stringCaches.Remove(cacheKey);
                    }
                }
            }

            var data = await dataFunc();
            // 数据没取对，就不要放进去了
            if (data != null)
            {
                stringCaches[cacheKey] = new CacheObject<string>
                {
                    Value = JsonSerializer.Serialize(data, jsonSerializerSettings),
                    ExpireTime = expireSpan
                };

                return data;
            }

            return null;
        }

        public Task RemoveCacheAsync(string category, string key)
        {
            var cacheKey = GetCacheKey(category, key);
            if (stringCaches.ContainsKey(cacheKey))
            {
                stringCaches.Remove(cacheKey);
            }
            return Task.CompletedTask;
        }

        public Task RSetAddAsync(string category, string key, params string[] values)
        {
            if (hsetCaches.TryGetValue(GetCacheKey(category, key), out var cache))
            {
                foreach (var val in values)
                {
                    cache.Value.Add(val);
                }
            }
            else
            {
                hsetCaches.Add(GetCacheKey(category, key), new CacheObject<HashSet<string>>
                {
                    Value = new HashSet<string>(values)
                });
            }

            return Task.CompletedTask;
        }

        public Task RSetRemAsync(string category, string key, params string[] values)
        {
            if (hsetCaches.TryGetValue(GetCacheKey(category, key), out var cache))
            {
                foreach (var val in values)
                {
                    cache.Value.Remove(val);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> RSetScanAsync(string category, string key, long cursor, string pattern, int count)
        {
            // pattern 这里只支持 prefix
            var prefix = pattern.Replace("*", "");
            var result = new List<string>();

            if (hsetCaches.TryGetValue(GetCacheKey(category, key), out var cache))
            {
                foreach (var val in cache.Value)
                {
                    if (val.StartsWith(prefix))
                    {
                        result.Add(val);
                    }
                }
            }

            return Task.FromResult(result.ToArray());

        }

        public Task SetCacheValueAsync(string category, string key, string value, TimeSpan expireSpan)
        {
            stringCaches[GetCacheKey(category, key)] = new CacheObject<string>() { Value = value, ExpireTime = expireSpan };

            return Task.CompletedTask;
        }


        public Task<T> GetCacheValueAsync<T>(string category, string key)
        {
            var cacheKey = GetCacheKey(category, key);

            if (stringCaches.TryGetValue(cacheKey, out var obj))
            {
                return Task.FromResult(JsonSerializer.Deserialize<T>(obj.Value));
            }

            return Task.FromResult(default(T));
        }

        public string GetCacheKey(string category, string key)
        {
            return $"fake:{category}:{key}";
        }

        // testability

        public bool CacheExisted(string category, string key)
        {
            var cacheKey = GetCacheKey(category, key);
            return stringCaches.ContainsKey(cacheKey);
        }

        public bool RSetContains(string category, string key, string value)
        {
            var cacheKey = GetCacheKey(category, key);

            if (hsetCaches.TryGetValue(cacheKey, out var obj))
            {
                return obj.Value.Contains(value);
            }

            return false;
        }

        public int RSetSize(string category, string key)
        {
            var cacheKey = GetCacheKey(category, key);

            if (hsetCaches.TryGetValue(cacheKey, out var obj))
            {
                return obj.Value.Count;
            }

            throw new Exception("Cache not found");
        }

        public string GetStringValue(string category, string key)
        {
            var cacheKey = GetCacheKey(category, key);

            if (stringCaches.TryGetValue(cacheKey, out var obj))
            {
                return obj.Value;
            }

            throw new Exception("Cache not found");
        }

        public Task<bool> KeyExistsAsync(string category, string key)
        {
            var cacheKey = GetCacheKey(category, key);

            if (stringCaches.TryGetValue(cacheKey, out var obj))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<long> IncrByAsync(string category, string key, long step = 1)
        {
            throw new NotImplementedException();
        }

        public Task ExpireAtAsync(string category, string key, DateTime expireTime)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string category, string key)
        {
            throw new NotImplementedException();
        }

        public Task<long> TtlAsync(string category, string key)
        {
            throw new NotImplementedException();
        }

        public Task ExpireCacheAsync(string cacheCategory, string handle, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}
