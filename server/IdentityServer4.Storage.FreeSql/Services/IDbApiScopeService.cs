// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services
{
    /// <summary>
    /// interface for api scope service
    /// </summary>
    public interface IDbApiScopeService
    {
        /// <summary>
        /// Create api scope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<long> CreateApiScopeAsync(CreateApiScopeRequest request);

        /// <summary>
        /// update api scope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateApiScopeAsync(UpdateApiScopeRequest request);

        /// <summary>
        /// remove api scope by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task RemoveApiScopeAsync(RemoveApiScopeRequest request);

        /// <summary>
        /// query all scope for manage
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagedResult<ApiScopeDto>> QueryApiScopesAsync(QueryApiScopesRequest request);

        /// <summary>
        /// find by scope names
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IEnumerable<ApiScopeDto>> FindApiScopesByNameAsync(FindApiScopesByNameRequest request);

        /// <summary>
        /// get all
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ApiScopeDto>> GetAllApiScopesAsync();

        /// <summary>
        /// get single
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ApiScopeDto> GetApiScopeByIdAsync(GetApiScopeByIdRequest request);
    }
}
