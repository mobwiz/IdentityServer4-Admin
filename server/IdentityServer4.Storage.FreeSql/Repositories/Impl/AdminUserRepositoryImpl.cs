// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Validation;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class AdminUserRepositoryImpl : InnerBaseRepository, IAdminUserRepository
    {
        public AdminUserRepositoryImpl(DatabaseConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        public async Task<long> CreateAdminUserAsync(MAdminUser adminUser)
        {
            // adminUser can't be null
            if (adminUser == null) throw new ArgumentNullException(nameof(adminUser));

            using var conn = GetConnection();
            var repos = conn.GetRepository<MAdminUser>();

            await repos.InsertAsync(adminUser);

            return adminUser.Id;

        }

        public async Task<MAdminUser> GetAdminUserByIdAsync(long id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            using var conn = GetConnection();
            var repos = conn.GetRepository<MAdminUser>();

            return await repos.Select.Where(x => x.Id == id).ToOneAsync();
        }

        public async Task<MAdminUser> GetAdminUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

            using var conn = GetConnection();
            var repos = conn.GetRepository<MAdminUser>();

            return await repos.Select.Where(x => x.Username == username).ToOneAsync();
        }

        public async Task<PagedResult<MAdminUser>> QueryAdminUsersAsync(string keyword, int page, int pageSize)
        {
            using var conn = GetConnection();
            var repos = conn.GetRepository<MAdminUser>();
            var query = repos.Select;
            query = query.WhereIf(!string.IsNullOrWhiteSpace(keyword), x => x.Username.Contains(keyword));

            var total = query.Count();

            var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<MAdminUser>
            {
                Items = list,
                TotalCount = total
            };
        }

        public async Task RemoveAdminUserAsync(long id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            using var conn = GetConnection();
            var repos = conn.GetRepository<MAdminUser>();

            var obj = await repos.Select.Where(x => x.Id == id).FirstAsync();

            if (obj != null)
            {
                await repos.DeleteAsync(obj);
            }
        }

        public async Task UpdateAdminUserAsync(MAdminUser adminUser)
        {
            if (adminUser == null) throw new ArgumentNullException(nameof(adminUser));

            using var conn = GetConnection();
            var repos = conn.GetRepository<MAdminUser>();

            await repos.UpdateAsync(adminUser);
        }
    }
}
