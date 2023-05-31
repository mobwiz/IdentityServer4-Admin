// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Repositories;
using IdentityServer4.Storage.FreeSql.Repositories.Impl;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class ApiScopeServiceImpl : IDbApiScopeService
    {
        private IApiScopeRepository _apiScopeRepository { get; }
        private IIdentityResourceRepository _identityResourceRepository { get; }
        private IMapper Mapper { get; }


        public ApiScopeServiceImpl(
            IMapper mapper,
            IApiScopeRepository apiScopeRepository,
            IIdentityResourceRepository identityResourceRepository)
        {
            Mapper = mapper;
            _apiScopeRepository = apiScopeRepository;
            _identityResourceRepository = identityResourceRepository;
        }

        public async Task<long> CreateApiScopeAsync(CreateApiScopeRequest request)
        {
            // 检查 request 是否为空
            if (request == null) throw new ArgumentNullException(nameof(request));

            // 检查 name 是否存在，若存在则抛出异常
            if (await _apiScopeRepository.SameNameExistedAsync(request.ApiScope.Name, 0L))
            {
                throw new BllException(400, $"Same api scope name: \"{request.ApiScope.Name}\" already exists.");
            }

            // identity 也不能有这个 resource anme
            if (await _identityResourceRepository.SameNameExistedAsync(request.ApiScope.Name, 0L))
            {
                throw new BllException(400, $"Same identity resource name: \"{request.ApiScope.Name}\" already exists.");
            }

            var scope = new MApiScope
            {
                Name = request.ApiScope.Name,
                DisplayName = request.ApiScope.DisplayName,
                Emphasize = request.ApiScope.Emphasize,
                Required = request.ApiScope.Required,
                Claims = request.ApiScope.Claims,
                Enabled = request.ApiScope.Enabled,
                CreatedBy = request.Operator,
                CreateTime = DateTime.Now,
                UpdateBy = "",
                UpdateTime = DateTime.MinValue
            };

            var id = await _apiScopeRepository.CreateApiScopeAsync(scope);

            return id;
        }

        public async Task<IEnumerable<ApiScopeDto>> FindApiScopesByNameAsync(FindApiScopesByNameRequest request)
        {
            // check the input parameters
            if (request == null) throw new ArgumentNullException(nameof(request));

            var scopes = request.Names.ToList();

            var list = await _apiScopeRepository.FindApiScopesByNameAsync(scopes);

            return list.Select(p => Mapper.Map<MApiScope, ApiScopeDto>(p));
        }

        public async Task<IEnumerable<ApiScopeDto>> GetAllApiScopesAsync()
        {
            var scopes = await _apiScopeRepository.GetAllApiScopesAsync();

            return scopes.Select(p => Mapper.Map<MApiScope, ApiScopeDto>(p)).ToList();
        }

        public async Task<PagedResult<ApiScopeDto>> QueryApiScopesAsync(QueryApiScopesRequest request)
        {
            // check  the input parameters
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.PageIndex <= 0) request.PageIndex = 1;
            if (request.PageSize <= 0) request.PageSize = 10;

            var queryResult = await _apiScopeRepository.QueryApiScopes(request.Keyword, request.Enabled, request.PageIndex, request.PageSize);

            var result = new PagedResult<ApiScopeDto>(
                queryResult.TotalCount,
                queryResult.Items.Select(p => Mapper.Map<MApiScope, ApiScopeDto>(p)));

            return result;
        }

        public async Task RemoveApiScopeAsync(RemoveApiScopeRequest request)
        {
            // check the input parameter
            if (request == null) throw new ArgumentNullException(nameof(request));

            await _apiScopeRepository.RemoveApiScopeAsync(request.Id);
        }

        public async Task UpdateApiScopeAsync(UpdateApiScopeRequest request)
        {
            // check the input parameter
            if (request == null) throw new ArgumentNullException(nameof(request));

            var scopeInfo = request.ApiScope;

            if (scopeInfo?.Id > 0)
            {
                if (await _apiScopeRepository.SameNameExistedAsync(request.ApiScope.Name, scopeInfo.Id))
                {
                    throw new BllException(400, $"Same api scope name: \"{request.ApiScope.Name}\" already exists.");
                }

                // identity 也不能有这个 resource anme
                if (await _identityResourceRepository.SameNameExistedAsync(request.ApiScope.Name, 0L))
                {
                    throw new BllException(400, $"Same identity resource name: \"{request.ApiScope.Name}\" already exists.");
                }

                var mapiScope = new MApiScope
                {
                    Id = scopeInfo.Id,
                    Claims = scopeInfo.Claims,
                    DisplayName = scopeInfo.DisplayName,
                    Emphasize = scopeInfo.Emphasize,
                    Name = scopeInfo.Name,
                    Required = scopeInfo.Required,
                    Enabled = scopeInfo.Enabled,
                    UpdateBy = request.Operator,
                    UpdateTime = DateTime.Now
                };

                await _apiScopeRepository.UpdateApiScopeAsync(mapiScope);
            }
        }

        public async Task<ApiScopeDto> GetApiScopeByIdAsync(GetApiScopeByIdRequest request)
        {
            // check the input parameter
            if (request == null) throw new ArgumentNullException(nameof(request));

            var scope = await _apiScopeRepository.GetApiScopeByIdAsync(request.Id);

            return Mapper.Map<ApiScopeDto>(scope);
        }
    }
}
