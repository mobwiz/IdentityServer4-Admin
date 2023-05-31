// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using IdentityServer4.Validation;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class RefreshTokenServiceImpl : IDbRefreshTokenService
    {
        private IRefreshTokenRepository _refreshTokenRepository;
        private IMapper _mapper;

        public RefreshTokenServiceImpl(IRefreshTokenRepository refreshTokenRepository, IMapper mapper)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _mapper = mapper;
        }

        public async Task<long> CreateRefreshTokenAsync(CreateRefreshTokenRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var mtoken = _mapper.Map<MRefreshToken>(request.Token);

            await _refreshTokenRepository.StoreRefreshTokenAsync(mtoken);

            return mtoken.Id;
        }

        public async Task<RefreshTokenDto> GetRefreshTokenByHandleAsync(GetRefreshTokenByHandleRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var obj = await _refreshTokenRepository.GetByHandleAsync(request.Handle);

            return _mapper.Map<RefreshTokenDto>(obj);
        }

        public async Task<PagedResult<RefreshTokenDto>> QueryRefreshTokensAsync(QueryRefreshTokensRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = await _refreshTokenRepository.QueryRefreshTokens(
                request.ClientId,
                request.SubjectId,
                request.SessionId,
                request.Handle, request.PageIndex, request.PageSize
                );

            return new PagedResult<RefreshTokenDto>
            {
                Items = result.Items.Select(_mapper.Map<RefreshTokenDto>),
                TotalCount = result.TotalCount
            };
        }

        public async Task RemoveRefreshTokenAsync(RemoveRefreshTokenRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveRefreshTokenByHandleAsync(RemoveRefreshTokenByHandleRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            await _refreshTokenRepository.DeleteByHandleAsync(request.Handle);
        }

        public async Task<IEnumerable<string>> RemoveRefreshTokensAsync(RemoveRefreshTokensRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = await _refreshTokenRepository.DeleteListAsync(request.ClientId, request.SubjectId, request.SessionId);

            return result;
        }

        public Task UpdateRefreshTokenAsync(UpdateRefreshTokenRequest updateRefreshTokenRequest)
        {
            throw new NotImplementedException();
        }
    }
}
