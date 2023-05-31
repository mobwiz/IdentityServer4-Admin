// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql;
using IdentityServer4.Storage.FreeSql.Entities;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class ReferenceTokenRepository : InnerBaseRepository, IReferenceTokenRepository
    {

        public ReferenceTokenRepository(DatabaseConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        public async Task CleanExpiredDataAsync()
        {
            using var conn = GetConnection();
            using var repos = conn.GetRepository<MReferenceToken>();

            int batchSize = 1000;

            var query = await repos.Select.Where(p => p.ExpireTime < DateTime.UtcNow).Take(batchSize).ToListAsync();

            while (query.Any())
            {
                foreach (var item in query)
                {
                    await repos.DeleteAsync(item);
                }

                query = await repos.Select.Where(p => p.ExpireTime < DateTime.UtcNow).Take(batchSize).ToListAsync();
            }
        }

        public async Task DeleteByHandleAsync(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle)) throw new ArgumentNullException(nameof(handle));

            using var repos = GetRepository();

            await repos.DeleteAsync(p => p.Handle == handle);
        }

        public async Task DeleteByHandleWithDelayAsync(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle)) throw new ArgumentNullException(nameof(handle));

            using var repos = GetRepository();

            await Task.Delay(1000);

            await repos.DeleteAsync(p => p.Handle == handle);
        }

        public async Task<IList<string>> DeleteListAsync(string subjectId, string clientId, string sid)
        {
            // check the parameters, can't be all null
            if (string.IsNullOrWhiteSpace(subjectId) && string.IsNullOrWhiteSpace(clientId) && string.IsNullOrWhiteSpace(sid))
                throw new ArgumentNullException("parameters can't be all null.");

            using var repos = GetRepository();

            var query = repos.Select;
            query = query.WhereIf(!string.IsNullOrWhiteSpace(subjectId), p => p.SubjectId == subjectId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(clientId), p => p.ClientId == clientId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(sid), p => p.SessionId == sid);
            var list = await query.ToListAsync();

            var tokens = list.Select(p => p.Handle).ToList();

            foreach (var item in list)
            {
                await repos.DeleteAsync(item);
            }

            return tokens;
        }

        public async Task<MReferenceToken> GetByHandleAsync(string handle)
        {
            // check the parameters
            if (string.IsNullOrWhiteSpace(handle)) throw new ArgumentNullException(nameof(handle));

            using var repos = GetRepository();

            return await repos.Select.Where(p => p.Handle == handle).ToOneAsync();
        }

        public async Task<IList<string>> GetReferenceTokensAsync(string subjectId)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(subjectId)) throw new ArgumentNullException(nameof(subjectId));

            using var repos = GetRepository();

            var query = repos.Select.Where(p => p.SubjectId == subjectId);

            return await query.ToListAsync(x => x.Handle);
        }

        public async Task<PagedResult<MReferenceToken>> QueryTokensAsync(string clientId, string subjectId, string sessionId, string handle, int pageIndex, int pageSize)
        {
            using var repos = GetRepository();

            var query = repos.Select;
            query = query.WhereIf(!string.IsNullOrWhiteSpace(clientId), p => p.ClientId == clientId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(subjectId), p => p.SubjectId == subjectId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(sessionId), p => p.SessionId == sessionId);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(handle), p => p.Handle == handle);

            var count = await query.CountAsync();
            var result = await query.OrderByDescending(p => p.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<MReferenceToken> { Items = result, TotalCount = count };
        }

        public async Task RemoveReferenceTokenBySubAsync(string subjectId)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(subjectId)) throw new ArgumentNullException(nameof(subjectId));

            using var repos = GetRepository();
            await repos.DeleteAsync(p => p.SubjectId == subjectId);
        }

        public async Task<long> StoreReferenceTokenAsync(MReferenceToken referenceToken)
        {
            // check parameters
            if (referenceToken == null) throw new ArgumentNullException(nameof(referenceToken));

            using var repos = GetRepository();

            var token = await repos.InsertOrUpdateAsync(referenceToken);
            return token.Id;
        }

        private IBaseRepository<MReferenceToken> GetRepository()
        {
            using var conn = GetConnection();
            return conn.GetRepository<MReferenceToken>();
        }
    }
}
