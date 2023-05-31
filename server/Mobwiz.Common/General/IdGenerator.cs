using Laiye.SaasMp.WebApi.Integration;
using Mobwiz.Common.Exceptions;

namespace Mobwiz.Common.General
{
    /// <summary>
    /// Id生成器
    /// </summary>
    public class IdGenerator : IDisposable, IIdGenerator
    {
        private const string CacheCategory = "idgenerator";

        private ICacheHelper RedisClient { get; }

        /// <summary>
        /// 实例化数据库路由
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="logger"></param>
        public IdGenerator(ICacheHelper redisClient)
        {
            RedisClient = redisClient;
        }

        private static DateTime DateTimeStartTime { get; } = DateTime.Parse("2017-01-01Z").ToUniversalTime();

        /// <summary>
        /// 生成Id
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dateTime"></param>
        /// <param name="inc">生成Id数量</param>
        /// <returns></returns>
        public Task<long> GenerateIdAsync(string key, DateTime dateTime = default, int inc = 1)
        {
            return Task.FromResult(GenerateId(key, dateTime));
        }

        /// <summary>
        /// 生成数据库Id
        /// </summary>
        /// <returns></returns>
        public long GenerateId(string key, DateTime dateTime = default, int inc = 1)
        {
            lock (this)
            {

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new BllException(9001005, "Entity key can't be null or empty");
                }
                if (dateTime == default)
                {
                    dateTime = DateTime.UtcNow;
                }
                dateTime = dateTime.ToUniversalTime();

                var cacheKey = $"entity_id:{key}:{dateTime.ToString("yyyyMMddHHmmss")}";

                var id = RedisClient.IncrByAsync(CacheCategory, cacheKey, inc).GetAwaiter().GetResult();
                RedisClient.ExpireCacheAsync(CacheCategory, cacheKey, TimeSpan.FromSeconds(7200));

                var result = (long)(dateTime - DateTimeStartTime).TotalSeconds << 24;
                result |= id;

                return result;
            }
        }

        /// <summary>
        /// 根据日期获取Id
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="inc">自增数</param>
        /// <param name="nodeId">节点Id</param>
        /// <returns></returns>
        public long GetIdByTime(DateTime dateTime, byte nodeId = 0, ushort inc = 0)
        {
            var result = (long)(dateTime - DateTimeStartTime).TotalSeconds << 32;
            result |= (long)nodeId << 16;
            result |= inc;
            return result;
        }
        public void Dispose()
        {
        }
    }
}
