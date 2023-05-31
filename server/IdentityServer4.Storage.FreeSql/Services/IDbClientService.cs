// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;

namespace IdentityServer4.Storage.FreeSql.Services
{
    /// <summary>
    /// Client Service
    /// </summary>
    public interface IDbClientService
    {
        /// <summary>
        /// Create a client
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<long> CreateClientAsync(CreateClientRequest request);

        /// <summary>
        /// update a client
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateClientAsync(UpdateClientRequest request);

        /// <summary>
        /// remove a client
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task RemoveClientAsync(RemoveClientRequest request);

        /// <summary>
        /// query clients
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagedResult<ClientDto>> QueryClientsAsync(QueryClientsRequest request);

        /// <summary>
        /// get by client_id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<ClientDto> GetClientByClientIdAsync(GetClientByClientIdRequest request);

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ClientDto> GetClientByIdAsync(GetClientByIdRequest request);

        /// <summary>
        /// get cors origins
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetCorsOriginsAsync();

        /// <summary>
        /// get valid redirect uris
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetValidRedirectUrisAsync();

        /// <summary>
        /// get valid post logout redirect uris
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetValidPostLogoutUrisAsync();
    }
}
