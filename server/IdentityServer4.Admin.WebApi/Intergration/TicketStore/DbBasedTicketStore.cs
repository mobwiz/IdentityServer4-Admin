// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using NetTopologySuite.Algorithm;
using System.Security.Cryptography;
using System.Web;

namespace IdentityServer4.Admin.WebApi.Intergration.TicketStore
{
    public class PostConfgureLocalTicketStoreOptions : IPostConfigureOptions<CookieAuthenticationOptions>
    {
        private ITicketStore _ticketStore;

        public PostConfgureLocalTicketStoreOptions(ITicketStore ticketStore)
        {
            _ticketStore = ticketStore;
        }

        public void PostConfigure(string name, CookieAuthenticationOptions options)
        {
            options.SessionStore = _ticketStore;
        }
    }

    public class DbBasedTicketStore : ITicketStore
    {
        private IDbTicketService _ticketService;

        public DbBasedTicketStore(IDbTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private const int DefaultExpireTime = 30 * 60;// 30 分钟

        public async Task RemoveAsync(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                await _ticketService.RemoveTicketByKeyAsync(key);
            }
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            // check the paramters
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            var ticketData = new TicketDataDto
            {
                Key = key,
                Base64Data = Convert.ToBase64String(SerializeToBytes(ticket)),
                SubjectId = ticket.Principal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "0",
                LastActivity = DateTime.UtcNow,
                ExpireTime = ticket.Properties.ExpiresUtc?.UtcDateTime ?? DateTime.UtcNow.AddSeconds(DefaultExpireTime)
            };

            await _ticketService.UpdateTicketAsync(ticketData);
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            // check the parameters
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var data = await _ticketService.GetTicketByKeyAsync(key);

            if (data == null)
            {
                return null;
            }

            if (data.ExpireTime < DateTime.UtcNow)
            {
                await _ticketService.RemoveTicketByKeyAsync(key);
                return null;
            }

            var ticket = DeserializeFromBytes(data.Data);
            return ticket;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {

            var ticketId = GetSafeRandomString(32);
            var ticketData = new TicketDataDto
            {
                Key = ticketId,
                Base64Data = Convert.ToBase64String(SerializeToBytes(ticket)),
                SubjectId = ticket.Principal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "0",
                LastActivity = DateTime.UtcNow,
                ExpireTime = ticket.Properties.ExpiresUtc?.UtcDateTime ?? DateTime.UtcNow.AddSeconds(DefaultExpireTime)
            };

            await _ticketService.CreateTicketAsync(ticketData);

            return ticketId;
        }

        private string GetSafeRandomString(int length)
        {
            var bits = length * 6;
            var byte_size = (bits + 7) / 8;
            var bytesarray = RandomNumberGenerator.GetBytes(byte_size);
            var base64 = Convert.ToBase64String(bytesarray);

            return base64.Replace("=", "").Replace("+", "-").Replace("/", "_");
        }

        private byte[] SerializeToBytes(AuthenticationTicket source)
            => TicketSerializer.Default.Serialize(source);

        private AuthenticationTicket? DeserializeFromBytes(byte[] source)
            => source == null ? null : TicketSerializer.Default.Deserialize(source);
    }
}
