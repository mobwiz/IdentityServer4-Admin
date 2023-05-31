// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services
{
    public interface IDbIdentityResourceService
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<long> CreateIdentityResourceAsync(CreateIdentityResourceRequest request);

        /// <summary>
        /// remove 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task RemoveIdentityResourceAsync(RemoveIdentityResourceRequest request);

        /// <summary>
        /// upate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateIdentityResourceAsync(UpdateIdentityResourceRequest request);

        /// <summary>
        /// query
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagedResult<IdentityResourceDto>> QueryIdentityResourcesAsync(QueryIdentityResourcesRequest request);

        /// <summary>
        /// find by name    
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IEnumerable<IdentityResourceDto>> FindIdentityResourcesByScopeNameAsync(FindIdentityResourcesByScopeNameRequest request);

        /// <summary>
        /// get all enabled
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IdentityResourceDto>> GetAllIdentityResourcesAsync();


        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IdentityResourceDto> GetIdentityResourceByIdAsync(GetIdentityResourceByIdRequest request);
    }
}
