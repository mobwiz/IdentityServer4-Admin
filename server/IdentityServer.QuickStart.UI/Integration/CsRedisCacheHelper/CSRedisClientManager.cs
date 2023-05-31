// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CSRedis;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;
using static CSRedis.CSRedisClient;

namespace IdentityServer.QuickStart.UI.Integration.CsRedisCacheHelper
{

    public class RedisConfig
    {
        public string ConnectionString { get; set; }
    }


    /// <summary>
    /// Redis管理器
    /// </summary>
    public class CSRedisClientManager : IDisposable
    {
        public CSRedisClient RedisClient { get; }

        private IList<CSRedisClient> SentinelRedisClients { get; } = new List<CSRedisClient>();

        private CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();

        private ILogger<CSRedisClientManager> Logger { get; }

        public CSRedisClientManager(
            IOptions<RedisConfig> redisOptions,
            ILogger<CSRedisClientManager> logger)
        {
            var connectionString = redisOptions.Value.ConnectionString;

            var connection = Parse(connectionString, out var sentinels);


            if (sentinels != null && sentinels.Any())
            {
                RedisClient = new CSRedisClient(connection, sentinels);

                foreach (var item in sentinels)
                {
                    Task.Factory.StartNew(() =>
                    {

                        while (!CancellationToken.IsCancellationRequested)
                        {
                            CSRedisClient result = null;
                            try
                            {
                                result = new CSRedisClient(item);

                                result.Subscribe(("+switch-master", new Action<SubscribeMessageEventArgs>(args =>
                                {
                                    logger.LogTrace($"+switch-master {args.Body}");
                                    if (RedisClient != null)
                                    {
                                        foreach (var item in RedisClient.Nodes.Values.ToArray())
                                        {
                                            // 为什么要用DateTime.Now，因为CSRedis内部也是用的DateTime.Now
                                            item.SetUnavailable(new Exception($"+switch-master {args.Body}"), DateTime.Now);
                                        }
                                    }
                                })));

                                logger.LogTrace($"sub {item} +switch-master");

                                SentinelRedisClients.Add(result);

                                return;

                                //return result;
                            }
                            catch (Exception ex)
                            {
                                logger.LogTrace(ex.ToString());
                                try
                                {
                                    result?.Dispose();
                                }
                                catch (Exception ex2)
                                {
                                    logger.LogTrace(ex2.ToString());
                                }
                            }

                            Thread.Sleep(1000);
                        }
                    });
                }
            }
            else
            {
                RedisClient = new CSRedisClient(connection);
            }
            Logger = logger;
        }


        //private CSRedisClient GetCSRedisClient(string connectionString)
        //{
        //    var connStr = Parse(connectionString, out var sentinels);

        //    var maskedConnStr = Regex.Replace(connStr, @"password=[^,]*", "password=******", RegexOptions.IgnoreCase);

        //    Console.WriteLine($"try to connect redis {maskedConnStr}");

        //    if (sentinels?.Any() == true)
        //    {
        //        Console.WriteLine($"try to connect redis sentinel: {string.Join(",", sentinels)}");
        //        return new CSRedisClient(connStr, sentinels);
        //    }

        //    return new CSRedisClient(connStr);
        //}

        /// <summary>
        /// 将Redis连接字符串转换为CSRedis可用的连接字符串
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sentinels"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Parse(string connectionString, out string[] sentinels)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var serviceName = "";
            var listHost = new List<string>();
            var listOther = new List<string>();

            if (connectionString.Contains(",serviceName="))
            {
                var arr = connectionString.Split(',');

                foreach (var item in arr)
                {
                    if (item.Contains(":"))
                    {
                        listHost.Add(item);
                    }
                    else if (item.Contains("="))
                    {
                        if (item.StartsWith("serviceName="))
                        {
                            serviceName = item.Substring("serviceName=".Length);
                        }
                        else
                        {
                            listOther.Add(item);
                        }
                    }
                }
            }

            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                sentinels = listHost.ToArray();
                sb.Append(serviceName);
                foreach (var item in listOther)
                {
                    sb.Append("," + item);
                }
            }
            else
            {
                sentinels = new string[0];
                sb.Append(connectionString);
            }

            // ensure there is no ',' at the end of sb                
            var result = sb.ToString().TrimEnd(',');

            if (!result.Contains("connectTimeout"))
            {
                sb.Append(",connectTimeout=1000");
            }
            if (!result.Contains("syncTimeout"))
            {
                sb.Append(",syncTimeout=1000");
            }
            if (!result.Contains("idleTimeout"))
            {
                sb.Append(",idleTimeout=10000");
            }
            if (!result.Contains("tryit"))
            {
                sb.Append(",tryit=5");
            }
            if (!result.Contains("poolSize"))
            {
                sb.Append(",poolSize=200");
            }
            result = sb.ToString();
            return result;

        }

        public void Dispose()
        {
            CancellationToken.Cancel();
            try
            {
                RedisClient.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            foreach (var item in SentinelRedisClients)
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
