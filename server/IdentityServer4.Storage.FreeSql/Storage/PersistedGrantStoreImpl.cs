// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Stores;

namespace IdentityServer4.Storage.FreeSql.Storage
{
    internal class PersistedGrantStoreImpl : IPersistedGrantStore
    {
        private IDbPersistedGrantService PersistedGrantService { get; }
        private IMapper Mapper { get; }

        public PersistedGrantStoreImpl(IDbPersistedGrantService persistedGrantService)
        {
            PersistedGrantService = persistedGrantService;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var list = await PersistedGrantService.GetListByFilterAsync(filter.SubjectId, filter.SessionId, filter.ClientId, filter.Type);

            return list.Select(p => Mapper.Map<PersistedGrant>(p));
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var obj = await PersistedGrantService.GetByKeyAsync(key);
            if (obj != null)
            {
                return Mapper.Map<PersistedGrant>(obj);
            }

            return null;
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            await PersistedGrantService.RemoveAllByFilterAsync(filter.SubjectId, filter.SessionId, filter.ClientId, filter.Type);
        }

        public async Task RemoveAsync(string key)
        {
            await PersistedGrantService.RemoveByKeyAsync(key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await this.PersistedGrantService.StoreGrantAsync(Mapper.Map<PersistedGrantDto>(grant));
        }
    }
}
