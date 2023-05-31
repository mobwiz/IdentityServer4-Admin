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
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class ApiResourceServiceImpl : IDbApiResourceService
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IMapper _mapper;

        public ApiResourceServiceImpl(IApiResourceRepository apiResourceRepository, IMapper mapper)
        {
            _apiResourceRepository = apiResourceRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateApiResourceAsync(CreateApiResourceRequest request)
        {
            // 检查request 是否为null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var info = request.ApiResource;
            if (info == null) throw new ArgumentOutOfRangeException(nameof(info));

            if (await _apiResourceRepository.SameNameExistedAsync(request.ApiResource.Name, 0L))
            {
                throw new BllException(400, "Same api resource name already existed");
            }

            var mapi = new MApiResource
            {
                Name = info.Name,
                DisplayName = info.DisplayName,
                Enabled = info.Enabled,
                CreatedBy = request.Operator,
                CreateTime = DateTime.Now,
                UpdateBy = "",
                UpdateTime = DateTime.MinValue
            };

            mapi.Claims = info.Claims.ToList();
            mapi.Scopes = info.Scopes.ToList();
            mapi.Secrets = info.Secrets.ToList();

            return await _apiResourceRepository.CreateApiResourceAsync(mapi);
        }

        public async Task<IEnumerable<ApiResourceDto>> FindApiResourcesByNamesAsync(FindApiResourcesByNamesRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Names == null) throw new ArgumentNullException(nameof(request.Names));

            var list = await _apiResourceRepository.FindApiResourcesByNamesAsync(request.Names.ToList());

            return list.Select(p => _mapper.Map<ApiResourceDto>(p));
        }

        public async Task<IEnumerable<ApiResourceDto>> FindApiResourcesByScopeNamesAsync(FindApiResourcesByScopeNameRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.ScopeNames == null) throw new ArgumentNullException(nameof(request.ScopeNames));

            var names = request.ScopeNames.ToList();

            var list = await _apiResourceRepository.FindApiResourcesByScopeNameAsync(names);

            return list.Select(p => _mapper.Map<ApiResourceDto>(p));
        }

        public async Task<IEnumerable<ApiResourceDto>> GetAllEnabledApiResources()
        {
            var list = await _apiResourceRepository.GetAllEnabledApiResourcesAsync();

            return list.Select(p => _mapper.Map<ApiResourceDto>(p));
        }

        public async Task<ApiResourceDto> GetApiResourceByIdAsync(GetApiResourceByIdRequest request)
        {
            // 检查参数是否为null
            if (request == null) throw new ArgumentNullException(nameof(request));
            // 检查 requst.id 是否大于0，否则抛出 argumentoutofrange 异常
            if (request.Id <= 0) throw new ArgumentOutOfRangeException(nameof(request.Id));

            var res = await _apiResourceRepository.GetApiResourceByIdAsync(request.Id);

            return _mapper.Map<ApiResourceDto>(res);
        }

        public async Task<PagedResult<ApiResourceDto>> QueryApiResourcesAsync(QueryApiResourcesRequest request)
        {
            // 检查request 是否为null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var queryResult = await _apiResourceRepository.QueryApiResourcesAsync(request.Keyword, request.Enabled, request.PageIndex, request.PageSize);


            var result = new PagedResult<ApiResourceDto>
            {
                TotalCount = queryResult.TotalCount,
                Items = queryResult.Items.Select(p => _mapper.Map<ApiResourceDto>(p))
            };

            return result;
        }

        public async Task RemoveApiResourceAsync(RemoveApiResourceRequest request)
        {
            // 检查request 是否为null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var id = request.Id;
            if (id <= 0) throw new ArgumentOutOfRangeException($"id={id}");

            await _apiResourceRepository.RemoveApiResourceAsync(id);
        }

        public async Task UpdateApiResourceAsync(UpdateApiResourceRequest request)
        {
            // 检查request 是否为null
            if (request == null) throw new ArgumentNullException(nameof(request));

            var info = request.ApiResource;
            if (info == null) throw new ArgumentNullException(nameof(request.ApiResource));

            if (await _apiResourceRepository.SameNameExistedAsync(request.ApiResource.Name, info.Id))
            {
                throw new BllException(400, "Same api resource name already existed");
            }

            var mapi = new MApiResource
            {
                Id = info.Id,
                Name = info.Name,
                DisplayName = info.DisplayName,
                Enabled = info.Enabled,
                UpdateTime = DateTime.Now,
                UpdateBy = request.Operator
            };

            mapi.Claims = info.Claims.ToList();
            mapi.Scopes = info.Scopes.ToList();
            mapi.Secrets = info.Secrets.ToList();

            await _apiResourceRepository.UpdateApiResourceAsync(mapi);
        }
    }
}
