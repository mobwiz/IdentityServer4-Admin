// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services
{
    /// <summary>
    /// Interface for api resoruce service
    /// </summary>
    public interface IDbApiResourceService
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<long> CreateApiResourceAsync(CreateApiResourceRequest request);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateApiResourceAsync(UpdateApiResourceRequest request);

        /// <summary>
        /// Remove by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task RemoveApiResourceAsync(RemoveApiResourceRequest request);

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResourceDto> GetApiResourceByIdAsync(GetApiResourceByIdRequest request);

        /// <summary>
        /// query api resources
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagedResult<ApiResourceDto>> QueryApiResourcesAsync(QueryApiResourcesRequest request);

        /// <summary>
        /// find by scope names
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IEnumerable<ApiResourceDto>> FindApiResourcesByScopeNamesAsync(FindApiResourcesByScopeNameRequest request);

        /// <summary>
        /// find by names
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IEnumerable<ApiResourceDto>> FindApiResourcesByNamesAsync(FindApiResourcesByNamesRequest request);

        /// <summary>
        /// get all
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ApiResourceDto>> GetAllEnabledApiResources();
    }
}
