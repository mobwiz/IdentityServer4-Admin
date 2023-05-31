// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FreeSql;
using IdentityServer4.Storage.FreeSql.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class KeyValueRepositoryImpl : InnerBaseRepository, IKeyValueRepository
    {
        public KeyValueRepositoryImpl(DatabaseConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public async Task<MKeyValue> GetItemAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            using var conn = GetConnection();

            using var repos = conn.GetRepository<MKeyValue>();

            return await repos.Select.Where(x => x.Key == key).ToOneAsync();
        }

        public async Task RemoveItemAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MKeyValue>();

            await repos.DeleteAsync(x => x.Key == key);
        }

        public async Task CreateItemAsync(MKeyValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MKeyValue>();

            await repos.InsertAsync(item);
        }

        public async Task UpdateItemAsync(MKeyValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using var conn = GetConnection();
            using var repos = conn.GetRepository<MKeyValue>();

            await repos.UpdateAsync(item);
        }
    }
}
