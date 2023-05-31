// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Types;
using IdentityServer4.Stores;
using Laiye.SaasMp.WebApi.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Storage
{
    internal class ClientStoreImpl : IClientStore
    {
        private const string CacheCategory = "ids:clientStore";

        private IDbClientService ClientService { get; }

        private ICacheHelper CacheHelper { get; }

        public ClientStoreImpl(IDbClientService clientService,
            ICacheHelper cacheHelper)
        {
            ClientService = clientService;
            CacheHelper = cacheHelper;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var clientInfo = await CacheHelper.GetFromCacheAsync(CacheCategory, clientId, TimeSpan.FromMinutes(30),
                async () =>
                {
                    var client = await ClientService.GetClientByClientIdAsync(new Services.Requests.GetClientByClientIdRequest { ClientId = clientId });
                    return client;
                }, true);

            return new Client
            {
                Enabled = clientInfo.Enabled == 1,
                ClientId = clientInfo.ClientId,
                ClientName = clientInfo.ClientName,
                Description = clientInfo.ClientDescription,
                AccessTokenType = clientInfo.AccessTokenType == EAccessTokenType.Jwt
                       ? AccessTokenType.Jwt : AccessTokenType.Reference,

                ClientUri = clientInfo.ClientUri,
                LogoUri = clientInfo.LogoUri,
                AllowedGrantTypes = clientInfo.AllowedGrantTypes,
                RedirectUris = clientInfo.RedirectUris,
                PostLogoutRedirectUris = clientInfo.PostLogoutRedirectUris,
                AllowedCorsOrigins = clientInfo.AllowedCorsOrigins,
                ClientSecrets = clientInfo.ClientSecrets.Select(p => new Secret(p.Sha256())).ToList(),
                AllowedScopes = clientInfo.AllowedScopes,
                RequireConsent = clientInfo.RequireConsent == 1,
                AllowRememberConsent = clientInfo.AllowRememberConsent == 1,
                RequirePkce = clientInfo.RequirePkce == 1,
                FrontChannelLogoutUri = clientInfo.FrontChannelLogoutUri,
                BackChannelLogoutUri = clientInfo.BackChannelLogoutUri,
                AllowOfflineAccess = clientInfo.AllowOfflineAccess == 1,
                RequireClientSecret = clientInfo.RequireClientSecret == 1,
                AccessTokenLifetime = clientInfo.TokenLifetime,
            };
        }
    }
}
