// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Laiye.SaasMp.WebApi.Integration;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.WebApi.Utils
{
    public class SecurityOptions
    {
        public int MaxTry { get; set; } = 5;
        public int FreezeSeconds { get; set; } = 600;
        public TimeSpan FreezeTime
        {
            get
            {
                return TimeSpan.FromSeconds(FreezeSeconds);
            }
        }
    }

    public class SecurityChecker
    {
        private ICacheHelper _cacheHelper;

        private const string CacheCategory = "security";

        private SecurityOptions _securityOptions;

        public SecurityChecker(ICacheHelper cacheHelper, IOptions<SecurityOptions> options)
        {
            _cacheHelper = cacheHelper;
            _securityOptions = options.Value;
        }

        public async Task<Tuple<bool, long, TimeSpan>> CheckIsFreezed(string username)
        {
            var count = await _cacheHelper.GetCacheValueAsync<long>(CacheCategory, GetUserFreezeKey(username));

            if (count >= _securityOptions.MaxTry)
            {
                var ttlSeconds = await _cacheHelper.TtlAsync(CacheCategory, GetUserFreezeKey(username));

                return new Tuple<bool, long, TimeSpan>(true, count, TimeSpan.FromSeconds((int)ttlSeconds));
            }

            return new Tuple<bool, long, TimeSpan>(false, 0, TimeSpan.Zero);
        }

        public async Task RecordFailedLogin(string username)
        {
            await _cacheHelper.IncrByAsync(CacheCategory, GetUserFreezeKey(username));
            await _cacheHelper.ExpireCacheAsync(CacheCategory, GetUserFreezeKey(username), _securityOptions.FreezeTime);
        }

        public async Task ClearFailedLogin(string username)
        {
            await _cacheHelper.RemoveCacheAsync(CacheCategory, GetUserFreezeKey(username));
        }

        private string GetUserFreezeKey(string username)
        {
            return $"failedlogin:{username}";
        }

    }

    public class SecureTextAttribute : ValidationAttribute
    {
        public string DangerChars { get; set; }
        public SecureTextAttribute()
        {
            DangerChars = "</>|':";
        }

        public SecureTextAttribute(string dangerChars)
        {
            DangerChars = dangerChars;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("{0} can't include these special chars \"{1}\"", name, DangerChars);
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var str = value.ToString();
            foreach (var c in DangerChars)
            {
                if (str.IndexOf(c) >= 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
