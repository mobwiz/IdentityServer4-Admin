// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services
{
    public interface IDbReferenceTokenService
    {
        Task<long> CreateReferenceTokenAsync(CreateReferenceTokenRequest request);

        //Task UpdateReferenceTokenAsync(UpdateReferenceTokenRequest request);

        Task RemoveReferenceTokenAsync(RemoveReferenceTokenRequest request);

        Task<PagedResult<ReferenceTokenDto>> QueryReferenceTokensAsync(QueryReferenceTokensRequest request);

        Task<ReferenceTokenDto> GetReferenceTokenByHandleAsync(GetReferenceTokenByHandleRequest request);

        Task RemoveReferenceTokenByHandleAsync(RemoveReferenceTokenByHandleRequest request);

        Task<IEnumerable<string>> RemoveReferenceTokensAsync(RemoveReferenceTokensRequest request);
    }


}
