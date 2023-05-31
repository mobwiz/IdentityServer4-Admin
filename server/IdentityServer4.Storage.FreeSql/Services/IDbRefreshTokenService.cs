// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services
{
    public interface IDbRefreshTokenService
    {
        Task<long> CreateRefreshTokenAsync(CreateRefreshTokenRequest request);

        //Task UpdateRefreshTokenAsync(UpdateRefreshTokenRequest request);

        Task RemoveRefreshTokenAsync(RemoveRefreshTokenRequest request);

        Task<PagedResult<RefreshTokenDto>> QueryRefreshTokensAsync(QueryRefreshTokensRequest request);

        Task<RefreshTokenDto> GetRefreshTokenByHandleAsync(GetRefreshTokenByHandleRequest request);

        Task RemoveRefreshTokenByHandleAsync(RemoveRefreshTokenByHandleRequest request);

        Task<IEnumerable<string>> RemoveRefreshTokensAsync(RemoveRefreshTokensRequest request);
        Task UpdateRefreshTokenAsync(UpdateRefreshTokenRequest updateRefreshTokenRequest);
    }


}
