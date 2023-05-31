// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using IdentityServer4.Stores;
using Laiye.SaasMp.WebApi.Integration;
using Mobwiz.Common.General;
using Microsoft.Extensions.Logging;
using System.Reflection.Emit;
using System.Text.Json;

namespace IdentityServer4.Storage.FreeSql.Storage
{
    internal class ReferenceTokenStoreImpl : IReferenceTokenStore
    {
        private const string CacheCategory = "ids:reftoken";
        private const int TokenRemainInCacheSeconds = 10;

        private ICacheHelper CacheHelper { get; }
        private IDbReferenceTokenService ReferenceTokenService { get; }
        private ILogger<ReferenceTokenStoreImpl> Logger { get; }

        private IIdGenerator IdGenerator { get; }

        public ReferenceTokenStoreImpl(ICacheHelper cacheHelper,
            IDbReferenceTokenService referenceTokenService,
            ILogger<ReferenceTokenStoreImpl> logger,
            IIdGenerator idGenerator)
        {
            CacheHelper = cacheHelper;
            ReferenceTokenService = referenceTokenService;
            Logger = logger;
            IdGenerator = idGenerator;
        }

        public async Task<Token> GetReferenceTokenAsync(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle)) throw new ArgumentNullException(handle);
            var tokenInstance = await CacheHelper.GetFromCacheAsync(CacheCategory, handle, TimeSpan.FromSeconds(60),
                async () =>
                {
                    var refToken = await ReferenceTokenService.GetReferenceTokenByHandleAsync(new GetReferenceTokenByHandleRequest
                    {
                        Handle = handle
                    });

                    if (refToken != null)
                    {
                        try
                        {
                            var token = JsonSerializer.Deserialize<Token>(refToken.Data, Helpers.GetDefaultJsonSerializerSettings());
                            return token;
                        }
                        catch (Exception)
                        {
                            await ReferenceTokenService.RemoveReferenceTokenByHandleAsync(new RemoveReferenceTokenByHandleRequest { Handle = handle });
                        }
                    }

                    return null;

                }, Helpers.GetDefaultJsonSerializerSettings());

            Logger.LogDebug($"GetReferenceToken=>{handle}，tokenInstance=>{tokenInstance != null}");
            return tokenInstance;
        }

        public async Task RemoveReferenceTokenAsync(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle)) throw new ArgumentNullException(nameof(handle));

            Logger.LogDebug($"RemoveReferenceTokenAsync=>{handle}");

            await CacheHelper.ExpireCacheAsync(CacheCategory, handle, TimeSpan.FromSeconds(TokenRemainInCacheSeconds));
            await this.ReferenceTokenService.RemoveReferenceTokenByHandleAsync(new RemoveReferenceTokenByHandleRequest { Handle = handle });
        }

        public async Task RemoveReferenceTokensAsync(string subjectId, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(clientId);

            Logger.LogDebug($"RemoveReferenceTokensAsync=>{subjectId},{clientId}");

            var tokens = await ReferenceTokenService.RemoveReferenceTokensAsync(new RemoveReferenceTokensRequest
            {
                ClientId = clientId,
                SubjectId = subjectId ?? ""
            });

            foreach (var token in tokens)
            {
                await CacheHelper.ExpireCacheAsync(CacheCategory, token, TimeSpan.FromSeconds(TokenRemainInCacheSeconds));
            }
        }

        public async Task<string> StoreReferenceTokenAsync(Token token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            var id = await IdGenerator.GenerateIdAsync(CacheCategory);

            // 增加 prefix，便于前端排序，区分token的新旧
            var handle = $"u_{id}{CryptoRandom.CreateUniqueId(32, CryptoRandom.OutputFormat.Hex).ToLower()}";

            Logger.LogDebug($"StoreReferenceTokenAsync=>{handle}");

            // TODO 如果之前有 token，可以读出来，然后将旧的会话失效掉

            // 一个 subject 只能有一个  token, 加一个 company id 的条件
            {
                var tokensRemoved = await ReferenceTokenService.RemoveReferenceTokensAsync(new RemoveReferenceTokensRequest
                {
                    ClientId = token.ClientId,
                    SubjectId = token.SubjectId ?? String.Empty
                });

                foreach (var item in tokensRemoved)
                {
                    await CacheHelper.ExpireCacheAsync(CacheCategory, item, TimeSpan.FromSeconds(TokenRemainInCacheSeconds));
                }
            }

            var sid = (from x in token.Claims
                       where x.Type == JwtClaimTypes.SessionId
                       select x.Value).SingleOrDefault() ?? "";
            var jsonData = JsonSerializer.Serialize(token, Helpers.GetDefaultJsonSerializerSettings());

            var request = new CreateReferenceTokenRequest
            {
                Token = new Services.Dto.ReferenceTokenDto
                {
                    ClientId = token.ClientId,
                    CreationTime = token.CreationTime,
                    Data = jsonData,
                    ExpiresIn = token.Lifetime,
                    Handle = handle,
                    SubjectId = token.SubjectId ?? String.Empty,
                    SessionId = sid
                }
            };
            await ReferenceTokenService.CreateReferenceTokenAsync(request);

            await CacheHelper.SetCacheValueAsync(CacheCategory, handle, jsonData, TimeSpan.FromSeconds(token.Lifetime));

            return handle;
        }
    }

}
