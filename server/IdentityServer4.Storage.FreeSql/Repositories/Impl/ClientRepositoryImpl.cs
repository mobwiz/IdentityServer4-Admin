// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Services;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Types;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class ClientRepositoryImpl : InnerBaseRepository, IClientRepository
    {
        public ClientRepositoryImpl(DatabaseConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        public Task<long> CreateClientAsync(MClient client)
        {
            // check input parameters
            if (client == null) throw new ArgumentNullException(nameof(client));

            if (string.IsNullOrEmpty(client.ClientId))
            {
                throw new BllException(500, "Client id must not be empty");
            }

            var id = 0L;

            using (var conn = GetConnection())
            {
                conn.Transaction(() =>
                {
                    var repos = conn.GetRepository<MClient>();

                    client.CreateTime = DateTime.UtcNow;

                    repos.Insert(client);

                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    strRepos.StoreValues(EStringType.ClientAllowedGrantType, client.Id, client.AllowedGrantTypes);
                    strRepos.StoreValues(EStringType.ClientRedirectUri, client.Id, client.RedirectUris);
                    strRepos.StoreValues(EStringType.ClientPostLogoutRedirectUri, client.Id, client.PostLogoutRedirectUris);
                    strRepos.StoreValues(EStringType.ClientCorsOrigin, client.Id, client.AllowedCorsOrigins);
                    strRepos.StoreValues(EStringType.ClientSecret, client.Id, client.ClientSecrets);
                    strRepos.StoreValues(EStringType.ClientAllowedScope, client.Id, client.AllowedScopes);

                    var claimsReps = new ClientClaimRepositoryImpl(conn);

                    claimsReps.SaveClaims(client.Id, client.ClientClaims.Select(p => new MClientClaim
                    {
                        ClientId = client.Id,
                        Type = p.Type,
                        Value = p.Value,
                    }));

                    id = client.Id;
                });
            }

            return Task.FromResult(id);

        }

        public async Task<IList<string>> GetAllowedCorsOriginsAsync()
        {
            using (var conn = GetConnection())
            {
                var repos = conn.GetRepository<MClient>();
                var clientIds = await repos.Select.Where(p => p.Enabled == 1).ToListAsync(x => x.Id);
                var strRepos = new StringCollectionRepositoryImpl(conn);

                var list = strRepos.GetValues(EStringType.ClientCorsOrigin, clientIds.ToArray());

                return list;
            }
        }

        public async Task<IList<string>> GetAllowedPostLogoutUrisAsync()
        {
            using (var conn = GetConnection())
            {
                var repos = conn.GetRepository<MClient>();
                var clientIds = await repos.Select.Where(p => p.Enabled == 1).ToListAsync(x => x.Id);
                var strRepos = new StringCollectionRepositoryImpl(conn);

                var list = strRepos.GetValues(EStringType.ClientPostLogoutRedirectUri, clientIds.ToArray());

                return list;
            }
        }

        public async Task<IList<string>> GetAllowedRedirectUrisAsync()
        {
            using (var conn = GetConnection())
            {
                var repos = conn.GetRepository<MClient>();
                var clientIds = await repos.Select.Where(p => p.Enabled == 1).ToListAsync(x => x.Id);
                var strRepos = new StringCollectionRepositoryImpl(conn);

                var list = strRepos.GetValues(EStringType.ClientRedirectUri, clientIds.ToArray());

                return list;
            }
        }

        public async Task<MClient?> GetClientByClientIdAsync(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));

            using (var conn = GetConnection())
            {
                var repos = conn.GetRepository<MClient>();
                var client = await repos.Select.Where(p => p.ClientId == clientId).FirstAsync();
                if (client != null)
                {
                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    var strItems = strRepos.GetAllsItemsById(client.Id);

                    client.AllowedGrantTypes = strItems.Where(p => p.StringType == EStringType.ClientAllowedGrantType).Select(p => p.StringValue).ToList();
                    client.RedirectUris = strItems.Where(p => p.StringType == EStringType.ClientRedirectUri).Select(p => p.StringValue).ToList();
                    client.PostLogoutRedirectUris = strItems.Where(p => p.StringType == EStringType.ClientPostLogoutRedirectUri).Select(p => p.StringValue).ToList();
                    client.AllowedCorsOrigins = strItems.Where(p => p.StringType == EStringType.ClientCorsOrigin).Select(p => p.StringValue).ToList();
                    client.ClientSecrets = strItems.Where(p => p.StringType == EStringType.ClientSecret).Select(p => p.StringValue).ToList();
                    client.AllowedScopes = strItems.Where(p => p.StringType == EStringType.ClientAllowedScope).Select(p => p.StringValue).ToList();

                    var claimRepos = new ClientClaimRepositoryImpl(conn);
                    client.ClientClaims = claimRepos.GetClaims(client.Id).ToList();
                    return client;
                }

                return null;
            }
        }

        public async Task<MClient?> GetClientByIdAsync(long id)
        {
            using (var conn = GetConnection())
            {
                var repos = conn.GetRepository<MClient>();
                var client = await repos.Select.Where(p => p.Id == id).FirstAsync();
                if (client != null)
                {
                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    var strItems = strRepos.GetAllsItemsById(client.Id);

                    client.AllowedGrantTypes = strItems.Where(p => p.StringType == EStringType.ClientAllowedGrantType).Select(p => p.StringValue).ToList();
                    client.RedirectUris = strItems.Where(p => p.StringType == EStringType.ClientRedirectUri).Select(p => p.StringValue).ToList();
                    client.PostLogoutRedirectUris = strItems.Where(p => p.StringType == EStringType.ClientPostLogoutRedirectUri).Select(p => p.StringValue).ToList();
                    client.AllowedCorsOrigins = strItems.Where(p => p.StringType == EStringType.ClientCorsOrigin).Select(p => p.StringValue).ToList();
                    client.ClientSecrets = strItems.Where(p => p.StringType == EStringType.ClientSecret).Select(p => p.StringValue).ToList();
                    client.AllowedScopes = strItems.Where(p => p.StringType == EStringType.ClientAllowedScope).Select(p => p.StringValue).ToList();

                    var claimRepos = new ClientClaimRepositoryImpl(conn);
                    client.ClientClaims = claimRepos.GetClaims(client.Id).ToList();

                    return client;
                }

                return null;
            }
        }

        public async Task<PagedResult<MClient>> QueryClientsAsync(ClientQueryParameter parameter)
        {
            using (var conn = GetConnection())
            {
                var repos = conn.GetRepository<MClient>();
                var query = repos.Select; //

                query = query.WhereIf(!string.IsNullOrWhiteSpace(parameter.Keyword),
                    p => p.ClientName.Contains(parameter.Keyword)
                    || p.ClientId.Contains(parameter.Keyword));

                query = query.WhereIf(parameter.Enabled != BoolCondition.All, p => p.Enabled == (byte)parameter.Enabled);
                query = query.OrderBy(p => p.DisplayOrder);

                var total = await query.CountAsync();
                var list = query.Skip((parameter.PageIndex - 1) * parameter.PageSize).Take(parameter.PageSize);

                return new PagedResult<MClient>(total, list.ToList());
            };
        }

        public Task RemoveClientAsync(long id)
        {
            using (var conn = GetConnection())
            {
                var repos = conn.GetRepository<MClient>();
                var client = repos.Select.Where(p => p.Id == id).First();
                if (client != null)
                {
                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    strRepos.RemoveValues(EStringType.ClientAllowedGrantType, client.Id);
                    strRepos.RemoveValues(EStringType.ClientRedirectUri, client.Id);
                    strRepos.RemoveValues(EStringType.ClientPostLogoutRedirectUri, client.Id);
                    strRepos.RemoveValues(EStringType.ClientCorsOrigin, client.Id);
                    strRepos.RemoveValues(EStringType.ClientSecret, client.Id);
                    strRepos.RemoveValues(EStringType.ClientAllowedScope, client.Id);

                    var claimsReps = new ClientClaimRepositoryImpl(conn);

                    claimsReps.RemoveClaims(client.Id);

                    repos.Delete(client);
                }
            }

            return Task.CompletedTask;
        }

        public Task UpdateClientAsync(MClient client)
        {
            if (string.IsNullOrEmpty(client.ClientId))
            {
                throw new BllException(500, "Client id must not be empty");
            }

            using (var conn = GetConnection())
            {
                conn.Transaction(() =>
                {
                    var repos = conn.GetRepository<MClient>();

                    var entity = repos.Select.Where(p => p.Id == client.Id).First();
                    if (entity != null)
                    {
                        entity.Enabled = client.Enabled;
                        entity.ClientId = client.ClientId;
                        entity.ClientName = client.ClientName;
                        entity.ClientUri = client.ClientUri;
                        entity.LogoUri = client.LogoUri;
                        entity.ClientDescription = client.ClientDescription;
                        entity.RequireConsent = client.RequireConsent;
                        entity.AllowRememberConsent = client.AllowRememberConsent;
                        entity.RequirePkce = client.RequirePkce;
                        entity.FrontChannelLogoutUri = client.FrontChannelLogoutUri;
                        entity.BackChannelLogoutUri = client.BackChannelLogoutUri;
                        entity.AllowOfflineAccess = client.AllowOfflineAccess;
                        entity.RequirePkce = client.RequirePkce;
                        entity.RequireClientSecret = client.RequireClientSecret;
                        entity.AccessTokenType = client.AccessTokenType;
                        entity.TokenLifetime = client.TokenLifetime;

                        repos.Update(entity);

                        var strRepos = new StringCollectionRepositoryImpl(conn);
                        strRepos.StoreValues(EStringType.ClientAllowedGrantType, entity.Id, client.AllowedGrantTypes);
                        strRepos.StoreValues(EStringType.ClientRedirectUri, entity.Id, client.RedirectUris);
                        strRepos.StoreValues(EStringType.ClientPostLogoutRedirectUri, entity.Id, client.PostLogoutRedirectUris);
                        strRepos.StoreValues(EStringType.ClientCorsOrigin, entity.Id, client.AllowedCorsOrigins);
                        strRepos.StoreValues(EStringType.ClientSecret, entity.Id, client.ClientSecrets);
                        strRepos.StoreValues(EStringType.ClientAllowedScope, entity.Id, client.AllowedScopes);

                        var claimsReps = new ClientClaimRepositoryImpl(conn);

                        claimsReps.SaveClaims(client.Id, client.ClientClaims.Select(p => new MClientClaim
                        {
                            ClientId = client.Id,
                            Type = p.Type,
                            Value = p.Value,
                        }));
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
