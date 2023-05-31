// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NLog;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdentityServer4.Admin.WebApi.Utils
{
    public static class Helper
    {
        public static Logger GetLogger(Type type)
        {
            Logger logger;
            if ("Development".Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                logger = LogManager.Setup().LoadConfigurationFromFile("nlog.Development.config").GetLogger(type.Name);
            }
            else
            {
                logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetLogger(type.Name);
            }

            return logger;
        }

        public static string GetSafeRandomString(int length)
        {
            var bits = length * 6;
            var byte_size = (bits + 7) / 8;
            var bytesarray = RandomNumberGenerator.GetBytes(byte_size);
            var base64 = Convert.ToBase64String(bytesarray);

            return base64.Replace("=", "").Replace("+", "-").Replace("/", "_");
        }

        public static string GetClientIp(this HttpContext context0)
        {
            Func<HttpContext, string> getIP = (HttpContext context) =>
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var value0))
                {
                    var xfor = value0.First();
                    if (!string.IsNullOrWhiteSpace(xfor))
                    {
                        var arr = xfor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Any())
                        {
                            return arr[0];
                        }
                    }
                }

                if (context.Request.Headers.TryGetValue("X-Real-Ip", out var value1))
                {
                    return value1.First();
                }

                return context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "0.0.0.0"; // unknown
            };

            var ip = getIP(context0);
            if (ip.IndexOf(':') > 0)
            {
                ip = ip.Substring(0, ip.IndexOf(':'));
            }

            return ip;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                list.Add(value);
            }
        }

        //public static JsonSerializerSettings GetJsonSerializerSettings()
        //{
        //    return new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //        ContractResolver = new CamelCasePropertyNamesContractResolver()
        //    };
        //}

        public static JsonSerializerOptions GetJsonSerializeOptions()
        {
            return new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
    }
}
