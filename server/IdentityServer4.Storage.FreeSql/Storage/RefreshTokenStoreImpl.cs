// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using IdentityServer4.Stores;
using Laiye.SaasMp.WebApi.Integration;
using System.Text.Json;

namespace IdentityServer4.Storage.FreeSql.Storage
{
    internal class RefreshTokenStoreImpl : IRefreshTokenStore
    {
        private const string CacheCategory = "ids:refreshtoken";

        private ICacheHelper _cacheHelper { get; }
        private IDbRefreshTokenService RefreshTokenService { get; }

        public RefreshTokenStoreImpl(IDbRefreshTokenService refreshTokenService)
        {
            RefreshTokenService = refreshTokenService;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenHandle)) throw new ArgumentNullException(nameof(refreshTokenHandle));

            var tokenInstance = await _cacheHelper.GetFromCacheAsync(CacheCategory, refreshTokenHandle, TimeSpan.FromSeconds(60), async () =>
            {
                var refToken = await RefreshTokenService.GetRefreshTokenByHandleAsync(new GetRefreshTokenByHandleRequest
                {
                    Handle = refreshTokenHandle
                });

                if (refToken != null)
                {
                    try
                    {
                        var token = JsonSerializer.Deserialize<RefreshToken>(refToken.Data, Helpers.GetDefaultJsonSerializerSettings());
                        return token;
                    }
                    catch (Exception)
                    {
                        await RefreshTokenService.RemoveRefreshTokenByHandleAsync(new RemoveRefreshTokenByHandleRequest
                        {
                            Handle = refreshTokenHandle
                        });
                    }
                }
                return null;
            }, Helpers.GetDefaultJsonSerializerSettings(), true);

            return tokenInstance;
        }

        public async Task RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenHandle))
                throw new ArgumentNullException(nameof(refreshTokenHandle));

            await this.RefreshTokenService.RemoveRefreshTokenByHandleAsync(new RemoveRefreshTokenByHandleRequest
            {
                Handle = refreshTokenHandle
            });

            await _cacheHelper.RemoveCacheAsync(CacheCategory, refreshTokenHandle);
        }

        public async Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            if (string.IsNullOrWhiteSpace(subjectId)) throw new ArgumentNullException(nameof(subjectId));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));

            // TODO 这里还应该有个批量

            var tokens = await this.RefreshTokenService.RemoveRefreshTokensAsync(new RemoveRefreshTokensRequest
            {
                ClientId = clientId,
                SubjectId = subjectId
            });

            foreach (var token in tokens)
            {
                await _cacheHelper.RemoveCacheAsync(CacheCategory, token);
            }
        }

        public async Task<string> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null) throw new ArgumentNullException(nameof(refreshToken));


            // 一个 subject 只能有一个 refresh token
            var tokens = await this.RefreshTokenService.RemoveRefreshTokensAsync(new RemoveRefreshTokensRequest
            {
                ClientId = refreshToken.ClientId,
                SubjectId = refreshToken.SubjectId
            });

            foreach (var token in tokens)
            {
                await _cacheHelper.RemoveCacheAsync(CacheCategory, token);
            }

            //await RemoveRefreshTokensAsync(refreshToken.SubjectId, refreshToken.ClientId);

            var jsonData = JsonSerializer.Serialize(refreshToken, Helpers.GetDefaultJsonSerializerSettings());
            var handle = CryptoRandom.CreateUniqueId(32, CryptoRandom.OutputFormat.Hex);

            var dataObj = new CreateRefreshTokenRequest
            {
                Token = new Services.Dto.RefreshTokenDto
                {
                    ClientId = refreshToken.ClientId,
                    Data = jsonData,
                    Handle = handle,
                    SubjectId = refreshToken.SubjectId,
                    CreationTime = refreshToken.CreationTime,
                    ExpiresIn = refreshToken.Lifetime,
                    SessionId = (from x in refreshToken.AccessToken.Claims
                           where x.Type == JwtClaimTypes.SessionId
                           select x.Value).SingleOrDefault() ?? ""
                }
            };

            await this.RefreshTokenService.CreateRefreshTokenAsync(dataObj);

            await _cacheHelper.SetCacheValueAsync(CacheCategory, handle, jsonData, TimeSpan.FromSeconds(refreshToken.Lifetime));


            return handle;
        }

        public async Task UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken)
        {
            if (string.IsNullOrWhiteSpace(handle)) throw new ArgumentNullException(nameof(handle));
            if (refreshToken == null) throw new ArgumentNullException(nameof(refreshToken));

            var jsonData = JsonSerializer.Serialize(refreshToken, Helpers.GetDefaultJsonSerializerSettings());

            await this.RefreshTokenService.UpdateRefreshTokenAsync(new UpdateRefreshTokenRequest
            {
                RefreshToken = new RefreshTokenDto
                {
                    ClientId = refreshToken.ClientId,
                    Data = jsonData,
                    Handle = handle,
                    SubjectId = refreshToken.SubjectId,
                    CreationTime = refreshToken.CreationTime,
                    ExpiresIn = refreshToken.Lifetime,
                }
            });

            await _cacheHelper.SetCacheValueAsync(CacheCategory, handle, jsonData, TimeSpan.FromSeconds(refreshToken.Lifetime));
        }
    }
}
