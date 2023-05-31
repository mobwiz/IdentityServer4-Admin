// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Services;
using IdentityServer4.Storage.FreeSql.Services;
using Laiye.SaasMp.WebApi.Integration;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Storage.FreeSql.Storage
{
    internal class CorsPolicyServiceImpl : ICorsPolicyService
    {
        private IDbClientService clientService;
        private ILogger<CorsPolicyServiceImpl> logger;

        private ICacheHelper CacheHelper { get; }

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="logger"></param>
        /// <param name="cacheHelper"></param>
        public CorsPolicyServiceImpl(IDbClientService clientService,
            ILogger<CorsPolicyServiceImpl> logger,
            ICacheHelper cacheHelper)
        {
            this.clientService = clientService;
            this.logger = logger;
            CacheHelper = cacheHelper;
        }

        /// <summary>
        /// Check if the origin is valid
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var list = await CacheHelper.GetFromCacheAsync("ids:corsOrigins", "all", TimeSpan.FromMinutes(5),
                async () =>
                    {
                        var list = (await clientService.GetCorsOriginsAsync())?.ToList() ?? new List<string>();
                        return list;
                    }, true);

            var valid = list.Contains(origin, StringComparer.OrdinalIgnoreCase);

            if (valid)
            {
                logger.LogDebug("Client list checked and origin: {0} is allowed", origin);
            }
            else
            {
                logger.LogDebug("Client list checked and origin: {0} is not allowed", origin);
            }

            return valid;
        }
    }
}
