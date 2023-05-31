// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Stores;
using Laiye.SaasMp.WebApi.Integration;

namespace IdentityServer4.Storage.FreeSql.Storage
{
    internal class ResourceStoreImpl : IResourceStore
    {
        private const string CacheCategory = "ids:resource";

        private IDbApiResourceService ApiResourceService { get; }
        private IDbApiScopeService ApiScopeService { get; }
        private IDbIdentityResourceService IdentityResourceService { get; }
        private ICacheHelper CacheHelper { get; }

        public ResourceStoreImpl(ICacheHelper cacheHelper,
            IDbApiResourceService apiResourceService,
            IDbIdentityResourceService identityResourceService,
            IDbApiScopeService apiScopeResource)
        {
            CacheHelper = cacheHelper;
            ApiResourceService = apiResourceService;
            IdentityResourceService = identityResourceService;
            ApiScopeService = apiScopeResource;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var list = new List<ApiResource>();
            var names = new HashSet<string>(apiResourceNames);
            var all = await GetAllResourcesAsync();
            foreach (var res in all.ApiResources)
            {
                if (names.Contains(res.Name))
                {
                    list.Add(res);
                }
            }

            return list;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = new List<ApiResource>();
            var scopes = new HashSet<string>(scopeNames);
            var all = await GetAllResourcesAsync();
            foreach (var a in all.ApiResources)
            {
                if (a.Scopes.Any(x => scopeNames.Contains(x)))
                {
                    list.Add(a);
                }
            }

            return list;
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var list = new List<ApiScope>();

            // 这里要过滤 identity 以及 offline access
            var scopes = scopeNames.ToArray();

            var all = await GetAllResourcesAsync();
            foreach (var scope in all.ApiScopes)
            {
                if (scopes.Contains(scope.Name))
                {
                    list.Add(scope);
                }
            }

            return list;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var list = new List<IdentityResource>();

            var allResources = await GetAllResourcesAsync();
            var identityResources = allResources.IdentityResources;
            var scopes = new HashSet<string>(scopeNames);
            foreach (var i in identityResources)
            {
                if (scopes.Contains(i.Name))
                {
                    list.Add(i);
                }
            }

            return list;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var resources = await CacheHelper.GetFromCacheAsync(CacheCategory,
                "all",
                TimeSpan.FromSeconds(300),
                async () =>
                {
                    var apiScopes = await this.ApiScopeService.GetAllApiScopesAsync();
                    var apiResources = await this.ApiResourceService.GetAllEnabledApiResources();
                    var identityResources = await this.IdentityResourceService.GetAllIdentityResourcesAsync();

                    var resources = new Resources()
                    {
                        OfflineAccess = true
                    };

                    foreach (var scope in apiScopes)
                    {
                        ApiScope obj = GetApiScopeByInfo(scope);
                        resources.ApiScopes.Add(obj);
                    }

                    foreach (var idr in identityResources)
                    {
                        IdentityResource obj = GetIdentityResourceByInfo(idr);

                        resources.IdentityResources.Add(obj);
                    }

                    foreach (var api in apiResources)
                    {
                        ApiResource obj = GetApiResourceByInfo(api);

                        resources.ApiResources.Add(obj);
                    }

                    return resources;
                }, true);

            return resources;
        }

        private static ApiResource GetApiResourceByInfo(ApiResourceDto api)
        {
            return new ApiResource
            {
                Name = api.Name,
                DisplayName = api.DisplayName,
                UserClaims = api.Claims,
                Scopes = api.Scopes,
                // new Secret("laiye".Sha256())
                ApiSecrets = api.Secrets.Select(p => new Secret(p.Sha256())).ToList()
            };
        }

        private static IdentityResource GetIdentityResourceByInfo(IdentityResourceDto idr)
        {
            return new IdentityResource
            {
                Name = idr.Name,
                DisplayName = idr.DisplayName,
                Emphasize = idr.Emphasize == 1,
                Required = idr.Required == 1,
                UserClaims = idr.Claims
            };
        }

        private static ApiScope GetApiScopeByInfo(ApiScopeDto scope)
        {
            return new ApiScope
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Emphasize = scope.Emphasize == 1,
                Required = scope.Required == 1,
                UserClaims = scope.Claims
            };
        }
    }
}
