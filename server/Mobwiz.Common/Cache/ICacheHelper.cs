
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laiye.SaasMp.WebApi.Integration
{
    // try to use unit test
    public interface ICacheHelper
    {
        Task<T> GetFromCacheAsync<T>(string category, string key, TimeSpan expireSpan, Func<Task<T>> dataFunc, bool randomExpire = false) where T : class;
        Task<T> GetFromCacheAsync<T>(string category, string key, TimeSpan expireSpan, Func<Task<T>> dataFunc, JsonSerializerOptions jsonSerializerSettings, bool randomExpire = false) where T : class;
        Task RemoveCacheAsync(string category, string key);
        Task SetCacheValueAsync(string category, string key, string value, TimeSpan expireSpan);
        Task<T> GetCacheValueAsync<T>(string category, string key);

        // set functions

        Task RSetAddAsync(string category, string key, params string[] values);
        Task<string[]> RSetScanAsync(string category, string key, long cursor, string pattern, int count);
        Task RSetRemAsync(string category, string key, params string[] values);

        // hset functions
        Task<long> IncrByAsync(string category, string key, long step = 1);

        Task ExpireAtAsync(string category, string key, DateTime expireTime);

        Task<bool> ExistsAsync(string category, string key);

        Task<long> TtlAsync(string category, string key);
        Task ExpireCacheAsync(string cacheCategory, string handle, TimeSpan timeSpan);
    }

    public static class CacheHelperConstants
    {
        public static JsonSerializerOptions DefaultJsonSerializerSettings = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }
}