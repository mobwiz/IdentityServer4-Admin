// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using Laiye.SaasUC.Service.UserModule.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class PersistedGrantRepositoryImpl : InnerBaseRepository, IPersistedGrantRepository
    {
        private const int MaxItemsToList = 1000;

        public PersistedGrantRepositoryImpl(DatabaseConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        public async Task CleanExpiredDataAsync()
        {
            using var conn = GetConnection();
            using var repos = conn.GetRepository<MPersistedGrant>();

            int batchSize = 1000;

            var query = await repos.Select.Where(p => p.Expiration < DateTime.UtcNow).Take(batchSize).ToListAsync();

            while (query.Any())
            {
                foreach (var item in query)
                {
                    await repos.DeleteAsync(item);
                }

                query = await repos.Select.Where(p => p.Expiration < DateTime.UtcNow).Take(batchSize).ToListAsync();
            }
        }

        public async Task DeleteByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException(nameof(key));

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MPersistedGrant>();

            await repos.DeleteAsync(p => p.Key == key);
        }

        public async Task DeleteListAsync(string subjectId, string clientId, string sessionId, string type)
        {
            // check parameter can't be all null
            if (string.IsNullOrEmpty(subjectId) && string.IsNullOrEmpty(clientId) && string.IsNullOrEmpty(sessionId) && string.IsNullOrEmpty(type))
                throw new ArgumentNullException("parameters can't be all null.");

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MPersistedGrant>();

            var query = repos.Select.Take(MaxItemsToList);

            query = query.WhereIf(!string.IsNullOrWhiteSpace(subjectId), x => x.SubjectId == subjectId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(clientId), x => x.ClientId == clientId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(sessionId), x => x.SessionId == sessionId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(type), x => x.Type == type);

            var list = await query.ToListAsync();
            foreach (var item in list)
            {
                await repos.DeleteAsync(item);
            }
        }

        public Task<MPersistedGrant> GetByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException(nameof(key));

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MPersistedGrant>();

            return repos.Select.Where(x => x.Key == key).FirstAsync();
        }

        public async Task<List<MPersistedGrant>> GetPersistedGrantsAsync(string subjectId, string clientId, string sessionId, string type)
        {
            if (string.IsNullOrEmpty(subjectId) && string.IsNullOrEmpty(clientId) && string.IsNullOrEmpty(sessionId) && string.IsNullOrEmpty(type))
                throw new ArgumentNullException("parameters can't be all null.");

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MPersistedGrant>();

            var query = repos.Select;

            query = query.WhereIf(!string.IsNullOrWhiteSpace(subjectId), x => x.SubjectId == subjectId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(clientId), x => x.ClientId == clientId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(sessionId), x => x.SessionId == sessionId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(type), x => x.Type == type);

            return await query.Take(MaxItemsToList).ToListAsync();
        }

        public async Task StorePersistedGrantAsync(MPersistedGrant persistedGrant)
        {
            if (persistedGrant == null) throw new ArgumentNullException();

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MPersistedGrant>();

            await repos.InsertOrUpdateAsync(persistedGrant);
        }
    }
}
