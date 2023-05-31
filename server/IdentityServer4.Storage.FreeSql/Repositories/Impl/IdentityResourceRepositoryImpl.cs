// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Types;
using Mobwiz.Common.BaseTypes;
using Mobwiz.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    internal class IdentityResourceRepositoryImpl : InnerBaseRepository, IIdentityResourceRepository
    {
        public IdentityResourceRepositoryImpl(DatabaseConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        public Task<long> CreateIdentityResourceAsync(MIdentityResource identityResource)
        {
            // check input
            if (identityResource == null) throw new ArgumentNullException(nameof(identityResource));

            var id = 0L;
            using (var conn = GetConnection())
            {
                conn.Transaction(() =>
                {
                    var repos = conn.GetRepository<MIdentityResource>();
                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    if (!repos.Select.Any(item => item.Name == identityResource.Name))
                    {
                        repos.Insert(identityResource);
                        if (identityResource.Claims.Any())
                        {
                            strRepos.StoreValues(EStringType.IdentityResourceClaim, identityResource.Id, identityResource.Claims);
                        }
                    }
                    id = identityResource.Id;
                });
            }
            return Task.FromResult(id);
        }

        public async Task<IList<MIdentityResource>> FindIdentityResourcesByScopeNameAsync(IList<string> names)
        {
            // check input
            if (names == null || names.Count == 0) throw new ArgumentNullException(nameof(names));

            using (var conn = GetConnection())
            {
                var list = await conn.Select<MIdentityResource>()
                    .Where(p => names.Contains(p.Name) && p.Enabled == 1)
                    .ToListAsync();

                var strRepos = new StringCollectionRepositoryImpl(conn);

                foreach (var idr in list)
                {
                    idr.Claims = strRepos.GetValues(EStringType.IdentityResourceClaim, idr.Id);
                }

                return list;
            }
        }

        public async Task<IList<MIdentityResource>> GetAllEnabledIdentityResourcesAsync()
        {
            using (var conn = GetConnection())
            {
                var list = await conn.GetRepository<MIdentityResource>().Select.Where(p => p.Enabled == 1).ToListAsync();
                var strRepos = new StringCollectionRepositoryImpl(conn);
                foreach (var item in list)
                {
                    item.Claims = strRepos.GetValues(EStringType.IdentityResourceClaim, item.Id);
                }

                return list;
            }
        }

        public async Task<MIdentityResource> GetIdentityResourceByIdAsync(long id)
        {
            // the id must be greater thant 0
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            using (var conn = GetConnection())
            {
                var obj = await conn.GetRepository<MIdentityResource>().Select.Where(p => p.Id == id).FirstAsync();
                if (obj != null)
                {
                    var strRepos = new StringCollectionRepositoryImpl(conn);
                    obj.Claims = strRepos.GetValues(EStringType.IdentityResourceClaim, obj.Id);
                }

                return obj;
            }
        }

        public Task<bool> InitData()
        {
            using (var conn = GetConnection())
            {
                conn.Transaction(() =>
                {
                    var repos = conn.GetRepository<MIdentityResource>();
                    var stringCollectionRepos = new StringCollectionRepositoryImpl(conn);
                    var openidRes = repos.Select.Where(p => p.Name == "openid").First();
                    if (openidRes == null)
                    {
                        var resource = new MIdentityResource
                        {
                            Enabled = 1,
                            Emphasize = 1,
                            Name = "openid",
                            DisplayName = "Your user identifier.",
                            Claims = new string[] { "sub" },
                            IsPreset = 1,
                            CreatedBy = "system",
                            CreateTime = DateTime.Now,
                        };
                        repos.Insert(resource);
                        stringCollectionRepos.StoreValues(EStringType.IdentityResourceClaim, resource.Id, resource.Claims);
                    }

                    var profileRes = repos.Select.Where(p => p.Name == "profile").First();
                    if (profileRes == null)
                    {
                        var resource = new MIdentityResource
                        {
                            Enabled = 1,
                            Emphasize = 1,
                            Name = "profile",
                            DisplayName = "Your user profile.",
                            Claims = new string[] {
                                "name",
                                "given_name",
                                "middle_name",
                                "preferred_username",
                                "profile",
                                "picture",
                                "website",
                                "email",     /** End-User's preferred e-mail address */
                                "email_verified", /** True if the End-User's e-mail address has been verified; otherwise false. */
                                "gender",
                                "birthdate",   /** End-User's birthday, represented as an ISO 8601:2004 [ISO8601‑2004] YYYY-MM-DD format */
                                "zoneinfo",   /** String from zoneinfo [zoneinfo] time zone database representing the End-User's time zone. */
                                "locale", /** End-User's locale, represented as a BCP47 [RFC5646] language tag. */
                                "phone_number",
                                "phone_number_verified",
                                "address", // End-User's preferred address in JSON [RFC4627] */
                                "updated_at",
                            },
                            IsPreset = 1,
                            CreatedBy = "system",
                            CreateTime = DateTime.Now,
                        };
                        repos.Insert(resource);
                        stringCollectionRepos.StoreValues(EStringType.IdentityResourceClaim, resource.Id, resource.Claims);
                    }
                });
            }

            return Task.FromResult(true);
        }

        public async Task<PagedResult<MIdentityResource>> QueryIdentityResourcesAsync(string keyword, BoolCondition enabled, int pageIndex, int pageSize)
        {
            // the pageindex and pagesize must be greater than  0
            if (pageIndex <= 0) throw new ArgumentNullException(nameof(pageIndex));
            if (pageSize <= 0) throw new ArgumentNullException(nameof(pageSize));

            using var conn = GetConnection();

            var query = conn.GetRepository<MIdentityResource>().Select;
            query = query.WhereIf(!string.IsNullOrEmpty(keyword), p => p.Name.Contains(keyword) || p.DisplayName.Contains(keyword));
            query = query.WhereIf(enabled != BoolCondition.All, p => p.Enabled == (int)enabled);

            var total = await query.CountAsync();
            var list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            var strRepos = new StringCollectionRepositoryImpl(conn);
            foreach (var item in list)
            {
                item.Claims = strRepos.GetValues(EStringType.IdentityResourceClaim, item.Id);
            }

            return new PagedResult<MIdentityResource>(total, list);

        }

        public Task RemoveIdentityResourceAsync(long id)
        {
            // the id must be greater than 0
            if (id <= 0) throw new ArgumentNullException(nameof(id));

            using (var conn = GetConnection())
            {
                conn.Transaction(() =>
                {
                    var identityResourceRepos = conn.GetRepository<MIdentityResource>();
                    var strRepos = new StringCollectionRepositoryImpl(conn);

                    var resource = identityResourceRepos.Select.Where(p => p.Id == id).First();

                    if (resource != null)
                    {
                        if (resource.IsPreset == 1)
                        {
                            throw new BllException(500, "Preset identity resource can't be removed");
                        }
                        strRepos.RemoveValues(EStringType.IdentityResourceClaim, resource.Id);
                    }

                    identityResourceRepos.Delete(resource);
                });
            }

            return Task.CompletedTask;
        }

        public async Task<bool> SameNameExistedAsync(string name, long id)
        {
            // the name must not be null
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var conn = GetConnection();
            var repos = conn.GetRepository<MIdentityResource>();

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

        public Task UpdateIdentityResourceAsync(MIdentityResource identityResource)
        {
            // check input
            if (identityResource == null) throw new ArgumentNullException(nameof(identityResource));

            using (var conn = GetConnection())
            {
                conn.Transaction(() =>
                {
                    var repos = conn.GetRepository<MIdentityResource>();

                    var entity = repos.Select.Where(p => p.Id == identityResource.Id).First();

                    if (entity != null)
                    {
                        if (entity.IsPreset == 1)
                        {
                            throw new BllException(500, "Preset identity resource can't be updated");
                        }

                        var strRepos = new StringCollectionRepositoryImpl(conn);
                        entity.Name = identityResource.Name;
                        entity.DisplayName = identityResource.DisplayName;
                        entity.Required = identityResource.Required;
                        entity.Emphasize = identityResource.Emphasize;
                        entity.Enabled = identityResource.Enabled;
                        repos.Update(entity);

                        strRepos.StoreValues(EStringType.IdentityResourceClaim, entity.Id, identityResource.Claims);
                    }
                });
            }
            return Task.CompletedTask;
        }
    }
}
