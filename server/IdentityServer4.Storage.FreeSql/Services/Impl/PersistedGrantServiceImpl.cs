// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Storage.FreeSql.Entities;
using Laiye.SaasUC.Service.UserModule.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Impl
{
    internal class PersistedGrantServiceImpl : IDbPersistedGrantService
    {
        private IPersistedGrantRepository _persistedGrantRepository;
        private IMapper _mapper;

        public PersistedGrantServiceImpl(IPersistedGrantRepository persistedGrantRepository, IMapper mapper)
        {
            _persistedGrantRepository = persistedGrantRepository;
            _mapper = mapper;
        }

        public async Task<PersistedGrantDto> GetByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            var entity = await _persistedGrantRepository.GetByKeyAsync(key);

            return _mapper.Map<PersistedGrantDto>(entity);
        }

        public async Task<IEnumerable<PersistedGrantDto>> GetListByFilterAsync(string subjectId, string sessionId, string clientId, string type)
        {
            var list = await _persistedGrantRepository.GetPersistedGrantsAsync(subjectId, sessionId, clientId, type);

            return list.Select(_mapper.Map<PersistedGrantDto>);
        }

        public async Task RemoveAllByFilterAsync(string subjectId, string sessionId, string clientId, string type)
        {
            await _persistedGrantRepository.DeleteListAsync(subjectId, sessionId, clientId, type);
        }

        public async Task RemoveByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            await _persistedGrantRepository.DeleteByKeyAsync(key);
        }

        public async Task StoreGrantAsync(PersistedGrantDto grant)
        {
            if (grant == null) throw new ArgumentNullException(nameof(grant));

            var entity = _mapper.Map<MPersistedGrant>(grant);

            await _persistedGrantRepository.StorePersistedGrantAsync(entity);
        }
    }
}
