// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Admin.WebApi.Intergration.CsRedisCacheHelper;
using IdentityServer4.Admin.WebApi.Utils;
using Laiye.SaasMp.WebApi.Integration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IdentityServer4.Admin.WebApi.Intergration.TicketStore
{
    public class CachedBasedTicketStore : ITicketStore
    {
        private readonly ICacheHelper _cacheHelper;
        private readonly ILogger<CachedBasedTicketStore> _logger;

        private const string CacheCategory = "ticket";

        public CachedBasedTicketStore(ICacheHelper cacheHelper,
            ILogger<CachedBasedTicketStore> logger)
        {
            _cacheHelper = cacheHelper;
            _logger = logger;
        }

        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            await _cacheHelper.RemoveCacheAsync(CacheCategory, key);
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            byte[] val = SerializeToBytes(ticket);
            // 用户指定key进行存储                
            await _cacheHelper.SetCacheValueAsync(CacheCategory, key, Convert.ToBase64String(val), GetExpireSpan(ticket));
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            var base64 = await _cacheHelper.GetCacheValueAsync<string>(CacheCategory, key);

            try
            {
                if (!string.IsNullOrEmpty(base64))
                {
                    var ticket = DeserializeFromBytes(Convert.FromBase64String(base64));
                    return ticket;
                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning("Can't deserialize the cached ticket.");
            }

            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            var ticketKey = Helper.GetSafeRandomString(32);
            var base64 = Convert.ToBase64String(SerializeToBytes(ticket));
            await _cacheHelper.SetCacheValueAsync(CacheCategory, ticketKey, base64, GetExpireSpan(ticket));
            return ticketKey;
        }

        private static byte[] SerializeToBytes(AuthenticationTicket source)
        {
            return TicketSerializer.Default.Serialize(source);
        }

        private static AuthenticationTicket? DeserializeFromBytes(byte[] source)
        {
            return source == null ? null : TicketSerializer.Default.Deserialize(source);
        }

        private TimeSpan GetExpireSpan(AuthenticationTicket ticket)
        {
            return (ticket.Properties?.ExpiresUtc - DateTime.UtcNow) ?? TimeSpan.FromMinutes(30);
        }
    }
}
