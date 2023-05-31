// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Repositories;
using IdentityServer4.Storage.FreeSql.Repositories.Impl;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using IdentityServer4.Storage.FreeSql.Types;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class IdentityResourceServiceImpl : IDbIdentityResourceService
    {
        private readonly IIdentityResourceRepository _identityResourceRepository;
        private readonly IMapper _mapper;

        private readonly IApiScopeRepository _apiScopeRepository;

        public IdentityResourceServiceImpl(IIdentityResourceRepository identityResourceRepository,
            IMapper mapper,
            IApiScopeRepository apiScopeRepository)

        {
            _identityResourceRepository = identityResourceRepository;
            _mapper = mapper;
            _apiScopeRepository = apiScopeRepository;
        }


        public async Task<long> CreateIdentityResourceAsync(CreateIdentityResourceRequest request)
        {
            // check parameters
            if (request == null) throw new ArgumentNullException(nameof(request));

            var info = request.Resource;
            if (info == null) throw new ArgumentNullException(nameof(info));


            if (await _identityResourceRepository.SameNameExistedAsync(request.Resource.Name, 0L))
            {
                throw new BllException(400, $"Same identity resource name {request.Resource.Name} already existed");
            }

            // Resource name can't be same as api scope name
            if (await _apiScopeRepository.SameNameExistedAsync(request.Resource.Name, 0L))
            {
                throw new BllException(400, $"Same api scope name {request.Resource.Name} already existed");
            }

            var resource = new MIdentityResource
            {
                Name = info.Name,
                DisplayName = info.DisplayName,
                Required = info.Required,
                Emphasize = info.Emphasize,
                Enabled = info.Enabled,
                CreatedBy = request.Operator,
                CreateTime = DateTime.Now
            };

            resource.Claims = info.Claims.ToList();

            var id = await _identityResourceRepository.CreateIdentityResourceAsync(resource);

            return id;
        }

        public async Task<IEnumerable<IdentityResourceDto>> FindIdentityResourcesByScopeNameAsync(FindIdentityResourcesByScopeNameRequest request)
        {
            // check parameters
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.ScopeNames?.Any() == true)
            {
                var result = await _identityResourceRepository.FindIdentityResourcesByScopeNameAsync(request.ScopeNames.ToList());

                return result.Select(p => _mapper.Map<IdentityResourceDto>(p));
            }

            return new List<IdentityResourceDto>();

        }

        public async Task<IEnumerable<IdentityResourceDto>> GetAllIdentityResourcesAsync()
        {
            var list = await _identityResourceRepository.GetAllEnabledIdentityResourcesAsync();

            return list.Select(p => _mapper.Map<IdentityResourceDto>(p)).ToList();
        }


        public async Task<IdentityResourceDto> GetIdentityResourceByIdAsync(GetIdentityResourceByIdRequest request)
        {
            // check parameters
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0) throw new ArgumentOutOfRangeException(nameof(request.Id));

            var result = await _identityResourceRepository.GetIdentityResourceByIdAsync(request.Id);

            return _mapper.Map<IdentityResourceDto>(result);
        }

        public async Task<PagedResult<IdentityResourceDto>> QueryIdentityResourcesAsync(QueryIdentityResourcesRequest request)
        {
            // check parameters
            if (request == null) throw new ArgumentNullException(nameof(request));

            var pageIndex = request.PageIndex;
            var pageSize = request.PageSize;

            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            var queryResult = await _identityResourceRepository.QueryIdentityResourcesAsync(request.Keyword,
                request.Enabled,
                request.PageIndex,
                request.PageSize);

            var outResult = new PagedResult<IdentityResourceDto>(
                queryResult.TotalCount
,
                queryResult.Items.Select(p => _mapper.Map<IdentityResourceDto>(p)).ToList());

            return outResult;
        }

        public async Task RemoveIdentityResourceAsync(RemoveIdentityResourceRequest request)
        {
            // check parameters
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.Id > 0)
            {
                await _identityResourceRepository.RemoveIdentityResourceAsync(request.Id);
            }
        }

        public async Task UpdateIdentityResourceAsync(UpdateIdentityResourceRequest request)
        {
            // check parameters
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Resource == null) throw new ArgumentNullException(nameof(request.Resource));

            if (request.Resource.Id <= 0) throw new BllException(400, "Resource id is required");

            // 检查名称是否存在，若存在，则抛出 LrdException
            if (await _identityResourceRepository.SameNameExistedAsync(request.Resource.Name, request.Resource.Id))
            {
                throw new BllException(400, $"Resource name {request.Resource.Name} already existed");
            }

            // Resource name can't be same as api scope name
            if (await _apiScopeRepository.SameNameExistedAsync(request.Resource.Name, 0L))
            {
                throw new BllException(400, $"Same api scope name {request.Resource.Name} already existed");
            }

            var info = request.Resource;
            var resource = new MIdentityResource
            {
                Id = info.Id,
                Name = info.Name,
                DisplayName = info.DisplayName,
                Required = info.Required,
                Emphasize = info.Emphasize,
                Enabled = info.Enabled,
                UpdateBy = request.Operator,
                UpdateTime = DateTime.Now
            };

            resource.Claims = info.Claims.ToList();

            await _identityResourceRepository.UpdateIdentityResourceAsync(resource);
        }
    }
}
