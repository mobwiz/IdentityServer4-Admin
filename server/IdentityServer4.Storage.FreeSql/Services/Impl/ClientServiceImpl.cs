// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class ClientServiceImpl : IDbClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientServiceImpl(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }


        public async Task<long> CreateClientAsync(CreateClientRequest request)
        {
            // check input parameter
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Client == null) throw new ArgumentNullException(nameof(request.Client));

            var clientInfo = request.Client;

            var clientExisted = await _clientRepository.GetClientByClientIdAsync(clientInfo.ClientId);
            if (clientExisted != null) throw new BllException(400, "Client id {0} already existed", clientInfo.ClientId);

            var mclient = _mapper.Map<MClient>(request.Client);
            mclient.AllowedGrantTypes = clientInfo.AllowedGrantTypes;
            mclient.RedirectUris = clientInfo.RedirectUris;
            mclient.PostLogoutRedirectUris = clientInfo.PostLogoutRedirectUris;
            mclient.AllowedCorsOrigins = clientInfo.AllowedCorsOrigins;
            mclient.ClientSecrets = clientInfo.ClientSecrets;
            mclient.AllowedScopes = clientInfo.AllowedScopes;

            mclient.CreatedBy = request.Operator;
            mclient.CreateTime = DateTime.Now;
            mclient.UpdateBy = "";
            mclient.UpdateTime = DateTime.MinValue;

            // 这里并没有保存 claims 和 claims 的值

            var result = await _clientRepository.CreateClientAsync(mclient);

            return result;
        }

        public async Task<ClientDto> GetClientByClientIdAsync(GetClientByClientIdRequest request)
        {
            // clientId can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.ClientId)) throw new ArgumentNullException(nameof(request.ClientId));

            var mclient = await _clientRepository.GetClientByClientIdAsync(request.ClientId);

            return _mapper.Map<ClientDto>(mclient);
        }

        public async Task<ClientDto> GetClientByIdAsync(GetClientByIdRequest request)
        {
            // clientId can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var mclient = await _clientRepository.GetClientByIdAsync(request.Id);

            return _mapper.Map<ClientDto>(mclient);
        }

        public async Task<IEnumerable<string>> GetCorsOriginsAsync()
        {
            var list = (await _clientRepository.GetAllowedCorsOriginsAsync()).Distinct();

            return list.ToList();
        }

        public async Task<IEnumerable<string>> GetValidPostLogoutUrisAsync()
        {
            return (await _clientRepository.GetAllowedPostLogoutUrisAsync()).Distinct();
        }

        public async Task<IEnumerable<string>> GetValidRedirectUrisAsync()
        {
            return (await _clientRepository.GetAllowedRedirectUrisAsync()).Distinct();
        }

        public async Task<PagedResult<ClientDto>> QueryClientsAsync(QueryClientsRequest request)
        {
            // request can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));
            // request.PageIndex must be greater than 0
            if (request.PageIndex <= 0) throw new ArgumentException("PageIndex must be greater than 0", nameof(request.PageIndex));
            // request.Pagesize must be greater than 0
            if (request.PageSize <= 0) throw new ArgumentException("PageSize must be greater than 0", nameof(request.PageSize));

            var queryResult = await _clientRepository.QueryClientsAsync(new ClientQueryParameter
            {
                Enabled = request.Enabled,
                Keyword = request.Keyword,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
            });

            return new PagedResult<ClientDto>
            {
                TotalCount = queryResult.TotalCount,
                Items = queryResult.Items.Select(p => _mapper.Map<ClientDto>(p)).ToArray()
            };
        }

        public async Task RemoveClientAsync(RemoveClientRequest request)
        {
            // request can't be null
            if (request == null) throw new ArgumentNullException(nameof(request));
            // request.Id must be greater thant 0
            if (request.Id <= 0) throw new ArgumentOutOfRangeException(nameof(request.Id));

            await _clientRepository.RemoveClientAsync(request.Id);
        }

        public async Task UpdateClientAsync(UpdateClientRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var clientInfo = request.Client;

            if (clientInfo == null) throw new ArgumentNullException(nameof(request.Client));

            if (clientInfo.Id <= 0) throw new ArgumentNullException(nameof(request.Client.Id));
            if (string.IsNullOrWhiteSpace(clientInfo.ClientId)) throw new ArgumentNullException(nameof(request.Client.ClientId));

            var client = await this._clientRepository.GetClientByClientIdAsync(clientInfo.ClientId);

            if (client != null && client.Id != clientInfo.Id)
            {
                throw new BllException(400, "Client with id {0} already existed", client.ClientId);
            }

            var mclient = _mapper.Map<MClient>(request.Client);

            mclient.AllowedGrantTypes = clientInfo.AllowedGrantTypes;
            mclient.RedirectUris = clientInfo.RedirectUris;
            mclient.PostLogoutRedirectUris = clientInfo.PostLogoutRedirectUris;
            mclient.AllowedCorsOrigins = clientInfo.AllowedCorsOrigins;
            mclient.ClientSecrets = clientInfo.ClientSecrets;
            mclient.AllowedScopes = clientInfo.AllowedScopes;

            mclient.UpdateBy = request.Operator;
            mclient.UpdateTime = DateTime.Now;

            await _clientRepository.UpdateClientAsync(mclient);
        }


        private ClientDto GetClientInfoByMClient(MClient mclient)
        {
            if (mclient == null) return null;

            var obj = _mapper.Map<ClientDto>(mclient);

            return obj;
        }
    }
}
