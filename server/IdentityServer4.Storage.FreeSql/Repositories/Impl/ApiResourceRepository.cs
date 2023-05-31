// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using IdentityServer4.Storage.FreeSql.Types;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class ApiResourceRepository : InnerBaseRepository, IApiResourceRepository
    {
        public ApiResourceRepository(DatabaseConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        public Task<long> CreateApiResourceAsync(MApiResource resource)
        {
            // check input parameter
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            var id = 0L;

            using var conn = GetConnection();
            conn.Transaction(() =>
            {
                var repos = conn.GetRepository<MApiResource>();
                repos.Insert(resource);

                var strRepos = new StringCollectionRepositoryImpl(conn);
                strRepos.StoreValues(EStringType.ApiResourceClaim, resource.Id, resource.Claims);
                strRepos.StoreValues(EStringType.ApiResourceScope, resource.Id, resource.Scopes);
                strRepos.StoreValues(EStringType.ApiResourceSecret, resource.Id, resource.Secrets);

                id = resource.Id;
            });

            return Task.FromResult(id);
        }

        public async Task<IList<MApiResource>> FindApiResourcesByNamesAsync(IList<string> names)
        {
            // the names must not be null or empty, and must contains at least one item
            if (names == null || names.Count == 0) throw new ArgumentNullException(nameof(names));

            using var conn = GetConnection();
            var repos = conn.GetRepository<MApiResource>();
            var list = await repos.Select.Where(p => p.Enabled == 1 && names.Contains(p.Name)).ToListAsync();

            var strRepos = new StringCollectionRepositoryImpl(conn);

            foreach (var item in list)
            {
                item.Claims = strRepos.GetValues(EStringType.ApiResourceClaim, item.Id);
                item.Scopes = strRepos.GetValues(EStringType.ApiResourceScope, item.Id);
                item.Secrets = strRepos.GetValues(EStringType.ApiResourceSecret, item.Id);
            }

            return list;
        }

        public async Task<IList<MApiResource>> FindApiResourcesByScopeNameAsync(IList<string> scopes)
        {
            // check input parameters
            if (scopes == null || scopes.Count == 0) throw new ArgumentNullException(nameof(scopes));

            using var conn = GetConnection();

            var list = await conn.Select<MApiResource>()
                    .Where(a => a.Enabled == 1 && conn.Select<MStringCollectionItem>()
                                    .Where(b => b.StringType == EStringType.ApiResourceScope && scopes.Contains(b.StringValue))
                                    .As("b").ToList(b => b.ParentId).Contains(a.Id)).ToListAsync();


            var strRepos = new StringCollectionRepositoryImpl(conn);
            foreach (var item in list)
            {
                item.Claims = strRepos.GetValues(EStringType.ApiResourceClaim, item.Id);
                item.Scopes = strRepos.GetValues(EStringType.ApiResourceScope, item.Id);
                item.Secrets = strRepos.GetValues(EStringType.ApiResourceSecret, item.Id);
            }

            return list;
        }

        public async Task<IList<MApiResource>> GetAllEnabledApiResourcesAsync()
        {
            using var conn = GetConnection();
            var repos = conn.GetRepository<MApiResource>();

            var query = await repos.Select.Where(p => p.Enabled == 1).ToListAsync();
            var list = query.ToList();
            var strRepos = new StringCollectionRepositoryImpl(conn);

            foreach (var item in list)
            {
                item.Claims = strRepos.GetValues(EStringType.ApiResourceClaim, item.Id);
                item.Scopes = strRepos.GetValues(EStringType.ApiResourceScope, item.Id);
                item.Secrets = strRepos.GetValues(EStringType.ApiResourceSecret, item.Id);
            }

            return list;
        }

        public async Task<MApiResource> GetApiResourceByIdAsync(long id)
        {
            // check the id must great than 0
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            using var conn = GetConnection();
            var respos = conn.GetRepository<MApiResource>();

            var item = await respos.Select.Where(p => p.Id == id).FirstAsync();

            if (item != null)
            {
                var strRepos = new StringCollectionRepositoryImpl(conn);
                item.Claims = strRepos.GetValues(EStringType.ApiResourceClaim, item.Id);
                item.Scopes = strRepos.GetValues(EStringType.ApiResourceScope, item.Id);
                item.Secrets = strRepos.GetValues(EStringType.ApiResourceSecret, item.Id);
            }

            return item;
        }

        public async Task<PagedResult<MApiResource>> QueryApiResourcesAsync(string keyword, BoolCondition enabled, int pageIndex, int pageSize)
        {
            using var conn = GetConnection();
            var repos = conn.GetRepository<MApiResource>();
            var query = repos.Select;
            // if keyword is not empty , then name and display should contain this keyword
            query = query.WhereIf(!string.IsNullOrWhiteSpace(keyword), p => p.Name.Contains(keyword) || p.DisplayName.Contains(keyword));
            query = query.WhereIf(enabled != BoolCondition.All, p => p.Enabled == (int)enabled);

            var total = await query.CountAsync();
            var list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            var strRepos = new StringCollectionRepositoryImpl(conn);

            foreach (var item in list)
            {
                item.Secrets = strRepos.GetValues(EStringType.ApiResourceSecret, item.Id);
                item.Scopes = strRepos.GetValues(EStringType.ApiResourceScope, item.Id);
                item.Claims = strRepos.GetValues(EStringType.ApiResourceClaim, item.Id);
            }

            return new PagedResult<MApiResource>
            {
                Items = list,
                TotalCount = total
            };
        }

        public Task RemoveApiResourceAsync(long id)
        {
            // check the id must great than 0
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            using var conn = GetConnection();
            conn.Transaction(() =>
            {
                var repos = GetConnection().GetRepository<MApiResource>();
                var resource = repos.Select.Where(p => p.Id == id).First();
                if (resource != null)
                {
                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    strRepos.RemoveValues(EStringType.ApiResourceClaim, resource.Id);
                    strRepos.RemoveValues(EStringType.ApiResourceScope, resource.Id);
                    strRepos.RemoveValues(EStringType.ApiResourceSecret, resource.Id);

                    repos.Delete(resource);
                }
            });

            return Task.CompletedTask;
        }

        public async Task<bool> SameNameExistedAsync(string name, long id)
        {
            // name must not be null or empty
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            using var conn = GetConnection();
            var repos = conn.GetRepository<MApiResource>();

            if (id > 0)
            {
                var existed = await repos.Select.Where(p => p.Name == name && p.Id != id).FirstAsync();
                return existed != null;
            }
            else
            {
                var existed = await repos.Select.Where(p => p.Name == name).FirstAsync();
                return existed != null;
            }
        }

        public Task UpdateApiResourceAsync(MApiResource resource)
        {
            // check input parameter
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            using var conn = GetConnection();

            conn.Transaction(() =>
            {
                var repos = conn.GetRepository<MApiResource>();
                var entity = repos.Select.Where(p => p.Id == resource.Id).First();
                if (entity != null)
                {
                    entity.Name = resource.Name;
                    entity.DisplayName = resource.DisplayName;
                    entity.Enabled = resource.Enabled;
                    entity.UpdateBy = resource.UpdateBy;
                    entity.UpdateTime = resource.UpdateTime;

                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    strRepos.StoreValues(EStringType.ApiResourceClaim, entity.Id, resource.Claims);
                    strRepos.StoreValues(EStringType.ApiResourceScope, entity.Id, resource.Scopes);
                    strRepos.StoreValues(EStringType.ApiResourceSecret, entity.Id, resource.Secrets);

                    repos.Update(entity);
                }
                else
                {
                    throw new BllException(404, $"Api resource with id {resource.Id} not found");
                }
            });

            return Task.CompletedTask;
        }
    }
}
