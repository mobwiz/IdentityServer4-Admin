// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class ReferenceTokenServiceImpl : IDbReferenceTokenService
    {
        private IReferenceTokenRepository _referenceTokenRepository;
        private IMapper _mapper;

        public ReferenceTokenServiceImpl(IMapper mapper, IReferenceTokenRepository referenceTokenRepository)
        {
            _mapper = mapper;
            _referenceTokenRepository = referenceTokenRepository;
        }

        public async Task<long> CreateReferenceTokenAsync(CreateReferenceTokenRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var entity = _mapper.Map<Entities.MReferenceToken>(request.Token);

            await _referenceTokenRepository.StoreReferenceTokenAsync(entity);

            return entity.Id;
        }

        public async Task<ReferenceTokenDto> GetReferenceTokenByHandleAsync(GetReferenceTokenByHandleRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var entity = await _referenceTokenRepository.GetByHandleAsync(request.Handle);

            return _mapper.Map<ReferenceTokenDto>(entity);
        }

        public async Task<PagedResult<ReferenceTokenDto>> QueryReferenceTokensAsync(QueryReferenceTokensRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = await _referenceTokenRepository.QueryTokensAsync(
                request.ClientId,
                request.SubjectId,
                request.SessionId,
                request.Handle,
                request.PageIndex,
                request.PageSize
                );

            return new PagedResult<ReferenceTokenDto>
            {
                Items = result.Items.Select(_mapper.Map<ReferenceTokenDto>),
                TotalCount = result.TotalCount,
            };
        }

        public async Task RemoveReferenceTokenAsync(RemoveReferenceTokenRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            throw new NotImplementedException();
        }

        public async Task RemoveReferenceTokenByHandleAsync(RemoveReferenceTokenByHandleRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            await _referenceTokenRepository.DeleteByHandleAsync(request.Handle);
        }

        public async Task<IEnumerable<string>> RemoveReferenceTokensAsync(RemoveReferenceTokensRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = await _referenceTokenRepository.DeleteListAsync(request.SubjectId, request.ClientId, request.SessionId);

            return result;
        }
    }
}
