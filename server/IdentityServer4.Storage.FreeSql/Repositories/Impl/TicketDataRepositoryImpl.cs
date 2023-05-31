// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class TicketDataRepositoryImpl : InnerBaseRepository, ITicketDataRepository
    {
        public TicketDataRepositoryImpl(DatabaseConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        public async Task CreateTicketDataAsync(MTicketData data)
        {
            // 检查参数
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MTicketData>();

            repos.Insert(data);
        }

        public async Task<IEnumerable<MTicketData>> GetExpiredTicketDataAsync(int batchSize)
        {
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MTicketData>();

            var list = await repos.Select.Where(p => p.ExpireTime < DateTime.UtcNow).Take(batchSize).ToListAsync();
            return list;
        }

        public async Task<MTicketData> GetTicketDataByKeyAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            using var conn = GetConnection();
            using var repos = conn.GetRepository<MTicketData>();

            return await repos.Select.Where(p => p.Key == key).FirstAsync();
        }

        public async Task RemoveTicketDataByKeyAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            using var conn = GetConnection();
            using var repos = conn.GetRepository<MTicketData>();

            var obj = await repos.Select.Where(p => p.Key == key).FirstAsync();
            if (obj != null)
            {
                await repos.DeleteAsync(obj);
            }
        }

        public async Task RemoveTicketDataByUserIdAsync(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
                throw new ArgumentOutOfRangeException(nameof(subjectId));

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MTicketData>();

            var obj = await repos.Select.Where(p => p.SubjectId == subjectId).FirstAsync();
            if (obj != null)
            {
                await repos.DeleteAsync(obj);
            }
        }

        public async Task UpdateTicketDataAsync(MTicketData data)
        {
            // 检查参数
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MTicketData>();
            await repos.UpdateAsync(data);
        }
    }
}